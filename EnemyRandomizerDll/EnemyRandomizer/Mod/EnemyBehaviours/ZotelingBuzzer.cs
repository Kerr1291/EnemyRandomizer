using HutongGames.PlayMaker.Actions;
using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class ZotelingBuzzer : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }
        private void Start()
        {
            _control.GetState("Reset").RemoveAction<SetHP>();
            _control.GetState("Reset").InsertMethod(0, () => Destroy(gameObject));

            //_control.SetState("Choice");
            _control.SetState("Buzzer Start");
        }
    }
}