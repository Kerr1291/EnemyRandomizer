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
using HutongGames.PlayMaker;
using Modding;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif
namespace EnemyRandomizerMod
{
    public class GiantFlyControl : MonoBehaviour
    {
        //default
        public static int babiesToSpawn = 7;

        static string MODHOOK_BeforeSceneLoad(string sceneName)
        {
            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            return sceneName;
        }

        void OnEnable()
        {
            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        }

        public void SelfSpawnBabies()
        {
            SpawnBabies(gameObject);
        }

        static void SpawnBabies(GameObject owner)
        {            
            try
            {
                Dev.Log("has database ref: " + EnemyRandomizerDatabase.GetDatabase.GetInvocationList().Length);
                if (EnemyRandomizerDatabase.GetDatabase != null)
                {
                    for (int i = 0; i < babiesToSpawn; ++i)
                    {
                        GameObject result = null;
                        if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
                        {
                            Dev.Log("trying to spawn via prefab " + src.prefabName);
                            result = EnemyRandomizerDatabase.GetDatabase().Spawn(src);
                        }
                        else
                        {
                            Dev.Log("trying to spawn via string");
                            result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly");
                        }

                        Dev.Log("result = " + result);
                        Dev.Log("self.Owner = " + owner);
                        if (result != null && owner != null)
                        {
                            result.transform.position = owner.transform.position;
                            result.SetActive(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        static void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        {
            orig(self);

            if (string.Equals(self.Name, "Spawn Flies 2"))
            {
                SpawnBabies(self.Fsm.Owner.gameObject);
            }
        }
    }

    internal class GiantFlyPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;
            var control = p.prefab.AddComponent<GiantFlyControl>();
        }
    }
}
