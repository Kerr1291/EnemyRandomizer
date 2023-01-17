using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace nv
{
    public static class TypeExtensions
    {
        static BindingFlags allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        
        public static IEnumerable<MemberInfo> GetPublicMembers(this Type t)
        {
            var publicMembers = t.GetMembers(BindingFlags.Public | BindingFlags.Instance).Select(x => x);
            publicMembers = publicMembers.Where(x => !x.Name.StartsWith("<"));
            return publicMembers;
        }

        public static IEnumerable<MemberInfo> GetNonpublicMembers(this Type t)
        {
            var nonpublicMembers = t.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance).Select(x => x);
            nonpublicMembers = nonpublicMembers.Where(x => !x.Name.StartsWith("<"));
            return nonpublicMembers;
        }

        public static IEnumerable<MemberInfo> GetInstanceMembers(this Type t, bool filterObsolete = true)
        {
            var publicMembers = t.GetPublicMembers();
            var nonpublicMembers = t.GetNonpublicMembers();
            var allMembers = publicMembers.Concat(nonpublicMembers);

            //filter out generic methods and generic special property methods
            allMembers = allMembers.Where(x => ((x as MethodInfo) == null) || (((x as MethodInfo) != null) && !((x as MethodInfo).IsSpecialName) && !((x as MethodInfo).ContainsGenericParameters)));

            if(filterObsolete)
            {
#if NET_4_6
                allMembers = allMembers.Where(x => x.GetCustomAttribute(typeof(ObsoleteAttribute)) == null).ToList();
#else
                allMembers = allMembers.Where(x =>
                {
                    var customAttributes = x.GetCustomAttributes(typeof(ObsoleteAttribute), true);
                    bool hasObsolete = customAttributes.Where(y => y.GetType().IsAssignableFrom(typeof(ObsoleteAttribute))).Any();
                    return !hasObsolete;
                });
#endif
            }

            allMembers = allMembers.Where(x => !x.Name.StartsWith("<"));
            return allMembers;
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type t, bool filterObsolete = true)
        {
            var allMembers = t.GetMembers(allFlags).AsEnumerable();

            //filter out generic methods and generic special property methods
            allMembers = allMembers.Where(x => ((x as MethodInfo) == null) || (((x as MethodInfo) != null) && !((x as MethodInfo).IsSpecialName) && !((x as MethodInfo).ContainsGenericParameters)));

            if(filterObsolete)
            {
#if NET_4_6
                allMembers = allMembers.Where(x => x.GetCustomAttribute(typeof(ObsoleteAttribute)) == null).ToList();
#else
                allMembers = allMembers.Where(x =>
                {
                    var customAttributes = x.GetCustomAttributes(typeof(ObsoleteAttribute), true);
                    bool hasObsolete = customAttributes.Where(y => y.GetType().IsAssignableFrom(typeof(ObsoleteAttribute))).Any();
                    return !hasObsolete;
                });
#endif
            }
            allMembers = allMembers.Where(x => !x.Name.StartsWith("<"));
            return allMembers;
        }


        public static FieldInfo GetFieldInfo(this Type t, string name, bool filterObsolete = true)
        {
            return t.GetAllMembers(filterObsolete).Where(x => x is FieldInfo).Cast<FieldInfo>().FirstOrDefault(y => y.Name == name);
        }

        public static PropertyInfo GetPropertyInfo(this Type t, string name, bool filterObsolete = true)
        {
            return t.GetAllMembers(filterObsolete).Where(x => x is PropertyInfo).Cast<PropertyInfo>().FirstOrDefault(y => y.Name == name);
        }

        public static MethodInfo GetMethodInfo(this Type t, string name, bool filterObsolete = true)
        {
            return t.GetAllMembers(filterObsolete).Where(x => x is MethodInfo).Cast<MethodInfo>().FirstOrDefault(y => y.Name == name);
        }
    }
}