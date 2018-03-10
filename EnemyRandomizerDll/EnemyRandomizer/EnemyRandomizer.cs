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


//TODO: move all the Log calls to use my Dev.Log
//TODO: change Dev.Log to print to the mod's Logging output when the #define exists
//TODO: continue the refactor toward the alpha build....


namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod<EnemyRandomizerSettings, EnemyRandomizerSettings>, ITogglableMod
    {
        //the user configurable seed for the randomizer
        int loadedBaseSeed = -1;
        public int LoadedBaseSeed {
            get {
                return loadedBaseSeed;
            }
            set {
                if( RandomizerReady && Settings != null )
                    Settings.BaseSeed = value;
                if( GlobalSettings != null )
                    GlobalSettings.BaseSeed = value;
                loadedBaseSeed = value;
            }
        }

        //For debugging, set this to true to have the scene replacer run its logic without doing anything
        //(Useful for testing without needing to wait through the load times)
        public const bool simulateReplacement = false;

        bool randomizerReady = false;
        bool RandomizerReady {
            get {
                return randomizerReady || simulateReplacement;
            }
            set {
                randomizerReady = value;
            }
        }

        public static EnemyRandomizer Instance { get; private set; }

        string recentHit = "";
        string fullVersionName = "0.0.13a"; 

        public Dictionary<string, List<string>> enemyTypes = new Dictionary<string, List<string>>();
        public List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        public List<string> loadedEnemyPrefabNames = new List<string>();
        public List<string> uniqueEnemyTypes = new List<string>();

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

        //Called when loading a save game
        void TryEnableEnemyRandomizerFromSave( SaveGameData data )
        {
            if( !databaseGenerated )
                return;

            RandomizerReady = true;

            Log( "Before: "+loadedBaseSeed );
            
            if( Settings != null )
                loadedBaseSeed = Settings.BaseSeed;
            else
                LoadedBaseSeed = GlobalSettings.BaseSeed;

            Log( "After: " + loadedBaseSeed );
            //chaosRNG = Settings.RNGChaosMode;
            //roomRNG = Settings.RNGRoomMode;
            //randomizeDisabledEnemies = Settings.RandomizeDisabledEnemies;
            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
        }
        
        //Call from New Game
        void TryEnableEnemyRandomizer()
        {
            if( !databaseGenerated )
                return;

            RandomizerReady = true;

            LoadedBaseSeed = GlobalSettings.BaseSeed;
            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
        }

        //call when returning to the main menu
        void DisableEnemyRandomizer()
        {
            RandomizerReady = false;
            RestoreLogic();
        }

        void SetupDefaultGlobalSettings()
        {
            string globalSettingsFilename = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";
            
            if( !File.Exists( globalSettingsFilename ) )
            {
                Log( "Global settings file not found, generating new one... File not found: " + globalSettingsFilename );
                //setup default global settings
                LoadedBaseSeed = GameRNG.Randi();
                ChaosRNG = false;
                RoomRNG = false;

                //catch all for a weird edge case
                if( LoadedBaseSeed == -1 )
                    LoadedBaseSeed = GameRNG.Randi();

                SaveGlobalSettings();
            }

            //catch all for a weird edge case
            if( LoadedBaseSeed == -1 )
                LoadedBaseSeed = GameRNG.Randi();
        }

        public override void Initialize()
        {
            if(Instance != null)
            {
                Log("Warning: EnemyRandomizer is a singleton. Trying to create more than one may cause issues!");
                return;
            }
            //Time.timeScale = 2f;

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
            RandomizerReady = false;
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


        public void Debug_PrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            if( otherCollider.gameObject.name != recentHit )
            {
                Log( "(" + otherCollider.gameObject.transform.position + ") HIT: " + otherCollider.gameObject.name );
                recentHit = otherCollider.gameObject.name;
            }
        }

    }
}
