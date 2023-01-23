using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class FailedChampion : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("FalseyControl");
        }

        private IEnumerator Start()
        {
            _control.SetState("Init");

            _control.Fsm.GetFsmFloat("Rage Point X").Value = bounds.center.x;
            _control.Fsm.GetFsmFloat("Range Max").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("Range Min").Value = bounds.xMin; 
                    
            yield return new WaitWhile(() => _control.ActiveStateName != "Dormant");

            _control.SendEvent("BATTLE START");
        }
    }
}