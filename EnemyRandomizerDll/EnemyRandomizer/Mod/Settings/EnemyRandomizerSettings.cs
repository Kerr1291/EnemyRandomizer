using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
 

namespace EnemyRandomizerMod
{
    //string lookup names for the config options
    public class EnemyRandomizerSettingsVars
    {
        public const string Seed = "Seed";
        public const string CustomSeed = "CustomSeed";

        public const string RNGChaosMode = "RNGChaosMode";
        public const string RNGRoomMode = "RNGRoomMode";
        public const string RandomizeGeo = "RandomizeGeo";
        public const string CheatNoclip = "(Cheat) No Clip";
        public const string CustomEnemies = "CustomEnemies";
        public const string GodmasterEnemies = "GodmasterEnemies";

        //change when the global settings are updated to force a recreation of the global settings
        public const string GlobalSettingsVersion = "0.1.0";
    }

    public static class EnemyRandomizerSettingsHelpers
    {
        public static Dictionary<string, Action<EnemyRandomizerSettings, bool>> BoolValues = new Dictionary<string, Action<EnemyRandomizerSettings, bool>>()
        {
            {"RNGChaosMode",     (x, v) => x.RNGChaosMode = v },
            {"RNGRoomMode",      (x, v) => x.RNGRoomMode = v            },
            {"RandomizeGeo",     (x, v) => x.RandomizeGeo = v            },
            {"CustomEnemies",    (x, v) => x.CustomEnemies = v            },
            {"GodmasterEnemies", (x, v) => x.GodmasterEnemies = v            },
            {"(Cheat) No Clip",  (x, v) => x.NoClip = v          }
        };

        public static Dictionary<string, Func<EnemyRandomizerSettings, bool>> GetBoolValues = new Dictionary<string, Func<EnemyRandomizerSettings, bool>>()
        {
            {"RNGChaosMode",     (x) => x.RNGChaosMode },
            {"RNGRoomMode",      (x) => x.RNGRoomMode            },
            {"RandomizeGeo",     (x) => x.RandomizeGeo            },
            {"CustomEnemies",    (x) => x.CustomEnemies          },
            {"GodmasterEnemies", (x) => x.GodmasterEnemies          },
            {"(Cheat) No Clip",  (x) => x.NoClip         }
        };

        public static Dictionary<string, Func<EnemyRandomizerSettings, string>> GetStringValues = new Dictionary<string, Func<EnemyRandomizerSettings, string>>()
        {
            {"RNGChaosMode",     (x) => "Chaos Mode" },
            {"RNGRoomMode",      (x) => "Room Mode"            },
            {"RandomizeGeo",     (x) => "Randomize Geo"            },
            {"CustomEnemies",    (x) => "Custom Enemies"          },
            {"GodmasterEnemies", (x) => "Godmaster Enemies"          },
            {"(Cheat) No Clip",  (x) => "(Cheat) No Clip"          }
        };

        public static Dictionary<string, Func<EnemyRandomizerSettings, string>> GetLocalStringValues = new Dictionary<string, Func<EnemyRandomizerSettings, string>>()
        {
            {"Seed",     (x) => "Seed (Click for new)" }
        };
    }

    //Global (non-player specific) settings
    public class EnemyRandomizerSettings
    {

        public void Reset()
        {
            EnemyRandomizerSettingsHelpers.BoolValues.Select(x => x.Value).ToList().ForEach(z => z.Invoke(this,false));
        }

        public string SettingsVersion = "0.0.0.0";

        public bool RNGChaosMode = false;

        //{
        //    get => GetBool(false);
        //    set
        //    {
        //        StringValues[EnemyRandomizerSettingsVars.RNGChaosMode] = "Chaos Mode";
        //        SetBool(value);
        //    }
        //}

        public bool RNGRoomMode = true;
        //{
        //    get => GetBool( true );
        //    set {
        //        StringValues[EnemyRandomizerSettingsVars.RNGRoomMode] = "Room Mode";
        //        SetBool( value );
        //    }
        //}

        public bool RandomizeGeo = false;
        //{
        //    get => GetBool( false );
        //    set {
        //        StringValues[ EnemyRandomizerSettingsVars.RandomizeGeo ] = "Randomize Geo";
        //        SetBool( value );
        //    }
        //}

        public bool CustomEnemies = false;
        //{
        //    get => GetBool( false );
        //    set {
        //        StringValues[ EnemyRandomizerSettingsVars.CustomEnemies ] = "Custom Enemies";
        //        SetBool( value );
        //    }
        //}

        public bool GodmasterEnemies = false;
        //    get => GetBool (false);
        //    set {
        //        StringValues[EnemyRandomizerSettingsVars.GodmasterEnemies] = "Godmaster Enemies";
        //        SetBool (value);
        //    }
        //}

        public bool NoClip = false;
    }

    //Player specific settings
    public class EnemyRandomizerPlayerSettings
    {
        public int Seed = -1;
        //public int Seed
        //{
        //    get => GetInt(-1);
        //    set
        //    {
        //        StringValues[EnemyRandomizerSettingsVars.Seed] = "Seed (Click for new)";
        //        SetInt(value);
        //    }
        //}
    }
}
