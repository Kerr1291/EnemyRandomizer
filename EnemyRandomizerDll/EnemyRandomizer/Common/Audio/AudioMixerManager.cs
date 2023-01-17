#define GDK_GAME
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;


//TODO: separate the audio mixer code from this class
namespace nv
{
    public class AudioMixerManager : MonoBehaviour
    {
        /// <summary>
        /// publish a MixerParameter to set the value contained within
        /// </summary>
        public class MixerParameter
        {
            /// <summary>
            /// If empty, will apply to all audio mixers
            /// </summary>
            public string mixerName;            
            public string paramName;            
            public float value;
        }

        [Header("Required")]
        [SerializeField]
        AudioMixer audioMixer;
        
        [Tooltip("The mixer must have the volume exposed with this parameter name")]
        [SerializeField]
        string VolumeParameterName = "MasterVolume";

        [Header("Settings")]
        [SerializeField]
        [Tooltip("Determines the maximum game volume used on the slider in the game. (ie. A game volume of 1 will really equal this value).")]
        [Range(0.0f, 1.0f)]
        float maxVolume = 0.0f;

        [SerializeField]
        [Tooltip("Determines the minimum game volume used on the slider in the game. (ie. A game volume of 0 will really equal this value).")]
        [Range(0.0f, 1.0f)]
        float minVolume = 1.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        float defaultVolume = 0.5f;

        public UnityEngine.UI.Slider uiController;

        [SerializeField]
        [Tooltip("Enable to have this mixer added to all audio sources without a mixer in a scene on load")]
        bool autoAddMixerOnSceneLoad;

        [SerializeField] 
        CommunicationNode comms;

        //used to prevent recursive volume updates when updating the ui controller
        bool lockOnValueChange;

        public virtual float MaxVolume
        {
            get
            {
                return maxVolume;
            }
        }

        public virtual float MinVolume
        {
            get
            {
                return minVolume;
            }
        }

        public virtual float Volume
        {
            get
            {
                //return ( (AudioListener.volume / 1f) - MinVolume) / (MaxVolume - MinVolume);
                return ( ( this[ VolumeParameterName ] / 1f ) - MinVolume ) / ( MaxVolume - MinVolume );
            }
            set
            {
                lockOnValueChange = true;
                float clampedValue = Mathf.Clamp01(value);
                //AudioListener.volume = ((MinVolume + clampedValue * (MaxVolume - MinVolume)));
                this[ VolumeParameterName ] = 1f * ( ( MinVolume + clampedValue * ( MaxVolume - MinVolume ) ) );
                UpdateUIController(clampedValue);
                lockOnValueChange = false;
            }
        }

        public virtual float this[string paramName]
        {
            get
            {
                float f = 0f;
                audioMixer.GetFloat(paramName, out f);
                return f;
            }
            set
            {
                audioMixer.SetFloat(paramName, value);
            }
        }

        IEnumerator Start()
        {
            Volume = defaultVolume;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= AddMixerToAudioSourcesOnSceneLoad;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += AddMixerToAudioSourcesOnSceneLoad;

            if(!GameObjectExtensions.FindObjectsOfType<AudioListener>().Any())
                gameObject.AddComponent<AudioListener>();

            yield break;
        }


        protected virtual void OnValidate()
        {
            if(uiController != null)
            {
                uiController.onValueChanged.RemovePersistentListener<float>(this, OnValueChange);
                UpdateUIController(Volume);
                uiController.onValueChanged.AddPersistentListener<float>(this, OnValueChange);
            }
        }

        protected virtual void OnEnable()
        {
            comms.EnableNode(this);

            if(uiController != null)
            {
                uiController.onValueChanged.RemoveListener(OnValueChange);
                UpdateUIController(Volume);
                uiController.onValueChanged.AddListener(OnValueChange);
            }
        }

        protected virtual void OnDisable()
        {
            comms.DisableNode();
            
            if(uiController != null)
            {
                uiController.onValueChanged.RemoveListener(OnValueChange);
            }
        }
        
        [ContextMenu("Set AudioSources with null mixer to this")]
        public virtual void ApplyAudioMixerToAudioSourcesWithNullMixer()
        {
            GameObjectExtensions.FindObjectsOfType<AudioSource>().ForEach(x =>
            {
                x.outputAudioMixerGroup = audioMixer.outputAudioMixerGroup;
            });
        }

        protected virtual void AddMixerToAudioSourcesOnSceneLoad(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if(!autoAddMixerOnSceneLoad)
                return;

            foreach(var rootGO in scene.GetRootGameObjects())
            {
                var emptyMixerSources = rootGO.GetComponentsInChildren<AudioSource>(true).Where(x => x != null && x.outputAudioMixerGroup == null).ToList();
                foreach(AudioSource source in emptyMixerSources)
                {
                    source.outputAudioMixerGroup = audioMixer.outputAudioMixerGroup;
                }
            }
        }

        [CommunicationCallback]
        public virtual void SetParameter(MixerParameter param, object publisher)
        {
            if(string.IsNullOrEmpty(param.mixerName))
                this[param.paramName] = param.value;
            else if(param.mixerName == audioMixer.name)
                this[param.paramName] = param.value;
        }

        protected virtual void UpdateUIController(float newVolume)
        {
            if(uiController == null)
                return;

            uiController.normalizedValue = newVolume;
        }

        protected virtual void OnValueChange(float newValue)
        {
            if(lockOnValueChange)
                return;

            if(uiController == null)
                return;

            Volume = newValue;
        }

        public static void StopAllPlayingSounds()
        {
            AudioListener.pause = false;
            GameObjectExtensions.FindObjectsOfType<AudioSource>().ForEach(x => x.Stop());
        }

        public static void PauseAllPlayingSounds()
        {
            AudioListener.pause = true;
        }

        public static void ResumeAllPlayingSounds()
        {
            AudioListener.pause = false;
        }
    }
}
