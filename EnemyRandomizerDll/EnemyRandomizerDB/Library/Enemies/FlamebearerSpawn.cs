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

namespace EnemyRandomizerMod
{
    public abstract class FlamebearerControl : FSMAreaControlEnemy
    {
        public override string FSMName => gameObject.GetComponent<PlayMakerFSM>().FsmName; //not actually sure

        public abstract int Level { get; }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            gameObject.GetComponent<BoxCollider2D>().enabled = true;

            var init = control.GetState("Init");
            init.DisableAction(2);
            init.AddCustomAction(() => { control.SendEvent("START"); });

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

        protected override void Show()
        {
            base.Show();
            if(control != null)
                control.Fsm.BroadcastEvent("GRIMMKIN SPAWN");
        }
    }

    public class FlamebearerSmallControl : FlamebearerControl
    {
        public override int Level => 1;
    }

    public class FlamebearerMedControl : FlamebearerControl
    {
        public override int Level => 2;
    }

    public class FlamebearerLargeControl : FlamebearerControl
    {
        public override int Level => 3;
    }

    public class FlamebearerSmallSpawner : DefaultSpawner<FlamebearerSmallControl>
    {
    }

    public class FlamebearerMedSpawner : DefaultSpawner<FlamebearerMedControl>
    {
    }

    public class FlamebearerLargeSpawner : DefaultSpawner<FlamebearerLargeControl>
    {
    }


    public class FlamebearerSpawnPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
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
