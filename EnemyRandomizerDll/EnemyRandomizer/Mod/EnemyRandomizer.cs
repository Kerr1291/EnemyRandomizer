using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Linq;
using UniRx;
using System;
using System.Reflection;
using UnityEngine.UI;

namespace EnemyRandomizerMod
{
    public class EnemyRandomizerSettings
    {
        public int seed = -1;
        public List<string> loadedLogics;
        public bool RandomizeBosses = true;
        public bool UseCustomSeed = false;
        public List<LogicSettings> logicSettings = new List<LogicSettings>();
    }

    public class LogicSettings
    {
        public string name;
        public List<LogicOption> options = new List<LogicOption>();
    }

    public class LogicOption
    {
        public string name;
        public bool value;
    }

    public static class LogicSettingsMethods
    {
        public static LogicSettings GetLogicSettings(this EnemyRandomizerSettings self, string logicName)
        {
            if (!self.logicSettings.Any(x => x.name == logicName))
            {
                self.logicSettings.Add(new LogicSettings() { name = logicName });
            }
            return self.logicSettings.FirstOrDefault(x => x.name == logicName);
        }

        public static LogicOption GetOption(this LogicSettings settings, string optionName)
        {
            if (!settings.options.Any(x => x.name == optionName))
            {
                settings.options.Add(new LogicOption() { name = optionName });
            }

            return settings.options.FirstOrDefault(x => x.name == optionName);
        }
    }

    //Player specific settings
    public class EnemyRandomizerPlayerSettings
    {
        public int enemyRandomizerSeed = -1;
    }

    public partial class EnemyRandomizer : Mod, ITogglableMod, IGlobalSettings<EnemyRandomizerSettings>, ILocalSettings<EnemyRandomizerPlayerSettings>
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

        public static bool bypassNextRandomization;

        public static bool DEBUG_SKIP_LOADING = false;

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
        static string currentVersionPrefix = Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString() + "[Alpha 5, Now with more jank!]";
        static string currentVersion = currentVersionPrefix;
            //Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString() + $" CURRENT SEED:[{GlobalSettings.seed}] -- TO CHANGE SEED --> MODS > ENEMY RANDOMIZER > ENEMY RANDOMIZER MODULES";

        protected static string GetVersionString()
        {
            string prefix = currentVersionPrefix;
            string postfix = "\n\t\tTO CHANGE --> OPTIONS > MODS > ENEMY RANDOMIZER OPTIONS > ENEMY RANDOMIZER MODULES";

            string seedInfo;

            if(GlobalSettings.UseCustomSeed)
                seedInfo = prefix + $" USING CUSTOM SEED:[{GlobalSettings.seed}] -- " + postfix;
            else
                seedInfo = prefix + $" NEW GAME WILL GENERATE NEW SEED -- " + postfix;

            return seedInfo;
        }


        public EnemyReplacer enemyReplacer = new EnemyReplacer();

        public Dictionary<string, IRandomizerLogic> logicTypes;

        public static ReactiveProperty<List<GameObject>> BlackBorders { get; protected set; }

        public EnemyRandomizer()
            : base("Enemy Randomizer")
        {
            Instance = this;
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
                if (!DEBUG_SKIP_LOADING)
                {
                    BattleManager.Init();
                    enemyReplacer.Setup(preloadedObjects);
                    LoadLogics();
                }
            }

            EnableMod();
            UIManager.instance.StartCoroutine(UpdateLabelOnLoad());
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

            ModHooks.OnEnableEnemyHook -= MODHOOK_OnHealthManagerObjectEnabled;
            ModHooks.OnEnableEnemyHook += MODHOOK_OnHealthManagerObjectEnabled;

            ModHooks.ObjectPoolSpawnHook -= MODHOOK_OnObjectPoolSpawn;
            ModHooks.ObjectPoolSpawnHook += MODHOOK_OnObjectPoolSpawn;

            //On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);
            //On.UIManager.UIClosePauseMenu += new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;

            GameManager.instance.UnloadingLevel -= Instance_UnloadingLevel;
            GameManager.instance.UnloadingLevel += Instance_UnloadingLevel;

#if DEBUG
            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
            ModHooks.SlashHitHook += DebugPrintObjectOnHit;
#endif

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
                UpdateModVersionLabel();
            };

            // can remove after testing
            On.QuitToMenu.Start += (orig, self) =>
            {
                if (GlobalSettings.UseCustomSeed)
                {
                    GeneralOptionsMenu?.Find("SeedInput")?.Show();
                }
                UpdateModVersionLabel();
                return orig(self);
            };
        }

        IEnumerator UpdateLabelOnLoad()
        {
            yield return new WaitUntil(() => GameObject.FindObjectOfType<ModVersionDraw>(true) != null);
            var mvd = GameObject.FindObjectOfType<ModVersionDraw>(true);
            yield return new WaitUntil(() => mvd.drawString.Contains(currentVersion));
            UpdateModVersionLabel();
        }

        void ModHooks_DrawBlackBordersHook(List<GameObject> obj)
        {
            Dev.Log("setting black borders");
            BlackBorders.Value = obj;
        }

        void Instance_UnloadingLevel()
        {
            Dev.Log("unloading level");
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

            ModHooks.ObjectPoolSpawnHook -= MODHOOK_OnObjectPoolSpawn;

            //On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;

            GameManager.instance.UnloadingLevel -= Instance_UnloadingLevel;

#if DEBUG
            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
#endif
            On.PersistentBoolItem.Awake -= ONHOOK_PersistentBoolItem_OnAwake;
            On.PlayMakerFSM.OnEnable -= ONHOOK_PlayMakerFSM_OnEnable;
            On.PersistentBoolItem.SetMyID -= ONHOOK_PersistentBoolItem_SetMyID;
            On.DamageHero.OnEnable -= ONHOOK_DamageHero_OnEnable;

            ModHooks.DrawBlackBordersHook -= ModHooks_DrawBlackBordersHook;
        }

        public void UpdateModVersionLabel()
        {
            string olds = currentVersion;
            string news = GetVersionString();
            //Dev.Log($"{olds}   {news}    {currentVersion}");
            currentVersion = news;
            var mvd = GameObject.FindObjectOfType<ModVersionDraw>(true);
            mvd.drawString = mvd.drawString.Replace(olds, news);
            //Dev.Log($"{mvd.drawString}");
        }

        public void EnableMod()
        {
            RegisterCallbacks();
            if (!DEBUG_SKIP_LOADING)
            {
                enemyReplacer.OnModEnabled();
            }
            isReloading = false;
            isDisabled = false;
        }

        ///Revert all changes the mod has made
        public void Unload()
        {
#if DEBUG
            {
                Dev.Logger.GuiLoggingEnabled = false;
                DevLogger.Instance.HideSlider();
                DevLogger.Instance.Show(false);
                Dev.Logger.LoggingEnabled = false;
            }
#endif
            enemyReplacer.OnModDisabled();
            UnRegisterCallbacks();
            isDisabled = true;
        }

        protected virtual GameObject RandomizeEnemy(GameObject enemyObject)
        {
            DoBattleSceneCheck(enemyObject);
            return enemyReplacer.RandomizeEnemy(enemyObject);
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
            SetGameSeed();
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        void MODHOOK_SavegameSaveHook(SaveGameData data)
        {
            enemyReplacer.OnSaveGame(PlayerSettings);
        }

        void MODHOOK_NewGameHook()
        {
            SetGameSeed();
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        GameObject MODHOOK_OnObjectPoolSpawn(GameObject originalGameObject)
        {
            var pooledGO = RandomizePooledSpawn(originalGameObject);

            //see if this pooled game object should me pooled by the randomizer
            if (pooledGO != null && RandoControlledPooling.Any(x => pooledGO.name.Contains(x)))
            {
                //can't be doing this for everything, is causing nullrefs
                //want to do it for some things, maybe based on a list?
                pooledGO.GetOrAddComponent<RecycleOnDisable>();
                var auto = pooledGO.GetComponent<AutoRecycleSelf>();
                if (auto != null)
                    GameObject.Destroy(auto);
            }

            return pooledGO;
        }

        public static List<string> RandoControlledPooling = new List<string>()
        {
            "Radiant Nail",
            "Dust Trail",
        };

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

        bool MODHOOK_OnHealthManagerObjectEnabled(GameObject healthManagerObject, bool isAlreadyDead)
        {
            if (isAlreadyDead)
                return isAlreadyDead;

            if (DoBypassCheck())
                return false;

            bool result = RandomizeEnemy(healthManagerObject) == null;
            return result;
        }

        string MODHOOK_BeforeSceneLoad(string sceneName)
        {
            ClearBypassCheck();
            return sceneName;
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

            if (BattleManager.Instance.Value == null && BattleManager.IsBattleManager(fsm.gameObject))
            {
                BattleManager.LoadFromFSM(fsm);
            }

            //enemyReplacer.OnPlaymakerFSMEnabled(fsm);
        }

        public void LoadLogics()
        {
            if (logicTypes != null && logicTypes.Count > 0)
                return;

            Dev.Log("loading logics");
            logicTypes = LogicLoader.LoadLogics();

            Dev.Log("constructing logics");
            //setup/construct all logics
            foreach (var logic in logicTypes.Select(x => x.Value))
            {
                logic.Setup(enemyReplacer.database);

                if (GlobalSettings.loadedLogics == null)
                    GlobalSettings.loadedLogics = new List<string>();

                if (GlobalSettings.loadedLogics.Contains(logic.Name))
                {
                    enemyReplacer.EnableLogic(logic);
                }
            }

            Dev.Log("loading subpages");

            if (enemyReplacer.loadedLogics == null)
                enemyReplacer.loadedLogics = new HashSet<IRandomizerLogic>();

            Dev.Log("checking missing");
            List<string> missingLogics = enemyReplacer.loadedLogics.Select(x => x.Name).Where(x => !GlobalSettings.loadedLogics.Contains(x)).ToList();

            missingLogics.ForEach(x =>
            {
                Dev.LogWarning($"Last time EnemyRandomizer was set to use {x} logic, which no longer exists!");
            });
        }

        //public static string GetCurrentMapZone()
        //{
        //    return GameManager.instance.GetCurrentMapZone();
        //}

        void DoBattleSceneCheck(GameObject gameObject)
        {
            if (!BattleManager.DidSceneCheck)
            {
                BattleManager.DoSceneCheck(gameObject);
            }
        }

        void ClearBypassCheck()
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
    }

    public class RecycleOnDisable : MonoBehaviour
    {
        public void OnDisable()
        {
            ObjectPool.Recycle(gameObject);
        }
    }
}