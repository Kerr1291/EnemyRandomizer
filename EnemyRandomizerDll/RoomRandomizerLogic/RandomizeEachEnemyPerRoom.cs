using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod
{
    public class RandomizeEachEnemyPerRoom : BaseRandomizerLogic
    {
        public override string Name => "Randomize Each Room";

        public override string Info => "Randomizes each enemy type in a room to something different.";

        protected override void SetupRNGForReplacement(string enemyName, string sceneName)
        {
            base.SetupRNGForReplacement(enemyName, sceneName);

            string trimmedEnemyName = GetDatabaseKey(enemyName);
            int stringHashValue = trimmedEnemyName.GetHashCode();
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
