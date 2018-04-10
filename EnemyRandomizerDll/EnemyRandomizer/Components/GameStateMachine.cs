using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using nv.Tests;
#endif

namespace nv
{
    public abstract class GameStateMachine : MonoBehaviour
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

        protected static void DoCameraEffect(string effectName)
        {
            //NOTE: taken from the game's health manager as a nice way to get the game camera
            GameCameras.instance.cameraShakeFSM.SendEvent(effectName);
        }

        protected static void DoEnemyKillShakeEffect()
        {
            DoCameraEffect("EnemyKillShake");
        }

        protected static void SendEventToFirstFSM(GameObject gameObject, string eventName)
        {
            if(gameObject == null)
                return;

            PlayMakerFSM fsm = gameObject.GetComponent<PlayMakerFSM>();
            if(fsm != null)
            {
                fsm.SendEvent(eventName);
            }
        }

        protected static void SendEventToAllFSMsOn(GameObject gameObject, string eventName)
        {
            if(gameObject == null)
                return;

            PlayMakerFSM[] fsms = gameObject.GetComponents<PlayMakerFSM>();
            foreach(var fsm in fsms)
            {
                fsm.SendEvent(eventName);
            }
        }

        protected static void SendEventToFSM(GameObject gameObject, string fsmName, string eventName)
        {
            if(gameObject == null)
                return;

            PlayMakerFSM fsm = FSMUtility.LocateFSM(gameObject, fsmName);
            if(fsm != null)
            {
                fsm.SendEvent(eventName);
            }
        }

        protected static void SendEventToFSM(string gameObjectName, string fsmName, string eventName)
        {
            SendEventToFSM(GameObject.Find(gameObjectName), fsmName, eventName);
        }

        protected static void PlayBossMusic(UnityEngine.Audio.AudioMixerSnapshot mixerSnapshot, MusicCue musicCue)
        {
            //set the audio mixer snapshot
            if(mixerSnapshot != null)
            {
                mixerSnapshot.TransitionTo(1f);
            }

            // play the boss music music
            GameManager instance = GameManager.instance;
            instance.AudioManager.ApplyMusicCue(musicCue, 0f, 0f, false);
        }

        protected static void FlipScale(GameObject owner)
        {
            owner.transform.localScale = owner.transform.localScale.SetX(-owner.transform.localScale.x);
        }

        protected virtual void PlayAnimation(tk2dSpriteAnimator tk2dAnimator, string animation)
        {
            Dev.Where();

            if(tk2dAnimator.GetClipByName(animation) == null)
            {
                Dev.Log("Warning: " + animation + " clip not found");
                return;
            }

            tk2dAnimator.AnimationCompleted = null;
            tk2dAnimator.Play(animation);
        }

        protected virtual IEnumerator PlayAndWaitForEndOfAnimation(tk2dSpriteAnimator tk2dAnimator, string animation, Action doWhileWaiting = null)
        {
            Dev.Where();

            if(tk2dAnimator.GetClipByName(animation) == null)
            {
                Dev.Log("Warning: " + animation + " clip not found");
                yield break;
            }

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play(animation);

            BlockingAnimationIsPlaying = true;

            while(BlockingAnimationIsPlaying)
            {
                if(doWhileWaiting != null)
                    doWhileWaiting.Invoke();

                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        protected virtual void OnAnimationComplete(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
        {
            Dev.Where();
            BlockingAnimationIsPlaying = false;

            if(sprite != null)
                sprite.AnimationCompleted = null;
        }

        static public void PlayOneShot(AudioSource source, AudioClip clip)
        {
            if(source != null && clip != null)
            {
                source.PlayOneShot(clip);
            }
        }

        static public void PlayOneShotRandom(AudioSource source, List<AudioClip> clip)
        {
            if(source != null && clip != null && clip.Count > 0)
            {
                AudioClip randomClip = clip.GetRandomElementFromList();
                source.PlayOneShot(randomClip);
            }
        }

        static public void ShowBossTitle(MonoBehaviour owner, GameObject areaTitleObject, float hideInSeconds, string largeMain = "", string largeSuper = "", string largeSub = "", string smallMain = "", string smallSuper = "", string smallSub = "")
        {
            //show hornet title
            if(areaTitleObject != null)
            {
#if UNITY_EDITOR
#else
                areaTitleObject.SetActive( true );
                foreach( FadeGroup f in areaTitleObject.GetComponentsInChildren<FadeGroup>() )
                {
                    f.FadeUp();
                }

                //TODO: add an offset to the positions and separate this into 2 functions, one for the big title and one for the small title
                areaTitleObject.FindGameObjectInChildren( "Title Small Main" ).GetComponent<Transform>().Translate( new Vector3( 4f, 0f, 0f ) );
                areaTitleObject.FindGameObjectInChildren( "Title Small Sub" ).GetComponent<Transform>().Translate( new Vector3( 4f, 0f, 0f ) );
                areaTitleObject.FindGameObjectInChildren( "Title Small Super" ).GetComponent<Transform>().Translate( new Vector3( 4f, 0f, 0f ) );

                areaTitleObject.FindGameObjectInChildren( "Title Small Main" ).GetComponent<TMPro.TextMeshPro>().text = smallMain;
                areaTitleObject.FindGameObjectInChildren( "Title Small Sub" ).GetComponent<TMPro.TextMeshPro>().text = smallSub;
                areaTitleObject.FindGameObjectInChildren( "Title Small Super" ).GetComponent<TMPro.TextMeshPro>().text = smallSuper;

                areaTitleObject.FindGameObjectInChildren( "Title Large Main" ).GetComponent<TMPro.TextMeshPro>().text = largeMain;
                areaTitleObject.FindGameObjectInChildren( "Title Large Sub" ).GetComponent<TMPro.TextMeshPro>().text = largeSub;
                areaTitleObject.FindGameObjectInChildren( "Title Large Super" ).GetComponent<TMPro.TextMeshPro>().text = largeSuper;

                if( hideInSeconds > 0f )
                {
                    //give it 3 seconds to fade in
                    owner.StartCoroutine( GameStateMachine.HideBossTitleAfter( areaTitleObject, hideInSeconds + 3f ) );
                }
#endif
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot show the boss title.");
            }
        }

        static public IEnumerator HideBossTitleAfter(GameObject areaTitleObject, float time)
        {
            yield return new WaitForSeconds(time);
            HideBossTitle(areaTitleObject);
            yield return new WaitForSeconds(3f);
            areaTitleObject.SetActive(false);
        }

        static public void HideBossTitle(GameObject areaTitleObject)
        {
            //show title
            if(areaTitleObject != null && areaTitleObject.activeInHierarchy)
            {
#if UNITY_EDITOR
#else
                foreach( FadeGroup f in areaTitleObject.GetComponentsInChildren<FadeGroup>() )
                {
                    f.FadeDown();
                }
#endif
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot hide the boss title.");
            }
        }        

        public static IEnumerator GetAudioPlayRandomClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayRandomLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onAudioPlayRandomLoaded(null);
#else
            var audioPlayRandom = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayRandom>(stateName, fsmName);

            //this is a prefab
            var clips = audioPlayRandom.audioClips.ToList();

            //send the clips out
            onAudioPlayRandomLoaded( clips );
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioPlayerOneShotClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayerOneShotLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onAudioPlayerOneShotLoaded(null);
#else
            var audioPlayerOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>(stateName, fsmName);
            
            //this is a prefab
            var clips = audioPlayerOneShot.audioClips.ToList();
            
            //send the clips out
            onAudioPlayerOneShotLoaded(clips);
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }


        public static IEnumerator GetGameObjectFromSpawnObjectFromGlobalPoolInFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfPrefab)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var spawnObjectFromGlobalPool = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>(stateName, fsmName);

            //this is a prefab
            var prefab = spawnObjectFromGlobalPool.gameObject.Value;

            if( returnCopyOfPrefab )
            {
                //so spawn one
                var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

                //send the loaded object out
                onGameObjectLoaded( spawnedCopy );
            }
            else
            {
                onGameObjectLoaded( prefab );
            }
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetGameObjectsFromSpawnRandomObjectsV2InFSM( GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded )
        {
            GameObject copy = go;
            if( !go.activeInHierarchy )
            {
                copy = GameObject.Instantiate( go ) as GameObject;
                copy.SetActive( true );
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var spawnRandomObjectsV2 = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SpawnRandomObjectsV2>( stateName, fsmName );

            //this is a prefab
            var prefab = spawnRandomObjectsV2.gameObject.Value;

            onGameObjectLoaded( prefab );
#endif
            if( copy != go )
                GameObject.Destroy( copy );

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetGameObjectFromCreateObjectInFSM( GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfPrefab )
        {
            GameObject copy = go;
            if( !go.activeInHierarchy )
            {
                copy = GameObject.Instantiate( go ) as GameObject;
                copy.SetActive( true );
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var createObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.CreateObject>( stateName, fsmName );

            //this is a prefab
            var prefab = createObject.gameObject.Value;

            if( returnCopyOfPrefab )
            {
                //so spawn one
                var spawnedCopy = GameObject.Instantiate( prefab ) as GameObject;

                //send the loaded object out
                onGameObjectLoaded( spawnedCopy );
            }
            else
            {
                onGameObjectLoaded( prefab );
            }
#endif
            if( copy != go )
                GameObject.Destroy( copy );

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetGameObjectFromFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var setGameObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetGameObject>(stateName, fsmName);            

            //this is a prefab
            var prefab = setGameObject.gameObject.Value;
            
            //so spawn one
            var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

            //send the loaded object out
            onGameObjectLoaded(spawnedCopy);
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }


        public static IEnumerator GetGameObjectFromSendEvent( GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfObject )
        {
            GameObject copy = go;
            if( !go.activeInHierarchy )
            {
                copy = GameObject.Instantiate( go ) as GameObject;
                copy.SetActive( true );
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var sendEventByName = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SendEventByName>(stateName, fsmName);

            //this might be a prefab
            if( returnCopyOfObject )
            {
                var prefab = sendEventByName.eventTarget.gameObject.GameObject.Value;

                //so spawn one
                var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

                //send the loaded object out
                onGameObjectLoaded( spawnedCopy );
            }
            else
            {
                var foundGO = sendEventByName.eventTarget.gameObject.GameObject.Value;

                onGameObjectLoaded( foundGO );
            }
#endif
            if( copy != go )
                GameObject.Destroy( copy );

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioSourceObjectFromFSM(GameObject go, string fsmName, string stateName, Action<AudioSource> onSourceLoaded)
        {
            Dev.Where();
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onSourceLoaded(null);
#else
            var audioOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(stateName, fsmName);
            
            //this is a prefab
            var aPlayer = audioOneShot.audioPlayer.Value;
            
            //so spawn one
            var spawnedCopy = GameObject.Instantiate(aPlayer) as GameObject;

            var audioSource = spawnedCopy.GetComponent<AudioSource>();

            var recycleComponent = audioSource.GetComponent<PlayAudioAndRecycle>();

            //stop it from killing itself
            if( recycleComponent != null )
                GameObject.DestroyImmediate( recycleComponent );

            //send the loaded object out
            onSourceLoaded(audioSource);
#endif

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioClipFromAudioPlaySimpleInFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onClipLoaded(null);
#else
            var audioPlaySimple = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlaySimple>(stateName, fsmName);

            var clip = audioPlaySimple.oneShotClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioClipFromFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            Dev.Where();
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onClipLoaded(null);
#else
            var audioOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(stateName, fsmName);
            
            var clip = audioOneShot.audioClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
#endif
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        
        public static UnityEngine.Audio.AudioMixerSnapshot GetSnapshotFromFSM(GameObject go, string fsmName, string stateName)
        {
#if UNITY_EDITOR
            return null;
#else
            var snapshot = go.GetFSMActionOnState<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>(stateName, fsmName);
            var mixerSnapshot = snapshot.snapshot.Value as UnityEngine.Audio.AudioMixerSnapshot;
            return mixerSnapshot;
#endif
        }

        public static MusicCue GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
#if UNITY_EDITOR
            return null;
#else
            var musicCue = go.GetFSMActionOnState<HutongGames.PlayMaker.Actions.ApplyMusicCue>(stateName, fsmName);
            MusicCue mc = musicCue.musicCue.Value as MusicCue;
            return mc;
#endif
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
#if UNITY_EDITOR
#else
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
#endif
        }

        //end helpers /////////////////////////////
    }//end class
}//end namespace
