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
            ("Randomize Hazards", "Should (some) hazards be randomized?", true),
            ("Randomize Effects", "Should (some) effects be randomized?", true),
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

        public override List<PrefabObject> GetValidReplacements(ObjectMetadata sourceData, List<PrefabObject> validReplacementObjects)
        {
            if (sourceData.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return GetValidEnemyReplacements(validReplacementObjects);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return GetValidHazardReplacements(validReplacementObjects);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return GetValidEffectReplacements(validReplacementObjects);
            }

            return validReplacementObjects;
        }

        public override ObjectMetadata GetReplacement(ObjectMetadata sourceData, List<PrefabObject> validReplacements, RNG rng)
        {
            if (sourceData.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return ReplaceEnemy(sourceData, validReplacements, rng);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return ReplaceHazardObject(sourceData, validReplacements, rng);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return ReplacePooledObject(sourceData, validReplacements, rng);
            }

            return sourceData;
        }

        public virtual List<PrefabObject> GetValidEnemyReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements;
        }

        public virtual List<PrefabObject> GetValidHazardReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements;
        }

        public virtual List<PrefabObject> GetValidEffectReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => !EnemyReplacer.ReplacementEffectsToSkip.Contains(x.prefabName)).ToList();
        }

        public virtual ObjectMetadata ReplaceEnemy(ObjectMetadata other, List<PrefabObject> validReplacements, RNG rng)
        {
            var prefab = validReplacements.GetRandomElementFromList(rng);
            GameObject newEnemy = Database.Spawn(prefab, other);
            newEnemy.SetParentToOthersParent(other);
            newEnemy.transform.position = other.ObjectPosition;
            ObjectMetadata metaObject = new ObjectMetadata();
            metaObject.Setup(newEnemy, Database);
            return metaObject;
        }

        public virtual ObjectMetadata ReplaceHazardObject(ObjectMetadata other, List<PrefabObject> validReplacements, RNG rng)
        {
            var prefab = validReplacements.GetRandomElementFromList(rng);
            GameObject newHazard = Database.Spawn(prefab, other);
            newHazard.SetParentToOthersParent(other);
            newHazard.transform.position = other.ObjectPosition;
            ObjectMetadata metaObject = new ObjectMetadata();
            metaObject.Setup(newHazard, Database);
            return metaObject;
        }

        public virtual ObjectMetadata ReplacePooledObject(ObjectMetadata other, List<PrefabObject> validReplacements, RNG rng)
        {
            var prefab = validReplacements.GetRandomElementFromList(rng);
            GameObject newEffect = Database.Spawn(prefab, other);
            newEffect.SetParentToOthersParent(other);
            newEffect.transform.position = other.ObjectPosition;
            ObjectMetadata metaObject = new ObjectMetadata();
            metaObject.Setup(newEffect, Database);
            return metaObject;
        }
    }
}
