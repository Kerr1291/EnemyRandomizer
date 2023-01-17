using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace nv
{
    [Serializable]
    public class SerializableType
    {
        Type storedType;

        [SerializeField]
        protected string typeAssemblyName;

        [SerializeField]
        protected string typeName;
        
        public SerializableType(Type type = null)
        {
            Serialize(type);
        }

        public SerializableType(string assemblyName, string fullName)
        {
            typeAssemblyName = assemblyName;
            typeName = fullName;
            storedType = Deserialize();
        }

        public Type StoredType
        {
            get
            {
                //if the data is lost (which it will be after every time unity rebuilds), rebuild it
                return storedType ?? (storedType = Deserialize());
            }
            set
            {
                Serialize(value);
            }
        }

        protected void Serialize(Type type)
        {
            storedType = type;
            if(type == null)
            {
                typeName = string.Empty;
                typeAssemblyName = string.Empty;
            }
            else
            {
                typeName = type.FullName;
                typeAssemblyName = Assembly.GetAssembly(type).FullName;
            }
        }

        protected Type Deserialize()
        {
            if(string.IsNullOrEmpty(typeAssemblyName))
                return null;

            try
            {
                Assembly typeAssembly = Assembly.Load(typeAssemblyName);
                return typeAssembly.GetType(typeName);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogWarning("Failed to load assembly and/or get serialized type using type name " + typeName + " in assembly " + typeAssemblyName + " Exception: "+e.Message);
                return null;
            }
        }
    }
}