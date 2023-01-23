using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class MassiveMossCharger : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Mossy Control");
        }

        private IEnumerator Start()
        {
            _control.GetAction<FloatCompare>("In Air").float2 = bounds.yMin + 2;
            
            _control.GetState("Music 2").RemoveAction<TransitionToAudioSnapshot>();

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Sleep");

            _control.Fsm.GetFsmFloat("X Max").Value = bounds.xMax - 4;
            _control.Fsm.GetFsmFloat("X Min").Value = bounds.xMin + 4;
            _control.Fsm.GetFsmFloat("Start Y").Value = bounds.yMin + 2;
            
            _control.SetState("Submerge 1");
        }
    }
}