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
    internal class FlamebearerSpawnPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn Control").Fsm.GetFsmGameObject("Grimmkin Obj").Value;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("FLAMEBEARER_CONVERSION old prefab name = " + p.prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            Dev.Log("FLAMEBEARER_CONVERSION New prefab name = " + keyName);
        }
    }
}
