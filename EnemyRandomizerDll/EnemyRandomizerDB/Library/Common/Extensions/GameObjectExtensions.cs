using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnemyRandomizerMod
{
    public static class GameObjectExtensions
    {
        public static GameObject FindResource(string pathName)
        {
            string[] path = pathName.Trim('/').Split('/');

            if (path.Length <= 0)
                return null;

            var possiblePrefabs = Resources.FindObjectsOfTypeAll<GameObject>();
            var found = possiblePrefabs.FirstOrDefault(x => x.GetSceneHierarchyPath() == pathName);

            return found;
        }

        public static bool ContainsType<T>(this UnityEngine.Object obj)
        {
            if(obj is GameObject || obj is Component)
            {
                if(obj is GameObject)
                {
                    return (obj as GameObject).GetComponents<Component>().Any(x => typeof(T).IsAssignableFrom(x.GetType()));
                }
                else// if(obj is Component)
                {
                    return (obj as Component).GetComponents<Component>().Any(x => typeof(T).IsAssignableFrom(x.GetType()));
                }
            }
            return false;
        }
		
        public static bool FindAndDestroyGameObjectInChildren( this GameObject gameObject, string name )
        {
            bool found = false;
            GameObject toDestroy = gameObject.FindGameObjectInChildrenWithName(name);
            if( toDestroy != null )
            {
                GameObject.Destroy( toDestroy );
                found = true;
            }
            return found;
        }

        public static List<GameObject> GetDirectChildren(this GameObject gameObject)
        {
            List<GameObject> children = new List<GameObject>();
            if(gameObject == null)
                return children;

            for(int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                children.Add(child.gameObject);
            }
            return children;
        }

        public static GameObject FindGameObjectInDirectChildren(this GameObject gameObject, string name)
        {
            if(gameObject == null)
                return null;

            for(int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                if(child.name == name)
                    return child.gameObject;
            }
            return null;
        }

        public static GameObject FindGameObjectInChildrenWithName( this GameObject gameObject, string name )
        {
            if( gameObject == null )
                return null;

            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.name == name )
                    return t.gameObject;
            }
            return null;
        }

        public static GameObject FindGameObjectNameContainsInChildren( this GameObject gameObject, string name )
        {
            if( gameObject == null )
                return null;

            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.name.Contains( name ) )
                    return t.gameObject;
            }
            return null;
        }

        public static List<GameObject> FindGameObjectsNameContainsInChildren(this GameObject gameObject, string name)
        {
            List<GameObject> children = new List<GameObject>();
            if (gameObject == null)
                return null;

            foreach (var t in gameObject.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.Contains(name))
                    children.Add( t.gameObject );
            }
            return children;
        }

        public static TComponent FindObjectOfType<TComponent>(bool includeInactive = true)
            where TComponent : Component
        {
            return FindObjectsOfType<TComponent>(includeInactive).FirstOrDefault();
        }

        public static List<TComponent> FindObjectsOfType<TComponent>(bool includeInactive = true, bool searchLoadingScenes = true)
            where TComponent : Component
        {
            List<TComponent> components = new List<TComponent>();
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!searchLoadingScenes && !s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        var objectsOfType = rootObject.GetComponentsInChildren<TComponent>(includeInactive);
                        if(objectsOfType.Length > 0)
                            components.AddRange(objectsOfType);
                    }
                }
            }
            else
            {
                for(int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!searchLoadingScenes && !s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        var objectsOfType = rootObject.GetComponentsInChildren<TComponent>(includeInactive);
                        if(objectsOfType.Length > 0)
                            components.AddRange(objectsOfType);
                    }
                }
            }
#else
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                if(!s.IsValid())
                    continue;
                if(!searchLoadingScenes && !s.isLoaded)
                    continue;
                var rootObjects = s.GetRootGameObjects();
                foreach(var rootObject in rootObjects)
                {
                    var objectsOfType = rootObject.GetComponentsInChildren<TComponent>(includeInactive);
                    if(objectsOfType.Length > 0)
                        components.AddRange(objectsOfType);
                }
            }
#endif
            return components;
        }

        public static IEnumerable<GameObject> EnumerateRootObjects(bool includeInactive = true)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        yield return rootObject;
                    }
                }
            }
            else
            {
                for(int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        yield return rootObject;
                    }
                }
            }
#else
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                if(!s.IsValid())
                    continue;
                if(!s.isLoaded)
                    continue;
                var rootObjects = s.GetRootGameObjects();
                foreach(var rootObject in rootObjects)
                {
                    yield return rootObject;
                }
            }
#endif
            yield break;
        }

        public static IEnumerable<TComponent> EnumerateRootObjects<TComponent>(bool includeInactive = true)
            where TComponent : Component
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        yield return rootObject.GetComponent<TComponent>();
                    }
                }
            }
            else
            {
                for(int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        yield return rootObject.GetComponent<TComponent>();
                    }
                }
            }
#else
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                if(!s.IsValid())
                    continue;
                if(!s.isLoaded)
                    continue;
                var rootObjects = s.GetRootGameObjects();
                foreach(var rootObject in rootObjects)
                {
                    yield return rootObject.GetComponent<TComponent>();
                }
            }
#endif
            yield break;
        }

        public static IEnumerable<TComponent> EnumerateComponentsInChildren<TComponent>(this GameObject go, bool includeInactive = true)
            where TComponent : Component
        {
            yield return go.GetComponent<TComponent>();
            for(int i = 0; i < go.transform.childCount; ++i)
            {
                var child = go.transform.GetChild(i);
                if(!child.gameObject.activeInHierarchy && !includeInactive)
                    continue;
                foreach(var c in EnumerateComponentsInChildren<TComponent>(child.gameObject, includeInactive))
                {
                    yield return c;
                }
            }
        }

        public static IEnumerable<TComponent> EnumerateObjectsOfType<TComponent>(bool includeInactive = true)
            where TComponent : Component
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        foreach(var c in rootObject.EnumerateComponentsInChildren<TComponent>(includeInactive))
                        {
                            yield return c;
                        }
                    }
                }
            }
            else
            {
                for(int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    Scene s = (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i));
                    if(!s.IsValid())
                        continue;
                    if(!s.isLoaded)
                        continue;
                    var rootObjects = s.GetRootGameObjects();
                    foreach(var rootObject in rootObjects)
                    {
                        foreach(var c in rootObject.EnumerateComponentsInChildren<TComponent>(includeInactive))
                        {
                            yield return c;
                        }
                    }
                }
            }
#else
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                if(!s.IsValid())
                    continue;
                if(!s.isLoaded)
                    continue;
                var rootObjects = s.GetRootGameObjects();
                foreach(var rootObject in rootObjects)
                {
                    foreach(var c in rootObject.EnumerateComponentsInChildren<TComponent>(includeInactive))
                    {
                        yield return c;
                    }
                }
            }
#endif
            yield break;
        }


        public static GameObject GetBattleScene(this HealthManager healthManager)
        {
            FieldInfo fi = typeof(HealthManager).GetField("battleScene", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return fi.GetValue(healthManager) as GameObject;
            }

            return null;
        }


        public static int GetSmallGeo(this HealthManager healthManager)
        {
            FieldInfo fi = typeof(HealthManager).GetField("smallGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return (int)fi.GetValue(healthManager);
            }

            return 0;
        }

        public static int GetMedGeo(this HealthManager healthManager)
        {
            FieldInfo fi = typeof(HealthManager).GetField("mediumGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return (int)fi.GetValue(healthManager);
            }

            return 0;
        }

        public static int GetLargeGeo(this HealthManager healthManager)
        {
            FieldInfo fi = typeof(HealthManager).GetField("largeGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return (int)fi.GetValue(healthManager);
            }

            return 0;
        }

        public static float GetRightScale(this Walker walker)
        {
            FieldInfo fi = typeof(Walker).GetField("rightScale", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return (float)fi.GetValue(walker);
            }
            return float.NaN;
        }

        public static void SetRightScale(this Walker walker, float value)
        {
            FieldInfo fi = typeof(Walker).GetField("rightScale", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                fi.SetValue(walker, value);
            }
        }

        //public static GameObject GetCorpseFromDeathEffects(this EnemyDeathEffects e)
        //{
        //    var rootType = e.GetType();

        //    System.Reflection.FieldInfo GetCorpseField(Type t)
        //    {
        //        return t.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    }

        //    while (rootType != typeof(EnemyDeathEffects) && rootType != null)
        //    {
        //        if (GetCorpseField(rootType) != null)
        //            break;
        //        rootType = rootType.BaseType;
        //    }

        //    if (rootType == null)
        //        return null;

        //    return (GameObject)GetCorpseField(rootType).GetValue(e);
        //}


        public static string GetPlayerDataNameFromDeathEffects(this EnemyDeathEffects e)
        {
            var rootType = e.GetType();

            System.Reflection.FieldInfo GetField(Type t)
            {
                return t.GetField("playerDataName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            while (rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                if (GetField(rootType) != null)
                    break;
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return null;

            return (string)GetField(rootType).GetValue(e);
        }

        public static void SetPlayerDataNameFromDeathEffects(this EnemyDeathEffects e, string pdName)
        {
            var rootType = e.GetType();

            System.Reflection.FieldInfo GetField(Type t)
            {
                return t.GetField("playerDataName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            while (rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                if (GetField(rootType) != null)
                    break;
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return;

            GetField(rootType).SetValue(e, pdName);
        }

        public static GameObject GetCorpsePrefabFromDeathEffects(this EnemyDeathEffects e)
        {
            var rootType = e.GetType();

            while (rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return null;

            return (GameObject)rootType.GetField("corpsePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(e); ;
        }

        public static GameObject FindGameObject(string pathName)
        {
            string[] path = pathName.Trim('/').Split('/');

            //Dev.Log("Searching " + string.Join(", ",path));
            //Dev.LogVarArray("splitpath", path);

            if(path.Length <= 0)
                return null;

            GameObject root = null;

            //search for a game object with a name that matches the first string
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene s = (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
                if(!s.IsValid() || !s.isLoaded)
                    continue;
                //Dev.Log("Searching " + s.name);
                root = s.GetRootGameObjects().Where(x => string.Compare(x.name,path[0]) == 0).FirstOrDefault();
                //Dev.LogVarArray("root scene object", s.GetRootGameObjects().Select(x => x.name).ToArray());

                if(root != null)
                    break;
            }

            //if(root == null)
            //{
            //    Dev.Log("did not find a root object");
            //}
            
            if(root == null)
                return null;

            return root.FindGameObject(pathName);
        }

        public static GameObject FindGameObject(this GameObject gameObject, string pathName)
        {
            string[] path = pathName.Trim('/').Split('/');

            if(gameObject.name != path[0])
                return null;

            List<string> remainingPath = new List<string>(path);
            remainingPath.RemoveAt(0);

            if(remainingPath.Count <= 0)
                return gameObject;

            string subPath = string.Join("/", remainingPath.ToArray());

            var children = gameObject.GetDirectChildren();

            foreach(var child in children)
            {
                GameObject found = child.FindGameObject(subPath);
                if(found != null)
                    return found;
            }

            return null;
        }

        public static string GetSceneHierarchyPath( this GameObject gameObject )
        {
            if( gameObject == null )
                return "null";

            string objStr = gameObject.name;

            if( gameObject.transform.parent != null )
                objStr = gameObject.transform.parent.gameObject.GetSceneHierarchyPath() + "/" + gameObject.name;

            return objStr;
        }

        public static IEnumerable<GameObject> EnumerateChildren(this GameObject gameObject)
        {
            if(gameObject == null)
                yield break;

            for(int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                yield return child.gameObject;
            }
        }

        public static IEnumerable<TComponent> EnumerateChildren<TComponent>(this GameObject gameObject)
             where TComponent : Component
        {
            if(gameObject == null)
                yield break;

            for(int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                TComponent c = child.GetComponent<TComponent>();
                if(c == null)
                    continue;

                yield return c;
            }
        }

        public static void PrintSceneHierarchyChildren(this GameObject gameObject, bool printComponents = false, System.IO.StreamWriter file = null, bool listComponentsOnly = false)
        {
            if(gameObject == null)
                return;

            if(file != null)
            {
                file.WriteLine("START =====================================================");
                file.WriteLine("Printing scene hierarchy for game object: " + gameObject.name);
            }
            else
            {
                Dev.Log("START =====================================================");
                Dev.Log("Printing scene hierarchy for game object: " + gameObject.name);
            }

            string parentObject = gameObject.GetSceneHierarchyPath();

            if(file != null)
            {
                file.WriteLine(parentObject);
            }
            else
            {
                Dev.Log(parentObject);
            }

            for(int k = 0; k < gameObject.transform.childCount; ++k)
            {
                Transform child = gameObject.transform.GetChild(k);
                string objectNameAndPath = child.gameObject.GetSceneHierarchyPath();

                string inactiveString = string.Empty;
                if(child != null && child.gameObject != null && !child.gameObject.activeInHierarchy)
                    inactiveString = " (inactive)";

                if(file != null)
                {
                    file.WriteLine(objectNameAndPath + inactiveString);
                }
                else
                {
                    Dev.Log(objectNameAndPath + inactiveString);
                }


                if(printComponents)
                {
                    string componentHeader = "";
                    for(int i = 0; i < (objectNameAndPath.Length - child.gameObject.name.Length); ++i)
                        componentHeader += " ";

                    foreach(Component c in child.GetComponents<Component>())
                    {
                        c.PrintComponentType(componentHeader, file);

                        if (listComponentsOnly)
                            continue;

                        if(c is Transform)
                            c.PrintTransform(componentHeader, file);
                        else
                            c.PrintComponentWithReflection(componentHeader, file);
                    }
                }
            }

            if(file != null)
            {
                file.WriteLine("END +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }
            else
            {
                Dev.Log("END +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }
        }

        public static void PrintSceneHierarchyTree(this GameObject gameObject, bool printComponents, string outputFilePath)
        {
            System.IO.StreamWriter file = null;
            if (!string.IsNullOrEmpty(outputFilePath))
            {
                try
                {
                    file = new System.IO.StreamWriter(outputFilePath);
                }
                catch (Exception e)
                {
                    Dev.Log("Exception!: " + e.Message);
                    file = null;
                }

                if (file != null)
                {
                    file.WriteLine("START =====================================================");
                    file.WriteLine("Printing scene hierarchy for object: " + gameObject.name + " ");
                    PrintSceneHierarchyTree(gameObject, printComponents, file);
                }
            }
            else
            {
                Dev.Log("START =====================================================");
                Dev.Log("Printing scene hierarchy for object: " + gameObject.name + " ");
                PrintSceneHierarchyTree(gameObject, printComponents, file);
            }
        }

        public static void PrintSceneHierarchyTree( this GameObject gameObject, bool printComponents = false, System.IO.StreamWriter file = null, bool listComponentsOnly = false )
        {
            if( gameObject == null )
                return;

            if( file != null )
            {
                file.WriteLine( "START =====================================================" );
                file.WriteLine( "Printing scene hierarchy for game object: " + gameObject.name );
            }
            else
            {
                Dev.Log( "START =====================================================" );
                Dev.Log( "Printing scene hierarchy for game object: " + gameObject.name );
            }

            foreach( Transform t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                string objectNameAndPath = t.gameObject.GetSceneHierarchyPath();

                string inactiveString = string.Empty;
                if(t != null && t.gameObject != null && !t.gameObject.activeInHierarchy)
                    inactiveString = " (inactive)";

                if( file != null )
                {
                    file.WriteLine( objectNameAndPath + inactiveString);
                }
                else
                {
                    Dev.Log( objectNameAndPath + inactiveString);
                }


                if( printComponents )
                {
                    string componentHeader = "";
                    for( int i = 0; i < ( objectNameAndPath.Length - t.gameObject.name.Length ); ++i )
                        componentHeader += " ";

                    foreach( Component c in t.GetComponents<Component>() )
                    {
                        c.PrintComponentType( componentHeader, file );

                        if (listComponentsOnly)
                            continue;

                        if(c is Transform)
                            c.PrintTransform(componentHeader, file);
                        else
                            c.PrintComponentWithReflection(componentHeader, file);
                    }
                }
            }

            if( file != null )
            {
                file.WriteLine( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
            }
            else
            {
                Dev.Log( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
            }
        }

        public static T GetOrAddComponent<T>( this GameObject source ) where T : UnityEngine.Component
        {
            try
            {
                T result = source.GetComponent<T>();
                if (result != null)
                    return result;
                result = source.AddComponent<T>();
                return result;
            }
            catch (Exception e)
            {
                Dev.LogError($"{source}: GetOrAddComponent has caught an uncaught exception with type {typeof(T)} on this game object ERROR{e.Message} STACKTRACE{e.StackTrace}.");
                return default(T);
            }
        }


        public static void SafeSetActive( this GameObject go, bool state )
        {
            if( go == null )
                return;

            try
            {
                go.SetActive(state);
            }
            catch(Exception e)
            {
                Dev.LogError($"{go}: SafeSetActive has caught an uncaught exception when attempting to activate a game object ERROR{e.Message} STACKTRACE{e.StackTrace}.");
            }
        }


        public static bool SafeIsActive( this GameObject go )
        {
            if( go == null )
                return false;

            return go.activeInHierarchy;
        }

        public static bool IsTouchableType(this GameObject touchableType)
        {
            if (touchableType.GetComponent<Collider>() != null)
                return true;
            if (touchableType.GetComponent<Collider2D>() != null)
                return true;
            if (touchableType.GetComponent<SpriteRenderer>() != null)
                return true;
            if (touchableType.GetComponent<RectTransform>() != null)
                return true;
            return false;
        }
    }
}
