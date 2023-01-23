using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nv
{
    public class MonoBehaviourFactory : MonoBehaviour, IMonoBehaviourFactory
    {
#if UNITY_EDITOR
        [ContextMenu("Add folder of prefabs")] 
        public virtual void SelectPrefabs()
        {
            string startingFolder = nv.editor.EditorData.Instance.GetData<string>( typeof( MonoBehaviourFactory ).Name + name, "SelectPrefabs" + "startingFolder" );
            string path = EditorUtility.SaveFolderPanel("Select folder to import", startingFolder, name);
            if( string.IsNullOrEmpty( path ) )
                return;
            nv.editor.EditorData.Instance.SetData<string>( startingFolder, typeof( MonoBehaviourFactory ).Name + name, "SelectPrefabs" + "startingFolder" );

            string[] fileEntries = Directory.GetFiles( path, "*.prefab", SearchOption.TopDirectoryOnly );

            List<PoolableMonoBehaviour> prefabs = new List<PoolableMonoBehaviour>();
            foreach( string fileName in fileEntries )
            {
                string localPath = "Assets" + fileName.Remove( 0, Application.dataPath.Length ); 
                PoolableMonoBehaviour asset = AssetDatabase.LoadAssetAtPath<PoolableMonoBehaviour>( localPath );                
                if( asset != null )
                {
                    objectMappingKeys.Add( asset.name );
                    objectMappingValues.Add( asset );
                }
            }

            EditorUtility.SetDirty( this );
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty( UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() );
            AssetDatabase.SaveAssets();
        }
#endif
        //TODO: switch these keys+values to a nice serializable dictionary
        [SerializeField]
        public List<string> objectMappingKeys;

        [SerializeField]
        public List<PoolableMonoBehaviour> objectMappingValues;

        protected List<string> EditorKeys
        {
            get
            {
                return objectMappingKeys != null ? objectMappingKeys : (objectMappingKeys = new List<string>());
            }
        }

        protected List<PoolableMonoBehaviour> EditorValues
        {
            get
            {
                return objectMappingValues != null ? objectMappingValues : (objectMappingValues = new List<PoolableMonoBehaviour>());
            }
        }

        protected Dictionary<string, MonoBehaviourPool<object, PoolableMonoBehaviour>> map;

        //Given a string key, maps to a prefab/type of object to create
        public virtual IDictionary<string, MonoBehaviourPool<object, PoolableMonoBehaviour>> Map
        {
            get
            {
                return map ?? (map = new Dictionary<string, MonoBehaviourPool<object, PoolableMonoBehaviour>>());
            }
            set
            {
                UnloadPools();
                Map.Clear();

                foreach(var kvp in value)
                    Map.Add(kvp.Key, kvp.Value);
            }
        }

        //Given a string key, maps to a prefab/type of object to create
        public virtual MonoBehaviourPool<object, PoolableMonoBehaviour> this[string key]
        {
            get
            {
                try
                {
                    return Map[key];
                }
                catch(KeyNotFoundException e)
                {
                    Debug.LogError(e.Message + " " + key);
                    throw e;
                }
            }
            set
            {
                if(Map.ContainsKey(key))
                {
                    Map[key].UnloadAll();
                }
                Map[key] = value;
            }
        }

        //Given a string key and some data, instantiates an object
        public virtual TPoolableMonoBehaviour Get<TPoolableMonoBehaviour>(string key, object setupData, params object[] initParams)
            where TPoolableMonoBehaviour : PoolableMonoBehaviour
        {
            return this[key].Get<TPoolableMonoBehaviour>(setupData, initParams);
        }

        //Given a string key and some data, instantiates an object
        public virtual PoolableMonoBehaviour Get(string key, object setupData, params object[] initParams)
        {
            return Get<PoolableMonoBehaviour>(key, setupData, initParams);
        }

        //For very specific edge cases where a delayed enpool was used (the enpooling is queued to be done on lateupdate in this case)
        public virtual PoolableMonoBehaviour CheckDelayedAndGet(string key, object setupData, params object[] initParams)
        {
            if(delayedEnPool.Count > 0 && delayedEnPool.ContainsKey(key) && delayedEnPool[key].Count > 0)
            {
                PoolableMonoBehaviour delayedItem = delayedEnPool[key].FirstOrDefault();
                delayedEnPool[key].RemoveAt(0);
                return delayedItem;
            }

            return Get<PoolableMonoBehaviour>(key, setupData, initParams);
        }

        Dictionary<string, List<PoolableMonoBehaviour>> delayedEnPool = new Dictionary<string, List<PoolableMonoBehaviour>>();

        //Given a string key and an object, queue it to be placed in this pool when unity hits the LateUpdate part of its game loop
        public virtual void DelayedEnPool(string key, PoolableMonoBehaviour objectToEnPool)
        {
            if(!delayedEnPool.ContainsKey(key))
                delayedEnPool.Add(key, new List<PoolableMonoBehaviour>());
            delayedEnPool[key].Add(objectToEnPool);
        } 

        //process the queue of enpools (if any) on late update
        public virtual void FlushDelayedEnPools()
        {
            if(delayedEnPool.Count <= 0)
                return;

            foreach(var pool in delayedEnPool)
            {
                if(pool.Value.Count > 0)
                {
                    pool.Value.ForEach(x => this[pool.Key].EnPool(x));
                    pool.Value.Clear();
                }
            }

            delayedEnPool.Clear();
        }

        protected virtual void LateUpdate()
        {
            FlushDelayedEnPools();
        }

        //Place an object into its pool
        public virtual void EnPool(string key, PoolableMonoBehaviour objectToEnPool)
        {
            this[key].EnPool(objectToEnPool);
        }

        //creates a MonoBehaviourPool
        public virtual MonoBehaviourPool<object, PoolableMonoBehaviour> Create(string key, PoolableMonoBehaviour prefab)
        {
            this[key] = new MonoBehaviourPool<object, PoolableMonoBehaviour>() { Prefab = prefab };
            return this[key];
        }

        protected virtual void SetupSymbolName(IObjectPool<object, PoolableMonoBehaviour> pool, object setupData, PoolableMonoBehaviour obj, params object[] initParams)
        {
            obj.name = setupData.ToString() + " " + obj.name;
        }

        protected virtual void SetupPools()
        {
            if(objectMappingValues == null || objectMappingValues.Count <= 0)
                return;

            for(int i = 0; i < objectMappingValues.Count; ++i)
            {
                if(Application.isEditor && !Application.isPlaying)
                {
                    if(objectMappingValues[i] == null)
                        continue;
                }

                if(objectMappingValues[i] == null)
                    throw new System.NullReferenceException(name + " object pool type at index " +i+ " is null.");

                Map[objectMappingKeys[i]] = new MonoBehaviourPool<object, PoolableMonoBehaviour>() { Prefab = objectMappingValues[i] };
            }
        }

        public virtual void UnloadPools()
        {
            foreach(var s in Map)
            {
                if(s.Value != null)
                    s.Value.UnloadAll();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            UnloadPools();
        }

        protected virtual void Awake()
        {
            SetupPools();
        }

        protected virtual void OnEnable()
        {
            SetupPools();
        }

        protected virtual void OnDestroy()
        {
            UnloadPools();
        }

        public static MonoBehaviourFactory Create(string name)
        {
            GameObject mbfact = new GameObject(name);
            mbfact.SetActive(false);
            return mbfact.AddComponent<MonoBehaviourFactory>();
            //CreateScriptableObject<TMonoBehaviourFactory>(Application.dataPath, allowCreateAssetInPlayMode: false);
        }
    }

    public class MonoBehaviourFactory<TMonoBehaviourFactory> : MonoBehaviourFactory
        where TMonoBehaviourFactory : MonoBehaviourFactory
    {
        new public static TMonoBehaviourFactory Create(string name)
        {
            GameObject mbfact = new GameObject(name);
            return mbfact.AddComponent<TMonoBehaviourFactory>();
                //CreateScriptableObject<TMonoBehaviourFactory>(Application.dataPath, allowCreateAssetInPlayMode: false);
        }
    }

    public interface IMonoBehaviourFactory
    {
        TPoolableMonoBehaviour Get<TPoolableMonoBehaviour>(string key, object setupData, params object[] initParams)
            where TPoolableMonoBehaviour : PoolableMonoBehaviour;

        PoolableMonoBehaviour Get(string key, object setupData, params object[] initParams);

        void EnPool(string key, PoolableMonoBehaviour objectToEnPool);
    }
}
