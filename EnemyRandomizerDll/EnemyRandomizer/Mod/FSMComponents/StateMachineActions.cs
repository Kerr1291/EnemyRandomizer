using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using nv;
using EnemyRandomizerMod.Extensions;

namespace EnemyRandomizerMod
{
    public abstract partial class GameStateMachine
    {
        protected struct SpawnRandomObjectsV2Data
        {
            public GameObject gameObject;
            public GameObject spawnPoint;
            public Vector3? position;
            public int? spawnMin;
            public int? spawnMax;
            public float? speedMin;
            public float? speedMax;
            public float? angleMin;
            public float? angleMax;
            public float? originVariationX;
            public float? originVariationY;
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

        protected static void PlayAnimation(tk2dSpriteAnimator tk2dAnimator, string animation, int frame)
        {
            Dev.Where();

            if(tk2dAnimator.GetClipByName(animation) == null)
            {
                Dev.Log("Warning: " + animation + " clip not found");
                return;
            }

            tk2dAnimator.AnimationCompleted = null;
            tk2dAnimator.PlayFromFrame(tk2dAnimator.GetClipByName(animation), frame);
        }

        protected static void PlayAnimation(tk2dSpriteAnimator tk2dAnimator, string animation)
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

        static protected void PlayOneShot(AudioSource source, AudioClip clip, float volume = 1f)
        {
            if(source != null && clip != null)
            {
                source.PlayOneShot(clip, volume);
            }
            else
            {
                if(source == null)
                    Dev.Log("Audio source is null! Cannot play sounds.");
                if(clip == null)
                    Dev.Log("Audio clip is null! Cannot play sound.");
            }
        }

        static protected void PlayOneShotRandom(AudioSource source, List<AudioClip> clip)
        {
            if(source != null && clip != null && clip.Count > 0)
            {
                AudioClip randomClip = clip.GetRandomElementFromList();
                source.PlayOneShot(randomClip);
            }
            else
            {
                if(source == null)
                    Dev.Log("Audio source is null! Cannot play sounds.");
                if(clip == null)
                    Dev.Log("Audio clip is null! Cannot play sound.");
                if(clip != null && clip.Count <= 0)
                    Dev.Log("Audio clip list is empty! No sounds to play.");
            }
        }

        static protected void ShowBossTitle(MonoBehaviour owner, GameObject areaTitleObject, float hideInSeconds, string largeMain = "", string largeSuper = "", string largeSub = "", string smallMain = "", string smallSuper = "", string smallSub = "")
        {
            //show hornet title
            if(areaTitleObject != null)
            {
                areaTitleObject.SetActive(true);
                foreach(FadeGroup f in areaTitleObject.GetComponentsInChildren<FadeGroup>())
                {
                    f.FadeUp();
                }

                //TODO: add an offset to the positions and separate this into 2 functions, one for the big title and one for the small title
                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Main").GetComponent<Transform>().Translate(new Vector3(4f, 0f, 0f));
                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Sub").GetComponent<Transform>().Translate(new Vector3(4f, 0f, 0f));
                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Super").GetComponent<Transform>().Translate(new Vector3(4f, 0f, 0f));

                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Main").GetComponent<TMPro.TextMeshPro>().text = smallMain;
                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Sub").GetComponent<TMPro.TextMeshPro>().text = smallSub;
                areaTitleObject.FindGameObjectInChildrenWithName("Title Small Super").GetComponent<TMPro.TextMeshPro>().text = smallSuper;

                areaTitleObject.FindGameObjectInChildrenWithName("Title Large Main").GetComponent<TMPro.TextMeshPro>().text = largeMain;
                areaTitleObject.FindGameObjectInChildrenWithName("Title Large Sub").GetComponent<TMPro.TextMeshPro>().text = largeSub;
                areaTitleObject.FindGameObjectInChildrenWithName("Title Large Super").GetComponent<TMPro.TextMeshPro>().text = largeSuper;

                if(hideInSeconds > 0f)
                {
                    //give it 3 seconds to fade in
                    owner.StartCoroutine(GameStateMachine.HideBossTitleAfter(areaTitleObject, hideInSeconds + 3f));
                }
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot show the boss title.");
            }
        }

        static protected IEnumerator HideBossTitleAfter(GameObject areaTitleObject, float time)
        {
            yield return new WaitForSeconds(time);
            HideBossTitle(areaTitleObject);
            yield return new WaitForSeconds(3f);
            areaTitleObject.SetActive(false);
        }

        static protected void HideBossTitle(GameObject areaTitleObject)
        {
            //show title
            if(areaTitleObject != null && areaTitleObject.activeInHierarchy)
            {
                foreach(FadeGroup f in areaTitleObject.GetComponentsInChildren<FadeGroup>())
                {
                    f.FadeDown();
                }
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot hide the boss title.");
            }
        }

        //Taken from Playmaker's SpawnRandomObjectsV2.cs FSM Action
        static protected void DoSpawnRandomObjectsV2(Rigidbody2D body, SpawnRandomObjectsV2Data data)
        {
            try
            {
                float vectorX = 0f;
                float vectorY = 0f;
                bool originAdjusted = false;
                GameObject value = data.gameObject;
                if(value != null)
                {
                    Vector3 a = Vector3.zero;
                    Vector3 zero = Vector3.zero;
                    if(data.spawnPoint != null)
                    {
                        a = data.spawnPoint.transform.position;
                        if(data.position.HasValue)
                        {
                            a += data.position.Value;
                        }
                    }
                    else if(data.position.HasValue)
                    {
                        a = data.position.Value;
                    }
                    int num = UnityEngine.Random.Range(data.spawnMin.Value, data.spawnMax.Value + 1);
                    for(int i = 1; i <= num; i++)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(value, a, Quaternion.Euler(zero));
                        float x = gameObject.transform.position.x;
                        float y = gameObject.transform.position.y;
                        float z = gameObject.transform.position.z;
                        if(data.originVariationX != null && data.originVariationX.HasValue)
                        {
                            x = gameObject.transform.position.x + UnityEngine.Random.Range(-data.originVariationX.Value, data.originVariationX.Value);
                            originAdjusted = true;
                        }
                        if(data.originVariationY != null && data.originVariationY.HasValue)
                        {
                            y = gameObject.transform.position.y + UnityEngine.Random.Range(-data.originVariationY.Value, data.originVariationY.Value);
                            originAdjusted = true;
                        }
                        if(originAdjusted)
                        {
                            gameObject.transform.position = new Vector3(x, y, z);
                        }
                        float num2 = UnityEngine.Random.Range(data.speedMin.Value, data.speedMax.Value);
                        float num3 = UnityEngine.Random.Range(data.angleMin.Value, data.angleMax.Value);
                        vectorX = num2 * Mathf.Cos(num3 * 0.0174532924f);
                        vectorY = num2 * Mathf.Sin(num3 * 0.0174532924f);
                        Vector2 velocity;
                        velocity.x = vectorX;
                        velocity.y = vectorY;
                        body.velocity = velocity;
                    }
                }
            }
            catch(Exception e)
            {
                Dev.Log("Exception: " + e.Source + " " + e.Message + " " + e.StackTrace);
            }
        }

        protected static void SpriteFaceObjectX(GameObject gameObject, GameObject target, bool rightIsNegative = true)
        {
            //by default we'll set it to facing right
            float previousScale = gameObject.transform.localScale.x;
            float facing = (rightIsNegative ? -1f : 1f);

            //then check if we should face left
            if(target.transform.position.x < gameObject.transform.position.x)
                facing = -facing;

            //preserve the scale, flip it if needed, update the transform
            float newX = Mathf.Abs(previousScale) * facing;
            gameObject.transform.localScale = gameObject.transform.localScale.SetX(newX);
        }

        protected static IEnumerator GetAudioPlayRandomClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayRandomLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var audioPlayRandom = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayRandom>(stateName, fsmName);

            //this is a prefab
            var clips = audioPlayRandom.audioClips.ToList();

            //send the clips out
            onAudioPlayRandomLoaded(clips);

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetAudioPlayerOneShotClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayerOneShotLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var audioPlayerOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>(stateName, fsmName);

            //this is a prefab
            var clips = audioPlayerOneShot.audioClips.ToList();

            //send the clips out
            onAudioPlayerOneShotLoaded(clips);
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }


        protected static IEnumerator GetGameObjectFromSpawnObjectFromGlobalPoolInFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfPrefab)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var spawnObjectFromGlobalPool = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool>(stateName, fsmName);

            //this is a prefab
            var prefab = spawnObjectFromGlobalPool.gameObject.Value;

            if(returnCopyOfPrefab)
            {
                //so spawn one
                var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

                //send the loaded object out
                onGameObjectLoaded(spawnedCopy);
            }
            else
            {
                onGameObjectLoaded(prefab);
            }
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetGameObjectsFromSpawnRandomObjectsV2InFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, int? actionIndex = null)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var spawnRandomObjectsV2 = copy.GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SpawnRandomObjectsV2>(stateName, fsmName);

            var action = spawnRandomObjectsV2[0];

            if(actionIndex != null && actionIndex.HasValue)
            {
                action = spawnRandomObjectsV2[actionIndex.Value];
            }

            //get the game object
            var prefab = action.gameObject.Value;

            onGameObjectLoaded(prefab);
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetGameObjectFromCreateObjectInFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfPrefab)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var createObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.CreateObject>(stateName, fsmName);

            //this is a prefab
            var prefab = createObject.gameObject.Value;

            if(returnCopyOfPrefab)
            {
                //so spawn one
                var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

                //send the loaded object out
                onGameObjectLoaded(spawnedCopy);
            }
            else
            {
                onGameObjectLoaded(prefab);
            }
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetGameObjectFromFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var setGameObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetGameObject>(stateName, fsmName);

            //this is a prefab
            var prefab = setGameObject.gameObject.Value;

            //so spawn one
            var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

            //send the loaded object out
            onGameObjectLoaded(spawnedCopy);
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }


        protected static IEnumerator GetGameObjectFromSendEvent(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded, bool returnCopyOfObject)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var sendEventByName = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SendEventByName>(stateName, fsmName);

            //this might be a prefab
            if(returnCopyOfObject)
            {
                var prefab = sendEventByName.eventTarget.gameObject.GameObject.Value;

                //so spawn one
                var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

                //send the loaded object out
                onGameObjectLoaded(spawnedCopy);
            }
            else
            {
                var foundGO = sendEventByName.eventTarget.gameObject.GameObject.Value;

                onGameObjectLoaded(foundGO);
            }
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetAudioSourceFromAudioPlayerOneShotSingleInFSM(GameObject go, string fsmName, string stateName, Action<AudioSource> onSourceLoaded)
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
            var audioOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(stateName, fsmName);

            //this is a prefab
            var aPlayer = audioOneShot.audioPlayer.Value;

            //so spawn one
            var spawnedCopy = GameObject.Instantiate(aPlayer) as GameObject;

            var audioSource = spawnedCopy.GetComponent<AudioSource>();

            var recycleComponent = audioSource.GetComponent<PlayAudioAndRecycle>();

            //stop it from killing itself
            if(recycleComponent != null)
                GameObject.DestroyImmediate(recycleComponent);

            //send the loaded object out
            onSourceLoaded(audioSource);

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetAudioClipFromAudioPlaySimpleInFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            var audioPlaySimple = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlaySimple>(stateName, fsmName);

            var clip = audioPlaySimple.oneShotClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        protected static IEnumerator GetAudioClipFromAudioPlayerOneShotSingleInFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded, int? actionIndex = null)
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
            var audioOneShot = copy.GetFSMActionsOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(stateName, fsmName);

            var action = audioOneShot[0];

            if(actionIndex != null && actionIndex.HasValue)
            {
                action = audioOneShot[actionIndex.Value];
            }

            var clip = action.audioClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }


        protected static UnityEngine.Audio.AudioMixerSnapshot GetSnapshotFromTransitionToAudioSnapshotInFSM(GameObject go, string fsmName, string stateName)
        {
            return GetValueFromAction<UnityEngine.Audio.AudioMixerSnapshot, TransitionToAudioSnapshot>(go, fsmName, stateName, "snapshot");
        }

        protected static MusicCue GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
            return GetValueFromAction<MusicCue, ApplyMusicCue>(go, fsmName, stateName, "musicCue");
        }

        /// <summary>
        /// Get the exact value from the action in the state in the fsm. Use the index parameter if there's more than one of the same action in the state. Returns the found value.
        /// Use this to get values that do not require the unity game object to be active.
        /// </summary>
        protected static T GetValueFromAction<T, U>(GameObject go, string fsmName, string stateName, string valueName, int? actionIndex = null)
            where U : FsmStateAction
        {
            List<U> actions = go.GetFSMActionsOnState<U>(stateName, fsmName);

            if(actions == null)
            {
                Dev.Log("Warning: No actions of type " + typeof(U).GetType().Name + " found on state " + stateName + " in fsm " + fsmName);
                return default(T);
            }

            U action = actions[0];

            if(actionIndex != null && actionIndex.HasValue)
            {
                action = actions[actionIndex.Value];
            }

            FieldInfo fi = action.GetType().GetField(valueName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if(fi == null)
            {
                Dev.Log(valueName + " not found on action " + action.GetType().Name + " in state " + stateName + " in fsm " + fsmName);

                Dev.Log("Valid valueName's on action: ");

                foreach(FieldInfo f in action.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    Dev.Log(f.Name);
                }

                return default(T);
            }

            object fieldValue = fi.GetValue(action);

            T realValue;

            if(fieldValue as FsmObject != null)
            {
                object val = (fieldValue as FsmObject).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmRect != null)
            {
                object val = (fieldValue as FsmRect).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmColor != null)
            {
                object val = (fieldValue as FsmColor).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmEnum != null)
            {
                object val = (fieldValue as FsmEnum).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmArray != null)
            {
                object val = (fieldValue as FsmArray).Values;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmBool != null)
            {
                object val = (fieldValue as FsmBool).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmFloat != null)
            {
                object val = (fieldValue as FsmFloat).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmInt != null)
            {
                object val = (fieldValue as FsmInt).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmMaterial != null)
            {
                object val = (fieldValue as FsmMaterial).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmQuaternion != null)
            {
                object val = (fieldValue as FsmQuaternion).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmString != null)
            {
                object val = (fieldValue as FsmString).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmTexture != null)
            {
                object val = (fieldValue as FsmTexture).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmVector2 != null)
            {
                object val = (fieldValue as FsmVector2).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmVector3 != null)
            {
                object val = (fieldValue as FsmVector3).Value;
                realValue = (T)(val);
            }
            else if(fieldValue as FsmOwnerDefault != null)
            {
                object val = (fieldValue as FsmOwnerDefault).GameObject.Value;
                realValue = (T)(val);
            }
            else if(fieldValue as NamedVariable != null)
            {
                object val = (fieldValue as NamedVariable).RawValue;
                realValue = (T)(val);
            }
            else
            {
                realValue = (T)(fieldValue);
            }


            return realValue;
        }

        /// <summary>
        /// Get the exact value from the action in the state in the fsm. Use the index parameter if there's more than one of the same action in the state. Returns the found value to onValueLoaded.
        /// Use this to get values that require the unity game object to be active. (Example: an AudioClip)
        /// </summary>
        protected static IEnumerator GetValueFromAction<T, U>(GameObject go, string fsmName, string stateName, string valueName, Action<T> onValueLoaded, int? actionIndex = null)
            where U : FsmStateAction
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();

            T realValue = GetValueFromAction<T, U>(copy, fsmName, stateName, valueName, actionIndex);

            //send the loaded value out
            onValueLoaded(realValue);

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        /// <summary>
        /// Get the exact value from the action in the state in the fsm. Use the index parameter if there's more than one of the same action in the state. Returns the found value and the unique name to onValueLoaded.
        /// The callback number recieves the name in this format: fsmName + stateName + actionTypeName + valueName + (if not null) actionIndex
        /// Example: For The FSM "Control", The state "Blow" the action "SpawnRandomObjectsV2" the valueName "gameObject" and actionIndex as null, the name will be ControlBlowSpawnRandomObjectsV2gameObject
        /// </summary>
        protected static IEnumerator GetValueFromAction<T, U>(GameObject go, string fsmName, string stateName, string valueName, Action<T, string> onValueLoaded, int? actionIndex = null)
            where U : FsmStateAction
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();

            T realValue = GetValueFromAction<T, U>(copy, fsmName, stateName, valueName, actionIndex);

            string uniqueName = fsmName + stateName + typeof(U).GetType().Name + valueName;
            if(actionIndex.HasValue)
            {
                uniqueName += actionIndex.Value;
            }

            //send the loaded value out
            onValueLoaded(realValue, uniqueName);

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        /// <summary>
        /// This one is kinda fancy. Create an output structure with variable names that exactly match the names of variables in the fsm state.
        /// Then, use a callback function that takes that output structure and this method will attempt to fill all the fields with data from that fsm action.
        /// The callback number recieves the name in this format: fsmName + stateName + actionTypeName + (if not null) actionIndex
        /// Example: For The FSM "Control", The state "Blow" the action "SpawnRandomObjectsV2" and actionIndex as null, the name will be ControlBlowSpawnRandomObjectsV2
        /// </summary>
        protected static IEnumerator GetDataFromAction<T, U>(GameObject go, string fsmName, string stateName, Action<T, string> onDataLoaded, int? actionIndex = null)
            where U : FsmStateAction
        {
            GameObject copy = go;
            if(!go.activeInHierarchy)
            {
                copy = GameObject.Instantiate(go) as GameObject;
                copy.SetActive(true);
            }

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();

            //get the fields on our output structure
            FieldInfo[] fields = typeof(T).GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            T output = Activator.CreateInstance<T>();

            //use the field names to get all the requested data from this action
            foreach(FieldInfo f in fields)
            {
                Type fType = f.FieldType;

                //we'll use our generic method with a return value matching the field's type
                //so we're calling this:
                //GetValueFromAction<fType, U>(copy, fsmName, stateName, f.FieldType.Name, actionIndex)
                MethodInfo getValueFromAction = typeof(GameStateMachine).GetMethod("GetValueFromAction"
                    , new Type[] { typeof(GameObject), typeof(string), typeof(string), typeof(string), typeof(int?) });
                getValueFromAction = getValueFromAction.MakeGenericMethod(new Type[] { fType, typeof(U) });

                //then set the value on our output structure
                f.SetValue(output, getValueFromAction.Invoke(copy, new object[] { copy, fsmName, stateName, f.FieldType.Name, actionIndex }));
            }

            string uniqueName = fsmName + stateName + typeof(U).GetType().Name;
            if(actionIndex.HasValue)
            {
                uniqueName += actionIndex.Value;
            }

            //send the loaded value out
            onDataLoaded(output, uniqueName);

            if(copy != go)
                GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }       
        //end helpers /////////////////////////////
    }//end class
}//end namespace
