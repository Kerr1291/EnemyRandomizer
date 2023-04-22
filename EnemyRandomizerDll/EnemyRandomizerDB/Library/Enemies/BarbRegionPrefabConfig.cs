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
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker.Actions;

namespace EnemyRandomizerMod
{
    public class HornetBarbControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => gameObject.GetComponent<PlayMakerFSM>().FsmName; //not actually sure

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        public void ActivateBarb()
        {
            control.SendEvent("BARB READY");
        }
    }

    public class HornetBarbSpawner : DefaultSpawner<HornetBarbControl>
    {
    }

    public class HornetBarbPrefabConfig : DefaultPrefabConfig<HornetBarbControl>
    {
    }

    public class BarbRegionPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var fsm = p.prefab.LocateMyFSM("Spawn Barbs").Fsm;
            var spawnState = fsm.GetState("Spawn 1");
            var poolAction = spawnState.GetAction<SpawnObjectFromGlobalPool>(3);
            var prefab = poolAction.gameObject.Value;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("BARB_REGION old prefab name = " + p.prefab.name);

            //get actual object prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            Dev.Log("BARB_REGION New prefab name = " + keyName);

            //if(keyName.Contains("Small"))
            //    p.prefab.AddComponent<FlamebearerSmallControl>();
            //else if (keyName.Contains("Med"))
            //    p.prefab.AddComponent<FlamebearerMedControl>();
            //else if (keyName.Contains("Large"))
            //    p.prefab.AddComponent<FlamebearerLargeControl>();
        }
    }
}
