using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

namespace nv
{
    public interface IFactory
    {
        void Create(ref object target, object setupData);
    }

    public interface IFactory<TCreationType, TSetupData> : IFactory
    {
        void Create(ref TCreationType target, TSetupData setupData);
    }

    public abstract class Factory<TCreationType, TSetupData> : IFactory<TCreationType, TSetupData>
    {
        public abstract void Create(ref TCreationType target, TSetupData setupData);

        public virtual void Create(ref object target, object setupData)
        {
            TCreationType castedTarget = (TCreationType)target;
            Create(ref castedTarget, (TSetupData)setupData);
            target = castedTarget;
        }
    }

    [Serializable] public class SerializedFactory : TypeFactory<IFactory> { }

    public static class IFactoryExtensions
    {
        public static TMember CreateAndPublish<TMember>(this IFactory factory, object setupData, UnityEvent<TMember> publishEvent = null)
        {
            TMember member = default(TMember);
            object memberAsObjectRef = null;
            factory.Create(ref memberAsObjectRef, setupData);
            member = (TMember)memberAsObjectRef;

            if(publishEvent != null)
                publishEvent.Invoke(member);

            return member;
        }
    }
}