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
        public override string Name => "Default Replacement Logic";

        public override string Info => "Defines if enemies, hazards, and effects should be replace/randomized.";

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Randomize Enemies", "Should enemies be randomized?", true),
            ("Randomize Hazards", "Should (some) hazards be randomized?", false),
            ("Randomize Effects", "Should (some) effects be randomized?", false),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }

        public override List<PrefabObject> GetValidReplacements(ObjectMetadata originalObject, List<PrefabObject> validReplacementObjects)
        {
            if (originalObject.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return GetValidEnemyReplacements(validReplacementObjects);
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

        public virtual List<PrefabObject> GetValidEnemyReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !EnemyReplacer.ReplacementEnemiesToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual List<PrefabObject> GetValidHazardReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !EnemyReplacer.ReplacementHazardsToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual List<PrefabObject> GetValidEffectReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !EnemyReplacer.ReplacementEffectsToSkip.Contains(x.prefabName)).ToList();
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
