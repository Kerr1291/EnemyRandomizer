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

        public override List<PrefabObject> GetValidReplacements(ObjectMetadata originalObject, List<PrefabObject> validReplacementObjects)
        {
            if (originalObject.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return GetValidEnemyReplacements(originalObject, validReplacementObjects);
            }

            else if (originalObject.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return GetValidHazardReplacements(validReplacementObjects);
            }

            else if (originalObject.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return GetValidEffectReplacements(validReplacementObjects);
            }

            return validReplacementObjects;
        }

        public override ObjectMetadata GetReplacement(ObjectMetadata newObject, ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            if (originalObject.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return ReplaceEnemy(originalObject, validReplacements, rng);
            }

            else if (originalObject.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return ReplaceHazardObject(originalObject, validReplacements, rng);
            }

            else if (originalObject.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return ReplacePooledObject(originalObject, validReplacements, rng);
            }

            return newObject == null ? originalObject : newObject;
        }

        public virtual List<PrefabObject> GetValidEnemyReplacements(ObjectMetadata originalObject, List<PrefabObject> validReplacements)
        {
            bool isFlyer = originalObject.IsFlying;
            bool isStatic = !originalObject.IsMobile;
            bool isWalker = originalObject.IsWalker;
            bool isClimbing = originalObject.IsClimbing;

            IEnumerable<PrefabObject> possible = validReplacements;
            if (originalObject.IsPogoLogic)
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

            if (MatchReplacements)
            {
                IEnumerable<ObjectMetadata> pMetas;

                possible = possible.Where(x => !MetaDataTypes.ReplacementEnemiesToSkip.Contains(x.prefabName));

                pMetas = possible.Select(x =>
                {
                    ObjectMetadata m = new ObjectMetadata();
                    m.Setup(x.prefab, Database);
                    return m;
                });

                if (isFlyer)
                {
                    pMetas = pMetas.Where(x => x.IsFlying);
                }
                else
                {
                    pMetas = pMetas.Where(x => !x.IsFlying);

                    if (isWalker)
                    {
                        pMetas = pMetas.Where(x => x.IsWalker);
                    }

                    if(isClimbing)
                    {
                        pMetas = pMetas.Where(x => x.IsClimbing);
                    }
                }

                if(isStatic)
                {
                    pMetas = pMetas.Where(x => !x.IsMobile);
                }

                possible = pMetas.Select(x => x.ObjectPrefab);

                return possible.ToList();
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

        public virtual ObjectMetadata ReplaceEnemy(ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(originalObject, validReplacements, rng);
        }

        public virtual ObjectMetadata ReplaceHazardObject(ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(originalObject, validReplacements, rng);
        }

        public virtual ObjectMetadata ReplacePooledObject(ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return ReplaceObject(originalObject, validReplacements, rng);
        }

        protected virtual ObjectMetadata ReplaceObject(ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            var prefab = validReplacements.GetRandomElementFromList(rng);
            GameObject newObject = Database.Spawn(prefab, originalObject);
            newObject.SetParentToOthersParent(originalObject);
            newObject.transform.position = originalObject.ObjectPosition;
            ObjectMetadata metaObject = new ObjectMetadata();
            metaObject.Setup(newObject, Database);
            return metaObject;
        }
    }
}
