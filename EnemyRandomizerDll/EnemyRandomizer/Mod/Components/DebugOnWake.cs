using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{
    //TODO: move the wake components into their own classes....
    public class DebugOnWake : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject owner;

        List<GameObject> lines;

        //the fsm to aim our events at
        public string fsmName;

        //the events to send the fsm
        public List<string> wakeEvents = new List<string>();

        //send the wake events every time we're in this state for the fsmName given
        public string sendWakeEventsOnState;

        //Dictionary = FSM and the current state it's in
        public Dictionary<PlayMakerFSM, string> fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

        //run logic on the FSMs every frame?
        public bool monitorFSMStates = false;

        //print debug bounding boxes and debug log info?
        public bool logFSM = true;

        private void OnDisable()
        {
            if (monitorFSMStates)
            {
                if (logFSM)
                    Dev.Log("DebugFSMS DebugOnWake was disabled, likely because the enemy died ");

                //final FSM info....
                foreach (var p in owner.GetComponentsInChildren<PlayMakerFSM>())
                {
                    if (p == null)
                        continue;

                    if (!fsmsOnObject.ContainsKey(p))
                    {
                        fsmsOnObject.Add(p, p.ActiveStateName);
                        if (logFSM)
                            Dev.Log("DebugFSMS :::: Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]");
                    }
                    else if (p.ActiveStateName != fsmsOnObject[p])
                    {
                        if (logFSM)
                            Dev.Log("DebugFSMS :::: " + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[p] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ((p.Fsm != null && p.Fsm.LastTransition != null) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE") + "]");
                        fsmsOnObject[p] = p.ActiveStateName;
                    }
                }
            }
        }

        IEnumerator DebugFSMS()
        {
            fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

            foreach (var p in owner.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                fsmsOnObject.Add(p, p.ActiveStateName);
                if (logFSM)
                    Dev.Log("Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]");
            }

            Dev.Where();

            while (monitorFSMStates)
            {
                if (owner == null)
                    yield break;

                foreach (var p in owner.GetComponentsInChildren<PlayMakerFSM>(true))
                {
                    if (p == null)
                        continue;

                    if (!fsmsOnObject.ContainsKey(p))
                    {
                        fsmsOnObject.Add(p, p.ActiveStateName);
                        if (logFSM)
                            Dev.Log("Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]");
                    }
                    else if (p.ActiveStateName != fsmsOnObject[p])
                    {
                        if (logFSM)
                            Dev.Log("" + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[p] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ((p.Fsm != null && p.Fsm.LastTransition != null) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE") + "]");
                        fsmsOnObject[p] = p.ActiveStateName;
                    }

                    //force-send an event on this state if everything matches?
                    if (!string.IsNullOrEmpty(sendWakeEventsOnState) && fsmName == p.FsmName && sendWakeEventsOnState == p.ActiveStateName)
                    {
                        if (p != null && wakeEvents != null)
                        {
                            foreach (string s in wakeEvents)
                            {
                                p.SendEvent(s);
                            }
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Start()
        {
            Dev.Where();
            while (owner == null)
            {
                yield return null;
            }

            if (logFSM)
            {
                if (collider != null)
                {
                    lines = new List<GameObject>();
                    lines = collider.bounds.CreateBoxOfLineRenderers(Color.green, -2.1f, .01f);
                    foreach (var go in lines)
                    {
                        go.transform.SetParent(owner.transform);
                        go.transform.localPosition = Vector3.zero;
                    }
                }
            }

            if (monitorFSMStates)
            {
                StartCoroutine(DebugFSMS());
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (monitorFSMStates)
                return;

            bool isPlayer = false;

            foreach (Transform t in collision.gameObject.GetComponentsInParent<Transform>())
            {
                if (t.gameObject == HeroController.instance.gameObject)
                {
                    isPlayer = true;
                    break;
                }
            }

            if (!isPlayer)
            {
                Dev.Log("Something not the player entered us!");
                return;
            }

            Dev.Log("Player entered our wake area! ");

            if (!string.IsNullOrEmpty(fsmName))
            {
                PlayMakerFSM fsm = FSMUtility.LocateFSM(owner, fsmName);

                if (fsm != null && wakeEvents != null)
                {
                    foreach (string s in wakeEvents)
                    {
                        Dev.Log("Sending event! " + s);
                        fsm.SendEvent(s);
                    }
                }
                else
                {
                    Dev.Log("Could not find FSM!");
                }
            }

            //remove this after waking up the enemy
            Destroy(gameObject);
        }


        public static DebugOnWake AddDebugOnWake(GameObject enemy, string fsmName = "", string wakeState = "", List<string> wakeEvents = null, bool checkFSMEveryFrame = true, Vector2? customColliderSize = null, bool logFSMTransitions = false)
        {
            GameObject wakeUpRoot = new GameObject(enemy.name + " DebugWake Object");
            wakeUpRoot.transform.SetParent(enemy.transform);
            wakeUpRoot.transform.localPosition = Vector3.zero;

            wakeUpRoot.layer = 13; //try this
            wakeUpRoot.tag = enemy.tag;

            BoxCollider2D box = wakeUpRoot.AddComponent<BoxCollider2D>();
            box.isTrigger = true;

            if (!customColliderSize.HasValue)
            {
                BoxCollider2D pbox = enemy.GetComponent<BoxCollider2D>();
                if (pbox != null)
                {
                    box.size = pbox.size;
                    box.offset = pbox.offset;
                }
            }
            else
            {
                box.size = customColliderSize.Value;
            }

            DebugOnWake specialWakeUp = wakeUpRoot.AddComponent<DebugOnWake>();
            specialWakeUp.collider = box;
            specialWakeUp.owner = enemy;
            specialWakeUp.fsmName = fsmName;
            specialWakeUp.wakeEvents = wakeEvents;

            specialWakeUp.monitorFSMStates = checkFSMEveryFrame;
            specialWakeUp.sendWakeEventsOnState = wakeState;
            specialWakeUp.logFSM = logFSMTransitions;

            return specialWakeUp;
        }

    }
}
