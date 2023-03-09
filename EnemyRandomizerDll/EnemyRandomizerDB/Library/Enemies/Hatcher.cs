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
using HutongGames.PlayMaker.Actions;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif
namespace EnemyRandomizerMod
{
    public class HatcherControl : MonoBehaviour
    {
        public int maxBabies = 3;
        public int babiesRemaining = 3;


        void Start()
        {
            On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild += GetRandomChild_DoGetRandomChild;

            babiesRemaining = maxBabies;

            var fsm = gameObject.LocateMyFSM("Hatcher");

            //replace get child count with "set int value" to manually set the value for cage children
            fsm.Fsm.GetState("Hatched Max Check").Actions = fsm.Fsm.GetState("Hatched Max Check").Actions.Select(x => {
                if (x.GetType() == typeof(HutongGames.PlayMaker.Actions.GetChildCount))
                {
                    var action = new HutongGames.PlayMaker.Actions.SetIntValue();
                    action.Init(x.State);
                    action.intVariable = new HutongGames.PlayMaker.FsmInt("Cage Children");
                    action.intValue = new HutongGames.PlayMaker.FsmInt();
                    action.intValue = babiesRemaining;
                    return action;
                }
                else
                {
                    return x;
                }
            }).ToArray();
        }

        void OnDestroy()
        {
            On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild -= GetRandomChild_DoGetRandomChild;
        }

        void GetRandomChild_DoGetRandomChild(On.HutongGames.PlayMaker.Actions.GetRandomChild.orig_DoGetRandomChild orig, HutongGames.PlayMaker.Actions.GetRandomChild self)
        {
            orig(self);

            //don't run this logic
            var owner = self.Fsm.GetOwnerDefaultTarget(self.gameObject);
            var other = this;
            if (owner != other.gameObject)
                return;

            try
            {
                GameObject result = null;

                if (other.babiesRemaining > 0)
                {
                    if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Hatcher Baby", out var src))
                    {
                        result = EnemyRandomizerDatabase.GetDatabase().Spawn(src);
                    }
                    else
                    {
                        result = EnemyRandomizerDatabase.GetDatabase().Spawn("Hatcher Baby");
                    }

                    if (result != null && self.Owner != null)
                    {
                        other.babiesRemaining--;
                        (self.Fsm.GetState("Hatched Max Check").Actions.FirstOrDefault(x => x is HutongGames.PlayMaker.Actions.SetIntValue) as HutongGames.PlayMaker.Actions.SetIntValue).intValue.Value = other.babiesRemaining;
                        result.transform.position = self.Owner.transform.position;
                        result.SetActive(true);
                    }
                }

                self.storeResult.Value = result;
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        public static void SpawnBabies(GameObject owner)
        {
            try
            {
                Dev.Log("has database ref: " + EnemyRandomizerDatabase.GetDatabase.GetInvocationList().Length);
                if (EnemyRandomizerDatabase.GetDatabase != null)
                {
                    for (int i = 0; i < 7; ++i)
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

                GameObject.Destroy(owner.GetComponent<PlayMakerFSM>());
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
            }
        }
    }

    internal class HatcherPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;
            var control = p.prefab.AddComponent<HatcherControl>();
        }
    }
}
