using UnityEngine;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    public class TEMPLATE_LOGIC_MODULE : BaseRandomizerLogic
    {
        public override string Name => "TEMPLATE_LOGIC_MODULE";

        public override string Info => "Describe what this module does";

        List<(string Name, string Info, bool DefaultState)> ModuleOptions = new List<(string Name, string Info, bool DefaultState)>()
        {
            (Name:"Example Option 1", Info:"This is a description of the option", DefaultState:true),
            (Name:"Example Option 2", Info:"This is a description of the option", DefaultState:true),
            (Name:"Example Option 3", Info:"This is a description of the option", DefaultState:true),
        };

        public virtual bool OptionEnabled(string name)
        {
            return Settings.GetOptionValue(name) ?? false;
        }

        public virtual bool OptionEnabled(int index)
        {
            int i = Mathf.Clamp(index, 0, ModuleOptions.Count);
            return OptionEnabled(ModuleOptions[i].Name);
        }

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => ModuleOptions;
        }

        /// <summary>
        /// Invoked when a game is started
        /// </summary>
        public override void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            base.OnStartGame(settings);
        }

        /// <summary>
        /// Gets the kinds of things that may be used as replacements
        /// </summary>
        public override List<PrefabObject> GetValidReplacements(ObjectMetadata sourceData, List<PrefabObject> validReplacementObjects)
        {
            return base.GetValidReplacements(sourceData, validReplacementObjects);
        }

        /// <summary>
        /// Use some kind of logic to replace things
        /// </summary>
        public override ObjectMetadata GetReplacement(ObjectMetadata newObject, ObjectMetadata originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return base.GetReplacement(newObject, originalObject, validReplacements, rng);
        }

        /// <summary>
        /// Use some kind of logic to optionally modify an object 
        /// </summary>
        public override ObjectMetadata ModifyObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            return base.ModifyObject(objectToModify, originalObject);
        }
    }
}





///// <summary>
///// Gets the kinds of hazards that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedHazardReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace/modify hazards
///// </summary>
//GameObject ReplaceHazardObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new hazard using the given source data
///// </summary>
//ObjectMetadata ModifyHazardObject(ObjectMetadata other, ObjectMetadata sourceData);

///// <summary>
///// Gets the kinds of effects that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedEffectReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace effects/projectiles
///// </summary>
//GameObject ReplacePooledObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new effect/projectile using the given source data
///// </summary>
//ObjectMetadata ModifyPooledObject(ObjectMetadata other, ObjectMetadata sourceData);