using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class EnemyEnabler : BaseRandomizerLogic
    {
        public override string Name => "Enemy Enabler";

        public override string Info => "Enable/Disable individual enemies from randomization";

        public override bool EnableByDefault => false;

        List<(string Name, string Info, bool DefaultState)> ModuleOptions = null;

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
        }

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
            get
            {
                if(ModuleOptions == null)
                {
                    ModuleOptions = Database.Enemies.Select(x =>
                    {
                        return (x.Value.prefabName, "", true);
                    }).ToList();
                }

                return ModuleOptions;
            }
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
            if(sourceData.ObjectType == PrefabObject.PrefabType.Enemy)
            {
                return validReplacementObjects.Where(x => OptionEnabled(x.prefabName)).ToList();
            }

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
