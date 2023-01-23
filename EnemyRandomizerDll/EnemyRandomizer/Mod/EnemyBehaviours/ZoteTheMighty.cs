using HutongGames.PlayMaker.Actions;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZoteTheMighty : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _control.GetAction<RandomFloat>("Spawn Antic").min = transform.position.x;
            _control.GetAction<RandomFloat>("Spawn Antic").max = transform.position.x;
            _control.GetAction<SetPosition>("Spawn Antic").y = transform.position.y;
            _control.GetAction<SetPosition>("Spawn Antic").y = transform.position.y;

            FsmState deathState = _control.GetState("Death");
            deathState.RemoveTransition("FINISHED");
            deathState.AddMethod(() =>
            {
                Destroy(gameObject);
            });

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("SPAWN");
        }
    }
}