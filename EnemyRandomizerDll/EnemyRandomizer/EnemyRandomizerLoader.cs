using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

using nv;

namespace EnemyRandomizerMod
{
    //parent this to the mage knight
    public class WakeUpMageKnight : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject mageKnight;

        private void OnTriggerEnter2D( Collider2D collision )
        {
            if( !collider.bounds.Contains( HeroController.instance.transform.position ) )
                return;

            PlayMakerFSM fsm = null;

            foreach( Component c in mageKnight.GetComponents<Component>() )
            {
                if( c as PlayMakerFSM != null )
                {
                    if( ( c as PlayMakerFSM ).FsmName == "Mage Knight" )
                    {
                        fsm = ( c as PlayMakerFSM );
                        break;
                    }
                }
            }

            if( fsm != null )
            {
                fsm.SendEvent( "FINISHED" );
                fsm.SendEvent( "WAKE" );
            }

            //remove this after waking up the enemy
            Destroy( gameObject );
        }
    }


    public class EnemyRandomizerLoader
    {
        public static EnemyRandomizerLoader Instance { get; private set; }

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        nv.Contractor databaseLoader;
        IEnumerator randomizerSceneProcessor = null;

        public bool DatabaseGenerated { get; private set; }

        int currentDatabaseIndex = 0;
        int loadCount = 0;

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

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += PrintNextSceneToLoad;

            comms = new CommunicationNode();
            comms.EnableNode( this );
        }

        public void Unload()
        {
            comms.DisableNode();

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;

            Instance = null;
        }


        void PrintNextSceneToLoad( Scene from, Scene to )
        {
            //For debugging
            Dev.Log( "Loading Scene [" + to.name + "]" );
        }

        /// <summary>
        /// Initial workhorse funciton. Load all the enemy types in the game.
        /// </summary>
        void LoadSceneData()
        {
            GameObject root = EnemyRandomizer.Instance.ModRoot;

            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach( GameObject go in rootGameObjects )
                {
                    //ignore the mod root
                    if( go.name == root.name )
                        continue;

                    foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
                    {
                        string name = t.gameObject.name;

                        if( name.IsSkipLoadingString() )
                            continue;

                        name = name.TrimGameObjectName();
                        
                        bool isInLoadedList = database.loadedEnemyPrefabNames.Contains(name);
                        if( isInLoadedList )
                            continue;
                                                
                        int indexOfRandomizerEnemyType = EnemyRandomizerDatabase.enemyTypeNames.IndexOf(name);
                        
                        if( indexOfRandomizerEnemyType >= 0 && t.gameObject.IsGameEnemy() )
                        {
                            GameObject prefab = GameObject.Instantiate(t.gameObject);
                            prefab.SetActive( false );
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //special logic for certain enemies:
                            prefab = ModifyGameObjectPrefab( prefab );

                            database.loadedEnemyPrefabs.Add( prefab );
                            database.loadedEnemyPrefabNames.Add( EnemyRandomizerDatabase.enemyTypeNames[indexOfRandomizerEnemyType] );
                            Dev.Log( "Adding enemy type: " + prefab.name + " to list with search string " + EnemyRandomizerDatabase.enemyTypeNames[ indexOfRandomizerEnemyType ] );                            
                        }//end if-enemy
                    }//end foreach transform in the root game objects
                }//end for each root game object
            }//iterate over all LOADED scenes
        }//end LoadSceneData()

        GameObject ModifyGameObjectPrefab( GameObject randoPrefab )
        {
            GameObject modifiedPrefab = randoPrefab;

            //TODO?: check out the "Climber Control" playmaker actions-- i think they're throwing nullref for the crystallized laser bug -- look into fixing that

            //Create a custom "wake up" base game object and put it on the mage knight
            if( randoPrefab.name.Contains( "Mage Knight" ) )
            {
                GameObject wakeUpRoot = new GameObject("MK Wake Up Object");
                wakeUpRoot.transform.SetParent( randoPrefab.transform );
                wakeUpRoot.transform.localPosition = Vector3.zero;

                BoxCollider2D box = wakeUpRoot.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2( 15f, 15f );

                WakeUpMageKnight specialWakeUp = wakeUpRoot.AddComponent<WakeUpMageKnight>();
                specialWakeUp.collider = box;
                specialWakeUp.mageKnight = randoPrefab;
            }

            //TODO: NEEDS TESTING see if this fixes the camera problem with him
            //remove the "Cam Lock" game object child from the crystal guardian (mega zombie beam miner)
            if( randoPrefab.name.Contains( "Mega Zombie Beam Miner" ) )
            {
                GameObject camLock = randoPrefab.FindGameObjectInChildren("Cam Lock");
                if( camLock != null )
                    GameObject.Destroy( camLock );
            }

            return modifiedPrefab;
        }

        public void BuildEnemyRandomizerDatabase()
        {
            Dev.Where();
            randomizerSceneProcessor = DoBuildDatabase();

            databaseLoader = new nv.Contractor
            {
                OnUpdate = BuildDatabase,
                Looping = true
            };
            databaseLoader.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
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
                //Dev.Log( "Additively loading scene " + GetSceneToLoadFromRandomizerData(currentDatabaseIndex));
                UnityEngine.SceneManagement.SceneManager.LoadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ), LoadSceneMode.Additive );
                loadCount++;
            }
            catch( Exception e )
            {
                Dev.Log( "Exception from scene: " + e.Message );
                PrintDebugLoadingError();
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
                Dev.Log( "Exception from scene: " + e.Message );
                PrintDebugLoadingError();
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

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;

            DatabaseGenerated = true;
            databaseLoader.Reset();
            randomizerSceneProcessor = null;

            GameManager.instance.LoadFirstScene();
        }

        protected virtual bool IsDoneLoadingRandomizerData()
        {
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
                    bool status = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isLoaded;
                    if(!status)
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

        protected virtual void PrintDebugLoadingError()
        {
            bool printInitial = true;
            foreach( string enemy in EnemyRandomizerDatabase.enemyTypeNames )
            {
                if( database.loadedEnemyPrefabNames.Contains( enemy ) )
                    continue;

                if( printInitial )
                {
                    Dev.Log( "Enemies not loaded so far:" );
                    printInitial = false;
                }

                Dev.Log( "Missing type: " + enemy );
            }
        }
    }
}
