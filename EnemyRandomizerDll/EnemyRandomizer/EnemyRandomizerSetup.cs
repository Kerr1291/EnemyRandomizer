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

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        nv.Contractor databaseLoader = new nv.Contractor();
        
        int currentDatabaseIndex = 0;

        IEnumerator randomizerSceneProcessor = null;

        bool databaseGenerated = false;
        bool isLoadingDatabase = false;
        int loadCount = 0;

        float databaseLoadProgress = 0f;
        float DatabaseLoadProgress {
            get {
                return databaseLoadProgress;
            }
            set {
                if( loadingBar != null )
                {
                    if( value < 1f )
                        loadingBar.gameObject.SetActive( true );
                    else
                        loadingBar.gameObject.SetActive( false );

                    loadingBar.normalizedValue = value;
                }
                databaseLoadProgress = value;
            }
        }

        void RestoreSetup()
        {
            currentDatabaseIndex = 0;
            databaseGenerated = false;
            isLoadingDatabase = false;

            databaseLoadProgress = 0f;
            loadCount = 0;

            //enemyTypes = new Dictionary<string, List<string>>();
            //loadedEnemyPrefabs = new List<GameObject>();
            //loadedEnemyPrefabNames = new List<string>();
            //uniqueEnemyTypes = new List<string>();
            databaseLoader = new nv.Contractor();
        }

        /// <summary>
        /// Initial workhorse funciton. Load all the enemy types in the game.
        /// </summary>
        void LoadSceneData()
        {
            GameObject root = ModRoot;

            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                //Log( "Loading from: " + UnityEngine.SceneManagement.SceneManager.GetSceneAt( i ).name );

                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach( GameObject go in rootGameObjects )
                {
                    //ignore the mod root
                    if( go.name == ModRoot.name )
                        continue;

                    foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
                    {
                        string name = t.gameObject.name;

                        if( SkipLoadingGameObject( name ) )
                            continue;                        

                        name = TrimEnemyNameToBeLoaded( name );

                        //bool isPossibleRandomizerEnemy = ListContainsString(name,EnemyRandoData.enemyTypeNames);
                        //if( isPossibleRandomizerEnemy )
                        //{
                        //    Log( "Possible to be a randomizer enemy type? Name= " + name );
                        //}

                        bool isInLoadedList = GetMatchingIndex(name,loadedEnemyPrefabNames) >= 0;
                        if( isInLoadedList )
                        {
                            //Log( "Already loaded = " + name );
                            continue;
                        }

                        //bool isRandomizerEnemyType = IsInList(name,EnemyRandoData.enemyTypeNames);
                        //if( isRandomizerEnemyType )
                        //{
                        //    Log( "Might be a randomizer enemy type? Name= "+name );
                        //}

                        int indexOfRandomizerEnemyType = GetMatchingIndex(name,EnemyRandoData.enemyTypeNames);

                        //if( indexOfRandomizerEnemyType >= 0 )
                        //{
                        //    Log( "More likely to be a randomizer enemy type! Suspected type index: "+indexOfRandomizerEnemyType+" = "+ EnemyRandoData.enemyTypeNames[ indexOfRandomizerEnemyType ] );
                        //    Log( "IsEnemyByFSM(t.gameObject) ? " + IsEnemyByFSM( t.gameObject ) );
                        //}

                        if( indexOfRandomizerEnemyType >= 0 && IsObjectAnEnemy(t.gameObject) )
                        {
                            GameObject prefab = GameObject.Instantiate(t.gameObject);
                            prefab.SetActive( false );
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //TODO: special logic for certain enemies:

                            //remove the "Cam Lock" game object child from the crystal guardian (mega zombie beam miner)

                            //check out the "Climber Control" playmaker actions-- i think they're throwing nullref for the crystallized laser bug -- look into fixing that

                            loadedEnemyPrefabs.Add( prefab );
                            loadedEnemyPrefabNames.Add( EnemyRandoData.enemyTypeNames[indexOfRandomizerEnemyType] );
                            Log( "Adding enemy type: " + prefab.name + " to list with search string " + EnemyRandoData.enemyTypeNames[ indexOfRandomizerEnemyType ] );                            
                        }//end if-enemy
                    }//end foreach transform in the root game objects
                }//end for each root game object
            }//iterate over all LOADED scenes
        }//end LoadSceneData()

        void BuildEnemyRandomizerDatabase()
        {
            Log( "starting" );
            randomizerSceneProcessor = DoBuildDatabase();

            isLoadingDatabase = true;
            databaseLoader.OnUpdate = BuildDatabase;
            databaseLoader.Looping = true;
            databaseLoader.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
            databaseLoader.Start();
        }

        public int GetSceneToLoadFromRandomizerData(int databaseIndex)
        {
            //return databaseIndex;

            //NORMAL way
            return EnemyRandoData.enemyTypeScenes[ databaseIndex ];
        }

        protected virtual void AdditivelyLoadCurrentScene()
        {
            try
            {
                Log( "Additively loading scene " + GetSceneToLoadFromRandomizerData(currentDatabaseIndex));
                UnityEngine.SceneManagement.SceneManager.LoadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ), LoadSceneMode.Additive );
                loadCount++;
            }
            catch( Exception e )
            {
                Log( "AdditivelyLoadCurrentScene:: Exception from scene: " + e.Message );
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
                Log( "Loading scene data: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                LoadSceneData();

                DatabaseLoadProgress = currentDatabaseIndex / (float)(EnemyRandoData.enemyTypeScenes.Count - 1);
                Log( "Loading Progress: " + DatabaseLoadProgress );

                Log( "Unloading scene: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                UnityEngine.SceneManagement.SceneManager.UnloadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
            }
            catch( Exception e )
            {
                Log( "ProcessCurrentSceneForDataLoad:: Exception from scene: " + e.Message );
                PrintDebugLoadingError();
            }
        }

        protected virtual void CompleteEnemyRandomizerDataLoad()
        {
            Log( "Loaded data from " + loadCount + " scenes." );
            PrintDebugLoadingError();
            databaseGenerated = true;
            isLoadingDatabase = false;
            databaseLoader.Reset();
            randomizerSceneProcessor = null;

            GameManager.instance.LoadFirstScene();
        }

        protected virtual void PrintDebugLoadingError()
        {
            bool printInitial = true;
            foreach( string enemy in EnemyRandoData.enemyTypeNames )
            {
                if( loadedEnemyPrefabNames.Contains( enemy ) )
                    continue;

                if( printInitial )
                {
                    Log( "Enemies not loaded so far:" );
                    printInitial = false;
                }

                Log( "Missing type: " + enemy );
            }
        }

        protected virtual bool IsDoneLoadingRandomizerData()
        {
            //TODO: FOR TESTING - CHANGE BACK
            //return ( currentDatabaseIndex + 1 ) >= 11;
            return ( currentDatabaseIndex + 1) >= EnemyRandoData.enemyTypeScenes.Count;
        }

        protected virtual void BuildDatabase()
        {
            if( randomizerSceneProcessor != null && !randomizerSceneProcessor.MoveNext() )
            {
                Log( "end of iterator" );
                randomizerSceneProcessor.Reset();
            }

            if( randomizerSceneProcessor != null && ( randomizerSceneProcessor.Current as bool? ) == false )
            {
                Log( "iterator returned false" );
                randomizerSceneProcessor.Reset();
            }
        }


        protected IEnumerator DoBuildDatabase()
        {
            while( !databaseGenerated )
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
    }
}
