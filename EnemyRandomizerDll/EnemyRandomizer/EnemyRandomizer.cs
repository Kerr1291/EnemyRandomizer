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

using System.IO;

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

        string fullVersionName = "0.2.0b";
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
            
            
            //TextAsset bindata = Resources.Load("_CP3") as TextAsset;
            //Dev.Log( bindata.text );


            ContractorManager.Instance.StartCoroutine( DebugInput() );
        }

        //bool suspended = false;
        IEnumerator DebugInput()
        {
            yield return new WaitForSeconds( 2f );
            MenuStyles.Instance.SetStyle( 4, true, false );

            GameObject prev = null;

            int i = 0;
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

                //if( UnityEngine.Input.GetKeyDown( KeyCode.V ) )
                //{
                //    while( GameObject.Find( "Hornet Boss 1" ) == null )
                //        yield return new WaitForEndOfFrame();

                //    GameObject.Find( "Hornet Boss 1" ).AddComponent<HornetBoss1>();
                //}

                //if( UnityEngine.Input.GetKeyDown( KeyCode.N ) )
                //{
                //    //MenuStyles.Instance.SetStyle( i, true, false );
                //    i = ( i + 1 ) % database.loadedEnemyPrefabs.Count;
                //    Dev.Log( "Index is now " + i + " = " + database.loadedEnemyPrefabNames[i] );
                //}
                //if( UnityEngine.Input.GetKeyDown( KeyCode.M ) )
                //{
                //    Vector3 worldPos = Vector3.zero;
                //    Vector2 mousePos = Input.mousePosition;
                //    Camera c = GameManager.instance.cameraCtrl.cam;

                //    Ray ray = c.ScreenPointToRay(new Vector3(mousePos.x,mousePos.y, c.nearClipPlane));
                //    worldPos = ray.origin + ray.direction * 26f;

                //    {
                //        Dev.Log( "Creating " + database.loadedEnemyPrefabNames[ i ] + " at " + worldPos );
                //        worldPos = new Vector3( worldPos.x, worldPos.y, 0f );
                //        prev = EnemyRandomizerLogic.Instance.CreateEnemy( database.loadedEnemyPrefabNames[ i ], worldPos );

                //        Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Room_Sly_Storeroom");
                //        GameObject enemyRoot = s.FindGameObject("_Enemies");
                //        if(enemyRoot != null)
                //        {
                //            prev.transform.SetParent( enemyRoot.transform );
                //        }
                //    }
                //}
                //if( UnityEngine.Input.GetKeyDown( KeyCode.K ) )
                //{
                //    if( prev != null )
                //    {
                //        HealthManager hm = prev.GetComponent<HealthManager>();
                //        hm.Die( null, AttackTypes.Generic, true );
                //    }
                //}
                if( UnityEngine.Input.GetKeyDown( KeyCode.P ) )
                {
                    if( prev != null )
                    {
                        System.IO.StreamWriter file = null;
                        file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/" + database.loadedEnemyPrefabNames[ i ] );
                        prev.PrintSceneHierarchyTree( true, file );
                        file.Close();
                    }
                }

                //if( suspended )
                //    Time.timeScale = 0f;

                //if( UnityEngine.Input.GetKeyDown( KeyCode.Q ) )
                //{
                //    Time.timeScale = 0f;
                //    suspended = true;
                //}
                //if( UnityEngine.Input.GetKeyDown( KeyCode.W ) )
                //{
                //    Time.timeScale = 1f;
                //    yield return new WaitForEndOfFrame();
                //    yield return new WaitForEndOfFrame();
                //    Time.timeScale = 0f;
                //}
                //if( UnityEngine.Input.GetKey( KeyCode.R ) )
                //{
                //    Time.timeScale = 1f;
                //    yield return new WaitForEndOfFrame();
                //    yield return new WaitForEndOfFrame();
                //    Time.timeScale = 0f;
                //}
                //if( UnityEngine.Input.GetKeyDown( KeyCode.E ) )
                //{
                //    Time.timeScale = 1f;
                //    suspended = false;
                //}

                if( UnityEngine.Input.GetKeyDown( KeyCode.A ) )
                {
                    //var hornet = database.loadedEnemyPrefabs[0];

                    ////TODO: test title
                    //yield return HornetBoss1.GetGameObjectFromFSM( hornet, "Control", "Flourish", SetAreaTitleReference );
                    //areaTitleObject.SetActive( true );
                    //ShowBossTitle("#MODDING STRIKES BACK", "GET READY FOR", "PART 2: ELECTRIC BOOGALOO", "HORNET");
                    //yield return new WaitForSeconds( 2f );
                    //HideBossTitle();
                    //yield return new WaitForSeconds( 2f );
                    //areaTitleObject.SetActive( false );
                    //yield return new WaitForEndOfFrame();

                    //System.IO.StreamWriter file = null;
                    //file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/" + areaTitleObject.name );
                    //areaTitleObject.gameObject.PrintSceneHierarchyTree( true, file );
                    //file.Close();

                    //float t = 10f;
                    //while(t > 0f)
                    //{
                    //    t -= Time.deltaTime;
                    //    HeroController.instance.transform.position = areaTitleObject.transform.position;
                    //    yield return new WaitForEndOfFrame();
                    //}

                    //areaTitleObject.PrintSceneHierarchyTree( true );

                    ////var hornet = GameObject.Find("Hornet Boss 1");
                    //var audioOneShot = hornet.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>("Flourish","Control");
                    //var up = Vector3.up;
                    //var pos = HeroController.instance.transform.position;
                    //var aPlayer = audioOneShot.audioPlayer.Value;
                    //var go = aPlayer.Spawn(pos, Quaternion.Euler(up));
                    //var audioSource = go.GetComponent<AudioSource>();
                    //var clip = audioSource.clip;
                    //float pitch = GameRNG.Rand(audioOneShot.pitchMin.Value,audioOneShot.pitchMax.Value);
                    //audioSource.pitch = pitch;
                    //audioSource.volume = audioOneShot.volume.Value;
                    //if(clip != null)
                    //{
                    //    audioSource.PlayOneShot( clip );
                    //}
                    //else
                    //{
                    //    Dev.Log( "whyyy" );
                    //}
                    //System.IO.StreamWriter file = null;
                    //file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/" + "HornetResource" );
                    //hornet.PrintSceneHierarchyTree( true, file );
                    //file.Close();

                    //Dev.Log( "trying snapshot" );
                    //var snapshot = hornet.GetFSMActionOnState<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>("Flourish","Control");
                    //var mixerSnapshot = snapshot.snapshot.Value as UnityEngine.Audio.AudioMixerSnapshot;
                    //mixerSnapshot.TransitionTo( 1f );

                    //var areaTitle = hornet.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetGameObject>("Flourish","Control");

                    //Dev.Log( ""+ areaTitle.gameObject.Value.transform.position );

                    //foreach( Component c in areaTitle.gameObject.Value.GetComponents<Component>() )
                    //{
                    //    Dev.Log( "c = " + c.GetType().Name );
                    //}

                    //var title = areaTitle.gameObject.Value;

                    /*
                    Dev.Log( "trying title" );
                    GameObject title = GameObject.Instantiate( Resources.FindObjectsOfTypeAll<AreaTitle>()[0].gameObject );
                    title.transform.position = HeroController.instance.transform.position;
                    title.gameObject.SetActive( true );

                    System.IO.StreamWriter file = null;
                    file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/" + "AreaTitle" );
                    title.gameObject.PrintSceneHierarchyTree( true, file );
                    file.Close();

                    PlayMakerFSM fsm = FSMUtility.GetFSM(title.gameObject);
                    if( fsm )
                    {
                        FSMUtility.SetBool( fsm, "Visited", true );
                        FSMUtility.SetBool( fsm, "NPC Title", true );
                        FSMUtility.SetBool( fsm, "Display Right", false );
                        FSMUtility.SetString( fsm, "Area Event", "HORNET" );
                    }
                    */

                    //Dev.Log( "trying title 2" );
                    //PlayMakerFSM titleFSM = title.GetComponent<PlayMakerFSM>();
                    //var fsmString = hornet.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetFsmString>("Flourish","Control");

                    //Dev.Log( "trying title 3" );
                    //HutongGames.PlayMaker.FsmString fString = titleFSM.Fsm.GetFsmString(fsmString.variableName.Value);
                    //fString.Value = "NOPE"; //fsmString.setValue.Value;

                    //Dev.Log( "trying music" ); 
                    //var musicCue = hornet.GetFSMActionOnState<HutongGames.PlayMaker.Actions.ApplyMusicCue>("Flourish","Control");
                    //MusicCue mc = musicCue.musicCue.Value as MusicCue;
                    //GameManager instance = GameManager.instance;
                    //instance.AudioManager.ApplyMusicCue( mc, 0f, 0f, false );


                    //GameObject h = GameObject.Find("Hornet Boss 1");
                    //PlayMakerFSM p = h.GetMatchingFSMComponent("Control","Flourish","SendEventByName");
                    //var e = h.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SendEventByName>("Flourish","Control");
                    //e.Fsm.Event( e.eventTarget, e.sendEvent.Value );

                    //foreach(var a in h.GetComponents<AudioSource>())
                    //{
                    //    Dev.Log( a.clip.name );
                    //}

                    //System.IO.StreamWriter file = null;
                    //file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/GameManager" );
                    //GameManager.instance.gameObject.PrintSceneHierarchyTree( true, file );
                    //file.Close();
                }
                //enter hornet
                if( UnityEngine.Input.GetKeyDown( KeyCode.S ) )
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
        //static PhysicsMaterial2D hbMat;
        //used while testing to record things hit by a player's nail
        static string debugRecentHit = "";
        static void DebugPrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            if( otherCollider.gameObject.name != debugRecentHit )
            {
                Dev.Log( "Hero at "+HeroController.instance.transform.position+" HIT: " + otherCollider.gameObject.name + " at (" + otherCollider.gameObject.transform.position + ")");
                debugRecentHit = otherCollider.gameObject.name;
            }

            //if( hbMat == null )
            //{
            //    hbMat = new PhysicsMaterial2D( "hb" );
            //    hbMat.bounciness = .9f;
            //    hbMat.friction = 0f;
            //}

            //if(HeroController.instance.playerData.equippedCharm_15)
            //{
            //    Rigidbody2D body = otherCollider.GetComponentInParent<Rigidbody2D>();
            //    if( body != null )
            //    {
            //        Vector2 blowDirection = otherCollider.transform.position - HeroController.instance.transform.position;
            //        float blowPower = 80f;
            //        body.sharedMaterial = hbMat;
            //        body.velocity += blowDirection.normalized * blowPower;
            //        body.isKinematic = false;
            //        body.interpolation = RigidbodyInterpolation2D.Interpolate;
            //        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            //        body.angularVelocity = 20f;
            //        body.gameObject.AddComponent<TakeDamageFromImpact>().blowVelocity = blowDirection.normalized * blowPower;
            //        body.gameObject.AddComponent<PreventOutOfBounds>();
            //    }
            //}
        }
    }

    //public class TakeDamageFromImpact : MonoBehaviour
    //{
    //    public Vector2 blowVelocity;
    //    HealthManager healthManager;
    //    Rigidbody2D body;
    //    void OnEnable()
    //    {
    //        healthManager = GetComponent<HealthManager>();
    //        body = GetComponent<Rigidbody2D>();
    //    }

    //    void Update()
    //    {
    //        body.position += blowVelocity;
    //        body.rotation += 5f;
    //        blowVelocity = blowVelocity * .92f;
    //        if( blowVelocity.magnitude <= 0.1f )
    //            Destroy( this );
    //    }

    //    void OnCollisionEnter2D( Collision2D collision )
    //    {
    //        HitInstance hit = new HitInstance()
    //        {
    //            AttackType = AttackTypes.Splatter,
    //            CircleDirection = false,
    //            DamageDealt = (int)blowVelocity.magnitude,
    //            Direction = 0f,
    //            IgnoreInvulnerable = false,
    //            IsExtraDamage = false,
    //            MagnitudeMultiplier = 1f,
    //            MoveAngle = 0f,
    //            MoveDirection = false,
    //            Multiplier = 1f,
    //            Source = HeroController.instance.gameObject,
    //            SpecialType = SpecialTypes.None
    //        };

    //        if( collision.gameObject.layer == 8 )
    //        {
    //            healthManager.Hit( hit );
    //            blowVelocity = collision.contacts[ 0 ].normal * blowVelocity.magnitude;
    //        }

    //        if( collision.gameObject.layer == 11 )
    //        {
    //            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
    //            if(rb != null)
    //            {
    //                blowVelocity = collision.contacts[ 0 ].normal * blowVelocity.magnitude;
    //                collision.gameObject.AddComponent<TakeDamageFromImpact>().blowVelocity = blowVelocity;
    //                collision.gameObject.AddComponent<PreventOutOfBounds>();
    //                //rb.velocity += body.velocity;
    //                healthManager.Hit( hit );
    //            }
    //        }
    //    }
    //}
}
