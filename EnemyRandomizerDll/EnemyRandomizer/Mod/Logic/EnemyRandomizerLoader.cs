using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using nv;
using EnemyRandomizerMod.Extensions;

namespace EnemyRandomizerMod
{   
    public class EnemyRandomizerLoader
    {
        public static EnemyRandomizerLoader Instance { get; private set; }

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        SmartRoutine databaseLoader;

        //IEnumerator randomizerSceneProcessor = null;

        public bool DatabaseGenerated { get; private set; }

        int currentDatabaseIndex = 0;

        int loadCount = 0;

        string currentlyLoadingScene = "";

        Scene lastLoadedScene;

        //bool IsLoadingDatabase {
        //    get {
        //        return randomizerSceneProcessor != null;
        //    }
        //}            

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
                if (i % 3 == 0)
                {
                    IEnumerator UnloadAssets()
                    {
                        yield return Resources.UnloadUnusedAssets();
                    }
                    
                    // Avoid over-allocationg memory by forcing a GC
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.StartCoroutine(UnloadAssets());
                    }

                    GC.Collect();
                }
                
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
                HornetBoss hornet = modifiedPrefab.AddComponent<HornetBoss>();
            }
            else if( name == "Moss Charger" )
            {
                Dev.Log( "TRYING " + ( i++ ).ToString() );
                {
                    DebugOnWake d = DebugOnWake.AddDebugOnWake( modifiedPrefab, "Mossy Control", "Hidden", new List<string>() { "IN RANGE" }, true, customWakeAreaSize, false );
                }
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
            databaseLoader = new SmartRoutine(DoBuildDatabase());
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
                Dev.Log ("Loading scene data: " + GetSceneToLoadFromRandomizerData (currentDatabaseIndex));
                LoadSceneData ();

                DatabaseLoadProgress = currentDatabaseIndex / (float)(EnemyRandomizerDatabase.EnemyTypeScenes.Count - 1);
                Dev.Log ("Loading Progress: " + DatabaseLoadProgress);

                Dev.Log ("Unloading scene: " + GetSceneToLoadFromRandomizerData (currentDatabaseIndex));
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (GetSceneToLoadFromRandomizerData (currentDatabaseIndex));
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
            databaseLoader.Stop();
            databaseLoader.Reset();
            //randomizerSceneProcessor = null;

            GameManager.instance.LoadFirstScene();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)               // Clear out extra loaded scenes to prevent inital load issue
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (scene != UnityEngine.SceneManagement.SceneManager.GetActiveScene())
                {
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        protected virtual bool IsDoneLoadingRandomizerData()
        {
            //return ( currentDatabaseIndex + 1 ) >= 421;
            return ( currentDatabaseIndex + 1) >= EnemyRandomizerDatabase.EnemyTypeScenes.Count;
        }

        //protected virtual void BuildDatabase()
        //{
        //    if( randomizerSceneProcessor != null && !randomizerSceneProcessor.MoveNext() )
        //    {
        //        randomizerSceneProcessor.Reset();
        //    }

        //    if( randomizerSceneProcessor != null && ( randomizerSceneProcessor.Current as bool? ) == false )
        //    {
        //        randomizerSceneProcessor.Reset();
        //    }
        //}

        protected IEnumerator DoBuildDatabase(params object[] args)
        {
            for (; ; )
            {
                while (!DatabaseGenerated)
                {
                    AdditivelyLoadCurrentScene();

                    //wait until all scenes are loaded
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount;)
                    {
                        bool status = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isLoaded && UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).IsValid();
                        if (!status)
                        {
                            i = 0;
                            yield return null;
                        }
                        else
                        {
                            ++i;
                        }
                    }

                    ProcessCurrentSceneForDataLoad();

                    yield return null;

                    if (IsDoneLoadingRandomizerData())
                    {
                        CompleteEnemyRandomizerDataLoad();
                    }
                    else
                    {
                        IncrementCurrentSceneIndex();
                    }

                    yield return null;
                }

                yield return null;
            }
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