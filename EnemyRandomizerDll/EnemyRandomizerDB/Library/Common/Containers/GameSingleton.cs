using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    public abstract class GameSingleton : MonoBehaviour
    {
        public abstract string DefaultName
        {
            get;
        }
		
        public abstract void OnCreate(); 
    }

    ///Implementation taken from: http://wiki.unity3d.com/index.php?title=Singleton
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class GameSingleton<T> : GameSingleton where T : GameSingleton
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                lock(_lock)
                {
                    if(_instance == null)
                    {
                        var instances = GameObjectExtensions.FindObjectsOfType<T>(true);
                        _instance = instances.FirstOrDefault();

                        if( instances.Count > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if(_instance == null)
                        {
                            if(applicationIsQuitting)
                            {
                                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                    "' already destroyed on application quit." +
                                    " Won't create again - returning null.");
                                return null;
                            }

                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            _instance.OnCreate();
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }

        public override string DefaultName
        {
            get
            {
                return "[Singleton] " + typeof(T).ToString();
            }
        }

        public override void OnCreate()
        {
            name = DefaultName;
            if(Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);

                Debug.Log("[Singleton] An instance of " + typeof(T) +
                    " is needed in the scene, so '" + gameObject +
                    "' was created with DontDestroyOnLoad.");
            }
        }
    }

}