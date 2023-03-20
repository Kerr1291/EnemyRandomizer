using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

namespace EnemyRandomizerMod
{
    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageLargePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;
        }
    }

    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageSmallPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;
            if(prefab == null) 
                prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;
        }
    }
}
