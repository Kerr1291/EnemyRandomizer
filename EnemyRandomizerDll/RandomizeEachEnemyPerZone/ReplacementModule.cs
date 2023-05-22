using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class ReplacementModule : BaseRandomizerLogic
    {
        public override string Name => "Replacement Logic";

        public override string Info => "Defines if enemies, hazards, and effects should be replace/randomized.";

        public override bool EnableByDefault => true;

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Randomize Enemies", "Should enemies be randomized?", true),
            ("Randomize Hazards", "[VERY JANKY]: Should (some) hazards be randomized?", false),
            ("Randomize Effects", "[EXTREMELY JANKY]:Should (some) effects be randomized?", false),
            ("Use basic replacement matching?", "Try to replace enemies with similar types? (DO NOT USE WITH ZOTE MODE!)", false)
        };

        public bool MatchReplacements
        {
            get
            {
                return Settings.GetOption(CustomOptions[3].Name).value;
            }
        }

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            //EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }

        public override List<PrefabObject> GetValidReplacements(GameObject originalObject, List<PrefabObject> validReplacementObjects)
        {
            if (originalObject.ObjectType() == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return GetValidEnemyReplacements(originalObject, validReplacementObjects);
            }

            else if (originalObject.ObjectType() == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return GetValidHazardReplacements(validReplacementObjects);
            }

            else if (originalObject.ObjectType() == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return GetValidEffectReplacements(validReplacementObjects);
            }

            return validReplacementObjects;
        }

        public override GameObject GetReplacement(GameObject currentPotentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            if (originalObject.ObjectType() == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return ReplaceEnemy(currentPotentialReplacement, originalObject, validReplacements, rng);
            }

            else if (originalObject.ObjectType() == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return ReplaceHazardObject(currentPotentialReplacement, originalObject, validReplacements, rng);
            }

            else if (originalObject.ObjectType() == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return ReplacePooledObject(currentPotentialReplacement, originalObject, validReplacements, rng);
            }

            return base.GetReplacement(currentPotentialReplacement, originalObject, validReplacements, rng);
        }

        public virtual List<PrefabObject> GetValidEnemyReplacements(GameObject originalObject, List<PrefabObject> validReplacements)
        {
            bool isFlyer = originalObject.IsFlying();
            bool isStatic = !originalObject.IsMobile();
            bool isWalker = originalObject.IsWalker();
            bool isClimbing = originalObject.IsClimbing();
            bool isBattleArena = originalObject.IsBattleEnemy();
            bool isBoss = originalObject.IsBoss();

            IEnumerable<PrefabObject> possible = validReplacements;
            if (originalObject.CheckIfIsPogoLogicType())
            {
                possible = possible.Where(x => !MetaDataTypes.BadPogoReplacement.Contains(x.prefabName));
                if(isFlyer)
                {
                    possible = possible.Where(x => MetaDataTypes.Flying.Contains(x.prefabName));
                }
                else
                {
                    possible = possible.Where(x => !MetaDataTypes.Flying.Contains(x.prefabName));
                    possible = possible.Where(x => !MetaDataTypes.Static.Contains(x.prefabName));
                }
            }

            if (!originalObject.CheckIfIsPogoLogicType() && MatchReplacements)
            {
                possible = possible.Where(x => !MetaDataTypes.ReplacementEnemiesToSkip.Contains(x.prefabName));


                if(isBoss)
                {
                    possible = possible.Where(x => SpawnerExtensions.IsBoss(x.prefabName));
                }
                else
                {
                    possible = possible.Where(x => !SpawnerExtensions.IsBoss(x.prefabName));

                    if (isFlyer)
                    {
                        possible = possible.Where(x => SpawnerExtensions.IsFlying(x.prefabName));
                    }
                    else
                    {
                        possible = possible.Where(x => !SpawnerExtensions.IsFlying(x.prefabName));

                        if (isWalker)
                        {
                            possible = possible.Where(x => SpawnerExtensions.IsMobile(x.prefabName));
                        }

                        if (isClimbing)
                        {
                            possible = possible.Where(x => SpawnerExtensions.IsClimbing(x.prefabName));
                        }
                    }

                    if (isStatic)
                    {
                        possible = possible.Where(x => !SpawnerExtensions.IsMobile(x.prefabName));
                    }
                }

                //possible = pMetas.Select(x => x.ObjectPrefab);
            }

            if(!originalObject.CheckIfIsPogoLogicType() && isBattleArena)
            {
                possible = possible.Where(x =>
                {
                    if(MetaDataTypes.SafeForArenas.TryGetValue(x.prefabName, out var isok))
                    {
                        return isok;
                    }
                    return false;
                }).ToList();
            }

            return possible.Where(x => !MetaDataTypes.ReplacementEnemiesToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual List<PrefabObject> GetValidHazardReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !MetaDataTypes.ReplacementHazardsToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual List<PrefabObject> GetValidEffectReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !MetaDataTypes.ReplacementEffectsToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual GameObject ReplaceEnemy(GameObject potentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(potentialReplacement, originalObject, validReplacements, rng);
        }

        public virtual GameObject ReplaceHazardObject(GameObject potentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(potentialReplacement, originalObject, validReplacements, rng);
        }

        public virtual GameObject ReplacePooledObject(GameObject potentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(potentialReplacement, originalObject, validReplacements, rng);
        }

        protected virtual PrefabObject GetObject(PrefabObject prefab, List<PrefabObject> validReplacements, RNG rng)
        {
            //if it's a flamebearer, try one more pick since they appear 3x more than anything else
            if (prefab.prefabName.Contains("Flame"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            //if it's a ghost warrior, pick again since there's several ghost bosses
            else if (prefab.prefabName.Contains("Ghost"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Mage"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Ruins"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Mushroom"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Fluke") || prefab.prefabName.Contains("fluke"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Zote"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            else //other enemy types with multiple entries
            if (prefab.prefabName.Contains("Colosseum"))
            {
                prefab = validReplacements.GetRandomElementFromList(rng);
            }
            return prefab;
        }

        protected virtual GameObject ReplaceObject(GameObject potentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            //TODO: weight the list for a more even distro of kinds of enemies
            var replacementPrefab = validReplacements.GetRandomElementFromList(rng);

            //lame shuffle -- TODO replace with a weighted table pull
            {
                for (int i = 0; i < 10; ++i)
                {
                    if (originalObject.GetObjectPrefab() == replacementPrefab)
                    {
                        replacementPrefab = GetObject(replacementPrefab, validReplacements, rng);
                    }
                }
            }

            //try and prevent vanilla objects
            if(validReplacements.Count > 1 && originalObject.GetObjectPrefab() == replacementPrefab)
            {
                int maxTries = 100;
                for (int i = 0; i < maxTries; ++i)
                {
                    if(originalObject.GetObjectPrefab() == replacementPrefab)
                    {
                        replacementPrefab = GetObject(replacementPrefab, validReplacements, rng);
                    }
                }
            }

            //is it vanilla? then we're not replacing anything
            if(originalObject.GetObjectPrefab() == replacementPrefab)
            {
                return potentialReplacement;
            }

            //do the replace
            return Database.Replace(originalObject, replacementPrefab);
        }
    }
}
