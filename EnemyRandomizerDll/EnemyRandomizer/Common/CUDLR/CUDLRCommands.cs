#if CUDLR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUDLR;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Reflection;
using System;

namespace nv
{
    public static class CUDLRCommands
    {
        private static string ToSingleString(string[] args)
        {
            string fullname = string.Empty;
            foreach(string s in args)
                fullname += s;
            return fullname;
        }

        //private static GameObject commandTargetGO;
        //private static Component commandTargetComponent;

        //private static string pwd;
        //private static string cwd;

        public static class csys
        {
            public const string root = "/";
            public const string up = "..";
            public static string cwd = root;
            public static string cwm = "";
            public static string pwd = root;
            public static bool echo = true;

            public static void Echo(Action<string[]> a) { if(echo) Debug.Log(a.Method.Name); }
        }
        
        [Command("init", "Sets up the debugging system")]
        public static void csys_init(string[] args)
        {
            csys.Echo(csys_init);
            SetMaxLines(new string[] { "10000" });
            Application.RegisterLogCallback(null);
        }

        [Command("ignore", "(-a [arg] | -rm [arg] | -ls | -c) Add/Remove/List/Clear strings to control filtering debug log output")]
        public static void csys_ignore(string[] args)
        {
            csys.Echo(csys_ignore);
            string addarg = null;
            if(GetValue(args,"-a", ref addarg))
            {
                DevLogger.Instance.IgnoreFilters.Add(addarg);
                Debug.Log("Added " + addarg + " to the ignore filters");
            }
            string rmarg = null;
            if(GetValue(args, "-rm", ref rmarg))
            {
                DevLogger.Instance.IgnoreFilters.Remove(rmarg);
                Debug.Log("Removed " + rmarg + " from the ignore filters");
            }
            if(HasKey(args, "-ls"))
            {
                Dev.LogVarArray("Ignore Filters", DevLogger.Instance.IgnoreFilters);
            }
            if(HasKey(args, "-c"))
            {
                DevLogger.Instance.IgnoreFilters.Clear();
                Debug.Log("Cleared the ignore filters");
            }
        }

        [Command("cwd", "Prints the current working hierarchy")]
        public static void csys_cwd(string[] args)
        {
            csys.Echo(csys_cwd);
            Debug.Log(csys.cwd);
        }

        [Command("cd", "Change the current working hierarchy to the given path (if it exists)")]
        public static void csys_cd(string[] args)
        {
            csys.Echo(csys_cd);
            string fullname = ToSingleString(args);
            if(fullname == csys.root)
            {
                csys.cwd = csys.root;
                Debug.Log(csys.cwd);
                return;
            }

            if(fullname == csys.up && csys.cwd != csys.root)
            {
                var t = GameObjectExtensions.FindGameObject(csys.cwd);
                if(t.transform.parent == null)
                {
                    csys.cwd = csys.root;
                    Debug.Log(csys.cwd);
                    return;
                }

                t = t.transform.parent.gameObject;
                csys.cwd = t.gameObject.GetSceneHierarchyPath();
                Debug.Log(csys.cwd);
                return;
            }

            string newPath = string.Empty;

            if(fullname.StartsWith("/"))
            {
                newPath = fullname;
            }
            else
            {
                if(csys.cwd == csys.root)
                    newPath = csys.cwd + fullname;
                else
                    newPath = csys.cwd + "/" + fullname;
            }

            var target = GameObjectExtensions.FindGameObject(newPath);
            if(target != null)
            {
                csys.cwd = newPath;
                Debug.Log(csys.cwd);
            }
            else
            {
                Debug.Log("Path does not exist: " + newPath);
            }
        }

        [Command("ll", "Print current working hierarchy. Params { Optional path or/and + or - and \"filter string\" } Example: ls + \"hello world\" will only show objects with hello world in the CURRENT hierarchy.")]
        public static void csys_ll(string[] args)
        {
            csys_ls(args);
        }

        [Command("ls", "Print current working hierarchy. Params { Optional path or/and + or - and \"filter string\" } Example: ls + \"hello world\" will only show objects with hello world in the CURRENT hierarchy.")]
        public static void csys_ls(string[] args)
        {
            csys.Echo(csys_ls);
            List<string> filters = new List<string>();
            string path = csys.cwd;
            bool? isInclude = null;
            int filterStart = 1;

            if(args.Length > 0)
            {
                if(args[0] == "+")
                    isInclude = true;
                else if(args[0] == "-")
                    isInclude = false;
                else 
                {
                    if(args[0].StartsWith("/"))
                    {
                        path = args[0];
                    }
                    else
                    {
                        if(csys.cwd == csys.root)
                            path += args[0];
                        else
                            path += "/" + args[0];
                    }

                    filterStart = 2;

                    if(args.Length > 2 && args[1] == "+")
                        isInclude = true;
                    else if(args.Length > 2 && args[1] == "-")
                        isInclude = false;
                }

                if(args.Length >= 2 && isInclude == null)
                {
                    Debug.Log("incorrect formatting. Expected: ls + filter OR ls - filter");
                    return;
                }

                for(int i = filterStart; i < args.Length; ++i)
                    filters.Add(args[i]);
            }

            IEnumerable<GameObject> ls_enum = null;

            if(path == csys.root)
                ls_enum = GameObjectExtensions.EnumerateRootObjects();
            else
            {
                if(GameObjectExtensions.FindGameObject(path) == null)
                {
                    Debug.Log("Bad path: " + path);
                    return;
                }

                ls_enum = GameObjectExtensions.FindGameObject(path).EnumerateChildren();
            }

            ls_enum.Where(x =>
            {
                if(filters.Count > 0)
                {
                    if(isInclude.Value)
                    {
                        return filters.Any(y => x.name.Contains(y));
                    }
                    return !filters.Any(y => x.name.Contains(y));
                }
                return true;
            }).ToList().ForEach(x => Debug.Log(x.gameObject.GetSceneHierarchyPath()));
        }

        static bool HasKey(string[] args, string key)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                if(args[i] == key)
                {
                    return true;
                }
            }
            return false;
        }

        static bool GetValue(string[] args, string key, ref string value)
        {
            for(int i = 0; i < args.Length; ++i)
            {
                if(args[i] == key)
                {
                    if((i + 1) < args.Length)
                    {
                        value = args[i + 1];
                        return true;
                    }
                }
            }
            return false;
        }

        [Command("print", "Things related to the given target. Options: -p Scene.Path.To.Target -c (ComponentName) -t [Member.Path.To.Target] -f -m -p -ch (fields/methods/properties/children)")]
        public static void csys_print(string[] args)
        {
            csys.Echo(csys_print);
            string path = csys.cwd;

            string targetPath = null;
            if(GetValue(args, "-p", ref targetPath))
            {
                if(targetPath.StartsWith("/"))
                {
                    path = targetPath;
                }
                else
                {
                    if(targetPath == csys.root)
                        path += targetPath;
                    else
                        path += "/" + targetPath;
                }
            }

            if(path == csys.root)
            {
                Debug.Log("Root is not an object. Nothing to print.");
                return;
            }

            GameObject source = GameObjectExtensions.FindGameObject(path);

            if(source == null)
            {
                Debug.Log("Could not find object with path name " + path);
                return;
            }

            Component cSource = null;

            object target = source;

            string component = null;
            if(GetValue(args,"-c",ref component))
            {
                if(component != null)
                {
                    cSource = source.GetComponent(component);
                    target = cSource;
                }
                else
                {
                    if(component == null)
                    {
                        Debug.Log(path + " Components:");
                        string header = "\t";
                        foreach(var c in source.GetComponents<Component>())
                        {
                            Debug.Log(header + c.GetType().Name);
                        }
                    }
                }
            }

            if(args.Length <= 0)
            {
                Debug.Log(path + " Components:");
                string header = "\t";
                foreach(var c in source.GetComponents<Component>())
                {
                    Debug.Log(header + c.GetType().Name);
                }
            }

            MemberInfo member = null;
            string memberPath = null;
            if(GetValue(args, "-t", ref memberPath))
            {
                object root = target;
                if(cSource != null)
                    root = cSource;
                object finalTarget = null;
                if(csys_get_member_from_path(root, memberPath, ref member, ref finalTarget))
                {
                    target = finalTarget;
                }
            }

            if(HasKey(args, "-p"))
            {
                csys_print_properties(target);
            }

            if(HasKey(args, "-f"))
            {
                csys_print_fields(target);
            }

            if(HasKey(args, "-m"))
            {
                csys_print_methods(target);
            }

            if(HasKey(args, "-ch"))
            {
                if(target is GameObject)
                {
                    Debug.Log("Printing children of " + (target as GameObject).GetSceneHierarchyPath());
                    (target as GameObject).EnumerateChildren().ToList().ForEach(x => Debug.Log(x.name + (x.activeInHierarchy ? string.Empty : " (inactive)")));
                }
                else if(target is Component)
                {
                    Debug.Log("Printing children of " + (target as Component).gameObject.GetSceneHierarchyPath());
                    (target as Component).gameObject.EnumerateChildren().ToList().ForEach(x => Debug.Log(x.name + (x.activeInHierarchy ? string.Empty : " (inactive)")));
                }
                else
                {
                    Debug.Log("Cannot print children of target as it is not a game object or a monobehaviour");
                }
            }
        }

        
        public static void csys_print_properties(object target)
        {
            if(target == null)
            {
                Debug.Log("target is null");
                return;
            }

            foreach(var m in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    Debug.Log(m.Name + ": " + m.GetValue(target, null));
                }
                catch(Exception e)
                {
                    Debug.Log("Exception when trying to get value of the property " + m.Name);
                }
            }
        }
        
        public static void csys_print_fields(object target)
        {
            if(target == null)
            {
                Debug.Log("target is null");
                return;
            }

            foreach(var m in target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    Debug.Log(m.Name + ": " + m.GetValue(target));
                }
                catch(Exception e)
                {
                    Debug.Log("Exception when trying to get value of the field " + m.Name);
                }
            }
        }
        
        public static void csys_print_methods(object target)
        {
            if(target == null)
            {
                Debug.Log("target is null");
                return;
            }

            foreach(var m in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                string parameters = string.Empty;
                foreach(var p in m.GetParameters())
                {
                    parameters += p.ParameterType.FullName + ", ";
                }

                Debug.Log(m.Name + " (" + parameters.TrimEnd(' ') + ")");
            }
        }


        [Command("print_rootasm", "Prints the names of loaded assemblies. -f filter string")]
        public static void csys_print_root_assemblies(string[] args)
        {
            csys.Echo(csys_print_root_assemblies);
            string matching = string.Empty;
            if(GetValue(args, "-f", ref matching))
            {
            }

            List<string> assemblies = new List<string>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(!string.IsNullOrEmpty(matching))
                {
                    assemblies.AddRange(a.GetTypes().Select(x => x.FullName).Distinct().Where(y => y.Contains(matching)));
                }
                else
                {
                    assemblies.AddRange(a.GetTypes().Select(x => x.FullName));
                }
            }

            assemblies = assemblies.Distinct().ToList();

            foreach(var n in assemblies)
            {
                Debug.Log(n);
            }
        }


        [Command("print_ns", "Prints the namespaces of loaded assemblies. -f optional required filter string")]
        public static void csys_print_assmblies_namspaces(string[] args)
        {
            csys.Echo(csys_print_assmblies_namspaces);
            string matching = string.Empty;
            if(GetValue(args, "-f", ref matching))
            {
            }

            List<string> namespaces = new List<string>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(!string.IsNullOrEmpty(matching))
                {
                    namespaces.AddRange(a.GetTypes().Select(x => x.Namespace).Distinct().Where(y => y != null && !y.Contains(".") && y.Contains(matching)));
                }
                else
                {
                    namespaces.AddRange(a.GetTypes().Select(x => x.Namespace).Distinct().Where(y => y != null && !y.Contains(".")));
                }
            }

            namespaces = namespaces.Distinct().ToList();

            foreach(var n in namespaces)
            {
                Debug.Log(n);
            }
        }


        [Command("print_asm", "Prints loaded assemblies. -n Namespace.To.Search  -f filter string.")]
        public static void csys_print_assemblies(string[] args)
        {
            csys.Echo(csys_print_assemblies);
            string namespaceToSearch = string.Empty;
            if(GetValue(args, "-n", ref namespaceToSearch))
            {
            }

            string matching = string.Empty;
            if(GetValue(args, "-f", ref matching))
            {
            }

            if(namespaceToSearch == string.Empty)
            {
                csys_print_root_assemblies(new string[0]);
                return;
            }

            List<string> namespaces = new List<string>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(!string.IsNullOrEmpty(matching))
                {
                    namespaces.AddRange(a.GetTypes().Select(x => x.Namespace).Distinct().Where(y => y != null && y.StartsWith(namespaceToSearch) && y.Contains(matching)));
                }
                else
                {
                    namespaces.AddRange(a.GetTypes().Select(x => x.Namespace).Distinct().Where(y => y != null && y.StartsWith(namespaceToSearch)));
                }
            }

            namespaces = namespaces.Distinct().ToList();

            foreach(var n in namespaces)
            {
                Debug.Log(n);
            }
        }

        [Command("print_types", "Print all the types in the given namespace.  -n Namespace.To.Search  -f filter string syntax: Namespace.To.Search (optional)MatchingString")]
        public static void csys_print_types(string[] args)
        {
            csys.Echo(csys_print_types);
            string namespaceToSearch = string.Empty;            
            if(GetValue(args, "-n", ref namespaceToSearch))
            {
            }

            string matching = string.Empty;
            if(GetValue(args, "-f", ref matching))
            {
            }

            List<string> types = new List<string>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(!string.IsNullOrEmpty(namespaceToSearch))
                {
                    if(!string.IsNullOrEmpty(matching))
                    {
                        types.AddRange(a.GetTypes().Select(x => x.FullName).Distinct().Where(y => y != null && y.StartsWith(namespaceToSearch) && y.Contains(matching)));
                    }
                    else
                    {
                        types.AddRange(a.GetTypes().Select(x => x.FullName).Distinct().Where(y => y != null && y.StartsWith(namespaceToSearch)));
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(matching))
                    {
                        types.AddRange(a.GetTypes().Where(y => y != null && !y.Namespace.Contains(".") && y.FullName.Contains(matching)).Select(x => x.FullName).Distinct());
                    }
                    else
                    {
                        types.AddRange(a.GetTypes().Where(y => y != null && !y.Namespace.Contains(".")).Select(x => x.FullName).Distinct());
                    }
                }
            }

            types = types.Distinct().ToList();

            foreach(var n in types)
            {
                Debug.Log(n);
            }
        }
        
        public static void csys_typels_methods(string[] args)
        {
            string typeName = string.Empty;
            if(args.Length > 0)
                typeName = args[0];

            List<MethodInfo> methods = new List<MethodInfo>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var bflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

                List<System.Type> types = new List<Type>();
                types.AddRange(a.GetTypes().Where(x => x.FullName == typeName));
                types.ForEach(y => methods.AddRange(y.GetMethods(bflags).Select(z => z)));
            }
            methods = methods.Distinct().ToList();

            foreach(var n in methods)
            {
                string parameters = string.Empty;
                foreach(var p in n.GetParameters())
                {
                    parameters += p.ParameterType.FullName + ", ";
                }

                Debug.Log(n.Name + " (" + parameters.TrimEnd(' ') + ")");
            }
        }

        public static void csys_typels_fields(string[] args)
        {
            string typeName = string.Empty;
            if(args.Length > 0)
                typeName = args[0];

            List<FieldInfo> fields = new List<FieldInfo>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var bflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

                List<System.Type> types = new List<Type>();
                types.AddRange(a.GetTypes().Where(x => x.FullName == typeName));
                types.ForEach(y => fields.AddRange(y.GetFields(bflags).Select(z => z)));
            }
            fields = fields.Distinct().ToList();

            foreach(var n in fields)
            {
                Debug.Log(n.Name + ": " + n.GetValue(null));
            }
        }


        public static void csys_typels_properties(string[] args)
        {
            string typeName = string.Empty;
            if(args.Length > 0)
                typeName = args[0];

            List<PropertyInfo> props = new List<PropertyInfo>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var bflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

                List<System.Type> types = new List<Type>();
                types.AddRange(a.GetTypes().Where(x => x.FullName == typeName));
                types.ForEach(y => props.AddRange(y.GetProperties(bflags).Select(z => z)));
            }
            props = props.Distinct().ToList();

            foreach(var n in props)
            {
                Debug.Log(n.Name + ": " + n.GetValue(null, null));
            }
        }

        [Command("type", "Interact with a type. syntax: Namespace.Typename set/invoke/ls { Path.To.Member } {param pairs} OR { -f -p -m } for the ls option")]
        public static void csys_type(string[] args)
        {
            csys.Echo(csys_type);
            string typeName = args[0];

            string command = args[1];
            if(command == "ls")
            {
                csys_typels(args);
                return;
            }

            string memberPath = args[2];
            object target = null;
            MemberInfo member = null;
            if(csys_get_member_from_type(typeName,memberPath,ref member, ref target))
            {
                if(command == "set")
                {
                    if(member is FieldInfo)
                    {
                        csys_set_field(member as FieldInfo, null, args[3], args[4]);
                    }
                    else if(member is PropertyInfo)
                    {
                        csys_set_property(member as PropertyInfo, null, args[3], args[4]);
                    }
                }
                else if(command == "invoke")
                {
                    if(member is MethodInfo)
                    {
                        csys_invoke(member as MethodInfo, null, args.Skip(3).ToArray());
                    }
                }
            }
        }

        //[Command("typels", "Print static information about a given type. syntax: Namespace.Typename -m -f -p  (methods, fields, properties)")]
        public static void csys_typels(string[] args)
        {
            if(args.ToList().Contains("-m"))
            {
                csys_typels_methods(args);
            }

            if(args.ToList().Contains("-f"))
            {
                csys_typels_fields(args);
            }

            if(args.ToList().Contains("-p"))
            {
                csys_typels_properties(args);
            }
        }


        static bool csys_get_member_from_type(string fullTypeName, string memberPath, ref MemberInfo member, ref object target)
        {
            Type targetType = null;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                targetType = a.GetTypes().Where(x => x.FullName == fullTypeName).FirstOrDefault();
                if(targetType != null)
                    break;
            }

            if(targetType == null)
            {
                Debug.Log("Could not locate the given type");
                return false;
            }

            string fullmember = memberPath;
            string[] path = fullmember.Split('.');

            target = null;

            //TODO: fix to allow for more robust handling...
            member = targetType.GetMember(path[0], flags)[0];

            if(member == null)
            {
                Debug.Log("Could not find the member on the target. Member: " + path[0]);
                return false;
            }

            for(int i = 1; i < path.Length; ++i)
            {
                if(member is PropertyInfo)
                {
                    target = (member as PropertyInfo).GetValue(target, null);
                }
                else if(member is FieldInfo)
                {
                    target = (member as FieldInfo).GetValue(target);
                }
                else if(member is MethodInfo)
                {
                    target = (member as MethodInfo).Invoke(target, null);
                }
                else
                {
                    Debug.Log("Part of the path is of an unsupported member type " + member.GetType());
                    return false;
                }

                if(target == null)
                {
                    Debug.Log("Part of the path was null or not found " + member.Name);
                    return false;
                }

                member = target.GetType().GetMember(path[i], flags)[0];

                if(member == null)
                {
                    Debug.Log("Could not find the member on the target component " + path[i]);
                    return false;
                }
            }

            return true;
        }

        static bool csys_get_member_from_path(object root, string memberPath, ref MemberInfo member, ref object target)
        {
            if(root == null)
            {
                Debug.Log("csys_get_member_from_path:: root may not be null");
                return false;
            }

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            string fullmember = memberPath;
            string[] path = fullmember.Split('.');

            target = root;

            //TODO: fix to allow for more robust handling...
            member = target.GetType().GetMember(path[0], flags)[0];

            if(member == null)
            {
                Debug.Log("Could not find the member on the target. Member: " + path[0]);
                return false;
            }

            for(int i = 1; i < path.Length; ++i)
            {
                if(member is PropertyInfo)
                {
                    target = (member as PropertyInfo).GetValue(target, null);
                }
                else if(member is FieldInfo)
                {
                    target = (member as FieldInfo).GetValue(target);
                }
                else if(member is MethodInfo)
                {
                    target = (member as MethodInfo).Invoke(target, null);
                }
                else
                {
                    Debug.Log("Part of the path is of an unsupported member type " + member.GetType());
                    return false;
                }

                if(target == null)
                {
                    Debug.Log("Part of the path was null or not found " + member.Name);
                    return false;
                }

                member = target.GetType().GetMember(path[i], flags)[0];

                if(member == null)
                {
                    Debug.Log("Could not find the member on the target component " + path[i]);
                    return false;
                }
            }

            return true;
        }


        public static void csys_set_property(PropertyInfo member, object target, string rawType, string rawValue)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetType(rawType, false) != null).GetType(rawType);
            if(type == null)
            {
                Debug.Log(rawType + " is not a valid type. Aborting invoke.");

                Debug.Log("Some common types are:");
                Debug.Log(typeof(int).FullName);
                Debug.Log(typeof(float).FullName);
                Debug.Log(typeof(string).FullName);
                Debug.Log(typeof(object).FullName);
                Debug.Log(typeof(char).FullName);
                Debug.Log(typeof(bool).FullName);

                return;
            }

            if(type.FullName == "System.Type")
            {
                Type param = Type.GetType(rawValue);
                member.SetValue(target, param, null);
            }
            else
            if(type.FullName.StartsWith("System"))
            {
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                object value = typeConverter.ConvertFromString(rawValue);
                member.SetValue(target, value, null);
            }
            else
            if(type.FullName.StartsWith("UnityEngine"))
            {
                object value = ParseUnityType(type, rawValue);
                member.SetValue(target, value, null);
            }
            else
            {
                Debug.Log("Conversion from string to " + type + " not defined.");
            }
        }

        public static void csys_set_field(FieldInfo member, object target, string rawType, string rawValue)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetType(rawType, false) != null).GetType(rawType);
            if(type == null)
            {
                Debug.Log(rawType + " is not a valid type. Aborting invoke.");

                Debug.Log("Some common types are:");
                Debug.Log(typeof(int).FullName);
                Debug.Log(typeof(float).FullName);
                Debug.Log(typeof(string).FullName);
                Debug.Log(typeof(object).FullName);
                Debug.Log(typeof(char).FullName);
                Debug.Log(typeof(bool).FullName);

                return;
            }

            if(type.FullName == "System.Type")
            {
                Type param = Type.GetType(rawValue);
                member.SetValue(target, param);
            }
            else
            if(type.FullName.StartsWith("System"))
            {
                System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                object value = typeConverter.ConvertFromString(rawValue);
                member.SetValue(target, value);
            }
            else
            if(type.FullName.StartsWith("UnityEngine"))
            {
                object value = ParseUnityType(type, rawValue);
                member.SetValue(target, value);
            }
            else
            {
                Debug.Log("Conversion from string to " + type + " not defined.");
            }
        }

        public static void csys_invoke(MethodInfo method, object target, string[] args)
        {
            List<System.Type> types = new List<System.Type>();
            List<object> values = new List<object>();

            for(int i = 0; i < args.Length; i += 2)
            {
                System.Type type = null;

                try
                {
                    type = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetType(args[i], false) != null).GetType(args[i]);
                }
                catch(Exception)
                {
                    Debug.Log("Failed to create type from string " + args[i]);
                }

                if(type == null)
                {
                    Debug.Log(args[i] + " is not a valid type. Aborting invoke.");

                    Debug.Log("Some common types are:");
                    Debug.Log(typeof(int).FullName);
                    Debug.Log(typeof(float).FullName);
                    Debug.Log(typeof(string).FullName);
                    Debug.Log(typeof(object).FullName);
                    Debug.Log(typeof(char).FullName);
                    Debug.Log(typeof(bool).FullName);

                    return;
                }

                if(type.FullName == "System.Type")
                {
                    Type param = Type.GetType(args[i + 1]);
                    values.Add(param);
                }
                else
                if(type.FullName.StartsWith("System"))
                {
                    System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                    object value = typeConverter.ConvertFromString(args[i + 1]);
                    values.Add(value);
                }
                else
                if(type.FullName.StartsWith("UnityEngine"))
                {
                    object value = ParseUnityType(type, args[i + 1]);
                    values.Add(value);
                }
                else
                {
                    Debug.Log("Conversion from string to " + type + " not defined.");
                }

                types.Add(type);
            }

            object result = method.Invoke(target, values.ToArray());
            Debug.Log("Invoked "+method.Name + " which returned "+result);
        }




        [Command("set", "Set a value. Options: -p (Optional/Scene/Path/To/Target) -c (ComponentName) -t [Member.Path.To.Target] A Pair of {Type Value}")]
        public static void csys_set(string[] args)
        {
            csys.Echo(csys_set);
            string path = csys.cwd;
            string targetPath = null;
            if(GetValue(args, "-p", ref targetPath))
            {
                if(targetPath.StartsWith("/"))
                {
                    path = targetPath;
                }
                else
                {
                    if(targetPath == csys.root)
                        path += targetPath;
                    else
                        path += "/" + targetPath;
                }
            }

            if(path == csys.root)
            {
                Debug.Log("Root is not an object. Nothing to set.");
                return;
            }

            GameObject source = GameObjectExtensions.FindGameObject(path);

            if(source == null)
            {
                Debug.Log("Could not find object with path name " + path);
                return;
            }

            Component cSource = null;

            object target = source;

            string component = null;
            if(GetValue(args, "-c", ref component))
            {
                cSource = source.GetComponent(component);
                target = cSource;
            }
            else
            {
                if(args.Length <= 1)
                {
                    Debug.Log("Invalid syntax.");
                    return;
                }
            }

            string valueArg = args[args.Length - 1];
            string typeArg = args[args.Length - 2];

            MemberInfo member = null;
            string memberPath = null;
            if(GetValue(args, "-t", ref memberPath))
            {
                object root = target;
                if(cSource != null)
                    root = cSource;
                object finalTarget = null;
                if(csys_get_member_from_path(root, memberPath, ref member, ref finalTarget))
                {
                    target = finalTarget;
                }
            }

            if(member is FieldInfo)
            {
                csys_set_field(member as FieldInfo, target, typeArg, valueArg);
            }
            else if(member is PropertyInfo)
            {
                csys_set_property(member as PropertyInfo, target, typeArg, valueArg);
            }
        }


        [Command("invoke", "Invoke a method. Options: -p (Scene.Path.To.Target) -c (ComponentName) -t [Member.Path.To.Target] Zero or more Pairs of {Type Value}")]
        public static void csys_invoke_local(string[] args)
        {
            csys.Echo(csys_invoke_local);
            string path = csys.cwd;

            string targetPath = null;
            if(GetValue(args, "-p", ref targetPath))
            {
                if(targetPath.StartsWith("/"))
                {
                    path = targetPath;
                }
                else
                {
                    if(targetPath == csys.root)
                        path += targetPath;
                    else
                        path += "/" + targetPath;
                }
            }

            if(path == csys.root)
            {
                Debug.Log("Root is not an object. Nothing to set.");
                return;
            }

            GameObject source = GameObjectExtensions.FindGameObject(path);

            if(source == null)
            {
                Debug.Log("Could not find object with path name " + path);
                return;
            }

            Component cSource = null;

            object target = source;

            string component = null;
            if(GetValue(args, "-c", ref component))
            {
                cSource = source.GetComponent(component);
                target = cSource;
            }
            else
            {
                if(args.Length <= 1)
                {
                    Debug.Log("Invalid syntax.");
                    return;
                }
            }

            string valueArg = args[args.Length - 1];
            string typeArg = args[args.Length - 2];

            MemberInfo member = null;
            string memberPath = null;
            if(GetValue(args, "-t", ref memberPath))
            {
                object root = target;
                if(cSource != null)
                    root = cSource;
                object finalTarget = null;
                if(csys_get_member_from_path(root, memberPath, ref member, ref finalTarget))
                {
                    target = finalTarget;
                }
            }

            if(member is MethodInfo)
            {
                int skip = 0;
                if(HasKey(args,"-p"))
                {
                    skip += 2;
                }
                if(HasKey(args, "-t"))
                {
                    skip += 2;
                }
                if(HasKey(args, "-c"))
                {
                    skip += 2;
                }
                
                csys_invoke(member as MethodInfo, target, args.Skip(skip).ToArray());
            }
        }
































        

        static object ParseUnityType(Type type, string data)
        {
            string trimmed = data.Trim('{', '}');
            string[] parts = trimmed.Split(',');
            if(type == typeof(UnityEngine.Rect))
            {
                return new Rect(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
            else if(type == typeof(UnityEngine.Vector2))
            {
                return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
            }
            else if(type == typeof(UnityEngine.Vector3))
            {
                return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
            }
            else if(type == typeof(UnityEngine.Vector4))
            {
                return new Vector4(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }
            else if(type == typeof(UnityEngine.Color))
            {
                return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
            }

            Debug.Log("Conversion from string to "+type+" not defined.");

            return null;
        }





        

        [Command("SetMaxLines", "Sets the maximum lines displayed in this console. NOTE: This requires an edit to CUDLR's Console.cs as the GDKs class has this value as a const")]
        public static void SetMaxLines(string[] args)
        {
            string fullname = ToSingleString(args);

            int lines = System.Convert.ToInt32(fullname);

            if(lines < 100)
                return;

            System.Type consoleType = CUDLR.Console.Instance.GetType();

            var fieldInfoOld = consoleType.GetField("MAX_LINES", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var fieldInfoNew = consoleType.GetField("maxLines", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            var fieldInfo = fieldInfoOld == null ? fieldInfoNew : fieldInfoOld;

            if(fieldInfo == null)
            {
                Debug.Log("Cannot find field MAX_LINES in the CUDLR Console class. By default this field is const. Go edit the class to remove const to enable this feature.");
                return;
            }

            try
            {
                fieldInfo.SetValue(CUDLR.Console.Instance, lines);
                Debug.Log("Successfully set the max lines");
                return;
            }
            catch(Exception)
            {
                Debug.Log("Class does not have the instance version. Trying the static version...");
            }

            try
            {
                fieldInfo.SetValue(null, lines);
                Debug.Log("Successfully set the max lines");
                return;
            }
            catch(Exception)
            {
                Debug.Log("Big problems. This should never happen.");
            }
        }

        [Command("PrintLoadedSceneNames", "Prints all loaded scene names")]
        public static void PrintLoadedSceneNames(string[] args)
        {
            SceneManagerExtensions.GetLoadedScenes().ToList().ForEach(x => Debug.Log(x.name));
        }

        [Command("PrintSceneHierarchy", "Prints the hierarchy of the given scene name")]
        public static void PrintSceneHierarchy(string[] args)
        {
            string fullname = ToSingleString(args);

            Scene toPrint = SceneManagerExtensions.GetLoadedScenes().ToList().Where(x => string.Compare(x.name, fullname) == 0).First();
            if(toPrint.IsValid())
            {
                toPrint.PrintHierarchy(verbose: false);
            }
        }

        [Command("PrintSceneHierarchyVerbose", "Prints the hierarchy of the given scene name and prints the components on each object")]
        public static void PrintSceneHierarchyVerbose(string[] args)
        {
            string fullname = ToSingleString(args);

            Scene toPrint = SceneManagerExtensions.GetLoadedScenes().ToList().Where(x => string.Compare(x.name, fullname) == 0).First();
            if(toPrint.IsValid())
            {
                toPrint.PrintHierarchy(verbose: true);
            }
        }

        [Command("PrintSceneHierarchyRoot", "Prints the root game objects of the hierarchy of the given scene name")]
        public static void PrintSceneHierarchyRoot(string[] args)
        {
            string fullname = ToSingleString(args);

            Scene toPrint = SceneManagerExtensions.GetLoadedScenes().ToList().Where(x => string.Compare(x.name, fullname) == 0).First();
            if(toPrint.IsValid())
            {
                toPrint.PrintHierarchyRoot();
            }
        }

        [Command("PrintSceneHierarchyChildren", "Print all children of the given game object. Pass the game object by using the full path name. Example: ThemeMain/Root/Code/Sprite")]
        public static void PrintSceneHierarchyChildren(string[] args)
        {
            string fullname = ToSingleString(args);

            GameObject target = GameObjectExtensions.FindGameObject(fullname);

            if(target == null)
            {
                Debug.Log("Could not find object with path name " + fullname);
                return;
            }

            target.PrintSceneHierarchyChildren();
        }

        [Command("PrintSceneHierarchyChildrenAndComponents", "Prints all the children of the given game object and their components. Pass the game object by using the full path name. Example: ThemeMain/Root/Code/Sprite")]
        public static void PrintSceneHierarchyChildrenAndComponents(string[] args)
        {
            string fullname = ToSingleString(args);

            GameObject target = GameObjectExtensions.FindGameObject(fullname);

            if(target == null)
            {
                Debug.Log("Could not find object with path name " + fullname);
                return;
            }

            target.PrintSceneHierarchyChildren(true);
        }

        [Command("PrintSceneHierarchyTree", "Prints the entire hierarchy of this game object. Pass the game object by using the full path name. Example: ThemeMain/Root/Code/Sprite")]
        public static void PrintSceneHierarchyTree(string[] args)
        {
            string fullname = ToSingleString(args);

            GameObject target = GameObjectExtensions.FindGameObject(fullname);

            if(target == null)
            {
                Debug.Log("Could not find object with path name " + fullname);
                return;
            }

            target.PrintSceneHierarchyTree();
        }

        [Command("PrintSceneHierarchyTreeAndComponents", "Prints the entire hierarchy of this game object and the components. Pass the game object by using the full path name. Example: ThemeMain/Root/Code/Sprite")]
        public static void PrintSceneHierarchyTreeAndComponents(string[] args)
        {
            string fullname = ToSingleString(args);

            GameObject target = GameObjectExtensions.FindGameObject(fullname);

            if(target == null)
            {
                Debug.Log("Could not find object with path name " + fullname);
                return;
            }

            target.PrintSceneHierarchyTree(true);
        }
    }
}
#endif