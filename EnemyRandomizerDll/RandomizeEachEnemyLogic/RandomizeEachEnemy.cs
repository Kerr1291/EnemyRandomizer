using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod
{
    public class RandomizeEachEnemy : BaseRandomizerLogic
    {
        public override string Name => "Randomize Each Enemy";

        public override string Info => "Randomizes each enemy in the game to something different.";

        protected override void SetupRNGForReplacement(string enemyName, string sceneName)
        {
            base.SetupRNGForReplacement(enemyName, sceneName);

            string rawEnemyName = enemyName;
            int stringHashValue = rawEnemyName.GetHashCode();
            int sceneHashValue = sceneName.GetHashCode();

            if (baseSeed >= 0)
            {
                rng.Seed = stringHashValue + baseSeed + sceneHashValue;
            }
            else
            {
                rng.Seed = stringHashValue + sceneHashValue;
            }
        }
    }
}
