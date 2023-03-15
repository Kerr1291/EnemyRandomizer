using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    public class ScaleRandomizedObjects : BaseRandomizerLogic
    {
        public override string Name => "Scale Randomized Objects";

        public override string Info => "Scales randomized objects to match the size of the objects they replaced.";

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Scale Enemies", "Should this effect randomized enemies?", true),
            ("Scale Hazards", "Should this effect randomized hazards?", false),
            ("Scale Effects", "Should this effect randomized effects?", false),
            ("Match Scaling", "Should enemies be scaled to a size that \'Makes Sense\' or just to random values? (Default is true)", true),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override ObjectMetadata ModifyObject(ObjectMetadata sourceData)
        {
            if (sourceData.ObjectType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
            {
                return ScaleObject(sourceData);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
            {
                return ScaleObject(sourceData);
            }

            else if (sourceData.ObjectType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
            {
                return ScaleObject(sourceData);
            }

            return sourceData;
        }

        public virtual ObjectMetadata ScaleObject(ObjectMetadata sourceData)
        {
            if(Settings.GetOption(CustomOptions[3].Name).value)
            {
                return ApplySizeScale(sourceData, sourceData.ObjectThisReplaced);
            }
            else
            {
                RNG prng = new RNG();
                prng.Seed = sourceData.ObjectName.GetHashCode() + sourceData.SceneName.GetHashCode();
                float scale = prng.Rand(.1f, 3f);
                sourceData.ApplySizeScale(scale);
            }
            return sourceData;
        }

        public virtual ObjectMetadata ApplySizeScale(ObjectMetadata sourceData, ObjectMetadata replacedObject)
        {
            if (replacedObject == null)
                return sourceData;

            float scale = sourceData.GetRelativeScale(replacedObject);
            sourceData.ApplySizeScale(scale);
            return sourceData;
        }
    }
}
