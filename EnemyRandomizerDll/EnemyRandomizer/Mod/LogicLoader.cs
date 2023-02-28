using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Linq;
using UniRx;
using System;
using System.Reflection;
using Modding.Utils;

namespace EnemyRandomizerMod
{
    internal static class LogicLoader
    {
        public static string RootAssetPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(EnemyRandomizer).Assembly.Location);
            }
        }

        private static bool TryAddLogicInstance(IRandomizerLogic logic)
        {
            if (LogicInstances.ContainsKey(logic.Name))
            {
                Dev.Log($"Found multiple enemy randomizer logics with name {logic.Name}. Skipping duplicate add...");
                return false;
            }

            LogicInstances.Add(logic.Name, logic);
            return true;
        }

        public static Dictionary<string,IRandomizerLogic> LogicInstances { get; private set; } = new Dictionary<string,IRandomizerLogic>();

        public static Dictionary<string, IRandomizerLogic> LoadLogics()
        {
            Dev.Log($"Loading assemblies and constructing enemy randomizer logics");

            var files = Directory.GetFiles(RootAssetPath, "*.dll", SearchOption.AllDirectories)
                .Where(x => Path.GetFileName(x) != "EnemyRandomizer.dll" && Path.GetFileName(x) != "EnemyRandomizerDB.dll").ToList();

            Dev.Log("Found potential logic dlls:");
            files.ForEach(x => Dev.Log(x));

            Assembly Resolve(object sender, ResolveEventArgs args)
            {
                var asm_name = new AssemblyName(args.Name);

                if (files.FirstOrDefault(x => x.EndsWith($"{asm_name.Name}.dll")) is string path)
                    return Assembly.LoadFrom(path);

                return null;
            }

            AppDomain.CurrentDomain.AssemblyResolve += Resolve;

            foreach (string path in files)
            {
                Dev.Log($"Loading assembly `{path}`");

                try
                {
                    var asm = Assembly.LoadFrom(path);

                    int loadedInstances = LogicInstances.Count;
                    Dev.Log($"Loading logic(s) in assembly `{asm.FullName}`");

                    try
                    {
                        asm.GetTypesSafely().Where(x => typeof(IRandomizerLogic).IsAssignableFrom(x)).ToList().ForEach(x => TryAddLogicInstance((IRandomizerLogic)Activator.CreateInstance(x)));
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Error before searching for logic types: ERROR:{e.Message} -- STACKTRACE:{e.StackTrace}");
                    }

                    if (loadedInstances == LogicInstances.Count)
                    {
                        AssemblyName info = asm.GetName();
                        Dev.Log($"Assembly {info.Name} ({info.Version}) loaded, but had 0 IRandomizerLogic's in it");
                    }
                }
                catch (FileLoadException e)
                {
                    Dev.LogError($"Unable to load assembly - {e}");
                }
                catch (BadImageFormatException e)
                {
                    Dev.LogError($"Assembly is bad image. {e}");
                }
                catch (PathTooLongException)
                {
                    Dev.LogError("Unable to load, path to dll is too long!");
                }
            }

            return LogicInstances;
        }
    }
}
