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
    }

    //Global (non-player specific) settings
    public class EnemyRandomizerSettings : IModSettings
    {
        public bool RNGChaosMode {
            get => GetBool( false );
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGChaosMode] = "Chaos Mode";
                SetBool( value );
            }
        }

        public bool RNGRoomMode {
            get => GetBool( false );
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGRoomMode] = "Room Mode";
                SetBool( value );
            }
        }
    }

    //Player specific settings
    public class EnemyRandomizerSaveSettings : IModSettings
    {
        public int Seed {
            get => GetInt(-1);
            set {
                StringValues[EnemyRandomizerSettingsVars.Seed] = "Seed (Click for new)";
                SetInt(value);
            }
        }
    }
}
