using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Events;

#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

//EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Fly",null);

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizerDatabase
    {
        public static string BlockHitEffectName = "Block Hit v2";
        static string DESTROY_ON_LOAD = "DestroyOnLoad";
        static string RESOURCES = "RESOURCES";

        public static Func<int> GetPlayerSeed;
        public static Func<int> GetCustomColoSeed;
        public static Func<EnemyRandomizerDatabase> GetDatabase;
        public static Func<ReactiveProperty<List<GameObject>>> GetBlackBorders;

        static UnityEvent<(GameObject newObject, GameObject oldObject)> onObjectReplaced;
        public static UnityEvent<(GameObject newObject, GameObject oldObject)> OnObjectReplaced
        {
            get
            {
                if (onObjectReplaced == null)
                {
                    onObjectReplaced = new UnityEvent<(GameObject newObject, GameObject oldObject)>();
                }
                return onObjectReplaced;
            }
        }

        /// <summary>
        /// Params: Position, Object Name, (Optional: null or Object Replacement Name), bool: SetActive? -- Returns the created (or replaced) game object using the randomizer's custom methods
        /// </summary>
        public static Func<Vector3, string, string, bool, GameObject> CustomSpawnWithLogic;

        /// <summary>
        /// Create this in the mod's GetPreloadNames() before returning from that method
        /// </summary>
        public static EnemyRandomizerDatabase Create(string filename = null)
        {
//#if !LIBRARY
//            if (filename == null)
//            {
//                return new EnemyRandomizerDatabase()
//                {
//                    version = EnemyRandomizerDB.Instance.GetVersion(),
//                    scenes = new List<SceneData>(),
//                    badSceneData = new List<SceneData>()
//                };
//            }
//            else
//            {
//#endif
                bool result = LoadDatabase(filename, out EnemyRandomizerDatabase db);

                if (!result)
                    return null;

                return db;
//#if !LIBRARY
//            }
//#endif
        }

        /// <summary>
        ///  Use in the mod's >>> public override List<(string, string)> GetPreloadNames()
        /// </summary>
        public List<(string, string)> GetPreloadNames()
        {
            var scenesSubSet = scenes.Where(x => !x.name.Contains(DESTROY_ON_LOAD) && !x.name.Contains(RESOURCES)).ToList();

            string[] scenesToCompile = scenesSubSet.Select(x => x.name).ToArray();
            List<(string, string)> sceneDataPairs = new List<(string, string)>();
            for (int s = 0; s < scenesToCompile.Length; ++s)
            {
                var scene = scenesSubSet[s];
                scene.sceneObjects.ForEach(x =>
                {
                    sceneDataPairs.Add((scene.name, x.path));
                });
            }

            if(DEBUG_VERBOSE_SPAWNER_ERRORS)
                sceneDataPairs.ToList().ForEach(x => Dev.Log($"PAIRS TO LOAD[{x.Item1}] - [{x.Item2}]"));

            return sceneDataPairs.ToList();
        }

        /// <summary>
        ///  Use in the mod's >>> public override (string, Func<IEnumerator>)[] PreloadSceneHooks()
        /// </summary>
        //public (string, Func<IEnumerator>)[] PreloadSceneHooks()
        //{
        //    var scenesSubSet = scenes.Where(x => !x.name.Contains(DESTROY_ON_LOAD) && !x.name.Contains(RESOURCES)).ToList();

        //    string[] scenesToCompile = scenesSubSet.Select(x => x.name).ToArray();

        //    Func<IEnumerator> GetPrefabCompilationMethod(string s)
        //    {
        //        IEnumerator CaptureSceneAndBuildPrefab()
        //        {
        //            yield break;
        //            //yield return BuildPrefabs(s);
        //        }

        //        return CaptureSceneAndBuildPrefab;
        //    }

        //    IEnumerable<Func<IEnumerator>> GetAllMethods(string[] scenes) { for (int i = 0; i < scenes.Length; ++i) yield return GetPrefabCompilationMethod(scenes[i]); };
        //    Func<IEnumerator>[] compilationMethods = GetAllMethods(scenesToCompile).ToArray();

        //    (string, Func<IEnumerator>)[] sceneMethodPairs = new (string, Func<IEnumerator>)[scenesToCompile.Length];
        //    for (int k = 0; k < sceneMethodPairs.Length; ++k)
        //    {
        //        sceneMethodPairs[k] = (scenesToCompile[k], compilationMethods[k]);
        //    }

        //    //for (int i = 0; i < sceneMethodPairs.Length; ++i)
        //    //{
        //    //    Dev.Log($"{sceneMethodPairs[i].Item1} - {sceneMethodPairs[i].Item2}");
        //    //}

        //    return sceneMethodPairs;
        //}

        public void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            preloadedObjects.ToList().ForEach(s =>
            {
                //match the preloader key to the database key
                SceneData scene = scenes.FirstOrDefault(y => y.name == s.Key);
                if (scene == null)
                    return;

                //map the key to the data object
                Scenes.Add(scene.name, scene);

                //iterate over the scene objects
                s.Value.ToList().ForEach(x =>
                {
                    if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                        Dev.Log($"SCENE:{s.Key} PATH:{x.Key} OBJECT:{x}");

                    if (x.Value == null)
                    {
                        if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                            Dev.LogWarning($"{x.Key} was not found in {s.Key} or references a null object!");
                        return;
                    }

                    //string key = ToDatabaseKey(x.Value.name);
                    string key = TrimEnd(x.Value.name, "(Clone)");
                    x.Value.name = key;

                    var sceneObject = scene.sceneObjects.FirstOrDefault(z => z.Name == key);
                    var loadedPrefab = CreatePrefabObject(x.Value.name, x.Value, sceneObject);

                    //var go = x.Value != null ? x.Value.name : "null";
                });
            });
        }

        /// <summary>
        /// Invoke in Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        /// </summary>
        public void Finalize(Action onComplete)
        {
            //Dev.Log("Trying to finalize loading");
            //find something in the menu, literally anything will work
            MonoBehaviour coroutineRoot = GameManager.instance;
            //Dev.Log("Found " + coroutineRoot.name);

            //Dev.Log("Starting routine");
            //finish loading
            coroutineRoot.StartCoroutine(FinalLoader(onComplete));
        }

        IEnumerator FinalLoader(Action onComplete)
        {
            Dev.Log("Finalizing loading");
            foreach(SceneData s in scenes)
            {
                FinalizeAndLoadSceneData(s);
            }

            Dev.Log("Object Database Loading Complete!");
            onComplete?.Invoke();
            yield break;
        }

        protected virtual void FinalizeAndLoadSceneData(SceneData s)
        {
            var unloadedObjects = s.sceneObjects.Where(x => x.Loaded == false);

            foreach (var sceneObject in unloadedObjects)
            {
                try
                {
                    //Dev.Log($"[ATTEMPTING TO LOAD RESOURCE]");
                    //Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name}");
                    //Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path}");
                    //Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name}");
                    //Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name} OBJ_SCENE:{sceneObject.Scene}");

                    if (s.name == "RESOURCES" || s.name.Contains(DESTROY_ON_LOAD))
                    {
                        Dev.Log("Trying to find resource");
                        var go = GameObjectExtensions.FindResource(sceneObject.path);
                        if (go == null)
                        {
                            Dev.Log("Cannot find resource to load; skipping");
                            continue;
                        }

                        Dev.Log("Trying to create prefab object");
                        var result = CreatePrefabObject(sceneObject.Name, go, sceneObject);

                        if (result == null)
                        {
                            Dev.Log("Cannot create prefab object to load resource; skipping");
                            continue;
                        }

                        //unload the resource if we're not going to use it
                        if (result.prefab != go)
                        {
                            Dev.Log("Unloading resource that will not be used...");
                            go.SetActive(false);
                            GameObject.Destroy(go);
                        }
                    }

                    Dev.Log($"[COMPLETED LOADING RESOURCE] NAME:{sceneObject.Name}");
                }
                catch(Exception e)
                {
                    Dev.LogError($"Failed to load resource or don't destroy on load object. ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }

            try
            {
                //verify
                var finalObjects = s.sceneObjects.Where(x => x.Loaded == false);

                foreach (var sceneObject in finalObjects)
                {
                    Dev.Log($"[NOT LOADED] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name} OBJ_SCENE:{sceneObject.Scene}");
                }
            }
            catch (Exception e)
            {
                Dev.LogError($"Failed to verify remaining unloaded objects. ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        public bool ContainsKey(string name)
        {
            return Objects.ContainsKey(name);
        }

        /// <summary>
        /// Spawn an object from the database. All objects may be checked via the Objects property in the database.
        /// </summary>
        /// <param name="p">The name of the database object to spawn</param>
        /// <returns>The object that was spawned. Returns null on failure.</returns>
        public GameObject Spawn(string name)
        {
            if (!Objects.TryGetValue(name, out PrefabObject p))
                return null;

            Dev.Log("trying to spawn "+ name);
            return Spawn(p);
        }


        /// <summary>
        /// Spawn an object from the database. All objects may be checked via the Objects property in the database.
        /// </summary>
        /// <param name="p">The name of the database object to spawn</param>
        /// <param name="defaultType">The fallback spawner to use if the object doesn't have one known to the database</param>
        /// <returns>The object that was spawned. Returns null on failure.</returns>
        public GameObject Spawn(PrefabObject p, Type defaultType = null)
        {
            if (defaultType == null)
                defaultType = typeof(DefaultSpawner);

            bool isDefault = GetSpawner(p, defaultType, out var spawner);
            Dev.Log("Spawner is " + spawner);
            if (spawner == null)
                return null;

            Dev.Log("finally trying to spawn "+p.prefabName);
            return spawner.Spawn(p, null);
        }

        /// <summary>
        /// Spawn an object from the database. All objects may be checked via the Objects property in the database.
        /// </summary>
        /// <param name="objectToReplace">A valid Metadata object that references the thing to replace</param>
        /// <param name="prefabToSpawn">The database reference to the object that will replace the source</param>
        /// <param name="defaultType">The fallback spawner to use if the object doesn't have one known to the database</param>
        /// <returns>The object that was spawned. Returns null on failure.</returns>
        public GameObject Replace(GameObject objectToReplace, PrefabObject prefabToSpawn, Type defaultType = null)
        {  
            if (defaultType == null)
                defaultType = typeof(DefaultSpawner);

            bool isDefault = GetSpawner(prefabToSpawn, defaultType, out var spawner);
            Dev.Log("in replace, Spawner is " + spawner);
            if (spawner == null)
                return null;

            Dev.Log($"trying to spawn {prefabToSpawn} as a replacement for {objectToReplace}");
            return spawner.Spawn(prefabToSpawn, objectToReplace);
        }

        public static string ModAssetPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(EnemyRandomizerDatabase).Assembly.Location);
            }
        }

        public static string GetDatabaseFilePath(string filename)
        {
            return Path.Combine(ModAssetPath, filename);
        }

        public static bool LoadDatabase(string fileName, out EnemyRandomizerDatabase db)
        {
            try
            {
                if (GetDatabaseFilePath(fileName).DeserializeXMLFromFile<EnemyRandomizerDatabase>(out db))
                {
                    Dev.Log($"Loaded EnemyRandomizerDatabase from {GetDatabaseFilePath(fileName)}");
                    Dev.Log("Loaded database version: " + db.version);

                    db.LinkObjectsToScenes();
                }
            }
            catch (Exception e)
            {
                Dev.LogError("Failed to load EnemyRandomizerDatabase data. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
                db = null;
            }

            return db != null;
        }

        //public static bool IsDatabaseObject(GameObject gameObject)
        //{
        //    //if (excludedComponents.Any(x => gameObject.GetComponent(x)))
        //    //    return false;

        //    //if (!dataBaseObjectComponents.Any(x => gameObject.GetComponent(x)))
        //    //    return false;

        //    string key = ToDatabaseKey(gameObject.name);

        //    if (string.IsNullOrEmpty(key))
        //        return false;

        //    return true;
        //}

        //public static bool IsDatabaseObject(string gameObjectName, EnemyRandomizerDatabase db = null)
        //{
        //    string key = ToDatabaseKey(gameObjectName);

        //    if (string.IsNullOrEmpty(key))
        //        return false;

        //    var DB = db == null ? EnemyRandomizerDatabase.GetDatabase() : db;

        //    return (DB.Objects.ContainsKey(key));
        //}

        //public static bool IsDatabaseKey(string databaseKey, EnemyRandomizerDatabase db = null)
        //{
        //    if (string.IsNullOrEmpty(databaseKey))
        //        return false;

        //    var DB = db == null ? EnemyRandomizerDatabase.GetDatabase() : db;

        //    return (DB.Objects.ContainsKey(databaseKey));
        //}

        public static string GetDatabaseKey(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            return ToDatabaseKey(gameObject.name);
        }

        public static string ToDatabaseKey(string databaseKey)
        {
            //check the string vs a known list of acceptable values first
            if (TryConvertUniqueNameToCommonName(databaseKey, out string firstAttempt))
                return firstAttempt;

            //clean generic junk from the strings
            garbageValues.ForEach(x => databaseKey = RemoveAll(databaseKey, x));

            //use specific characters to chop off string data after them
            endingValues.ForEach(x => databaseKey = TrimStringAfter(databaseKey,x));

            //is this a database key that ends in a number?
            if (IsAKeyEndingWithANumber(databaseKey))
                return databaseKey;

            //clean numbers off the ends of the string
            numberValues.ForEach(x => databaseKey = TrimStringAfter(databaseKey,x));

            //after most modifications, check the string vs a known list of acceptable values again
            if (TryConvertUniqueNameToCommonName(databaseKey,out string finalAttempt))
                return finalAttempt;

            //final whitespace trim
            databaseKey = databaseKey.Trim();

            //check to see if this is a special forbidden item
            if (IsBadDatabaseKeyItem(databaseKey))
                return null;

            //return the generated key
            return databaseKey;
        }

        /// <summary>
        /// This allows the spawned object to be processed by the enemy randomizer. This means the returned object might not be what you expect...
        /// </summary>
        public static GameObject CustomSpawn(Vector3 pos, string objectName, bool setActive = true)
        {
            try
            {
                var enemy = EnemyRandomizerDatabase.GetDatabase().Spawn(objectName);

                if (enemy != null)
                {
                    enemy.transform.position = pos;
                    if (setActive)
                    {
                        GameObject spawnedObject = enemy;
                        var handle = EnemyRandomizerDatabase.OnObjectReplaced.AsObservable().Subscribe(x =>
                        {
                            spawnedObject = x.newObject;
                        });
                        enemy.SetActive(true);
                        handle.Dispose();
                        return spawnedObject;
                    }
                    return enemy;
                }
                else
                {
                    if (!EnemyRandomizerDatabase.GetDatabase().ContainsKey(objectName))
                        throw new KeyNotFoundException($"{objectName} was not found in the object database");
                }
            }
            catch (KeyNotFoundException e)
            {
                Dev.LogError("Error: Invalid or missing object key: " + e.Message + e.StackTrace);
            }
            catch (Exception e)
            {
                Dev.LogError($"CustomSpawn: Error Spawning object, {objectName} at {pos} : " + e.Message + e.StackTrace);
            }

            return null;
        }
    }

    public static class DatabaseExt
    {
        ///// <summary>
        ///// Get the metadata wrapper for this game object, if no database is provided this call will try and use the global getter
        ///// </summary>
        //public static ObjectMetadata ToMetadata(this GameObject gameObject, EnemyRandomizerDatabase database = null)
        //{
        //    if(database == null)
        //    {
        //        if (EnemyRandomizerDatabase.GetDatabase != null)
        //        {
        //            database = EnemyRandomizerDatabase.GetDatabase();
        //        }
        //    }

        //    if (database == null)
        //    {
        //        Dev.LogError("A database reference is required to generate the metadata object. None was provided and the fallback global getter was null.");
        //        return null;
        //    }

        //    if (!EnemyRandomizerDatabase.IsDatabaseObject(gameObject))
        //        return null;

        //    return ObjectMetadata.Get(gameObject);
        //}

        public static List<PrefabObject> GetObjectTypeCollection(this GameObject gameObject)
        {
            var database = EnemyRandomizerDatabase.GetDatabase();

            if (database == null)
            {
                Dev.LogError("A database reference is required to generate the metadata object. None was provided and the fallback global getter was null.");
                return null;
            }

            if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy)
                return database.enemyPrefabs;
            else if (gameObject.ObjectType() == PrefabObject.PrefabType.Hazard)
                return database.hazardPrefabs;
            else if (gameObject.ObjectType() == PrefabObject.PrefabType.Effect)
                return database.effectPrefabs;
            else if (gameObject.ObjectType() == PrefabObject.PrefabType.Other)
                return database.otherPrefabs;
            else
            {
                Dev.LogError("Should not happen! If a new case or type has been added then update this as this will be expensive to return");
                return database.enemyPrefabs.Concat(database.hazardPrefabs).Concat(database.effectPrefabs).ToList();
            }
        }
    }
}
