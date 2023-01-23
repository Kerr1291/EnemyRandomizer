using HutongGames.PlayMaker.Actions;
using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZoteSalubra : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }
        private IEnumerator Start()
        {
            _control.GetAction<RandomFloat>("Appear").min = transform.position.x;
            _control.GetAction<RandomFloat>("Appear").max = transform.position.x;
            _control.GetAction<SetPosition>("Appear").y = transform.position.y;
            _control.GetAction<SetPosition>("Appear").y = transform.position.y;
            
            _control.GetState("Death").RemoveAction<SetIsDead>();
            _control.GetState("Death").RemoveAction<SetHP>();
            _control.GetState("Death").RemoveTransition("FINISHED");
            _control.GetState("Death").InsertMethod(0, () =>
            {
                Destroy(gameObject);
            });

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("START");
        }
    }
}
