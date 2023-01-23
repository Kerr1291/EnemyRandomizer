using HutongGames.PlayMaker.Actions;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZoteThwomp : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }
        private IEnumerator Start()
        {
            _control.GetAction<RandomFloat>("Set Pos").min = transform.position.x;
            _control.GetAction<RandomFloat>("Set Pos").max = transform.position.x;
            _control.GetAction<SetPosition>("Set Pos").y = transform.position.y;
            _control.GetAction<SetPosition>("Set Pos").y = transform.position.y;

            _control.GetState("Break").InsertMethod(0, () =>
            {
                Destroy(gameObject);
            });
            _control.GetState("Break").RemoveTransition("FINISHED");

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("GO");
        }

        private void Update()
        {
            if (_control.ActiveStateName == "Wait")
            {
                _control.SendEvent("GO");
            }
        }
    }
}