using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    public class MonoBehaviourPool<TObjectData, TPoolableMonoBehaviour> : ObjectPool<TObjectData, TPoolableMonoBehaviour>
        where TPoolableMonoBehaviour : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// This is used by default when CreateObject is called as the prefab to instantitate.
        /// </summary>
        public virtual TPoolableMonoBehaviour Prefab { get; set; }

        public override TPoolableType Get<TPoolableType>(TObjectData setupData, params object[] initParams)
        {
            if(Prefab == null)
                throw new System.NullReferenceException(typeof(MonoBehaviourPool<TObjectData, TPoolableMonoBehaviour>).Name + " Prefab must not be null when CreateObject is called.");

            return (TPoolableType)Get(Prefab.GetType(), setupData, initParams);
        }

        protected override TPoolableMonoBehaviour GetObject(System.Type poolableType, TObjectData setupData, params object[] initParams)
        {
            TPoolableMonoBehaviour objectToActivate = GetPool(Prefab).Dequeue();

            Transform parent = GetParentTransformFromParams(initParams);

            objectToActivate.transform.SetParent(parent,false);

            return objectToActivate;
        }

        protected override TPoolableMonoBehaviour CreateObject(System.Type poolableType, TObjectData setupData, params object[] initParams)
        {
            Transform parent = GetParentTransformFromParams(initParams);

            TPoolableMonoBehaviour newObject = (TPoolableMonoBehaviour)UnityEngine.MonoBehaviour.Instantiate(Prefab, parent, false);
            newObject.OnCreate(setupData);
            Invoke(OnCreateObject, newObject, setupData);
            return newObject;
        }

        protected virtual Transform GetParentTransformFromParams(params object[] initParams)
        {
            Transform parent = null;

            //get the first transform in the list of params (if present)
            if(initParams.Length > 0)
            {
                parent = initParams.FirstOrDefault(x => x as Transform != null) as Transform;
            }

            return parent;
        }
    }
}