using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public class HornetCorpse : Physics2DSM
    {
        public MeshRenderer meshRenderer;
        public tk2dSpriteAnimator tk2dAnimator;
        public ParticleSystem grassEscape;
        public tk2dSpriteAnimator leaveAnim;
        public GameObject startPt;
        public ParticleSystem grass;
        public tk2dSpriteAnimator thread;

        public UnityEngine.Audio.AudioMixerSnapshot audioSnapshot;
        public Dictionary<string, AudioClip> audioClips;
        public Dictionary<string, GameObject> prefabs;

        //use for some sound effects
        public AudioSource actorAudioSource;

        public override bool Running {
            get {
                return gameObject.activeInHierarchy;
            }

            set {
                gameObject.SetActive( value );
            }
        }

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            meshRenderer = GetComponent<MeshRenderer>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            audioClips = new Dictionary<string, AudioClip>();
            prefabs = new Dictionary<string, GameObject>();
        }

        protected virtual void OnCollisionStay2D( Collision2D collision )
        {
            if( collision.gameObject.layer == collisionLayer )
            {
                CheckTouching( collisionLayer );
            }
        }

        protected override IEnumerator Init()
        {
            yield return base.Init();

            nextState = LimitY;

            yield break;
        }

        protected virtual IEnumerator LimitY()
        {
            nextState = SetPD;
            yield break;
        }
        protected virtual IEnumerator SetPD()
        {
            nextState = Blow;
            yield break;
        }
        protected virtual IEnumerator Blow()
        {
            nextState = Launch;
            yield break;
        }
        protected virtual IEnumerator Launch()
        {
            nextState = InAir;
            yield break;
        }
        protected virtual IEnumerator InAir()
        {
            nextState = Land;
            yield break;
        }
        protected virtual IEnumerator Land()
        {
            nextState = CheckPos;
            yield break;
        }
        protected virtual IEnumerator CheckPos()
        {
            nextState = R;
            nextState = L;
            yield break;
        }
        protected virtual IEnumerator R()
        {
            nextState = Jump;
            yield break;
        }
        protected virtual IEnumerator L()
        {
            nextState = Jump;
            yield break;
        }
        protected virtual IEnumerator Jump()
        {
            nextState = ThrowStart;
            yield break;
        }
        protected virtual IEnumerator ThrowStart()
        {
            nextState = Throw;
            yield break;
        }
        protected virtual IEnumerator Throw()
        {
            nextState = Yank;
            yield break;
        }
        protected virtual IEnumerator Yank()
        {
            nextState = End;
            yield break;
        }
        protected virtual IEnumerator End()
        {
            //destroy this
            yield break;
        }

        protected void SetFightGates( bool closed )
        {
            if( closed )
            {
                FSMUtility.LocateFSM( GameObject.Find( "Battle Gate A" ), "BG Control" )?.SendEvent( "BG CLOSE" );
                FSMUtility.LocateFSM( GameObject.Find( "Battle Gate (1)" ), "BG Control" )?.SendEvent( "BG CLOSE" );
            }
            else
            {
                FSMUtility.LocateFSM( GameObject.Find( "Battle Gate A" ), "BG Control" )?.SendEvent( "BG OPEN" );
                FSMUtility.LocateFSM( GameObject.Find( "Battle Gate (1)" ), "BG Control" )?.SendEvent( "BG OPEN" );
            }
        }

        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources();

            string bossFSMName = "Control";

            yield return GetGameObjectFromCreateObjectInFSM( gameObject, bossFSMName, "Blow", SetPrefab, false );//???

            //TODO: give this a way to get the 2nd action
            yield return GetGameObjectsFromSpawnRandomObjectsV2InFSM( gameObject, bossFSMName, "Blow", SetPrefab );//???
            yield return GetGameObjectsFromSpawnRandomObjectsV2InFSM( gameObject, bossFSMName, "Blow", SetPrefab );//???
            yield return GetAudioSourceObjectFromFSM( gameObject, bossFSMName, "Blow", SetActorAudioSource );
            yield return GetAudioClipFromFSM( gameObject, bossFSMName, "Blow", SetAudioClip );//Hornet_Fight_Death_01
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Blow", SetAudioClip );//boss_explode_clean
            yield return GetAudioClipFromFSM( gameObject, bossFSMName, "Jump", SetAudioClip );//Hornet_Fight_Stun_02
            yield return GetAudioClipFromFSM( gameObject, bossFSMName, "Throw", SetAudioClip );//hornet_needle_thow_spin

            //TODO: give this a way to get the 2nd action
            yield return GetAudioClipFromFSM( gameObject, bossFSMName, "Yank", SetAudioClip );//Hornet_Fight_Yell_03
            yield return GetAudioClipFromFSM( gameObject, bossFSMName, "Yank", SetAudioClip );//hornet_dash
            audioSnapshot = GetSnapshotFromFSM( gameObject, bossFSMName, "Blow" );//Silent
        
            //load child references
            if(gameObject.FindGameObjectInChildren( "Thread" ) != null )
                thread = gameObject.FindGameObjectInChildren( "Thread" ).GetComponent<tk2dSpriteAnimator>();
            if(gameObject.FindGameObjectInChildren( "Grass" ) != null )
                grass = gameObject.FindGameObjectInChildren( "Grass" ).GetComponent<ParticleSystem>();
            if( gameObject.FindGameObjectInChildren( "Start Pt" ) != null )
                startPt = gameObject.FindGameObjectInChildren( "Start Pt" );
            if(gameObject.FindGameObjectInChildren( "Leave Anim" ) != null )
                leaveAnim = gameObject.FindGameObjectInChildren( "Leave Anim" ).GetComponent<tk2dSpriteAnimator>();
            if( gameObject.FindGameObjectInChildren( "Grass Escape" ) != null )
                grassEscape = gameObject.FindGameObjectInChildren( "Grass Escape" ).GetComponent<ParticleSystem>();
        }

        void SetPrefab( GameObject prefab )
        {
            if( prefab == null )
            {
                Dev.Log( "Warning: prefab is null!" );
                return;
            }

            Dev.Log( "Added: "+ prefab.name + " to prefabs!" );
            prefabs.Add( prefab.name, prefab );
        }

        void SetAudioClip( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: audio clip is null!" );
                return;
            }

            Dev.Log( "Added: " + clip.name + " to audioClips!" );
            audioClips.Add( clip.name, clip );
        }

        void SetActorAudioSource( AudioSource source )
        {
            if( source == null )
            {
                Dev.Log( "Warning: Actor AudioSource failed to load and is null!" );
                return;
            }

            actorAudioSource = source;
            actorAudioSource.transform.SetParent( gameObject.transform );
            actorAudioSource.transform.localPosition = Vector3.zero;
        }
    }
}