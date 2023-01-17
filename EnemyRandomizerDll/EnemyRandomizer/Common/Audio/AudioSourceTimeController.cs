using UnityEngine;
using System.Collections;
using System.Linq;

namespace nv
{
    [System.Serializable]
    public class AudioSourceTimeController
    {        
        public AudioSourceTimeController()
        {

        }

        public AudioSourceTimeController(AudioSource unitySource)
        {
            source = unitySource;
        }

        [SerializeField]
        public AudioSource source;

        [SerializeField]
        public float savedAudioClipTime = 0.0f;

        /// <summary>
        /// By calling this Play() method the user may resume from the saved time
        /// </summary>
        public virtual void Play( bool resume = false )
        {
            if( resume == true )
            {
                savedAudioClipTime = savedAudioClipTime >= source.clip.length ? 0f : savedAudioClipTime;
                source.time = savedAudioClipTime;
            }
            else
            {
                savedAudioClipTime = 0.0f;
                source.time = 0f;
            }

            source.Play();
        }

        /// <summary>
        /// By calling this Play() method the user may start the clip at a different time
        /// </summary>
        public virtual void Play( float startTime )
        {
            savedAudioClipTime = startTime;
            Play( true );
        }

        /// <summary>
        /// Stop the sound right now.
        /// If the audio source is playing, the time will be saved off and may be used on next play.
        /// </summary>
        public virtual void Stop()
        {
            savedAudioClipTime = source.time;
            source.Stop();
        }
    }
}
