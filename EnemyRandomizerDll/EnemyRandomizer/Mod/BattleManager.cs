using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using nv;
using UniRx;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public class BattleManager : MonoBehaviour
    {
        //TODO extract and customize these managers
        public static List<string> battleControllers = new List<string>()
                {
                    "Battle Scene Ore",
                    "Battle Start",
                    "Battle Scene",
                    "Battle Scene v2",
                    "Battle Music",
                    "Mantis Battle",
                    "Lurker Control",
                    "Battle Control",
                    "Grimm Holder",
                    "Grub Scene",
                    "Boss Scene Controller",
                    "Colosseum Manager",
                };

        public static bool IsBattleManager(GameObject gameObject)
        {
            return battleControllers.Contains(gameObject.name);
        }

        public static ReactiveProperty<BattleManager> Instance { get; protected set; }
        public static ReactiveProperty<BattleStateMachine> StateMachine { get; protected set; }
        //static protected Dictionary<int, BattleWave> BattleWaves
        //{
        //    get
        //    {
        //        if (battleWaves == null)
        //            battleWaves = new Dictionary<int, BattleWave>();
        //        return battleWaves;
        //    }
        //}
        //static protected Dictionary<int, BattleWave> battleWaves;

        public HashSet<GameObject> nonWaveEnemies = new HashSet<GameObject>();

        public static void Init()
        {
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>();
            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();
            //BattleWaves.Clear();
        }

        public static void OnEnemyDeathEvent(GameObject gameObject)
        {
            if (Instance.HasValue && Instance.Value != null)
            {
                var bmo = gameObject.GetComponent<BattleManagedObject>();
                if (bmo != null)
                {
                    bmo.myWave.RegisterEnemyDeath(bmo);
                }
            }
        }

        public static bool DidSceneCheck;

        public static void DoSceneCheck(GameObject sceneObject)
        {
            Scene currentScene = sceneObject.scene;

            var roots = currentScene.GetRootGameObjects();
            var found = roots.FirstOrDefault(x => IsBattleManager(x));

            if(found)
            {
                LoadFromFSM(found.GetComponent<PlayMakerFSM>());
            }
                 
            BattleManager.DidSceneCheck = true;
        }

        public static void LoadFromFSM(PlayMakerFSM fsm)
        {
            if (Instance.HasValue && Instance.Value != null)
                return;

            bool isValid = BattleManager.IsBattleManager(fsm.gameObject);

            if (isValid)
            {
                //we found it, but it's done so we don't need to do anything
                var pbi = fsm.GetComponent<PersistentBoolItem>();
                if (pbi != null && pbi.persistentBoolData.activated)
                    return;

                //attach our own
                Instance.Value = fsm.gameObject.AddComponent<BattleManager>();

                //set it up
                Instance.Value.Setup(fsm.gameObject.scene, fsm, pbi);
            }
        }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>(this);
            else
                Instance.Value = this;

            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();            
        }

        protected virtual void OnDestroy()
        {
            Instance.Value = null;
            if (StateMachine.Value != null)
                StateMachine.Value.Dispose();
            StateMachine.Value = null;
        }

        public void Setup(Scene scene, PlayMakerFSM fsm, PersistentBoolItem item)
        {
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>(this);

            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();

            //if true, no point in this and destroy it
            if (item == null || item.persistentBoolData.activated)
            {
                Cleanup();
                return;
            }

            if(Instance.Value != this)
                Instance.Value = this;

            StateMachine.Value = BattleStateMachine.Create(scene, fsm, item);
            StateMachine.Value.Build(fsm);
            StateMachine.Value.OnComplete.AddListener(Cleanup);

            Destroy(fsm);

            if (StateMachine.Value.InitState != null)
                StateMachine.Value.InitState.Invoke();

            nonWaveEnemies.ToList().ForEach(x =>
            {
                x.SafeSetActive(true);
            });
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                StateMachine.Value.NotifyPlayerEnteredTrigger();
            }
        }

        protected virtual void Cleanup()
        {
            Instance.Value = null;
            StateMachine.Value = null;
            //BattleWaves.Clear();
            Destroy(this);
        }

        //public void RegisterEnemy(BattleManagedObject bmo)
        //{
        //    //special exception, let these randomize normally
        //    if (bmo.gameObject.GetSceneHierarchyPath().Contains("Pre Battle Enemies")
        //        || (!string.IsNullOrEmpty(bmo.originalGameObjectPath) && bmo.originalGameObjectPath.Contains("Pre Battle Enemies")))
        //    {
        //        nonWaveEnemies.Add(bmo.gameObject);
        //        return;
        //    }

        //    if (StateMachine.Value != null)
        //    {
        //        StateMachine.Value.RegisterEnemy(bmo);
        //        return;
        //    }

        //    int wave = bmo.GetMyWave();

        //    if (!BattleWaves.TryGetValue(wave, out var bwave))
        //    {
        //        if(wave >= 0)
        //        {
        //            bwave = new BattleWave(wave, bmo.gameObject.scene);
        //            BattleWaves.Add(wave, bwave);
        //        }
        //        else
        //        {
        //            if(!BattleWaves.ContainsKey(0))
        //            {
        //                BattleWaves.Add(wave, new BattleWave(0,bmo.gameObject.scene));
        //            }

        //            var w0 = BattleWaves[0];
        //            //add all subwaves to wave 0 for now until we find a case where this doesn't work out..
        //            int subwave = -wave;
        //            if (!w0.SubWaves.TryGetValue(subwave, out var sw))
        //            {
        //                sw = new BattleWave(subwave, bmo.gameObject.scene);
        //                w0.SubWaves.Add(subwave, sw);
        //            }

        //            sw.AddEnemy(bmo);
        //            return;
        //        }
        //    }

        //    bwave.AddEnemy(bmo);
        //}

        //public void UnregisterEnemy(BattleManagedObject bmo)
        //{
        //    //special exception, let these randomize normally
        //    if (bmo.gameObject.GetSceneHierarchyPath().Contains("Pre Battle Enemies")
        //        || (!string.IsNullOrEmpty(bmo.originalGameObjectPath) && bmo.originalGameObjectPath.Contains("Pre Battle Enemies")))
        //    {
        //        nonWaveEnemies.Remove(bmo.gameObject);
        //        return;
        //    }

        //    if (StateMachine.Value != null)
        //    {
        //        StateMachine.Value.UnregisterEnemy(bmo);
        //        return;
        //    }

        //    UnregisterEnemy(bmo, bmo.GetMyWave());
        //}

        //public void UnregisterEnemy(BattleManagedObject bmo, int oldWave)
        //{
        //    //special exception, let these randomize normally
        //    if (bmo.gameObject.GetSceneHierarchyPath().Contains("Pre Battle Enemies")
        //        || (!string.IsNullOrEmpty(bmo.originalGameObjectPath) && bmo.originalGameObjectPath.Contains("Pre Battle Enemies")))
        //    {
        //        nonWaveEnemies.Remove(bmo.gameObject);
        //        return;
        //    }

        //    if (StateMachine.Value != null)
        //    {
        //        StateMachine.Value.UnregisterEnemy(bmo, bmo.GetMyWave());
        //        return;
        //    }

        //    if (BattleWaves.TryGetValue(oldWave, out var bwave))
        //    {
        //        bwave.RemoveEnemy(bmo);

        //        //if the wave is empty, remove it
        //        if (bwave.WaveSizeOnSetup <= 0)
        //        {
        //            bwave.Dispose();
        //            BattleWaves.Remove(oldWave);
        //        }
        //    }
        //}
    }
}




/*

//static List<string> battleControllers = new List<string>()
//                {
//                    "Battle Scene Ore",
//                    "Battle Start",
//                    "Battle Scene",
//                    "Battle Scene v2",
//                    "Battle Music",
//                    "Mantis Battle",
//                    "Lurker Control",
//                    "Battle Control",
//                    "Grimm Holder",
//                    "Grub Scene",
//                    "Boss Scene Controller",
//                    "Colosseum Manager",
//                };

1-

init
BoolTest -> 


*/





//public static Dictionary<string, Dictionary<string, BattleActions>> GameBattleActions = new Dictionary<string, Dictionary<string, BattleActions>>()
//        {
//            {//entry
//                "Crossroads_10_boss", new Dictionary<string, BattleActions>()
//                {
//                    { "Init", new BattleActions()
//                        {
//                            gameObjectToDestroy = @"Battle Scene/FK Armour",
//                            musicRegionToEnable = null,
//                            musicRegionToDestroy = null,
//                            cameraLockToEnable = @"Battle Scene/CameraLockArea B",
//                            cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
//                        }
//                    },
//                    { "Start", new BattleActions()
//                        {
//                            gameObjectToDestroy = null,
//                            musicRegionToEnable = null,
//                            musicRegionToDestroy = @"Battle Scene/Music Region B",
//                            cameraLockToEnable = @"Battle Scene/CameraLockArea B",
//                            cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
//                        }
//                    },
//                }
//            },//end entry
            
//            //{//entry
//            //    "Crossroads_04", new Dictionary<string, BattleActions>()
//            //    {
//            //        { "Init", new BattleActions()
//            //            {
//            //                gameObjectToDestroy = @"Battle Scene/FK Armour",
//            //                musicRegionToEnable = null,
//            //                musicRegionToDestroy = null,
//            //                cameraLockToEnable = @"Battle Scene/CameraLockArea B",
//            //                cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
//            //            }
//            //        },
//            //        { "Start", new BattleActions()
//            //            {
//            //                gameObjectToDestroy = null,
//            //                musicRegionToEnable = null,
//            //                musicRegionToDestroy = @"Battle Scene/Music Region B",
//            //                cameraLockToEnable = @"Battle Scene/CameraLockArea B",
//            //                cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
//            //            }
//            //        },
//            //    }
//            //},//end entry
//        };