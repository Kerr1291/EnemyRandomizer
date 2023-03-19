using UnityEngine;

namespace EnemyRandomizerMod
{


    public class GreyPrinceControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }
    }

    public class GreyPrinceSpawner : DefaultSpawner<GreyPrinceControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<GreyPrinceControl>();
            fsm.control = go.LocateMyFSM("Control");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
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

                //remove the states that were also part of that
                //fsm.Fsm.RemoveState("Dormant");
                //fsm.Fsm.RemoveState("GG Pause");
            }
        }
    }




















}
