using HutongGames.PlayMaker.Actions;
using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZoteTurret : EnemyBehaviour
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

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("GO");
        }
    }
}
