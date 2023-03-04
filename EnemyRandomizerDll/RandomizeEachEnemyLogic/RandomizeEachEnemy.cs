using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using nv;

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

        public override GameObject ReplaceEnemy(GameObject other)
        {
            SetupRNGForReplacement(other.name, other.scene.name);
            var prefab = Database.enemyPrefabs.GetRandomElementFromList(rng);
            GameObject newEnemy = Database.Spawn(prefab);
            newEnemy.transform.parent = other.transform.parent;
            newEnemy.transform.localPosition = other.transform.localPosition;
            newEnemy.SafeSetActive(other.activeSelf);
            return newEnemy;
        }

        public override GameObject ReplaceHazardObject(GameObject other)
        {
            SetupRNGForReplacement(other.name, other.scene.name);
            var prefab = Database.hazardPrefabs.GetRandomElementFromList(rng);
            GameObject newHazard = Database.Spawn(prefab);
            newHazard.transform.parent = other.transform.parent;
            newHazard.transform.localPosition = other.transform.localPosition;
            newHazard.SafeSetActive(other.activeSelf);
            return newHazard;
        }

        public override GameObject ReplacePooledObject(GameObject other)
        {
            SetupRNGForReplacement(other.name, other.scene.name);
            var prefab = Database.effectPrefabs.GetRandomElementFromList(rng);
            GameObject newEffect = Database.Spawn(prefab);
            newEffect.transform.parent = other.transform.parent;
            newEffect.transform.localPosition = other.transform.localPosition;
            newEffect.SafeSetActive(other.activeSelf);
            return newEffect;
        }
    }
}
