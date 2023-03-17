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

        public PlayMakerFSM FSM { get; protected set; }

        void Start()
        {
            FSM = GetComponent<PlayMakerFSM>();
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
            //On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild += GetRandomChild_DoGetRandomChild;

            babiesRemaining = maxBabies;

            //var fsm = gameObject.LocateMyFSM("Hatcher");

            ////replace get child count with "set int value" to manually set the value for cage children
            //fsm.Fsm.GetState("Hatched Max Check").Actions = fsm.Fsm.GetState("Hatched Max Check").Actions.Select(x => {
            //    if (x.GetType() == typeof(HutongGames.PlayMaker.Actions.GetChildCount))
            //    {
            //        var action = new HutongGames.PlayMaker.Actions.SetIntValue();
            //        action.Init(x.State);
            //        action.intVariable = new HutongGames.PlayMaker.FsmInt("Cage Children");
            //        action.intValue = new HutongGames.PlayMaker.FsmInt();
            //        action.intValue = babiesRemaining;
            //        return action;
            //    }
            //    else
            //    {
            //        return x;
            //    }
            //}).ToArray();
        }

        void OnDestroy()
        {
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            //On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild -= GetRandomChild_DoGetRandomChild;
        }

        void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        {
            orig(self);

            if (self == null || self.Fsm != FSM.Fsm)
                return;

            try
            {

                Dev.Log(self.Name);

                if (self.Name == "Distance Fly")
                {
                    if (babiesRemaining > 0)
                    {
                        Dev.Log("Chance to spawn babies state");
                        FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
                    }
                }

                if (self.Name == "Hatched Max Check")
                {
                    if (babiesRemaining > 0)
                    {
                        Dev.Log("spawn babies");
                        SpawnBabies();
                        //FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[1].EventName);
                    }
                }

            }
            catch(Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
            }
            //if (string.Equals(self.Name, "Fire Anticipate"))
            //{
            //    if (babiesRemaining > 0)
            //    {
            //        SpawnBabies();
            //        FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
            //    }
            //}
        }

        //void GetRandomChild_DoGetRandomChild(On.HutongGames.PlayMaker.Actions.GetRandomChild.orig_DoGetRandomChild orig, HutongGames.PlayMaker.Actions.GetRandomChild self)
        //{
        //    orig(self);

        //    //don't run this logic
        //    var owner = self.Fsm.GetOwnerDefaultTarget(self.gameObject);
        //    var other = this;
        //    if (owner != other.gameObject)
        //        return;

        //    self.storeResult.Value = SpawnBabies();
        //}

        public GameObject SpawnBabies()
        {
            try
            {
                GameObject result = null;

                if (babiesRemaining > 0)
                {
                    //TODO: add a scene reference for "Hatcher Baby"
                    if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
                    {
                        result = EnemyRandomizerDatabase.GetDatabase().Spawn(src, null);
                    }
                    else
                    {
                        result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
                    }

                    if (result != null)
                    {
                        babiesRemaining--;
                        //(FSM.Fsm.GetState("Hatched Max Check").Actions.FirstOrDefault(x => x is HutongGames.PlayMaker.Actions.SetIntValue) as HutongGames.PlayMaker.Actions.SetIntValue).intValue.Value = babiesRemaining;
                        result.transform.position = transform.position;
                        result.SetActive(true);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
            }

            return null;
        }

        //public static void SpawnBabies(GameObject owner)
        //{
        //    try
        //    {
        //        Dev.Log("has database ref: " + EnemyRandomizerDatabase.GetDatabase.GetInvocationList().Length);
        //        if (EnemyRandomizerDatabase.GetDatabase != null)
        //        {
        //            for (int i = 0; i < 7; ++i)
        //            {
        //                GameObject result = null;
        //                if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
        //                {
        //                    Dev.Log("trying to spawn via prefab " + src.prefabName);
        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn(src, null);
        //                }
        //                else
        //                {
        //                    Dev.Log("trying to spawn via string");
        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
        //                }

        //                Dev.Log("result = " + result);
        //                Dev.Log("self.Owner = " + owner);
        //                if (result != null && owner != null)
        //                {
        //                    result.transform.position = owner.transform.position;
        //                    result.SetActive(true);
        //                }
        //            }
        //        }

        //        GameObject.Destroy(owner.GetComponent<PlayMakerFSM>());
        //    }
        //    catch (Exception e)
        //    {
        //        Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
        //    }
        //}
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
