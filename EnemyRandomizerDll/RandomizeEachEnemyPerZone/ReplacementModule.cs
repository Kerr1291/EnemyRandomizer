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
            ("Use basic replacement matching?", "Try to replace enemies with similar types? (DO NOT USE WITH ZOTE MODE!)", false),
            ("Allow bad replacements?", "Try and pick enemies that won't instantly die when spawned", false)
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
            //special logic for replacing lasers or worms
            if (originalObject != null)
            {
                //special logic for replacing this
                if (originalObject.GetObjectPrefab().prefabName == "Laser Turret Frames" ||
                    originalObject.GetObjectPrefab().prefabName == "Worm")
                {
                    List<string> validLaserReplacements = new List<string>()
                    {
                        "Laser Turret Frames",
                        "Big Centipede",
                        "Zote Turret",
                        "White Palace Fly",
                        "Inflater",
                        "Mawlek Turret",
                        "Blow Fly",
                        "Moss Flyer",
                        "Lazy Flyer Enemy",
                        "Plant Trap",
                        "Ceiling Dropper",
                        "Jellyfish",
                        "Abyss Tendrils",
                        "Worm",
                        "Mender Bug",
                    };

                    return validLaserReplacements.Select(x => SpawnerExtensions.GetObjectPrefab(x)).ToList();
                }
            }

            bool isFlyer = originalObject.IsFlying();
            bool isStatic = !originalObject.IsMobile();
            bool isWalker = originalObject.IsWalker();
            bool isClimbing = originalObject.IsClimbing();
            bool isBattleArena = originalObject.IsBattleEnemy();
            bool isBoss = originalObject.IsBoss();
            //var up = originalObject.GetUpFromSelfAngle(false);
            //bool isSideWays = false;
            //bool isUp = false;
            
            //if(Mathnv.FastApproximately(up.y,0f, 0.1f) && (Mathnv.FastApproximately(up.x, 1f, 0.1f) || Mathnv.FastApproximately(up.x, -1f, 0.1f)))
            //{
            //    isSideWays = true;
            //}
            //else if (up.y > 0)
            //    isUp = true;

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
            else if (originalObject.IsSmasher())
            {
                possible = possible.Where(x => MetaDataTypes.GoodSmasherReplacement.Keys.Contains(x.prefabName));
            }

            //if it's not a pogo or smasher, can do normal replacement stuff
            if ((!originalObject.CheckIfIsPogoLogicType() && !originalObject.IsSmasher()))
            {
                //if we're replacing a boss, we're gonna now pull from the special boss list
                if(isBoss)
                {
                    possible = possible.Where(x => MetaDataTypes.SafeForBossReplacementWeights.ContainsKey(x.prefabName));
                }
                else
                {
                    if (MatchReplacements)
                    {
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

                    if (isBattleArena)
                    {
                        possible = possible.Where(x =>
                        {
                            if (MetaDataTypes.SafeForArenas.TryGetValue(x.prefabName, out var isok))
                            {
                                return isok;
                            }
                            return false;
                        }).ToList();
                    }

                    //if allow bad replacements is off, then try and do this to have fewer enemies spawn into spikes and stuff
                    if (Settings.GetOption(CustomOptions[4].Name).value == false)
                    {
                        //if (!isFlyer && (isSideWays || !isUp))
                        //{
                        //    if (isUp)
                        //    {
                        //        //diagonal? do nothing
                        //    }
                        //    else if (isSideWays)
                        //    {
                        //        //sideways, let's replace with a wall or static enemy
                        //        possible = possible.Where(x => SpawnerExtensions.IsWallMounted(x.prefabName));
                        //    }
                        //    else
                        //    {
                        //        //down
                        //        possible = possible.Where(x => SpawnerExtensions.IsWallMounted(x.prefabName));
                        //    }
                        //}
                        //else
                        {
                            //normal upright enemy
                            if (isFlyer)
                            {
                                possible = possible.Where(x => SpawnerExtensions.IsFlying(x.prefabName));
                            }
                            else
                            {
                                possible = possible.Where(x => !SpawnerExtensions.IsFlying(x.prefabName));
                            }
                        }
                    }
                }
            }


            //always filter through this final filter
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

        protected virtual List<float> GetBossWeights(List<PrefabObject> validReplacements)
        {
            return validReplacements.Select(x => x.BossRNGWeight).ToList();
        }

        protected virtual PrefabObject GetObject(List<PrefabObject> validReplacements, List<float> weights, RNG rng)
        {
            int replacementIndex = rng.WeightedRand(weights);
            //Dev.Log("SELCTED REPLACEMENT INDEX " + replacementIndex + " WHICH IS "+ validReplacements[replacementIndex]);
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

            if (originalObject != null)
            {
                //surprise, sometimes it's a saw
                if (originalObject.GetObjectPrefab().prefabName == "Laser Turret Frames" ||
                    originalObject.GetObjectPrefab().prefabName == "Worm")
                {
                    bool isSaw = SpawnerExtensions.RollProbability(out _, 2, 100);

                    if (isSaw)
                    {
                        var up = originalObject.GetUpFromSelfAngle(false);
                        replacementPrefab = Database.Objects["wp_saw"];
                        var saw = Database.Replace(originalObject, replacementPrefab);
                        var damageEnemies = saw.GetOrAddComponent<DamageEnemies>();
                        damageEnemies.circleDirection = true;
                        damageEnemies.damageDealt = 1000;
                        var t = saw.GetOrAddComponent<CustomTweener>();
                        {
                            var min = SpawnerExtensions.GetRayOn(originalObject.transform.position, -up, 20f);
                            var max = SpawnerExtensions.GetRayOn(originalObject.transform.position, up, 20f);
                            t.from = min.point;
                            t.to = max.point;
                            t.travelTime = UnityEngine.Random.Range(1f, 5f);
                            t.lerpFunc = t.easeInOutQuad;
                        }
                        return saw;
                    }
                }
            }

            if (validReplacements.Count == 1)
            {
                replacementPrefab = validReplacements.FirstOrDefault();
            }

            //try and prevent vanilla objects
            if(validReplacements.Count > 1 && (originalObject.GetObjectPrefab() == replacementPrefab || replacementPrefab == null))
            {
                List<float> weights = null;
                var originalPrefab = originalObject.GetObjectPrefab();
                if (originalPrefab.prefabType == PrefabObject.PrefabType.Enemy)
                {
                    if(SpawnerExtensions.IsBoss(originalPrefab.prefabName))
                    {
                        //Dev.Log("THIS IS A BOSS; POSSIBLE BOSSES____");
                        weights = GetBossWeights(validReplacements);
                        //validReplacements.ForEach(x => Dev.Log($"{x} - BOSS WEIGHT:{x.BossRNGWeight}"));
                    }
                    else
                    {
                        weights = GetWeights(validReplacements);
                    }
                }
                int maxTries = 10;
                for (int i = 0; i < maxTries; ++i)
                {
                    if(replacementPrefab == null || originalPrefab == replacementPrefab)
                    {
                        if(weights != null)
                        {
                            //Dev.Log("GETTING WEIGHTED REPLACEMENT -- WEIGHTS ARE");
                            //weights.ForEach(x => Dev.Log($"{x}"));
                            //Dev.Log("GETTING WEIGHTED REPLACEMENT");
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
            var replacement = Database.Replace(originalObject, replacementPrefab);
            //Dev.Log("FINALLY: " + replacement);
            return replacement;
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
