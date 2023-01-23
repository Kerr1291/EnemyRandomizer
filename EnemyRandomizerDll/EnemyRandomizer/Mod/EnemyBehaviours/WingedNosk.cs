using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class WingedNosk : EnemyBehaviour
    {
        private PlayMakerFSM _nosk;

        private void Awake()
        {
            _nosk = gameObject.LocateMyFSM("Hornet Nosk");
        }

        private IEnumerator Start()
        {
            _nosk.Fsm.GetFsmFloat("X Max").Value = bounds.xMax;
            _nosk.Fsm.GetFsmFloat("X Min").Value = bounds.xMin;
            _nosk.Fsm.GetFsmFloat("Y Max").Value = bounds.yMax;
            _nosk.Fsm.GetFsmFloat("Y Min").Value = bounds.yMin;
            _nosk.Fsm.GetFsmFloat("Swoop Height").Value = bounds.center.y;
            
            _nosk.GetAction<FloatCompare>("Swoop L").float2 = bounds.xMax;
            _nosk.GetAction<FloatCompare>("Swoop R").float2 = bounds.xMin;
            _nosk.GetAction<FloatCompare>("Shift Down?").float2 = bounds.center.y;
            _nosk.GetAction<SetPosition>("Roof Impact").y = bounds.yMax + 2;
            _nosk.GetAction<SetPosition>("Roof Return").y = bounds.center.y;
            
            _nosk.SetState("Init");

            yield return new WaitUntil(() => _nosk.ActiveStateName == "Dormant");

            _nosk.SetState("Idle");
        }
    }
}