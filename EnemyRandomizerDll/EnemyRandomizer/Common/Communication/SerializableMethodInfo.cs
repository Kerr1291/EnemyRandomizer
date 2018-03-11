using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Wraps a <see cref="MethodInfo"/> to allow it to be serialized by Unity.
/// </summary>
/// <remarks>
/// From here: http://answers.unity3d.com/questions/1159523/saved-methodinfo-variable-resets-on-compile.html
/// </remarks>
[System.Serializable]
public class SerializableMethodInfo : ISerializationCallbackReceiver
{
    public SerializableMethodInfo(MethodInfo aMethodInfo)
    {
        methodInfo = aMethodInfo;
    }

    public MethodInfo methodInfo;
    public SerializableType type;
    public string methodName;
    public List<SerializableType> parameters = null;
    //public int flags = 0;

    public void OnBeforeSerialize()
    {
        //Debug.LogFormat("SerializableMethodInfo.OnBeforeSerialize()");

        if(methodInfo == null)
            return;

        type = new SerializableType(methodInfo.DeclaringType);
        methodName = methodInfo.Name;

        //if (methodInfo.IsPrivate)
        //    flags |= (int)BindingFlags.NonPublic;
        //else
        //    flags |= (int)BindingFlags.Public;

        //if (methodInfo.IsStatic)
        //    flags |= (int)BindingFlags.Static;
        //else
        //    flags |= (int)BindingFlags.Instance;

        var p = methodInfo.GetParameters();
        if(p != null && p.Length > 0)
        {
            parameters = new List<SerializableType>(p.Length);
            for(int i = 0; i < p.Length; i++)
            {
                parameters.Add(new SerializableType(p[i].ParameterType));
            }
        }
        else
            parameters = null;
    }

    public void OnAfterDeserialize()
    {
        ////Debug.LogFormat("SerializableMethodInfo.OnAfterDeserialize()");

        if(type == null)
        {
            Debug.LogErrorFormat("type is null");
            return;
        }
        else
        {
            ////Debug.LogFormat("type = {0}", type.type);
        }

        if(string.IsNullOrEmpty(methodName))
        {
            Debug.LogErrorFormat("methodName is null");
            return;
        }
        else
        {
            ////Debug.LogFormat("methodName = {0}", methodName);
        }

        var t = type.type;
        System.Type[] param = null;

        if(parameters != null && parameters.Count > 0)
        {
            param = new System.Type[parameters.Count];
            for(int i = 0; i < parameters.Count; i++)
            {
                param[i] = parameters[i].type;
            }
        }

        if(param == null)
            methodInfo = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        else
            methodInfo = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, param, null);

        if(methodInfo == null)
        {
            Debug.LogErrorFormat("method that was named " + methodName + " methodInfo is null!");
        }
    }
}
