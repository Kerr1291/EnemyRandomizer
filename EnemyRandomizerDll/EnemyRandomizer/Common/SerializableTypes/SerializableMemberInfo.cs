using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace nv
{
    [System.Serializable]
    public class SerializableMemberInfo
    {
        public MemberInfo Info
        {
            get
            {
                //if the data is lost (which it will be after every time unity rebuilds), rebuild it
                if(info == null)
                {
                    if(string.IsNullOrEmpty(typeAssemblyName))
                    {
                        return null;
                    }

                    Assembly typeAssembly = Assembly.Load(typeAssemblyName);
                    System.Type targetType = typeAssembly.GetType(typeName);

                    if(targetType == null)
                    {
                        Debug.LogWarning("Did not find type " + typeName);
                        return null;
                    }

                    if(string.IsNullOrEmpty(memberName) || !targetType.GetAllMembers().Select(x => x.Name).Contains(memberName))
                    {
                        return null;
                    }
                    
                    info = targetType.GetFieldInfo(memberName);

                    if(info == null)
                    {
                        info = targetType.GetPropertyInfo(memberName);
                    }
                    if(info == null && methodParameters != null)
                    {
                        info = targetType.GetMethod(memberName, methodParameters.Select(x => x.StoredType).Where(x => x != null).ToArray());
                    }
                }

                return info;
            }
            set
            {
                info = value;
                if(info != null)
                {
                    typeName = info.ReflectedType.FullName;
                    typeAssemblyName = Assembly.GetAssembly(info.ReflectedType).FullName;
                    memberName = info.Name;

                    if(info as MethodInfo != null)
                    { 
                        methodParameters = (info as MethodInfo).GetParameters().Select(x => new SerializableType(x.ParameterType)).ToArray();
                    }
                    else
                    {
                        methodParameters = null;
                    }
                }
            }
        }

        public object GetValue(object instance)
        {
            MemberInfo minfo = Info;
            var fi = minfo as FieldInfo;
            if(fi != null)
            {
                object targetValue = fi.GetValue(instance);
                return targetValue;
            }
            var pi = minfo as PropertyInfo;
            if(pi != null)
            {
#if NET_4_6
                object targetValue = pi.GetValue(instance);
#else
                object targetValue = pi.GetValue(instance, null);
#endif
                return targetValue;
            }
            var mi = minfo as MethodInfo;
            if(mi != null)
            {
                //TODO: wrap in a try/catch in edit mode so we don't get spammed with errors
                object targetValue = mi.Invoke(instance, null);
                return targetValue;
            }
            return null;
        }

        public T GetValue<T>(object instance)
        {
            MemberInfo minfo = Info;
            var fi = minfo as FieldInfo;
            if(fi != null)
            {
                T targetValue = (T)fi.GetValue(instance);
                return targetValue;
            }
            var pi = minfo as PropertyInfo;
            if(pi != null)
            {
#if NET_4_6
                T targetValue = (T)pi.GetValue(instance);
#else
                T targetValue = (T)pi.GetValue(instance, null);
#endif
                return targetValue;
            }
            var mi = minfo as MethodInfo;
            if(mi != null)
            {
                T targetValue = (T)mi.Invoke(instance, null);
                return targetValue;
            }

            return default(T);
        }

        public void SetValue(object instance, object value)
        {
            MemberInfo minfo = Info;
            var fi = minfo as FieldInfo;
            if(fi != null)
            {
                fi.SetValue(instance, value);
            }
            var pi = minfo as PropertyInfo;
            if(pi != null)
            {
#if NET_4_6
                pi.SetValue(instance, value);
#else
                pi.SetValue(instance, value, null);
#endif
            }
            var mi = minfo as MethodInfo;
            if(mi != null)
            {
                mi.Invoke(instance, new object[] { value });
            }
        }

        MemberInfo info;

        [SerializeField]
        string typeAssemblyName;

        [SerializeField]
        string typeName;

        [SerializeField]
        string memberName;
        
        [SerializeField]
        SerializableType[] methodParameters;
    }
}