using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class GreyPrinceControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
        {

        };

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected override IEnumerator Start()
        {
            return base.Start();
        }
    }

    public class GreyPrinceSpawner : DefaultSpawner<GreyPrinceControl>
    {
    }

    public class GreyPrincePrefabConfig : DefaultPrefabConfig<GreyPrinceControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");
                //remove the transitions related to chain spawning zotes for the event
                fsm.RemoveTransition("Dormant", "ZOTE APPEAR");
                fsm.RemoveTransition("Dormant", "GG BOSS");
                fsm.RemoveTransition("GG Pause", "FINISHED");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Level 1", "FINISHED", "Enter 1");
                fsm.ChangeTransition("Level 2", "FINISHED", "Enter 1");
                fsm.ChangeTransition("Level 3", "FINISHED", "Enter 1");
                fsm.ChangeTransition("4+", "FINISHED", "Enter 1");

                fsm.ChangeTransition("Enter 3", "FINISHED", "Roar End");

                //remove the states that were also part of that
                //fsm.Fsm.RemoveState("Dormant");
                //fsm.Fsm.RemoveState("GG Pause");
            }
        }
    }




















}
