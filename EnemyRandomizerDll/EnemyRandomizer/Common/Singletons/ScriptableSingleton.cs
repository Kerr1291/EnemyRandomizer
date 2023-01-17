using UnityEngine;
using System.Security.AccessControl;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nv
{
    //Note: a monobehaviour in a scene must contain a reference to the scriptable object asset for it to be loaded automatically
    public abstract class ScriptableSingleton : ScriptableObject
    {
        public abstract string DefaultName
        {
            get;
        }

        public abstract void OnCreate();
    }

    ///Implementation taken based on: http://wiki.unity3d.com/index.php?title=Singleton
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// </summary>
    public class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableSingleton
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if(_instance == null)
                    {
                        LoadSingletonInstance();
                    }

                    return _instance;
                }
            }
        }
		
        public override string DefaultName
        {
            get
            {
                return "[Scriptable Singleton] " + typeof(T).ToString(); 
            }
        }

        public override void OnCreate()
        {
            name = DefaultName;            
        }

        static string UnityPath
        {
            get
            {
                return Application.dataPath;
            }
        }

        //asset must live inside a /Resources/ folder somewhere in the project if it needs to be loaded by Resources.Load at runtime
        protected static string assetCreationDirectory = "/Resources/";
        protected static string AssetCreationDirectory
        {
            get
            {
                return UnityPath + assetCreationDirectory;
            }
        }

        //update this if you need runtime loading without a reference to this script and you want to move your scriptable object to a different folder
        protected static string assetLocalDirectory = null;
        protected static string AssetLocalDirectory
        {
            get
            {
                return string.IsNullOrEmpty(assetLocalDirectory) ? string.Empty : assetLocalDirectory;
            }
        }

        //update this if you need runtime loading without a reference to this script and you want to rename your scriptable object
        protected static string assetName = null;
        protected static string AssetName
        {
            get
            {
                return string.IsNullOrEmpty(assetName) ? typeof(T).ToString() : assetName;
            }
        }

        protected static string AssetEditorPath
        {
            get
            {
                return UnityPath + AssetCreationDirectory + AssetName;
            }
        }

        protected static string AssetLocalPath
        {
            get
            {
                return AssetLocalDirectory + AssetName;
            }
        }

        protected static void LoadSingletonInstance()
        {
            if(_instance != null)
                return;
#if UNITY_EDITOR
            string[] foundAssets = AssetDatabase.FindAssets("t:" + typeof(T).ToString());
            if(foundAssets.Length > 0)
            {
                _instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(foundAssets[0]));
            }

            if(foundAssets.Length > 1)
            {
                Debug.LogError("[Singleton] More than one scriptable singleton found. " +
                    " - there should never be more than 1 singleton!" +
                    " This LoadSingletonInstance call will return the first one found...");
            }
#else
            _instance = (T)(Resources.FindObjectsOfTypeAll(typeof(T)).FirstOrDefault());                        
#endif

            if(_instance == null)
            {
                //try to load it before we create it
                _instance = Resources.Load<T>(AssetLocalPath);

                if(_instance == null)
					CreateSingletonInstance();
            }
        }

        static protected void CreateSingletonInstance()
        {
            if(_instance != null)
                return;
#if UNITY_EDITOR
            CreateEditorInstance();
#else
            CreateRuntimeInstance();

            Debug.Log("[Scriptable Singleton] An instance of " + typeof(T) +
                " is needed, so '" + _instance +
                "' was created. WARNING: This may not have been intended. If your scriptable singleton is not referenced by any (loaded) monobehaviour then the asset will not have been loaded yet. You may either need to add a reference to this somewhere, or manually load the asset yourself.");
#endif
            _instance.OnCreate();

            Debug.Log("[Scriptable Singleton] An instance of " + typeof(T) +
                " is needed, so '" + _instance +
                "' was created.");
        }
        
        static protected T CreateEditorInstance(string assetCreationDirectory, string assetCreationName)
        {
#if UNITY_EDITOR
            if(_instance != null)
                return _instance;

            _instance = ScriptableObject.CreateInstance<T>();
            
            if(!System.IO.Directory.Exists(assetCreationDirectory))
            {
                System.IO.Directory.CreateDirectory(assetCreationDirectory);
            }

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(assetCreationDirectory);
            di.Attributes = System.IO.FileAttributes.Normal;

            assetCreationDirectory = "Assets" + assetCreationDirectory.Remove(0, Application.dataPath.Length);

            string assetCreationPath = string.Format("{0}{1}.asset", assetCreationDirectory, assetCreationName);

            AssetDatabase.CreateAsset(_instance, assetCreationPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            return _instance;
        }

        static protected T CreateEditorInstance()
        {
            return CreateEditorInstance(AssetCreationDirectory, AssetName);
        }

        static protected T CreateRuntimeInstance()
        {
            if(_instance != null)
                return _instance;            

            _instance = ScriptableObject.CreateInstance<T>();
            _instance.name = AssetName + "(Runtime Instance)";
            return _instance;
        }
    }
}