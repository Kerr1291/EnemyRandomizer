using System;
using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Uumuu : EnemyBehaviour
    {
        private PlayMakerFSM _bounds;
        private PlayMakerFSM _jellyfish;

        private void Awake()
        {
            _bounds = gameObject.LocateMyFSM("Bounds");
            _jellyfish = gameObject.LocateMyFSM("Mega Jellyfish");
        }

        private IEnumerator Start()
        {
            _bounds.Fsm.GetFsmFloat("X Max").Value = bounds.xMax;
            _bounds.Fsm.GetFsmFloat("X Min").Value = bounds.xMin;
            _bounds.Fsm.GetFsmFloat("Y Max").Value = bounds.yMax;
            _bounds.Fsm.GetFsmFloat("Y Min").Value = bounds.yMin;

            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            var constrainPosition = GetComponent<ConstrainPosition>();
            constrainPosition.xMax = bounds.xMax;
            constrainPosition.xMin = bounds.xMin;
            constrainPosition.yMax = bounds.yMax;
            constrainPosition.yMin = bounds.yMin;

            _jellyfish.SetState("Init");

            yield return new WaitUntil(() => _jellyfish.ActiveStateName == "Sleep");
            
            _jellyfish.SetState("Start");
        }
    }
}