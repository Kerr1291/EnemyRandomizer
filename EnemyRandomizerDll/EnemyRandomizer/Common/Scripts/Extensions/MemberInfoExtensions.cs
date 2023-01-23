using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace nv
{
    public static class MemberInfoExtensions
    {
        public static Type GetTypeFromMember(this MemberInfo m)
        {
            var mi = m as MethodInfo;
            if(mi != null)
                return mi.ReturnType;
            var pi = m as PropertyInfo;
            if(pi != null)
                return pi.PropertyType;
            var fi = m as FieldInfo;
            if(fi != null)
                return fi.FieldType;
            return null;
        }

        public static List<MemberInfo> FilterMembersByType<T>(this List<MemberInfo> allMembers)
        {
            var fields = allMembers.OfType<FieldInfo>().Cast<FieldInfo>().Where(x => x.FieldType == typeof(T)).Cast<MemberInfo>();
            var props = allMembers.OfType<PropertyInfo>().Cast<PropertyInfo>().Where(x => x.PropertyType == typeof(T)).Cast<MemberInfo>();
            var methods = allMembers.OfType<MethodInfo>().Cast<MethodInfo>().Where(x => (x.ReturnType == typeof(T)) || (x.GetParameters().Length > 0 && x.GetParameters()[0].ParameterType == typeof(T))).Cast<MemberInfo>();
            methods = methods.Where(x => !x.Name.StartsWith("<"));
            return fields.Concat(props).Concat(methods).ToList();
        }

        public static List<MemberInfo> FilterMembersByType(this List<MemberInfo> allMembers, Type type)
        {
            var fields = allMembers.OfType<FieldInfo>().Cast<FieldInfo>().Where(x => x.FieldType == type).Cast<MemberInfo>();
            var props = allMembers.OfType<PropertyInfo>().Cast<PropertyInfo>().Where(x => x.PropertyType == type).Cast<MemberInfo>();
            var methods = allMembers.OfType<MethodInfo>().Cast<MethodInfo>().Where(x => (x.ReturnType == type) || (x.GetParameters().Length > 0 && x.GetParameters()[0].ParameterType == type)).Cast<MemberInfo>();
            methods = methods.Where(x => !x.Name.StartsWith("<"));
            return fields.Concat(props).Concat(methods).ToList();
        }
    }
}