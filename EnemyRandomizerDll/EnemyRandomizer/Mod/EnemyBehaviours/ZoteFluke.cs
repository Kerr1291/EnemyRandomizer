using HutongGames.PlayMaker.Actions;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZoteFluke : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }
        private IEnumerator Start()
        {
            _control.GetAction<RandomFloat>("Pos").min = transform.position.x;
            _control.GetAction<RandomFloat>("Pos").max = transform.position.x;
            _control.GetAction<SetPosition>("Pos").y = transform.position.y;
            _control.GetAction<SetPosition>("Pos").y = transform.position.y;

            FsmState deathState = _control.GetState("Death");
            deathState.RemoveAction<SetIsDead>();
            deathState.RemoveAction<SetHP>();
            deathState.RemoveTransition("FINISHED");
            deathState.AddMethod(() =>
            {
                Destroy(gameObject);
            });

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("GO");
        }

        private void Update()
        {
            if (_control.ActiveStateName == "Sleeping")
            {
                _control.SendEvent("GO");
            }
        }
    }
}