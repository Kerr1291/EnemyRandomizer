using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod
{

    public class MawlekBodyControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }

        private IEnumerator Start()
        {
            control.SetState("Init");

            control.GetState("Wake Land").AddCustomAction(() => control.SetState("Start"));

            yield return new WaitWhile(() => control.ActiveStateName != "Dormant");

            control.SendEvent("WAKE");
        }
    }

    public class MawlekBodySpawner : DefaultSpawner<MawlekBodyControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<MawlekBodyControl>();
            fsm.control = go.LocateMyFSM("Mawlek Control");

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
    public class MawlekBodyPrefabConfig : DefaultPrefabConfig<MawlekBodyControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Mawlek Control");
            }
        }
    }
}
