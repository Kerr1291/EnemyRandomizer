using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;

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
            ("Match Audio to Scaling", "Should enemies have their sounds changed too?", true),
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

            float scale = replacedObject.GetRelativeScale(sourceData, .2f);
            sourceData.ApplySizeScale(scale);

            if (Settings.GetOption(CustomOptions[4].Name).value)
            {
                SetAudioToMatchScale(sourceData);
            }

            return sourceData;
        }

        public virtual void SetAudioToMatchScale(ObjectMetadata sourceData)
        {
            if (!Mathnv.FastApproximately(sourceData.SizeScale, 1f, .01f))
                return;

            float max = 2f;
            float min = .5f;
            Range range = new Range(min, max);
            float t = range.NormalizedValue(sourceData.SizeScale);
            float pitch = max - range.Evaluate(t);

            var go = sourceData.Source;
            var audioSources = go.GetComponentsInChildren<AudioSource>();
            var audioSourcesPitchRandomizer = go.GetComponentsInChildren<AudioSourcePitchRandomizer>();
            //var audioPlayActions = go.GetActionsOfType<AudioPlay>();
            var audioPlayOneShot = go.GetActionsOfType<AudioPlayerOneShot>();
            var audioPlayRandom = go.GetActionsOfType<AudioPlayRandom>();
            var audioPlayOneShotSingle = go.GetActionsOfType<AudioPlayerOneShotSingle>();
            //var audioPlayInState = go.GetActionsOfType<AudioPlayInState>();
            var audioPlayRandomSingle = go.GetActionsOfType<AudioPlayRandomSingle>();
            //var audioPlaySimple = go.GetActionsOfType<AudioPlaySimple>();
            //var audioPlayV2 = go.GetActionsOfType<AudioPlayV2>();
            var audioPlayAudioEvent = go.GetActionsOfType<PlayAudioEvent>();


            audioSources.ToList().ForEach(x => x.pitch = pitch);
            audioSourcesPitchRandomizer.ToList().ForEach(x => x.pitchLower = pitch);
            audioSourcesPitchRandomizer.ToList().ForEach(x => x.pitchUpper = pitch);
            audioPlayOneShot.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayOneShot.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayRandom.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandom.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayOneShotSingle.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayOneShotSingle.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandomSingle.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandomSingle.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayAudioEvent.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayAudioEvent.ToList().ForEach(x => x.pitchMax = pitch);
        }
    }
}
