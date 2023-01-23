using System;
using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class SoulWarrior : EnemyBehaviour
    {
        private PlayMakerFSM _knight;

        private void Awake()
        {
            _knight = gameObject.LocateMyFSM("Mage Knight");
        }

        private IEnumerator Start()
        {
            _knight.Fsm.GetFsmFloat("Tele X Max").Value = bounds.xMax;
            _knight.Fsm.GetFsmFloat("Tele X Min").Value = bounds.xMin;
            
            _knight.SetState("Init");
            
            yield return new WaitWhile(() => _knight.ActiveStateName != "Sleep");

            _knight.SendEvent("WAKE");
            
            yield return new WaitWhile(() => _knight.ActiveStateName != "Wake");
            
            _knight.SetState("Idle");
        }
    }
}