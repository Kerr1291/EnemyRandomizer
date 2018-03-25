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
     * --health scuttler is the blue health bug
     * 
     * --things replacing baby centipede need to be adjusted to be sure they're in a safe spot like the siblings since they can spawn inside the ground
     * 
     * --add? Tentacle Box monster
     * 
     * --removed mender bug as he was always showing up as disabled
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

        string fullVersionName = "0.2.0";
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

        //if randomizeGeo is enabled, then we will put a random amount of geo on enemies
        bool randomizeGeo = false;
        public bool RandomizeGeo {
            get {
                return randomizeGeo;
            }
            set {
                if( GlobalSettings != null )
                    GlobalSettings.RandomizeGeo = value;
                randomizeGeo = value;
            }
        }

        //if custom enemies is enabled, then we are allowed to add custom enemies that don't exist in the base game
        bool customEnemies = false;
        public bool CustomEnemies {
            get {
                return customEnemies;
            }
            set {
                if( GlobalSettings != null )
                    GlobalSettings.CustomEnemies = value;
                customEnemies = value;
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


            ContractorManager.Instance.StartCoroutine( DebugInput() );
        }

        IEnumerator DebugInput()
        {
            yield return new WaitForSeconds( 2f );
            MenuStyles.Instance.SetStyle( 4, true, false );


            //int i = 0;
            //while( true )
            //{
            //    yield return new WaitForEndOfFrame();
            //    if( UnityEngine.Input.GetKeyDown( KeyCode.Y ) )
            //    {
            //        MenuStyles.Instance.SetStyle( i, true, false );
            //        i = ( i + 1 ) % MenuStyles.Instance.styles.Length;
            //    }
            //}
            //yield break;
        }

        void SetupDefaulSettings()
        {
            string globalSettingsFilename = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            bool forceReloadGlobalSettings = false;
            if( GlobalSettings != null && GlobalSettings.SettingsVersion != EnemyRandomizerSettingsVars.GlobalSettingsVersion )
            {
                forceReloadGlobalSettings = true;
            }
            else
            {
                Log( "Global settings version match!" );
            }

            if( forceReloadGlobalSettings || !File.Exists( globalSettingsFilename ) )
            {
                if( forceReloadGlobalSettings )
                {
                    Log( "Global settings are outdated! Reloading global settings" );
                }
                else
                {
                    Log( "Global settings file not found, generating new one... File was not found at: " + globalSettingsFilename );
                }

                GlobalSettings.Reset();

                GlobalSettings.SettingsVersion = EnemyRandomizerSettingsVars.GlobalSettingsVersion;

                ChaosRNG = false;
                RoomRNG = true;
                RandomizeGeo = false;
                CustomEnemies = false;
            }

            OptionsMenuSeed = GameRNG.Randi();

            SaveGlobalSettings();
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
            {
                GameSeed = PlayerSettingsSeed;
            }
            else
            {
                GameSeed = OptionsMenuSeed;
            }

            EnableEnemyRandomizer();
        }

        //Call from New Game
        void EnableEnemyRandomizerFromNewGame()
        {
            GameSeed = OptionsMenuSeed;
            PlayerSettingsSeed = GameSeed;
            EnableEnemyRandomizer();
        }

        void EnableEnemyRandomizer()
        {
            SetNoclip( false );
            RandomizerReady = true;

            simulateReplacement = !loader.DatabaseGenerated;

            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
            RandomizeGeo = GlobalSettings.RandomizeGeo;
            CustomEnemies = GlobalSettings.CustomEnemies;
        }

        //call when returning to the main menu
        void DisableEnemyRandomizer()
        {
            SetNoclip( false );
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

        public void SetNoclip(bool state)
        {
            noclip = state;

            if( noclip )
            {
                Dev.Log( "Enabled noclip" );
                noclipPos = HeroController.instance.gameObject.transform.position;
                noClipRunner.OnUpdate = DoNoclip;
                noClipRunner.Looping = true;
                noClipRunner.Start();
            }
            else
            {
                noClipRunner.Reset();
                Dev.Log( "Disabled noclip" );
            }
        }

        public bool NoClipState {
            get {
                return noclip;
            }
        }

        Contractor noClipRunner = new Contractor();
        Vector3 noclipPos;
        bool noclip = false;
        public void DoNoclip()
        {
            if( HeroController.instance == null || HeroController.instance.gameObject == null || !HeroController.instance.gameObject.activeInHierarchy )
                return;

            if( noclip )
            {
                if( GameManager.instance.inputHandler.inputActions.left.IsPressed )
                {
                    noclipPos = new Vector3( noclipPos.x - Time.deltaTime * 20f, noclipPos.y, noclipPos.z );
                }

                if( GameManager.instance.inputHandler.inputActions.right.IsPressed )
                {
                    noclipPos = new Vector3( noclipPos.x + Time.deltaTime * 20f, noclipPos.y, noclipPos.z );
                }

                if( GameManager.instance.inputHandler.inputActions.up.IsPressed )
                {
                    noclipPos = new Vector3( noclipPos.x, noclipPos.y + Time.deltaTime * 20f, noclipPos.z );
                }

                if( GameManager.instance.inputHandler.inputActions.down.IsPressed )
                {
                    noclipPos = new Vector3( noclipPos.x, noclipPos.y - Time.deltaTime * 20f, noclipPos.z );
                }

                if( HeroController.instance.transitionState.ToString() == "WAITING_TO_TRANSITION" )
                {
                    HeroController.instance.gameObject.transform.position = noclipPos;
                }
                else
                {
                    noclipPos = HeroController.instance.gameObject.transform.position;
                }
            }
        }

        //used while testing to record things hit by a player's nail
        static string debugRecentHit = "";
        static void DebugPrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            if( otherCollider.gameObject.name != debugRecentHit )
            {
                Dev.Log( "Hero at "+HeroController.instance.transform.position+" HIT: " + otherCollider.gameObject.name + " at (" + otherCollider.gameObject.transform.position + ")");
                debugRecentHit = otherCollider.gameObject.name;
            }
        }
    }
}
