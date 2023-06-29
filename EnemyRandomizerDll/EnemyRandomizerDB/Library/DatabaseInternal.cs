using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizerDatabase
    {
#if DEBUG
        bool DEBUG_VERBOSE_SPAWNER_ERRORS = true;
#else
        bool DEBUG_VERBOSE_SPAWNER_ERRORS = false;
#endif
        void LinkObjectsToScenes()
        {
            for (int i = 0; i < scenes.Count; ++i)
            {
                scenes[i].sceneObjects.ForEach(x => x.Scene = scenes[i]);
            }
        }

        PrefabObject CreatePrefabObject(string name, GameObject go, SceneObject s)
        {
            PrefabObject prefabObject = null;

            if (s.Loaded)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.LogError($"[{s.Scene.name}] {s.path} = This scene object has already been loaded!");
                return s.LoadedObject;
            }

            string typeName = ToDatabaseKey(name);
            if (!string.IsNullOrEmpty(s.customTypeName))
            {
                typeName = s.customTypeName;
            }

            var config = GetPrefabConfig(typeName, typeof(DefaultPrefabConfig));

            if (config != null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"[{s.Scene.name}] {s.path} = {config.GetType()} - Running Setup");

                prefabObject = new PrefabObject()
                {
                    prefab = go,
                    source = s,
                    prefabType = PrefabObject.PrefabType.None
                };

                config.SetupPrefab(prefabObject);

                if (prefabObject.prefab == null)
                {
                    Dev.LogError($"{s.path} : PREFAB OBJECT CREATED WITH NULL GAME OBJECT REFERENCE");
                    return null;
                }

                if (string.IsNullOrEmpty(prefabObject.prefabName))
                {
                    prefabObject.prefabName = ToDatabaseKey(prefabObject.prefab.name);
                }

                //same as the preloaded object
                if(go == prefabObject.prefab)
                {
                    //TODO: may need to clone the new object
                }
                else
                {
                    //it's something new now
                }
            }
            else
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.LogError($"[{s.Scene.name}] {s.path} = FAILED TO FIND ANY PREFAB CONFIG FOR THIS TYPE (even a default one)");
            }

            if (prefabObject == null)
                return null;

            //don't double load
            if (Objects.ContainsKey(prefabObject.prefabName))
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.LogWarning($"{prefabObject.prefabName} already exists in the loaded object database");

                return null;
            }
            else
            {
                prefabObject.prefab.name = ToDatabaseKey(prefabObject.prefab.name);
                prefabObject.prefab.SetActive(false);

                var constrainPos = prefabObject.prefab.GetComponent<ConstrainPosition>();
                if(constrainPos)
                {
                    GameObject.Destroy(constrainPos);
                }

                GameObject.DontDestroyOnLoad(prefabObject.prefab);
            }

            //setup this stuff after "setup prefab" because the prefab name could've changed
            if (enemyNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Enemy;
                enemyPrefabs.Add(prefabObject);
                Enemies.Add(prefabObject.prefabName, prefabObject);
            }
            else if (hazardNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Hazard;
                hazardPrefabs.Add(prefabObject);
                Hazards.Add(prefabObject.prefabName, prefabObject);
            }
            else if (effectNames.Contains(prefabObject.prefabName))
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Effect;
                effectPrefabs.Add(prefabObject);
                Effects.Add(prefabObject.prefabName, prefabObject);
            }
            else
            {
                prefabObject.prefabType = PrefabObject.PrefabType.Other;
                otherPrefabs.Add(prefabObject);
                Others.Add(prefabObject.prefabName, prefabObject);
            }

            Objects.Add(prefabObject.prefabName, prefabObject);

            s.Loaded = true;
            s.LoadedObject = prefabObject;

            return prefabObject;
        }

        IPrefabConfig GetPrefabConfig(string prefabConfigNameToUse, Type defaultType = null)
        {
            string typeName = "EnemyRandomizerMod." + string.Join("",prefabConfigNameToUse.Split(' ')) + "PrefabConfig";
            Type configType = null;

            if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                Dev.Log("Building prefab for type " + typeName);

            try
            {
                configType = typeof(EnemyRandomizerDatabase).Assembly.GetType(typeName);
            }
            catch (Exception e)
            {
            }

            if (configType == null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.LogWarning($"Cannot find prefab config with type {typeName} in the database assembly");
            }
            else
            {
                if (!typeof(IPrefabConfig).IsAssignableFrom(configType))
                {
                    Dev.LogError($"configType given is not an IPrefabConfig: {configType}");
                    return null;
                }
            }

            if (configType == null && defaultType != null)
            {
                if (!typeof(IPrefabConfig).IsAssignableFrom(defaultType))
                {
                    Dev.LogError($"Default configType given is not an IPrefabConfig: {defaultType}");
                    return null;
                }

                configType = defaultType;
            }

            return (IPrefabConfig)Activator.CreateInstance(configType);
        }

        public ISpawner GetSpawner(string name)
        {
            string typeName = "EnemyRandomizerMod." + string.Join("", name.Split(' ')) + "Spawner";
            Type spawnerType = null;
            ISpawner spawnerTypeToUse = null;

            try
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"Trying to get spawner of type {typeName}");
                spawnerType = typeof(EnemyRandomizerDatabase).Assembly.GetType(typeName);
            }
            catch (Exception e)
            {
                Dev.LogWarning($"No spawner found for {typeName} from the EnemyRandomizerDatabase assembly.");
            }

            if (spawnerType == null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"No matching spawner type found for {typeName}");

                return null;
            }

            spawnerTypeToUse = (ISpawner)Activator.CreateInstance(spawnerType);
            return spawnerTypeToUse;
        }

        //TODO: fix this to load types from other assemblies/namespaces/etc
        bool GetSpawner(PrefabObject p, Type defaultType, out ISpawner spawnerTypeToUse)
        {
            bool isDefault = false;
            string typeName = "EnemyRandomizerMod." + string.Join("", p.prefabName.Split(' ')) + "Spawner";

            if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                Dev.Log(typeName);
            Type spawnerType = null;

            try
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"Trying to spawn type {typeName}");
                spawnerType = typeof(EnemyRandomizerDatabase).Assembly.GetType(typeName);
            }
            catch (Exception e)
            {
                Dev.LogError($"Exception getting type {typeName} from the EnemyRandomizerDatabase assembly. Loading from other assemblies is not yet supported (TODO)");
            }

            if(spawnerType == null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                {
                    Dev.LogWarning($"Cannot find spawner with type {typeName} in the database assembly");
                }
            }

            if (spawnerType == null && defaultType != null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"Trying to spawn default type {defaultType}");

                if (!typeof(ISpawner).IsAssignableFrom(defaultType))
                {
                    Dev.LogError($"Default spawner type given is not an ISpawner: {defaultType}");
                    spawnerTypeToUse = null;
                    return false;
                }

                isDefault = true;
                spawnerType = defaultType;
            }

            if (spawnerType == null)
            {
                if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                    Dev.Log($"No matching spawner type found for {p.prefabName}");

                spawnerTypeToUse = null;
                return false;
            }

            if (DEBUG_VERBOSE_SPAWNER_ERRORS)
                Dev.Log("A spawner type was found and will be created: " + spawnerType.Name);

            spawnerTypeToUse = (ISpawner)Activator.CreateInstance(spawnerType);
            return isDefault;
        }

        static bool TryConvertUniqueNameToCommonName(string nameKey, out string convertedName)
        {
            return uniqueNameToCommonNameMap.TryGetValue(nameKey, out convertedName);
        }

        static bool IsAKeyEndingWithANumber(string nameKey)
        {
            return keysEndingWithASpecialCharacter.Contains(nameKey);
        }

        static bool IsBadDatabaseKeyItem(string key)
        {
            if (badEnemyDatabaseKeys.Any(x => key.Contains(x)))
                return true;

            if (key.Length < 3)
                return true;

            return false;
        }

        static string RemoveAll(string nameKey, string stringToRemove)
        {
            while (nameKey.Contains(stringToRemove))
            {
                nameKey = nameKey.Remove(stringToRemove);
            }
            return nameKey.Trim();
        }

        static string TrimStringAfter(string nameKey, string keyIndex)
        {
            int indexOfStartParethesis = nameKey.IndexOf(keyIndex);
            if (indexOfStartParethesis > 0)
                nameKey = nameKey.Substring(0, indexOfStartParethesis);

            return nameKey.Trim();
        }

        public static string TrimEnd(string str, string toRemove)
        {
            int length = str.Length;
            int toRemoveLength = toRemove.Length;
            if (str.Contains(toRemove))
                return str.Substring(0, length - toRemoveLength);
            else
                return str;
        }

        public static List<System.Type> dataBaseObjectComponents = new List<System.Type>()
        {
            typeof(EnemyDreamnailReaction),
            typeof(HealthManager),
            typeof(DamageHero),
            typeof(EnemyDeathEffects),
            typeof(DamageEnemies),
            typeof(ParticleSystem),
        };

        static List<System.Type> excludedComponents = new List<System.Type>()
        {
            typeof(TMPro.TextMeshPro),
        };

        static List<string> garbageValues = new List<string>()
        {
            {"(Clone)"},
            {"Fixed"  }
        };

        static List<string> endingValues = new List<string>()
        {
            {" ("},
            {"("}
        };

        static List<string> numberValues = new List<string>()
        {
            {" 1"},
            {" 2"},
            {" 3"},
            {" 4"},
            {" 5"},
            {" 6"},
            {" 7"},
            {" 8"},
            {" 9"},
            {" 10"},
        };

        static Dictionary<string, string> uniqueNameToCommonNameMap = new Dictionary<string, string>()
        {
            {"Spawn Roller v2", "Roller"},
            {"Giant Buzzer", "Giant Buzzer" },
            {"Giant Buzzer Col", "Giant Buzzer Col" },
            {"Electric Mage New", "Electric Mage"},
            {"Ruins Sentry FatB", "Ruins Sentry Fat"},
            {"Ruins Flying SentryB", "Ruins Flying Sentry"},
            {"Ruins SentryB", "Ruins Sentry"},
            {"Hatcher NP", "Hatcher"}, 
            {"Mega Jellyfish GG", "Mega Jellyfish"},
            {"Moss Knight C", "Moss Knight"},
            {"Moss Knight B", "Moss Knight"},
            {"Super Spitter Col", "Super Spitter"},
            {"Zombie Basic One 1 (Missing Prefab)", "Zombie Runner"},
            {"Great Shield Zombie bottom", "Great Shield Zombie" },
            {"Ruins Sentry Fat B", "Ruins Sentry Fat" },
            {"Mawlek Turret Ceiling", "Mawlek Turret" },
            {"Buzzer R", "Buzzer" },
            {"Roller R", "Roller" },
            {"Super Spitter R", "Super Spitter" },
            {"Colosseum_Armoured_Roller R", "Colosseum_Armoured_Roller" },
            {"Colosseum_Armoured_Mosquito R", "Colosseum_Armoured_Mosquito" },
            {"Ceiling Dropper Col", "Ceiling Dropper" },
            {"Buzzer Col", "Buzzer" },
            {"_0092_fountain (1)", "_0092_fountain_1"},
            {"Giant Fly Col", "Giant Fly" },
            {"Big Centipede Col", "Big Centipede" },
        };

        static List<string> keysEndingWithASpecialCharacter = new List<string>()
        {
            {"Sword 1"},
            {"Sword 2"},
            {"Sword 3"},
            {"Sword 4"},
            {"Bugs In 1"},
            {"Bugs In 2"},
            {"Zombie Spider 1"},
            {"Zombie Spider 2"},
            {"Zombie Fungus A"},
            {"Zombie Fungus B"},
            {"Hornet Boss 1"},
            {"Hornet Boss 2"},
        };

        
        //Don't load data for these enemies
        static List<string> badEnemyDatabaseKeys = new List<string>()
        {
            {"Hollow Shade"},
            {"Grub Mimic"},
            {"Shell"},
            {"Cap Hit"},
            {"Head"},
            {"Head Box"},
            {"Tinger"},
            {"Dummy Mantis"},
            {"Fluke Fly Spawner"},
            {"Gate Mantis"},
            {"Real Bat"},
            {"Hatcher Baby Spawner"},
            {"Parasite Balloon Spawner"},
            {"Hiveling Spawner"},
            {"Fluke Fly Spawner"},
            {"Baby Centipede Spawner"},
            {"Mage Balloon Spawner"},
            {"Jellyfish Baby Inert"},
            {"Mantis Lord S2" },
            {"Mantis Lord" },
            {"Mantis Lord S1" },
            {"Mantis Lord S3" },
            {"Hurt Box" },
        };
    }
}