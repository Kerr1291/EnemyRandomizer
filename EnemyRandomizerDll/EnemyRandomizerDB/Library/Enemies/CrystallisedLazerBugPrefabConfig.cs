using UnityEngine;

namespace EnemyRandomizerMod
{
    public class CrystallisedLazerBugControl : DefaultSpawnedEnemyControl
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

    public class CrystallisedLazerBugSpawner : DefaultSpawner<CrystallisedLazerBugControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<CrystallisedLazerBugControl>();
            //fsm.control = go.LocateMyFSM("Chase");

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

    public class CrystallisedLazerBugPrefabConfig : DefaultPrefabConfig<CrystallisedLazerBugControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                //var fsm = p.prefab.LocateMyFSM("Chase");
                var hm = p.prefab.GetComponent<HealthManager>();
                hm.IsInvincible = false;
            }
        }
    }
}
