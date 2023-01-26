using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
using System.Reflection;

namespace EnemyRandomizerMod
{
    public static class RandomizerEnemyFactory
    {
        static Dictionary<string, System.Type> validEnemyTypes = new Dictionary<string, System.Type>();

        public static bool IsValid<TEnemyType>()
        {
            return validEnemyTypes.ContainsKey(typeof(TEnemyType).Name);
        }

        public static bool IsValid(string typeName)
        {
            return validEnemyTypes.ContainsKey(typeName);
        }

        /// <summary>
        /// If another mod or other assembly wanted to create their own custom overrides for how enemies are loaded
        /// they could use this method to register all the types defined in their assembly.
        /// </summary>
        public static void RegisterValidTypesInAssembly(Type typeFromAssemblyToUse)
        {
            //search the given assembly for anything using the IRandomizerEnemy interface
            List<Type> validTypes = Assembly.GetAssembly(typeFromAssemblyToUse).GetTypes().Where(x => typeof(IRandomizerEnemy).IsAssignableFrom(x)).ToList();

            validTypes.ForEach(x => validEnemyTypes.Add(x.Name, x));
        }

        public static void RegisterEnemyType<TEnemyType>()
            where TEnemyType : class, IRandomizerEnemy
        {
            if (!validEnemyTypes.ContainsKey(typeof(TEnemyType).Name))
            {
                validEnemyTypes.Add(typeof(TEnemyType).Name, typeof(TEnemyType));
            }
        }

        public static void RegisterEnemyType(string typeName)
        {
            if (!validEnemyTypes.ContainsKey(typeName))
            {
                validEnemyTypes.Add(typeName, typeof(EnemyRandomizer).Assembly.GetType(typeName));
            }
        }

        public static IRandomizerEnemy Create(string typeName, EnemyData thisEnemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            if (validEnemyTypes.TryGetValue(typeName, out var enemyType))
            {
                var newEnemy = (IRandomizerEnemy)Activator.CreateInstance(enemyType);
                newEnemy.LinkDataObjects(thisEnemy, knownEnemyTypes, prefabObject);
                newEnemy.SetupPrefab();
                newEnemy.DebugPrefab();
                return newEnemy;
            }

            return null;
        }

        public static TEnemyType Create<TEnemyType>(EnemyData thisEnemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
            where TEnemyType : class, IRandomizerEnemy
        {
            if (validEnemyTypes.TryGetValue(typeof(TEnemyType).Name, out var enemyType))
            {
                var newEnemy = (TEnemyType)Activator.CreateInstance(enemyType);
                newEnemy.LinkDataObjects(thisEnemy, knownEnemyTypes, prefabObject);
                newEnemy.SetupPrefab();
                newEnemy.DebugPrefab();
                return newEnemy;
            }

            return null;
        }
    }
}
