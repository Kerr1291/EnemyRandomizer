using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizerDatabase
    {
        static string DESTROY_ON_LOAD = "DestroyOnLoad";
        static string RESOURCES = "RESOURCES";

        public static Func<EnemyRandomizerDatabase> GetDatabase;

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
                var unloadedObjects = s.sceneObjects.Where(x => x.Loaded == false);

                foreach(var sceneObject in unloadedObjects)
                {
                    //Dev.Log($"[NOT LOADED] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name} OBJ_SCENE:{sceneObject.Scene}");

                    if(s.name == "RESOURCES" || s.name.Contains(DESTROY_ON_LOAD))
                    {
                        var go = FindResource(sceneObject.path);
                        if (go == null)
                        {
                            Dev.Log("Failed to load resource");
                            continue;
                        }

                        var result = CreatePrefabObject(sceneObject.Name, go, sceneObject);

                        //unload the resource if we're not going to use it
                        if (result.prefab != go)
                        {
                            go.SetActive(false);
                            GameObject.Destroy(go);
                        }
                    }
                }

                //verify
                var finalObjects = s.sceneObjects.Where(x => x.Loaded == false);

                foreach (var sceneObject in finalObjects)
                {
                    Dev.Log($"[NOT LOADED] NAME:{sceneObject.Name} - PATH:{sceneObject.path} - SCENE:{s.name} OBJ_SCENE:{sceneObject.Scene}");
                }
            }

            Dev.Log("Object Database Loading Complete!");
            onComplete?.Invoke();
            yield break;
        }

        public GameObject Spawn(string name)
        {
            if (!Objects.TryGetValue(name, out PrefabObject p))
                return null;

            return Spawn(p);
        }

        public GameObject Spawn(PrefabObject p, Type defaultType = null)
        {
            if (defaultType == null)
                defaultType = typeof(DefaultSpawner);

            var spawner = GetSpawner(p, defaultType);
            Dev.Log("Spawner is " + spawner);
            if (spawner == null)
                return null;

            Dev.Log("finally trying to spawn "+p.prefabName);
            return spawner.Spawn(p);
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

            if (TryConvertUniqueNameToCommonName(databaseKey,out string finalAttempt))
                return finalAttempt;

            //final whitespace trim
            databaseKey = databaseKey.Trim();

            if (IsBadDatabaseKeyItem(databaseKey))
                return null;

            //return the generated key
            return databaseKey;
        }
    }
}
