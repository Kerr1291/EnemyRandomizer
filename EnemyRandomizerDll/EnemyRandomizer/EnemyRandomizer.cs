using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;


/*
 * Top TODOs:
 * 
 * 0. figure out which enemies aren't being loaded and see where they're supposed to come from
 * 1. Spread out enemy scene replacements over time instead of all at once
 * 2. Prevent infinite checking iterations from happening
 * 3. debug print out what enemies from the type list aren't being loaded (and what scenes are loading nothing)
 * 4. add tutorial_01 to the randomized scenes
 * 5. have the randomizer trigger as loaded when starting a new save
 * 
 * try something with this to kill enemies
            //FSMUtility.LocateFSM( enemy, "health_manager_enemy" ).SetState( "Decrement Health" );
 * 
 * */

namespace EnemyRandomizerMod
{
    public class EnemyRandomizer : Mod<EnemyRandomizerSettings>, ITogglableMod
    {
        void EnableEnemyRandomizer(int id)
        {
            if(databaseGenerated)
                randomizerReady = true;
        }

        public override void Initialize()
        {
            if(Instance != null)
            {
                Log("Warning: EnemyRandomizer is a singleton. Trying to create more than one may cause issues!");
                return;
            }

            Instance = this;

            Log("Enemy Randomizer Mod initializing!");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ToggleBuildRandoDatabaseUI;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartRandomEnemyLocator;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.ColliderCreateHook += OnLoadObjectCollider;

            ModHooks.Instance.SavegameLoadHook -= EnableEnemyRandomizer;
            ModHooks.Instance.SavegameLoadHook += EnableEnemyRandomizer;

            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;
            ModHooks.Instance.SlashHitHook += Debug_PrintObjectOnHit;

            //ModHooks.Instance.DashPressedHook += StartLoad; //used for testing
        }

        public void Unload()
        {
            Instance = null;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.SavegameLoadHook -= EnableEnemyRandomizer;
            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;

            GameObject root = GameObject.Find("RandoRoot");

            if( root != null )
            {
                GameObject.Destroy( root );
            }

            if( menu != null )
            {
                GameObject.Destroy( menu );
            }

            Restore();
        }

        void Restore()
        {
            nextDatabaseIndex = 0;
            currentDatabaseIndex = 0;
            menu = null;
            databaseGenerated = false;
            isLoadingDatabase = false;
            loadCount = 0;
            enemyTypes = new Dictionary<string, List<string>>();
            loadedEnemyPrefabs = new List<GameObject>();
            loadedEnemyPrefabNames = new List<string>();
            uniqueEnemyTypes = new List<string>();
            databaseLoader = new nv.Contractor();
            loadingBar = null;
            randomizerReady = false;
            randomEnemyLocator = new nv.Contractor();
            databaseLoadProgress = 0f;
            recentHit = "";
        }

        public override string GetVersion()
        {
            return "0.0.1";// (XML Version: " + xmlVer + ")";
            //return "1.5.1 (XML Version: " + xmlVer + ")";
        }

        public override bool IsCurrent()
        {
            return true;
            //TODO: update after checking in correct version
            //try
            //{
            //    GithubVersionHelper helper = new GithubVersionHelper("Kerr1291/EnemyRandomizerMod");
            //    Log("Github = " + helper.GetVersion());
            //    return GetVersion().StartsWith(helper.GetVersion());
            //}
            //catch(Exception)
            //{
            //    return true;
            //}
        }

        void Debug_PrintObjectOnHit(Collider2D otherCollider, GameObject gameObject)
        {
            if(otherCollider.gameObject.name != recentHit)
            {
                Log( "("+otherCollider.gameObject.transform.position+") HIT: " + otherCollider.gameObject.name);
                recentHit = otherCollider.gameObject.name;
            }
        }

        public static EnemyRandomizer Instance { get; private set; }

        //TODO: update to not be hardcoded like this :P
        string bundlePath = "K:/Games/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/mainui";
        string recentHit = "";
        int nextDatabaseIndex = 0;
        int currentDatabaseIndex = 0;
        GameObject menu = null;
        bool databaseGenerated = false;
        bool isLoadingDatabase = false;
        int loadCount = 0;
        Dictionary<string, List<string>> enemyTypes = new Dictionary<string, List<string>>();
        List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        List<string> loadedEnemyPrefabNames = new List<string>();
        List<string> uniqueEnemyTypes = new List<string>();
        nv.Contractor databaseLoader = new nv.Contractor();

        UnityEngine.UI.Slider loadingBar = null;

        bool randomizerReady = false;

        nv.Contractor randomEnemyLocator = new nv.Contractor();

        float databaseLoadProgress = 0f;
        float DatabaseLoadProgress
        {
            get
            {
                return databaseLoadProgress;
            }
            set
            {
                if(loadingBar != null)
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

        void OnLoadObjectCollider(GameObject potentialEnemy)
        {
            if(!randomizerReady)
                return;

            bool isRandoEnemy = IsRandomizerEnemy(potentialEnemy);
            if(isRandoEnemy)
                RandomizeEnemy(potentialEnemy);
        }

        void LocateAndRandomizeEnemies()
        {
            Log("Searching the scene for enemies and randomizing them!");

            //iterate over the loaded scenes
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                //iterate over the loaded game objects
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach(GameObject rootGameObject in rootGameObjects)
                {
                    //and their children
                    foreach(Transform t in rootGameObject.GetComponentsInChildren<Transform>(true))
                    {
                        //skip child components of randomized enemies
                        foreach(Transform p in t.GetComponentsInParent<Transform>(true))
                        {
                            if(p.name.Contains("Rando"))
                                continue;
                        }

                        //abort randomizing on summon scenes since it can cause a softlock
                        if( t.name.Contains( "Summon" ) )
                            return;

                        GameObject potentialEnemy = t.gameObject;

                        bool isRandoEnemy = IsRandoEnemyType(potentialEnemy);
                        if(isRandoEnemy)
                            RandomizeEnemy(potentialEnemy);
                    }
                }
            }

            randomEnemyLocator.Reset();
        }

        bool IsRandomizerEnemy(GameObject enemy)
        {
            return IsEnemyByFSM(enemy) && IsRandoEnemyType(enemy);
        }

        bool IsEnemyByFSM(GameObject enemy)
        {
            return ( FSMUtility.ContainsFSM( enemy, "health_manager_enemy" ) ) || ( FSMUtility.ContainsFSM( enemy, "health_manager" ) );
        }

        bool IsRandoEnemyType(GameObject enemy)
        {
            //Log("Found an enemy? Seeing if it is an enemy and if we should randomize it. ");
            //Log("Object Name: " + enemy.name);

            if( enemy.name.Contains( "Corpse" ) )
                return false;

            if( enemy.name.Contains( "Lil Jellyfish" ) )
                return false;

            if( enemy.name.Contains( "Summon" ) )
                return false;

            var words = loadedEnemyPrefabNames.Select(w => @"\b" + Regex.Escape(w) + @"\b").ToArray();
            var pattern = new Regex("(" + string.Join(")|(", words) + ")");
            bool isRandoEnemyType = pattern.IsMatch(enemy.name);

            return isRandoEnemyType;
        }

        bool IsInList(GameObject enemy, List<string> typeList)
        {
            var words = typeList.Select(w => @"\b" + Regex.Escape(w) + @"\b").ToArray();
            var pattern = new Regex("(" + string.Join(")|(", words) + ")");
            bool isInList = pattern.IsMatch(enemy.name);
            return isInList;
        }

        void RandomizeEnemy(GameObject enemy)
        {
            Log( "Randomizing: " + enemy.name );

            //this failsafe is needed here because the other checks for "Summon" spawners seem to fail....
            if( enemy.name.Contains( "Summon" ) )
            {
                return;
            }


            GameObject replacement = GetRandomEnemyReplacement(enemy);
            ReplaceEnemy(enemy, replacement);
        }

        public class FLAGS
        {
            static public int GROUND = 1;
            static public int FLYING = 2;
            static public int SMALL = 4;
            static public int MED = 8;
            static public int BIG = 16;
            static public int WALL = 32;
            static public int HARD = 64;
        }

        int GetTypeFlags(GameObject enemy)
        {
            bool isGround = IsInList(enemy, EnemyRandoData.groundEnemyTypeNames);
            bool isFlying = IsInList(enemy, EnemyRandoData.flyerEnemyTypeNames);
            bool isSmall = IsInList(enemy, EnemyRandoData.smallEnemyTypeNames);
            bool isMed = IsInList(enemy, EnemyRandoData.mediumEnemyTypeNames);
            bool isLarge = IsInList(enemy, EnemyRandoData.bigEnemyTypeNames);
            bool isWall = IsInList(enemy, EnemyRandoData.wallEnemyTypeNames);
            bool isHard = IsInList(enemy, EnemyRandoData.hardEnemyTypeNames);

            int flags = 0;
            flags |= (isGround ? 1 : 0) << 0;
            flags |= (isFlying ? 1 : 0) << 1;
            flags |= (isSmall ? 1 : 0) << 2;
            flags |= (isMed ? 1 : 0) << 3;
            flags |= (isLarge ? 1 : 0) << 4;
            flags |= (isWall ? 1 : 0) << 5;
            flags |= (isHard ? 1 : 0) << 6;

            return flags;
        }

        bool HasSameType(int flagsA, int flagsB)
        {
            if((flagsA & FLAGS.GROUND) > 0 && (flagsB & FLAGS.GROUND) > 0)
            {
                return true;
            }
            if((flagsA & FLAGS.FLYING) > 0 && (flagsB & FLAGS.FLYING) > 0)
            {
                return true;
            }
            if((flagsA & FLAGS.WALL) > 0 && (flagsB & FLAGS.WALL) > 0)
            {
                return true;
            }
            return false;
        }

        bool HasSameSize(int flagsA, int flagsB)
        {
            if((flagsA & FLAGS.SMALL) > 0 && (flagsB & FLAGS.SMALL) > 0)
            {
                return true;
            }
            if((flagsA & FLAGS.MED) > 0 && (flagsB & FLAGS.MED) > 0)
            {
                return true;
            }
            if((flagsA & FLAGS.BIG) > 0 && (flagsB & FLAGS.BIG) > 0)
            {
                return true;
            }
            return false;
        }

        GameObject GetRandomEnemyReplacement(GameObject enemy)
        {
            int enemyFlags = GetTypeFlags(enemy);

            //search for a compatible replacement
            int randomReplacement = -1;
            while(randomReplacement < 0)
            {
                int temp = UnityEngine.Random.Range(0, loadedEnemyPrefabs.Count);

                GameObject tempPrefab = loadedEnemyPrefabs[temp];

                int tempFlags = GetTypeFlags(tempPrefab);
                bool isValid = false;

                if(HasSameType(enemyFlags, tempFlags))
                {
                    if(HasSameSize(enemyFlags, tempFlags))
                        isValid = true;
                }

                if((enemyFlags & FLAGS.WALL) > 0 && (tempFlags & FLAGS.WALL) > 0)
                {
                    isValid = true;
                }

                if(isValid)
                    randomReplacement = temp;
            }            

            GameObject prefab = loadedEnemyPrefabs[randomReplacement];
            Log("Spawning rando monster: " + prefab.name + " from index " + randomReplacement + " out of " + loadedEnemyPrefabs.Count + " to replace " + enemy.name);
            return prefab;
        }

        void ReplaceEnemy(GameObject oldEnemy, GameObject replacementPrefab)
        {
            //where we'll place the new enemy in the scene
            oldEnemy.SetActive(false);

            GameObject newEnemy = InstantiateEnemy(replacementPrefab);

            ScaleRandomizedEnemy(newEnemy);
            RotateRandomizedEnemy(newEnemy, oldEnemy);
            PositionRandomizedEnemy(newEnemy, oldEnemy);
            NameRandomizedEnemy( newEnemy );

            newEnemy.SetActive(true);

            //NEW: TESTING THIS FUNCTIONALITY
            GameObject.Destroy(oldEnemy);
        }

        GameObject InstantiateEnemy(GameObject prefab)
        {
            //where we'll place the new enemy in the scene
            GameObject newEnemyRoot = GameObject.Find("_Enemies");

            GameObject newEnemy = UnityEngine.Object.Instantiate(prefab) as GameObject;

            newEnemy.transform.SetParent(newEnemyRoot.transform);

            //TODO: generate an interesting or unique name?
            newEnemy.tag = prefab.tag;

            return newEnemy;
        }

        void NameRandomizedEnemy(GameObject newEnemy)
        {
            newEnemy.name = "Rando Enemy"; //gameObject.name; //if we put the game object's name here it'll re-randomize itself (whoops)
        }

        void ScaleRandomizedEnemy(GameObject newEnemy)
        {
            //TODO as a fun factor option, try scaling the new enemy?
        }

        void PositionRandomizedEnemy(GameObject newEnemy, GameObject oldEnemy)
        {
            //TODO adjust the position to take into account the new monster type and/or size
            newEnemy.transform.position = oldEnemy.transform.position;

            Vector3 positionOffset = Vector3.zero;

            int flags = GetTypeFlags(newEnemy);
            if((flags & FLAGS.GROUND) > 0)
            {
                Vector3 toGround = GetVectorToGround(newEnemy);
                Vector3 onGround = GetPointOnGround(newEnemy);

                newEnemy.transform.position = onGround;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                positionOffset = new Vector3( 0f, collider.size.y, 0f );
            }
            
            newEnemy.transform.position = newEnemy.transform.position + new Vector3( positionOffset.x, positionOffset.y, positionOffset.z );

            //if(newEnemy.name.Contains("Lesser Mawlek"))
            //{

            //}
        }
        //    public class mything
        //    {
        //        public StateFlags flags = 0;

        //        public void SetFlags()
        //        {
        //            flags |= ( isGround ? StateFlags.Ground : 0 );
        //        }

        //        public void CheckFlags()
        //        {
        //            if( flags.HasFlag( StateFlags.Ground & StateFlags.Small ) )
        //        //burp
        //}
        //    }
        //TODO: finish tomorrow, cast a ray to the ground and use it in position randomized enemy to place them on the ground
        Vector3 GetPointOnGround( GameObject entitiy )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,5f, Physics2D.AllLayers);

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Log( "GetPointOnGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        return v.point;
                    }
                }
            }
            else
            {
                Log( "GetPointOnGround:: RaycastHit2D is null! " );
            }

            return Vector3.zero;
        }

        Vector3 GetVectorToGround( GameObject entitiy )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,5f, Physics2D.AllLayers);

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Log( "GetVectorToGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Log( "GetVectorToGround:: RaycastHit2D is null! " );
            }

            return Vector3.zero;
        }

        void RotateRandomizedEnemy(GameObject newEnemy, GameObject oldEnemy)
        {
            //TODO adjust the rotation to take into account the new monster type and/or size
            newEnemy.transform.rotation = oldEnemy.transform.rotation;
        }

        void StartRandomEnemyLocator(Scene from, Scene to)
        {
            Log( "Transitioning FROM [" + from.name + "] TO [" + to.name + "]" );
            if(!randomizerReady)
                return;

            //ignore randomizing on scenes that aren't in-game scenes
            if(to.buildIndex <= 36 || to.buildIndex > 362)
                return;

            randomEnemyLocator.Reset();

            float randomEnemyLocatorDelayTimer = 1f;

            randomEnemyLocator.OnComplete = LocateAndRandomizeEnemies;
            randomEnemyLocator.Duration = randomEnemyLocatorDelayTimer;

            randomEnemyLocator.Start();
        }






        void ToggleBuildRandoDatabaseUI(Scene from, Scene to)
        {
            if( isLoadingDatabase )
                return;

            bool isTitleScreen = (string.Compare(to.name, "Menu_Title") == 0);
            ShowRandoDatabaseUI(isTitleScreen);
        }

        void ShowRandoDatabaseUI(bool show)
        {
            if(databaseGenerated)
                return;

            if(menu == null)
            {
                LoadRandoDatabaseUI();
            }

            if(menu != null)
            {
                menu.SetActive(show);
            }
        }

        void LoadRandoDatabaseUI()
        {
            Log("Loading mainui bundle from: " + bundlePath);
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            string loadUIRoot = "RandoMainUI(Clone)";
            string loadingBarName = "RandoStartupLoading";
            string enableButtonName = "EnableEnemeyRando";

            if(bundle == null)
            {
                Log("mainui bundle not found!!");
                return;
            }

            string[] names = bundle.GetAllAssetNames();

            foreach(var s in names)
            {
                Log("Loading asset: " + s);
                GameObject bundleObject = bundle.LoadAsset(s) as GameObject;
                GameObject newObject = null;

                if(bundleObject != null)
                {
                    newObject = GameObject.Instantiate(bundleObject);

                    if(newObject.name == loadUIRoot)
                        menu = newObject;
                }
            }

            if(menu == null)
            {
                Log("Failed to load main randomizer ui!");
                return;
            }

            //setup enable randomizer button
            GameObject enableRandoButton = FindGameObjectInChildren(menu, enableButtonName);
            UnityEngine.UI.Button enableButton = enableRandoButton.GetComponent<UnityEngine.UI.Button>();
            enableButton.onClick.AddListener(BuildEnemyRandomizerDatabase);

            //setup the loading bar
            if(loadingBar == null)
            {
                GameObject loadingBarObj = FindGameObjectInChildren(menu, loadingBarName);
                loadingBar = loadingBarObj.GetComponent<UnityEngine.UI.Slider>();
                loadingBar.gameObject.SetActive( false );
            }

            //keep objects we've instantiated around, but unload the rest of the bundle
            bundle.Unload( false );
        }
        


        void BuildEnemyRandomizerDatabase()
        {
            isLoadingDatabase = true;
            databaseLoader.OnUpdate = BuildDatabase;
            databaseLoader.Looping = true;
            databaseLoader.SetUpdateRate(nv.Contractor.UpdateRateType.Frame);
            databaseLoader.Start();
        }


        protected virtual int GetCurrentSceneIndex()
        {
            return -1;
        }

        protected virtual void AdditivelyLoadCurrentScene()
        {
            try
            {
                Log( "Additively loading scene" + EnemyRandoData.enemyTypeScenes[ nextDatabaseIndex ] );
                UnityEngine.SceneManagement.SceneManager.LoadScene( EnemyRandoData.enemyTypeScenes[ nextDatabaseIndex ], LoadSceneMode.Additive );
                loadCount++;
            }
            catch( Exception e )
            {
                Log( "Exception from scene: " + e.Message );
                PrintDebugLoadingError();
            }
        }

        protected virtual void IncrementCurrentSceneIndex()
        {
            currentDatabaseIndex = nextDatabaseIndex;
            nextDatabaseIndex += 1;
        }

        protected virtual void ProcessCurrentSceneForDataLoad()
        {
            Log( "Loading scene data: " + EnemyRandoData.enemyTypeScenes[ currentDatabaseIndex ] );
            LoadSceneData();

            DatabaseLoadProgress = currentDatabaseIndex / (float)EnemyRandoData.enemyTypeScenes.Count;
            Log( "Loading Progress: " + DatabaseLoadProgress );

            Log( "Unloading scene: " + EnemyRandoData.enemyTypeScenes[ currentDatabaseIndex ] );
            UnityEngine.SceneManagement.SceneManager.UnloadScene( EnemyRandoData.enemyTypeScenes[ currentDatabaseIndex ] );
        }

        protected virtual void CompleteEnemyRandomizerDataLoad()
        {
            Log( "Loaded data from " + loadCount + " scenes." );
            databaseGenerated = true;
            isLoadingDatabase = false;
            databaseLoader.Reset();

            GameManager.instance.LoadFirstScene();
        }

        protected virtual void FinalizeEnemeyRandomizerDataLoad()
        {
            if( currentDatabaseIndex >= 0 )
                UnityEngine.SceneManagement.SceneManager.UnloadScene( EnemyRandoData.enemyTypeScenes[ currentDatabaseIndex ] );
            UnityEngine.SceneManagement.SceneManager.UnloadScene( EnemyRandoData.enemyTypeScenes[ nextDatabaseIndex ] );
        }

        protected virtual void PrintDebugLoadingError()
        {
            Log( "Enemies not loaded so far:" );
            foreach( string enemy in EnemyRandoData.enemyTypeNames )
            {
                if( loadedEnemyPrefabNames.Contains( enemy ) )
                    continue;
                Log( "Missing type: " + enemy );
            }
        }

        protected virtual void BuildDatabase()
        {
            try
            {
                if( currentDatabaseIndex >= 0)
                {
                    ProcessCurrentSceneForDataLoad();
                }

                if( nextDatabaseIndex >= EnemyRandoData.enemyTypeScenes.Count )
                {
                    CompleteEnemyRandomizerDataLoad();
                }
                else
                {
                    AdditivelyLoadCurrentScene();
                    IncrementCurrentSceneIndex();
                }

                if(databaseGenerated)
                {
                    FinalizeEnemeyRandomizerDataLoad();
                }
            }
            catch(Exception e)
            {
                Log("Exception from scene: " + e.Message);
                PrintDebugLoadingError();
            }
        }


        /// <summary>
        /// Initial workhorse funciton. Load all the enemy types in the game.
        /// </summary>
        void LoadSceneData()
        {
            GameObject root = GameObject.Find("RandoRoot");

            if(root == null)
            {
                root = new GameObject("RandoRoot");
                GameObject.DontDestroyOnLoad(root);
            }

            //iterate over the loaded scenes
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach(GameObject go in rootGameObjects)
                {
                    foreach(Transform t in go.GetComponentsInChildren<Transform>(true))
                    {

                        if( t.gameObject.name.Contains( "Mender" ) )
                        {
                            Log( "Found mender bug!" );
                            DebugPrintObjectTree( t.gameObject );
                        }

                        if(FSMUtility.ContainsFSM(t.gameObject, "health_manager_enemy") || ( FSMUtility.ContainsFSM( t.gameObject, "health_manager" ) ) )
                        {
                            foreach(var name in EnemyRandoData.enemyTypeNames)
                            {
                                if(t.gameObject.name.Contains(name))
                                {
                                    if(loadedEnemyPrefabNames.Contains(name))
                                        break;

                                    if(uniqueEnemyTypes.Contains(t.gameObject.name))
                                        break;

                                    if(name == "Fly" && t.gameObject.name.Contains("Fly"))
                                    {
                                        if((t.gameObject.name.Contains("Flying") || t.gameObject.name.Contains("Flyer")))
                                        {
                                            continue;
                                        }
                                    }

                                    //don't grab spawners....
                                    if( t.gameObject.name.Contains( "Spawner" ) )
                                        continue;

                                    if( t.gameObject.name.Contains( "Summon" ) )
                                        continue;

                                    GameObject prefab = GameObject.Instantiate(t.gameObject);
                                    prefab.SetActive(false);
                                    GameObject.DontDestroyOnLoad(prefab);
                                    prefab.transform.SetParent(root.transform);

                                    loadedEnemyPrefabs.Add(prefab);
                                    loadedEnemyPrefabNames.Add(name);
                                    Log("Adding enemy type: " + prefab.name + " to list with search string " + name);

                                    uniqueEnemyTypes.Add(t.gameObject.name);
                                }
                            }//end foreach enemyTypeNames
                        }//end if-enemy
                    }//end foreach transform in the root game objects
                }//end for each root game object
            }//iterate over all LOADED scenes
        }//end LoadSceneData()        

        public static void DebugPrintAllObjects(string sceneName)
        {
            Instance.Log("START =====================================================");
            Instance.Log("Printing scene hierarchy for scene: " + sceneName);
            GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
            foreach(GameObject go in rootGameObjects)
            {
                foreach(Transform t in go.GetComponentsInChildren<Transform>(true))
                {
                    string objectNameAndPath = GetObjectPath(t);
                    Instance.Log(objectNameAndPath);
                }
            }
            Instance.Log("END +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }

        public static void DebugPrintObjectTree( GameObject root )
        {
            if( root == null )
                return;

            Instance.Log( "DebugPrintObjectTree START =====================================================" );
            Instance.Log( "Printing scene hierarchy for scene: " + root.name );
            foreach( Transform t in root.GetComponentsInChildren<Transform>( true ) )
            {
                string objectNameAndPath = GetObjectPath(t);
                Instance.Log( objectNameAndPath );
            }
            Instance.Log( "DebugPrintObjectTree END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
        }


        public static string GetObjectPath(Transform t)
        {
            string objStr = t.name;

            if(t.parent != null)
                objStr = GetObjectPath(t.parent) + "\\" + t.name;

            return objStr;
        }

        public static GameObject FindGameObjectInChildren(GameObject obj, string name)
        {
            foreach(var t in obj.GetComponentsInChildren<Transform>(true))
            {
                if(t.name == name )
                    return t.gameObject;
            }
            return null;
        }
    }
}
