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
    internal class ColosseumCageLargePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

            Dev.Log("LOADING_COLO Old prefab " + p.prefab);
            Dev.Log("LOADING_COLO New prefab " + prefab);

            //if (p.prefab.GetSceneHierarchyPath().Contains("Wave 1"))
            //{

            //}
            //else if (p.prefab.GetSceneHierarchyPath().Contains("Wave 1"))
            //{

            //}

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            Dev.Log("New prefab name = " + keyName);
        }
    }

    internal class ColosseumCageSmallPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;
            if(prefab == null) 
                prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;

            Dev.Log("LOADING_COLO Old prefab " + p.prefab);
            Dev.Log("LOADING_COLO New prefab " + prefab);

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            Dev.Log("New prefab name = " + keyName);
        }
    }


    //internal class Colosseum_Shield_ZombiePrefabConfig : Colosseum_Cage_Prefab
    //{
    //}


    //internal class Colosseum_Armoured_RollerPrefabConfig : Colosseum_Cage_Prefab
    //{
    //}

    //internal class Colosseum_MinerPrefabConfig : Colosseum_Cage_Prefab
    //{
    //}
}
