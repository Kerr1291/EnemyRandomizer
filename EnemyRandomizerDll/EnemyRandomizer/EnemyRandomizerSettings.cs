using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace EnemyRandomizerMod
{
    public class EnemyRandomizerSettings : IModSettings
    {
        public bool RNGChaosMode {
            get => GetBool( false );
            set {
                StringValues[ "RNGChaosMode" ] = "Chaos Mode";
                SetBool( value );
            }
        }

        public bool RNGRoomMode {
            get => GetBool( false );
            set {
                StringValues[ "RNGRoomMode" ] = "Room Mode";
                SetBool( value );
            }
        }

        public bool RandomizeDisabledEnemies {
            get => GetBool( false );
            set {
                StringValues[ "RandomizeDisabledEnemies" ] = "Rando Disabled Enemies";
                SetBool( value );
            }
        }

        public int BaseSeed {
            get => GetInt( -1 );
            set {
                StringValues[ "BaseSeed" ] = "Seed (Click for new)";
                SetInt( value );
            }
        }
    }
}
