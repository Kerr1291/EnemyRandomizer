//using UnityEngine.SceneManagement;
//using UnityEngine;
//using Language;
//using On;
//using EnemyRandomizerMod;
//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Serialization;
//using System.Collections;
//using System;
//using HutongGames.PlayMaker;
//using HutongGames.PlayMaker.Actions;
//using Satchel;
//using Satchel.Futils;
//namespace EnemyRandomizerMod
//{
//    public class HatcherControl_OLD : DefaultSpawnedEnemyControl
//    {
//        public int maxBabies = 3;
//        public int babiesRemaining = 3;
//        public float delayTime = 5f;
//        public AudioClip clip;
//        public AudioSource audio;

//        public PlayMakerFSM FSM { get; set; }

//        void Start()
//        {
//            FSM = GetComponent<PlayMakerFSM>();
//            var audioState = FSM.GetState("Fire").GetAction<AudioPlayerOneShot>(0);
//            clip = audioState.audioClips.FirstOrDefault();
//            audio = GetComponent<AudioSource>();


//            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
//            //On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild += GetRandomChild_DoGetRandomChild;

//            babiesRemaining = maxBabies;

//            //var fsm = gameObject.LocateMyFSM("Hatcher");

//            ////replace get child count with "set int value" to manually set the value for cage children
//            //fsm.Fsm.GetState("Hatched Max Check").Actions = fsm.Fsm.GetState("Hatched Max Check").Actions.Select(x => {
//            //    if (x.GetType() == typeof(HutongGames.PlayMaker.Actions.GetChildCount))
//            //    {
//            //        var action = new HutongGames.PlayMaker.Actions.SetIntValue();
//            //        action.Init(x.State);
//            //        action.intVariable = new HutongGames.PlayMaker.FsmInt("Cage Children");
//            //        action.intValue = new HutongGames.PlayMaker.FsmInt();
//            //        action.intValue = babiesRemaining;
//            //        return action;
//            //    }
//            //    else
//            //    {
//            //        return x;
//            //    }
//            //}).ToArray();
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();

//            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
//            //On.HutongGames.PlayMaker.Actions.GetRandomChild.DoGetRandomChild -= GetRandomChild_DoGetRandomChild;
//        }

//        IEnumerator babySpawner;

//        IEnumerator SpawnBabiesAfterDelay(float time)
//        {
//            if (!FSM.enabled)
//                FSM.enabled = true;
//            FSM.SetState("Distance Fly");
//            yield return new WaitForSeconds(time);
//            SpawnBabies();
//            babySpawner = null;

//            if (babiesRemaining > 0)
//            {
//                babySpawner = SpawnBabiesAfterDelay(delayTime);
//                StartCoroutine(babySpawner);
//                if (!FSM.enabled)
//                    FSM.enabled = true;
//                FSM.SetState("Distance Fly");
//            }
//        }

//        void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
//        {
//            orig(self);

//            if (self == null || self.Fsm != FSM.Fsm)
//                return;

//            try
//            {

//                Dev.Log(self.Name);

//                if (self.Name == "Distance Fly")
//                {
//                    if (babiesRemaining > 0)
//                    {
//                        Dev.Log("Chance to spawn babies state");
//                        if (babySpawner == null)
//                        {
//                            babySpawner = SpawnBabiesAfterDelay(delayTime);
//                            StartCoroutine(babySpawner);
//                            //FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
//                        }
//                        //FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
//                    }
//                }

//                //if (self.Name == "Hatched Max Check")
//                //{
//                //    if (babiesRemaining > 0)
//                //    {
//                //        //SpawnBabies();
//                //        if (babySpawner == null)
//                //        {
//                //            babySpawner = SpawnBabiesAfterDelay(delayTime);
//                //            StartCoroutine(babySpawner);
//                //            FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
//                //        }
//                //    }
//                //}

//            }
//            catch(Exception e)
//            {
//                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
//            }
//            //if (string.Equals(self.Name, "Fire Anticipate"))
//            //{
//            //    if (babiesRemaining > 0)
//            //    {
//            //        SpawnBabies();
//            //        FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
//            //    }
//            //}
//        }

//        //void GetRandomChild_DoGetRandomChild(On.HutongGames.PlayMaker.Actions.GetRandomChild.orig_DoGetRandomChild orig, HutongGames.PlayMaker.Actions.GetRandomChild self)
//        //{
//        //    orig(self);

//        //    //don't run this logic
//        //    var owner = self.Fsm.GetOwnerDefaultTarget(self.gameObject);
//        //    var other = this;
//        //    if (owner != other.gameObject)
//        //        return;

//        //    self.storeResult.Value = SpawnBabies();
//        //}

//        public GameObject SpawnBabies()
//        {
//            Dev.Log("spawn babies");
//            try
//            {
//                GameObject result = null;

//                if (babiesRemaining > 0)
//                {
//                    //TODO: add a scene reference for "Hatcher Baby"
//                    if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
//                    {
//                        result = EnemyRandomizerDatabase.GetDatabase().Spawn(src, null);
//                    }
//                    else
//                    {
//                        result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
//                    }

//                    if (result != null)
//                    {
//                        babiesRemaining--;
//                        //(FSM.Fsm.GetState("Hatched Max Check").Actions.FirstOrDefault(x => x is HutongGames.PlayMaker.Actions.SetIntValue) as HutongGames.PlayMaker.Actions.SetIntValue).intValue.Value = babiesRemaining;
//                        result.transform.position = transform.position;
//                        result.SetActive(true);
//                        if(audio != null && clip != null)
//                        {
//                            audio.PlayOneShot(clip);
//                        }
//                    }
//                }

//                return result;
//            }
//            catch (Exception e)
//            {
//                Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
//            }

//            return null;
//        }

//        //public static void SpawnBabies(GameObject owner)
//        //{
//        //    try
//        //    {
//        //        Dev.Log("has database ref: " + EnemyRandomizerDatabase.GetDatabase.GetInvocationList().Length);
//        //        if (EnemyRandomizerDatabase.GetDatabase != null)
//        //        {
//        //            for (int i = 0; i < 7; ++i)
//        //            {
//        //                GameObject result = null;
//        //                if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
//        //                {
//        //                    Dev.Log("trying to spawn via prefab " + src.prefabName);
//        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn(src, null);
//        //                }
//        //                else
//        //                {
//        //                    Dev.Log("trying to spawn via string");
//        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
//        //                }

//        //                Dev.Log("result = " + result);
//        //                Dev.Log("self.Owner = " + owner);
//        //                if (result != null && owner != null)
//        //                {
//        //                    result.transform.position = owner.transform.position;
//        //                    result.SetActive(true);
//        //                }
//        //            }
//        //        }

//        //        GameObject.Destroy(owner.GetComponent<PlayMakerFSM>());
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        Dev.LogError($"Caught exception trying to spawn a custom hatcher child! {e.Message} STACKTRACE:{e.StackTrace}");
//        //    }
//        //}
//    }

//    public class HatcherSpawner : DefaultSpawner<HatcherControl>
//    {
//        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
//        {
//            var go = base.Spawn(p, source);
//            var fsm = go.GetComponent<HatcherControl>();
//            fsm.FSM = go.LocateMyFSM("Control");

//            if (source.IsBoss)
//            {
//                //TODO:
//            }
//            else
//            {
//                //var hm = go.GetComponent<HealthManager>();
//                //hm.hp = source.MaxHP;
//            }

//            return go;
//        }
//    }

//    public class HatcherPrefabConfig : DefaultPrefabConfig<HatcherControl>
//    {
//        public override void SetupPrefab(PrefabObject p)
//        {
//            base.SetupPrefab(p);
//            //string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
//            //p.prefabName = keyName;
//            //var control = p.prefab.AddComponent<HatcherControl>();
//        }
//    }
//}
