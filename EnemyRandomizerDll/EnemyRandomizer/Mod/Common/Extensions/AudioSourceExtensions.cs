using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace nv
{
    public static class AudioSourceExtensions
    {
        public static TimedRoutine Fade(this AudioSource target, float fadeToVolume, float fadeTime, bool stopPlaybackOnStop = false)
        {
            if(target == null)
                return null;

            float startVolume = target.volume;
            TimedRoutine fadeAction = new TimedRoutine(fadeTime);

            fadeAction.OnLerp = (float timeNormalized) =>
            {
                target.volume = Mathf.Lerp(startVolume, fadeToVolume, timeNormalized);
            };

            fadeAction.OnStop = (bool isComplete) =>
            {
                if(stopPlaybackOnStop)
                    target.Stop();
            };

            fadeAction.Start();

            return fadeAction;
        }

        public static TimedRoutine Fade(this AudioSource target, float fadeToVolume, float fadeTime, bool stopPlaybackOnStop, bool restoreVolumeOnStop)
        {
            if(target == null)
                return null;

            float startVolume = target.volume;
            TimedRoutine fadeAction = new TimedRoutine(fadeTime);

            fadeAction.OnLerp = (float timeNormalized) =>
            {
                target.volume = Mathf.Lerp(startVolume, fadeToVolume, timeNormalized);
            };

            fadeAction.OnStop = (bool isComplete) =>
            {
                if(stopPlaybackOnStop)
                    target.Stop();

                if(restoreVolumeOnStop)
                    target.volume = startVolume;
            };

            fadeAction.Start();
            return fadeAction;
        }        
    }
}