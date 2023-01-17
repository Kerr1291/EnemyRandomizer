using UnityEngine;
using System;

namespace nv
{
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolableObject, IComparable
    {
        public virtual object Data
        {
            get; protected set;
        }
        
        public virtual object GetObjectToEnPool()
        {
            return this;
        }

        public virtual void OnEnPool()
        {
            Data = null;
            gameObject.SetActive(false);
        }

        public virtual void OnActivate(object data)
        {
            Setup(data);
        }

        public virtual void OnCreate(object data)
        {
            Setup(data);
        }

        public virtual void OnUnload()
        {
            Data = null;
            if(gameObject == null)
                return;
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
                GameObject.DestroyImmediate(gameObject);
            else
#endif
                GameObject.Destroy(gameObject);
        }

        public virtual int CompareTo(object obj)
        {
            if(obj == null)
                return 1;

            return ToString().CompareTo(obj.ToString());
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        protected virtual void Setup(object data)
        {
            gameObject.SetActive(true);
            Data = data;
        }
    }

    public class PoolableMonoBehaviour<TData> : PoolableMonoBehaviour, IPoolableObject<TData>, IComparable<PoolableMonoBehaviour<TData>>
        where TData : IComparable<TData>
    {
        public new virtual TData Data
        {
            get
            {
                return (TData)base.Data;
            }
            set
            {
                base.Data = value;
            }
        }

        public override void OnActivate(object data)
        {
            OnActivate((TData)data);
        }

        public override void OnCreate(object data)
        {
            OnCreate((TData)data);
        }

        protected override void Setup(object data)
        {
            Setup((TData)data);
        }

        public virtual void OnActivate(TData data)
        {
            Setup(data);
        }

        public virtual void OnCreate(TData data)
        {
            Setup(data);
        }

        protected virtual void Setup(TData data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public override int CompareTo(object obj)
        {
            if(obj == null)
                return 1;

            if(Data == null)
                return -1;

            if(obj is PoolableMonoBehaviour<TData>)
                return CompareTo((PoolableMonoBehaviour<TData>)obj);
            else
                return base.CompareTo(obj);
        }

        public virtual int CompareTo(PoolableMonoBehaviour<TData> other)
        {
            if(other == null)
                return 1;

            if(Data == null)
                return -1;

            return Data.CompareTo(other.Data);
        }
    }
}