using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class VengeflyKing : EnemyBehaviour
    {
        private PlayMakerFSM _buzzer;

        private void Awake()
        {
            _buzzer = gameObject.LocateMyFSM("Big Buzzer");
        }

        private IEnumerator Start()
        {
            _buzzer.GetAction<SetPosition>("Swoop In").z = 0.0f;
            _buzzer.GetAction<Translate>("Swoop In").y = -15.0f;
            _buzzer.GetAction<iTweenMoveBy>("Swoop In").vector = Vector3.zero;
            _buzzer.GetAction<iTweenMoveBy>("Swoop In").time = 0.0f;

            _buzzer.SetState("Init");

            yield return new WaitUntil(() => _buzzer.ActiveStateName == "Swoop In");

            _buzzer.SendEvent("SUMMON");

        }
    }
}
