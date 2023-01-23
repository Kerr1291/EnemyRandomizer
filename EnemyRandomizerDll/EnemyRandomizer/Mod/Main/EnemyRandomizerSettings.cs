using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Xml.Serialization;
using System;
using System.Linq;

namespace EnemyRandomizerMod
{
    //Global (non-player specific) settings
    public class EnemyRandomizerSettings
    {
        public string SettingsVersion = "Not Yet Alpha";

        public bool RNGChaosMode = false;

        public bool RNGRoomMode = true;

        public bool RandomizeGeo = false;

        public bool CustomEnemies = true;

        public bool GodmasterEnemies = false;

        public bool AllowBossSpawns = false;

        public bool AllowHardSpawns = true;

        public bool TryMatchingReplacements = true;

        public bool NoClip = false;
    }

    //Player specific settings
    public class EnemyRandomizerPlayerSettings
    {
        public int Seed = -1;
    }

    public partial class EnemyRandomizer
    {
        //Settings objects provided by the mod base class
        public static EnemyRandomizerSettings GlobalSettings = new EnemyRandomizerSettings();
        public void OnLoadGlobal(EnemyRandomizerSettings s) => GlobalSettings = s;
        public EnemyRandomizerSettings OnSaveGlobal() => GlobalSettings;

        public static EnemyRandomizerPlayerSettings PlayerSettings = new EnemyRandomizerPlayerSettings();
        public void OnLoadLocal(EnemyRandomizerPlayerSettings s) => PlayerSettings = s;
        public EnemyRandomizerPlayerSettings OnSaveLocal() => PlayerSettings;

        public EnemyRandomizerCustomSettings CustomSettings
        {
            get
            {
                if (customSettings == null)
                {
                    LoadCustomSettings();
                    SaveCustomSettings();
                }
                return customSettings;
            }
        }

        const string currentVersion = "2023.1.20";
        const string defaultCustomSettingsFileName = "settings.xml";
        const string defaultEnemyDataFileName = "enemies.xml";
        const string defaultArenaDataFileName = "arenas.xml";
        const string defaultExclusionsDataFilePath = "exclusions.xml";
        const string defaultObjectsDataFilePath = "objects.xml";

        public override string GetVersion()
        {
            GlobalSettings.SettingsVersion = currentVersion;
            return GlobalSettings.SettingsVersion;
        }

        public static string ModAssetPath
        {
            get
            {
                return Path.GetDirectoryName(typeof(EnemyRandomizer).Assembly.Location);
            }
        }

        public static string GetModAssetPath(string filename)
        {
            return Path.Combine(ModAssetPath, filename);
        }

        public string CustomSettingsFilePath
        {
            get
            {
                if(string.IsNullOrEmpty(customSettingsFilePath))
                {
                    customSettingsFilePath = GetModAssetPath(defaultCustomSettingsFileName);
                }
                return customSettingsFilePath;
            }
            set
            {
                customSettingsFilePath = value;
                LoadCustomSettings();
            }
        }
        public string customSettingsFilePath;

        public string EnemyDataFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(enemyDataFilePath))
                {
                    enemyDataFilePath = GetModAssetPath(defaultEnemyDataFileName);
                }
                return enemyDataFilePath;
            }
            set
            {
                enemyDataFilePath = value;
                //TODO: reload the mod? this may not be something possible so think about removing this setter in the future...
            }
        }
        public string enemyDataFilePath;

        public string ArenaDataFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(arenaDataFilePath))
                {
                    arenaDataFilePath = GetModAssetPath(defaultArenaDataFileName);
                }
                return arenaDataFilePath;
            }
            set
            {
                arenaDataFilePath = value;
                //TODO: reload the mod? this may not be something possible so think about removing this setter in the future...
            }
        }
        public string arenaDataFilePath;

        public string ExclusionsDataFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(exclusionsDataFilePath))
                {
                    exclusionsDataFilePath = GetModAssetPath(defaultExclusionsDataFilePath);
                }
                return exclusionsDataFilePath;
            }
            set
            {
                exclusionsDataFilePath = value;
                //TODO: reload the mod? this may not be something possible so think about removing this setter in the future...
            }
        }
        public string exclusionsDataFilePath;

        public string ObjectsDataFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(objectsDataFilePath))
                {
                    objectsDataFilePath = GetModAssetPath(defaultExclusionsDataFilePath);
                }
                return objectsDataFilePath;
            }
            set
            {
                objectsDataFilePath = value;
                //TODO: reload the mod? this may not be something possible so think about removing this setter in the future...
            }
        }
        public string objectsDataFilePath;

        public EnemyRandomizerCustomSettings customSettings;
        public GameObjectData randomizerEnemies;
        public ArenaList randomizerArenas;
        public ExclusionList randomizerExclusions;
        public ObjectData randomizerObjects;//TODO: get our custom objects loaded

        public void SaveCustomSettings()
        {
            CustomSettingsFilePath.SerializeXMLToFile(customSettings);
        }

        public void LoadCustomSettings()
        {
            Dev.Log($"Loading custom settings from {CustomSettingsFilePath}");
            if (!CustomSettingsFilePath.DeserializeXMLFromFile<EnemyRandomizerCustomSettings>(out customSettings))
            {
                Dev.Log($"Failed to find custom settings file. Generating new one.");
                customSettings = GenerateDefaultCustomSettings();
            }
        }

        protected virtual EnemyRandomizerCustomSettings GenerateDefaultCustomSettings()
        {
            EnemyRandomizerCustomSettings defaultSettings = new EnemyRandomizerCustomSettings();
            return defaultSettings;
        }

        public void SaveEnemyData()
        {
            EnemyDataFilePath.SerializeXMLToFile(randomizerEnemies);
        }

        public void LoadEnemyData()
        {
            try
            {
                if (EnemyDataFilePath.DeserializeXMLFromFile<GameObjectData>(out randomizerEnemies))
                    Dev.Log($"Loaded Enemy Randomizer data from {EnemyDataFilePath}");

                Dev.Log("Loaded entries: " + randomizerEnemies.enemyData.Count);
            }
            catch (Exception e)
            {
                //DevLogger.Instance.Show(true);

                Dev.LogError("Failed to  Enemy Randomizer data. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
            }
        }

        public void SaveArenaData()
        {
            ArenaDataFilePath.SerializeXMLToFile(randomizerArenas);
        }

        public void LoadArenaData()
        {
            //Load arena data file
            try
            {
                if (ArenaDataFilePath.DeserializeXMLFromFile<ArenaList>(out randomizerArenas))
                    Dev.Log($"Arena randomizer info successfully loaded from {ArenaDataFilePath}");
            }
            catch (Exception e)
            {
                //DevLogger.Instance.Show(true);

                Dev.LogError("Failed to load arena randomization data. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
            }
        }

        public void SaveExclusionData()
        {
            ExclusionsDataFilePath.SerializeXMLToFile(randomizerExclusions);
        }

        public void LoadExclusionData()
        {
            //Load exclusion data file
            try
            {
                if (ExclusionsDataFilePath.DeserializeXMLFromFile<ExclusionList>(out randomizerExclusions))
                    Dev.Log($"Randomizer exclusion info successfully loaded from {ExclusionsDataFilePath}");
            }
            catch (Exception e)
            {
                //DevLogger.Instance.Show(true);

                Dev.LogError("Failed to load randomization exclusion data. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
            }
        }

        public void SaveObjectData()
        {
            //TODO
            //ObjectsDataFilePath.SerializeXMLToFile();
        }

        public void LoadObjectData()
        {
            //TODO
        }


        /// <summary>
        /// A mapping of enemy names to their xml data
        /// </summary>
        public Dictionary<string, EnemyData> EnemyDataMap
        {
            get
            {
                //TODO: rewrite this to fix duplicated entries
                if (enemyDataMap == null)
                {
                    var keys = randomizerEnemies.enemyData.Select(x => x.name);
                    var values = randomizerEnemies.enemyData.Select(x => x);

                    //TODO: put error checking in for having multiple enemies with the same name

                    enemyDataMap = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                }
                return enemyDataMap;
            }
        }
        Dictionary<string, EnemyData> enemyDataMap;
    }
}
