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
    [RequireComponent(typeof(BoxCollider2D))]
    public class EvadeRange : MonoBehaviour
    {
        public bool ObjectIsInRange { get; private set; }
        public bool IsOnCooldown { get; private set; }

        public float onTimeMin = 1f;
        public float onTimeMax = 2f;

        public float offTimeMin = 2f;
        public float offTimeMax = 3f;

        BoxCollider2D bodyCollider;

        IEnumerator currentState = null;

        public void DisableEvadeForTime(float disableTime)
        {
            if(IsOnCooldown)
                return;

            StartCoroutine(EnableEvadeAfterTime(disableTime));
        }

        IEnumerator EnableEvadeAfterTime(float time)
        {
            IsOnCooldown = true;
            if( bodyCollider == null )
                bodyCollider = GetComponent<BoxCollider2D>();
            bodyCollider.enabled = false;
            yield return new WaitForSeconds(time);
            bodyCollider.enabled = true;
            IsOnCooldown = false;
        }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = false;
        }
        
        IEnumerator Start()
        {
            bodyCollider = GetComponent<BoxCollider2D>();
            currentState = On();
            for( ; ;)
            {
                yield return new WaitForEndOfFrame();

                if( IsOnCooldown )
                    continue;

                yield return currentState;
            }

            yield break;
        }

        IEnumerator On()
        {
            float time = GameRNG.Rand(onTimeMin,onTimeMax);
            yield return new WaitForSeconds( time );
            bodyCollider.enabled = true;
            currentState = Off();
            yield break;
        }

        IEnumerator Off()
        {
            float time = GameRNG.Rand(offTimeMin,offTimeMax);
            yield return new WaitForSeconds( time );
            ObjectIsInRange = false;
            bodyCollider.enabled = false;
            currentState = On();
            yield break;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class RunAwayCheck : MonoBehaviour
    {
        public bool ObjectIsInRange { get; private set; }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = false;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class RefightRange : MonoBehaviour
    {
        public bool ObjectIsInRange { get; private set; }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = false;
        }
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class SphereRange : MonoBehaviour
    {
        public bool ObjectIsInRange { get; private set; }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = false;
        }
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class ADashRange : MonoBehaviour
    {
        public bool ObjectIsInRange { get; private set; }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            ObjectIsInRange = false;
        }
    }

    public class FlashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        MeshRenderer meshRenderer;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            meshRenderer = GetComponent<MeshRenderer>();

            gameObject.SetActive(true);

            isAnimating = true;

            StartCoroutine(MainAILoop());
        }

        public void Stop()
        {
            isAnimating = false;
            tk2dAnimator.Stop();
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(;;)
            {
                if(parent == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            meshRenderer.enabled = true;

            //TODO: figure out how Reposition is being set?, if Reposition is true, set this
            transform.localPosition = new Vector3( 0f, 0f, 0f );

            //TODO: figure out how Reset Rotation bool is being set, if true set the rotation to identity
            transform.localRotation = Quaternion.identity;

            Vector3 localScale = transform.localScale;
            Vector3 lossyScale = transform.lossyScale;

            //TODO: figure out how "Unparent" bool is being set, if true, unparent this
            transform.SetParent( null );

            transform.localScale = lossyScale;

            yield return PlayFromFrameAndWaitForEndOfAnimation( 0 );

            isAnimating = false;

            transform.SetParent( parent.transform );

            transform.localScale = localScale;

            meshRenderer.enabled = false;

            currentState = null;

            gameObject.SetActive( false );

            yield break;
        }

        IEnumerator PlayFromFrameAndWaitForEndOfAnimation( int frame )
        {
            bool blockingAnimationIsPlaying = true;
            tk2dAnimator.AnimationCompleted = ( tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip ) => { blockingAnimationIsPlaying = false; };
            tk2dAnimator.PlayFromFrame( frame );

            while( blockingAnimationIsPlaying )
            {
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ADashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        MeshRenderer meshRenderer;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            meshRenderer = GetComponent<MeshRenderer>();

            gameObject.SetActive(true);

            isAnimating = true;

            StartCoroutine(MainAILoop());
        }

        public void Stop()
        {
            isAnimating = false;
            tk2dAnimator.Stop();
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(;;)
            {
                if(parent == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            meshRenderer.enabled = true;

            //TODO: figure out how Reposition is being set?, if Reposition is true, set this
            transform.localPosition = new Vector3( 3f, .3f, 0f );

            //TODO: figure out how Reset Rotation bool is being set, if true set the rotation to identity
            transform.localRotation = Quaternion.identity;

            Vector3 localScale = transform.localScale;
            Vector3 lossyScale = transform.lossyScale;
            
            //TODO: figure out how "Unparent" bool is being set, if true, unparent this
            transform.SetParent( null );

            transform.localScale = lossyScale;

            yield return PlayFromFrameAndWaitForEndOfAnimation( 0 );

            isAnimating = false;

            transform.SetParent( parent.transform );

            transform.localScale = localScale;

            meshRenderer.enabled = false;

            currentState = null;

            gameObject.SetActive( false );

            yield break;
        }

        IEnumerator PlayFromFrameAndWaitForEndOfAnimation( int frame )
        {
            bool blockingAnimationIsPlaying = true;
            tk2dAnimator.AnimationCompleted = ( tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip ) => { blockingAnimationIsPlaying = false; };

            tk2dAnimator.PlayFromFrame( frame );

            while( blockingAnimationIsPlaying )
            {
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
    }


    public class GDashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        MeshRenderer meshRenderer;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            meshRenderer = GetComponent<MeshRenderer>();

            gameObject.SetActive(true);

            isAnimating = true;

            StartCoroutine(MainAILoop());
        }

        public void Stop()
        {
            isAnimating = false;
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(;;)
            {
                if(parent == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            meshRenderer.enabled = true;

            //TODO: figure out how Reposition is being set?, if Reposition is true, set this
            transform.localPosition = new Vector3( 6.7f, 1f, 0f );

            //TODO: figure out how Reset Rotation bool is being set, if true set the rotation to identity
            transform.localRotation = Quaternion.identity;

            Vector3 localScale = transform.localScale;
            Vector3 lossyScale = transform.lossyScale;
            
            //TODO: figure out how "Unparent" bool is being set, if true, unparent this
            transform.SetParent( null );

            transform.localScale = lossyScale;
            
            yield return PlayFromFrameAndWaitForEndOfAnimation( 0 );

            isAnimating = false;

            transform.SetParent( parent.transform );

            transform.localScale = localScale;

            meshRenderer.enabled = false;

            currentState = null;

            gameObject.SetActive( false );

            yield break;
        }

        IEnumerator PlayFromFrameAndWaitForEndOfAnimation( int frame )
        {
            bool blockingAnimationIsPlaying = true;
            tk2dAnimator.AnimationCompleted = (tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip) => { blockingAnimationIsPlaying = false; };

            tk2dAnimator.PlayFromFrame( frame );

            while( blockingAnimationIsPlaying )
            {
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
    }

    public class ThrowEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        MeshRenderer meshRenderer;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            meshRenderer = GetComponent<MeshRenderer>();

            gameObject.SetActive(true);

            isAnimating = true;

            StartCoroutine(MainAILoop());
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(;;)
            {
                if(parent == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            meshRenderer.enabled = true;

            //TODO: figure out how Reposition is being set?, if Reposition is true, set this
            transform.localPosition = new Vector3( 1.3f, -.1f, 0f );

            //TODO: figure out how Reset Rotation bool is being set, if true set the rotation to identity
            transform.localRotation = Quaternion.identity;

            Vector3 localScale = transform.localScale;
            Vector3 lossyScale = transform.lossyScale;

            //TODO: figure out how "Unparent" bool is being set, if true, unparent this
            transform.SetParent( null );

            transform.localScale = lossyScale;

            yield return PlayFromFrameAndWaitForEndOfAnimation( 0 );

            isAnimating = false;

            transform.SetParent( parent.transform );

            transform.localScale = localScale;

            meshRenderer.enabled = false;

            currentState = null;

            gameObject.SetActive( false );

            yield break;
        }

        IEnumerator PlayFromFrameAndWaitForEndOfAnimation( int frame )
        {
            bool blockingAnimationIsPlaying = true;
            tk2dAnimator.AnimationCompleted = ( tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip ) => { blockingAnimationIsPlaying = false; };

            tk2dAnimator.PlayFromFrame( frame );

            while( blockingAnimationIsPlaying )
            {
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }
    }

    public class SphereBall : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        float sphereTime;
        float sphereStartSize;
        float sphereEndSize;

        public void Play(GameObject parent, float sphereTime, float sphereStartSize, float sphereEndSize)
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();

            gameObject.SetActive(true);

            isAnimating = true;

            this.sphereTime = sphereTime;
            this.sphereStartSize = sphereStartSize;
            this.sphereEndSize = sphereEndSize;

            //TODO: move the sphere starting size and growing size/rate to variables
            transform.localScale = new Vector3( sphereStartSize, sphereStartSize, 0f );

            //TODO: check iTweenScaleTo to see what value's it's using for Sphere Ball

            StartCoroutine(MainAILoop());
        }

        public void Stop()
        {
            isAnimating = false;
            currentState = null;
            gameObject.SetActive(false);
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(;;)
            {
                if(parent == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish
            //TODO: tween to the final size

            currentState = Complete();

            yield break;
        }

        IEnumerator Grow()
        {
            Dev.Where();

            Vector3 targetScale = new Vector3(sphereEndSize,sphereEndSize,1f);
            Vector3 velocity = Vector3.zero;

            float time = 0f;
            while(time < sphereTime)
            {
                Vector3 scale = transform.localScale;
                scale = Vector3.SmoothDamp( scale, targetScale, ref velocity, sphereTime );
            }

            yield break;
        }

        IEnumerator Complete()
        {
            Dev.Where();

            //TODO: finish

            isAnimating = false;

            yield break;
        }
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Needle : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        public PolygonCollider2D bodyCollider;
        public Rigidbody2D body;

        public GameObject owner;
        public GameObject thread;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        float startDelay;
        Vector2 throwVelocity;
        float throwAngle;
        float returnSpeed;

        //TODO: improvement: change to not use the throw angle and just use the throw velocity
        public void Play(GameObject owner, float startDelay, Vector2 throwVelocity, float throwAngle, float returnSpeed )
        {
            this.owner = owner;
            tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
            bodyCollider = gameObject.GetComponent<PolygonCollider2D>();
            body = gameObject.GetComponent<Rigidbody2D>();

            gameObject.SetActive(true);

            thread = gameObject.FindGameObjectInChildren( "Thread" );

            isAnimating = true;

            this.startDelay = startDelay;
            this.throwVelocity = throwVelocity;
            this.throwAngle = throwAngle;
            this.returnSpeed = returnSpeed;

            StartCoroutine(MainAILoop());
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Out();

            for(;;)
            {
                if(owner == null)
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Out()
        {
            Dev.Where();

            yield return new WaitForSeconds( startDelay );

            transform.localRotation = Quaternion.identity;

            FaceAngle( throwAngle );
            //FaceAngle( 180f ); //not sure what the point of this one is 

            body.velocity = throwVelocity;

            //TODO: make a variable for this
            float deceleration = .8f;
            for(; ; )
            {
                Vector2 velocity = body.velocity;
                if( velocity.x < 0f )
                {
                    velocity.x *= deceleration;
                    if( velocity.x > 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                else if( velocity.x > 0f )
                {
                    velocity.x *= deceleration;
                    if( velocity.x < 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                if( velocity.y < 0f )
                {
                    velocity.y *= deceleration;
                    if( velocity.y > 0f )
                    {
                        velocity.y = 0f;
                    }
                }
                else if( velocity.y > 0f )
                {
                    velocity.y *= deceleration;
                    if( velocity.y < 0f )
                    {
                        velocity.y = 0f;
                    }
                }
                body.velocity = velocity;

                if( velocity.magnitude <= 0f )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            currentState = Return();

            yield break;
        }

        bool isReturnComplete = false;

        void iTweenOnComplete( int aniTweenID )
        {
            isReturnComplete = true;
        }

        IEnumerator Return()
        {
            Dev.Where();
            isReturnComplete = false;

            Hashtable hashtable = new Hashtable();
            hashtable.Add( "position", owner.transform.position );
            hashtable.Add( "speed", returnSpeed );
            hashtable.Add( "easetype", iTween.EaseType.easeInSine );
            hashtable.Add( "looptype", iTween.LoopType.none );
            hashtable.Add( "oncomplete", "iTweenOnComplete" );
            hashtable.Add( "oncompleteparams", 0 );
            iTween.MoveTo( gameObject, hashtable );

            thread.SetActive( true );

            //TODO: make a variable for this
            float deceleration = .8f;
            for(; ; )
            {
                Vector2 velocity = body.velocity;
                if( velocity.x < 0f )
                {
                    velocity.x *= deceleration;
                    if( velocity.x > 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                else if( velocity.x > 0f )
                {
                    velocity.x *= deceleration;
                    if( velocity.x < 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                if( velocity.y < 0f )
                {
                    velocity.y *= deceleration;
                    if( velocity.y > 0f )
                    {
                        velocity.y = 0f;
                    }
                }
                else if( velocity.y > 0f )
                {
                    velocity.y *= deceleration;
                    if( velocity.y < 0f )
                    {
                        velocity.y = 0f;
                    }
                }
                body.velocity = velocity;

                if( isReturnComplete )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            currentState = Complete();

            yield break;
        }

        IEnumerator Complete()
        {
            Dev.Where();

            isAnimating = false;
            gameObject.SetActive( false );

            yield break;
        }

        void FaceAngle( float offset )
        {
            Vector2 velocity = body.velocity;
            float z = Mathf.Atan2(velocity.y, velocity.x) * 57.2957764f + offset;
            transform.localEulerAngles = new Vector3( 0f, 0f, z );
        }
    }


    [RequireComponent(typeof(BoxCollider2D))]
    public class NeedleTink : MonoBehaviour
    {
        public void SetParent(Transform t)
        {
            //if deparenting, hide the parent
            if(transform.parent != null && t == null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            }

            gameObject.transform.SetParent(t);
            gameObject.transform.localPosition = Vector2.zero;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(AudioSource))]
    public class HornetBoss1 : MonoBehaviour
    {
        //components used by the boss
        public GameObject owner;
        public BoxCollider2D bodyCollider;
        public Rigidbody2D body;
        public MeshRenderer meshRenderer;
        public AudioSource runAudioSource;
        public GameObject areaTitleObject;
        public PolygonCollider2D hitADash;
        public PolygonCollider2D hitGDash;
        public ParticleSystem dustHardLand;

        public tk2dSpriteAnimator tk2dAnimator;
        public HealthManager healthManager;
        public ThrowEffect throwEffect;
        public EvadeRange evadeRange;
        public RunAwayCheck runAwayCheck;
        public SphereRange sphereRange;
        public SphereRange aSphereRange;
        public RefightRange refightRange;
        public ADashRange aDashRange;
        public ADashEffect aDashEffect;
        public GDashEffect gDashEffect;
        public SphereBall sphereBall;
        public FlashEffect flashEffect;

        //hornet's projectile weapon & the tink effect that goes with it
        public Needle needle;
        public NeedleTink needleTink;

        //use for some sound effects
        public AudioSource actorAudioSource;

        //used to play the boss background music
        //TODO: uncomment in real build and remove the "object" version
        public AudioClip hornetYell;
        public List<AudioClip> hornetAttackYells;
        public AudioClip hornetThrowSFX;
        public AudioClip hornetCatchSFX;
        public List<AudioClip> hornetLaughs;
        public AudioClip hornetSmallJumpSFX;
        public AudioClip hornetGroundLandSFX;
        public AudioClip hornetJumpSFX;
        public List<AudioClip> hornetJumpYells;
        public AudioClip hornetLandSFX;
        public List<AudioClip> hornetAGDashYells;
        public AudioClip hornetDashSFX;
        public AudioClip hornetWallLandSFX;
        public AudioClip hornetSphereSFX;
#if UNITY_EDITOR
        public object fightMusic;
#else
        public MusicCue fightMusic;
#endif
        public UnityEngine.Audio.AudioMixerSnapshot fightMusicSnapshot;

        //have this set by an outside controller to start the fight
        public bool wake = false;

        //TODO: set when hit
        public bool wasHitRecently;

#if UNITY_EDITOR
        //stuff used by the testing framework
        public bool tempDone = false;
#else
#endif

        //variables used by the state machine that we set here
        public int maxHP = 225;
        public float runSpeed = 8f;
        public float evadeJumpAwaySpeed = 22f;
        public float evadeJumpAwayTimeLength = .25f;
        public float throwDistance = 10f;
        public float throwSpeed = 40f;
        public float throwReturnSpeed = 30f;
        public float throwWindUpTime = .3f;
        public float jumpDistance = 10f;
        public float jumpVelocityY = 41f;
        public float minAirSphereHeight = 5f;
        public float normGravity2DScale = 1.5f;
        public float normShortJumpGravity2DScale = 2f;
        public float airFireSpeed = 30f;
        public float gDashSpeed = 25f;
        public float maxGDashTime = .35f;
        public float aSphereTime = 1f;
        public float gSphereTime = 1f;
        public float aSphereSize = 1.5f;
        public float gSphereSize = 1.5f;

        public float escalationHPPercentage = .4f;
        public float chanceToThrow = .8f;

        public float normRunWaitMin = .5f;
        public float normRunWaitMax = .1f;
        public float normIdleWaitMin = .5f;
        public float normIdleWaitMax = .75f;
        public float normEvadeCooldownMin = 1f;
        public float normEvadeCooldownMax = 2f;
        public float normDmgIdleWaitMin = .25f;
        public float normDmgIdleWaitMax = .4f;
        public float normAirDashPauseMin = .15f;
        public float normAirDashPauseMax = .4f;

        public float esRunWaitMin = .35f;
        public float esRunWaitMax = .75f;
        public float esIdleWaitMin = .1f;
        public float esIdleWaitMax = .4f;

        public int maxMissADash = 5;
        public int maxMissASphere = 7;
        public int maxMissGDash = 5;
        public int maxMissThrow = 3;

        //TODO: convert to a weighted table type
        Dictionary<Func<IEnumerator>, float> dmgResponseChoices;
        public Dictionary<Func<IEnumerator>, float> DmgResponseChoices
        {
            get
            {
                if(dmgResponseChoices == null)
                {
                    dmgResponseChoices = new Dictionary<Func<IEnumerator>, float>();
                    dmgResponseChoices.Add(EvadeAntic, .3f);
                    dmgResponseChoices.Add(SetJumpOnly, .15f);
                    dmgResponseChoices.Add(MaybeGSphere, .15f);
                    dmgResponseChoices.Add(DmgIdle, .4f);
                }
                return dmgResponseChoices;
            }
        }

        //do we respond to world collisions in these directions?
        public bool checkUp = false;
        public bool checkDown = false;
        public bool checkLeft = true;
        public bool checkRight = true;

        //variables used by the state machine that the states set
        bool blockingAnimationIsPlaying = false;
        float airDashPause;
        float jumpPoint;
        float nextThrowAngle;
        float runWaitMin;
        float runWaitMax;
        float idleWaitMin;
        float idleWaitMax;
        float evadeCooldownMin;
        float evadeCooldownMax;
        float dmgIdleWaitMin;
        float dmgIdleWaitMax;
        float airDashPauseMin;
        float airDashPauseMax;
        float returnXScale;

        bool topHit = false;
        bool rightHit = false;
        bool bottomHit = false;
        bool leftHit = false;
        bool canStunRightNow = true;
        bool escalated = false;
        bool willSphere = false;

        int ctIdle = 0;
        int ctRun = 0;
        int ctAirDash = 0;
        int ctASphere = 0;
        int ctGDash = 0;
        int ctThrow = 0;
        int msAirdash = 0;
        int msASphere = 0;
        int msGDash = 0;
        int msThrow = 0;

        Vector2 aDashVelocity;

        void SetFightGates(bool closed)
        {
            //TODO
        }

        void SetupDefaultParams()
        {
            //original setup logic that was duplicated in both wake states
            owner.transform.localScale = owner.transform.localScale.SetX(-1f);
            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            //setup our custom variables
            healthManager.hp = maxHP;

            idleWaitMin = normIdleWaitMin;
            idleWaitMax = normIdleWaitMax;

            runWaitMin = normRunWaitMin;
            runWaitMax = normRunWaitMax;

            evadeCooldownMin = normEvadeCooldownMin;
            evadeCooldownMax = normEvadeCooldownMax;

            dmgIdleWaitMin = normDmgIdleWaitMin;
            dmgIdleWaitMax = normDmgIdleWaitMax;

            airDashPauseMin = normAirDashPauseMin;
            airDashPauseMax = normAirDashPauseMax;
        }

        //current state of the state machine
        Func<IEnumerator> nextState = null;
        
        IEnumerator Start()
        {
            SetupRequiredReferences();

            //wait for the references to be ready in the playmaker fsm
            yield return ExtractReferencesFromPlayMakerFSMs();

            RemoveDeprecatedComponents();

            StartCoroutine(MainAILoop());

            yield break;
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();

            nextState = Init;

            IEnumerator currentState = nextState();
            nextState = null;

            for(;;)
            {
                if(owner == null)
                    yield break;
                
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

                    currentState = nextState();
                    nextState = null;
                }

                //TODO: remove as the states get implemented
                //yield return new WaitForEndOfFrame();
            }
            yield break;
        }
        
        IEnumerator Init()
        {
            Dev.Where();
            body.gravityScale = normGravity2DScale;

            nextState = Inert;

            yield break;
        }


        IEnumerator Inert()
        {
            Dev.Where();
            //TODO: move this check into a helper function and replace constants with variables
            int test = GameManager.instance.playerData.GetInt("hornetGreenpath");

            if(test >= 4)
            {
                nextState = RefightReady;
            }
            //UNTESTED CODEPATH
            else
            {
                while(!wake)
                {
                    yield return new WaitForEndOfFrame();
                }

                nextState = Wake;
            }

            yield break;
        }

        IEnumerator Wake()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            nextState = Flourish;

            yield break;
        }

        //the start of the fight!
        IEnumerator Flourish()
        {
            Dev.Where();

            SetupDefaultParams();

            //close the gates
            SetFightGates(true);

            ShowBossTitle(2f, "", "", "", "HORNET", "", "THE GUARDIAN");
            
            PlayOneShot(hornetYell);

            PlayBossMusic();

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Flourish");

            nextState = Idle;

            yield break;
        }

        IEnumerator Idle()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;

            //TODO: test this
            FaceObject(hero.gameObject, false, false);

            airDashPause = 999f;

            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            PlayAnimation( "Idle");

            body.velocity = Vector2.zero;

            if(evadeRange.ObjectIsInRange)
            {
                nextState = EvadeAntic;
            }
            else
            {
                //use a while loop to yield that way other events may force a transition 
                float randomDelay = GameRNG.Rand(idleWaitMin, idleWaitMax);
                while(randomDelay > 0f)
                {
                    yield return new WaitForEndOfFrame();

                    //did something hit us?
                    if(wasHitRecently)
                    {
                        //CODEPATH UNTESTED
                        nextState = DmgResponse;
                        yield break;
                    }

                    randomDelay -= Time.deltaTime;
                }

                //nothing hit us, choose the next state with 50/50
                List<Func<IEnumerator>> nextStates = new List<Func<IEnumerator>>()
                {
                    MaybeFlip, MaybeGSphere
                };

                //TODO: look into why it gets stuck here, something is wrong with the logic...
                int saveCounter = 0;
                bool flag = false;
                while(!flag)
                {
                    //TODO: create a weighted table type that has max hit/miss settings
                    int randomWeightedIndex = GameRNG.Rand(0, nextStates.Count);
                    Dev.Log( "Idle next choice " + randomWeightedIndex );
                    if(randomWeightedIndex == 0 && ctIdle < 2)
                    {
                        ctIdle += 1;
                        ctRun = 0;
                        Dev.Log( "idle chosen and is now " + ctIdle );
                        nextState = nextStates[0];
                        flag = true;
                    }
                    else if(randomWeightedIndex == 1 && ctRun < 2)
                    {
                        ctIdle = 0;
                        ctRun += 1;
                        Dev.Log( "run chosen and is now " + ctRun );
                        nextState = nextStates[1];
                        flag = true;
                    }
                    saveCounter++;
                    if( saveCounter > 100 )
                    {
                        nextState = nextStates[ 0 ];
                        flag = true;
                    }
                }
            }

            yield break;
        }

        IEnumerator MaybeFlip()
        {
            Dev.Where();

            //50/50 chance to flip
            bool shouldFlip = GameRNG.CoinToss();
            if(shouldFlip)
            {
                FlipScale();
            }
            Dev.Log( "flipped?" + shouldFlip );

            nextState = RunAway;

            yield break;
        }

        IEnumerator RunAway()
        {
            Dev.Where();

            Dev.Log( "runAwayCheck.ObjectIsInRange" + runAwayCheck.ObjectIsInRange );
            //TODO: play around with this, something seems off about it
            if(runAwayCheck.ObjectIsInRange)
            {
                //face the knight
                HeroController hero = HeroController.instance;
                FaceObject(hero.gameObject, false, false);

                //then flip the other way
                FlipScale();
                Dev.Log( "x scale = " + transform.localScale.x );
            }

            nextState = RunAntic;

            yield break;
        }

        IEnumerator RunAntic()
        {
            Dev.Where();
                        
            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Evade Antic");

            nextState = Run;

            yield break;
        }

        IEnumerator Run()
        {
            Dev.Where();

            runAudioSource.Play();

            float xVel = owner.transform.localScale.x * -runSpeed;

            PlayAnimation( "Run");

            //TODO: maybe continually set this?
            body.velocity = new Vector2(xVel, 0f);
            
            float randomDelay = GameRNG.Rand(runWaitMin, runWaitMax);
            while(randomDelay > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did something hit us?
                if(wasHitRecently)
                {
                    nextState = DmgResponse;
                    break;
                }

                if(evadeRange.ObjectIsInRange)
                {
                    nextState = EvadeAntic;
                    break;
                }

                if(rightHit || leftHit)
                {
                    nextState = MaybeGSphere;
                    break;
                }

                randomDelay -= Time.deltaTime;
            }

            //do this by default
            if( nextState == null )
                nextState = MaybeGSphere;

            runAudioSource.Stop();

            yield break;
        }

        IEnumerator MaybeGSphere()
        {
            Dev.Where();

            runAudioSource.Stop();

            if(sphereRange.ObjectIsInRange)
            {
                float randomChoice = GameRNG.Randf();
                if(randomChoice > chanceToThrow)
                {
                    nextState = SphereAnticG;
                }
                else
                {
                    nextState = CanThrow;
                }
            }
            else
            {
                nextState = CanThrow;
            }

            yield break;
        }

        IEnumerator CanThrow()
        {
            Dev.Where();

            Vector3 currentPosition = owner.transform.position;

            Vector2 throwOrigin = currentPosition;
            Vector2 throwDirection = Vector2.left;

            //TODO: custom enhancement, get the unity vector direction to the hero and throw along that line
            HeroController hero = HeroController.instance;
            var direction = DoCheckDirection(hero.gameObject);

            if(direction.right)
            {
                throwDirection = Vector2.right;
            }

            RaycastHit2D raycastHit2D2 = Physics2D.Raycast(throwOrigin, throwDirection, throwDistance, 1 << 8);
            if(raycastHit2D2.collider != null)
            {
                //there's a wall, we cannot throw!
                nextState = MoveChoiceB;
            }
            else
            {
                //we can throw!
                nextState = MoveChoiceA;
            }

            yield break;
        }

        //"can throw"
        IEnumerator MoveChoiceA()
        {
            Dev.Where();

            bool flag = false;
            bool flag2 = false;
            int num = 0;
            while(!flag)
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0, 3);
                if(ctAirDash >= maxMissADash)
                {
                    flag2 = true;
                    num = 0;
                }
                if(ctASphere >= maxMissASphere)
                {
                    flag2 = true;
                    num = 1;
                }
                if(ctGDash >= maxMissGDash)
                {
                    flag2 = true;
                    num = 2;
                }
                if(ctThrow >= maxMissThrow)
                {
                    flag2 = true;
                    num = 2;
                }

                //were we forced to use a skill? if so, record that and move on
                if(flag2)
                {
                    flag = true;

                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow = 0;

                    msAirdash += 1;
                    msASphere += 1;
                    msGDash += 1;
                    msThrow += 1;

                    if(num == 0)
                    {
                        msAirdash = 0;
                        ctAirDash = 1;
                        nextState = SetADash;
                    }
                    if(num == 1)
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        nextState = SetSphereA;
                    }
                    if(num == 2)
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        nextState = GDashAntic;
                    }
                    if(num == 3)
                    {
                        msThrow = 0;
                        ctThrow = 1;
                        nextState = ThrowAntic;
                    }
                }
                //else, randomly pick a skill to use
                else if(randomWeightedIndex == 0 && ctAirDash < 2)
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow = 0;
                    nextState = SetADash;
                    flag = true;
                }
                else if(randomWeightedIndex == 1 && ctASphere < 1)
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    ctThrow = 0;
                    nextState = SetSphereA;
                    flag = true;
                }
                else if(randomWeightedIndex == 2 && ctGDash < 2)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    ctThrow = 0;
                    nextState = GDashAntic;
                    flag = true;
                }
                else if(randomWeightedIndex == 3 && ctThrow < 1)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow += 1;
                    nextState = ThrowAntic;
                    flag = true;
                }
            }

            if(nextState == null )
                nextState = ThrowAntic;

            yield break;
        }

        //"cannot throw"
        IEnumerator MoveChoiceB()
        {
            Dev.Where();


            int maxMissADash = 5;
            int maxMissASphere = 7;
            int maxMissGDash = 5;

            bool flag = false;
            bool flag2 = false;
            int num = 0;
            while(!flag)
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0, 2);
                if(ctAirDash >= maxMissADash)
                {
                    flag2 = true;
                    num = 0;
                }
                if(ctASphere >= maxMissASphere)
                {
                    flag2 = true;
                    num = 1;
                }
                if(ctGDash >= maxMissGDash)
                {
                    flag2 = true;
                    num = 2;
                }

                //were we forced to use a skill? if so, record that and move on
                if(flag2)
                {
                    flag = true;

                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;

                    msAirdash += 1;
                    msASphere += 1;
                    msGDash += 1;

                    if(num == 0)
                    {
                        msAirdash = 0;
                        ctAirDash = 1;
                        nextState = SetADash;
                    }
                    if(num == 1)
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        nextState = SetSphereA;
                    }
                    if(num == 2)
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        nextState = GDashAntic;
                    }
                }
                //else, randomly pick a skill to use
                else if(randomWeightedIndex == 0 && ctAirDash < 2)
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    nextState = SetADash;
                    flag = true;
                }
                else if(randomWeightedIndex == 1 && ctASphere < 1)
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    nextState = SetSphereA;
                    flag = true;
                }
                else if(randomWeightedIndex == 2 && ctGDash < 2)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    nextState = GDashAntic;
                    flag = true;
                }
            }

            if( nextState == null )
                nextState = SetSphereA;

            yield break;
        }

        IEnumerator ThrowAntic()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;

            //disable stun control
            canStunRightNow = false;

            //change our collider size to match the throw attack
            bodyCollider.offset = new Vector2(1f, -.3f);
            bodyCollider.size = new Vector2(1f, 2.6f);

            //face the hero
            FaceObject(hero.gameObject, false );

            //stop moving
            body.velocity = Vector2.zero;
                        
            PlayOneShotRandom(hornetAttackYells);

            //play throwing animation
            //wait here until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Throw Antic");

            nextState = MaybeLock;

            yield break;
        }

        IEnumerator MaybeLock()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;

            //get the angle to our hero
            float angleToTarget = GetAngleToTarget(hero.gameObject, 0f, 0f);

            if(angleToTarget <= 90f)
            {
                nextState = LockR;
            }
            else if(angleToTarget <= 180f)
            {
                nextState = LockL;
            }
            else if(angleToTarget <= 270f)
            {
                nextState = LockUL;
            }
            else if(angleToTarget <= 360f)
            {
                nextState = LockUR;
            }
            else
            {
                nextState = Throw;
            }

            yield break;
        }

        IEnumerator LockL()
        {
            Dev.Where();

            nextThrowAngle = 180f;
            nextState = Throw;

            yield break;
        }

        IEnumerator LockR()
        {
            Dev.Where();

            nextThrowAngle = 0f;
            nextState = Throw;

            yield break;
        }

        IEnumerator LockUL()
        {
            Dev.Where();

            nextThrowAngle = 180f;
            nextState = Throw;

            yield break;
        }

        IEnumerator LockUR()
        {
            Dev.Where();

            nextThrowAngle = 0f;
            nextState = Throw;

            yield break;
        }

        IEnumerator Throw()
        {
            Dev.Where();

            //play the throw sound effect
            PlayOneShot(hornetThrowSFX);

            //start the throw effect
            throwEffect.Play(owner);

            //shake the camera a bit
            DoEnemyKillShakeEffect();

            //change our collider size to match the during-throw attack
            bodyCollider.offset = new Vector2(.1f, -1.0f);
            bodyCollider.size = new Vector2(1.4f, 1.2f);

            //TODO: make the throw direction more dynamic
            Vector2 throwDirection = Vector2.right;
            if( owner.transform.localScale.x > 0f )
                throwDirection = Vector2.left;

            Vector2 throwVelocity = throwDirection * throwSpeed;

            //start throwing the needle
            needle.Play( owner, throwWindUpTime, throwVelocity, nextThrowAngle, throwReturnSpeed );

            //put the needle tink on the needle
            needleTink.SetParent(needle.transform);

            //start the throw animation
            PlayAnimation( "Throw");

            //wait one frame before ending
            yield return new WaitForEndOfFrame();

            nextState = Thrown;

            yield break;
        }

        IEnumerator Thrown()
        {
            Dev.Where();

            //wait while the needle does its thing (boomerang effect)
            while(needle.isAnimating)
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = ThrowRecover;

            yield break;
        }

        IEnumerator ThrowRecover()
        {
            Dev.Where();

            //play catch sound
            PlayOneShot(hornetCatchSFX);

            //remove tink effect
            needleTink.SetParent(null);

            //allow stunning again
            canStunRightNow = true;

            nextState = Escalation;

            yield break;
        }

        IEnumerator SetADash()
        {
            Dev.Where();

            airDashPause = GameRNG.Rand(airDashPauseMin, airDashPauseMax);
            willSphere = false;

            nextState = JumpAntic;

            yield break;
        }

        IEnumerator JumpAntic()
        {
            Dev.Where();

            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            body.velocity = Vector2.zero;
                        
            FaceObject(HeroController.instance.gameObject, true );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Jump Antic");

            nextState = AimJump;

            yield break;
        }

        IEnumerator AimJump()
        {
            Dev.Where();

            if(willSphere)
            {
                nextState = AimSphereJump;
            }
            else
            {
                //TODO: enchancement: make hornet jump at/near/away from the player

                Vector3 currentPosition = owner.transform.position;
                Vector2 jumpOrigin = currentPosition;
                Vector2 jumpDirectionL = Vector2.left;
                Vector2 jumpDirectionR = Vector2.right;

                float xMin = -jumpDistance;
                float xMax = jumpDistance;

                //get max L x jump distance
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionL, jumpDistance, 1 << 8);
                    if(raycastHit2D2.collider != null)
                    {
                        xMin = raycastHit2D2.transform.position.x;
                    }
                }

                //get max R x jump distance
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionR, jumpDistance, 1 << 8);
                    if(raycastHit2D2.collider != null)
                    {
                        xMax = raycastHit2D2.transform.position.x;
                    }
                }

                jumpPoint = GameRNG.Rand(xMin, xMax);

                //if it's too close, don't jump
                if(Mathf.Abs(jumpPoint - currentPosition.x) < 2.5f)
                {
                    nextState = ReAim;
                }
                else
                {
                    nextState = Jump;
                }
            }

            yield break;
        }

        IEnumerator Jump()
        {
            Dev.Where();

            PlayOneShotRandom(hornetJumpYells);
            PlayOneShot(hornetJumpSFX);

            PlayAnimation( "Jump");

            //TODO: this seems weird, see how it turns out
            body.velocity = new Vector2(jumpPoint, jumpVelocityY);

            nextState = InAir;

            yield break;
        }

        IEnumerator InAir()
        {
            Dev.Where();

            float startHeight = owner.transform.position.y;
            
            //TODO: debug and figure out why we're instantly "landing" mid jump sometimes

            //change collision check directions for jumping
            EnableCollisionsInDirection( false, true, false, false );

            Dev.Log( "Enabled downward collisions" );

            float airDashTimer = airDashPause;
            for(;;)
            {
                yield return new WaitForEndOfFrame();

                bool withinSphereHeightRange = Mathf.Abs(owner.transform.position.y - startHeight) < minAirSphereHeight;
                bool isFalling = body.velocity.y < 0f;

                if( willSphere && isFalling && withinSphereHeightRange )
                {
                    nextState = MaybeDoSphere;
                    break;
                }

                if( airDashTimer <= 0f )
                {
                    nextState = ADashAntic;
                    break;
                }

                //did we hit a wall? end evade timer early
                if(bottomHit)
                {
                    nextState = Land;
                    break;
                }

                airDashTimer -= Time.deltaTime;
            }

            //restore collision check directions
            EnableCollisionsInDirection( false, false, true, true );

            yield break;
        }

        //TODO: this looks redundant, look into changing all re-aims to just call AirJump() again
        IEnumerator ReAim()
        {
            Dev.Where();
            
            nextState = AimJump;

            yield break;
        }

        IEnumerator Land()
        {
            Dev.Where();

            PlayOneShot(hornetLandSFX);
                        
            body.gravityScale = normGravity2DScale;

            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            owner.transform.rotation = Quaternion.identity;

            owner.transform.localScale = owner.transform.localScale.SetY(1f);

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Land");

            nextState = Escalation;

            yield break;
        }

        IEnumerator ADashAntic()
        {
            Dev.Where();

            if(aDashRange.ObjectIsInRange)
            {
                HeroController hero = HeroController.instance;

                FaceObject( hero.gameObject );

                bodyCollider.offset = new Vector2(1.1f, -.9f);
                bodyCollider.size = new Vector2(1.2f, 1.4f);

                body.velocity = Vector2.zero;

                body.gravityScale = 0f;

                PlayOneShotRandom(hornetAGDashYells);

                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation("A Dash Antic");

                nextState = Fire;
            }
            else
            {
                nextState = InAir;
            }
            
            yield break;
        }

        IEnumerator Fire()
        {
            Dev.Where();

            PlayOneShot(hornetDashSFX);
            
            //TEST THIS
            hitADash.isTrigger = true;

            GameObject hero = HeroController.instance.gameObject;
            
            float angleToTarget = GetAngleToTarget(hero, 0f, -.5f);

            //TODO: see if this even works
            Vector2 pos = owner.transform.position;
            Vector2 fireVelocity = GetVelocityToTarget(pos, new Vector3(0f,-5f * owner.transform.localScale.x,0f), hero.transform.position, airFireSpeed, 0f);

            body.velocity = fireVelocity;
            
            Vector2 otherVelocity = GetVelocityFromSpeedAndAngle(airFireSpeed, angleToTarget);

            body.velocity = otherVelocity;
            aDashVelocity = body.velocity;

            bodyCollider.offset = new Vector2(.1f, 0f);
            bodyCollider.size = new Vector2(1.5f, 1.0f);

            hitADash.gameObject.SetActive(true);
            
            //owner.transform.localScale = owner.transform.localScale.SetY(1f);
            
            FaceAngle(0f);
            
            Vector3 eulerAngles = owner.transform.eulerAngles;
            float zAngle = eulerAngles.z;

            if(zAngle > 90f && zAngle < 270f)
            {
                nextState = FiringR;
            }
            else
            {
                nextState = FiringL;
            }

            yield break;
        }

        IEnumerator FiringL()
        {
            Dev.Where(); 

            returnXScale = 1f;

            owner.transform.localScale = owner.transform.localScale.SetX( 1f );

            nextState = ADash;

            yield break;
        }

        IEnumerator FiringR()
        {
            Dev.Where();

            returnXScale = -1f;

            owner.transform.localScale = owner.transform.localScale.SetX( -1f );

            nextState = ADash;

            yield break;
        }

        IEnumerator ADash()
        {
            Dev.Where();

            aDashEffect.Play(owner);

            PlayOneShot(hornetDashSFX);

            DoEnemyKillShakeEffect();

            airDashPause = 999;

            PlayAnimation( "A Dash");
            
            //change collision check directions for air dashing
            EnableCollisionsInDirection( true, true, true, true );

            while( !bottomHit && !rightHit && !leftHit && !topHit )
            {
                yield return new WaitForEndOfFrame();

                body.velocity = aDashVelocity;

                //did we hit a wall? end evade timer early
                if( bottomHit )
                {
                    nextState = LandY;
                    break;
                }
                if( topHit )
                {
                    nextState = HitRoof;
                    break;
                }
                if( leftHit )
                {
                    nextState = WallL;
                    break;
                }
                if( rightHit )
                {
                    nextState = WallR;
                    break;
                }
            }

            //restore collision check directions
            EnableCollisionsInDirection( false, false, true, true );



            //float startHeight = owner.transform.position.y;

            //Vector3 down = Vector3.down;
            //Vector3 up = Vector3.up;
            //Vector3 left = Vector3.up;
            //Vector3 right = Vector3.up;
            //float nearDist = 1f;

            //bool earlyOut = false;

            //while(!earlyOut)
            //{
            //    Vector3 position = owner.transform.position;
            //    float closeToSurface = .55f;//TODO: add collider sizes to this
            //    if(!earlyOut)
            //    {
            //        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, down, nearDist, 1 << 8);
            //        if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.y - position.y) < closeToSurface)
            //        {
            //            nextState = LandY;
            //            earlyOut = true;
            //        }
            //    }
            //    if(!earlyOut)
            //    {
            //        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, up, nearDist, 1 << 8);
            //        if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.y - position.y) < closeToSurface)
            //        {
            //            nextState = HitRoof;
            //            earlyOut = true;
            //        }
            //    }
            //    if(!earlyOut)
            //    {
            //        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, left, nearDist, 1 << 8);
            //        if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.x - position.x) < closeToSurface)
            //        {
            //            nextState = WallL;
            //            earlyOut = true;
            //        }
            //    }
            //    if(!earlyOut)
            //    {
            //        RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, right, nearDist, 1 << 8);
            //        if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.x - position.x) < closeToSurface)
            //        {
            //            nextState = WallR;
            //            earlyOut = true;
            //        }
            //    }
            //    if( !earlyOut )
            //        yield return new WaitForEndOfFrame();
            //    else
            //        break;
            //}

            if( nextState == null )
                nextState = LandY;

            yield break;
        }

        void DoWallLand(float xScale)
        {
            PlayOneShot(hornetWallLandSFX);

            owner.transform.rotation = Quaternion.identity;

            body.velocity = Vector2.zero;

            owner.transform.localScale = owner.transform.localScale.SetX(xScale);

            hitADash.gameObject.SetActive(false);

            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);
        }

        IEnumerator WallL()
        {
            Dev.Where();

            DoWallLand(1f);

            yield return  PlayAndWaitForEndOfAnimation("Wall Impact");

            nextState = JumpR;

            yield break;
        }

        IEnumerator WallR()
        {
            Dev.Where();

            DoWallLand(-1f);

            yield return  PlayAndWaitForEndOfAnimation("Wall Impact");

            nextState = JumpL;

            yield break;
        }

        void DoShortJump(float xDirection)
        {
            float xScale = -1f * Mathf.Sign(xDirection);

            body.velocity = new Vector2(jumpDistance * xDirection, jumpVelocityY * .5f);

            PlayAnimation( "Jump" );

            body.gravityScale = normShortJumpGravity2DScale;

            owner.transform.localScale = owner.transform.localScale.SetX(xScale);
        }

        IEnumerator JumpL()
        {
            Dev.Where();

            DoShortJump(-1f);

            nextState = InAir;

            yield break;
        }

        IEnumerator JumpR()
        {
            Dev.Where();

            DoShortJump(1f);

            nextState = InAir;

            yield break;
        }

        IEnumerator LandY()
        {
            Dev.Where();

            owner.transform.localScale = owner.transform.localScale.SetX( returnXScale );

            hitADash.gameObject.SetActive(false);

            nextState = HardLand;

            yield break;
        }

        IEnumerator HitRoof()
        {
            Dev.Where();
            
            owner.transform.localScale = owner.transform.localScale.SetX( returnXScale );

            hitADash.gameObject.SetActive(false);

            body.velocity = Vector2.zero;

            hitADash.enabled = false;

            body.gravityScale = normShortJumpGravity2DScale;
            
            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            owner.transform.rotation = Quaternion.identity;
            
            nextState = InAir;

            yield break;
        }

        IEnumerator HardLand()
        {
            Dev.Where();
            
            dustHardLand.Play();

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Hard Land");

            blockingAnimationIsPlaying = true;

            body.gravityScale = normGravity2DScale;

            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            owner.transform.rotation = Quaternion.identity;

            owner.transform.localScale = owner.transform.localScale.SetY(1f);

            float decelerationX = .8f;
            for(;;)
            {
                Vector2 velocity = body.velocity;
                if(velocity.x < 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x > 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                else if(velocity.x > 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x < 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                body.velocity = velocity;

                if(!blockingAnimationIsPlaying)
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }


            nextState = Escalation;

            yield break;
        }

        IEnumerator MaybeDoSphere()
        {
            Dev.Where();

            willSphere = false;

            if(aSphereRange.ObjectIsInRange)
            {
                nextState = SphereAnticA;
            }
            else
            {
                nextState = InAir;
            }

            yield break;
        }

        IEnumerator SphereAnticA()
        {
            Dev.Where();

            willSphere = false;

            body.gravityScale = 0f;

            FaceObject(HeroController.instance.gameObject, false );
            
            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Sphere Antic A");

            blockingAnimationIsPlaying = true;

            PlayOneShotRandom(hornetAttackYells);

            float deceleration = .8f;
            for(;;)
            {
                Vector2 velocity = body.velocity;
                if(velocity.x < 0f)
                {
                    velocity.x *= deceleration;
                    if(velocity.x > 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                else if(velocity.x > 0f)
                {
                    velocity.x *= deceleration;
                    if(velocity.x < 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                if(velocity.y < 0f)
                {
                    velocity.y *= deceleration;
                    if(velocity.y > 0f)
                    {
                        velocity.y = 0f;
                    }
                }
                else if(velocity.y > 0f)
                {
                    velocity.y *= deceleration;
                    if(velocity.y < 0f)
                    {
                        velocity.y = 0f;
                    }
                }
                body.velocity = velocity;

                if(!blockingAnimationIsPlaying)
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = SphereA;

            yield break;
        }

        IEnumerator SphereA()
        {
            Dev.Where();

            PlayOneShot(hornetSphereSFX);

            sphereBall.Play(owner, aSphereTime, .8f, aSphereSize);
            flashEffect.Play(owner);

            DoEnemyKillShakeEffect();

            PlayAnimation( "Sphere Attack");

            //TODO: move to variables
            float waitTime = aSphereTime;
            float deceleration = .8f;
            while(waitTime > 0f)
            {
                //TODO: add the "IsNone" check to the DecelerateV2 printing

                waitTime -= Time.fixedDeltaTime;

                Vector2 velocity = body.velocity;
                if(velocity.x < 0f)
                {
                    velocity.x *= deceleration;
                    if(velocity.x > 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                else if(velocity.x > 0f)
                {
                    velocity.x *= deceleration;
                    if(velocity.x < 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                if(velocity.y < 0f)
                {
                    velocity.y *= deceleration;
                    if(velocity.y > 0f)
                    {
                        velocity.y = 0f;
                    }
                }
                else if(velocity.y > 0f)
                {
                    velocity.y *= deceleration;
                    if(velocity.y < 0f)
                    {
                        velocity.y = 0f;
                    }
                }
                body.velocity = velocity;
                
                yield return new WaitForFixedUpdate();
            }
            
            nextState = SphereRecoverA;

            yield break;
        }

        IEnumerator SphereRecoverA()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation("Sphere Recover A");

            sphereBall.Stop();

            nextState = SphereAEnd;

            yield break;
        }

        IEnumerator SphereAEnd()
        {
            Dev.Where();

            body.gravityScale = normGravity2DScale;

            PlayAnimation( "Fall");

            nextState = InAir;

            yield break;
        }

        IEnumerator AimSphereJump()
        {
            Dev.Where();

            //TODO: enchancement: make hornet jump at/near/away from the player

            Vector3 currentPosition = owner.transform.position;
            Vector2 jumpOrigin = currentPosition;
            Vector2 jumpDirectionL = Vector2.left;
            Vector2 jumpDirectionR = Vector2.right;

            float xMin = -jumpDistance;
            float xMax = jumpDistance;

            //get max L x jump distance
            {
                RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionL, jumpDistance, 1 << 8);
                if(raycastHit2D2.collider != null)
                {
                    xMin = raycastHit2D2.transform.position.x;
                }
            }

            //get max R x jump distance
            {
                RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionR, jumpDistance, 1 << 8);
                if(raycastHit2D2.collider != null)
                {
                    xMax = raycastHit2D2.transform.position.x;
                }
            }

            jumpPoint = GameRNG.Rand(xMin, xMax);            

            nextState = Jump;

            yield break;
        }

        IEnumerator SetJumpOnly()
        {
            Dev.Where();

            airDashPause = 999f;

            willSphere = false;

            nextState = JumpAntic;

            yield break;
        }

        IEnumerator SetSphereA()
        {
            Dev.Where();

            willSphere = true;

            airDashPause = 999f;

            nextState = JumpAntic;

            yield break;
        }

        IEnumerator GDashAntic()
        {
            Dev.Where();
            
            HeroController hero = HeroController.instance;
            FaceObject(hero.gameObject);

            bodyCollider.offset = new Vector2(1.1f, -.9f);
            bodyCollider.size = new Vector2(1.2f, 1.4f);

            body.velocity = Vector2.zero;

            PlayOneShotRandom(hornetAGDashYells);

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("G Dash Antic");
            
            nextState = GDash;

            yield break;
        }

        IEnumerator GDash()
        {
            Dev.Where();

            PlayAnimation( "G Dash");
            
            PlayOneShot(hornetDashSFX);

            DoEnemyKillShakeEffect();

            gDashEffect.Play(owner);

            bodyCollider.offset = new Vector2(0.1f, -.8f);
            bodyCollider.size = new Vector2(1.6f, 1.5f);

            hitGDash.gameObject.SetActive(true);

            float xScale = owner.transform.localScale.x;

            body.velocity = new Vector2(-gDashSpeed * xScale, 0f);

            float waitTimer = maxGDashTime;
            while(waitTimer > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? then end the dash.
                if(rightHit || leftHit)
                {
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            nextState = GDashRecover1;

            yield break;
        }

        IEnumerator GDashRecover1()
        {
            Dev.Where();
            
            bodyCollider.offset = new Vector2(1.1f, -0.9f);
            bodyCollider.size = new Vector2(1.2f, 1.4f);

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("G Dash Recover1");

            blockingAnimationIsPlaying = true;

            //TODO: move into a variable
            float decelerationX = .77f;
            for(;;)
            {
                //TODO: add the "IsNone" check to the DecelerateXY printing

                Vector2 velocity = body.velocity;
                if(velocity.x < 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x > 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                else if(velocity.x > 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x < 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                body.velocity = velocity;

                if(!blockingAnimationIsPlaying)
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = GDashRecover2;

            yield break;
        }

        IEnumerator GDashRecover2()
        {
            Dev.Where();
            
            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("G Dash Recover2");

            blockingAnimationIsPlaying = true;

            float decelerationX = .75f;
            for(;;)
            {
                //TODO: add the "IsNone" check to the DecelerateXY printing

                Vector2 velocity = body.velocity;
                if(velocity.x < 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x > 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                else if(velocity.x > 0f)
                {
                    velocity.x *= decelerationX;
                    if(velocity.x < 0f)
                    {
                        velocity.x = 0f;
                    }
                }
                body.velocity = velocity;

                if(!blockingAnimationIsPlaying)
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = Escalation;

            yield break;
        }

        IEnumerator SphereAnticG()
        {
            Dev.Where();
            
            body.velocity = Vector2.zero;
            
            bodyCollider.offset = new Vector2(0.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);
            
            FaceObject(HeroController.instance.gameObject, false );

            PlayOneShotRandom(hornetAttackYells);

            yield return PlayAndWaitForEndOfAnimation("Sphere Antic G");

            nextState = Sphere;

            yield break;
        }

        IEnumerator Sphere()
        {
            Dev.Where();
            PlayOneShot(hornetSphereSFX);

            sphereBall.Play(owner, gSphereTime, .8f, gSphereSize);
            flashEffect.Play(owner);

            DoEnemyKillShakeEffect();

            PlayAnimation( "Sphere Attack");

            //TODO: move the wait value into a variable
            yield return new WaitForSeconds(gSphereTime);

            nextState = SphereRecover;

            yield break;
        }

        IEnumerator SphereRecover()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation("Sphere Recover G");

            sphereBall.Stop();

            nextState = Escalation;

            yield break;
        }

        IEnumerator EvadeAntic()
        {
            Dev.Where();

            runAudioSource.Stop();

            if(evadeRange.ObjectIsInRange)
            {
                //put her evade on cooldown
                float randomDelay = GameRNG.Rand(evadeCooldownMin, evadeCooldownMax);
                evadeRange.DisableEvadeForTime(randomDelay);

                //stop her moving
                body.velocity = Vector2.zero;

                //make her face you
                HeroController hero = HeroController.instance;
                FaceObject( hero.gameObject, true );

                //animate the evade-anticipation                
                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation("Evade Antic");

                nextState = Evade;
            }
            else
            {
                nextState = MaybeGSphere;
            }

            yield break;
        }

        IEnumerator Evade()
        {
            Dev.Where();

            PlayOneShotRandom(hornetLaughs);
            PlayOneShot(hornetSmallJumpSFX);

            PlayAnimation("Evade");

            float xScale = owner.transform.localScale.x;
            float jumpAwaySpeed = xScale * evadeJumpAwaySpeed;

            body.velocity = new Vector2(jumpAwaySpeed, 0f);
            float waitTimer = evadeJumpAwayTimeLength;
            while(waitTimer > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if(rightHit || leftHit)
                {
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            nextState = EvadeLand;

            yield break;
        }

        IEnumerator EvadeLand()
        {
            Dev.Where();

            //animate the evade-landing            
            body.velocity = Vector2.zero;

            PlayOneShot(hornetGroundLandSFX);

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Land" );

            nextState = AfterEvade;

            yield break;
        }

        IEnumerator AfterEvade()
        {
            Dev.Where();

            bool attack = GameRNG.CoinToss();
            if(attack)
            {
                nextState = MaybeGSphere;
            }
            else
            {
                nextState = Idle;
            }

            yield break;
        }

        IEnumerator DmgResponse()
        {
            Dev.Where();

            runAudioSource.Stop();

            int choice = GameRNG.WeightedRand(DmgResponseChoices.Values.ToList());
            nextState = DmgResponseChoices.Keys.ToList()[choice];

            yield break;
        }

        IEnumerator DmgIdle()
        {
            Dev.Where();

            float randomDelay = GameRNG.Rand(dmgIdleWaitMin, dmgIdleWaitMax);
            while(randomDelay > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if(rightHit || leftHit)
                {
                    break;
                }

                randomDelay -= Time.deltaTime;
            }

            nextState = MaybeGSphere;

            yield break;
        }


        IEnumerator RefightReady()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            PlayAnimation( "Idle");

            //wait for player to get close
            while(!refightRange.ObjectIsInRange)
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = RefightWake;

            yield break;
        }

        //TODO: state seems redundant, look into removing
        IEnumerator RefightWake()
        {
            Dev.Where();

            //TODO: activate all children? this is probably a state machine thing that we don't need anymore

            nextState = Flourish;

            yield break;
        }

        //TODO: hook something up to cause a transition to here
        IEnumerator StunStart()
        {
            Dev.Where();
            //TODO
            nextState = StunAir;

            yield break;
        }

        IEnumerator StunAir()
        {
            Dev.Where();
            //TODO
            nextState = StunLand;

            yield break;
        }

        IEnumerator StunLand()
        {
            Dev.Where();
            //TODO
            nextState = StunRecover;

            yield break;
        }

        IEnumerator StunRecover()
        {
            Dev.Where();
            //TODO
            nextState = SetJumpOnly;

            yield break;
        }

        IEnumerator Escalation()
        {
            Dev.Where();

            //see if we're low on hp and should act faster
            float hpRemainingPercent = (float)healthManager.hp / (float)maxHP;
            if(!escalated && hpRemainingPercent < escalationHPPercentage)
            {
                runWaitMin = esRunWaitMin;
                runWaitMax = esRunWaitMax;

                idleWaitMax = esIdleWaitMax;
                idleWaitMin = esIdleWaitMin;

                //TODO: escalate her evade timers
            }

            nextState = Idle;

            yield break;
        }

        public void DoEnemyKillShakeEffect()
        {
            //grab the camera's parent and shake it
            GameObject cam = GameObject.Find("CameraParent");
            if(cam != null)
            {
                cam.GetComponent<PlayMakerFSM>().SendEvent("EnemyKillShake");
            }
            else
            {
                Dev.Log("Cannot find camera to send shake event!");
            }
        }

        public void PlayOneShot(AudioClip clip)
        {
            if(actorAudioSource != null && clip != null)
            {
                actorAudioSource.PlayOneShot(clip);
            }
        }

        public void PlayOneShotRandom(List<AudioClip> clip)
        {
            if(actorAudioSource != null && clip != null && clip.Count > 0)
            {
                AudioClip randomClip = clip.GetRandomElementFromList();
                actorAudioSource.PlayOneShot(randomClip);
            }
        }

        void ShowBossTitle(float hideInSeconds, string largeMain = "", string largeSuper = "", string largeSub = "", string smallMain = "", string smallSuper = "", string smallSub = "")
        {
            //no point in doing this
            if(hideInSeconds <= 0f)
                hideInSeconds = 0f;

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

                //give it 3 seconds to fade in
                StartCoroutine( HideBossTitleAfter( hideInSeconds + 3f ) );
#endif
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot show the boss title.");
            }
        }

        IEnumerator HideBossTitleAfter(float time)
        {
            yield return new WaitForSeconds(time);
            HideBossTitle();
            yield return new WaitForSeconds(3f);
            areaTitleObject.SetActive(false);
        }

        void HideBossTitle()
        {
            //show hornet title
            if(areaTitleObject != null)
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

        void PlayBossMusic()
        {
            //set the audio mixer snapshot
            if(fightMusicSnapshot != null)
            {
                fightMusicSnapshot.TransitionTo(1f);
            }

            // play the boss music music
            GameManager instance = GameManager.instance;
            instance.AudioManager.ApplyMusicCue(fightMusic, 0f, 0f, false);
        }

        //Variables used by helper functions

        const float RAYCAST_LENGTH = 0.08f;

        List<Vector2> topRays = new List<Vector2>();
        List<Vector2> rightRays = new List<Vector2>();
        List<Vector2> bottomRays = new List<Vector2>();
        List<Vector2> leftRays = new List<Vector2>();

        public struct DirectionSet
        {
            public bool above;
            public bool below;
            public bool right;
            public bool left;
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if( collision.gameObject.layer == 8 )
            {
                CheckTouching( 8 );
            }
        }

        void OnCollisionEnter2D( Collision2D collision )
        {
            if( collision.gameObject.layer == 8 )
            {
                CheckTouching( 8 );
            }
        }

        void OnCollisionExit2D( Collision2D collision )
        {
            if( collision.gameObject.layer == 8 )
            {
                CheckTouching( 8 );
            }
        }

        void FlipScale()
        {
            owner.transform.localScale = owner.transform.localScale.SetX(-owner.transform.localScale.x);
        }

        void FaceObject(GameObject objectToFace, bool spriteFacesRight = false, bool resetFrame = false, string playNewAnimation = "")
        {
            Dev.Where();
            Vector3 localScale = owner.transform.localScale;
            float xScale = localScale.x;

            if(owner.transform.position.x < objectToFace.transform.position.x)
            {
                if(spriteFacesRight)
                {
                    if(localScale.x != xScale)
                    {
                        localScale.x = xScale;
                        if(resetFrame)
                        {
                            this.tk2dAnimator.PlayFromFrame(0);
                        }
                        if(!string.IsNullOrEmpty(playNewAnimation))
                        {
                            this.tk2dAnimator.Play(playNewAnimation);
                        }
                    }
                }
                else if(localScale.x != -xScale)
                {
                    localScale.x = -xScale;
                    if(resetFrame)
                    {
                        this.tk2dAnimator.PlayFromFrame(0);
                    }
                    if(!string.IsNullOrEmpty(playNewAnimation))
                    {
                        this.tk2dAnimator.Play(playNewAnimation);
                    }
                }
            }
            else if(spriteFacesRight)
            {
                if(localScale.x != -xScale)
                {
                    localScale.x = -xScale;
                    if(resetFrame)
                    {
                        this.tk2dAnimator.PlayFromFrame(0);
                    }
                    if(!string.IsNullOrEmpty(playNewAnimation))
                    {
                        this.tk2dAnimator.Play(playNewAnimation);
                    }
                }
            }
            else if(localScale.x != xScale)
            {
                localScale.x = xScale;
                if(resetFrame)
                {
                    this.tk2dAnimator.PlayFromFrame(0);
                }
                if(!string.IsNullOrEmpty(playNewAnimation))
                {
                    this.tk2dAnimator.Play(playNewAnimation);
                }
            }
            owner.transform.localScale = localScale;
        }

        void FaceAngle(float offset)
        {
            Vector2 velocity = body.velocity;
            float z = Mathf.Atan2(velocity.y, velocity.x) * 57.2957764f + offset;
            owner.transform.localEulerAngles = new Vector3(0f, 0f, z);
        }
            
        void PlayAnimation(string animation)
        {
            Dev.Where();

            if( tk2dAnimator.GetClipByName( animation ) == null )
            {
                Dev.Log( "Warning: " + animation + " clip not found" );
                return;
            }

            tk2dAnimator.AnimationCompleted = null;
            tk2dAnimator.Play( animation );
        }

        IEnumerator PlayAndWaitForEndOfAnimation(string animation)
        {
            Dev.Where();

            if( tk2dAnimator.GetClipByName( animation ) == null )
            {
                Dev.Log( "Warning: " + animation + " clip not found" );
                yield break;
            }

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( animation );
            
            blockingAnimationIsPlaying = true;
            
            while( blockingAnimationIsPlaying )
            {
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        void OnAnimationComplete(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
        {
            Dev.Where();
            blockingAnimationIsPlaying = false;
            tk2dAnimator.AnimationCompleted = null;
        }
        
        void EnableCollisionsInDirection( bool up, bool down, bool left, bool right )
        {
            checkUp = up;
            checkDown = down;
            checkLeft = left;
            checkRight = right;

            //clear any hit flags we're no longer colliding with
            if( !up )
                topHit = false;
            if( !down )
                bottomHit = false;
            if( !left )
                leftHit = false;
            if( !right )
                rightHit = false;
        }

        void CheckTouching(LayerMask layer)
        {
            if(this.checkUp)
            {
                this.topRays.Clear();
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y));
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.max.y));
                this.topRays.Add(this.bodyCollider.bounds.max);
                this.topHit = false;
                for(int i = 0; i < 3; i++)
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(this.topRays[i], Vector2.up, 0.08f, 1 << layer);
                    if(raycastHit2D.collider != null)
                    {
                        this.topHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkRight)
            {
                this.rightRays.Clear();
                this.rightRays.Add(this.bodyCollider.bounds.max);
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.center.y));
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y));
                this.rightHit = false;
                for(int j = 0; j < 3; j++)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.rightRays[j], Vector2.right, 0.08f, 1 << layer);
                    if(raycastHit2D2.collider != null)
                    {
                        this.rightHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkDown)
            {
                Dev.Log( "Checking for downward collisions" );
                this.bottomRays.Clear();
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y));
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.min.y));
                this.bottomRays.Add(this.bodyCollider.bounds.min);
                this.bottomHit = false;
                
                for(int k = 0; k < 3; k++)
                {
                    RaycastHit2D raycastHit2D3 = Physics2D.Raycast(this.bottomRays[k], -Vector2.up, 0.08f, 1 << layer);
                    if(raycastHit2D3.collider != null)
                    {
                        Dev.Log( "Raycast down hit "+ raycastHit2D3.collider?.gameObject?.name );
                        this.bottomHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
                if(!bottomHit)
                {
                    Dev.Log( "Raycast down missed" );
                }
            }
            if(this.checkLeft)
            {
                this.leftRays.Clear();
                this.leftRays.Add(this.bodyCollider.bounds.min);
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.center.y));
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y));
                this.leftHit = false;
                for(int l = 0; l < 3; l++)
                {
                    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(this.leftRays[l], -Vector2.right, 0.08f, 1 << layer);
                    if(raycastHit2D4.collider != null)
                    {
                        this.leftHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
        }//end CheckTouching

        //if the values are within the tolerance, the object is not enough in that direction to be considered offset from us
        DirectionSet DoCheckDirection(GameObject target, float toleranceX = 0.1f, float toleranceY = 0.5f )
        {
            DirectionSet direction = new DirectionSet();
            float num = owner.transform.position.x;
            float num2 = owner.transform.position.y;
            float num3 = target.transform.position.x;
            float num4 = target.transform.position.y;

            direction.right = (num < num3) && Mathf.Abs( num - num3 ) > toleranceX;
            direction.left = (num > num3 ) && Mathf.Abs( num - num3 ) > toleranceX;
            direction.above = (num2 < num4 ) && Mathf.Abs( num2 - num4 ) > toleranceY;
            direction.below = (num2 > num4 ) && Mathf.Abs( num2 - num4 ) > toleranceY;

            return direction;
        }

        //TODO: change to a static function that takes 3 vectors, origin, target, and offsets
        float GetAngleToTarget(GameObject target, float offsetX, float offsetY)
        {
            float num = target.transform.position.y + offsetY - owner.transform.position.y;
            float num2 = target.transform.position.x + offsetX - owner.transform.position.x;
            float num3;
            for(num3 = Mathf.Atan2(num, num2) * 57.2957764f; num3 < 0f; num3 += 360f)
            {
            }
            return num3;
        }

        static Vector2 GetVelocityToTarget(Vector2 self, Vector2 projectile, Vector2 target, float speed, float spread = 0f)
        {
            float num = target.y + projectile.y - self.y;
            float num2 = target.x + projectile.x - self.x;
            float num3 = Mathf.Atan2(num, num2) * 57.2957764f;
            if(Mathf.Abs(spread) > Mathf.Epsilon)
            {
                num3 += GameRNG.Rand(-spread, spread);
            }
            float x = speed * Mathf.Cos(num3 * 0.0174532924f);
            float y = speed * Mathf.Sin(num3 * 0.0174532924f);
            Vector2 velocity;
            velocity.x = x;
            velocity.y = y;
            return velocity;
        }

        static Vector2 GetVelocityFromSpeedAndAngle(float speed, float angle)
        {
            float x = speed * Mathf.Cos(angle * 0.0174532924f);
            float y = speed * Mathf.Sin(angle * 0.0174532924f);
            Vector2 velocity;
            velocity.x = x;
            velocity.y = y;
            return velocity;
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


#if UNITY_EDITOR
        static UnityEngine.Audio.AudioMixerSnapshot GetSnapshotFromFSM(GameObject go, string fsmName, string stateName)
        {
            return null;
#else
        public static UnityEngine.Audio.AudioMixerSnapshot GetSnapshotFromFSM(GameObject go, string fsmName, string stateName)
        {
            var snapshot = go.GetFSMActionOnState<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>(stateName, fsmName);
            var mixerSnapshot = snapshot.snapshot.Value as UnityEngine.Audio.AudioMixerSnapshot;
            return mixerSnapshot;
#endif
        }

#if UNITY_EDITOR
        static object GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
            return null;
#else
        public static MusicCue GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
            var musicCue = go.GetFSMActionOnState<HutongGames.PlayMaker.Actions.ApplyMusicCue>(stateName, fsmName);
            MusicCue mc = musicCue.musicCue.Value as MusicCue;
            return mc;
#endif
        }

        //Setup functions///////////////////////////////////////////////////////////////////////////


        void SetupRequiredReferences()
        {
            owner = gameObject;
            bodyCollider = GetComponent<BoxCollider2D>();
            body = GetComponent<Rigidbody2D>();
            meshRenderer = GetComponent<MeshRenderer>();
            runAudioSource = GetComponent<AudioSource>();

            healthManager = GetComponent<HealthManager>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();

            if(gameObject.FindGameObjectInChildren("Refight Range") != null)
                refightRange = gameObject.FindGameObjectInChildren("Refight Range").AddComponent<RefightRange>();
            if(gameObject.FindGameObjectInChildren("Evade Range") != null)
                evadeRange = gameObject.FindGameObjectInChildren("Evade Range").AddComponent<EvadeRange>();
            if(gameObject.FindGameObjectInChildren("Sphere Range") != null)
                sphereRange = gameObject.FindGameObjectInChildren("Sphere Range").AddComponent<SphereRange>();
            if(gameObject.FindGameObjectInChildren("A Sphere Range") != null)
                aSphereRange = gameObject.FindGameObjectInChildren("A Sphere Range").AddComponent<SphereRange>();
            if(gameObject.FindGameObjectInChildren("A Dash Range") != null)
                aDashRange = gameObject.FindGameObjectInChildren("A Dash Range").AddComponent<ADashRange>();
            if(gameObject.FindGameObjectInChildren("Hit ADash") != null)
                hitADash = gameObject.FindGameObjectInChildren("Hit ADash").GetComponent<PolygonCollider2D>();
            if(gameObject.FindGameObjectInChildren("Hit GDash") != null)
                hitGDash = gameObject.FindGameObjectInChildren("Hit GDash").GetComponent<PolygonCollider2D>();
            if(gameObject.FindGameObjectInChildren("Dust HardLand") != null)
                dustHardLand = gameObject.FindGameObjectInChildren("Dust HardLand").GetComponent<ParticleSystem>();
            if(gameObject.FindGameObjectInChildren("Run Away Check") != null)
                runAwayCheck = gameObject.FindGameObjectInChildren("Run Away Check").AddComponent<RunAwayCheck>();

            
            if(gameObject.FindGameObjectInChildren("Throw Effect") != null)
                throwEffect = gameObject.FindGameObjectInChildren("Throw Effect").AddComponent<ThrowEffect>();
            if(gameObject.FindGameObjectInChildren("A Dash Effect") != null)
                aDashEffect = gameObject.FindGameObjectInChildren("A Dash Effect").AddComponent<ADashEffect>();
            if(gameObject.FindGameObjectInChildren("G Dash Effect") != null)
                gDashEffect = gameObject.FindGameObjectInChildren("G Dash Effect").AddComponent<GDashEffect>();
            if(gameObject.FindGameObjectInChildren("Sphere Ball") != null)
                sphereBall = gameObject.FindGameObjectInChildren("Sphere Ball").AddComponent<SphereBall>();
            if(gameObject.FindGameObjectInChildren("Flash Effect") != null)
                flashEffect = gameObject.FindGameObjectInChildren("Flash Effect").AddComponent<FlashEffect>();

            //TODO: replace this with a load from the effects database
            if(UnityEngine.SceneManagement.SceneManager.GetSceneByName("Fungus1_04_boss").FindGameObject("Needle") != null)
                needle = UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Needle" ).AddComponent<Needle>();

            if(GameObject.Find("Needle Tink") != null)
                needleTink = GameObject.Find("Needle Tink").AddComponent<NeedleTink>();

#if UNITY_EDITOR
            healthManager = gameObject.AddComponent<HealthManager>();
            tk2dAnimator = gameObject.AddComponent<tk2dSpriteAnimator>();
            evadeRange = gameObject.AddComponent<EvadeRange>();
            sphereRange = gameObject.AddComponent<SphereRange>();
            refightRange = gameObject.AddComponent<RefightRange>();
            needle = new GameObject("Needle").AddComponent<Needle>();
            needleTink = new GameObject("Needle Tink").AddComponent<NeedleTink>();
#endif
        }

        IEnumerator ExtractReferencesFromPlayMakerFSMs()
        {
            //load resources for the boss
            string bossFSMName = "Control";

            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Sphere", SetHornetSphereSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Wall L", SetHornetWallLandSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Fire", SetHornetDashSFX);
            yield return GetAudioPlayerOneShotClipsFromFSM(owner, bossFSMName, "ADash Antic", SetHornetAGDashYells);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Land", SetHornetLandSFX);
            yield return GetAudioPlayerOneShotClipsFromFSM(owner, bossFSMName, "Jump", SetHornetJumpYells);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Jump", SetHornetJumpSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Evade Land", SetHornetGroundLandSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Evade", SetHornetSmallJumpSFX);
            yield return GetAudioPlayerOneShotClipsFromFSM(owner, bossFSMName, "Evade", SetHornetLaughs);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Throw Recover", SetHornetCatchSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, bossFSMName, "Throw", SetHornetThrowSFX);
            yield return GetAudioPlayerOneShotClipsFromFSM(owner, bossFSMName, "Throw Antic", SetHornetAttackYells);
            yield return GetGameObjectFromFSM(owner, bossFSMName, "Flourish", SetAreaTitleReference);
            yield return GetAudioSourceObjectFromFSM(owner, bossFSMName, "Flourish", SetActorAudioSource);
            yield return GetAudioClipFromFSM(owner, bossFSMName, "Flourish", SetHornetYell);
            fightMusic = GetMusicCueFromFSM(owner, bossFSMName, "Flourish");
            fightMusicSnapshot = GetSnapshotFromFSM(owner, bossFSMName, "Flourish");

            //load resources for additional objects

            //TODO: load the things for the needle (i think)

            yield break;
        }

        void SetActorAudioSource(AudioSource source)
        {
            if(source == null)
            {
                Dev.Log("Warning: Actor AudioSource failed to load and is null!");
                return;
            }

            actorAudioSource = source;
            actorAudioSource.transform.SetParent(owner.transform);
            actorAudioSource.transform.localPosition = Vector3.zero;
        }

        void SetHornetSphereSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet sphere sfx clip is null!");
                return;
            }

            hornetSphereSFX = clip;
        }

        void SetHornetWallLandSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet wall land sfx clip is null!");
                return;
            }

            hornetWallLandSFX = clip;
        }

        void SetHornetDashSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet dash sfx clip is null!");
                return;
            }

            hornetDashSFX = clip;
        }

        void SetHornetAGDashYells(List<AudioClip> clips)
        {
            if(clips == null)
            {
                Dev.Log("Warning: hornet ag dash yells are null clips!");
                return;
            }

            hornetAGDashYells = clips;
        }

        void SetHornetLandSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet land sfx clip is null!");
                return;
            }

            hornetLandSFX = clip;
        }

        void SetHornetJumpSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet jump sfx clip is null!");
                return;
            }

            hornetJumpSFX = clip;
        }

        void SetHornetJumpYells(List<AudioClip> clips)
        {
            if(clips == null)
            {
                Dev.Log("Warning: hornet jump yells are null clips!");
                return;
            }

            hornetJumpYells = clips;
        }

        void SetHornetSmallJumpSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet small jump sfx clip is null!");
                return;
            }

            hornetSmallJumpSFX = clip;
        }

        void SetHornetGroundLandSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet ground land sfx clip is null!");
                return;
            }

            hornetGroundLandSFX = clip;
        }

        void SetHornetThrowSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet throw sfx clip is null!");
                return;
            }

            hornetThrowSFX = clip;
        }

        void SetHornetCatchSFX(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet catch sfx clip is null!");
                return;
            }

            hornetCatchSFX = clip;
        }

        void SetHornetYell(AudioClip clip)
        {
            if(clip == null)
            {
                Dev.Log("Warning: hornet yell clip is null!");
                return;
            }

            hornetYell = clip;
        }

        void SetHornetAttackYells(List<AudioClip> clips)
        {
            if(clips == null)
            {
                Dev.Log("Warning: hornet throw yells are null clips!");
                return;
            }

            hornetAttackYells = clips;
        }

        void SetHornetLaughs(List<AudioClip> clips)
        {
            if(clips == null)
            {
                Dev.Log("Warning: hornet laughs are null clips!");
                return;
            }

            hornetLaughs = clips;
        }

        void SetAreaTitleReference(GameObject areaTitle)
        {
            if(areaTitle == null)
            {
                Dev.Log("Warning: Area Title GameObject failed to load and is null!");
                return;
            }

            AreaTitle title = areaTitle.GetComponent<AreaTitle>();

            foreach(PlayMakerFSM p in areaTitle.GetComponentsInChildren<PlayMakerFSM>())
            {
                GameObject.DestroyImmediate(p);
            }

            GameObject.DestroyImmediate(title);

            areaTitleObject = areaTitle;
            areaTitleObject.SetActive(false);
        }

        void RemoveDeprecatedComponents()
        {
#if UNITY_EDITOR
#else
            foreach( PlayMakerFSM p in owner.GetComponentsInChildren<PlayMakerFSM>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerUnity2DProxy p in owner.GetComponentsInChildren<PlayMakerUnity2DProxy>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFixedUpdate p in owner.GetComponentsInChildren<PlayMakerFixedUpdate>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( DeactivateIfPlayerdataTrue p in owner.GetComponentsInChildren<DeactivateIfPlayerdataTrue>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFSM p in needle.GetComponentsInChildren<PlayMakerFSM>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFSM p in needleTink.GetComponentsInChildren<PlayMakerFSM>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFixedUpdate p in needle.GetComponentsInChildren<PlayMakerFixedUpdate>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( DeactivateIfPlayerdataTrue p in needleTink.GetComponentsInChildren<DeactivateIfPlayerdataTrue>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTweenFSMEvents p in owner.GetComponentsInChildren<iTweenFSMEvents>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in owner.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in owner.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
#endif
        }

        //end helpers /////////////////////////////
    }//end class
}//end namespace
