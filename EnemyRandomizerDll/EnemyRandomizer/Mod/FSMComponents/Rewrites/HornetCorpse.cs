using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using nv;

namespace EnemyRandomizerMod
{
    public class HornetCorpse : Physics2DSM
    {
        public HornetBoss owner;
        
        public MeshRenderer meshRenderer;
        public tk2dSpriteAnimator tk2dAnimator;
        public ParticleSystem grassEscape;
        public tk2dSpriteAnimator leaveAnim;
        public GameObject startPt;
        public ParticleSystem grass;
        public tk2dSpriteAnimator thread;

        public UnityEngine.Audio.AudioMixerSnapshot audioSnapshot;
        public Dictionary<string, AudioClip> audioClips;
        public Dictionary<string, GameObject> gameObjects;
        public Dictionary<string, ParticleSystem> particleSystems;
        protected Dictionary<string, SpawnRandomObjectsV2Data> spawnRandomObjectsV2Data;

        //use for some sound effects
        public AudioSource actorAudioSource;

        public float xLaunchVelocity = 5f;
        public float yLaunchVelocity = 20f;
        public float woundedWaitTime = 3f;
        public float hornetLeaveJumpWaitTime = .415f;
        public float escapeSpeed = 40f;

        string objectToDestroyName;
        float escapeRotation = 0f;
        RaycastHit2D escapeRoute;

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
            meshRenderer = GetComponent<MeshRenderer>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            audioClips = new Dictionary<string, AudioClip>();
            gameObjects = new Dictionary<string, GameObject>();
            particleSystems = new Dictionary<string, ParticleSystem>();
            spawnRandomObjectsV2Data = new Dictionary<string, SpawnRandomObjectsV2Data>();
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            if(collision.gameObject.layer == collisionLayer)
            {
                CheckTouching(collisionLayer);
            }
        }        

        protected override IEnumerator Init()
        {
            yield return base.Init();

            nextState = LimitPos;

            yield break;
        }

        protected virtual IEnumerator LimitPos()
        {
            //Not sure what this is for, seems to place hornet's corpse in the right spot on startup?
            nextState = SetPD;
            yield break;
        }

        protected virtual IEnumerator SetPD()
        {
            if(owner.checkPlayerData)
            {
                GameManager.instance.playerData.SetBool("hornet1Defeated", true);
                GameManager.instance.AwardAchievement("HORNET_1");
            }
            yield break;
        }

        protected virtual IEnumerator Blow()
        {
            if(audioSnapshot != null)
            {
                audioSnapshot.TransitionTo(1f);
            }

            PlayOneShot(audioClips["Hornet_Fight_Death_01"]);
            PlayOneShot(audioClips["boss_explode_clean"]);

            GameObject objectToDestroy = GameObject.Find(objectToDestroyName);
            if(objectToDestroy != null)
            {
                Destroy(objectToDestroy);
            }
            else
            {
                Dev.LogWarning(objectToDestroy + " is null! Cannot destroy!");
            }

            SpawnRandomObjectsV2Data data0 = spawnRandomObjectsV2Data["ControlBlowSpawnRandomObjectsV2"];
            DoSpawnRandomObjectsV2(body, data0);

            SpawnRandomObjectsV2Data data1 = spawnRandomObjectsV2Data["ControlBlowSpawnRandomObjectsV21"];
            DoSpawnRandomObjectsV2(body, data1);

            GameObject.Instantiate<GameObject>(gameObjects["White Wave"], transform.position, Quaternion.identity);

            DoCameraEffect("AverageShake");

            nextState = Launch;
            yield break;
        }

        protected virtual IEnumerator Launch()
        {
            GameObject hero = HeroController.instance.gameObject;

            //set the corpse facing the proper way
            SpriteFaceObjectX(gameObject, hero, false);

            //launch away from our hero
            Vector2 launchVelocity = new Vector2(transform.localScale.x * xLaunchVelocity, yLaunchVelocity);
            body.velocity = launchVelocity;

            //make sure the corpse is oriented properly
            transform.rotation = Quaternion.identity;

            nextState = InAir;
            yield break;
        }

        protected virtual IEnumerator InAir()
        {
            //change collision check directions for the corpse, respond if the corpse flies into a wall or hits the ground
            EnableCollisionsInDirection(false, true, true, true);
            
            //keep the rotation straight? (not sure what the point of this line is in the state machine)
            transform.rotation = Quaternion.identity;

            while(!bottomHit && !rightHit && !leftHit )
            {
                yield return new WaitForEndOfFrame();
            }

            //once we've landed, restore the previous collision responses
            RestorePreviousCollisionDirections();

            nextState = Land;
            yield break;
        }

        protected virtual IEnumerator Land()
        {
            tk2dAnimator.Play("Wounded");

            body.velocity = Vector2.zero;

            yield return new WaitForSeconds(woundedWaitTime);
            
            //keep the rotation straight? (not sure what the point of this line is in the state machine)
            transform.rotation = Quaternion.identity;

            nextState = CheckPos;
            yield break;
        }

        protected virtual IEnumerator CheckPos()
        {
            //previously, this state checked a hard coded position value to see if hornet was on the left or right side of the boss arena.
            //instead, we're going to use a raycast and use the farther away point to determine which way we go

            Vector2 upLeft = new Vector2(-1f, 1f);
            Vector2 upRight = new Vector2(1f, 1f);

            RaycastHit2D? escapePath = GetRaycastWithMaxDistance(transform.position, upLeft, upRight);
            escapeRoute = escapePath.Value;

            //flip to face toward the escape
            float xDirection = Mathf.Sign(escapePath.Value.point.x - transform.position.x);
            transform.localScale = transform.localScale.SetX(xDirection);

            if(xDirection < 0f)
            {
                escapeRotation = 45f;
            }
            else
            {
                escapeRotation = -45f;
            }

            nextState = Jump;
            yield break;
        }

        protected virtual IEnumerator Jump()
        {            
            PlayOneShot(audioClips["Hornet_Fight_Stun_02"]);

            grass.Play();

            meshRenderer.enabled = false;

            leaveAnim.gameObject.SetActive(true);

            ClearPreviousCollisions();
            GetComponent<Collider2D>().enabled = false;

            body.isKinematic = true;

            leaveAnim.Play("Jump Full");

            //TODO: use iTweenMoveBy to jump into the air, in the meantime, I will create a temporary jump animation using smoothdamp
            float distanceToEscape = (startPt.transform.position - transform.position).magnitude;

            //calculate the new escape point farther away (inside the wall)
            Vector3 escapePoint = startPt.transform.position;
            Vector3 escapeVelocity = Vector3.zero;

            float waitTime = hornetLeaveJumpWaitTime;

            //smoothdamp to that position
            while(waitTime > 0f)
            {
                yield return new WaitForEndOfFrame();
                transform.position = Vector3.SmoothDamp(transform.position, escapePoint, ref escapeVelocity, hornetLeaveJumpWaitTime, escapeSpeed, Time.deltaTime);
                distanceToEscape = (escapePoint - transform.position).magnitude;
                waitTime -= Time.deltaTime;
            }

            nextState = ThrowStart;
            yield break;
        }

        protected virtual IEnumerator ThrowStart()
        {
            yield return PlayAndWaitForEndOfAnimation(leaveAnim, "Throw Side Start");

            nextState = Throw;
            yield break;
        }

        protected virtual IEnumerator Throw()
        {
            thread.gameObject.SetActive(true);

            PlayOneShot(audioClips["hornet_needle_thow_spin"]);

            leaveAnim.Play("Throw Side");

            yield return PlayAndWaitForEndOfAnimation(thread, "Thread 1");

            nextState = Yank;
            yield break;
        }

        protected virtual IEnumerator Yank()
        {
            leaveAnim.Play("Harpoon Side");

            PlayOneShot(audioClips["Hornet_Fight_Yell_03"]);

            thread.transform.SetParent(null);
            
            Vector3 escapeRotationVector = new Vector3(0f, 0f, escapeRotation);
            transform.Rotate(escapeRotationVector);

            thread.transform.SetParent(transform);

            PlayOneShot(audioClips["hornet_dash"], 1.15f);

            //TODO: Need to check iTweenMoveBy and see what it's doing... but in the meantime I'm just going to try and re-create the escape animation myself using smoothdamp
            
            //make hornet zoom away, move to state End when the movement is done
            //get the escape direction and add some distance one so the smoothdamp finishes inside the wall
            float distanceToEscape = escapeRoute.distance + 10f;

            //calculate the new escape point farther away (inside the wall)
            Vector3 escapePoint = (escapeRoute.point - (Vector2)transform.position).normalized * (distanceToEscape);
            Vector3 escapeVelocity = Vector3.zero;

            //smoothdamp to that position
            while(distanceToEscape > .5f)
            {
                yield return new WaitForEndOfFrame();
                transform.position = Vector3.SmoothDamp(transform.position, escapePoint, ref escapeVelocity, 1f, escapeSpeed, Time.deltaTime);
                distanceToEscape = (escapePoint - transform.position).magnitude;
            }

            nextState = End;
            yield break;
        }

        protected virtual IEnumerator End()
        {
            DoEnemyKillShakeEffect();

            grassEscape.transform.SetParent(null);

            grassEscape.transform.position = escapeRoute.point;
            grassEscape.transform.rotation = Quaternion.identity;

            grassEscape.Play();

            meshRenderer.enabled = false;
            thread.GetComponent<MeshRenderer>().enabled = false;
            leaveAnim.GetComponent<MeshRenderer>().enabled = false;

            //create a broadcast event
            HutongGames.PlayMaker.FsmEventTarget broadcastEvent = new HutongGames.PlayMaker.FsmEventTarget();
            broadcastEvent.target = HutongGames.PlayMaker.FsmEventTarget.EventTarget.BroadcastAll;
            broadcastEvent.excludeSelf = false;
            broadcastEvent.fsmName = "Control";
            broadcastEvent.sendToChildren = false;
            broadcastEvent.fsmComponent = null;
            
            //TODO: TEST, not sure if this will work
            GameManager.instance.GetComponent<PlayMakerFSM>().Fsm.Event(broadcastEvent, "HORNET LEAVE");

            //destroy this.. and our owner?
            Destroy(gameObject);
            Destroy(owner);

            //make sure the particle effect object is cleaned up
            Destroy(grassEscape, 5f);

            yield break;
        }

        protected virtual void SetFightGates(bool closed)
        {
            if(closed)
            {
                SendEventToFSM("Battle Gate A", "BG Control", "BG CLOSE");
                SendEventToFSM("Battle Gate A (1)", "BG Control", "BG CLOSE");
            }
            else
            {
                SendEventToFSM("Battle Gate A", "BG Control", "BG OPEN");
                SendEventToFSM("Battle Gate A (1)", "BG Control", "BG OPEN");
            }
        }

        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources();

            string bossFSMName = "Control";

            objectToDestroyName = GetValueFromAction<string, FindGameObject>(gameObject, bossFSMName, "Blow", "objectName");

            yield return GetGameObjectFromCreateObjectInFSM(gameObject, bossFSMName, "Blow", SetGameObject, false);//White Wave
            
            yield return GetDataFromAction<SpawnRandomObjectsV2Data, SpawnRandomObjectsV2>(gameObject, bossFSMName, "Blow", SetSpawnRandomObjectsV2DataWithName);   //ControlBlowSpawnRandomObjectsV2
            yield return GetDataFromAction<SpawnRandomObjectsV2Data, SpawnRandomObjectsV2>(gameObject, bossFSMName, "Blow", SetSpawnRandomObjectsV2DataWithName, 1); //ControlBlowSpawnRandomObjectsV21

            yield return GetAudioSourceFromAudioPlayerOneShotSingleInFSM(gameObject, bossFSMName, "Blow", SetActorAudioSource);
            yield return GetAudioClipFromAudioPlayerOneShotSingleInFSM(gameObject, bossFSMName, "Blow", SetAudioClip);//Hornet_Fight_Death_01
            yield return GetAudioClipFromAudioPlaySimpleInFSM(gameObject, bossFSMName, "Blow", SetAudioClip);//boss_explode_clean
            yield return GetAudioClipFromAudioPlayerOneShotSingleInFSM(gameObject, bossFSMName, "Jump", SetAudioClip);//Hornet_Fight_Stun_02
            yield return GetAudioClipFromAudioPlayerOneShotSingleInFSM(gameObject, bossFSMName, "Throw", SetAudioClip);//hornet_needle_thow_spin

            yield return GetValueFromAction<AudioClip, AudioPlayerOneShotSingle>(gameObject, bossFSMName, "Yank", "audioClip", SetAudioClip, 0);//Hornet_Fight_Yell_03
            yield return GetValueFromAction<AudioClip, AudioPlayerOneShotSingle>(gameObject, bossFSMName, "Yank", "audioClip", SetAudioClip, 1);//hornet_dash
            audioSnapshot = GetSnapshotFromTransitionToAudioSnapshotInFSM(gameObject, bossFSMName, "Blow");//Silent

            //load child references
            if(gameObject.FindGameObjectInChildrenWithName("Thread") != null)
                thread = gameObject.FindGameObjectInChildrenWithName("Thread").GetComponent<tk2dSpriteAnimator>();
            if(gameObject.FindGameObjectInChildrenWithName("Grass") != null)
                grass = gameObject.FindGameObjectInChildrenWithName("Grass").GetComponent<ParticleSystem>();
            if(gameObject.FindGameObjectInChildrenWithName("Start Pt") != null)
                startPt = gameObject.FindGameObjectInChildrenWithName("Start Pt");
            if(gameObject.FindGameObjectInChildrenWithName("Leave Anim") != null)
                leaveAnim = gameObject.FindGameObjectInChildrenWithName("Leave Anim").GetComponent<tk2dSpriteAnimator>();
            if(gameObject.FindGameObjectInChildrenWithName("Grass Escape") != null)
                grassEscape = gameObject.FindGameObjectInChildrenWithName("Grass Escape").GetComponent<ParticleSystem>();
        }

        //protected override void RemoveDeprecatedComponents()
        //{
        //    base.RemoveDeprecatedComponents();
        //}

        protected virtual void PlayOneShotRandom(List<AudioClip> clips)
        {
            PlayOneShotRandom(actorAudioSource, clips);
        }

        protected virtual void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            PlayOneShot(actorAudioSource, clip, volume);
        }

        protected virtual void PlayAnimation(string animation)
        {
            PlayAnimation(tk2dAnimator, animation);
        }

        protected virtual void PlayAnimation(string animation, int frame)
        {
            PlayAnimation(tk2dAnimator, animation, frame);
        }

        protected virtual void PlayAnimation(tk2dSpriteAnimationClip animation)
        {
            tk2dAnimator.Play(animation);
        }

        protected virtual IEnumerator PlayAndWaitForEndOfAnimation(string animation, Action doWhileWaiting = null)
        {
            yield return PlayAndWaitForEndOfAnimation(tk2dAnimator, animation, doWhileWaiting);
        }

        void SetGameObject(GameObject go)
        {
            if(go == null)
            {
                Dev.Log("Warning: prefab is null!");
                return;
            }

            Dev.Log("Added: " + go.name + " to gameObjects!");
            gameObjects.Add(go.name, go);
        }

        void SetGameObjectWithName(GameObject go, string uniqueName)
        {
            if(go == null)
            {
                Dev.Log("Warning: "+ uniqueName + " is null!");
                return;
            }

            Dev.Log("Added: " + uniqueName + " to gameObjects!");
            gameObjects.Add(uniqueName, go);
        }

        void SetSpawnRandomObjectsV2DataWithName(SpawnRandomObjectsV2Data data, string uniqueName)
        {
            if(data.gameObject == null)
            {
                Dev.Log("Warning: " + uniqueName + "'s prefab is null!");
                return;
            }

            Dev.Log("Added: " + uniqueName + " data to spawnRandomObjectsV2Data!");
            spawnRandomObjectsV2Data.Add(uniqueName, data);
        }

        void SetAudioClip(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: audio clip is null!");
                return;
            }

            Dev.Log("Added: " + clip.name + " to audioClips!");
            audioClips.Add(clip.name, clip);
        }

        void SetActorAudioSource(AudioSource source)
        {
            if(source == null)
            {
                Dev.Log("Warning: Actor AudioSource failed to load and is null!");
                return;
            }

            actorAudioSource = source;
            actorAudioSource.transform.SetParent(gameObject.transform);
            actorAudioSource.transform.localPosition = Vector3.zero;
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
            where D : Dictionary<string, T>
            where T : class
        {
            if(value == null)
            {
                Dev.Log("Warning: " + name + " is null!");
                return;
            }

            Dev.Log("Added: " + name + " to dictionary of " + dictionary.GetType().Name + "!");
            dictionary.Add(name, value);
        }
    }
}