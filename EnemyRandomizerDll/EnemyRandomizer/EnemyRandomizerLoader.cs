using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

using ModCommon;
using HutongGames.PlayMaker.Actions;

namespace EnemyRandomizerMod
{   
    public class EnemyRandomizerLoader
    {
        public static EnemyRandomizerLoader Instance { get; private set; }

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        Contractor databaseLoader;

        IEnumerator randomizerSceneProcessor = null;

        public bool DatabaseGenerated { get; private set; }

        int currentDatabaseIndex = 0;

        int loadCount = 0;

        string currentlyLoadingScene = "";

        Scene lastLoadedScene;

        bool IsLoadingDatabase {
            get {
                return randomizerSceneProcessor != null;
            }
        }            

        float databaseLoadProgress = 0f;
        float DatabaseLoadProgress {
            get {
                return databaseLoadProgress;
            }
            set {
                databaseLoadProgress = value;
                
                comms.Publish( new LoadingProgressEvent() { progress = value } );
            }
        }

        public EnemyRandomizerLoader( EnemyRandomizerDatabase database )
        {
            this.database = database;
        }

        public void Setup()
        {
            Instance = this;

            comms = new CommunicationNode();
            comms.EnableNode( this );
        }

        public void Unload()
        {
            comms.DisableNode();
            
            Instance = null;
        }

        /// <summary>
        /// Initial workhorse funciton. Load all the enemy types in the game.
        /// </summary>
        /// 
        void LoadSceneData()
        {
            GameObject root = EnemyRandomizer.Instance.ModRoot;
            
            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                Scene sceneToLoad = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if( sceneToLoad.name == Menu.RandomizerMenu.MainMenuSceneName )
                    continue;

                LoadLevelParts( sceneToLoad );

                currentlyLoadingScene = sceneToLoad.name;

                lastLoadedScene = sceneToLoad;

                Dev.Log( "Loading Scene [" + currentlyLoadingScene + "]" );

                foreach( var go in Resources.FindObjectsOfTypeAll<HealthManager>() )
                {
                    string name = go.name;

                    name = name.TrimGameObjectName();

                    if( name.IsSkipLoadingString() )
                        continue;

                    if( name == "Hatcher" && !database.loadedEnemyPrefabNames.Contains( "Hatcher Baby" ) )
                        continue;

                    if( go.gameObject.IsGameEnemy() )
                    {
                        bool isInLoadedList = database.loadedEnemyPrefabNames.Contains(name);
                        if( isInLoadedList )
                            continue;

                        Dev.Log( "Loading enemy " + go.name );
                        GameObject prefab = go.gameObject;
                        if( name.IsCopyOnLoadingString() )
                        {
                            prefab = (GameObject)GameObject.Instantiate( go.gameObject );
                        }

                        //special logic for certain enemies:
                        prefab = ModifyGameObjectPrefab( prefab, name );

                        database.loadedEnemyPrefabs.Add( prefab );
                        database.loadedEnemyPrefabNames.Add( name );
                        Dev.Log( "Adding enemy type: " + prefab.name + " to list with search string " + name );
                    }//end if-enemy
                }//iterate over resources

                //iterate over the the game objects in the scene
                foreach(GameObject rootGo in sceneToLoad.GetRootGameObjects())
                {
                    foreach( Transform t in rootGo.GetComponentsInChildren<Transform>() )
                    {
                        string name = t.gameObject.name;

                        name = name.TrimGameObjectName();

                        bool isInLoadedEffectList = database.loadedEffectPrefabs.ContainsKey( name );
                        if( isInLoadedEffectList )
                            continue;

                        bool isInEffectList = EnemyRandomizerDatabase.effectObjectNames.Contains( name );
                        if( !isInEffectList )
                            continue;

                        GameObject effectPrefab = t.gameObject;
                        GameObject.DontDestroyOnLoad( effectPrefab );
                        effectPrefab.transform.SetParent( root.transform );

                        //special logic for certain enemies:
                        effectPrefab = ModifyGameObjectPrefab( effectPrefab, name );

                        database.loadedEffectPrefabs.Add( name, effectPrefab );
                        Dev.Log( "Adding enemy effect: " + effectPrefab.name + " to loaded effect list with key " + name );
                        break;
                    }
                }//iterate over the game objects in the scene
            }//iterate over all LOADED scenes       
        }//end LoadSceneData()

        GameObject ModifyGameObjectPrefab( GameObject randoPrefab, string name )
        {
            Transform root = EnemyRandomizer.Instance.ModRoot.transform;
            Vector2 customWakeAreaSize = new Vector2( 40f, 20f );
            
            GameObject modifiedPrefab = randoPrefab;
            
            //modifications done to all enemies
            modifiedPrefab.SetActive( false );
            GameObject.DontDestroyOnLoad( modifiedPrefab );
            modifiedPrefab.transform.SetParent( root.transform );
            int i = 0;
            Dev.Log( "TRYING "+ i++ );
            //delete persistant bool items
            { 
                PersistentBoolItem pbi = modifiedPrefab.GetComponent<PersistentBoolItem>();
                if( pbi != null )
                {
                    GameObject.Destroy( pbi );
                }
            }

            //remove this, because it can deactivate some enemies....
            {
                DeactivateIfPlayerdataTrue toRemove = modifiedPrefab.GetComponent<DeactivateIfPlayerdataTrue>();
                if( toRemove != null )
                {
                    GameObject.Destroy( toRemove );
                }
            }

            Dev.Log( "TRYING " + i++ );
            //remove any FSMs that have a persistant bool check
            if(name != "Mender Bug" )
            {
                PlayMakerFSM deleteFSM = modifiedPrefab.GetMatchingFSMComponent("FSM","Check","PlayerDataBoolTest");
                //remove the persistant bool check item
                if( deleteFSM != null )
                {
                    GameObject.Destroy( deleteFSM );
                }
            }

            //modifiactions to specific enemies below

            Dev.Log( "TRYING " + i++ );
            //Create a custom "wake up" base game object and put it on the mage knight
            if( name == "Mage Knight" )
            {
                GameObject wakeUpRoot = new GameObject( "MK Wake Up Object" );
                wakeUpRoot.transform.SetParent( modifiedPrefab.transform );
                wakeUpRoot.transform.localPosition = Vector3.zero;

                wakeUpRoot.layer = 13; //try this
                wakeUpRoot.tag = modifiedPrefab.tag;

                BoxCollider2D box = wakeUpRoot.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = customWakeAreaSize;

                WakeUpMageKnight specialWakeUp = wakeUpRoot.AddComponent<WakeUpMageKnight>();
                specialWakeUp.collider = box;
                specialWakeUp.mageKnight = modifiedPrefab;
            }
            else if( name == "Electric Mage" )
            {
                Dev.Log( "TRYING " + i++ );
                //try to fix the electric mage
                DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Electric Mage", "Init", new List<string>() { "FINISHED" }, true, customWakeAreaSize, false );
            }
            else if( name == "Black Knight" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                //try to fix the electric mage
                DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Black Knight", "Rest", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
            }
            else if( name == "Mage" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                //try to fix the mage
                DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Mage", "Manual Sleep", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
            }
            else if( name == "Mage Lord" || "Dream Mage Lord" == name )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                //try to fix the mage
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Mage Lord", "Sleep", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
                }

                PlayMakerFSM destroy = modifiedPrefab.GetMatchingFSMComponent( "Destroy If Defeated", "Check", "PlayerDataBoolTest" );
                if( destroy != null )
                {
                    GameObject.Destroy( destroy );
                }
            }
            else if( name == "Zombie Beam Miner Rematch" || name == "Mega Zombie Beam Miner" )
            {
            Dev.Log( "TRYING " + ( i++ ).ToString() );
                //remove the "Cam Lock" game object child from the crystal guardian (Zombie Beam Miner Rematch)
                modifiedPrefab.FindAndDestroyGameObjectInChildren( "Cam Lock" );
            }
            else if( name == "Slash Spider" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                //fix the slash spider from getting stuck
                DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Slash Spider", "Waiting", new List<string>() { "WAKE" }, true, null, false );
            }
            else if( name == "Mender Bug" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    List<PlayerDataBoolTest> actions = modifiedPrefab.GetFSMActionsOnStates<PlayerDataBoolTest>( new List<string>() { "Sign Broken?" }, "Mender Bug Ctrl" );
                    foreach( var a in actions )
                    {
                        a.boolName = "openingCreditsPlayed";
                    }
                }
                {
                    List<RandomInt> actions = modifiedPrefab.GetFSMActionsOnStates<RandomInt>( new List<string>() { "Chance" }, "Mender Bug Ctrl" );
                    foreach( var a in actions )
                    {
                        a.min = 1;
                        a.max = 1;
                    }
                }
                {
                    List<IntCompare> actions = modifiedPrefab.GetFSMActionsOnStates<IntCompare>( new List<string>() { "Chance" }, "Mender Bug Ctrl" );
                    foreach( var a in actions )
                    {
                        a.integer1 = 1;
                        a.integer2 = 1;
                    }
                }
            }
            else if( name == "Hatcher" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    GameObject emptyRoot = new GameObject( "EmptyRoot" );
                    emptyRoot.transform.position = Vector3.zero;
                    List<GetRandomChild> actions = modifiedPrefab.GetFSMActionsOnStates<GetRandomChild>( new List<string>() { "Fire" }, "Hatcher" );
                    foreach( var a in actions )
                    {
                        GameObject hatcherBaby = database.loadedEnemyPrefabs[ database.loadedEnemyPrefabNames.IndexOf( "Hatcher Baby" ) ];
                        hatcherBaby.transform.SetParent( emptyRoot.transform );
                        a.gameObject = new HutongGames.PlayMaker.FsmOwnerDefault() { GameObject = emptyRoot, OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.SpecifyGameObject };
                    }
                }
            }
            else if( name == "Infected Knight" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    List<PlayerDataBoolTest> actions = modifiedPrefab.GetFSMActionsOnStates<PlayerDataBoolTest>( new List<string>() { "Init" }, "IK Control" );
                    foreach( var a in actions )
                    {
                        a.boolName = "openingCreditsPlayed";
                    }
                }
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "IK Control", "Sleep", new List<string>() { "BATTLE START" }, true, customWakeAreaSize, false );
                }
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "IK Control", "Waiting", new List<string>() { "BATTLE START" }, true, customWakeAreaSize, false );
                }
            }
            else if( name == "Jar Collector" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Control", "Sleep", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
                }
            }
            else if( name == "Hornet Boss 1" )
            {
                //TODO: add our custom component to her and her needle and thread
                //modifiedPrefab.AddComponent<HornetBoss1>();

                //Dev.Log( "TRYING " + ( i++ ).ToString() );
                //{
                //    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Control", "Inert", new List<string>() { "REFIGHT" }, true, customWakeAreaSize, false );
                //}
                //{
                //    List<IntCompare> actions = modifiedPrefab.GetFSMActionsOnStates<IntCompare>( new List<string>() { "Inert" }, "Control" );
                //    foreach( var a in actions )
                //    {
                //        a.integer1 = 0;
                //        a.integer2 = 0;
                //    }
                //}
            }
            else if( name == "Moss Charger" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Mossy Control", "Hidden", new List<string>() { "IN RANGE" }, true, customWakeAreaSize, false );
                }
                //{
                //    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Mossy Control", "Hero Beyond?", new List<string>() { "FINISHED" }, true, customWakeAreaSize, false );
                //}
            }
            else if( name == "Mushroom Brawler" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Shroom Brawler", "Sleep", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
                }
            }
            else if( name == "Garden Zombie" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Attack", "Spawn Idle", new List<string>() { "SPAWN" }, true, customWakeAreaSize, false );
                }
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Attack", "Idle", new List<string>() { "HERO IN RANGE" }, true, customWakeAreaSize, false );
                }
            }
            else if( name == "Mantis Traitor Lord" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "", "", new List<string>(), true, null, true );
                }
            }
            else if( name == "Hive Knight" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                //{
                //    List<PlayerDataBoolTest> actions = modifiedPrefab.GetFSMActionsOnStates<PlayerDataBoolTest>( new List<string>() { "Sign Broken?" }, "Control" );
                //    foreach( var a in actions )
                //    {
                //        a.boolName = "openingCreditsPlayed";
                //    }
                //}
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Control", "Sleep", new List<string>() { "WAKE" }, true, customWakeAreaSize, false );
                }
            }


            if( name.Contains( "Flamebearer" ) )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Control", "Init", new List<string>() { "START" }, true, customWakeAreaSize, false );
            }

            return modifiedPrefab;
        }

        public void BuildEnemyRandomizerDatabase()
        {
            Dev.Where();
            randomizerSceneProcessor = DoBuildDatabase();

            databaseLoader = new Contractor
            {
                OnUpdate = BuildDatabase,
                Looping = true
            };
            databaseLoader.SetUpdateRate( Contractor.UpdateRateType.Frame );
            databaseLoader.Start();
        }

        public int GetSceneToLoadFromRandomizerData(int databaseIndex)
        {
            return EnemyRandomizerDatabase.EnemyTypeScenes[ databaseIndex ];
        }

        protected virtual void AdditivelyLoadCurrentScene()
        {
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ), LoadSceneMode.Additive );
                loadCount++;
            }
            catch( Exception e )
            {
                Dev.Log( "Exception from scene "+ currentlyLoadingScene +" #" + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) +" with message: "+ e.Message );
            }
        }

        protected virtual void IncrementCurrentSceneIndex()
        {
            currentDatabaseIndex += 1;
        }

        protected virtual void ProcessCurrentSceneForDataLoad()
        {
            try
            {
                Dev.Log( "Loading scene data: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                LoadSceneData();
                
                DatabaseLoadProgress = currentDatabaseIndex / (float)(EnemyRandomizerDatabase.EnemyTypeScenes.Count - 1);
                Dev.Log( "Loading Progress: " + DatabaseLoadProgress );

                Dev.Log( "Unloading scene: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                UnityEngine.SceneManagement.SceneManager.UnloadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
            }
            catch( Exception e )
            {
                Dev.Log( "Exception from scene " + currentlyLoadingScene + " #" + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) + " with message: " + e.Message );
            }
        }

        protected virtual void CompleteEnemyRandomizerDataLoad()
        {
            Dev.Log( "Loaded data from " + loadCount + " scenes." );
            //PrintDebugLoadingError();

            //TODO:uncomment
            //For debugging: print all the loaded enemies
            //foreach( GameObject go in database.loadedEnemyPrefabs )
            //{
            //    go.PrintSceneHierarchyTree( true );
            //}

            //print em
            //foreach(var r in Resources.FindObjectsOfTypeAll<Transform>() )
            //{
            //    System.IO.StreamWriter file = null;
            //    file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/Resources/" + r.name );
            //    r.gameObject.PrintSceneHierarchyTree( true, file );
            //    file.Close();
            //}

            Dev.LogVarArray( "Resources", Resources.FindObjectsOfTypeAll<Transform>() );
            Dev.LogVarArray( "PersistentBools", Resources.FindObjectsOfTypeAll<PersistentBoolItem>() );
            Dev.LogVarArray( "EnemyResources", Resources.FindObjectsOfTypeAll<HealthManager>() );
            //Dev.LogVarArray( "Resources", Resources.FindObjectsOfTypeAll<Transform>() );
            //Dev.LogVarArray( "Enemy Prefabs", database.loadedEnemyPrefabs );
            Dev.LogVarArray( "Enemies", database.loadedEnemyPrefabNames );
            //Dev.LogVarArray( "ScenesToSkip", database.emptyScenesToSkipOnLoad );
            //Dev.LogVarArray( "ScenesWithEnemies", database.scenesLoaded );

            //For debugging: print all the loaded effects
            //foreach( var effect in database.loadedEffectPrefabs )
            //{
            //    effect.Value.PrintSceneHierarchyTree( true );
            //}


            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;

            DatabaseGenerated = true;
            databaseLoader.Reset();
            randomizerSceneProcessor = null;

            GameManager.instance.LoadFirstScene();
        }

        protected virtual bool IsDoneLoadingRandomizerData()
        {
            //return ( currentDatabaseIndex + 1 ) >= 421;
            return ( currentDatabaseIndex + 1) >= EnemyRandomizerDatabase.EnemyTypeScenes.Count;
        }

        protected virtual void BuildDatabase()
        {
            if( randomizerSceneProcessor != null && !randomizerSceneProcessor.MoveNext() )
            {
                randomizerSceneProcessor.Reset();
            }

            if( randomizerSceneProcessor != null && ( randomizerSceneProcessor.Current as bool? ) == false )
            {
                randomizerSceneProcessor.Reset();
            }
        }

        protected IEnumerator DoBuildDatabase()
        {
            while( !DatabaseGenerated )
            {
                AdditivelyLoadCurrentScene();
                
                //wait until all scenes are loaded
                for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; )
                {
                    bool status = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isLoaded && UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).IsValid();
                    if(!status)
                    {
                        i = 0;
                        yield return true;
                    }
                    else
                    {
                        ++i;
                    }
                }

                ProcessCurrentSceneForDataLoad();

                yield return true;

                if( IsDoneLoadingRandomizerData() )
                {
                    CompleteEnemyRandomizerDataLoad();
                }
                else
                {
                    IncrementCurrentSceneIndex();
                }

                yield return true;
            }

            yield return false;
        }

        public void LoadLevelParts(Scene scene)
        {
            //load some scene parts for use in things
            if( scene.buildIndex == 367 )
            {
                {
                    LoadLevelPart( "wp_plat_float_05", "Platform_Block" );
                    LoadLevelPart( "wp_plat_float_01_wide", "Platform_Long" );
                    
                    LoadLevelPart( "white_palace_floor_set_02 (22)", "Floor" );
                    LoadLevelPart( "white_palace_wall_set_01 (9)", "Wall" );
                }
            }
        }

        public void LoadLevelPart(string name, string customName = "")
        {
            GameObject go2 = GameObject.Find(name);
            GameObject go = null;
            if( go2 != null )
                go = GameObject.Instantiate( go2 );
            if( go != null )
            {
                Transform root = EnemyRandomizer.Instance.ModRoot.transform;
                GameObject.DontDestroyOnLoad( go );
                go.SetActive( false );
                string addName = name;
                if( !string.IsNullOrEmpty( customName ) )
                {
                    addName = customName;
                    go.name = customName;
                }
                database.levelParts.Add( addName, go );
                go.transform.SetParent( root );
            }
        }

    }
}



//protected virtual void PrintDebugLoadingError()
//{
//    bool printInitial = true;
//    foreach( string enemy in EnemyRandomizerDatabase.enemyTypeNames )
//    {
//        if( database.loadedEnemyPrefabNames.Contains( enemy ) )
//            continue;

//        if( printInitial )
//        {
//            Dev.Log( "Enemies not loaded so far:" );
//            printInitial = false;
//        }

//        Dev.Log( "Missing type: " + enemy );
//    }
//}





//if( addToSkip )
//{
//    if(!database.emptyScenesToSkipOnLoad.Contains( sceneToLoad.buildIndex ) )
//        database.emptyScenesToSkipOnLoad.Add( sceneToLoad.buildIndex );
//}
//else
//{
//    database.scenesLoaded.Add( sceneToLoad.buildIndex );
//}

//GameObject[] rootGameObjects = sceneToLoad.GetRootGameObjects();
//foreach( GameObject go in rootGameObjects )
//{
//    //ignore the mod root
//    if( go.name == root.name )
//        continue;

//    if( sceneToLoad.buildIndex == 244 )
//    {
//        int indexOfEffectType = EnemyRandomizerDatabase.effectObjectNames.IndexOf(go.name);

//        if( indexOfEffectType >= 0 )
//        {
//            GameObject prefab = go;
//            GameObject.DontDestroyOnLoad( prefab );
//            prefab.transform.SetParent( root.transform );

//            //special logic for certain enemies:
//            prefab = ModifyGameObjectPrefab( prefab );

//            //don't actually add this one
//            //database.loadedEffectPrefabs.Add( EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ], prefab );
//            Dev.Log( "Saving special enemy effect: " + prefab.name );
//        }//end if-enemy
//    }

//    //save off teleplanes
//    //if( sceneToLoad.buildIndex == 96 )
//    {
//        if(go.name.Contains( "Teleplanes" ) )
//        {
//            GameObject prefab = go;
//            GameObject.DontDestroyOnLoad( prefab );
//            prefab.transform.SetParent( root.transform );

//            //don't actually add this one, keep it around because the mages need it
//            Dev.Log( "Saving special enemy effect: " + prefab.name );
//            continue;
//        }
//    }

//    bool isInEffectList = database.loadedEffectPrefabs.ContainsKey(go.name);
//    if( isInEffectList )
//        continue;

//    //load beam effects
//    if( sceneToLoad.buildIndex == 241 )
//    {
//        int indexOfEffectType = EnemyRandomizerDatabase.effectObjectNames.IndexOf(go.name);

//        if( indexOfEffectType >= 0 )
//        {
//            GameObject prefab = go;
//            GameObject.DontDestroyOnLoad( prefab );
//            prefab.transform.SetParent( root.transform );

//            //special logic for certain enemies:
//            prefab = ModifyGameObjectPrefab( prefab );

//            database.loadedEffectPrefabs.Add( EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ], prefab );
//            Dev.Log( "Adding enemy effect: " + prefab.name + " to loaded effect list with search string " + EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ] );
//        }//end if-enemy
//    }

//    foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
//    {
//        string name = t.gameObject.name;

//        if( name.IsSkipLoadingString() )
//            continue;

//        name = name.TrimGameObjectName();

//        bool isInLoadedList = database.loadedEnemyPrefabNames.Contains(name);
//        if( isInLoadedList )
//            continue;

//        //if( debugOnce && name.Contains( "Zombie Beam Miner" ) && !name.Contains( "Rematch" ) )
//        //{
//        //    debugOnce = false;
//        //    //t.gameObject.PrintSceneHierarchyTree( true );
//        //    sceneToLoad.PrintHierarchy( i );
//        //}

//        //int indexOfRandomizerEnemyType = EnemyRandomizerDatabase.enemyTypeNames.IndexOf(name);

//        //if( indexOfRandomizerEnemyType >= 0 && t.gameObject.IsGameEnemy() )
//        //{
//        if( t.gameObject.IsGameEnemy() )
//        {
//            //addToSkip = false;

//            //Dev.Log( "Loading index " + indexOfRandomizerEnemyType );
//            Dev.Log( "Loading enemy " + t.gameObject.name );
//            GameObject prefab = null;
//            //if( name.Contains("Zombie Beam Miner") || name == "Mage" )
//            //{
//            //TODO: test
//                prefab = t.gameObject;
//            //}
//            //else
//            //{
//            //    prefab = GameObject.Instantiate(t.gameObject);
//            //}

//            prefab.SetActive( false );
//            GameObject.DontDestroyOnLoad( prefab );
//            prefab.transform.SetParent( root.transform );

//            //special logic for certain enemies:
//            prefab = ModifyGameObjectPrefab( prefab );

//            database.loadedEnemyPrefabs.Add( prefab );
//            //database.loadedEnemyPrefabNames.Add( EnemyRandomizerDatabase.enemyTypeNames[indexOfRandomizerEnemyType] );
//            database.loadedEnemyPrefabNames.Add( name );
//            //Dev.Log( "Adding enemy type: " + prefab.name + " to list with search string " + EnemyRandomizerDatabase.enemyTypeNames[ indexOfRandomizerEnemyType ] );
//            Dev.Log( "Adding enemy type: " + prefab.name + " to list with search string " + name );
//        }//end if-enemy
//    }//end foreach transform in the root game objects
//}//end for each root game object

//if(addToSkip)
//{
//    database.emptyScenesToSkipOnLoad.Add( sceneToLoad.buildIndex );
//}
//else
//{   
//    database.scenesLoaded.Add( sceneToLoad.buildIndex );
//}



////find and remove the FSM that kills the boss
//for(int i = 0; i < modifiedPrefab.GetComponentsInChildren<PlayMakerFSM>().Length; ++i )
//{
//    PlayMakerFSM fsm = modifiedPrefab.GetComponentsInChildren<PlayMakerFSM>()[i];
//    if(fsm.FsmName == "FSM")
//    {
//        foreach( var s in fsm.FsmStates )
//        {
//            if( s.Name == "Check" )
//            {
//                foreach( var a in s.Actions )
//                {
//                    if(a.GetType().Name == "PlayerDataBoolTest" )
//                    {
//                        deleteFSM = fsm;
//                        break;
//                    }
//                }

//                if( deleteFSM != null )
//                    break;
//            }
//        }
//    }

//    if( deleteFSM != null )
//        break;
//}

//TODO: don't think i need this
//DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Beam Miner", new List<string>() { "FINISHED" } );
//d.monitorFSMStates = true;
//d.sendWakeEventsOnState = "Beam End";
//d.logFSM = false;

//if( modifiedPrefab.name.Contains( "Garden" ) )
//{
//    //    //this fixes the slash spider!
//    //    //DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
//    //    DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
//    //    d.monitorFSMStates = true;
//    //    //d.monitorFSMStates = true;
//    //    //d.sendWakeEventsOnState = "Waiting";
//    //    //d.logFSM = false;

//    //    //HutongGames.PlayMaker.Actions.GetColliderRange gcr = modifiedPrefab.GetFSMActionOnState<HutongGames.PlayMaker.Actions.GetColliderRange>("Select Target","Mage");

//    //    //Dev.Log( "PRINTING!!!" );
//    //    //PlayMakerFSM fsm = FSMUtility.LocateFSM( modifiedPrefab, "Mage" );
//    //    //if( fsm != null && gcr != null )
//    //    //{
//    //    //    if( gcr.gameObject != null )
//    //    //    {
//    //    //        Dev.Log( "DebugFSMS OO -- " + gcr.gameObject.OwnerOption );
//    //    //        Dev.Log( "DebugFSMS GO -- " + gcr.gameObject.GameObject );
//    //    //    }
//    //    //    if( gcr.gameObject.GameObject != null )
//    //    //    {
//    //    //        Dev.Log( "DebugFSMS GON -- " + gcr.gameObject.GameObject.Name );
//    //    //    }
//    //    //}


//    //TODO: move all this debug crap into a function 

//    //{
//    //    HutongGames.PlayMaker.Actions.FindGameObject y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.FindGameObject>("Initialise","attack_range_detect");

//    //    if( y != null )
//    //    {
//    //        if( y.objectName != null )
//    //            Dev.Log( "DebugFSMS FindGameObject objectName --- " + y.objectName.Value );
//    //        if( y.withTag != null )
//    //            Dev.Log( "DebugFSMS FindGameObject withTag --- " + y.withTag.Value );
//    //        if( y.store != null )
//    //            Dev.Log( "DebugFSMS FindGameObject store --- " + y.store.Name );
//    //    }
//    //}

//    //{
//    //    List<HutongGames.PlayMaker.Actions.GetPosition> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.GetPosition>("Raycast","attack_range_detect");

//    //    if( y != null )
//    //    {
//    //        foreach( var yy in y )
//    //        {
//    //            Dev.Log( "DebugFSMS GetPosition Name --- " + yy.gameObject.GameObject.Name );
//    //            Dev.Log( "DebugFSMS GetPosition Value --- " + yy.gameObject.GameObject.Value );
//    //        }
//    //    }
//    //}

//    //{
//    //    List<HutongGames.PlayMaker.Actions.SetFloatValue> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetFloatValue>("Raycast","attack_range_detect");

//    //    if( y != null )
//    //    {
//    //        foreach( var yy in y )
//    //        {
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue?.Name );
//    //        }
//    //    }
//    //}

//    //{
//    //    List<HutongGames.PlayMaker.Actions.FloatSubtract> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.FloatSubtract>("Raycast","attack_range_detect");

//    //    if( y != null )
//    //    {
//    //        foreach( var yy in y )
//    //        {
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract?.Name );
//    //        }
//    //    }
//    //}

//    //{
//    //    List<HutongGames.PlayMaker.Actions.SetVector2XY> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetVector2XY>("Raycast","attack_range_detect");

//    //    if( y != null )
//    //    {
//    //        foreach( var yy in y )
//    //        {
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable.Name );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable?.Value );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value?.Name );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x?.Name );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y );
//    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y?.Name );
//    //        }
//    //    }
//    //}

//    //{
//    //    HutongGames.PlayMaker.Actions.RayCast2d x = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.RayCast2d>("Raycast","attack_range_detect");

//    //    if( x != null )
//    //    {
//    //        //checkFSM = fsm;
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Name );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Value.transform.position );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromPosition );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.direction );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.space );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.distance );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.minDepth );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.maxDepth );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.hitEvent );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeDidHit );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitObject );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitPoint );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitNormal );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitDistance );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitFraction );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.repeatInterval );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.layerMask );
//    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.invertMask );
//    //    }
//    //}










//    //    //modifiedPrefab.PrintSceneHierarchyTree( true );
//    //    //UnityEngine.SceneManagement.SceneManager.GetSceneByName( currentlyLoadingScene ).PrintHierarchy();
//}