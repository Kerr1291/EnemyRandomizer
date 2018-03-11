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

namespace EnemyRandomizerMod
{
    /*
     * 
     *  NOTES
     * 
     * 
     * 
     * --add Centipede Hatcher to rando monsters?
     * they're extra dangerous because theirs spawns are randomized
     * 
     * --health scuttler is the blue health bug
     * 
     * --things replacing baby centipede need to be adjusted to be sure they're in a safe spot like the siblings since they can spawn inside the ground
     * 
     * --add? Tentacle Box monster
     * 
     * --crystal guardian will have to be removed or have its camera effect fixed, it causes the camera to pan way off to the side on activation
     * 
     * --mage knight seems to have spawning issue, need to experiement with it more
     * 
     * --removed mender bug as he was always showing up as disabled
     * 
     * 
     * //Possible way to show enemy hitboxes:
     *   https://github.com/AllanBishop/UnityPhysicsDebugDraw2D
     * 
     * 
     * 
     */


    public partial class EnemyRandomizer : Mod<EnemyRandomizerSaveSettings, EnemyRandomizerSettings>, ITogglableMod
    {
        public static EnemyRandomizer Instance { get; private set; }

        CommunicationNode comms;

        Menu.RandomizerMenu menu;
        EnemyRandomizerLoader loader;
        EnemyRandomizerDatabase database;
        EnemyRandomizerLogic logic;

        string fullVersionName = "0.1.1";
        string modRootName = "RandoRoot";
        
        GameObject modRoot;
        public GameObject ModRoot {
            get {
                if( modRoot == null )
                {
                    modRoot = new GameObject(modRootName);
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

        //For debugging, the scene replacer will run its logic without doing anything
        //(Useful for testing without needing to wait through the load times)
        //This is set to true if the game is started without loading the database
        bool simulateReplacement = false;

        bool randomizerReady = false;
        bool RandomizerReady {
            get {
                return randomizerReady || simulateReplacement;
            }
            set {
                if(value && value != randomizerReady)
                {
                    logic.Setup( simulateReplacement );
                }
                if( !value && value != randomizerReady )
                {
                    logic.Unload();
                }
                randomizerReady = value;
            }
        }

        //value that can be set if a player enters the options menu
        //on startup, this value is randomized
        public int OptionsMenuSeed { get; set; }

        //the user configurable seed for the randomizer
        public int GameSeed { get; set; }

        //nice access to the player settings seed
        public int PlayerSettingsSeed {
            get {
                if (Settings == null)
                    return -1;
                return Settings.Seed;
            }
            set {
                if (Settings != null)
                    Settings.Seed = value;
            }
        }
        
        //set to false then the seed will be based on the type of enemy we're going to randomize
        //this will make each enemy type randomize into the same kind of enemy
        //if set to true, it also disables roomRNG and all enemies will be totally randomized
        bool chaosRNG = false;
        public bool ChaosRNG {
            get {
                return chaosRNG;
            }
            set {
                if( GlobalSettings != null )
                    GlobalSettings.RNGChaosMode = value;
                chaosRNG = value;
            }
        }

        //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
        //this will cause enemy types within the same room to be randomized the same
        //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
        bool roomRNG = true;
        public bool RoomRNG {
            get {
                return roomRNG;
            }
            set {
                if( GlobalSettings != null )
                    GlobalSettings.RNGRoomMode = value;
                roomRNG = value;
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
            comms = new CommunicationNode();
            comms.EnableNode( this );

            Log("Enemy Randomizer Mod initializing!");

            SetupDefaulSettings();

            UnRegisterCallbacks();
            RegisterCallbacks();

            //create the database that will hold all the loaded enemies
            if( database == null )
                database = new EnemyRandomizerDatabase();

            if( logic == null )
                logic = new EnemyRandomizerLogic( database );

            //create the loader which will handle loading all the enemy types in the game
            if( loader == null )
                loader = new EnemyRandomizerLoader( database );

            //Create all mod UI elements and their manager
            if( menu == null )
                menu = new Menu.RandomizerMenu();

            database.Setup();
            loader.Setup();
            menu.Setup(); 
        }

        void SetupDefaulSettings()
        {
            string globalSettingsFilename = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            if (!File.Exists(globalSettingsFilename))
            {
                Log("Global settings file not found, generating new one... File was not found at: " + globalSettingsFilename);

                ChaosRNG = false;
                RoomRNG = false;

                SaveGlobalSettings();
            }

            OptionsMenuSeed = GameRNG.Randi();
        }

        void RegisterCallbacks()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CheckAndDisableLogicInMenu;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CheckAndDisableLogicInMenu;
            ModHooks.Instance.AfterSavegameLoadHook += TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook += EnableEnemyRandomizerFromNewGame;
            ModHooks.Instance.SlashHitHook += DebugPrintObjectOnHit;
        }

        void UnRegisterCallbacks()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CheckAndDisableLogicInMenu;
            ModHooks.Instance.AfterSavegameLoadHook -= TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook -= EnableEnemyRandomizerFromNewGame;
            ModHooks.Instance.SlashHitHook -= DebugPrintObjectOnHit;
        }

        void CheckAndDisableLogicInMenu( Scene from, Scene to )
        {
            if( to.name == Menu.RandomizerMenu.MainMenuSceneName )
            {
                DisableEnemyRandomizer();
            }
        }

        ///Revert all changes the mod has made
        public void Unload()
        {
            DisableEnemyRandomizer();

            UnRegisterCallbacks();

            menu.Unload();
            loader.Unload();
            database.Unload();

            ModRoot = null;

            comms.DisableNode();
            Instance = null;
        }

        //Called when loading a save game
        void TryEnableEnemyRandomizerFromSave(SaveGameData data)
        {
            if( Settings != null )
                GameSeed = Settings.Seed;
            else
                GameSeed = OptionsMenuSeed;

            EnableEnemyRandomizer();
        }

        //Call from New Game
        void EnableEnemyRandomizerFromNewGame()
        {
            GameSeed = OptionsMenuSeed;
            EnableEnemyRandomizer();
        }

        void EnableEnemyRandomizer()
        {
            RandomizerReady = true;

            simulateReplacement = !loader.DatabaseGenerated;

            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
        }

        //call when returning to the main menu
        void DisableEnemyRandomizer()
        {
            RandomizerReady = false;
        }

        //TODO: update when version checker is fixed in new modding API version
        public override string GetVersion()
        {
            return fullVersionName;
        }

        //TODO: update when version checker is fixed in new modding API version
        public override bool IsCurrent()
        {
            return true;        
        }

        //used while testing to record things hit by a player's nail
        static string debugRecentHit = "";
        static void DebugPrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            if( otherCollider.gameObject.name != debugRecentHit )
            {
                Dev.Log( "(" + otherCollider.gameObject.transform.position + ") HIT: " + otherCollider.gameObject.name );
                debugRecentHit = otherCollider.gameObject.name;
            }
        }
    }
}
