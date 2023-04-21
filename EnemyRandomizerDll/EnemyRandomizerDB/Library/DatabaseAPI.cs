using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using UniRx;

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
        static string DESTROY_ON_LOAD = "DestroyOnLoad";
        static string RESOURCES = "RESOURCES";

        public static Func<EnemyRandomizerDatabase> GetDatabase;
        public static Func<ReactiveProperty<List<GameObject>>> GetBlackBorders;

        //Params: Position, Object Name, (Optional: null or Object Replacement Name), bool: SetActive? -- Returns the created (or replaced) game object using the randomizer's custom methods
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

                if (db.badSceneData == null)
                    db.badSceneData = new List<SceneData>();

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

            if(verboseSpawnerErrorsForDebuggingOnly)
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
                    if (verboseSpawnerErrorsForDebuggingOnly)
                        Dev.Log($"SCENE:{s.Key} PATH:{x.Key} OBJECT:{x}");

                    if (x.Value == null)
                    {
                        if (verboseSpawnerErrorsForDebuggingOnly)
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
                    Dev.Log($"[ATTEMPTING TO LOAD RESOURCE]");
                    Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name}");
                    Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path}");
                    Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name}");
                    Dev.Log($"[ATTEMPTING TO LOAD RESOURCE] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name} OBJ_SCENE:{sceneObject.Scene}");

                    if (s.name == "RESOURCES" || s.name.Contains(DESTROY_ON_LOAD))
                    {
                        Dev.Log("Trying to find resource");
                        var go = FindResource(sceneObject.path);
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

        /// <summary>
        /// Spawn an object from the database. All objects may be checked via the Objects property in the database.
        /// </summary>
        /// <param name="p">The name of the database object to spawn</param>
        /// <param name="source">Optional: source data that may be used to configure the object</param>
        /// <param name="defaultType">Optional: the fallback spawner to use if the object doesn't have one defined</param>
        /// <returns>The object that was spawned. Returns null on failure.</returns>
        public GameObject Spawn(string name, ObjectMetadata source)
        {
            if (!Objects.TryGetValue(name, out PrefabObject p))
                return null;

            return Spawn(p, source);
        }

        /// <summary>
        /// Spawn an object from the database. All objects may be checked via the Objects property in the database.
        /// </summary>
        /// <param name="p">The database object to spawn</param>
        /// <param name="source">Optional: source data that may be used to configure the object</param>
        /// <param name="defaultType">Optional: the fallback spawner to use if the object doesn't have one defined</param>
        /// <returns>The object that was spawned. Returns null on failure.</returns>
        public GameObject Spawn(PrefabObject p, ObjectMetadata source, Type defaultType = null)
        {  
            if (defaultType == null)
                defaultType = typeof(DefaultSpawner);

            bool isDefault = GetSpawner(p, defaultType, out var spawner);
            Dev.Log("Spawner is " + spawner);
            if (spawner == null)
                return null;

            Dev.Log("finally trying to spawn "+p.prefabName);
            var result = spawner.Spawn(p, source);

            if(result != null && isDefault)
            {
                var defaultControl = result.AddComponent<DefaultSpawnedEnemyControl>();
                defaultControl.Setup(source);
            }

            return result;
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

        public static bool IsDatabaseObject(GameObject gameObject)
        {
            if (excludedComponents.Any(x => gameObject.GetComponent(x)))
                return false;

            if (!dataBaseObjectComponents.Any(x => gameObject.GetComponent(x)))
                return false;

            string key = ToDatabaseKey(gameObject.name);

            if (string.IsNullOrEmpty(key))
                return false;

            return true;
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

        public static GameObject CustomSpawn(Vector3 pos, string objectName, bool setActive = true)
        {
            try
            {
                var enemy = EnemyRandomizerDatabase.GetDatabase().Spawn(objectName, null);
                if (enemy != null)
                {
                    enemy.transform.position = pos;
                    //var defaultEnemyControl = enemy.GetComponent<DefaultSpawnedEnemyControl>();
                    if (setActive)
                        enemy.SetActive(true);
                    return enemy;
                }
            }
            catch (Exception e)
            {
                Dev.LogError("Custom Spawn Error: " + e.Message);
            }

            return null;
        }
    }

    public static class DatabaseExt
    {
        /// <summary>
        /// Get the metadata wrapper for this game object, if no database is provided this call will try and use the global getter
        /// </summary>
        public static ObjectMetadata ToMetadata(this GameObject gameObject, EnemyRandomizerDatabase database = null)
        {
            if(database == null)
            {
                if (EnemyRandomizerDatabase.GetDatabase != null)
                {
                    database = EnemyRandomizerDatabase.GetDatabase();
                }
            }

            if (database == null)
            {
                Dev.LogError("A database reference is required to generate the metadata object. None was provided and the fallback global getter was null.");
                return null;
            }

            ObjectMetadata metaObject = new ObjectMetadata();
            metaObject.Setup(gameObject, database);
            return metaObject;
        }

        public static List<PrefabObject> GetObjectTypeCollection(this ObjectMetadata metaObject, EnemyRandomizerDatabase database = null)
        {
            if (database == null)
            {
                if (EnemyRandomizerDatabase.GetDatabase != null)
                {
                    database = EnemyRandomizerDatabase.GetDatabase();
                }
            }

            if (database == null)
            {
                Dev.LogError("A database reference is required to generate the metadata object. None was provided and the fallback global getter was null.");
                return null;
            }

            if (metaObject.ObjectType == PrefabObject.PrefabType.Enemy)
                return database.enemyPrefabs;
            else if (metaObject.ObjectType == PrefabObject.PrefabType.Hazard)
                return database.hazardPrefabs;
            else if (metaObject.ObjectType == PrefabObject.PrefabType.Effect)
                return database.effectPrefabs;
            else
            {
                Dev.LogError("Should not happen! If a new case or type has been added then update this as this will be expensive to return");
                return database.enemyPrefabs.Concat(database.hazardPrefabs).Concat(database.effectPrefabs).ToList();
            }
        }
    }
}
