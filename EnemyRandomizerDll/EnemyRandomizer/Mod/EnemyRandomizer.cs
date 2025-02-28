﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Linq;
using UniRx;
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEngine.UI;
using Satchel.BetterMenus;
using UnityEngine.Events;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using Satchel;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod,// ITogglableMod, (currently broken) 
        IGlobalSettings<EnemyRandomizerSettings>, ILocalSettings<EnemyRandomizerPlayerSettings>, IDisposable
    {
        public static EnemyRandomizer Instance 
        {
            get
            {
                if (instance == null)
                    instance = new EnemyRandomizer();
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        static EnemyRandomizer instance;

        //true when the mod is disabled (but its data remains loaded...)
        public static bool isDisabled;

        //true during initialize when the mod is loading after the first time
        public static bool isReloading;

        //if true, will skip all logic on the next spawn and create a vanilla enemy
        public static bool bypassNextRandomization;

        //if true will allow an enemy spawned to run through modules that modify but do not replace the enemy
        public static bool bypassNextReplacement;
        
        //if bypass next replacement is true, this will be checked to pull a custom replacement, if desired
        public static string debugCustomReplacement = null;

        //use this to test menus and test module loading -- will skip the loading of all preloads
        public static bool DEBUG_SKIP_LOADING = false;

        /// <summary>
        /// Set to true when the mod is finished loading
        /// </summary>
        public ReactiveProperty<bool> IsModLoaded { get; protected set; }

        /// <summary>
        /// Additional callback to hook into, invoked at the same time as IsModLoaded
        /// </summary>
        public UnityEvent<EnemyRandomizer> OnModLoaded { get; protected set; }

        public override string GetVersion()
        {
            return currentVersion;
        }

        public override int LoadPriority()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes((long)(0xDEADBEEF)), 0);
        }

        public static string ModAssetPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(EnemyRandomizer).Assembly.Location);
            }
        }

        public static string GetModAssetPath(string filename) 
        {
            return Path.Combine(ModAssetPath, filename);
        }

        //Settings objects provided by the mod base class
        public static EnemyRandomizerSettings GlobalSettings = new EnemyRandomizerSettings();
        public void OnLoadGlobal(EnemyRandomizerSettings s) => GlobalSettings = s;
        public EnemyRandomizerSettings OnSaveGlobal() => GlobalSettings;

        public static EnemyRandomizerPlayerSettings PlayerSettings = new EnemyRandomizerPlayerSettings();
        public void OnLoadLocal(EnemyRandomizerPlayerSettings s) => PlayerSettings = s;
        public EnemyRandomizerPlayerSettings OnSaveLocal() => PlayerSettings;

        const string defaultDatabaseFilePath = "EnemyRandomizerDatabase.xml";
        static string currentVersionPrefix = Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString() + "[Alpha 9.2.4]";
        static string currentVersion = currentVersionPrefix;
            //Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString() + $" CURRENT SEED:[{GlobalSettings.seed}] -- TO CHANGE SEED --> MODS > ENEMY RANDOMIZER > ENEMY RANDOMIZER MODULES";

        protected static string GetVersionString()
        {
            string prefix = currentVersionPrefix;
            string postfix = string.Empty;// "\n\t\tTO CHANGE --> OPTIONS > MODS > ENEMY RANDOMIZER OPTIONS > ENEMY RANDOMIZER MODULES";

            string seedInfo;

            if(GlobalSettings.UseCustomSeed)
                seedInfo = prefix + $" USING CUSTOM SEED:[{GlobalSettings.seed}] -- " + postfix;
            else
                seedInfo = prefix + $" NEW GAME WILL GENERATE NEW SEED -- " + postfix;

            return string.Empty;
        }


        public EnemyReplacer enemyReplacer = new EnemyReplacer();

        public Dictionary<string, IRandomizerLogic> logicTypes;

        public static ReactiveProperty<List<GameObject>> BlackBorders { get; protected set; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        public EnemyRandomizer()
            : base("Enemy Randomizer")
        {
            Instance = this;
            IsModLoaded = new ReactiveProperty<bool>(false);
            OnModLoaded = new UnityEvent<EnemyRandomizer>();
            IsModLoaded.SkipLatestValueOnSubscribe().Where(x => x == true).Subscribe(_ => OnModLoaded?.Invoke(this)).AddTo(disposables);
            ModHooks.FinishedLoadingModsHook -= ModHooks_FinishedLoadingModsHook;
            ModHooks.FinishedLoadingModsHook += ModHooks_FinishedLoadingModsHook;

#if DEBUG
            {
                Dev.Logger.LoggingEnabled = true;
                Dev.Logger.GuiLoggingEnabled = true;
                DevLogger.Instance.ShowSlider();
                DevLogger.Instance.Show(true);

                //hide the debug window
                var logHider = Observable.Timer(TimeSpan.FromSeconds(2f)).DoOnCompleted(() =>
                {
                    DevLogger.Instance.Show(false);
                    DevLogger.Instance.Show(false);
                }).Subscribe();
            }
#else
            Dev.Logger.LoggingEnabled = true;
            Dev.Logger.GuiLoggingEnabled = false;
#endif
            Dev.Log("Created " + currentVersion);
        }

        protected virtual void ModHooks_FinishedLoadingModsHook()
        {
            SetIsLoaded();
            ModHooks.FinishedLoadingModsHook -= ModHooks_FinishedLoadingModsHook;
        }

        protected virtual void ResetToDefaults()
        {
            try
            {
                {
                    GlobalSettings.balanceReplacementHP = true;
                    GlobalSettings.randomizeReplacementGeo = true;
                    GlobalSettings.allowCustomEnemies = true;
                    GlobalSettings.allowEnemyRandoExtras = true;
                    GlobalSettings.UseCustomSeed = false;
                    GlobalSettings.UseCustomColoSeed = false;
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Zote Mode"];
                        EnemyRandomizer.instance.enemyReplacer.DisableLogic(logic);
                        RootMenuObject.Find(logic.Name).Hide();
                    }
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Enemy Filter"];
                        EnemyRandomizer.instance.enemyReplacer.DisableLogic(logic);
                        RootMenuObject.Find(logic.Name).Hide();
                    }
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Enemy Enabler"];
                        EnemyRandomizer.instance.enemyReplacer.DisableLogic(logic);
                        RootMenuObject.Find(logic.Name).Hide();
                    }
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Replacement Logic"];
                        logic.Settings.GetOption("Randomize Enemies").value = true;
                        logic.Settings.GetOption("Randomize Hazards").value = false;
                        logic.Settings.GetOption("Randomize Effects").value = false;
                        logic.Settings.GetOption("Use basic replacement matching?").value = false;
                        logic.Settings.GetOption("Allow bad replacements?").value = false;
                        EnemyRandomizer.instance.enemyReplacer.EnableLogic(logic);
                        RootMenuObject.Find(logic.Name).Show();
                    }
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Randomization Modes"];
                        logic.Settings.GetOption("Object").value = true;
                        logic.Settings.GetOption("Transition").value = false;
                        logic.Settings.GetOption("Room").value = false;
                        logic.Settings.GetOption("Zone").value = false;
                        logic.Settings.GetOption("Type").value = false;
                        EnemyRandomizer.instance.enemyReplacer.EnableLogic(logic);
                        RootMenuObject.Find(logic.Name).Show();
                    }
                    {
                        var logic = EnemyRandomizer.instance.logicTypes["Enemy Size Changer"];
                        logic.Settings.GetOption("Match Audio to Scaling").value = true;
                        logic.Settings.GetOption("Match").value = true;
                        logic.Settings.GetOption("Random").value = false;
                        logic.Settings.GetOption("All Big").value = false;
                        logic.Settings.GetOption("All Tiny").value = false;
                        EnemyRandomizer.instance.enemyReplacer.EnableLogic(logic);
                        RootMenuObject.Find(logic.Name).Show();
                    }
                }
            }
            catch (Exception e) { }//later..
        }

        public override List<(string, string)> GetPreloadNames()
        {
            if (DEBUG_SKIP_LOADING)
                return new List<(string, string)>(); 

            return enemyReplacer.GetPreloadNames(defaultDatabaseFilePath);
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (instance == null)
                instance = this;

            BlackBorders = new ReactiveProperty<List<GameObject>>();

            if (preloadedObjects != null && !DEBUG_SKIP_LOADING)
            {
                if (preloadedObjects.Count <= 0)
                {
                    Modding.Logger.LogError("Enemy randomizer got zero preloaded objects. Mod will not work.");
                    throw new ArgumentException("Enemy Randomizer failed to load because it was not provided with any enemy prefabs, typically because the database failed to load for some reason.", "preloadedObjects");
                }
            }

            base.Initialize(preloadedObjects);


            if (isDisabled)
            {
                //passed in when reloading
                isReloading = preloadedObjects == null;
            }

            if (!isReloading)
            {
                BattleManager.Init();
                if (!DEBUG_SKIP_LOADING)
                {
                    enemyReplacer.Setup(preloadedObjects);

                    if (EnemyRandomizer.Instance.logicTypes == null)
                        EnemyRandomizer.Instance.logicTypes = LogicLoader.LoadLogics();
                    
                    enemyReplacer.ConstructLogics(GlobalSettings.loadedLogics, logicTypes);

                }
            }

            EnableMod();

            //UIManager.instance.StartCoroutine(UpdateLabelOnLoad());
        }


        void RegisterCallbacks()
        {
            ModHooks.SavegameLoadHook -= MODHOOK_LoadFromSave;
            ModHooks.SavegameLoadHook += MODHOOK_LoadFromSave;

            ModHooks.BeforeSavegameSaveHook -= MODHOOK_SavegameSaveHook;
            ModHooks.BeforeSavegameSaveHook += MODHOOK_SavegameSaveHook;

            ModHooks.NewGameHook -= MODHOOK_NewGameHook;
            ModHooks.NewGameHook += MODHOOK_NewGameHook;

            ModHooks.OnReceiveDeathEventHook -= MODHOOK_RecieveDeathEvent;
            ModHooks.OnReceiveDeathEventHook += MODHOOK_RecieveDeathEvent;

            ModHooks.ColliderCreateHook -= ModHooks_ColliderCreateHook;
            ModHooks.ColliderCreateHook += ModHooks_ColliderCreateHook;

            ModHooks.OnEnableEnemyHook -= MODHOOK_OnHealthManagerObjectEnabled;
            ModHooks.OnEnableEnemyHook += MODHOOK_OnHealthManagerObjectEnabled;

            ModHooks.ObjectPoolSpawnHook -= MODHOOK_OnObjectPoolSpawn;
            ModHooks.ObjectPoolSpawnHook += MODHOOK_OnObjectPoolSpawn;

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;

            GameManager.instance.UnloadingLevel -= Instance_UnloadingLevel;
            GameManager.instance.UnloadingLevel += Instance_UnloadingLevel;

#if DEBUG
            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
            ModHooks.SlashHitHook += DebugPrintObjectOnHit;
#endif
            ModHooks.SlashHitHook -= MODHOOK_SlashHitHook;
            ModHooks.SlashHitHook += MODHOOK_SlashHitHook;

            //override to set if an object is semiPersistent or not
            On.PersistentBoolItem.Awake -= ONHOOK_PersistentBoolItem_OnAwake;
            On.PersistentBoolItem.Awake += ONHOOK_PersistentBoolItem_OnAwake;

            //override this to use the proper unique scene/key for the randomized enemy so that it always references the same ID
            //do that by setting the "persistentBoolData" on the object
            On.PersistentBoolItem.SetMyID -= ONHOOK_PersistentBoolItem_SetMyID;
            On.PersistentBoolItem.SetMyID += ONHOOK_PersistentBoolItem_SetMyID;

            //hook into this to grab important FSM objects like the colo/boss/battle managers
            On.PlayMakerFSM.OnEnable -= ONHOOK_PlayMakerFSM_OnEnable;
            On.PlayMakerFSM.OnEnable += ONHOOK_PlayMakerFSM_OnEnable;

            //hook into this to replace hazards
            On.DamageHero.OnEnable -= ONHOOK_DamageHero_OnEnable;
            On.DamageHero.OnEnable += ONHOOK_DamageHero_OnEnable;

            ModHooks.DrawBlackBordersHook -= ModHooks_DrawBlackBordersHook;
            ModHooks.DrawBlackBordersHook += ModHooks_DrawBlackBordersHook;

            On.HeroController.Start += (orig, self) =>
            {
                orig(self);
                GeneralOptionsMenu.Find("SeedInput").Hide();
                //UpdateModVersionLabel();
            };

            // can remove after testing
            On.QuitToMenu.Start += (orig, self) =>
            {
                if (GlobalSettings.UseCustomSeed)
                {
                    GeneralOptionsMenu?.Find("SeedInput")?.Show();
                }
                //UpdateModVersionLabel();
                return orig(self);
            };
        }

        private void ModHooks_ColliderCreateHook(GameObject obj)
        {
            try
            {
                if (obj != null && obj.name.Contains("Health Scuttler") && obj.GetComponent<SpawnedObjectControl>() == null && ObjectMetadata.Get(obj) == null)
                {
                    //test.....
                    RandomizeEnemy(obj);
                }
                else if (obj != null && obj.name.Contains("Jelly Egg Bomb") && obj.GetComponent<SpawnedObjectControl>() == null && ObjectMetadata.Get(obj) == null)
                {
                    //test.....
                    RandomizeEnemy(obj);
                }
                else if (obj != null && obj.name.Contains("mines_stomper") && obj.transform.parent != null && obj.transform.parent.gameObject.GetComponent<SpawnedObjectControl>() == null && ObjectMetadata.Get(obj.transform.parent.gameObject) == null)
                {
                    //test.....
                    RandomizeEnemy(obj.transform.parent.gameObject);
                }
            }
            catch (Exception e) { Dev.Log($"Caught exception in ModHooks_ColliderCreateHook :::: \n{e.Message}\n{e.StackTrace}"); }
        }

        //IEnumerator UpdateLabelOnLoad()
        //{
        //    yield return new WaitUntil(() => GameObject.FindObjectOfType<ModVersionDraw>(true) != null);
        //    var mvd = GameObject.FindObjectOfType<ModVersionDraw>(true);
        //    yield return new WaitUntil(() => mvd.drawString.Contains(currentVersion));
        //    UpdateModVersionLabel();
        //}

        void ModHooks_DrawBlackBordersHook(List<GameObject> obj)
        {
            if (EnemyReplacer.VERBOSE_LOGGING)
            {
                Dev.Log("=======================================================================================");
                Dev.Log("==                                                                                   ==");
                Dev.Log("==  NEW SCENE HAS BEEN ENTERED              SCENE IS BEING LOADED                    ==");
                Dev.Log("==                                                                                   ==");
                Dev.Log("==                         SCENE BOARDERS LOADED                                     ==");
                Dev.Log("==                                                                                   ==");
                Dev.Log("VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV");
            }
            BlackBorders.Value = obj;
            if(BlackBorders != null && BlackBorders.Value != null && BlackBorders.Value.Count > 0)
            {
                BattleManager.DoSceneCheck(obj.First());
            }
        }

        void Instance_UnloadingLevel()
        {
            if (EnemyReplacer.VERBOSE_LOGGING)
            {
                Dev.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Dev.Log("==                                                                                   ==");
                Dev.Log("==  OLD SCENE EXITED                        OBJECT LOADING PAUSED                    ==");
                Dev.Log("==                                          SCENE BOARDERS CLEARED                   ==");
                Dev.Log("==  SCENE IS BEING UNLOADED                                                          ==");
                Dev.Log("==                                                                                   ==");
                Dev.Log("=======================================================================================");
            }
            try
            {
                if (BlackBorders.Value != null)
                    BlackBorders.Value.Clear();

                if (BattleManager.Instance.Value != null)
                    BattleManager.Instance.Value.Clear();
                enemyReplacer.ClearPendingLoads();
            }
            catch (Exception e)
            {
                Dev.LogError($"Error unloading level: MESSAGE:{e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        void UnRegisterCallbacks()
        {
            ModHooks.SavegameLoadHook -= MODHOOK_LoadFromSave;

            ModHooks.BeforeSavegameSaveHook -= MODHOOK_SavegameSaveHook;

            ModHooks.NewGameHook -= MODHOOK_NewGameHook;

            ModHooks.OnReceiveDeathEventHook -= MODHOOK_RecieveDeathEvent;

            ModHooks.OnEnableEnemyHook -= MODHOOK_OnHealthManagerObjectEnabled;

            ModHooks.ColliderCreateHook -= ModHooks_ColliderCreateHook;

            ModHooks.ObjectPoolSpawnHook -= MODHOOK_OnObjectPoolSpawn;

            //On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;

            GameManager.instance.UnloadingLevel -= Instance_UnloadingLevel;

#if DEBUG
            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
#endif
            ModHooks.SlashHitHook -= MODHOOK_SlashHitHook;

            On.PersistentBoolItem.Awake -= ONHOOK_PersistentBoolItem_OnAwake;
            On.PlayMakerFSM.OnEnable -= ONHOOK_PlayMakerFSM_OnEnable;
            On.PersistentBoolItem.SetMyID -= ONHOOK_PersistentBoolItem_SetMyID;
            On.DamageHero.OnEnable -= ONHOOK_DamageHero_OnEnable;

            ModHooks.DrawBlackBordersHook -= ModHooks_DrawBlackBordersHook;
        }

        //public void UpdateModVersionLabel()
        //{
        //    //TODO: put a condition here to add more info
        //    //after testing a few versions without this notice to see if it was useful or not
        //    return;
        //    //string olds = currentVersion;
        //    //string news = GetVersionString();
        //    ////Dev.Log($"{olds}   {news}    {currentVersion}");
        //    //currentVersion = news;
        //    //var mvd = GameObject.FindObjectOfType<ModVersionDraw>(true);
        //    //mvd.drawString = mvd.drawString.Replace(olds, news);
        //}

        public void EnableMod()
        {
#if DEBUG
            {
                Dev.Logger.GuiLoggingEnabled = true;
                DevLogger.Instance.ShowSlider();
                DevLogger.Instance.Show(true);
                Dev.Logger.LoggingEnabled = true;
            }
#endif
            RegisterCallbacks();
            if (!DEBUG_SKIP_LOADING)
            {
                enemyReplacer.OnModEnabled();
            }

            isReloading = false;
            isDisabled = false;
        }

        public void SetIsLoaded()
        {
            IsModLoaded.Value = true;
        }

        ///Revert all changes the mod has made
        public void Unload()
        {
            try
            {
                enemyReplacer.OnModDisabled();
            }
            catch(Exception e)
            {
                Dev.LogError($"Caught unhandled exception when trying to invoke OnModDisabled in Unload() for the enemy replacer {e.Message}\n{e.StackTrace}");
            }

            try
            {
                UnRegisterCallbacks();
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught unhandled exception when trying to invoke UnRegisterCallbacks in Unload() for enemy randomizer {e.Message}\n{e.StackTrace}");
            }

            isDisabled = true;

#if DEBUG
            {
                Dev.Logger.GuiLoggingEnabled = false;
                DevLogger.Instance.HideSlider();
                DevLogger.Instance.Show(false);
                Dev.Logger.LoggingEnabled = false;
            }
#endif
        }

        bool didCompatCheck = false;
        bool isRandomizerLoaded = false;

        protected virtual GameObject RandomizeEnemy(GameObject enemyObject)
        {
            DoBattleSceneCheck(enemyObject);

            if(!didCompatCheck)
            {
                didCompatCheck = true;
                isRandomizerLoaded = DoModCompatCheck();                
            }

            if(isRandomizerLoaded)
            {
                if (!CompatCheck(enemyObject))
                    return enemyObject;
            }

            return enemyReplacer.RandomizeEnemy(enemyObject);
        }

        protected virtual bool DoModCompatCheck()
        {
            return ModHooks.GetAllMods().Any(x => x.GetName().Contains("Randomizer") && !x.GetName().Contains("Enemy"));
        }

        protected virtual bool CompatCheck(GameObject enemyObject)
        {
            if(enemyObject.name.Contains("Gorgeous"))
            {
                return false;
            }
            else if(enemyObject.name.Contains("Egg Sac"))
            {
                return false;
            }

            return true;
        }

        protected virtual GameObject RandomizeHazard(DamageHero hazardObject)
        {
            var hm = hazardObject.GetComponent<HealthManager>();
            if (hm != null && hazardObject.gameObject.activeSelf)
            {
                return RandomizeEnemy(hazardObject.gameObject);
            }
            else
            {
                return enemyReplacer.RandomizeHazard(hazardObject);
            }
        }

        protected virtual GameObject RandomizePooledSpawn(GameObject pooledObject)
        {
            var dh = pooledObject.GetComponent<DamageHero>();
            if (dh != null && pooledObject.activeSelf)
            {
                return RandomizeHazard(dh);
            }
            else
            {
                return enemyReplacer.RandomizeEffect(pooledObject);
            }
        }

        protected virtual void SetGameSeed()
        {
            if (PlayerSettings.enemyRandomizerSeed <= 0)
            {
                if (GlobalSettings.UseCustomSeed && GlobalSettings.seed > -1)
                {
                    Dev.Log("Using the custom seed from global settings");
                    PlayerSettings.enemyRandomizerSeed = GlobalSettings.seed;
                }
                else
                {
                    Dev.Log("Generating new random seed for this save file");
                    RNG rng = new RNG();
                    rng.Reset();
                    PlayerSettings.enemyRandomizerSeed = rng.Rand(0, int.MaxValue);
                }
            }
            else
            {
                Dev.Log("Using the previously saved seed");
            }

            Dev.Log("USING SEED: " + PlayerSettings.enemyRandomizerSeed.ToString().Colorize(Color.magenta), Color.green);
        }

        void MODHOOK_LoadFromSave(int saveSlot)
        {
            if (!GlobalSettings.hasDoneAlpha92Reset)
            {
                GlobalSettings.DoAlpha9Reset();
                ResetToDefaults();
            }
            SetGameSeed();
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        void MODHOOK_SavegameSaveHook(SaveGameData data)
        {
            enemyReplacer.OnSaveGame(PlayerSettings);
        }

        void MODHOOK_NewGameHook()
        {
            if (!GlobalSettings.hasDoneAlpha92Reset)
            {
                GlobalSettings.DoAlpha9Reset();
                ResetToDefaults();
            }
            SetGameSeed();
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        GameObject MODHOOK_OnObjectPoolSpawn(GameObject originalGameObject)
        {
            var pooledGO = RandomizePooledSpawn(originalGameObject);

            //see if this pooled game object should me pooled by the randomizer
            //TODO:? look into this another time, for now just disable this this functionality since we're not using it
            //if (pooledGO != null && MetaDataTypes.RandoControlledPooling.Any(x => pooledGO.name.Contains(x)))
            //{
            //    //can't be doing this for everything, is causing nullrefs
            //    //want to do it for some things, maybe based on a list?
            //    pooledGO.GetOrAddComponent<RecycleOnDisable>();
            //    var auto = pooledGO.GetComponent<AutoRecycleSelf>();
            //    if (auto != null)
            //        GameObject.Destroy(auto);
            //}

            return pooledGO;
        }

        void MODHOOK_RecieveDeathEvent(
            EnemyDeathEffects enemyDeathEffects,
            bool eventAlreadyReceived,
            ref float? attackDirection,
            ref bool resetDeathEvent,
            ref bool spellBurn,
            ref bool isWatery
        )
        {
            if (eventAlreadyReceived)
                return;

            //enemyReplacer.OnEnemyDeathEvent(enemyDeathEffects.gameObject);
            BattleManager.OnEnemyDeathEvent(enemyDeathEffects.gameObject);
        }

        /// <summary>
        /// Return false if creating a new object, if true is returned then this is already dead
        /// </summary>
        bool MODHOOK_OnHealthManagerObjectEnabled(GameObject healthManagerObject, bool isAlreadyDead)
        {
            if (isAlreadyDead)
                return isAlreadyDead;

            if (DoBypassCheck())
                return false;

            //if the original object is already dead, don't randomize it
            if (!isAlreadyDead)
            {
                RandomizeEnemy(healthManagerObject);
                return false;
            }

            //delete this if it's already dead
            SpawnerExtensions.DestroyObject(healthManagerObject);

            return true;
        }

        string MODHOOK_BeforeSceneLoad(string sceneName)
        {
#if DEBUG
            edc.Instance.loaded = true;
#endif
            ResetBattleSceneCheck();
            return sceneName;
        }

        static void MODHOOK_SlashHitHook(Collider2D otherCollider, GameObject gameObject)
        {
            try
            {
                //invoke the custom hit methods for any spawned objects struck by the player's nail
                var soc = otherCollider.GetComponent<SpawnedObjectControl>();
                if (soc != null)
                {
                    soc.Hit(new HitInstance()
                    {
                        Source = HeroController.instance.gameObject,
                        AttackType = AttackTypes.Nail,
                        CircleDirection = false,
                        Direction = HeroController.instance.gameObject.transform.position.ToVec2().GetActualDirection(otherCollider.transform.position.ToVec2()),
                        IgnoreInvulnerable = false,
                        MagnitudeMultiplier = 1.0f,
                        MoveAngle = 0f,
                        MoveDirection = false,
                        Multiplier = 1f,
                        SpecialType = SpecialTypes.None,
                        IsExtraDamage = false,
                        DamageDealt = PlayerData.instance.nailDamage,                         
                    });
                }
            }
            catch(Exception e)
            {
                Dev.LogError("Caught unhandled exception in Hit callback the player's nail hit " + otherCollider.gameObject + e.StackTrace + e.Message);
            }
        }

        void ONHOOK_DamageHero_OnEnable(On.DamageHero.orig_OnEnable orig, DamageHero self)
        {
            orig(self);
            RandomizeHazard(self);
        }

        void ONHOOK_PersistentBoolItem_OnAwake(On.PersistentBoolItem.orig_Awake orig, PersistentBoolItem self)
        {
            bool useCustom = enemyReplacer.OnPersistentBoolItemLoaded(self);
            if (!useCustom)
                orig(self);
        }

        void ONHOOK_PersistentBoolItem_SetMyID(On.PersistentBoolItem.orig_SetMyID orig, PersistentBoolItem self)
        {
            bool useCustom = enemyReplacer.OnPersistentBoolItemSetMyID(self);
            if (!useCustom)
                orig(self);
        }

        void ONHOOK_PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);

            if (fsm == null)
                return;

            if (BattleManager.Instance.Value == null && BattleManager.IsBattleManager(fsm.gameObject))
            {
                BattleManager.LoadFromFSM(fsm);
            }

            try
            {
                if (fsm != null)
                {
                    //never spawn this, the white screen is annoying
                    if (fsm.gameObject.name.Contains("Corpse Zote Ordeal First"))
                    {
                        SpawnerExtensions.DestroyObject(fsm.gameObject);
                        return;
                    }

                    if (fsm.gameObject.name.Contains("Flamebearer Spawn") && GameManager.instance.playerData.grimmChildLevel > 0 && GameManager.instance.playerData.equippedCharm_40)
                    {
                        fsm.GetState("Ready").InsertCustomAction(() => {
                            PlayMakerFSM.BroadcastEvent("ALERT");
                            foreach(var f in GameObjectExtensions.EnumerateRootObjects(true).Where(x => x.name.Contains("Flamebearer") && x.name.Contains("(Clone)") && !x.activeInHierarchy && x.gameObject.GetComponent<FlameBearerFixer>()))
                            {
                                f.SafeSetActive(true);
                            }
                        }, 0);                       
                    }

                    var obj = fsm.gameObject;
                    if (obj != null && obj.name.Contains("mines_stomper") && obj.transform.parent != null && obj.transform.parent.gameObject.GetComponent<SpawnedObjectControl>() == null && ObjectMetadata.Get(obj.transform.parent.gameObject) == null)
                    {
                        //test.....
                        RandomizeEnemy(obj.transform.parent.gameObject);
                    }
                }
            }
            catch (Exception e) { }//nom
        }

        void DoBattleSceneCheck(GameObject gameObject)
        {
            if (!BattleManager.DidSceneCheck)
            {
                BattleManager.DoSceneCheck(gameObject);
            }
        }

        void ResetBattleSceneCheck()
        {
            BattleManager.DidSceneCheck = false;
        }

        bool DoBypassCheck()
        {
            if (EnemyRandomizer.bypassNextRandomization)
            {
                EnemyRandomizer.bypassNextRandomization = false;
                return true;
            }

            return false;
        }

        public static bool DoReplacementBypassCheck(bool peek = false)
        {
            //don't randomize in colo
            if (GameManager.instance.GetCurrentMapZone() == "COLOSSEUM" ||
                GameManager.instance.GetCurrentMapZone() == "FINAL_BOSS")
            {
                EnemyRandomizer.bypassNextReplacement = true;
            }

            if (EnemyRandomizer.bypassNextReplacement)
            {
                if (!peek)
                {
                    EnemyRandomizer.bypassNextReplacement = false;
                }
                return true;
            }

            return false;
        }

        public static bool HasCustomBypassReplacement()
        {
            if (!string.IsNullOrEmpty(debugCustomReplacement))
            {
                return true;
            }

            return false;
        }

        public static string GetCustomBypassReplacement()
        {
            return debugCustomReplacement;
        }

        public void Dispose()
        {
            ((IDisposable)disposables).Dispose();
        }

        /// <summary>
        /// Spawn exactly what you want!
        /// Also allows to test the replacement functionality by providing an optional replacement which will be immediately processed.
        /// </summary>
        public static GameObject CustomSpawn(Vector3 pos, string objectName, string replacement = null, bool setActive = true)
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || !activeScene.isLoaded)
            {
                //not really an error, but mark it as one for now
                Dev.LogError("Error: Cannot spawn something without a valid scene");
                return null;
            }

            try
            {
                if(setActive)
                    EnemyRandomizer.bypassNextReplacement = true;
                //EnemyRandomizer.debugCustomReplacement = replacement;

                return EnemyRandomizerDatabase.CustomSpawn(pos, objectName, replacement, setActive);
            }
            catch (Exception e)
            {
                Dev.LogError($"Custom spawn error: {e.Message} stacktrace: {e.StackTrace}");
            }

            return null;
        }

        public static void ClearBypass()
        {
            EnemyRandomizer.bypassNextReplacement = false;
        }
    }


    //public class RecycleOnDisable : MonoBehaviour
    //{
    //    public void OnDisable()
    //    {
    //        ObjectPool.Recycle(gameObject);
    //    }
    //}
}





//TODO: will need to do in a different place since other mods could load their modules later on
//Dev.Log("checking missing");
//List<string> missingLogics = EnemyRandomizer.instance.enemyReplacer.loadedLogics.Select(x => x.Name).Where(x => !GlobalSettings.loadedLogics.Contains(x)).ToList();

//missingLogics.ForEach(x =>
//{
//    Dev.LogWarning($"Last time EnemyRandomizer had loaded the logic module {x}, which no longer exists!");
//    //TODO: remove them? for now just post a warning
//});
//}