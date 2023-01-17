using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
namespace nv.Reels.editor
{
    [CustomPropertyDrawer(typeof(TypeFactory), useForChildren: true)]
    public class TypeFactoryDrawer : PropertyDrawer
    {
        List<Type> baseTypes = new List<Type>();
        List<Type> iTypes = new List<Type>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var parent = GetPropertyObject(property.propertyPath, property.serializedObject.targetObject);
            TypeFactory _target = parent as TypeFactory;

            if(_target.RequiredBaseType == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            DrawTypeList(_target, ref iTypes, property.propertyPath + ":");
        }

        public void DrawTypeList(TypeFactory targetRef, ref List<Type> iTypes, string label)
        {
            int currentIType = 0;

            if(iTypes.Count <= 0)
                iTypes = GetSubclassTypes(targetRef.RequiredBaseType).ToList();

            if(targetRef.FactoryType != null)
            {
                currentIType = string.IsNullOrEmpty(targetRef.FactoryType.FullName)
                    ? 0 : iTypes.Select(x => x.FullName).ToList().IndexOf(targetRef.FactoryType.FullName);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);
            currentIType = EditorGUILayout.Popup(currentIType, iTypes.Select(x => x.Name).ToArray());
            EditorGUILayout.EndHorizontal();

            if(iTypes.Count >= 0)
            {
                Type iType = iTypes[currentIType];

                targetRef.FactoryType = iType;
            }
        }

        public static object GetPropertyObject(string path, object obj)
        {
            var fields = path.Split('.');

            FieldInfo info = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if(info == null)
            {
                PropertyInfo pinfo = obj.GetType().GetProperty(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                obj = pinfo.GetValue(obj, null);
            }
            else
            {
                obj = info.GetValue(obj);
            }

            return obj;
        }

        //public static object GetParentObject(string path, object obj)
        //{
        //    var fields = path.Split('.');

        //    if(fields.Length == 1)
        //        return obj;

        //    FieldInfo info = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    obj = info.GetValue(obj);

        //    return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        //}

        protected virtual List<Type> GetSubclassTypes(Type baseClassType)
        {
            //get all types
            var preTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

            //filter out compiler generated types and linq types
            preTypes = preTypes.Where(x => (!x.Name.StartsWith("<") && (string.IsNullOrEmpty(x.Namespace) || !x.Namespace.Contains("Linq"))));

            //filter out non generic based types
            List<Type> types = preTypes.Where(y => baseClassType.IsAssignableFrom(y) && !y.IsInterface && !y.IsAbstract).ToList();

            return types;
        }
    }
}
#endif

namespace nv
{
    [Serializable]
    public class TypeFactory : IFactory
    {
        public TypeFactory(Type factoryType = null, Type baseTypeConstraint = null)
        {
            RequiredBaseType = baseTypeConstraint;
            FactoryType = factoryType;
        }

        [SerializeField]
        protected SerializableType requiredBaseType;

        [SerializeField]
        protected SerializableType factoryType;

        public virtual Type FactoryType
        {
            get
            {
                return factoryType == null ? null : factoryType.StoredType;
            }
            set
            {
                factoryType.StoredType = value;
                AssertCheckRequiredType(FactoryType);
            }
        }

        public virtual Type RequiredBaseType
        {
            get
            {
                return requiredBaseType == null ? null : requiredBaseType.StoredType;
            }
            set
            {
                requiredBaseType.StoredType = value;
            }
        }

        public virtual bool CanStoreType(Type potentialType)
        {
            if(RequiredBaseType != null && !RequiredBaseType.IsAssignableFrom(potentialType))
                return false;
            return true;
        }

        public virtual void AssertCheckRequiredType(Type wrongType)
        {
            if(!CanStoreType(wrongType))
                throw new InvalidCastException("This type factory requires stored types to derive from " + RequiredBaseType);
        }

        /// <summary>
        /// Override this in your derived type to change how instantiation of a new type is performed.
        /// </summary>
        public virtual object Create(params object[] args)
        {
            return Activator.CreateInstance(FactoryType, args);
        }

        public virtual TBaseType Create<TBaseType>(params object[] args)
        {
            AssertCheckRequiredType(typeof(TBaseType));
            return (TBaseType)Create(args);
        }

        public virtual void Create(ref object target, object args)
        {
            target = Create(args);
        }

        public virtual void Create<TBaseType>(ref TBaseType target, object args)
        {
            target = Create<TBaseType>(args);
        }
    }

    [Serializable]
    public class TypeFactory<TBaseType> : TypeFactory
    {
        public TypeFactory(Type factoryType = null)
            : base(factoryType, typeof(TBaseType)) { }
    }

    [Serializable]
    public class TypeFactory<TBaseType, TFactoryType> : TypeFactory<TBaseType>
    {
        public TypeFactory()
            : base(typeof(TFactoryType)) { }
    }
}