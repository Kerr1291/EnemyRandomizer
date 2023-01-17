using System.Linq;
using System;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine;
using System.Linq.Expressions;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
//TODO: finish later.... eventually want to create an attribute that allows you to serialize delegates using the extension code below as helpers
//#if UNITY_EDITOR
//using UnityEditor;
//namespace nv.editor
//{

//    [CustomPropertyDrawer(typeof(SerializeDelegate))]
//    public class SerializeDelegateAttributeDrawer : PropertyDrawer
//    {
//        private FieldInfo fieldInfo = null;

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            UnityEngine.Object target = property.serializedObject.targetObject;

//            // Find the property field using reflection, in order to get access to its getter/setter.
//            if(fieldInfo == null)
//                fieldInfo = target.GetType().GetField(((SerializeDelegate)attribute).FieldName,
//                                                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

//            if(fieldInfo != null)
//            {
//                // Retrieve the value using the property getter:
//                object value = fieldInfo.GetValue(target);

//                // Draw the property, checking for changes:
//                EditorGUI.BeginChangeCheck();
//                value = DrawProperty(position, property.propertyType, fieldInfo.PropertyType, value, label);

//                // If any changes were detected, call the property setter:
//                if(EditorGUI.EndChangeCheck() && fieldInfo != null)
//                {

//                    // Record object state for undo:
//                    Undo.RecordObject(target, "Inspector");

//                    // Call property setter:
//                    fieldInfo.SetValue(target, value, null);
//                }
//            }
//            else
//            {
//                EditorGUI.LabelField(position, "Error: could not retrieve property.");
//            }
//        }
//    }
//}
//#endif
//namespace nv
//{
//    public delegate void RenderSymbolDelegate(ReelRenderer reelRenderer, PoolableMonoBehaviour symbol, int index);

//    /// <summary>
//    /// Allow the user to define this method in the editor, but don't use it at runtime due to the slow speed compared to a normal delegate.
//    /// </summary>
//    [SerializeField]
//    private RenderSymbolEvent renderSymbolBehavior;
//    protected RenderSymbolDelegate renderSymbol = null;
//    public virtual RenderSymbolDelegate RenderSymbol
//    {
//        get
//        {
//            //this doesn't serialize, so the getter should return a new list of delegates if called in the editor
//            if(renderSymbol == null && renderSymbolBehavior != null && renderSymbolBehavior.GetPersistentEventCount() > 0)
//                renderSymbol = renderSymbolBehavior.ToDelegate<RenderSymbolDelegate>();
//            return renderSymbol;
//        }
//        set
//        {
//            if(Application.isPlaying)
//            {
//                renderSymbol = value;
//            }
//            else
//            {
//                foreach(var d in value.GetInvocationList())
//                {
//                    renderSymbolBehavior.AddPersistentListener<ReelRenderer>(d.Target as UnityEngine.Object, d.Method.Name);
//                }
//            }
//        }
//    }

//    [System.AttributeUsage(System.AttributeTargets.Field)]
//    public class SerializeDelegate : PropertyAttribute
//    {
//        public string FieldName { get; private set; }
//        public SerializeDelegate(string fieldName, Type delegateType, Type unityEventType)
//        {
//            this.FieldName = fieldName;
//        }
//    }
//}

namespace nv
{
    public static class UnityEventExtensions
    {
        static BindingFlags unityEventBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener(this UnityEventBase unityEvent, Action delegateMethod)
        {
            foreach(var e in delegateMethod.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.AddPersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0>(this UnityEventBase unityEvent, Action<T0> delegateMethod)
        {
            foreach(var e in delegateMethod.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.AddPersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0, T1>(this UnityEventBase unityEvent, Action<T0, T1> delegateMethod)
        {
            foreach(var e in delegateMethod.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.AddPersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0, T1, T2>(this UnityEventBase unityEvent, Action<T0, T1, T2> delegateMethod)
        {
            foreach(var e in delegateMethod.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.AddPersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0, T1, T2, T3>(this UnityEventBase unityEvent, Action<T0, T1, T2, T3> delegateMethod)
        {
            foreach(var e in delegateMethod.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.AddPersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener(this UnityEventBase unityEvent, UnityEngine.Object target, Action method)
        {
            unityEvent.AddPersistentListener(target, method.Method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0> method)
        {
            unityEvent.AddPersistentListener(target, method.Method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0,T1>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0,T1> method)
        {
            unityEvent.AddPersistentListener(target, method.Method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0,T1,T2>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0,T1,T2> method)
        {
            unityEvent.AddPersistentListener(target, method.Method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<T0, T1, T2, T3>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0, T1, T2, T3> method)
        {
            unityEvent.AddPersistentListener(target, method.Method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts. This version assumes the target type is the type that contains the method.
        /// </summary>
        public static void AddPersistentListener(this UnityEventBase unityEvent, UnityEngine.Object target, string method)
        {
            unityEvent.AddPersistentListener(target.GetType(), target, method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener<TTargetType>(this UnityEventBase unityEvent, UnityEngine.Object target, string method)
        {
            unityEvent.AddPersistentListener(typeof(TTargetType), target, method);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener(this UnityEventBase unityEvent, Type targetType, UnityEngine.Object target, string method)
        {
            MethodInfo targetMethod = targetType.GetMethod(method, unityEventBindingFlags);
            unityEvent.AddPersistentListener(target, targetMethod);
        }

        /// <summary>
        /// Add this listener to the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void AddPersistentListener(this UnityEventBase unityEvent, UnityEngine.Object target, MethodInfo method)
        {
            if(unityEvent == null)
                Debug.Log("unityEvent is null!");

            if(target == null)
                Debug.Log("target is null!");

            if(method == null)
                Debug.Log("method info is null!");

            unityEvent.RemovePersistentListener(target, method);

            int newEventIndex = unityEvent.GetPersistentEventCount();

#if UNITY_EDITOR
            MethodInfo addPersistentListener = typeof(UnityEventBase).GetMethod("AddPersistentListener", BindingFlags.NonPublic | BindingFlags.Instance);
            addPersistentListener.Invoke(unityEvent, null);
            
            MethodInfo registerListenerIntoInspector = typeof(UnityEventBase).GetMethod("RegisterPersistentListener", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(int), typeof(UnityEngine.Object), typeof(MethodInfo) }, null);
            registerListenerIntoInspector.Invoke(unityEvent, new object[] { newEventIndex, target, method });
#else
            MethodInfo addListener = typeof(UnityEventBase).GetMethod("AddListener", BindingFlags.NonPublic | BindingFlags.Instance);
            addListener.Invoke(unityEvent, new object[] { target, method });
#endif
        }


        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener( this UnityEventBase unityEvent, Action method )
        {
            foreach( var e in method.GetInvocationList() )
            {
                if( e.Target is UnityEngine.Object )
                    unityEvent.RemovePersistentListener( e.Target as UnityEngine.Object, e.Method );
            }
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0>( this UnityEventBase unityEvent, Action<T0> method )
        {
            foreach( var e in method.GetInvocationList() )
            {
                if( e.Target is UnityEngine.Object )
                    unityEvent.RemovePersistentListener( e.Target as UnityEngine.Object, e.Method );
            }
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0, T1>(this UnityEventBase unityEvent, Action<T0, T1> method)
        {
            foreach(var e in method.GetInvocationList())
            {
                if(e.Target is UnityEngine.Object)
                    unityEvent.RemovePersistentListener(e.Target as UnityEngine.Object, e.Method);
            }
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener(this UnityEventBase unityEvent, UnityEngine.Object target, Action method)
        {
            unityEvent.RemovePersistentListener(target, method.Method);
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0> method)
        {
            unityEvent.RemovePersistentListener(target, method.Method);
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0, T1>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0, T1> method)
        {
            unityEvent.RemovePersistentListener(target, method.Method);
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0, T1, T2>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0, T1, T2> method)
        {
            unityEvent.RemovePersistentListener(target, method.Method);
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener<T0, T1, T2, T3>(this UnityEventBase unityEvent, UnityEngine.Object target, Action<T0, T1, T2, T3> method)
        {
            unityEvent.RemovePersistentListener(target, method.Method);
        }

        /// <summary>
        /// Remove this listener from the serialized list of listeners on this unity event. This is primarily for use in editor scripts.
        /// </summary>
        public static void RemovePersistentListener(this UnityEventBase unityEvent, UnityEngine.Object target, MethodInfo method)
        {
#if UNITY_EDITOR
            MethodInfo removePersistentListener = typeof(UnityEventBase).GetMethod("RemovePersistentListener", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(UnityEngine.Object), typeof(MethodInfo) }, null);
            removePersistentListener.Invoke(unityEvent, new object[] { target, method });
#else
            MethodInfo removeListener = typeof(UnityEventBase).GetMethod("RemoveListener", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(UnityEngine.Object), typeof(MethodInfo) }, null);
            removeListener.Invoke(unityEvent, new object[] { target, method });
#endif
        }

        /// <summary>
        /// Copies out the delegates from inside the unity event. Useful for converting an event that was set in the inspector into a delegate that would be used in runtime code (faster).
        /// </summary>
        public static TDelegate ToDelegate<TDelegate>(this UnityEventBase unityEvent, bool showTypeMismatchError = true)
            where TDelegate : class
        {
            if(!typeof(Delegate).IsAssignableFrom(typeof(TDelegate)))
                throw new InvalidCastException(typeof(TDelegate).Name + " does not derive from System.Delegate.");

            MethodInfo delegateMethod = typeof(TDelegate).GetMethod("Invoke");
            Type[] delegateParameterTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();

            Delegate result = null;
            for(int i = 0; i < unityEvent.GetPersistentEventCount(); ++i)
            {
                string methodName = unityEvent.GetPersistentMethodName(i);
                object target = unityEvent.GetPersistentTarget(i);

                if(target == null)
                    continue;

                if(string.IsNullOrEmpty(methodName))
                    continue;

                MethodInfo mi = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, delegateParameterTypes, null);

                //this can happen if the untiy event is pointed at something that doesn't match the delegate parameter types
                if(mi == null)
                {
                    if(showTypeMismatchError)
                        Debug.LogError(methodName + " will not be returned from this conversion because it does not exist or does not match the required delegate method signature: " + string.Join(", ", delegateParameterTypes.Select(x => x.Name).ToArray()));
                    continue;
                }

                //convert the reflected methodinfo and target into our delegate type and assign it
                if(result == null)
                    result = Delegate.CreateDelegate(typeof(TDelegate), target, mi, true);
                else
                    result = Delegate.Combine(result, Delegate.CreateDelegate(typeof(TDelegate), target, mi, true));
            }

            return result as TDelegate;
        }

        //TODO: move to other extension class....
        public static Type[] GetParameterTypes(this Delegate delegateType)
        {
            MethodInfo delegateMethod = delegateType.GetType().GetMethod("Invoke");
            Type[] delegateParameterTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            return delegateParameterTypes;
        }
    }
}