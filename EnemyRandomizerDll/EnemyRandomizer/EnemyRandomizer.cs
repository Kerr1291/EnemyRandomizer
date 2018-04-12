using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;

using ModCommon;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod<EnemyRandomizerSaveSettings, EnemyRandomizerSettings>, ITogglableMod
    {
        public static EnemyRandomizer Instance { get; private set; }

        CommunicationNode comms;

        Menu.RandomizerMenu menu;
        EnemyRandomizerLoader loader;
        EnemyRandomizerDatabase database;
        EnemyRandomizerLogic logic;

        string fullVersionName = "0.2.x (in development)";
        string modRootName = "RandoRoot";

        //public const bool kmode = true;
        
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

        //bool suspended = false;
        IEnumerator DebugInput()
        {
            yield return new WaitForSeconds( 2f );
            MenuStyles.Instance.SetStyle( 4, true, false );
            
            while( true )
            {
                yield return null;

                if( HeroController.instance != null )
                {
                    if(HeroController.instance.playerData.health < 4)
                    {
                        HeroController.instance.MaxHealth();
                    }
                }
                //enter hornet
                if( UnityEngine.Input.GetKeyDown( KeyCode.L ) )
                {
                    GameManager.instance.playerData.SetInt( "hornetGreenpath", 0 );
                    GameManager.instance.playerData.SetBool( "hornet1Defeated", false );
                    GameManager.instance.playerData.SetBool( "disablePause", true );
                    yield return EnterZone( "Fungus1_04", "right1", "Hornet Boss 1" );
                    yield return new WaitForSeconds( 2f );
                    HeroController.instance.transform.position = GameObject.Find("Hornet Boss 1").transform.position + new Vector3(5f,0f,0f);
                }
            }
            yield break;
        }
        

        public IEnumerator EnterSandbox()
        {
            //find a source transition
            string currentSceneTransition = GameObject.FindObjectOfType<TransitionPoint>().gameObject.name;
            string currentScene = GameManager.instance.sceneName;

            //update the last entered
            TransitionPoint.lastEntered = currentSceneTransition;

            //place us in sly's storeroom
            GameManager.instance.BeginSceneTransition( new GameManager.SceneLoadInfo
            {
                SceneName = "Room_Sly_Storeroom",
                EntryGateName = "top1",
                HeroLeaveDirection = new GlobalEnums.GatePosition?( GlobalEnums.GatePosition.door ),
                EntryDelay = 1f,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false
            } );

            while( GameObject.Find( "Sly Basement NPC" ) == null )
                yield return new WaitForEndOfFrame();

            foreach( var roof in GameObject.FindObjectsOfType<Roof>() )
            {
                GameObject.Destroy( roof );
            }

            //remove the roofs
            GameObject.Destroy( GameObject.Find( "Chunk 0 0" ).GetComponents<EdgeCollider2D>()[ 1 ] );
            GameObject.Destroy( GameObject.Find( "Chunk 0 1" ).GetComponents<EdgeCollider2D>()[ 1 ] );

            
            GameObject.Destroy( GameObject.Find( "wall collider" ) );


            GameObject.Destroy( GameObject.Find( "Walk Area" ) );
            GameObject.Destroy( GameObject.Find( "Shop Menu" ) );
            GameObject.Destroy( GameObject.Find( "Sly Basement NPC" ) );
            GameObject.Destroy( GameObject.Find( "Roof Collider (2)" ) );
            GameObject.Destroy( GameObject.Find( "Roof Collider (1)" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0008_18" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0004_21" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0003_22" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0027_1 (3)" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0009_17 (3)" ) );
            GameObject.Destroy( GameObject.Find( "Sly_Storeroom_0027_1 (2)" ) );


            Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Room_Sly_Storeroom");

            GameObject.Destroy( s.FindGameObject( "Shop Item ShellFrag Sly1(Clone)" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item VesselFrag Sly1" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item Ch GeoGatherer(Clone)" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item Ch Wayward Compass(Clone)" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item Lantern(Clone)" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item White Key(Clone)" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item VesselFrag Sly1" ) );
            GameObject.Destroy( s.FindGameObject( "Shop Item VesselFrag Sly1(Clone)" ) );

            GameObject.Destroy( s.FindGameObject( "Dream Dialogue" ) );

            foreach( var roof in GameObject.FindObjectsOfType<SpriteRenderer>() )
            {
                if( roof.transform.position.x < 80f && roof.transform.position.x > -1f )
                {
                    if( roof.transform.position.y > 5f )
                        GameObject.Destroy( roof.gameObject );

                    else if( roof.transform.position.z < -2f )
                        GameObject.Destroy( roof.gameObject );
                }
            }

            foreach( var roof in GameObject.FindObjectsOfType<MeshRenderer>() )
            {
                GameObject.Destroy( roof );
            }

            SpawnLevelPart( "Platform_Block", new Vector3( 75f, 10f, 0f ) );
            SpawnLevelPart( "Platform_Long", new Vector3( 50f, 7.5f, 0f ) );
            SpawnLevelPart( "Platform_Block", new Vector3( 25f, 5f, 0f ) );

            //Good ground spawn: 64, 6, 0

            //TODO: Make the exit back to the previous scene work
            TransitionPoint exit = GameObject.Find( "door1" ).GetComponent<TransitionPoint>();
            exit.targetScene = currentScene;
            exit.entryPoint = currentSceneTransition;
        }

        //copied and modified from "TransitionPoint.cs"
        public GlobalEnums.GatePosition GetGatePosition(string name)
        {
            if( name.Contains( "top" ) )
            {
                return GlobalEnums.GatePosition.top;
            }
            if( name.Contains( "right" ) )
            {
                return GlobalEnums.GatePosition.right;
            }
            if( name.Contains( "left" ) )
            {
                return GlobalEnums.GatePosition.left;
            }
            if( name.Contains( "bot" ) )
            {
                return GlobalEnums.GatePosition.bottom;
            }
            if( name.Contains( "door" ) )
            {
                return GlobalEnums.GatePosition.door;
            }
            Dev.LogError( "Gate name " + name + "does not conform to a valid gate position type. Make sure gate name has the form 'left1'" );
            return GlobalEnums.GatePosition.unknown;
        }

        //from will be top1,left1,right1,door1,etc...
        public IEnumerator EnterZone(string name, string from, string waitUntilGameObjectIsLoaded = "", List<string> removeList = null )
        {
            //find a source transition
            string currentSceneTransition = GameObject.FindObjectOfType<TransitionPoint>().gameObject.name;
            string currentScene = GameManager.instance.sceneName;

            //update the last entered
            TransitionPoint.lastEntered = currentSceneTransition;

            //place us in sly's storeroom
            GameManager.instance.BeginSceneTransition( new GameManager.SceneLoadInfo
            {
                SceneName = name,
                EntryGateName = from,
                HeroLeaveDirection = new GlobalEnums.GatePosition?( GlobalEnums.GatePosition.door ),
                EntryDelay = 1f,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false
            } );

            if( !string.IsNullOrEmpty( waitUntilGameObjectIsLoaded ) )
            {
                while( GameObject.Find( waitUntilGameObjectIsLoaded ) == null )
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            if( removeList != null )
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
                foreach( string s in removeList )
                {
                    GameObject.Destroy( scene.FindGameObject( s ) );
                }
            }
        }

        public GameObject SpawnLevelPart(string name, Vector3 position)
        {
            GameObject go = (GameObject)GameObject.Instantiate( database.levelParts[name], position, Quaternion.identity );
            go.SetActive( true );
            return go;
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
            Dev.Where();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CheckAndDisableLogicInMenu;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CheckAndDisableLogicInMenu;
            ModHooks.Instance.AfterSavegameLoadHook += TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook += EnableEnemyRandomizerFromNewGame;
            ModHooks.Instance.SlashHitHook += DebugPrintObjectOnHit;
        }

        void UnRegisterCallbacks()
        {
            Dev.Where();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= CheckAndDisableLogicInMenu;
            ModHooks.Instance.AfterSavegameLoadHook -= TryEnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook -= EnableEnemyRandomizerFromNewGame;
            ModHooks.Instance.SlashHitHook -= DebugPrintObjectOnHit;
        }

        //used while testing to record things hit by a player's nail
        static string debugRecentHit = "";
        static void DebugPrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            //Dev.Where();
            if( otherCollider.gameObject.name != debugRecentHit )
            {
                Dev.Log( "Hero at " + HeroController.instance.transform.position + " HIT: " + otherCollider.gameObject.name + " at (" + otherCollider.gameObject.transform.position + ")" );
                debugRecentHit = otherCollider.gameObject.name;
            }
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

        Roguelike roguelike;

        void EnableEnemyRandomizer()
        {
            Dev.Where();
            Tools.SetNoclip( false );
            RandomizerReady = true;

            simulateReplacement = !loader.DatabaseGenerated;

            ChaosRNG = GlobalSettings.RNGChaosMode;
            RoomRNG = GlobalSettings.RNGRoomMode;
            RandomizeGeo = GlobalSettings.RandomizeGeo;
            CustomEnemies = GlobalSettings.CustomEnemies;

            roguelike = new Roguelike(); 
            GameManager.instance.StartCoroutine( roguelike.Init( GameSeed ) );
             
            //if( kmode )
            //{
            //    PlayerData instance = PlayerData.instance;
            //    if( instance != null )
            //    {
            //        instance.hasCharm = true;
            //        instance.hasQuill = true;
            //        instance.equippedCharm_2 = true;
            //        instance.hasMap = true;
            //        instance.mapDirtmouth = true;
            //        instance.mapCrossroads = true;
            //        instance.mapGreenpath = true;
            //        instance.mapFogCanyon = false;
            //        instance.mapRoyalGardens = true;
            //        instance.mapFungalWastes = false;
            //        instance.mapCity = true;
            //        instance.mapWaterways = false;
            //        instance.mapMines = true;
            //        instance.mapDeepnest = true;
            //        instance.mapCliffs = true;
            //        instance.mapOutskirts = true;
            //        instance.mapRestingGrounds = true;
            //        instance.mapAbyss = true;
            //        instance.openedMapperShop = true;
            //    }
            //}
        }

        //call when returning to the main menu
        void DisableEnemyRandomizer()
        {
            Tools.SetNoclip( false );
            RandomizerReady = false;
            Dev.Log( "Play Time was " + GameManager.instance.PlayTime );
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
    }
}
