using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using nv;
using UniRx;

namespace EnemyRandomizerMod
{
    public class BattleWave
    {
        public HashSet<BattleManagedObject> defaultEnemies = new HashSet<BattleManagedObject>();
        public HashSet<BattleManagedObject> waveEnemies = new HashSet<BattleManagedObject>();

        public int Total
        {
            get; protected set;
        }

        public ReactiveProperty<int> dead;

        public BattleWave()
        {
            dead = new ReactiveProperty<int>(0);
        }

        public void GenerateWave()
        {
            Total = defaultEnemies.Count + waveEnemies.Count;

            //TODO:
        }

        public void PlayWave()
        {
            //TODO:
        }

        //move wave functions into this class....
    }

    //public class GameStateActions
    //{
    //    //the scene to run these done
    //    public string scene;

    //    //the state name to run these on
    //    public string state;

    //    public UnityEngine.Audio.AudioMixerSnapshot silenceSnapshot;
    //    public System.Action<PlayMakerFSM> onStateAction;
    //}

    public class BattleActions
    {
        //the scene to run these done
        public string scene;

        //the state name to run these on
        public string state;

        //the below are game heriarchy paths -- null values will be ignored
        public string gateScene;
        public string gameObjectToDestroy;
        public string musicRegionToEnable;
        public string musicRegionToDestroy;
        public string cameraLockToEnable;
        public string cameraLockToDisable;

        public static Dictionary<string, List<string>> SceneBattleGates = new Dictionary<string, List<string>>()
        {
            {
                "Crossroads_10", new List<string>()
                {
                    @"Battle Gate 2 (1)",
                    @"Battle Gate 3",
                    @"Battle Gate",
                }
            },
        };

        public static void OpenGates(string scene)
        {
            if(SceneBattleGates.TryGetValue(scene, out var gates))
            {
                gates.Select(x => GameObjectExtensions.FindGameObject(x)).Where(x => x != null).Select(x => x.GetComponent<PlayMakerFSM>()).ToList().ForEach(x =>
                {
                    x.SendEvent("BG OPEN");
                });
            }
        }

        public static void CloseGates(string scene)
        {
            if (SceneBattleGates.TryGetValue(scene, out var gates))
            {
                gates.Select(x => GameObjectExtensions.FindGameObject(x)).Where(x => x != null).Select(x => x.GetComponent<PlayMakerFSM>()).ToList().ForEach(x =>
                {
                    x.SendEvent("BG CLOSE");
                });
            }
        }

        //public static Dictionary<string, Dictionary<string, GameStateActions>> OnGameStateActions;

        //public static void BuildGameStateActions(PlayMakerFSM fsm)
        //{
        //    OnGameStateActions = new Dictionary<string, Dictionary<string, GameStateActions>>()
        //    {
        //        {//entry
        //            "Abyss_17", new Dictionary<string, BattleActions>()
        //        {
        //            { "End", new GameStateActions()
        //                {
        //                 scene = "Abyss_17",
        //                  state = "End",
        //                  onStateAction = x => { }
        //                }
        //            },
        //        }
        //        },//end entry
        //};
        //}

        public static Dictionary<string, Dictionary<string, BattleActions>> GameBattleActions = new Dictionary<string, Dictionary<string, BattleActions>>()
        {
            {//entry
                "Crossroads_10_boss", new Dictionary<string, BattleActions>()
                {
                    { "Init", new BattleActions()
                        {
                            gameObjectToDestroy = @"Battle Scene/FK Armour",
                            musicRegionToEnable = null,
                            musicRegionToDestroy = null,
                            cameraLockToEnable = @"Battle Scene/CameraLockArea B",
                            cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
                        }
                    },
                    { "Start", new BattleActions() 
                        {
                            gameObjectToDestroy = null,
                            musicRegionToEnable = null,
                            musicRegionToDestroy = @"Battle Scene/Music Region B",
                            cameraLockToEnable = @"Battle Scene/CameraLockArea B",
                            cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
                        } 
                    },
                }
            },//end entry
            
            //{//entry
            //    "Crossroads_04", new Dictionary<string, BattleActions>()
            //    {
            //        { "Init", new BattleActions()
            //            {
            //                gameObjectToDestroy = @"Battle Scene/FK Armour",
            //                musicRegionToEnable = null,
            //                musicRegionToDestroy = null,
            //                cameraLockToEnable = @"Battle Scene/CameraLockArea B",
            //                cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
            //            }
            //        },
            //        { "Start", new BattleActions()
            //            {
            //                gameObjectToDestroy = null,
            //                musicRegionToEnable = null,
            //                musicRegionToDestroy = @"Battle Scene/Music Region B",
            //                cameraLockToEnable = @"Battle Scene/CameraLockArea B",
            //                cameraLockToDisable = @"Battle Scene/CameraLockArea B2",
            //            }
            //        },
            //    }
            //},//end entry
        };

        protected static void TryDestroyGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    GameObject.Destroy(go);
            }
        }

        protected static void TryDisableGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    go.SetActive(false);
            }
        }

        protected static void TryEnableGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    go.SetActive(true);
            }
        }

        protected void UpdateObjectsInState()
        {
            TryDestroyGameObjectAtPath(gameObjectToDestroy);
            TryDestroyGameObjectAtPath(musicRegionToDestroy);
            TryDisableGameObjectAtPath(cameraLockToDisable);
            TryDisableGameObjectAtPath(musicRegionToEnable);
            TryDisableGameObjectAtPath(cameraLockToEnable);
        }

        public static void RunActionsOnState(string sceneName, string stateName)
        {
            if(GameBattleActions.TryGetValue(sceneName, out var battleActions))
            {
                if(battleActions.TryGetValue(stateName, out var actions))
                {
                    actions.UpdateObjectsInState();
                }
            }
        }
    }

    public class BattleManager : MonoBehaviour
    {
        public EnemyReplacer replacer;

        //WILL BE DELETED ON BATTLE START
        public PlayMakerFSM fsm;

        public PersistentBoolItem isBattleCompleteGameState;
        public BoxCollider2D triggerArea;
        public MusicRegion battleMusicRegion;
        public UnityEngine.Audio.AudioMixerSnapshot silenceSnapshot;

        public List<CameraLockArea> cameraLocks = new List<CameraLockArea>();

        public Dictionary<int, BattleWave> BattleWaves
        {
            get
            {
                if(battleWaves == null)
                    battleWaves = new Dictionary<int, BattleWave>();
                return battleWaves;
            }
        }
        public Dictionary<int, BattleWave> battleWaves;

        public string FsmSceneName { get; protected set; }

        public string FsmStartStateName { get; protected set; }

        public virtual bool StartOnTriggerEnter { get; protected set; }

        public bool InProgress { get; protected set; }

        public int CurrentWave { get; protected set; }

        public BattleWave ActiveBattleWave
        {
            get
            {
                return BattleWaves[CurrentWave];
            }
        }

        public bool IsDone
        {
            get
            {
                return isBattleCompleteGameState.persistentBoolData.activated;
            }
        }

        public bool shouldDestoryCamlocksAtEnd;

        public void Setup(EnemyReplacer replacer, PlayMakerFSM source, PersistentBoolItem item)
        {
            this.replacer = replacer;
            fsm = source;

            FsmSceneName = fsm.gameObject.scene.name;

            isBattleCompleteGameState = item;
            InProgress = false;

            triggerArea = GetStartTriggerArea();
            StartOnTriggerEnter = ShouldStartOnTriggerEnter();

            var init = GetStartingState();
            var battleStates = GetBattleStates(init);

            BattleActions.RunActionsOnState(FsmSceneName, init.Name);

            GenerateWaves(init, battleStates);

            cameraLocks = GetCameraLocksFromScene(fsm.gameObject).ToList();
            battleMusicRegion = GetMusicRegionsFromScene(fsm.gameObject).FirstOrDefault();

            if (!StartOnTriggerEnter)
                Begin(false);

            Destroy(fsm);
            fsm = null;
        }

        protected virtual void SetupSpecialData(string scene)
        {
            if (scene.Contains("Crossroads_10"))
            {
                shouldDestoryCamlocksAtEnd = true;
            }

            if(scene.Contains("Abyss_17"))
            {
                silenceSnapshot = GetAudioMixerSnapshotFromState(fsm.GetState("Completed"));
            }
        }

        protected virtual void GenerateWaves(FsmState startingState, IEnumerable<FsmState> battleStates)
        {
            //TODO: add audio snapshot transition for wave
            //TODO: add delay between wave transitions
            //TODO: add camera lock stuff on this battle
            if (FsmSceneName.Contains("Crossroads_08"))
            {

            }
        }

        protected virtual BoxCollider2D GetStartTriggerArea()
        {
            var result = fsm.GetComponent<BoxCollider2D>();

            if (result == null)
            {
                //some battle scenes have no trigger areas -- this is fine
            }

            return result;
        }

        protected virtual bool ShouldStartOnTriggerEnter()
        {
            if (triggerArea == null)
                return false;

            return true;
        }

        public void RegisterEnemy(BattleManagedObject bmo)
        {
            if (!BattleWaves.TryGetValue(bmo.GetMyWave(), out var bwave))
            {
                bwave = new BattleWave();
            }

            if(bmo.ThisIsSourceObject)
            {
                bwave.defaultEnemies.Add(bmo);
            }
            else
            {
                bwave.waveEnemies.Add(bmo);
            }

            bmo.myWave = bwave;
        }

        public void UnregisterEnemy(BattleManagedObject bmo)
        {
            if (BattleWaves.TryGetValue(bmo.GetMyWave(), out var bwave))
            {
                if (bmo.ThisIsSourceObject)
                {
                    if (bwave.defaultEnemies.Contains(bmo))
                        bwave.defaultEnemies.Remove(bmo);
                }
                else
                {
                    if (bwave.waveEnemies.Contains(bmo))
                        bwave.waveEnemies.Remove(bmo);
                }

                bmo.myWave = null;
            }
        }

        public virtual void RegisterEnemyDeath(BattleManagedObject deadEnemy)
        {
            deadEnemy.myWave.waveEnemies.Remove(deadEnemy);
            deadEnemy.myWave.dead.Value++;
        }

        System.IDisposable enemyDeadHandle;

        protected virtual void OnEnable()
        {
            //TODO: ????
            //deadEnemy.myWave.dead.
        }

        public virtual void OnEnemyDead()
        {
            if (CheckIsWaveDone())
            {
                DoNext();
            }
        }

        public virtual bool CheckIsWaveDone()
        {
            return (ActiveBattleWave.Total == ActiveBattleWave.dead);
        }

        public virtual void DoNext()
        {
            var next = GetNextWave();
            if(next == null)
                SetCompleted();
            else
                PlayWave(next);
        }

        public virtual void Begin(bool fromTrigger, bool closeGates = false)
        {
            GetFirstWave();
            InProgress = true;
            if(closeGates)
                CloseGates();

            //TODO: put the actions inside the state/waves ? -- anyway, let them be some generic lambdas or w/e
            BattleActions.RunActionsOnState(FsmSceneName, "Start");
        }

        public virtual BattleWave GetFirstWave()
        {
            //TODO: use sorted data set, remove this index entirely
            CurrentWave = 0;
            return ActiveBattleWave;
        }

        public virtual BattleWave GetNextWave()
        {
            //TODO: use sorted data set, remove this index entirely
            CurrentWave++;
            return ActiveBattleWave;
        }

        //move this play wave function into the wave class....
        public virtual void PlayWave(BattleWave wave)
        {
            BattleActions.RunActionsOnState(FsmSceneName, wave.originalStateName);
            BattleActions.RunActionsOnState(FsmSceneName, $"wave{CurrentWave}");
            ActiveBattleWave.Play();
        }

        public virtual void CloseGates()
        {
            var battleGate = GameObject.Find("Battle Gate");
            if (battleGate != null)
            {
                BattleActions.CloseGates(battleGate.scene.name);
            }
        }

        public virtual void OpenGates()
        {
            var battleGate = GameObject.Find("Battle Gate");
            if (battleGate != null)
            {
                BattleActions.OpenGates(battleGate.scene.name);
            }
        }

        public virtual void UnlockCameras(bool destroy = true)
        {
            cameraLocks.ForEach(x => x.gameObject.SetActive(false));
            if(destroy)
                cameraLocks.ForEach(x => GameObject.Destroy(x.gameObject));
        }

        public virtual void StopMusic()
        {
            if (battleMusicRegion != null)
                battleMusicRegion.gameObject.SetActive(false);

            if (FsmSceneName.Contains("Abyss_17"))
            {
                if (silenceSnapshot != null)
                    silenceSnapshot.TransitionTo(3f);
            }
        }

        public virtual void MarkPersistantBoolComplete()
        {
            isBattleCompleteGameState.persistentBoolData.activated = true;
            global::SceneData.instance.SaveMyState(isBattleCompleteGameState.persistentBoolData);
        }

        public virtual void Cleanup()
        {
            Destroy(this);
        }

        public virtual void SetCompleted()
        {
            BattleActions.RunActionsOnState(FsmSceneName, "End");

            //unlock cameras
            UnlockCameras(shouldDestoryCamlocksAtEnd);

            //stop music
            StopMusic();

            //open gates
            OpenGates();

            //mark event complete
            MarkPersistantBoolComplete();

            //remove this manager
            Cleanup();
        }

        protected virtual HutongGames.PlayMaker.FsmState GetStartingState()
        {
            var init = fsm.FsmStates.FirstOrDefault(x => x.Name == "Init");
            if (init == null)
            {
                //assume idle is the starting state
                init = fsm.FsmStates.FirstOrDefault(x => x.Name == "Idle");
            }

            return init;
        }

        protected virtual IEnumerable<HutongGames.PlayMaker.FsmState> GetBattleStates(FsmState startingState)
        {
            //these are states we don't care to scrape for data (the starting state, usually init) is also included
            string[] badStates = new string[]
            {
            "Pause",
            "Detect",
            "PrePause",
            "Activate",
            "Activated",
            "Completed"
            };

            return fsm.FsmStates.Where(x => !badStates.Contains(x.Name) && startingState.Name != x.Name);
        }

        protected virtual IEnumerable<HutongGames.PlayMaker.FsmStateAction> GetBattleActions(IEnumerable<HutongGames.PlayMaker.FsmState> battleStates)
        {
            System.Type[] badActions = new System.Type[]
            {
            typeof(HutongGames.PlayMaker.Actions.BoolTest),
            };

            return battleStates.SelectMany(x => x.Actions).Where(x => !badActions.Contains(x.GetType()));
        }

        protected virtual FsmState GetState(string name)
        {
            return fsm.GetState(name);
        }

        protected virtual UnityEngine.Audio.AudioMixerSnapshot GetAudioMixerSnapshotFromState(HutongGames.PlayMaker.FsmState state)
        {
            return state.Actions.OfType<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>()
                .Cast<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>()
                .Where(x => x != null)
                .Where(x => x.snapshot.Value != null)
                .Select(x => x.snapshot.Value)
                .FirstOrDefault() as UnityEngine.Audio.AudioMixerSnapshot;
        }

        protected virtual int GetBattleEnemiesFromState(HutongGames.PlayMaker.FsmState state)
        {
            return state.Actions.OfType<HutongGames.PlayMaker.Actions.IntCompare>()
                .Cast<HutongGames.PlayMaker.Actions.IntCompare>()
                .Where(x => x != null)
                .Where(x => x.integer1.Name == "Battle Enemies")
                .Select(x => x.integer2.Value)
                .FirstOrDefault();
        }

        public IEnumerable<HutongGames.PlayMaker.FsmState> GetBattleWaveStates(HutongGames.PlayMaker.FsmState state)
        {
            return fsm.FsmStates
                        .Where(x => x.Actions
                                    .OfType<IntCompare>()
                                    .Cast<IntCompare>()
                                    .Where(y => y != null)
                                    .Any(y => y.integer1.Name == "Battle Enemies")
                               );
        }

        public int GetBattleEnemiesFromFsm()
        {
            return fsm.FsmStates.Sum(x => GetBattleEnemiesFromState(x));
        }

        public IEnumerable<CameraLockArea> GetCameraLocksFromScene(GameObject battleManagerOwner)
        {
            return battleManagerOwner.GetComponentsFromScene<CameraLockArea>();
        }

        public IEnumerable<MusicRegion> GetMusicRegionsFromScene(GameObject battleManagerOwner)
        {
            return battleManagerOwner.GetComponentsFromScene<MusicRegion>();
        }

        //may not even trigger in some cases...
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (InProgress)
                return;

            if (IsDone)
                return;

            if (StartOnTriggerEnter)
            {
                if (collision.tag == "Player")
                {
                    Begin(true, true);
                }
            }
        }
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




//use to add extra enemies to a battle
//public void AddEnemy(GameObject newEnemy, int? wave)
//{
//    //don't allow an enemy to be added to a previous wave
//    if((wave.HasValue && wave.Value < currentWave) || !wave.HasValue)
//    {
//        wave = currentWave;
//    }

//    if(waves[wave.Value] == null)
//    {
//        waves[wave.Value] = new List<GameObject>();
//    }

//    waves[wave.Value].Add(newEnemy);

//    if (isBattleActive && wave.Value == currentWave)
//    {
//        newEnemy.SetActive(true);
//    }
//    else
//    {
//        newEnemy.SetActive(false);
//    }
//}

//protected virtual void BuildGates(PlayMakerFSM source)
//{

//}

//protected virtual void BuildWaves(PlayMakerFSM source)
//{

//}

//public virtual void ReplaceEnemyInBattleManager(GameObject newEnemy, GameObject oldEnemy)
//{
//    //TODO: search the battle manager for the old enemy and update/place/configure the new one accordingly
//}}


//public void BeginBattle()
//{
//    isBattleActive = true;

//    //TODO: logic for starting the battle
//}