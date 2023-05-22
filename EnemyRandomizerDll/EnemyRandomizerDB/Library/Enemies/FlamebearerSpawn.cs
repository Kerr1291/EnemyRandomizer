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
        public abstract int Level { get; }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var hpScaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpScaler != null)
            {
                Destroy(hpScaler);
            }

            gameObject.GetComponent<BoxCollider2D>().enabled = true;

            var setLevel = control.GetState("Set Level");
            control.OverrideState("Set Level", () => {
                if (Level == 1)
                    control.SendEvent("Level 1");
                else if (Level == 1)
                    control.SendEvent("Level 2");
                else if (Level == 1)
                    control.SendEvent("Level 3");
                else 
                    control.SendEvent("FINISHED");
            });

            this.InsertHiddenState(control, "Init", "START", "Set Level");
        }

        protected override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            var result = base.GetStartingMaxHP(objectThatWillBeReplaced);
            float hpScale = GameObject.FindObjectsOfType<FlamebearerControl>().Length;

            if (hpScale < 1f)
                hpScale = 2f;
            else if (hpScale > 1f)
                hpScale *= 4f;

            float curHP = result;
            float newHP = curHP / hpScale;

            //maps get insane when there's lots of these.... so scale down their HP really fast if there's more than 1
            return Mathf.Clamp(Mathf.FloorToInt(newHP), 1, Mathf.FloorToInt(curHP));
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


            Dev.Log("MODIFYING FSM");

            var control = prefab.LocateMyFSM("Control");

            Dev.Log("GOT CONTROL "+ control);

            var init = control.GetState("Init");

            Dev.Log("GOT INIT " + init);

            Dev.Log($"init BEFORE {init.Actions.Length}");
            init.DisableAction(2);
            init.AddCustomAction(() => { control.SendEvent("START"); });
            p.prefab.AddComponent<FlameBearerFixer>();
            Dev.Log($"init AFTER {init.Actions.Length}");

            Dev.Log("FLAMEBEARER_CONVERSION New prefab name = " + keyName);
        }
    }

    public class FlameBearerFixer : MonoBehaviour
    {
        IEnumerator Start()
        {
            var fsm = gameObject.LocateMyFSM("Control");
            var init = fsm.GetState("Init");
            init.DisableAction(2);
            init.AddCustomAction(() => { fsm.SendEvent("START"); });
            yield return new WaitUntil(() => fsm.ActiveStateName == "Init");
            for(; ; )
            {
                if (fsm == null)
                    yield break;

                if(fsm.ActiveStateName == "Init")
                {
                    var active = fsm.GetState(fsm.ActiveStateName);
                    if (active.ActiveActionIndex >= 22)
                        fsm.SendEvent("START");
                }
                else
                {
                    yield break;
                }

                yield return null;
            }
        }
    }
}
