using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using nv;

namespace EnemyRandomizerMod
{
    //Feel free to copy and use this as a base for starting a new state machine
    public class TemplateSM : GameStateMachine
    {
        //Example components our state machine might need
        public MeshRenderer mainRenderer;
        public tk2dSpriteAnimator mainAnimator;
        public AudioSource actorAudioSource;
        public UnityEngine.Audio.AudioMixerSnapshot audioSnapshot;

        //Example helpers
        public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        public Dictionary<string, ParticleSystem> particleSystems = new Dictionary<string, ParticleSystem>();
        public Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

        ///This tells the base class under what conditions your state machine should abort or is considered "not running"
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

        /// Call any get's here. This will be called each time the object becomes active, before your states are entered.
        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            mainRenderer = GetComponent<MeshRenderer>();
            mainAnimator = GetComponent<tk2dSpriteAnimator>();
        }
        
        /// Use this is the entry point, use this to transition into your state machine's logic
        protected override IEnumerator Init()
        {
            yield return base.Init();

            //Start the state machine logic here
            
            //maybe wait in this state until something happens
            //or execute our state logic over time in here
            for(;;)
            {
                //do logic or w/e
                yield return new WaitForEndOfFrame();
            }

            //set the next state to transition to
            //nextState = Init;

            //end of state, will transition to whatever nextState is set to
            //yield break;
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

        /// Destroy any logic we're overriding here. For example, if your state machine reimplements the logic for the Superdash FSM, then find and destroy that FSM here.
        /// Using the base class logic will cause ALL PLAYMAKER FSMS and other related components to be destroyed on this game object and all its children automatically.
        protected override void RemoveDeprecatedComponents()
        {
            //TODO: uncomment to remove a lot of stuff on this object and all its children
            //base.RemoveDeprecatedComponents();
        }

        //Example of using a non-generic function to get a value from GetValueFromAction
        protected virtual void SetAudioSource(AudioSource value)
        {
            if(value == null)
            {
                Dev.Log("Warning: SetAudioSource is null!");
                return;
            }
            actorAudioSource = value;
        }

        //Example of using a generic function to get a value from GetValueFromAction
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

        //Example helper for the generic getter function
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