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
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{
    public class HornetBarbControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var init = control.GetState("Init");
            init.DisableAction(7);

            var dc = control.GetState("Distance Check");
            control.OverrideState("Distance Check", () => { control.SendEvent("FINISHED"); });
        }

        public void ActivateBarb()
        {
            control.SendEvent("BARB READY");
        }
    }

    public class HornetBarbSpawner : DefaultSpawner<HornetBarbControl>
    {
    }

    public class HornetBarbPrefabConfig : DefaultPrefabConfig
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






    public class FountainCenterControl : MonoBehaviour
    {
    }

    public class FountainCenterSpawner : DefaultSpawner { }

    public class FountainCenterPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<FountainCenterControl>();
        }
    }

    public class _0083_fountainPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            prefab.name = "Fountain Center";

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("Loaded Fountain center = " + p.prefab.name);

            p.prefabName = "Fountain Center";
            p.prefab = prefab;
        }
    }








    public class FountainBackControl : MonoBehaviour { }

    public class FountainBackSpawner : DefaultSpawner { }

    public class FountainBackPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<FountainBackControl>();
        }
    }

    public class _0082_fountainPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            prefab.name = "Fountain Back";

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("Loaded Fountain center = " + p.prefab.name);

            p.prefabName = prefab.name;
            p.prefab = prefab;
        }
    }










    public class FountainLeftControl : MonoBehaviour { }

    public class FountainLeftSpawner : DefaultSpawner { }

    public class FountainLeftPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<FountainLeftControl>();
        }
    }







    public class FountainRightControl : MonoBehaviour { }

    public class FountainRightSpawner : DefaultSpawner { }

    public class FountainRightPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<FountainRightControl>();
        }
    }

    public class _0092_fountainPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;

            if(p.source.path.Contains("(1)"))
            {
                prefab.name = "Fountain Left";
            }
            else
            {
                prefab.name = "Fountain Right";
            }

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("Loaded = " + p.prefab.name);

            p.prefabName = prefab.name;
            p.prefab = prefab;
        }
    }

    public class _0092_fountain_1PrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;

            {
                prefab.name = "Fountain Left";
            }

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("Loaded = " + p.prefab.name);

            p.prefabName = prefab.name;
            p.prefab = prefab;
        }
    }




    public class GG_Statue_ZotePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
            var bossStatue = p.prefab.GetComponentInChildren<BossStatue>(true);
            if (bossStatue != null)
                GameObject.Destroy(bossStatue);
        }
    }


    public class GG_Statue_GorbPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
            var bossStatue = p.prefab.GetComponentInChildren<BossStatue>(true);
            if (bossStatue != null)
                GameObject.Destroy(bossStatue);
        }
    }

    public class GG_Statue_GreyPrincePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
            var bossStatue = p.prefab.GetComponentInChildren<BossStatue>(true);
            if (bossStatue != null)
                GameObject.Destroy(bossStatue);
        }
    }

    public class Knight_v01PrincePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
            var bossStatue = p.prefab.GetComponentInChildren<BossStatue>(true);
            if (bossStatue != null)
                GameObject.Destroy(bossStatue);
        }
    }


    public class gg_blue_corePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
            var dpdpt = p.prefab.GetComponentInChildren<DeactivateIfPlayerdataTrue>(true);
            if (dpdpt != null)
                GameObject.Destroy(dpdpt);
            var dreamReact = p.prefab.LocateMyFSM("Dream React");
            {
                var state = dreamReact.GetState("Take Control");
                var alist = state.Actions.ToList();
                for (int i = 0; i < alist.Count; ++i)
                {
                    state.DisableAction(i);
                }
            }
            {
                var state = dreamReact.GetState("Regain Control");
                var alist = state.Actions.ToList();
                for (int i = 0; i < alist.Count; ++i)
                {
                    state.DisableAction(i);
                }
            }
        }
    }


    public class dream_beam_animationPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var prefab = p.prefab;
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);
            p.prefabName = prefab.name;
            p.prefab = prefab;
        }
    }
}
