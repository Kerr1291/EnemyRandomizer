//using System.Collections.Generic;
//using System.Collections;
//using UnityEngine;
//using System.Linq;
//using HutongGames.PlayMaker.Actions;
//using HutongGames.PlayMaker;
//
//using UniRx;
//using UnityEngine.SceneManagement;
//using System;
//using UnityEngine.Events;

//namespace EnemyRandomizerMod
//{
//    public class BattleAction : IDisposable
//    {
//        public virtual void Setup(BattleState bs, Scene gameScene, FsmStateAction source) { }

//        public virtual void Invoke() { }

//        protected CompositeDisposable disposables = new CompositeDisposable();

//        public virtual void Dispose()
//        {
//            ((IDisposable)disposables).Dispose();
//        }
//    }

//    public class EventAction : BattleAction
//    {
//        public BattleState bs;
//        public string eventName;

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            this.bs = bs;
//        }
//    }
    

//    public class DoSendEventByName : EventAction
//    {
//        public Fsm fsm;

//        public override void Invoke()
//        {
//            var eventTarget = new FsmEventTarget();
//            eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;

//            fsm.Event(eventTarget, eventName);
//            bs.HandleTransitionEvent(eventName);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            base.Setup(bs, gameScene, source);
//            var src = (SendEventByName)source;
//            fsm = src.Fsm;
//            eventName = src.sendEvent.Value;
//        }
//    }

//    //public class DoSendEventByBool : EventAction
//    //{
//    //    public bool boolName;//todo
//    //    public Fsm fsm;

//    //    public override void Invoke()
//    //    {
//    //        var eventTarget = new FsmEventTarget();
//    //        eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;

//    //        fsm.Event(eventTarget, eventName);
//    //        bs.HandleTransitionEvent(eventName);
//    //    }

//    //    public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//    //    {
//    //        base.Setup(bs, gameScene, source);
//    //        var src = (PlayerDataBoolTest)source;
//    //        fsm = src.Fsm;
//    //        eventName = src.isTrue.Name;
//    //    }
//    //}

//    public class DoTransitionToAudioSnapshot : BattleAction
//    {
//        public UnityEngine.Audio.AudioMixerSnapshot snapshot;
//        public float time;

//        public override void Invoke()
//        {
//            if (snapshot != null)
//                snapshot.TransitionTo(3f);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            base.Setup(bs, gameScene, source);
//            var src = (TransitionToAudioSnapshot)source;

//            snapshot = src.snapshot.Value as UnityEngine.Audio.AudioMixerSnapshot;
//            time = src.transitionTime.Value;
//        }
//    }

//    public class DoWait : EventAction
//    {
//        public float timeToWait;

//        public override void Invoke()
//        {
//            Observable.Timer(System.TimeSpan.FromSeconds(timeToWait)).Subscribe(x => bs.HandleTransitionEvent(eventName)).AddTo(disposables);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            base.Setup(bs, gameScene, source);
//            var src = (Wait)source;
//            timeToWait = src.time.Value;
//            eventName = src.finishEvent.Name;
//        }
//    }

//    public class DoTrigger2dEvent : EventAction
//    {
//        public bool active = false;

//        public override void Invoke()
//        {
//            active = true;
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            base.Setup(bs, gameScene, source);
//            var src = (Trigger2dEvent)source;
//            eventName = src.sendEvent.Name;

//            BattleManager.StateMachine.Value.PlayerEnteredTrigger.Where(x => x == true).Subscribe(_ =>
//            {
//                if(active)
//                    bs.HandleTransitionEvent(eventName);
//            });
//        }
//    }

//    public class DoIntCompare : BattleAction
//    {
//        public BattleState bs;
//        public int value;

//        public string equal;
//        public string lessthan;
//        public string greaterthan;

//        public bool active = false;

//        public override void Invoke()
//        {
//            active = true;
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            this.bs = bs;

//            var src = (IntCompare)source;
//            value = src.integer2.Value;
//            equal = src.equal.Name;
//            lessthan = src.lessThan.Name;
//            greaterthan = src.greaterThan.Name;

//            BattleManager.StateMachine.Value.BattleEnemies.SkipLatestValueOnSubscribe().Subscribe(x =>
//            {
//                if (!active)
//                    return;

//                active = false;

//                if (x < value)
//                {
//                    bs.HandleTransitionEvent(lessthan);
//                }
//                if (x > value)
//                {
//                    bs.HandleTransitionEvent(greaterthan);
//                }
//                if (x == value)
//                {
//                    bs.HandleTransitionEvent(equal);
//                }
//            }).AddTo(disposables);
//        }
//    }


//    public class DoSetInvincible : BattleAction
//    {
//        string originalNamePath;
//        bool stateToSet;

//        public override void Invoke()
//        {
//            var go = GameObject.FindObjectsOfType<ManagedObject>().FirstOrDefault(x => x.originalGameObjectPath == originalNamePath);
//            if (go == null)
//                return;
//            var hm = go.GetComponent<HealthManager>();
//            if (hm != null)
//                hm.IsInvincible = stateToSet;
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            var src = (SetInvincible)source;
//            originalNamePath = src.target.GameObject.Value.GetSceneHierarchyPath();
//            stateToSet = src.Invincible.Value;
//        }
//    }

//    public class DoTakeDamage : BattleAction
//    {
//        string originalNamePath;
//        HitInstance hit;

//        public override void Invoke()
//        {
//            var go = GameObject.FindObjectsOfType<ManagedObject>().FirstOrDefault(x => x.originalGameObjectPath == originalNamePath);
//            if (go == null)
//                return;

//            HitTaker.Hit(go.gameObject, hit, 3);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            var src = (TakeDamage)source;
//            originalNamePath = src.Target.Value.GetSceneHierarchyPath();

//            hit = new HitInstance
//            {
//                Source = BattleManager.Instance.Value.gameObject,
//                AttackType = (AttackTypes)src.AttackType.Value,
//                CircleDirection = src.CircleDirection.Value,
//                DamageDealt = src.DamageDealt.Value,
//                Direction = src.Direction.Value,
//                IgnoreInvulnerable = src.IgnoreInvulnerable.Value,
//                MagnitudeMultiplier = src.MagnitudeMultiplier.Value,
//                MoveAngle = src.MoveAngle.Value,
//                MoveDirection = src.MoveDirection.Value,
//                Multiplier = (src.Multiplier.IsNone ? 1f : src.Multiplier.Value),
//                SpecialType = (SpecialTypes)src.SpecialType.Value,
//                IsExtraDamage = false
//            };
//        }
//    }


//    public class BattleObjectAction : BattleAction
//    {
//        public string path;
//    }

//    public partial class BattleWave : BattleAction
//    {
//        public int WaveIndex { get; protected set; }
//        public Scene BattleScene { get; protected set; }
//        public ReactiveProperty<int> EnemiesDead { get; protected set; }
//        public UnityEvent<BattleWave> OnWaveStart { get; protected set; }
//        public UnityEvent<BattleWave> OnWaveNext { get; protected set; }
//        public UnityEvent<BattleWave> OnWaveEnd { get; protected set; }
//        public bool WaveStarted { get; protected set; }
//        public bool WaveEnded { get; protected set; }
//        public ReadOnlyReactiveProperty<int> EnemiesRemaining { get; protected set; }
//        public HashSet<BattleManagedObject> ActiveWaveEnemies { get; protected set; }
//        public int EnemiesRemainingToTriggerNext { get; protected set; }

//        public ReactiveProperty<int> Total { get; protected set; }
//        public int WaveSizeOnSetup { get { return defaultEnemies.Count + waveEnemies.Count; } }

//        public Dictionary<int,BattleWave> SubWaves = new Dictionary<int, BattleWave>();
//        public BattleWave ParentWave { get; set; }

//        protected HashSet<BattleManagedObject> defaultEnemies = new HashSet<BattleManagedObject>();
//        protected HashSet<BattleManagedObject> waveEnemies = new HashSet<BattleManagedObject>();

//        public bool instantActivation = true;

//        public override void Invoke()
//        {
//            if (instantActivation)
//            {
//                defaultEnemies.Concat(waveEnemies).Where(x => x != null).Distinct().ToList().ForEach(x => x.gameObject.SetActive(true));
//            }
//            else
//            {

//            }

//            this.OnWaveStart.Invoke(this);
//        }

//        public int GetWaveIndexFromStateName(string name)
//        {
//            if (!name.Contains("Wave"))
//                return -1;

//            var splitname = name.Split(' ');
//            var last = splitname.Last();
//            if(int.TryParse(last, out int result))
//            {
//                return result;
//            }

//            return -1;
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            BattleScene = gameScene;
//            string stateName = bs.name;
//            if(source is SetFsmGameObject)
//            {
//                var src = (SetFsmGameObject)source;
//                if(WaveIndex < 0)
//                {
//                    WaveIndex = GetWaveIndexFromStateName(source.Name);
//                }

//                if (WaveIndex < 0)
//                {
//                    WaveIndex = StateMachines.StateNameToBattleWave[stateName];
//                }
//            }
//            else if(source is SetIntValue)
//            {
//                var src = (SetIntValue)source;
//                if (WaveIndex < 0)
//                {
//                    WaveIndex = GetWaveIndexFromStateName(source.Name);
//                }

//                if (WaveIndex < 0)
//                {
//                    WaveIndex = StateMachines.StateNameToBattleWave[stateName];
//                }
//            }

//            if(WaveIndex < 0)
//            {
//                Dev.LogError("Wave index cannot be negative when setting up a default wave!");
//            }

//            if(BattleManager.StateMachine.Value.BattleWaves.ContainsKey(WaveIndex))
//            {
//                Dev.LogError("Cannot create a duplicate battle wave! This should never happen...");
//            }

//            BattleManager.StateMachine.Value.BattleWaves.Add(WaveIndex, this);
//        }

//        protected virtual void OnWaveEnding(BattleWave wave)
//        {
//            //do subwaves exist?
//            if(SubWaves.Count > 0)
//            {
//                //order them by index
//                var waves = SubWaves.Select(x => x.Value).OrderBy(x => x.WaveIndex).ToList();

//                //if there's more than 1, link them
//                if(SubWaves.Count > 1)
//                {
//                    for(int i = waves.Count-1; i > 0; --i)
//                    {
//                        waves[i-1].OnWaveEnd.AsObservable().Subscribe(x => waves[i].Invoke()).AddTo(disposables);
//                    }
//                }

//                //set the last subwave to trigger the parent wave end
//                waves[waves.Count - 1].OnWaveEnd.AsObservable().Subscribe(x => OnWaveEnd.Invoke(this)).AddTo(disposables);

//                //start the subwaves
//                waves[0].Invoke();

//                //if there are subwaves, make sure to update the battle enemies counter when we reach zero
//                OnWaveEnd.AsObservable().Subscribe(_ => BattleManager.StateMachine.Value.BattleEnemies.Value = 0).AddTo(disposables);
//            }
//            else
//            {
//                OnWaveEnd.Invoke(this);
//            }
//        }

//        protected virtual void OnWaveStarting(BattleWave wave)
//        {
//            if(SubWaves.Count > 0)
//            {
//                BattleManager.StateMachine.Value.BattleEnemies.Value = Total.Value + SubWaves.Sum(x => x.Value.Total.Value);
//                //yes... we're not updating the battle enemies counter when subwaves are involved until all subwaves are complete
//            }
//            else
//            {
//                BattleManager.StateMachine.Value.BattleEnemies.Value = Total.Value;
//                if(ParentWave == null)
//                    EnemiesRemaining.SkipLatestValueOnSubscribe().Subscribe(x => BattleManager.StateMachine.Value.BattleEnemies.Value = x).AddTo(disposables);
//            }

//            WaveStarted = true;
//        }


//        public BattleWave()
//        {
//            Total = new ReactiveProperty<int>(0);
//            WaveIndex = -1;
//            EnemiesDead = new ReactiveProperty<int>(0);
//            WaveStarted = false;
//            OnWaveStart = new UnityEvent<BattleWave>();
//            OnWaveEnd = new UnityEvent<BattleWave>();

//            OnWaveStart.AsObservable().Subscribe(x => OnWaveStarting(x)).AddTo(disposables);
//            OnWaveEnd.AsObservable().Subscribe(_ => WaveEnded = true).AddTo(disposables);
//            EnemiesRemaining = EnemiesDead.SkipLatestValueOnSubscribe().Select(x => Total.Value - x).ToReadOnlyReactiveProperty();
//            EnemiesRemaining.SkipLatestValueOnSubscribe().Where(x => x == 0).Subscribe(_ => OnWaveEnding(this)).AddTo(disposables);
            
//            OnWaveNext = new UnityEvent<BattleWave>();
//            EnemiesRemainingToTriggerNext = 0;
//            EnemiesRemaining.SkipLatestValueOnSubscribe().Where(x => x == EnemiesRemainingToTriggerNext).Subscribe(_ => OnWaveNext.Invoke(this)).AddTo(disposables);
//        }

//        public BattleWave(int windex, Scene bscene)
//        {
//            Total = new ReactiveProperty<int>(0);
//            WaveIndex = windex;
//            BattleScene = bscene;
//            EnemiesDead = new ReactiveProperty<int>(0);
//            WaveStarted = false;
//            OnWaveStart = new UnityEvent<BattleWave>();
//            OnWaveEnd = new UnityEvent<BattleWave>();
//            OnWaveStart.AsObservable().Subscribe(x => OnWaveStarting(x)).AddTo(disposables);
//            OnWaveEnd.AsObservable().Subscribe(_ => WaveEnded = true).AddTo(disposables);
//            EnemiesRemaining = EnemiesDead.SkipLatestValueOnSubscribe().Select(x => Total.Value - x).ToReadOnlyReactiveProperty();
//            EnemiesRemaining.SkipLatestValueOnSubscribe().Where(x => x == 0).Subscribe(_ => OnWaveEnding(this)).AddTo(disposables);

//            OnWaveNext = new UnityEvent<BattleWave>();
//            EnemiesRemainingToTriggerNext = 0;
//            EnemiesRemaining.SkipLatestValueOnSubscribe().Where(x => x == EnemiesRemainingToTriggerNext).Subscribe(_ => OnWaveNext.Invoke(this)).AddTo(disposables);
//        }
//    }

//    public class DestroyGameObject : BattleObjectAction
//    {
//        public override void Invoke()
//        {
//            StateMachines.TryDestroyGameObjectAtPath(path);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            var src = (DestroyObject)source;
//            path = src.gameObject.Value.GetSceneHierarchyPath();
//        }
//    }

//    public class DisableGameObject : BattleObjectAction
//    {
//        public override void Invoke()
//        {
//            StateMachines.TryDisableGameObjectAtPath(path);
//        }
//    }

//    public class EnableGameObject : BattleObjectAction
//    {
//        bool activate = false;

//        public override void Invoke()
//        {
//            if (activate)
//                StateMachines.TryEnableGameObjectAtPath(path);
//            else
//                StateMachines.TryDisableGameObjectAtPath(path);
//        }

//        public override void Setup(BattleState bs, Scene gameScene, FsmStateAction source)
//        {
//            var src = (ActivateGameObject)source;

//            this.activate = src.activate.Value;

//            if(src.gameObject.GameObject.Value == null)
//            {
//                if(src.gameObject.GameObject.Name.Contains("Camera Lock"))
//                {
//                    if(gameScene.name.Contains("Crossroads_08"))
//                    {
//                        this.path = GameObject.Find("CameraLockArea B").GetSceneHierarchyPath();
//                    }
//                    if (gameScene.name.Contains("Crossroads_09"))
//                    {
//                        if(source.Name.Contains("NB"))
//                            this.path = GameObject.Find("CameraLockArea NB").GetSceneHierarchyPath();
//                        else
//                            this.path = GameObject.Find("CameraLockArea B").GetSceneHierarchyPath();
//                    }
//                }
//            }
//            else
//            {
//                path = src.gameObject.GameObject.Value.GetSceneHierarchyPath();
//            }
//        }
//    }

//    public class GameAction : BattleAction
//    {
//        public UnityEvent action;

//        public override void Invoke()
//        {
//            action.Invoke();
//        }
//    }

//    public partial class BattleState : IDisposable
//    {
//        public string name { get; set; }
//        public Scene scene { get; set; }

//        public List<BattleAction> actions { get; set; }

//        public ReactiveProperty<bool> complete { get; protected set; }

//        public Dictionary<string,string> Transitions;

//        public bool IsEndState { get; protected set; }

//        public UnityEvent<BattleState> onInvoke;

//        public BattleState()
//        {
//            onInvoke = new UnityEvent<BattleState>();
//            actions = new List<BattleAction>();
//            complete = new ReactiveProperty<bool>(false);
//            Transitions = new Dictionary<string, string>();
//        }

//        public void Invoke()
//        {
//            if (onInvoke != null)
//                onInvoke.Invoke(this);

//            for(int i = 0; i < actions.Count; ++i)
//            {
//                actions[i].Invoke();
//                if (complete.Value)
//                    return;
//            }

//            //hack to make the detect work on this boss room -- brooding mawlek
//            if (scene.name.Contains("Crossroads_09") && name.Contains("Detect"))
//            {
//                var go = GameObjectExtensions.FindGameObject("_Enemies/Mawlek Body/Alert Range New");
//                var c = go.GetComponent<BoxCollider2D>();
//                BattleManager.Instance.Value.transform.position = go.transform.position;
//                var bgo = BattleManager.Instance.Value.gameObject;
//                var b = bgo.AddComponent<BoxCollider2D>();
//                b.isTrigger = c.isTrigger;
//                bgo.layer = go.layer;
//                b.size = c.size;
//                b.offset = c.offset;
//                b.tag = c.tag;

//                if (actions.Count <= 0)
//                {
//                    BattleManager.StateMachine.Value.PlayerEnteredTrigger.Where(x => x == true).Subscribe(_ =>
//                    {
//                        HandleTransitionEvent("Start");
//                    });
//                }

//                return;
//            }

//            if (IsEndState)
//            {
//                complete.Value = true;
//                BattleManager.StateMachine.Value.End();
//                return;
//            }

//            if (!complete.Value)
//            {
//                HandleTransitionEvent(FsmEvent.Finished.Name);
//            }

//            if(!complete.Value && actions.Count <= 0 && Transitions.Count > 0)
//            {
//                HandleTransitionEvent(Transitions.First().Key);
//            }
//        }

//        public void Dispose()
//        {
//            actions?.ForEach(x => x?.Dispose());
//        }

//        public void HandleTransitionEvent(string name)
//        {
//            if(Transitions.ContainsKey(name))
//            {
//                complete.Value = true;
//                BattleManager.StateMachine.Value.PlayState(name);
//            }
//        }
//    }

//    public partial class BattleStateMachine : IDisposable
//    {
//        public BattleStateMachine(Scene scene, PersistentBoolItem item, PlayMakerFSM fsm)
//        {
//            BattleScene = scene;
//            IsBattleComplete = item;
//            OnComplete = new UnityEvent();
//            States = new Dictionary<string, BattleState>();

//            cameraLocks = StateMachines.GetCameraLocksFromScene(fsm.gameObject).ToList();
//            battleMusicRegion = StateMachines.GetMusicRegionsFromScene(fsm.gameObject).FirstOrDefault();
//            PlayerEnteredTrigger = new ReactiveProperty<bool>(false);

//            BattleWaves = new Dictionary<int, BattleWave>();
//            BattleEnemies = new ReactiveProperty<int>(0);
//        }

//        public Scene BattleScene { get; set; }
//        public string SceneName { get { return BattleScene.name; } }
//        public PersistentBoolItem IsBattleComplete { get; protected set; }
//        public UnityEvent OnComplete { get; protected set; }
//        public Dictionary<string, BattleState> States { get; protected set; }

//        public MusicRegion battleMusicRegion { get; protected set; }
//        public List<CameraLockArea> cameraLocks { get; protected set; }

//        public ReactiveProperty<bool> PlayerEnteredTrigger { get; protected set; }
//        public bool InProgress { get; protected set; }
//        public Dictionary<int, BattleWave> BattleWaves { get; protected set; }

//        public BattleState InitState { get; protected set; }

//        public ReactiveProperty<int> BattleEnemies { get; protected set; }

//        public void PlayState(string name)
//        {
//            if(States.ContainsKey(name))
//            {
//                States[name].Invoke();
//            }
//        }

//        public void End()
//        {
//            MarkPersistantBoolComplete();
//            StopMusic();
//            UnlockCameras();
//            OnComplete?.Invoke();
//        }

//        public void Dispose()
//        {
//            States.Values.ToList().ForEach(x => x.Dispose());
//            States.Clear();
//        }

//        public static BattleStateMachine Create(Scene scene, PlayMakerFSM fsm, PersistentBoolItem item)
//        {
//            BattleStateMachine bsm = new BattleStateMachine(scene, item, fsm);
//            return bsm;
//        }

//        public void Build(PlayMakerFSM fsm)
//        {
//            var init = StateMachines.GetInitState(fsm);
//            var battleStates = StateMachines.GetBattleStates(fsm, init);

//            InitState = BattleState.Build(BattleScene, init);

//            InitState.onInvoke.AsObservable().Subscribe(_ => InProgress = true);

//            States = battleStates.Select(x => BattleState.Build(BattleScene, x)).ToDictionary(x => x.name);
//            States.Add(InitState.name, InitState);
//        }
//    }


//    public static partial class StateMachines
//    {
//        public static Dictionary<System.Type, System.Type> PtoA = new Dictionary<Type, Type>()
//        {
//            { typeof(DestroyObject),                    typeof(DestroyGameObject) },
//            { typeof(ActivateGameObject),               typeof(EnableGameObject)  },
//            { typeof(IntCompare),                       typeof(DoIntCompare)  },
//            { typeof(SetInvincible),                    typeof(DoSetInvincible)  },
//            { typeof(TakeDamage),                       typeof(DoTakeDamage)  },
//            { typeof(SendEventByName),                  typeof(DoSendEventByName)  },
//            //{ typeof(PlayerDataBoolTest),                  typeof(DoSendEventByBool)  },
//            { typeof(Trigger2dEvent),                   typeof(DoTrigger2dEvent)  },
//            { typeof(Wait),                             typeof(DoWait)  },
//            { typeof(TransitionToAudioSnapshot),        typeof(DoTransitionToAudioSnapshot)  },

//            { typeof(SetIntValue),          typeof(BattleWave)  },
//            { typeof(SetFsmGameObject),     typeof(BattleWave)  },

//        };

//        public static Dictionary<string, int> StateNameToBattleWave = new Dictionary<string, int>()
//        {
//            {"Start", 0},
//            {"Init", 0},
//        };

//        //these are states we don't care to scrape for data (the starting state, usually init) is also included
//        public static string[] BadStates = new string[]
//        {
//            "Pause",
//            "PrePause",
//            "Activate",
//            "Activated"
//        };

//        //skip these ones
//        public static System.Type[] BadActions = new System.Type[]
//        {
//            typeof(HutongGames.PlayMaker.Actions.BoolTest),
//            typeof(HutongGames.PlayMaker.Actions.FindChild),
//            typeof(HutongGames.PlayMaker.Actions.GetOwner),
//            typeof(HutongGames.PlayMaker.Actions.FindGameObject),
//            typeof(HutongGames.PlayMaker.Actions.SetFsmBool),
//            typeof(HutongGames.PlayMaker.Actions.ActivateAllChildren),
//            typeof(HutongGames.PlayMaker.Actions.SetFsmGameObject), //TODO: write some code to emit the reward item of a boss
//        };

//        public static Dictionary<string, int> BattleWaveMap = new Dictionary<string, int>()
//        {
//            //Crossroads_10_boss
//            {@"Battle Scene/False Knight New", 0},

//            //Abyss_17
//            //{@"Battle Scene Ore", 0},
            
//            //Crossroads_04
//            {@"_Enemies/Giant Fly", 0},
//            {@"_Enemies/Fly Spawn", -1},

//            //crossroads 09
//            {$"_Enemies/Mawlek Body",0},
//        };
//    }

//    //public static partial class BattleGates
//    //{
//    //    public static Dictionary<string, List<string>> SceneGates = new Dictionary<string, List<string>>()
//    //    {
//    //        {
//    //            "Crossroads_10", new List<string>()
//    //            {
//    //                @"Battle Gate 2 (1)",
//    //                @"Battle Gate 3",
//    //                @"Battle Gate",
//    //            }
//    //        },
//    //    };
//    //}
//}
