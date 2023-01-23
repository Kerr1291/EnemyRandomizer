using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
//using EnemyRandomizerMod.Extensions;
using System.Linq;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        public RNG rng;

        void SetupRNGForReplacement(string sceneName, string enemyName)
        {
            if (rng == null)
            {
                if (PlayerSettings.Seed >= 0)
                {
                    rng = new RNG(PlayerSettings.Seed);
                }
                else
                {
                    rng = new RNG();
                    rng.Reset();

                    //save off the generated seed
                    PlayerSettings.Seed = rng.Seed;
                }
            }

            //if not set, enemy replacements will be completely random
            if (!GlobalSettings.RNGChaosMode)
            {
                //set the seed based on the type of enemy we're going to randomize
                //this "should" make each enemy type randomize into the same kind of enemy
                int stringHashValue = enemyName.TrimGameObjectName().GetHashCode();

                //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
                //this will cause enemy types within the same room to be randomized the same
                //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
                if (GlobalSettings.RNGRoomMode)
                {
                    int sceneHashValue = sceneName.GetHashCode();

                    if (PlayerSettings.Seed >= 0)
                    {
                        rng.Seed = stringHashValue + PlayerSettings.Seed + sceneHashValue;
                    }
                    else
                    {
                        rng.Seed = stringHashValue + sceneHashValue;
                    }
                }
                else
                {
                    if (PlayerSettings.Seed >= 0)
                    {
                        rng.Seed = stringHashValue + PlayerSettings.Seed;
                    }
                    else
                    {
                        rng.Seed = stringHashValue;
                    }
                }

                //Dev.Log("Settings seed to " + rng.Seed);
            }
        }


        //TODO: make a new exclusion list (check the string extensions stuff)
        GameObject ReplaceEnemy(GameObject oldEnemy, List<string> exclusions = null)
        {
            //First process skips or exclusions
            List<string> defaultExclusions = new List<string>()
            {
                "Hollow Knight Boss",
                "Radiance",
                "Hollow Shade"
                //"Big Bee (3)" //TODO: improve this so it only happens in scene: Hive_04
            };

            if (oldEnemy == null)
            {
                //DevLogger.Instance.Show(true);
                Dev.LogError("Cannot replace a null enemy!");
                return oldEnemy;
            }

            string enemyName = oldEnemy.name;

            if (string.IsNullOrEmpty(enemyName))
            {
                //DevLogger.Instance.Show(true);
                Dev.LogError("Cannot replace an enemy with no name!");
                return oldEnemy;
            }

            if (enemyName.StartsWith(EnemyRandomizer.ENEMY_RANDO_PREFIX))
                return oldEnemy;

            string trimmedName = enemyName.TrimGameObjectName();

            if ((exclusions != null && exclusions.Contains(trimmedName)) || defaultExclusions.Contains(trimmedName))
            {
                oldEnemy.name = EnemyRandomizer.ENEMY_RANDO_PREFIX + enemyName;
                return oldEnemy;
            }

            if (trimmedName == "Big Bee (3)" && oldEnemy.scene.name == "Hive_04")
            {
                oldEnemy.name = EnemyRandomizer.ENEMY_RANDO_PREFIX + enemyName;
                return oldEnemy;
            }

            //Ok, now we're committed to a replacement. Setup the RNG
            SetupRNGForReplacement(oldEnemy.scene.name, trimmedName);

            //Now for a bit of new logic-- we can always replace an enemy at by this point
            //so we just need to decide if it's going to be a "matched" replacement, or not.
            //
            //DEFINITION: A matched replacement is one that we can look up the source for in our EnemyDataMap
            //            and then do some logic to select a specific kind of replacement.
            //            An UNmatched replacement is one where the source enemy has no outcome on the 
            //            one to be selected by the randomizer.

            bool useMatchingReplacementIfPossible = GlobalSettings.TryMatchingReplacements;
            bool hasMatchingData = EnemyDataMap.TryGetValue(trimmedName, out EnemyData matchingData);
            bool isMatchingReplacement = hasMatchingData && useMatchingReplacementIfPossible;
            bool isInColosseum = oldEnemy.scene.name.ToLower().Contains("colosseum");

            if (hasMatchingData)
            {
                Dev.Log($"Original enemy {enemyName} processed name {trimmedName} has data entry: {hasMatchingData}");
            }

            List<EnemyData> possibleEnemyReplacements;

            if (isMatchingReplacement)
            {
                Dev.Log("Trying to get a set of matching enemies for replacement");
                possibleEnemyReplacements = GetPotentialMatchingEnemyReplacements(oldEnemy, matchingData);
            }
            else
            {
                //DevLogger.Instance.Show(true);
                Dev.LogError($"Cannot find {trimmedName} (Originally named {enemyName}) in data map. Will proceeed without performing a matching replacement on {enemyName}.");

                possibleEnemyReplacements = GetPotentialEnemyReplacements(oldEnemy);
            }

            if (isInColosseum)
            {
                possibleEnemyReplacements = possibleEnemyReplacements.Where(x => !x.excludeFromColosseum).ToList();
            }

            if (!GlobalSettings.AllowBossSpawns)
            {
                possibleEnemyReplacements = possibleEnemyReplacements.Where(x => !x.isBoss).ToList();
            }

            if (!GlobalSettings.AllowHardSpawns)
            {
                possibleEnemyReplacements = possibleEnemyReplacements.Where(x => !x.isHard).ToList();
            }

            if (matchingData != null)
            {
                possibleEnemyReplacements = possibleEnemyReplacements.Where(x => x.name != matchingData.name).ToList();
            }

            if (possibleEnemyReplacements == null || possibleEnemyReplacements.Count <= 0)
            {
                //DevLogger.Instance.Show(true);
                Dev.LogError($"Failed to produce any possible enemy replacements! This should never happen!");
                return oldEnemy;
            }

            EnemyData selectedReplacementData = possibleEnemyReplacements.GetRandomElementFromList(rng);

            GameObject newEnemy = null;

            try
            {
                newEnemy = selectedReplacementData.loadedEnemy.Instantiate(selectedReplacementData, oldEnemy, isMatchingReplacement ? matchingData : null);
            }
            catch (System.Exception e)
            {
                Dev.LogError($"Exception trying to instantiate new enemy using enemy type {selectedReplacementData.loadedEnemy.GetType().Name}! Error:" + e.Message);
            }

            if (newEnemy == null)
                return oldEnemy;

            try
            {
                newEnemy.SetActive(true);
            }
            catch (System.Exception e)
            {
                Dev.LogError("Exception trying to activate new enemy! Error:" + e.Message);
            }

            //hide the old enemy for now
            try
            {
                oldEnemy.SetActive(false);
            }
            catch (System.Exception e)
            {
                Dev.Log("Exception trying to deactivate old enemy! Error:" + e.Message);
            }

            return newEnemy;
        }

        //This version assumes no matching
        List<EnemyData> GetPotentialEnemyReplacements(GameObject oldEnemy)
        {
            List<EnemyData> potentialReplacements;
            if (!GlobalSettings.CustomEnemies)
            {
                potentialReplacements = randomizerEnemies.enemyData.Where(x => !x.isCustomEnemy).ToList();
            }
            else
            {
                potentialReplacements = randomizerEnemies.enemyData;
            }

            if (!GlobalSettings.GodmasterEnemies)
            {
                potentialReplacements = potentialReplacements.Where(x => !x.isGodmaster).ToList();
            }

            //if you have the void charm, shade siblings are no longer enemies
            if (HeroController.instance.playerData.royalCharmState > 2)
            {
                potentialReplacements = potentialReplacements.Where(x => !x.name.Contains("Shade Sibling")).ToList();
            }

            if (oldEnemy.transform.up.y < 0f && oldEnemy.name.TrimGameObjectName() == "Mawlek Turret")
            {
                potentialReplacements = potentialReplacements.Where(x => x.name != "Mawlek Turret").ToList();
            }

            if (oldEnemy.transform.up.y > 0f && oldEnemy.name.TrimGameObjectName() == "Mawlek Turret Ceiling")
            {
                potentialReplacements = potentialReplacements.Where(x => x.name != "Mawlek Turret Ceiling").ToList();
            }

            //TODO: we will still use some rules here, like filter out custom enemies or custom enemies ONLY, etc...
            return potentialReplacements;
        }

        //This version assumes matching
        List<EnemyData> GetPotentialMatchingEnemyReplacements(GameObject oldEnemy, EnemyData sourceData)
        {
            Dev.Log("Seeking replacement for : " + sourceData.name);

            var subset = randomizerEnemies.enemyData.Where(x =>
            {
                return sourceData.IsValidTypeReplacement(x);
            });

            Dev.Log("Possible replacements after type matching: " + subset.ToList().Count);
            subset.ToList().ForEach(x => Dev.Log(x.name));

            subset = subset.Where(y =>
            {
                return sourceData.IsValidSizeReplacement(y);
            });

            Dev.Log("Possible replacements after size matching: " + subset.ToList().Count);
            subset.ToList().ForEach(x => Dev.Log(x.name));

            if (!GlobalSettings.GodmasterEnemies)
            {
                subset = subset.Where(x => !x.isGodmaster);

                Dev.Log("Possible replacements after godmaster filter: " + subset.ToList().Count);
                subset.ToList().ForEach(x => Dev.Log(x.name));
            }

            //if you have the void charm, shade siblings are no longer enemies
            if (HeroController.instance.playerData.royalCharmState > 2)
            {
                subset = subset.Where(x => !x.name.Contains("Shade Sibling"));

                Dev.Log("Possible replacements after shade sibling filter: " + subset.ToList().Count);
                subset.ToList().ForEach(x => Dev.Log(x.name));
            }

            if (oldEnemy.transform.up.y < 0f && oldEnemy.name.TrimGameObjectName() == "Mawlek Turret")
            {
                subset = subset.Where(x => x.name != "Mawlek Turret");

                Dev.Log("Possible replacements after Mawlek Turret filter: " + subset.ToList().Count);
                subset.ToList().ForEach(x => Dev.Log(x.name));
            }

            if (oldEnemy.transform.up.y > 0f && oldEnemy.name.TrimGameObjectName() == "Mawlek Turret Ceiling")
            {
                subset = subset.Where(x => x.name != "Mawlek Turret Ceiling");

                Dev.Log("Possible replacements after Mawlek Turret filter: " + subset.ToList().Count);
                subset.ToList().ForEach(x => Dev.Log(x.name));
            }

            return subset.ToList();
        }
    }
}
