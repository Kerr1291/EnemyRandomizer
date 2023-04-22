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
using UniRx.Triggers;
using System;
using System.Reflection;
using UnityEngine.UI;
using Satchel.BetterMenus;
using UnityEngine.Events;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod, ITogglableMod, IGlobalSettings<EnemyRandomizerSettings>, ILocalSettings<EnemyRandomizerPlayerSettings>, IDisposable
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
        static string currentVersionPrefix = Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString() + "[Alpha 8 oat pt 2]";
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
            //TODO: put a condition here to add more info
            //after testing a few versions without this notice to see if it was useful or not
            return;
            //string olds = currentVersion;
            //string news = GetVersionString();
            ////Dev.Log($"{olds}   {news}    {currentVersion}");
            //currentVersion = news;
            //var mvd = GameObject.FindObjectOfType<ModVersionDraw>(true);
            //mvd.drawString = mvd.drawString.Replace(olds, news);
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

        public void SetIsLoaded()
        {
            IsModLoaded.Value = true;
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
            if (pooledGO != null && EnemyReplacer.RandoControlledPooling.Any(x => pooledGO.name.Contains(x)))
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
            ResetBattleSceneCheck();
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

            if (fsm == null)
                return;

            if (BattleManager.Instance.Value == null && BattleManager.IsBattleManager(fsm.gameObject))
            {
                BattleManager.LoadFromFSM(fsm);
            }
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

        public static bool DoReplacementBypassCheck()
        {
            if (EnemyRandomizer.bypassNextReplacement)
            {
                EnemyRandomizer.bypassNextReplacement = false;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            ((IDisposable)disposables).Dispose();
        }

        public static GameObject DebugSpawnEnemy(string enemyName, string replacement = null)
        {
            try
            {
                EnemyRandomizer.bypassNextReplacement = true;
                EnemyRandomizer.debugCustomReplacement = replacement;

                var enemy = EnemyRandomizerDatabase.GetDatabase().Spawn(enemyName, null);
                if (enemy != null)
                {
                    var pos = HeroController.instance.transform.position + Vector3.right * 5f;
                    enemy.transform.position = pos;
                    var defaultEnemyControl = enemy.GetComponent<DefaultSpawnedEnemyControl>();
                    enemy.SetActive(true);
                    return enemy;
                }
            }
            catch (Exception e)
            {
                Dev.LogError("Error: " + e.Message);
            }

            return null;
        }

        /// <summary>
        /// Spawn exactly what you want!
        /// Also allows to test the replacement functionality by providing an optional replacement which will be immediately processed.
        /// </summary>
        public static GameObject CustomSpawn(Vector3 pos, string objectName, string replacement = null, bool setActive = true)
        {
            try
            {
                EnemyRandomizer.bypassNextReplacement = true;
                EnemyRandomizer.debugCustomReplacement = replacement;

                return EnemyRandomizerDatabase.CustomSpawn(pos, objectName, setActive);
            }
            catch (Exception e)
            {
                Dev.LogError("Custom Spawn Error: " + e.Message);
            }

            return null;
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





//TODO: will need to do in a different place since other mods could load their modules later on
//Dev.Log("checking missing");
//List<string> missingLogics = EnemyRandomizer.instance.enemyReplacer.loadedLogics.Select(x => x.Name).Where(x => !GlobalSettings.loadedLogics.Contains(x)).ToList();

//missingLogics.ForEach(x =>
//{
//    Dev.LogWarning($"Last time EnemyRandomizer had loaded the logic module {x}, which no longer exists!");
//    //TODO: remove them? for now just post a warning
//});
//}