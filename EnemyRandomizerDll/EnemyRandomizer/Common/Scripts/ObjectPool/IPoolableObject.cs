namespace nv
{
    public interface IPoolableObject
    {
        /// <summary>
        /// The data used to configure this object.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Defines how the object should be managed when returned to a pool.
        /// </summary>
        /// <returns>The object to be placed into a pool. If null is returned, the object is unloaded.</returns>
        object GetObjectToEnPool();

        /// <summary>
        /// Invoked after an object is returned to a pool.
        /// </summary>
        void OnEnPool();

        /// <summary>
        /// Invoked after a pool activates an object.
        /// </summary>
        void OnActivate(object data);

        /// <summary>
        /// Invoked after an object is first created.
        /// </summary>
        void OnCreate(object data);

        /// <summary>
        /// Invoked after an object is removed from a pool entirely. 
        /// </summary>
        void OnUnload();
    }

    /// <summary>
    /// This interface is for use with the ObjectPool type.
    /// </summary>
    public interface IPoolableObject<TObjectData> : IPoolableObject
    {
        /// <summary>
        /// The data used to configure this object.
        /// </summary>
        new TObjectData Data { get; }

        /// <summary>
        /// Invoked after a pool activates an object.
        /// </summary>
        void OnActivate(TObjectData data);

        /// <summary>
        /// Invoked after an object is first created.
        /// </summary>
        void OnCreate(TObjectData data);
    }
}