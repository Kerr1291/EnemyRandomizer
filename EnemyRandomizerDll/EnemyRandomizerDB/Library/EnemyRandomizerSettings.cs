using System.Collections.Generic;

#if !LIBRARY
#else
using Dev = Modding.Logger;
#endif

//EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Fly",null);

namespace EnemyRandomizerMod
{
    /// <summary>
    /// General/global mod settings
    /// </summary>
    public class EnemyRandomizerSettings
    {
        public int seed = -1;
        public List<string> loadedLogics = new List<string>();
        public bool UseCustomSeed = false;
        public List<LogicSettings> logicSettings = new List<LogicSettings>();

        public int customColoSeed = -1;
        public bool UseCustomColoSeed = false;

        public bool balanceReplacementHP = true;
        public bool randomizeReplacementGeo = true;
        public bool allowCustomEnemies = true;

        //custom fountain, zote meme bench, etc.
        public bool allowEnemyRandoExtras = true;

        public bool hasDoneAlpha92Reset => __hasDoneAlpha92Reset;

        public bool __hasDoneAlpha92Reset = false;

        public void DoAlpha9Reset()
        {
            seed = -1;
            UseCustomSeed = false;
            customColoSeed = -1;
            UseCustomColoSeed = false;
            balanceReplacementHP = true;
            randomizeReplacementGeo = true;
            allowCustomEnemies = true;
            allowEnemyRandoExtras = true;
            __hasDoneAlpha92Reset = true;
        }
    }


    /// <summary>
    /// Save file specific settings
    /// </summary>
    public class EnemyRandomizerPlayerSettings
    {
        public int enemyRandomizerSeed = -1;
    }
}
