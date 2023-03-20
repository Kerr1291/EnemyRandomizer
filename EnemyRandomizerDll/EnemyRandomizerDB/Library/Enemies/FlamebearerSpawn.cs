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
    public class FlamebearerControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            float hpScale = GameObject.FindObjectsOfType<FlamebearerControl>().Length;

            if (hpScale < 1f)
                hpScale = 1f;
            else if (hpScale > 1f)
                hpScale *= 2f;

            float curHP = thisMetadata.EnemyHealthManager.hp;
            float newHP = curHP / hpScale;

            //maps get insane when there's lots of these.... so scale down their HP really fast if there's more than 1
            thisMetadata.EnemyHealthManager.hp = Mathf.Clamp(Mathf.FloorToInt(newHP), 1, Mathf.FloorToInt(curHP));
        }
    }

    public class FlamebearerSmallControl : FlamebearerControl
    {
    }
    public class FlamebearerMedControl : FlamebearerControl
    {
    }
    public class FlamebearerLargeControl : FlamebearerControl
    {
    }

    public class FlamebearerSmallSpawner : DefaultSpawner<FlamebearerControl>
    {
    }

    public class FlamebearerMedSpawner : DefaultSpawner<FlamebearerControl>
    {
    }

    public class FlamebearerLargeSpawner : DefaultSpawner<FlamebearerControl>
    {
    }


    public class FlamebearerSpawnPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab.LocateMyFSM("Spawn Control").Fsm.GetFsmGameObject("Grimmkin Obj").Value;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            //Dev.Log("FLAMEBEARER_CONVERSION old prefab name = " + p.prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            //Dev.Log("FLAMEBEARER_CONVERSION New prefab name = " + keyName);

            if(keyName.Contains("Small"))
                p.prefab.AddComponent<FlamebearerSmallControl>();
            else if (keyName.Contains("Med"))
                p.prefab.AddComponent<FlamebearerMedControl>();
            else if (keyName.Contains("Large"))
                p.prefab.AddComponent<FlamebearerLargeControl>();
        }
    }
}
