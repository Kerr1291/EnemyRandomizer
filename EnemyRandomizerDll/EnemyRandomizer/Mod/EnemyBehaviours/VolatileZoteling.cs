using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class VolatileZoteling : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _control.GetAction<RandomFloat>("Set Pos", 4).min = transform.position.x;
            _control.GetAction<RandomFloat>("Set Pos", 4).max = transform.position.x;
            _control.GetAction<RandomFloat>("Set Pos", 5).min = transform.position.y;
            _control.GetAction<RandomFloat>("Set Pos", 5).max = transform.position.y;
            _control.GetAction<WaitRandom>("Spawn Pause").timeMin = 0.0f;
            _control.GetAction<WaitRandom>("Spawn Pause").timeMax = 0.0f;
            _control.GetState("Reset").RemoveAction<SetHP>();
            _control.GetState("Reset").RemoveAction<SetIsDead>();
            _control.GetState("Reset").InsertMethod(0, () => Destroy(gameObject));

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            _control.SendEvent("BALLOON SPAWN");

        }
    }
}
