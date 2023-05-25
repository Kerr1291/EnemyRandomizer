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

        protected virtual List<float> GetWeights(List<PrefabObject> validReplacements)
        {
            return validReplacements.Select(x => x.DefaultRNGWeight).ToList();
        }

        protected virtual PrefabObject GetObject(List<PrefabObject> validReplacements, List<float> weights, RNG rng)
        {
            int replacementIndex = rng.WeightedRand(weights);
            return validReplacements[replacementIndex];
        }

        protected virtual PrefabObject GetObject(List<PrefabObject> validReplacements, RNG rng)
        {
            PrefabObject replacementPrefab = null;
            replacementPrefab = validReplacements.GetRandomElementFromList(rng);
            return replacementPrefab;
        }

        protected virtual GameObject ReplaceObject(GameObject potentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            PrefabObject replacementPrefab = null;

            if(validReplacements.Count == 1)
            {
                replacementPrefab = validReplacements.FirstOrDefault();
            }

            //try and prevent vanilla objects
            if(validReplacements.Count > 1 && (originalObject.GetObjectPrefab() == replacementPrefab || replacementPrefab == null))
            {
                List<float> weights = null;
                var originalPrefab = originalObject.GetObjectPrefab();
                if (originalPrefab.prefabType == PrefabObject.PrefabType.Enemy)
                    weights = GetWeights(validReplacements);
                int maxTries = 10;
                for (int i = 0; i < maxTries; ++i)
                {
                    if(replacementPrefab == null || originalPrefab == replacementPrefab)
                    {
                        if(weights != null)
                        {
                            replacementPrefab = GetObject(validReplacements, weights, rng);
                        }
                        else
                        {
                            replacementPrefab = GetObject(validReplacements, rng);
                        }

                        if (originalPrefab != replacementPrefab)
                            break;
                    }
                    else
                    {
                        break;
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


        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public override bool WillFilterType(PrefabObject.PrefabType prefabType)
        {
            return prefabType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value ||
                   prefabType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value ||
                   prefabType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public override bool WillReplaceType(PrefabObject.PrefabType prefabType)
        {
            return prefabType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value ||
                   prefabType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value ||
                   prefabType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, modify a given type?
        /// </summary>
        public override bool WillModifyType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }
    }
}
