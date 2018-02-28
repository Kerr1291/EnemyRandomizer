using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;

using nv;


/*
 * Top TODOs:
 * 
 * 1. get rotated enemies to orient to the walls/cieling
 * 
 * 2. find a way to serialize playmaker fsms....
 * 
 * 3. test rando for softlocks && bugs
 * 
 * 
 * try something with this to kill enemies
            //FSMUtility.LocateFSM( enemy, "health_manager_enemy" ).SetState( "Decrement Health" );
 * 
 * */

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod<EnemyRandomizerSettings, EnemyRandomizerSettings>, ITogglableMod
    {
        //TODO: allow a user configurable option for this
        //the user configurable seed for the randomizer
        int loadedBaseSeed = -1;
        public int LoadedBaseSeed {
            get {
                return loadedBaseSeed;
            }
            set {
                if( randomizerReady && Settings != null )
                    Settings.BaseSeed = value;
                if( GlobalSettings != null )
                    GlobalSettings.BaseSeed = value;
                loadedBaseSeed = value;
            }
        }

        bool randomizerReady = false;

        public static EnemyRandomizer Instance { get; private set; }

        string recentHit = "";
        string fullVersionName = "0.0.2";

        Dictionary<string, List<string>> enemyTypes = new Dictionary<string, List<string>>();
        List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        List<string> loadedEnemyPrefabNames = new List<string>();
        List<string> uniqueEnemyTypes = new List<string>();

        GameObject modRoot;
        GameObject ModRoot {
            get {
                if( modRoot == null )
                {
                    modRoot = new GameObject( "RandoRoot" );
                    GameObject.DontDestroyOnLoad( modRoot );
                }
                return modRoot;
            }
            set {
                if( modRoot != null && value != modRoot )
                {
                    GameObject.Destroy( modRoot );
                }
                modRoot = value;
            }
        }


        GameObject replacementRoot;
        GameObject ReplacementRoot {
            get {
                if( replacementRoot == null )
                {
                    replacementRoot = new GameObject( "ReplacementRoot" );
                    GameObject.DontDestroyOnLoad( replacementRoot );
                }
                return replacementRoot;
            }
            set {
                if( replacementRoot != null && value != replacementRoot )
                {
                    GameObject.Destroy( replacementRoot );
                }
                replacementRoot = value;
            }
        }

        //Called when loading a save game
        void TryEnableEnemyRandomizerFromSave( SaveGameData data )
        {
            if( !databaseGenerated )
                return;

            randomizerReady = true;

            Log( "Before: "+loadedBaseSeed );

            //TODO: Make sure this is happening!
            loadedBaseSeed = Settings.BaseSeed;

            Log( "After: " + loadedBaseSeed );
            chaosRNG = Settings.RNGChaosMode;
            roomRNG = Settings.RNGRoomMode;
            randomizeDisabledEnemies = Settings.RandomizeDisabledEnemies;
        }

        //TODO: REMOVE NON-SEED SETTINGS FROM PLAYER SPECIFIC DATA
        //Call from New Game
        void TryEnableEnemyRandomizer()
        {
            if( !databaseGenerated )
                return;

            randomizerReady = true;

            LoadedBaseSeed = GlobalSettings.BaseSeed;
            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
            RandomizeDisabledEnemies = GlobalSettings.RandomizeDisabledEnemies;
        }

        //call when returning to the main menu
        void DisableEnemyRandomizer()
        {
            randomizerReady = false;
            RestoreLogic();
        }

        void SetupDefaultGlobalSettings()
        {
            string globalSettingsFilename = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            //TODO: while debugging, always reload the defaults
            if( !File.Exists( globalSettingsFilename ) )
            {
                Log( "Global settings file not found, generating new one... File not found: " + globalSettingsFilename );
                //setup default global settings
                LoadedBaseSeed = GameRNG.Randi();
                ChaosRNG = false;
                RoomRNG = false;
                RandomizeDisabledEnemies = false;
                SaveGlobalSettings();
            }
        }

        public override void Initialize()
        {
            if(Instance != null)
            {
                Log("Warning: EnemyRandomizer is a singleton. Trying to create more than one may cause issues!");
                return;
            }

            Instance = this;

            SetupDefaultGlobalSettings();

            Log("Enemy Randomizer Mod initializing!");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ToggleBuildRandoDatabaseUI;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartRandomEnemyLocator;            

            //TODO: may not be needed anymore...??
            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.ColliderCreateHook += OnLoadObjectCollider;

            ModHooks.Instance.AfterSavegameLoadHook -= TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.AfterSavegameLoadHook += TryEnableEnemyRandomizerFromSave;

            ModHooks.Instance.NewGameHook -= TryEnableEnemyRandomizer;
            ModHooks.Instance.NewGameHook += TryEnableEnemyRandomizer;

            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;
            ModHooks.Instance.SlashHitHook += Debug_PrintObjectOnHit;

            LoadConfigUI();
        }

        public void Unload()
        {
            Instance = null;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.AfterSavegameLoadHook -= TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook -= TryEnableEnemyRandomizer;
            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;

            ModRoot = null;

            if( menu != null )
            {
                GameObject.Destroy( menu );
            }

            Restore();
        }
        
        void Restore()
        {
            randomizerReady = false;
            recentHit = "";

            RestoreUI();
            RestoreSetup();
            RestoreLogic();
        }

        public override string GetVersion()
        {
            if( fullVersionName.Length < 2 )
            {
                //try
                //{
                //    GithubVersionHelper helper = new GithubVersionHelper("Kerr1291/EnemyRandomizer");
                //    fullVersionName = "0.0.1 (Github version: " + helper.GetVersion() + ")";
                //}
                //catch(Exception e)
                //{
                //    fullVersionName = "0.0.1";
                //}
            }
            return fullVersionName;
        }

        public override bool IsCurrent()
        {
            try
            {
                //GithubVersionHelper helper = null;
                //try
                //{
                //    helper = new GithubVersionHelper("Kerr1291/EnemyRandomizer");
                //    Log( "Github = " + helper.GetVersion() );
                //}
                //catch( Exception e )
                //{
                //    helper = null;
                //}

                return true;
                //return helper == null || GetVersion().StartsWith( helper.GetVersion() );
            }
            catch( Exception )
            {
                return true;
            }
        }

    }
}
