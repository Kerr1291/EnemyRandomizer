using System;
using System.Collections;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{
    public abstract partial class GameStateMachine : MonoBehaviour
    {
        //the only thing that needs to be implemented by subclasses
        public abstract bool Running
        {
            get; set;
        }

        public virtual bool BlockingAnimationIsPlaying
        {
            get; protected set;
        }
        
        //current state of the state machine
        protected Func<IEnumerator> nextState = null;
        protected Func<IEnumerator> overrideNextState = null;
        protected IEnumerator currentState;
        protected IEnumerator mainLoop;
        
        protected virtual void OnEnable()
        {
            if(mainLoop != null)
            {
                CleanUpObjectOnReEnable();
            }

            SetupRequiredReferences();

            StartCoroutine(Setup());
        }

        //called when re-enabling a state machine that was disabled while running
        protected virtual void CleanUpObjectOnReEnable()
        {
            mainLoop = null;
            currentState = null;
            overrideNextState = null;
            nextState = null;
            BlockingAnimationIsPlaying = false;
        }

        protected virtual IEnumerator Setup()
        {
            //wait for the references to be ready in the playmaker fsm
            yield return ExtractReferencesFromExternalSources();

            RemoveDeprecatedComponents();

            Running = true;
                        
            currentState = Init();
            nextState = null;

            mainLoop = MainAILoop();
            StartCoroutine(mainLoop);

            yield break;
        }

        protected virtual void Update()
        {
            if(!Running && mainLoop != null)
            {
                overrideNextState = null;
                nextState = null;
                if(currentState != null)
                    StopCoroutine(currentState);
                if(mainLoop != null)
                    StopCoroutine(mainLoop);
            }
        }

        protected virtual void OnDisable()
        {
            Running = false;
        }

        protected virtual IEnumerator MainAILoop()
        {
            Dev.Where();

            for(;;)
            {
                if(!Running)
                    break;

                yield return currentState;

                if(nextState != null)
                {
                    //TODO: remove as the states get implemented
                    //Dev.Log( "State Complete - Hit N to advance" );
                    //while( !Input.GetKeyDown( KeyCode.N ) )
                    //{
                    //    yield return new WaitForEndOfFrame();
                    //}
                    //Dev.Log( "Next" );
                    if(overrideNextState != null)
                    {
                        nextState = overrideNextState;
                        overrideNextState = null;
                    }

                    currentState = nextState();
                    nextState = null;
                }
            }

            yield break;
        }

        protected virtual IEnumerator Init()
        {
            Dev.Where();

            yield break;
        }        

        //Setup functions///////////////////////////////////////////////////////////////////////////
        
        protected virtual void SetupRequiredReferences()
        {
        }

        protected virtual IEnumerator ExtractReferencesFromExternalSources()
        {
            yield break;
        }

        protected virtual void RemoveDeprecatedComponents()
        {
            foreach( PlayMakerFSM p in gameObject.GetComponentsInChildren<PlayMakerFSM>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerUnity2DProxy p in gameObject.GetComponentsInChildren<PlayMakerUnity2DProxy>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFixedUpdate p in gameObject.GetComponentsInChildren<PlayMakerFixedUpdate>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( DeactivateIfPlayerdataTrue p in gameObject.GetComponentsInChildren<DeactivateIfPlayerdataTrue>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTweenFSMEvents p in gameObject.GetComponentsInChildren<iTweenFSMEvents>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in gameObject.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in gameObject.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
        }
        //end helpers /////////////////////////////
    }//end class
}//end namespace
