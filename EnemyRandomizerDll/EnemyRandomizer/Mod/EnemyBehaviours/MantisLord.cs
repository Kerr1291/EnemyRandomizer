using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class MantisLord : EnemyBehaviour
    {
        private PlayMakerFSM _lord;

        private void Awake()
        {
            _lord = gameObject.LocateMyFSM("Mantis Lord");
        }

        private void Start()
        {
            _lord.SetState("Init");

            //_lord.Fsm.GetFsmBool("Sub").Value = true;
            
            _lord.Fsm.GetFsmFloat("Dash Hero L").Value = bounds.center.x - 0.1f;
            _lord.Fsm.GetFsmFloat("Dash Hero R").Value = bounds.center.x + 0.1f;
            _lord.Fsm.GetFsmFloat("Dash X L").Value = bounds.center.x - 8;
            _lord.Fsm.GetFsmFloat("Dash X R").Value = bounds.center.x + 8;
            _lord.Fsm.GetFsmFloat("Dash Y").Value = bounds.yMin + 2.5f;
            _lord.Fsm.GetFsmFloat("Dstab X Max").Value = bounds.xMax;
            _lord.Fsm.GetFsmFloat("Dstab X Min").Value = bounds.xMin;
            _lord.Fsm.GetFsmFloat("Land Y").Value = bounds.yMin + 1.75f;
            _lord.Fsm.GetFsmFloat("Throw Hero L").Value = bounds.center.x - 8;
            _lord.Fsm.GetFsmFloat("Throw Hero R").Value = bounds.center.x + 8;
            _lord.Fsm.GetFsmFloat("Wall X L").Value = bounds.xMin + 1;
            _lord.Fsm.GetFsmFloat("Wall X R").Value = bounds.xMax - 1;
            _lord.Fsm.GetFsmFloat("Wall Y Max").Value = bounds.yMax - 1;
            _lord.Fsm.GetFsmFloat("Wall Y Min").Value = bounds.yMin + 1;
            
            _lord.GetAction<FloatCompare>("Attack Choice", 2).float2.Value = bounds.yMin + 3;
            _lord.GetAction<FloatCompare>("Attack Choice", 3).float2.Value = bounds.xMin;
            _lord.GetAction<FloatCompare>("Attack Choice", 4).float2.Value = bounds.xMax;
            _lord.GetAction<FloatClamp>("Attack Choice").minValue.Value = bounds.yMin;
            _lord.GetAction<FloatClamp>("Attack Choice").maxValue.Value = bounds.yMax - 4;
        }
    }
}