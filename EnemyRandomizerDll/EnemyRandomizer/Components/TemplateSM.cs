using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using nv.Tests;
#endif

namespace nv
{
    //Feel free to copy and use this as a base for starting a new state machine
    public class TemplateSM : GameStateMachine
    {
        public MeshRenderer mainRenderer;
        public tk2dSpriteAnimator mainAnimator;
        public AudioSource actorAudioSource;
        public UnityEngine.Audio.AudioMixerSnapshot audioSnapshot;

        public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        public Dictionary<string, ParticleSystem> particleSystems = new Dictionary<string, ParticleSystem>();
        public Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

        public override bool Running
        {
            get
            {
                return gameObject.activeInHierarchy;
            }

            set
            {
                gameObject.SetActive(value);
            }
        }

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            mainRenderer = GetComponent<MeshRenderer>();
            mainAnimator = GetComponent<tk2dSpriteAnimator>();
        }

        protected override IEnumerator Init()
        {
            yield return base.Init();

            //Start the state machine logic here
            
            //maybe wait in this state until something happens
            //or execute our state logic over time in here
            for(;;)
            {
                yield return new WaitForEndOfFrame();
            }

            //set the next state to transition to
            //nextState = Init;

            yield break;
        }

        /// <summary>
        /// Pull out any needed references from state machines or other game objects here
        /// 
        /// Main helper functions to use:
        /// 
        /// This function returns the value from the state machine, use for things that exist even when the object is inactive (like bools and stuff)
        /// public static T GetValueFromAction<T, U>(GameObject go, string fsmName, string stateName, string valueName, int? actionIndex = null)
        /// 
        /// 
        /// This function returns the value from the state machine using a callback and takes a few frames to make sure the unity game object is active and initialized before pulling the values out.
        /// This is required because some values, like audio clips, are null on prefabs that haven't been instantiated.
        /// public static IEnumerator GetValueFromAction<T, U>(GameObject go, string fsmName, string stateName, string valueName, Action<T> onValueLoaded, int? actionIndex = null)
        ///
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources();

            //TODO: load any references that are needed

            //Example: Load an audio mixer snapshop
            audioSnapshot = GetValueFromAction<UnityEngine.Audio.AudioMixerSnapshot, TransitionToAudioSnapshot>(gameObject, "FSMName", "StateName", "variableName");

            //Example: Load an audio source
            yield return GetValueFromAction<AudioSource, AudioPlayerOneShotSingle>(gameObject, "FSMName", "StateName", "variableName", SetAudioSource);

            //Example: Load an audio clip
            yield return GetValueFromAction<AudioClip, AudioPlayerOneShotSingle>(gameObject, "FSMName", "StateName", "variableName", SetStateMachineValue, 0);
        }

        protected override void RemoveDeprecatedComponents()
        {
            //TODO: uncomment to remove a lot of stuff on this object and all its children
            //base.RemoveDeprecatedComponents();
        }

        protected virtual void SetAudioSource(AudioSource value)
        {
            if(value == null)
            {
                Dev.Log("Warning: SetAudioSource is null!");
                return;
            }
            actorAudioSource = value;
        }

        protected virtual void SetStateMachineValue<T>(T value)
        {
            if(value as AudioClip != null)
            {
                var v = value as AudioClip;
                SetStateMachineValue(audioClips, v.name, v);
            }
            else if(value as ParticleSystem != null)
            {
                var v = value as ParticleSystem;
                SetStateMachineValue(particleSystems, v.name, v);
            }
            else if(value as GameObject != null)
            {
                var v = value as GameObject;
                SetStateMachineValue(gameObjects, v.name, v);
            }
            else
            {
                if(value != null)
                {
                    Dev.Log("Warning: No handler defined for SetStateMachineValue for type " + value.GetType().Name);
                }
                else
                {
                    Dev.Log("Warning: value is null!");
                }
            }
        }

        void SetStateMachineValue<D, T>(D dictionary, string name, T value)
            where D : Dictionary<string,T>
            where T : class
        {
            if(value == null)
            {
                Dev.Log("Warning: "+name+" is null!");
                return;
            }

            Dev.Log("Added: " + name + " to dictionary of "+ dictionary.GetType().Name+ "!");
            dictionary.Add(name, value);
        }
    }
}