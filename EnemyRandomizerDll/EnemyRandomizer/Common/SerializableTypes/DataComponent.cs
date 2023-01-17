using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

namespace nv
{
    /// <summary>
    /// This event publishes the data and the type of data.
    /// This is used to allow the event to register in the inspector of the generic classes below.
    /// </summary>
    [Serializable] public class OnSetDataEvent : UnityEvent<Type, object> { }
    
    /// <summary>
    /// A class to wrap a unity component/inspector view around a non-monobehaviour type.
    /// This type assumes that the data will be initialized at runtime.
    /// </summary>
    public class DataComponent<TSetupData, TComponentData> : MonoBehaviour
        where TSetupData : class
        where TComponentData : class
    {
        public OnSetDataEvent onSetData;

        [SerializeField]
        protected TypeFactory factory = new TypeFactory<IFactory>();        
        public virtual TypeFactory Factory
        {
            get
            {
                return factory;
            }
        }

        public virtual TComponentData Data { get; protected set; }

        public virtual void SetData(TSetupData setupData)
        {
            if(setupData == null)
            {
                Data = null;
            }
            else
            {
                object data = Data;
                Factory.Create(ref data, setupData);
                Data = (TComponentData)data;
            }

            if(onSetData != null)
                onSetData.Invoke(typeof(TComponentData), Data);
        }
    }

    /// <summary>
    /// A simple class to wrap a unity component/inspector view around a non-monobehavior type.
    /// </summary>
    public class SerializedDataComponent<TComponentData> : MonoBehaviour
    {
        public OnSetDataEvent onSetData;

        [SerializeField]
        protected TComponentData data;
        public virtual TComponentData Data
        {
            get
            {
                return data;
            }
            protected set
            {
                data = value;

                if(onSetData != null)
                    onSetData.Invoke(typeof(TComponentData), Data);
            }
        }
    }
}