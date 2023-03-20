using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

namespace EnemyRandomizerMod
{
//    internal static class DatabaseExtensions
//    {  
//        internal static string GetSceneHierarchyPath(this GameObject gameObject)
//        {
//            if (gameObject == null)
//                return "null";

//            string objStr = gameObject.name;

//            if (gameObject.transform.parent != null)
//                objStr = gameObject.transform.parent.gameObject.GetSceneHierarchyPath() + "/" + gameObject.name;

//            return objStr;
//        }

//        internal static string Remove(this string str, string toRemove)
//        {
//            int length = str.Length;
//            if (str.Contains(toRemove))
//            {
//                int toRemoveLength = toRemove.Length;
//                int toRemoveIndex = str.IndexOf(toRemove);
//                return str.Substring(0, toRemoveIndex) +
//                       str.Substring(toRemoveIndex + toRemoveLength);
//            }
//            else
//                return str;
//        }

//        internal static bool DeserializeXMLFromFile<T>(this string path, out T data) where T : class
//        {
//            data = null;

//            if (!File.Exists(path))
//            {
//                //throw new FileLoadException( "No file found at " + path );
//                Dev.LogError($"No file found at {path}");
//                return false;
//            }

//            bool returnResult = true;

//            XmlSerializer serializer = new XmlSerializer(typeof(T));
//            FileStream fstream = null;
//            try
//            {
//                fstream = new FileStream(path, FileMode.Open);
//            }
//            catch (System.ArgumentException ex1)
//            {
//#if UNITY_EDITOR
//                Dev.LogError( "Error opening file." );
//                Dev.LogError( ex1.GetType() + "/" + ex1.ParamName + "/" + ex1.Message );
//#else
//                Console.WriteLine("Error opening file.");
//                Console.WriteLine(ex1.GetType() + "/" + ex1.ParamName + "/" + ex1.Message);
//#endif

//                returnResult = false;
//                fstream.Close();
//                fstream = null;
//            }
//            catch (System.Exception ex2)
//            {
//#if UNITY_EDITOR
//                Dev.LogError( "Error opening file." );
//#else
//                Console.WriteLine("Error opening file.");
//#endif

//                while (ex2 != null)
//                {
//#if UNITY_EDITOR
//                    Dev.LogError( ex2.GetType() + "/" + ex2.Message + "/" );
//#else
//                    Console.WriteLine(ex2.GetType() + "/" + ex2.Message + "/");
//#endif
//                    ex2 = ex2.InnerException;
//                }

//                returnResult = false;
//                fstream.Close();
//                fstream = null;
//            }


//            try
//            {
//                if (fstream != null)
//                    data = serializer.Deserialize(fstream) as T;
//            }
//            catch (System.Xml.XmlException ex3)
//            {
//#if UNITY_EDITOR
//                Dev.LogError( "Error Deserializing file." );
//                Dev.LogError( ex3.Message + "/" + ex3.LineNumber + "/" + ex3.LinePosition );
//#else
//                Console.WriteLine("Error Deserializing file.");
//                Console.WriteLine(ex3.Message + "/" + ex3.LineNumber + "/" + ex3.LinePosition);
//#endif

//                returnResult = false;
//            }
//            catch (System.ArgumentException ex4)
//            {
//#if UNITY_EDITOR
//                Dev.LogError( "Error Deserializing file." );
//                Dev.LogError( ex4.GetType() + "/" + ex4.ParamName + "/" + ex4.Message );
//#else
//                Console.WriteLine("Error Deserializing file.");
//                Console.WriteLine(ex4.GetType() + "/" + ex4.ParamName + "/" + ex4.Message);
//#endif

//                returnResult = false;
//            }
//            catch (System.Exception ex5)
//            {
//#if UNITY_EDITOR
//                Dev.LogError( "Error Deserializing file." );
//#else
//                Console.WriteLine("Error Deserializing file.");
//#endif

//                while (ex5 != null)
//                {
//#if UNITY_EDITOR
//                    Dev.LogError( ex5.GetType() + "/" + ex5.Message + "/" );
//#else
//                    Console.WriteLine(ex5.GetType() + "/" + ex5.Message + "/");
//#endif
//                    ex5 = ex5.InnerException;
//                }

//                returnResult = false;
//            }
//            finally
//            {
//                if (fstream != null)
//                    fstream.Close();
//            }

//            return returnResult;
//        }
//    }

    public partial class EnemyRandomizerDatabase
    {

        static List<GameObject> GetDirectChildren(GameObject gameObject)
        {
            List<GameObject> children = new List<GameObject>();
            if (gameObject == null)
                return children;

            for (int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                children.Add(child.gameObject);
            }
            return children;
        }

        GameObject FindResource(string pathName)
        {
            string[] path = pathName.Trim('/').Split('/');

            if (path.Length <= 0)
                return null;

            var possiblePrefabs = Resources.FindObjectsOfTypeAll<GameObject>();
            var found = possiblePrefabs.FirstOrDefault(x => x.GetSceneHierarchyPath() == pathName);

            //if(found == null)
            //{
            //    Dev.Log("===========================================================");
            //    Dev.Log($"Was searching for {pathName} in resources and could not find it. Dumping possible matches");
            //    possiblePrefabs.ToList().ForEach(x => Dev.Log($"[OBJ:{x} SCENE:{x.scene.name} NAME:{x.name} PATH:{x.GetSceneHierarchyPath()}]"));
            //    Dev.Log("===========================================================");
            //}

            return found;
        }

        //static GameObject FindGameObject(string pathName)
        //{
        //    string[] path = pathName.Trim('/').Split('/');

        //    if (path.Length <= 0)
        //        return null;

        //    GameObject root = null;

        //    //search for a game object with a name that matches the first string
        //    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
        //    {
        //        Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
        //        if (!s.IsValid() || !s.isLoaded)
        //            continue;
        //        //Dev.Log("Searching " + s.name);
        //        root = s.GetRootGameObjects().Where(x => string.Compare(x.name, path[0]) == 0).FirstOrDefault();
        //        //Dev.LogVarArray("root scene object", s.GetRootGameObjects().Select(x => x.name).ToArray());

        //        if (root != null)
        //            break;
        //    }

        //    if (root == null)
        //        return null;

        //    return FindGameObject(root,pathName);
        //}

        //static GameObject FindGameObject(GameObject gameObject, string pathName)
        //{
        //    string[] path = pathName.Trim('/').Split('/');

        //    if (gameObject.name != path[0])
        //        return null;

        //    List<string> remainingPath = new List<string>(path);
        //    remainingPath.RemoveAt(0);

        //    if (remainingPath.Count <= 0)
        //        return gameObject;

        //    string subPath = string.Join("/", remainingPath.ToArray());

        //    var children = GetDirectChildren(gameObject);

        //    foreach (var child in children)
        //    {
        //        GameObject found = FindGameObject(child,subPath);
        //        if (found != null)
        //            return found;
        //    }

        //    return null;
        //}

        void LinkObjectsToScenes()
        {
            for (int i = 0; i < scenes.Count; ++i)
            {
                scenes[i].sceneObjects.ForEach(x => x.Scene = scenes[i]);
            }
        }

        PrefabObject CreatePrefabObject(string name, GameObject go, SceneObject s)
        {
            PrefabObject prefabObject = null;

            if (s.Loaded)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.LogError($"[{s.Scene.name}] {s.path} = This scene object has already been loaded!");
                return s.LoadedObject;
            }

            string typeName = ToDatabaseKey(name);
            if (!string.IsNullOrEmpty(s.customTypeName))
            {
                typeName = s.customTypeName;
            }

            var config = GetPrefabConfig(typeName, typeof(DefaultPrefabConfig));

            if (config != null)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.Log($"[{s.Scene.name}] {s.path} = {config.GetType()} - Running Setup");

                prefabObject = new PrefabObject()
                {
                    prefab = go,
                    source = s,
                    prefabType = PrefabObject.PrefabType.None
                };

                config.SetupPrefab(prefabObject);

                if (prefabObject.prefab == null)
                {
                    Dev.LogError($"{s.path} : PREFAB OBJECT CREATED WITH NULL GAME OBJECT REFERENCE");
                    return null;
                }

                if (string.IsNullOrEmpty(prefabObject.prefabName))
                {
                    prefabObject.prefabName = ToDatabaseKey(prefabObject.prefab.name);
                }

                //same as the preloaded object
                if(go == prefabObject.prefab)
                {
                    //TODO: may need to clone the new object
                }
                else
                {
                    //it's something new now
                }
            }
            else
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.LogError($"[{s.Scene.name}] {s.path} = FAILED TO FIND ANY PREFAB CONFIG FOR THIS TYPE (even a default one)");
            }

            if (prefabObject == null)
                return null;

            //don't double load
            if (Objects.ContainsKey(prefabObject.prefabName))
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.LogWarning($"{prefabObject.prefabName} already exists in the loaded object database");
                return null;
            }
            else
            {
                prefabObject.prefab.name = ToDatabaseKey(prefabObject.prefab.name);
                prefabObject.prefab.SetActive(false);
                GameObject.DontDestroyOnLoad(prefabObject.prefab);
            }

            //setup this stuff after "setup prefab" because the prefab name could've changed
            if (enemyNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Enemy;
                enemyPrefabs.Add(prefabObject);
                Enemies.Add(prefabObject.prefabName, prefabObject);
            }
            else if (hazardNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Hazard;
                hazardPrefabs.Add(prefabObject);
                Hazards.Add(prefabObject.prefabName, prefabObject);
            }
            else if (effectNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Effect;
                effectPrefabs.Add(prefabObject);
                Effects.Add(prefabObject.prefabName, prefabObject);
            }
            else
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Other;
            }

            Objects.Add(prefabObject.prefabName, prefabObject);

            s.Loaded = true;
            s.LoadedObject = prefabObject;

            return prefabObject;
        }

        //TODO: remove me -- keeping for now in case i want to pull something out of here later. I was playing around with doing the object loading "by hand" but now we let the mod hooks handle it
        //IEnumerator BuildPrefabs(string sceneData)
        //{
        //    yield break;
        //    //if(sceneData == "Crossroads_01")
        //    //{
        //    //    Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Crossroads_01");
        //    //    Dev.Log($"SCENE STATE - [{s.buildIndex}][{s.name}] - VALID:{s.IsValid()} LOADED:{s.isLoaded}");
        //    //    var gos = s.GetRootGameObjects();
        //    //    gos.ToList().ForEach(x => nv.GameObjectExtensions.PrintSceneHierarchyTree(x, false, file: null));

        //    //    Dev.Log("trying method 1");
        //    //    var menderbug1 = FindGameObject("_Scenery/Mender Bug");
        //    //    Dev.Log($"Mender bug results: {menderbug1} ");
        //    //    if (menderbug1 != null)
        //    //    {
        //    //        var menderBug1Copy = GameObject.Instantiate(menderbug1);
        //    //        Dev.Log($"Mender bug results: {menderbug1}{menderBug1Copy} ");
        //    //    }
        //    //    Dev.Log("trying method 2");

        //    //    var menderbug2 = GameObject.Find("Mender Bug");
        //    //    Dev.Log($"Mender bug results: {menderbug1} - {menderbug2} ");
        //    //    if (menderbug2 != null)
        //    //    {
        //    //        var menderBug2Copy = GameObject.Instantiate(menderbug2);
        //    //        Dev.Log($"Mender bug results: {menderbug1} - {menderbug2}{menderBug2Copy} ");
        //    //    }

        //    //    Dev.Log("trying method 3");
        //    //    var menderbug3 = gos.SelectMany(x => x.GetComponentsInChildren<Transform>(true)).FirstOrDefault(x => x.name == "Mender Bug");
        //    //    Dev.Log($"Mender bug results: {menderbug1} - {menderbug2} - {menderbug3}");
        //    //    if (menderbug3 != null)
        //    //    {
        //    //        var menderBug3Copy = GameObject.Instantiate(menderbug3);
        //    //        Dev.Log($"Mender bug results: {menderbug1} - {menderbug2} - {menderbug3}{menderBug3Copy}");
        //    //    }

        //    //    Dev.Log("trying method 4");
        //    //    var menderbug4 = gos.Select(x => x).FirstOrDefault(x => x.transform.Find("Mender Bug") != null).transform.Find("Mender Bug").gameObject;
        //    //    Dev.Log($"Mender bug results: {menderbug1} - {menderbug2} - {menderbug3}");
        //    //    if (menderbug4 != null)
        //    //    {
        //    //        var menderBug4Copy = GameObject.Instantiate(menderbug4);
        //    //        Dev.Log($"Mender bug results: {menderbug1} - {menderbug2} - {menderbug3} - {menderbug4}{menderBug4Copy}");
        //    //    }
        //    //}

        //    //yield break;


        //    //Dev.Log("Building prefabs for scene " + sceneData);

        //    //SceneData scene = scenes.FirstOrDefault(s => s.name == sceneData);
        //    //if (scene == null)
        //    //    yield break;

        //    //Scenes.Add(scene.name, scene);

        //    //scene.sceneObjects.ForEach(x =>
        //    //{
        //    //    Dev.Log("Searching path " + x.path);
        //    //    GameObject go = null;
                
        //    //    go = FindResource(x.path);

        //    //    if(go == null)
        //    //    {
        //    //        go = FindGameObject(x.path);
        //    //    }

        //    //    if (go == null)
        //    //    {
        //    //        Dev.Log("Failed to load " + x.path + " during scene load of " + sceneData);
        //    //        return;
        //    //    }

        //    //    CreatePrefabObject(x.Name, go, x);
        //    //});


        //    //Dev.Log("build prefabs step complete");

        //    //yield return null;
        //}

        bool verboseSpawnerErrorsForDebuggingOnly = true;

        IPrefabConfig GetPrefabConfig(string prefabConfigNameToUse, Type defaultType = null)
        {
            string typeName = "EnemyRandomizerMod." + string.Join("",prefabConfigNameToUse.Split(' ')) + "PrefabConfig";
            Type configType = null;

            if (verboseSpawnerErrorsForDebuggingOnly)
                Dev.Log("Building prefab for type " + typeName);

            try
            {
                configType = typeof(EnemyRandomizerDatabase).Assembly.GetType(typeName);
            }
            catch (Exception e)
            {
            }

            if (configType == null)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                {
                    Dev.LogError($"Cannot find prefab config with type {typeName} in the database assembly");
                    if (!once)
                        typeof(EnemyRandomizerDatabase).Assembly.GetTypes().ToList().ForEach(x => Dev.Log($"Type = {x.Name}"));
                    once = true;
                }
            }
            else
            {
                if (!typeof(IPrefabConfig).IsAssignableFrom(configType))
                {
                    Dev.LogError($"configType given is not an IPrefabConfig: {configType}");
                    return null;
                }
            }

            if (configType == null && defaultType != null)
            {
                if (!typeof(IPrefabConfig).IsAssignableFrom(defaultType))
                {
                    Dev.LogError($"Default configType given is not an IPrefabConfig: {defaultType}");
                    return null;
                }

                configType = defaultType;
            }

            return (IPrefabConfig)Activator.CreateInstance(configType);
        }
        static bool once = false;
        bool GetSpawner(PrefabObject p, Type defaultType, out ISpawner spawnerTypeToUse)
        {
            bool isDefault = false;
            string typeName = "EnemyRandomizerMod." + string.Join("", p.prefabName.Split(' ')) + "Spawner";
            Dev.Log(typeName);
            Type spawnerType = null;

            try
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.Log($"Trying to spawn type {typeName}");
                spawnerType = typeof(EnemyRandomizerDatabase).Assembly.GetType(typeName);
            }
            catch (Exception e)
            {
            }

            if(spawnerType == null)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.Log($"Failed to spawn type {typeName}");

                if (verboseSpawnerErrorsForDebuggingOnly)
                {
                    Dev.LogError($"Cannot find spawner with type {typeName} in the database assembly");
                    if(!once)
                    typeof(EnemyRandomizerDatabase).Assembly.GetTypes().ToList().ForEach(x => Dev.Log($"Type = {x.Name}"));
                    once = true;
                }
            }

            if (spawnerType == null && defaultType != null)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.Log($"Trying to spawn default type {defaultType}");
                if (!typeof(ISpawner).IsAssignableFrom(defaultType))
                {
                    Dev.LogError($"Default spawner type given is not an ISpawner: {defaultType}");
                    spawnerTypeToUse = null;
                    return false;
                }

                isDefault = true;
                spawnerType = defaultType;
            }


            if (spawnerType == null)
            {
                if (verboseSpawnerErrorsForDebuggingOnly)
                    Dev.Log($"No matching spawner type found for {p.prefabName}");
                spawnerTypeToUse = null;
                return false;
            }

            if (verboseSpawnerErrorsForDebuggingOnly)
                Dev.Log("A spawner type was found and will be created: " + spawnerType.Name);

            spawnerTypeToUse = (ISpawner)Activator.CreateInstance(spawnerType);
            return isDefault;
        }

        static bool TryConvertUniqueNameToCommonName(string nameKey, out string convertedName)
        {
            return uniqueNameToCommonNameMap.TryGetValue(nameKey, out convertedName);
        }

        static bool IsAKeyEndingWithANumber(string nameKey)
        {
            return keysEndingWithASpecialCharacter.Contains(nameKey);
        }

        static bool IsBadDatabaseKeyItem(string key)
        {
            //if (badObjectKeys.Contains(key))
            //    return true;

            //if (badObjectKeyPrefix.Any(x => key.StartsWith(x)))
            //    return true;

            //if (badObjectKeySuffix.Any(x => key.EndsWith(x)))
            //    return true;

            //if (goodObjectKeyTokens.Any(x => key.Contains(x)))
            //    return false;

            //if (badObjectKeyTokens.Any(x => key.Contains(x)))
            //    return true;

            if (badEnemyDatabaseKeys.Any(x => key.Contains(x)))
                return true;

            if (key.Length < 3)
                return true;

            return false;
        }

        static string RemoveAll(string nameKey, string stringToRemove)
        {
            while (nameKey.Contains(stringToRemove))
            {
                nameKey = nameKey.Remove(stringToRemove);
            }
            return nameKey.Trim();
        }

        static string TrimStringAfter(string nameKey, string keyIndex)
        {
            int indexOfStartParethesis = nameKey.IndexOf(keyIndex);
            if (indexOfStartParethesis > 0)
                nameKey = nameKey.Substring(0, indexOfStartParethesis);

            return nameKey.Trim();
        }

        public static string TrimEnd(string str, string toRemove)
        {
            int length = str.Length;
            int toRemoveLength = toRemove.Length;
            if (str.Contains(toRemove))
                return str.Substring(0, length - toRemoveLength);
            else
                return str;
        }

        public static List<System.Type> dataBaseObjectComponents = new List<System.Type>()
        {
            typeof(EnemyDreamnailReaction),
            typeof(HealthManager),
            typeof(DamageHero),
            typeof(EnemyDeathEffects),
            typeof(DamageEnemies),
            typeof(ParticleSystem),
        };

        static List<System.Type> excludedComponents = new List<System.Type>()
        {
            typeof(TMPro.TextMeshPro),
        };


        static List<string> garbageValues = new List<string>()
        {
            {"(Clone)"},
            {"Fixed"  }
        };

        static List<string> endingValues = new List<string>()
        {
            {" ("},
            {"("}
        };

        static List<string> numberValues = new List<string>()
        {
            {" 1"},
            {" 2"},
            {" 3"},
            {" 4"},
            {" 5"},
            {" 6"},
            {" 7"},
            {" 8"},
            {" 9"},
            {" 10"},
        };

        static Dictionary<string, string> uniqueNameToCommonNameMap = new Dictionary<string, string>()
        {
            {"Spawn Roller v2", "Roller"},
            {"Electric Mage New", "Electric Mage"},
            {"Ruins Sentry FatB", "Ruins Sentry Fat"},
            {"Ruins Flying SentryB", "Ruins Flying Sentry"},
            {"Ruins SentryB", "Ruins Sentry"},
            {"Hatcher NP", "Hatcher"}, //Hatcher NP -- investigate this in fsm viewer //check this -- Hatcher Summon
            {"Mega Jellyfish GG", "Mega Jellyfish"},
            {"Moss Knight C", "Moss Knight"},
            {"Moss Knight B", "Moss Knight"},
            {"Super Spitter Col", "Super Spitter"},
            {"Zombie Basic One 1 (Missing Prefab)", "Zombie Runner"},
            {"Great Shield Zombie bottom", "Great Shield Zombie" },
            {"Ruins Sentry Fat B", "Ruins Sentry Fat" },
            {"Mawlek Turret Ceiling", "Mawlek Turret" }
        };

        //Spitter Summon v2 -- check fsm

        static List<string> keysEndingWithASpecialCharacter = new List<string>()
        {
            {"Zombie Spider 1"},
            {"Zombie Spider 2"},
            {"Zombie Fungus A"},
            {"Zombie Fungus B"},
            {"Hornet Boss 1"},
            {"Hornet Boss 2"},
        };


        //Don't load data for these enemies
        static List<string> badEnemyDatabaseKeys = new List<string>()
        {
            {"Hollow Shade"},
            {"Grub Mimic"},
            {"Shell"},
            {"Cap Hit"},
            {"Head"},
            {"Head Box"},
            {"Tinger"},
            {"Dummy Mantis"},
            {"Fluke Fly Spawner"},
            {"Gate Mantis"},
            {"Real Bat"},
            {"Hatcher Baby Spawner"},
            {"Parasite Balloon Spawner"},
            {"Hiveling Spawner"},
            {"Fluke Fly Spawner"},
            {"Baby Centipede Spawner"},
            {"Mage Balloon Spawner"},
            {"Jellyfish Baby Inert"},
            {"Mantis Lord S2" },
            {"Mantis Lord" },
            {"Mantis Lord S1" },
            {"Mantis Lord S3" },
            {"Hurt Box" },
        };
    }
}






//[XmlRoot(ElementName = "enemyList")]
//public class RandomizerObjectDefinitions : ISceneDataProvider
//{
//    [XmlElement(ElementName = "enemy")]
//    public List<RandomizerObjectDefinition> objectDefinitions;

//    List<(string SceneName, string GameObjectPath)> ISceneDataProvider.GetSceneDataList()
//    {
//        Dev.Log("Count: " + objectDefinitions.Count);
//        return objectDefinitions.Select(x => (SceneName: x.sceneName, GameObjectPath: x.gameObjectPath)).ToList();
//    }

//    [XmlIgnore]
//    static RandomizerObjectDefinitions Instance { get; set; }

//    /// <summary>
//    /// A mapping of enemy names to their xml data
//    /// </summary>
//    [XmlIgnore]
//    public static Dictionary<string, RandomizerObjectDefinition> LoadedObjects
//    {
//        get
//        {
//            //TODO: rewrite this to fix duplicated entries
//            if (loadedObjects == null && Instance.objectDefinitions != null && Instance.objectDefinitions.Count > 0)
//            {
//                var keys = Instance.objectDefinitions.Select(x => x.name);
//                var values = Instance.objectDefinitions.Select(x => x);

//                //TODO: put error checking in for having multiple enemies with the same name
//                var temp = keys.Zip(values, (k, v) => new { k, v });

//                HashSet<string> distinct = new HashSet<string>();
//                var filter = temp.Where(x =>
//                {
//                    bool result = distinct.Add(x.k);
//                    if (!result)
//                    {
//                        Dev.LogError($"Duplicate object name detected! {x.k} already exists in the xml!! Please rename this object or remove the duplicate. This entry will not be loaded.");
//                    }
//                    return result;
//                });

//                var filtered = filter.ToList();
//                loadedObjects = filtered.ToDictionary(x => x.k, x => x.v);
//            }
//            return loadedObjects;
//        }
//    }

//    [XmlIgnore]
//    static Dictionary<string, RandomizerObjectDefinition> loadedObjects;
//}

//[XmlRoot(ElementName = "enemy")]
//public class RandomizerObjectDefinition
//{
//    /// <summary>
//    /// The unique name given to this enemy type
//    /// </summary>
//    [XmlAttribute(AttributeName = "name")]
//    public string name;

//    /// <summary>
//    /// The class used to instantiate, configure, and control this enemy
//    /// </summary>
//    [XmlElement(ElementName = "configurationTypeName", IsNullable = true)]
//    public string typeName;

//    /// <summary>
//    /// The scene this enemy is found in
//    /// </summary>
//    [XmlElement(ElementName = "sceneName")]
//    public string sceneName;

//    /// <summary>
//    /// The location of the game object that represents this enemy
//    /// </summary>
//    [XmlElement(ElementName = "gameObjectPath")]
//    public string gameObjectPath;

//    [XmlIgnore]
//    public IRandomizerEnemy randomizerObject;

//    public override string ToString()
//    {
//        return name;
//    }
//}