using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using ModCommon;

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

        //change when the global settings are updated to force a recreation of the global settings
        public const string GlobalSettingsVersion = "0.0.8";
    }

    //Global (non-player specific) settings
    public class EnemyRandomizerSettings : IModSettings
    {
        public void Reset()
        {
            BoolValues.Clear();
            StringValues.Clear();
            IntValues.Clear();
            FloatValues.Clear();

            //foreach(string s in EnemyRandomizerDatabase.enemyTypeNames )
            //{
            //    StringValues.Add( s, s );
            //    BoolValues.Add( s, true );
            //}
        }


        public string SettingsVersion {
            get => GetString( "0.0.0" );
            set {
                SetString( value );
            }
        }

        public bool RNGChaosMode {
            get => GetBool( false );
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGChaosMode] = "Chaos Mode";
                SetBool( value );
            }
        }

        public bool RNGRoomMode {
            get => GetBool( true );
            set {
                StringValues[EnemyRandomizerSettingsVars.RNGRoomMode] = "Room Mode";
                SetBool( value );
            }
        }

        public bool RandomizeGeo {
            get => GetBool( false );
            set {
                StringValues[ EnemyRandomizerSettingsVars.RandomizeGeo ] = "Randomize Geo";
                SetBool( value );
            }
        }

        public bool CustomEnemies {
            get => GetBool( false );
            set {
                StringValues[ EnemyRandomizerSettingsVars.CustomEnemies ] = "Custom Enemies";
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
