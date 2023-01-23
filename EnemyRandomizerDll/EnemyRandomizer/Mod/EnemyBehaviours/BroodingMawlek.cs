using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class BroodingMawlek : EnemyBehaviour
    {
        private PlayMakerFSM _control;
        
        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Mawlek Control");
        }

        private IEnumerator Start()
        {
            _control.SetState("Init");

            _control.GetState("Wake Land").AddMethod(() => _control.SetState("Start"));
            
            yield return new WaitWhile(() => _control.ActiveStateName != "Dormant");
            
            _control.SendEvent("WAKE");
        }

        private void Update()
        {
            Modding.Logger.Log("[Mawlek] " + _control.ActiveStateName);
        }
    }
}