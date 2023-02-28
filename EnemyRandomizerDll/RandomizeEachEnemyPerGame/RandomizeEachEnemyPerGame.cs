using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod
{

    public class RandomizeEachEnemyPerGame : BaseRandomizerLogic
    {
        public override string Name => "Randomize Each Game";

        public override string Info => "Randomizes each enemy type in the game to something different.";

        protected override void SetupRNGForReplacement(string enemyName, string sceneName)
        {
            base.SetupRNGForReplacement(enemyName, sceneName);

            string trimmedEnemyName = GetDatabaseKey(enemyName);
            int stringHashValue = trimmedEnemyName.GetHashCode();

            if (baseSeed >= 0)
            {
                rng.Seed = stringHashValue + baseSeed;
            }
            else
            {
                rng.Seed = stringHashValue;
            }
        }
    }
}
