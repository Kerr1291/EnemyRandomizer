using UnityEngine;

namespace EnemyRandomizerMod
{

    public class ZombieSpider1Control : DefaultSpawnedEnemyControl
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

    public class ZombieSpider1Spawner : DefaultSpawner<ZombieSpider1Control>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZombieSpider1Control>();
            fsm.control = go.LocateMyFSM("Chase");

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

    public class ZombieSpider1PrefabConfig : DefaultPrefabConfig<ZombieSpider1Control>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Chase");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Check Battle", "BATTLE", "Wait 2");

                fsm.RemoveTransition("Battle Inert", "BATTLE START");
                //fsm.Fsm.RemoveState("Battle Inert");
            }
        }
    }












}
