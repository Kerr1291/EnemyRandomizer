using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace EnemyRandomizerMod
{
    public class ObjectPool<TObjectData, TObjectType> : Collection<TObjectType>, IObjectPool<TObjectData, TObjectType>
        where TObjectType : class, IPoolableObject
    {
        public delegate void CollectionNotificationDelegate(IList<TObjectType> thisCollection);
        public delegate void ItemNotificationDelegate(IList<TObjectType> thisCollection, int index, TObjectType item);
        public delegate void ItemsNotificationDelegate(IList<TObjectType> thisCollection, ICollection<TObjectType> itemsToSet);

        public virtual ObjectPoolDelegate<TObjectData, TObjectType> OnCreateObject { get; set; }
        public virtual ObjectPoolDelegate<TObjectData, TObjectType> OnUnloadObject { get; set; }
        public virtual ObjectPoolDelegate<TObjectData, TObjectType> OnEnPoolObject { get; set; }
        public virtual ObjectPoolDelegate<TObjectData, TObjectType> OnDePoolObject { get; set; }

        /// <summary>
        /// This callback may be used to globally register for the creation of any new collection
        /// </summary>
        public static CollectionNotificationDelegate OnNewCollectionCreated { get; set; }

        public virtual CollectionNotificationDelegate OnClearItems { get; set; }
        public virtual ItemNotificationDelegate OnInsertItem { get; set; }
        public virtual ItemNotificationDelegate OnSetItem { get; set; }
        public virtual ItemNotificationDelegate OnRemoveItem { get; set; }
        public virtual ItemsNotificationDelegate OnSetItems { get; set; }

        /// <summary>
        /// This callback may be used to register for any change that occurs
        /// </summary>
        public virtual Action OnChanged { get; set; }

        public static implicit operator List<TObjectType>(ObjectPool<TObjectData, TObjectType> items)
        {
            return items.Items.ToList();
        }

        public static implicit operator TObjectType[] (ObjectPool<TObjectData, TObjectType> items)
        {
            return items.Items.ToArray();
        }

        public virtual ICollection<TObjectType> ActiveObjects
        {
            get
            {
                return Items;
            }
        }

        protected virtual Dictionary<string,Queue<TObjectType>> PooledObjects { get; set; }

        public ObjectPool()
            : base()
        {
            Setup();
        }

        public ObjectPool(IList<TObjectType> items)
            : base(items)
        {
            Setup();
        }

        public ObjectPool(ICollection<TObjectType> items)
            : base(items.ToList())
        {
            Setup();
        }

        public virtual TPoolType Get<TPoolType>(TObjectData setupData, params object[] initParams)
            where TPoolType : TObjectType
        {
            return (TPoolType)Get(typeof(TPoolType), setupData, initParams);
        }

        public virtual TObjectType Get(Type poolableObjectType, TObjectData setupData, params object[] initParams)
        {
            if(!typeof(TObjectType).IsAssignableFrom(poolableObjectType))
                throw new InvalidCastException(poolableObjectType.Name + " is not a subclass of " + typeof(TObjectType).Name);

            //use the ternary operator here to avoid allocating a temp/default item.
            TObjectType objectToActivate = (PooledObjects.ContainsKey(poolableObjectType.Name) && (PooledObjects[poolableObjectType.Name].Count > 0)) ? GetObject(poolableObjectType, setupData, initParams) : CreateObject(poolableObjectType, setupData, initParams);

            Items.Add(objectToActivate);
            objectToActivate.OnActivate(setupData);
            Invoke(OnDePoolObject, objectToActivate, setupData, initParams);
            InvokeOnChanged();
            return objectToActivate;
        }

        public virtual IList<TPoolableType> Get<TPoolableType>(IList<TObjectData> setupData, params object[] initParams)
               where TPoolableType : TObjectType
        {
            IList<TPoolableType> items = new TPoolableType[setupData.Count];

            Get<TPoolableType>(ref items, setupData, initParams);

            return items;
        }

        public virtual IList<TObjectType> Get(Type poolableObjectType, IList<TObjectData> setupData, params object[] initParams)
        {
            IList<TObjectType> items = new TObjectType[setupData.Count];

            Get(ref items, poolableObjectType, setupData, initParams);

            return items;
        }

        public void Get<TPoolableType>(ref IList<TPoolableType> objects, IList<TObjectData> setupData, params object[] initParams)
               where TPoolableType : TObjectType
        {
            int i = 0;
            for(; i < setupData.Count; ++i)
            {
                TPoolableType objectToActivate = Get<TPoolableType>(setupData[i], initParams);

                if(objects.Count <= i)
                    objects.Add(objectToActivate);
                else
                    objects[i] = objectToActivate;
            }

            for(int j = objects.Count - 1; j > i; --j)
            {
                objects.RemoveAt(j);
            }
        }

        public void Get(ref IList<TObjectType> objects, Type poolableObjectType, IList<TObjectData> setupData, params object[] initParams)
        {
            int i = 0;
            for(; i < setupData.Count; ++i)
            {
                TObjectType objectToActivate = Get(poolableObjectType, setupData[i], initParams);

                if(objects.Count <= i)
                    objects.Add(objectToActivate);
                else
                    objects[i] = objectToActivate;
            }

            for(int j = objects.Count - 1; j > i; --j)
            {
                objects.RemoveAt(j);
            }
        }

        public virtual void EnPool(TObjectType activeObject)
        {
            //UnityEngine.Debug.Log("Enpool invoked for object " + activeObject);
            //UnityEngine.Debug.Log("Before Active objects = " + Items.Count);
            Items.Remove(activeObject);

            //the user's pooling strategy might request that the object be destroyed
            TObjectType poolResult = activeObject.GetObjectToEnPool() as TObjectType;
            if(poolResult != null)
            {
                //UnityEngine.Debug.Log("Pooling object " + poolResult);
                Invoke(OnEnPoolObject, poolResult);
                activeObject.OnEnPool();
                EnPoolObject(poolResult);
            }
            else
            {
                //UnityEngine.Debug.Log("Unloading object " + activeObject);
                Invoke(OnUnloadObject, activeObject);
                activeObject.OnUnload();
            }
            //UnityEngine.Debug.Log("After Active objects = " + Items.Count);
            InvokeOnChanged();
        }
        
        public virtual void EnPool(IList<TObjectType> setupData)
        {
            for(int i = setupData.Count - 1; i >= 0; --i)
            {
                EnPool(setupData[i]);
            }
        }
        
        public virtual void EnPoolAll()
        {
            for(int i = Items.Count - 1; i >= 0; --i)
            {
                EnPool(Items[i]);
            }
        }
        
        public virtual void Unload(TObjectType poolObject)
        {
            Items.Remove(poolObject);
            Invoke(OnUnloadObject, poolObject);
            if(poolObject != null)
                poolObject.OnUnload();
        }

        public virtual void Unload(IList<TObjectType> poolObject)
        {
            for(int i = poolObject.Count - 1; i >= 0; --i)
            {
                Unload(poolObject[i]);
            }
        }

        public virtual void UnloadAll(bool includeActiveObjects = false)
        {
            if(includeActiveObjects)
                Clear();

            foreach(var poolType in PooledObjects)
            {
                foreach(TObjectType poolObject in poolType.Value)
                {
                    if(poolObject == null)
                        continue;
                    Unload(poolObject);
                }
            }
            PooledObjects.Clear();
        }

        /// <summary>
        /// The behavior of the ctor. This allows deriving classes to override the ctor behavior.
        /// </summary>
        protected virtual void Setup()
        {
            PooledObjects = new Dictionary<string, Queue<TObjectType>>();
            InvokeOnNewCollectionCreated();
        }

        protected virtual void EnPoolObject(TObjectType objectToEnPool)
        {
            Queue<TObjectType> pool = GetPool(objectToEnPool);

            if(GetPool(objectToEnPool) == null)
                pool = AddPool(objectToEnPool);

            pool.Enqueue(objectToEnPool);
        }

        protected virtual TPoolableType GetObject<TPoolableType>(TObjectData setupData, params object[] initParams)
                    where TPoolableType : TObjectType
        {
            return (TPoolableType)GetObject(typeof(TPoolableType), setupData, initParams);
        }

        protected virtual TObjectType GetObject(Type poolableType, TObjectData setupData, params object[] initParams)
        {
            return GetPool(poolableType).Dequeue();
        }

        /// <summary>
        /// Invoke the ctor on the type that takes the setup data type.
        /// </summary>
        protected virtual TPoolableType CreateObject<TPoolableType>(TObjectData setupData, params object[] initParams)
                    where TPoolableType : TObjectType
        {
            return (TPoolableType)CreateObject(typeof(TPoolableType), setupData, initParams);
        }

        protected virtual TObjectType CreateObject(Type poolableType, TObjectData setupData, params object[] initParams)
        {
            TObjectType newObject = (TObjectType)Activator.CreateInstance(poolableType);
            newObject.OnCreate(setupData);
            Invoke(OnCreateObject, newObject, setupData, initParams);
            return newObject;
        }

        protected virtual Queue<TObjectType> AddPool(TObjectType poolableObject)
        {
            return AddPool(poolableObject.GetType());
        }

        protected virtual Queue<TObjectType> AddPool<TPoolType>()
            where TPoolType : TObjectType
        {
            return AddPool(typeof(TPoolType));
        }

        protected virtual Queue<TObjectType> AddPool(Type poolType)
        {
            PooledObjects.Add(poolType.Name, new Queue<TObjectType>());
            return GetPool(poolType);
        }

        protected virtual Queue<TObjectType> GetPool(TObjectType poolableObject)
        {
            return GetPool(poolableObject.GetType());
        }

        protected virtual Queue<TObjectType> GetPool<TPoolType>()
            where TPoolType : TObjectType
        {
            return GetPool(typeof(TPoolType));
        }

        protected virtual Queue<TObjectType> GetPool(Type poolType)
        {
            return PooledObjects.ContainsKey(poolType.Name) ? PooledObjects[poolType.Name] : null;
        }

        /// <summary>
        /// The behavior of the Clear() method. EnPool all active items and then clear the active list.
        /// </summary>
        protected override void ClearItems()
        {
            EnPoolAll();
            if(OnClearItems != null)
                OnClearItems.Invoke(this);
            InvokeOnChanged();
        }

        protected virtual void Invoke(ObjectPoolDelegate<TObjectData, TObjectType> callback, TObjectType item)
        {
            if(callback != null)
                callback.Invoke(this, (TObjectData)item.Data, item);
        }

        protected virtual void Invoke(ObjectPoolDelegate<TObjectData, TObjectType> callback, TObjectType item, TObjectData setupData, params object[] initParams)
        {
            if(callback != null)
                callback.Invoke(this, setupData, item, initParams);
        }

        public virtual void SetItems(IEnumerable<TObjectType> items)
        {
            Items.Clear();
            if(Items is List<TObjectType>)
            {
                (Items as List<TObjectType>).AddRange(items);
            }
            else
            {
                foreach(var item in items)
                {
                    Items.Add(item);
                }
            }
            if(OnSetItems != null)
                OnSetItems.Invoke(this, Items);
            InvokeOnChanged();
        }

        public virtual void SetItems<U>(U items)
            where U : ICollection<TObjectType>
        {
            Items.Clear();
            if(Items is List<TObjectType>)
            {
                (Items as List<TObjectType>).AddRange(items);
            }
            else
            {
                foreach(var item in items)
                {
                    Items.Add(item);
                }
            }
            if(OnSetItems != null)
                OnSetItems.Invoke(this, Items);
            InvokeOnChanged();
        }

        protected override void InsertItem(int index, TObjectType item)
        {
            base.InsertItem(index, item);
            if(OnInsertItem != null)
                OnInsertItem.Invoke(this, index, item);
            InvokeOnChanged();
        }

        protected override void SetItem(int index, TObjectType item)
        {
            base.SetItem(index, item);
            if(OnSetItem != null)
                OnSetItem.Invoke(this, index, item);
            InvokeOnChanged();
        }

        protected override void RemoveItem(int index)
        {
            TObjectType itemThatWasRemoved = Items[index];
            base.RemoveItem(index);
            if(OnRemoveItem != null)
                OnRemoveItem.Invoke(this, index, itemThatWasRemoved);
            InvokeOnChanged();
        }

        protected virtual void InvokeOnNewCollectionCreated()
        {
            if(OnNewCollectionCreated != null)
                OnNewCollectionCreated.Invoke(this);
        }

        protected virtual void InvokeOnChanged()
        {
            if(OnChanged != null)
                OnChanged.Invoke();
        }
    }
}