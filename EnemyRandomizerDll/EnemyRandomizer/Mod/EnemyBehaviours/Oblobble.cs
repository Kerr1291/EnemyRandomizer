using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class Oblobble : EnemyBehaviour
    {
        private PlayMakerFSM _attack;
        private PlayMakerFSM _bounce;

        private void Awake()
        {
            _attack = gameObject.LocateMyFSM("Fatty Fly Attack");
            _bounce = gameObject.LocateMyFSM("fat fly bounce");
        }    
        
        private IEnumerator Start()
        {
            _bounce.GetAction<SetPosition>("Swoop In").z = 0.0f;
            _bounce.GetAction<Translate>("Swoop In").y = -15.0f;
            _bounce.GetAction<iTweenMoveBy>("Swoop In").vector = Vector3.zero;
            _bounce.GetAction<iTweenMoveBy>("Swoop In").time = 0.0f;

            _attack.SetState("Init");
            _bounce.SetState("Initialise");

            yield return new WaitUntil(() => _bounce.ActiveStateName == "Swoop In");
            _bounce.SendEvent("SUMMON");
        }
    }
}