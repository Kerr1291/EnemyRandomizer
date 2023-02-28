using System.Collections.Generic;

namespace nv
{
    public delegate void ObjectPoolDelegate<TObjectData, TObjectType>(IObjectPool<TObjectData, TObjectType> thisPool, TObjectData objectBeingActedUpon, TObjectType objectData, params object[] actionParams)
                where TObjectType : class, IPoolableObject;

    public interface IObjectPool<TObjectData, TObjectType> : ICollection<TObjectType>
                where TObjectType : class, IPoolableObject
    {
        ObjectPoolDelegate<TObjectData, TObjectType> OnCreateObject { get; set; }
        ObjectPoolDelegate<TObjectData, TObjectType> OnUnloadObject { get; set; }
        ObjectPoolDelegate<TObjectData, TObjectType> OnEnPoolObject { get; set; }
        ObjectPoolDelegate<TObjectData, TObjectType> OnDePoolObject { get; set; }
        
        ICollection<TObjectType> ActiveObjects { get; }

        /// <summary>
        /// Get an object from the pool. Init it with the given data.
        /// </summary>
        TPoolableType Get<TPoolableType>(TObjectData setupData, params object[] initParams)
            where TPoolableType : TObjectType;

        /// <summary>
        /// Get an object from the pool. Init it with the given data.
        /// </summary>
        TObjectType Get(System.Type poolableObjectType, TObjectData setupData, params object[] initParams);

        /// <summary>
        /// Get objects from the pool. Init them with the given data.
        /// </summary>
        IList<TPoolableType> Get<TPoolableType>(IList<TObjectData> setupData, params object[] initParams)
            where TPoolableType : TObjectType;

        /// <summary>
        /// Get objects from the pool. Init them with the given data.
        /// </summary>
        IList<TObjectType> Get(System.Type poolableObjectType, IList<TObjectData> setupData, params object[] initParams);
        
        /// <summary>
        /// Get objects from the pool. Init them with the given data.
        /// </summary>
        void Get<TPoolableType>(ref IList<TPoolableType> objects, IList<TObjectData> setupData, params object[] initParams)
            where TPoolableType : TObjectType;

        /// <summary>
        /// Get objects from the pool. Init them with the given data.
        /// </summary>
        void Get(ref IList<TObjectType> objects, System.Type poolableObjectType, IList<TObjectData> setupData, params object[] initParams);

        /// <summary>
        /// Place an object into the pool or unload it depending on the item's pooling strategy.
        /// </summary>
        void EnPool(TObjectType activeObject);

        /// <summary>
        /// Place objects into the pool or unload them depending on the their pooling strategy.
        /// </summary>
        void EnPool(IList<TObjectType> activeObjects);

        /// <summary>
        /// Place all active objects in the pool.
        /// </summary>
        void EnPoolAll();

        /// <summary>
        /// Unload this object and remove it from the pool.
        /// </summary>
        void Unload(TObjectType poolObject);

        /// <summary>
        /// Unload objects and remove them from the pool.
        /// </summary>
        void Unload(IList<TObjectType> poolObjects);

        /// <summary>
        /// Unload objects and remove them from the pool.
        /// </summary>
        /// <param name="includeActiveObjects">Should this unload "active" objects or only excess objects that reside in the pool.</param>
        void UnloadAll(bool includeActiveObjects);
    }
}
