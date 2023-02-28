using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod
{
    public class RandomizeEachEnemyPerZone : BaseRandomizerLogic
    {
        public override string Name => "Randomize Each Zone";

        public override string Info => "Randomizes each enemy type in a zone to something different.";

        protected override void SetupRNGForReplacement(string enemyName, string sceneName)
        {
            base.SetupRNGForReplacement(enemyName, sceneName);

            string trimmedEnemyName = GetDatabaseKey(enemyName);
            int stringHashValue = trimmedEnemyName.GetHashCode();
            int zoneHashValue = EnemyRandomizer.GetCurrentMapZone().GetHashCode();

            if (baseSeed >= 0)
            {
                rng.Seed = stringHashValue + baseSeed + zoneHashValue;
            }
            else
            {
                rng.Seed = stringHashValue + zoneHashValue;
            }
        }
    }
}
