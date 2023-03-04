using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language; 
using On;
using nv;
using System.Linq;
using UniRx;
using System;
using System.Reflection;

namespace EnemyRandomizerMod
{
    //Global (non-player specific) settings
    public class EnemyRandomizerSettings
    {
        public string currentLogic;
        public bool IsNoClip = false;
    }

    //Player specific settings
    public class EnemyRandomizerPlayerSettings
    {
        public int seed;
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
        }
        static EnemyRandomizer instance;

        //true when the mod is disabled (but its data remains loaded...)
        public static bool isDisabled;

        //true during initialize when the mod is loading after the first time
        public static bool isReloading;

        public static bool bypassNextRandomizeEnemy;

        public override string GetVersion()
        {
            return currentVersion;
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
        static string currentVersion = Assembly.GetAssembly(typeof(EnemyRandomizer)).GetName().Version.ToString();

        public EnemyReplacer enemyReplacer = new EnemyReplacer();

        public Dictionary<string, IRandomizerLogic> logicTypes;

        public EnemyRandomizer()
            :base("Enemy Randomizer")
        {
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
                Dev.Logger.LoggingEnabled = false;
                Dev.Logger.GuiLoggingEnabled = false;
#endif
            Dev.Log("Created "+ currentVersion);
        }

        public override List<(string, string)> GetPreloadNames()
        {
            return enemyReplacer.GetPreloadNames(defaultDatabaseFilePath);
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (instance == null)
                instance = this;

            if (preloadedObjects != null)
            {
                if(preloadedObjects.Count <= 0)
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
                enemyReplacer.Setup(preloadedObjects);
                LoadLogics();
            }

            EnableMod();
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

            On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);
            On.UIManager.UIClosePauseMenu += new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;
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
        }

        void UnRegisterCallbacks()
        {
            ModHooks.SavegameLoadHook -= MODHOOK_LoadFromSave;

            ModHooks.BeforeSavegameSaveHook -= MODHOOK_SavegameSaveHook;

            ModHooks.NewGameHook -= MODHOOK_NewGameHook;

            ModHooks.OnReceiveDeathEventHook -= MODHOOK_RecieveDeathEvent;

            ModHooks.OnEnableEnemyHook -= MODHOOK_OnHealthManagerObjectEnabled;

            ModHooks.ObjectPoolSpawnHook -= MODHOOK_OnObjectPoolSpawn;

            On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
#if DEBUG
            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
#endif
            On.PersistentBoolItem.Awake -= ONHOOK_PersistentBoolItem_OnAwake;
            On.PlayMakerFSM.OnEnable -= ONHOOK_PlayMakerFSM_OnEnable;
            On.PersistentBoolItem.SetMyID -= ONHOOK_PersistentBoolItem_SetMyID;
            On.DamageHero.OnEnable -= ONHOOK_DamageHero_OnEnable;
        }

        public void EnableMod()
        {
            RegisterCallbacks();
            enemyReplacer.OnModEnabled();
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

        void MODHOOK_LoadFromSave(int saveSlot)
        {
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        void MODHOOK_SavegameSaveHook(SaveGameData data)
        {
            enemyReplacer.OnSaveGame(PlayerSettings);
        }

        void MODHOOK_NewGameHook()
        {
            enemyReplacer.OnStartGame(PlayerSettings);
        }

        GameObject MODHOOK_OnObjectPoolSpawn(GameObject originalGameObject)
        {
            return enemyReplacer.SpawnPooledObject(originalGameObject);
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

            enemyReplacer.OnEnemyDeathEvent(enemyDeathEffects.gameObject);
            BattleManager.OnEnemyDeathEvent(enemyDeathEffects.gameObject);
        }

        bool MODHOOK_OnHealthManagerObjectEnabled(GameObject healthManagerObject, bool isAlreadyDead)
        {
            if (isAlreadyDead)
                return isAlreadyDead;

            if(!BattleManager.DidSceneCheck)
            {
                BattleManager.DoSceneCheck(healthManagerObject);
            }

            enemyReplacer.OnEnemyLoaded(healthManagerObject);

            return isAlreadyDead;
        }

        string MODHOOK_BeforeSceneLoad(string sceneName)
        {
            enemyReplacer.OnBeforeSceneLoad();

            BattleManager.DidSceneCheck = false;

            return sceneName;
        }

        void ONHOOK_DamageHero_OnEnable(On.DamageHero.orig_OnEnable orig, DamageHero self)
        {
            orig(self);
            enemyReplacer.OnDamageHeroEnabled(self);
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

            enemyReplacer.OnPlaymakerFSMEnabled(fsm);
        }

        //public void EnableLogic(string logicName)
        //{
        //    enemyReplacer.SetLogic(logicTypes[logicName]);
        //}

        void LoadLogics()
        {
            logicTypes = LogicLoader.LoadLogics();

            //setup/construct all logics
            foreach (var logic in logicTypes.Select(x => x.Value))
            {
                logic.Setup(enemyReplacer);

                //assign the last used logic if it's in the list
                if (EnemyRandomizer.GlobalSettings.currentLogic == logic.Name)
                    enemyReplacer.currentLogic = logic;
            }

            if (!string.IsNullOrEmpty(EnemyRandomizer.GlobalSettings.currentLogic) && enemyReplacer.currentLogic == null)
            {
                Dev.LogError($"Last time EnemyRandomizer was set to use {EnemyRandomizer.GlobalSettings.currentLogic} logic, which no longer exists!");
            }
        }

        public static string GetCurrentMapZone()
        {
            return GameManager.instance.GetCurrentMapZone();
        }

        //public static GameObject DebugReplaceEnemy(string enemyName, GameObject enemyToReplace)
        //{
        //    try 
        //    {
        //        if (EnemyRandomizer.Instance.EnemyDataMap.TryGetValue(enemyName, out RandomizerObjectDefinition data))
        //        {
        //            string trimmedName = enemyToReplace.name.TrimGameObjectName();
        //            if (EnemyRandomizer.Instance.EnemyDataMap.TryGetValue(trimmedName, out RandomizerObjectDefinition replaceData))
        //            {
        //                enemyToReplace.SetActive(false);
        //                var enemy = data.randomizerObject.Instantiate();
        //                enemy.SetupRandomizerComponents(data.randomizerObject, enemyToReplace, replaceData);
        //                enemy.SetActive(true);
        //                return enemy;
        //            }
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Dev.LogError("Error: " + e.Message);
        //    }

        //    return null;
        //}

        //public static void DebugSpawnThenReplaceEnemy(string spawnName, string replaceName)
        //{
        //    try
        //    { 
        //        DebugReplaceEnemy(replaceName, DebugSpawnEnemy(spawnName));
        //    }
        //    catch(Exception e)
        //    {
        //        Dev.LogError("Error: " + e.Message);
        //    }
        //}

    }

    //public class RandomizeWhenVisible : MonoBehaviour
    //{
    //    IEnumerator Start()
    //    {
    //        Dev.Log("Waiting to randomize " + gameObject);
    //        if (gameObject == null)
    //            yield break;

    //        Collider2D collider = gameObject.GetComponent<Collider2D>();
    //        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
    //        if (collider == null && renderer == null)
    //            yield break;

    //        yield return new WaitUntil(() => {
    //            if (collider != null && renderer == null)
    //                return collider.enabled;
    //            else if (collider == null && renderer != null)
    //                return renderer.enabled;
    //            else //if (collider != null && renderer != null)
    //                return collider.enabled && renderer.enabled;
    //        });

    //        Dev.Log("Randomizng newly activated " + gameObject);
    //        EnemyRandomizer.Instance.ReplaceEnemy(gameObject);
    //        GameObject.Destroy(this);
    //    }
    //}


    //EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Crawler");
    //EnemyRandomizerMod.EnemyRandomizer.DebugSpawnThenReplaceEnemy("Crawler","Roller");

}
