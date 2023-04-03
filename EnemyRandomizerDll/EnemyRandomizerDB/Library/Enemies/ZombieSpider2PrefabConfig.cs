using UnityEngine;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{



    public class ZombieSpider2Control : DefaultSpawnedEnemyControl
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

    public class ZombieSpider2Spawner : DefaultSpawner<ZombieSpider2Control>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZombieSpider2Control>();
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
    public class ZombieSpider2PrefabConfig : DefaultPrefabConfig<ZombieSpider2Control>
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
