//using System.Collections;
//using UnityEngine;

//namespace EnemyRandomizerMod
//{


//    public class TEMPLATE_Control : DefaultSpawnedEnemyControl
//    {
//        public PlayMakerFSM control;

//        public override void Setup(ObjectMetadata other)
//        {
//            base.Setup(other);
//        }

//        protected virtual void OnEnable()
//        {
//        }
//    }

//    public class TEMPLATE_Spawner : DefaultSpawner<TEMPLATE_Control>
//    {
//        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
//        {
//            var go = base.Spawn(p, source);
//            var fsm = go.GetComponent<TEMPLATE_Control>();
//            fsm.control = go.LocateMyFSM("Control");

//            if (source.IsBoss)
//            {
//                //TODO:
//            }
//            else
//            {
//                //var hm = go.GetComponent<HealthManager>();
//                //hm.hp = source.MaxHP;
//            }

//            return go;
//        }
//    }
//    public class TEMPLATE_PrefabConfig : DefaultPrefabConfig<TEMPLATE_Control>
//    {
//        public override void SetupPrefab(PrefabObject p)
//        {
//            base.SetupPrefab(p);

//            {
//                var fsm = p.prefab.LocateMyFSM("Control");
//            }
//        }
//    }



//}
