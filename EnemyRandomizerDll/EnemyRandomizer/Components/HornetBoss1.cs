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
        public bool objectIsInRange;
        public bool IsOnCooldown { get; private set; }

        BoxCollider2D bodyCollider;

        public void DisableEvadeForTime(float disableTime)
        {
            if(IsOnCooldown)
                return;

            StartCoroutine(EnableEvadeAfterTime(disableTime));
        }

        IEnumerator EnableEvadeAfterTime(float time)
        {
            IsOnCooldown = true;
            bodyCollider = GetComponent<BoxCollider2D>();
            bodyCollider.enabled = false;
            yield return new WaitForSeconds(time);
            bodyCollider.enabled = true;
            IsOnCooldown = false;
        }

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            objectIsInRange = false;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class RunAwayCheck : MonoBehaviour
    {
        public bool objectIsInRange;

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            objectIsInRange = false;
        }
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class RefightRange : MonoBehaviour
    {
        public bool objectIsInRange;

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            objectIsInRange = false;
        }
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class SphereRange : MonoBehaviour
    {
        public bool objectIsInRange;

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            objectIsInRange = false;
        }
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class ADashRange : MonoBehaviour
    {
        public bool objectIsInRange;

        public void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D(Collider2D collisionInfo)
        {
            objectIsInRange = false;
        }
    }

    public class FlashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

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
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish

            currentState = Complete();

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

    public class ADashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

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
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish

            currentState = Complete();

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


    public class GDashEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

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
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish

            currentState = Complete();

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

    public class ThrowEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

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
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish

            currentState = Complete();

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

    public class SphereBall : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

            gameObject.SetActive(true);

            isAnimating = true;

            //TODO: move the sphere starting size and growing size/rate to variables
            transform.localScale = new Vector3(.8f, .8f, 1f);

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
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
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

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        public void Play(GameObject parent)
        {
            this.parent = parent;
            tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
            bodyCollider = gameObject.GetComponent<PolygonCollider2D>();
            body = gameObject.GetComponent<Rigidbody2D>();

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

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();

            //TODO: finish

            currentState = Complete();

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
        public float jumpDistance = 10f;
        public float jumpVelocityY = 41f;
        public float minAirSphereHeight = 5f;
        public float normGravity2DScale = 1.5f;
        public float normShortJumpGravity2DScale = 2f;
        public float airFireSpeed = 12f; //TODO: look this up from state Fire in action FireAtTarget

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
        IEnumerator currentState = null;

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
            currentState = Init();

            for(;;)
            {
                if(owner == null)
                    yield break;

                yield return currentState;

                //Dev.Log("Next");

                //TODO: remove as the states get implemented
                //yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Init()
        {
            Dev.Where();
            body.gravityScale = normGravity2DScale;

            currentState = Inert();

            yield break;
        }


        IEnumerator Inert()
        {
            Dev.Where();
            int test = GameManager.instance.playerData.GetInt("hornetGreenpath");

            if(test >= 4)
            {
                currentState = RefightReady();
            }
            else
            {
                while(!wake)
                {
                    yield return new WaitForEndOfFrame();
                }

                currentState = Wake();
            }

            yield break;
        }

        IEnumerator Wake()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            currentState = Flourish();

            yield break;
        }

        //the start of the fight!
        IEnumerator Flourish()
        {
            Dev.Where();

            SetupDefaultParams();

            //close the gates
            SetFightGates(true);

            ShowBossTitle(2f, "", "", "", "HORNET");
            
            PlayOneShot(hornetYell);

            PlayBossMusic();

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Flourish");

            currentState = Idle();

            yield break;
        }

        IEnumerator Idle()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;
            FaceObject(hero.gameObject, false, false);

            airDashPause = 999f;

            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            tk2dAnimator.Play("Idle");

            body.velocity = Vector2.zero;

            if(evadeRange.objectIsInRange)
            {
                currentState = EvadeAntic();
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
                        currentState = DmgResponse();
                        yield break;
                    }

                    randomDelay -= Time.deltaTime;
                }

                //nothing hit us, choose the next state with 50/50
                List<Func<IEnumerator>> nextStates = new List<Func<IEnumerator>>()
                {
                    MaybeFlip, MaybeGSphere
                };

                bool flag = false;
                while(!flag)
                {
                    int randomWeightedIndex = GameRNG.Rand(0, nextStates.Count - 1);
                    if(randomWeightedIndex == 0 && ctIdle < 2)
                    {
                        ctIdle += 1;
                        ctRun = 0;
                        currentState = nextStates[0].Invoke();
                        flag = true;
                    }
                    else if(randomWeightedIndex == 1 && ctRun < 2)
                    {
                        ctIdle = 0;
                        ctRun += 1;
                        currentState = nextStates[1].Invoke();
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
            bool nextState = GameRNG.CoinToss();
            if(nextState)
            {
                FlipScale();
            }

            currentState = RunAway();

            yield break;
        }

        IEnumerator RunAway()
        {
            Dev.Where();
            
            //TODO: play around with this, something seems off about it
            if(runAwayCheck.objectIsInRange)
            {
                //face the knight
                HeroController hero = HeroController.instance;
                FaceObject(hero.gameObject, false, false);

                //then flip the other way
                FlipScale();
            }

            currentState = RunAntic();

            yield break;
        }

        IEnumerator RunAntic()
        {
            Dev.Where();
                        
            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Evade Antic");

            currentState = Run();

            yield break;
        }

        IEnumerator Run()
        {
            Dev.Where();

            runAudioSource.Play();

            float xVel = owner.transform.localScale.x * runSpeed;

            tk2dAnimator.Play("Run");
            body.velocity = new Vector2(xVel, 0f);

            //do this by default
            currentState = MaybeGSphere();

            float randomDelay = GameRNG.Rand(runWaitMin, runWaitMax);
            while(randomDelay > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did something hit us?
                if(wasHitRecently)
                {
                    currentState = DmgResponse();
                    break;
                }

                if(evadeRange.objectIsInRange)
                {
                    currentState = EvadeAntic();
                    break;
                }

                if(rightHit || leftHit)
                {
                    currentState = MaybeGSphere();
                    break;
                }

                randomDelay -= Time.deltaTime;
            }

            runAudioSource.Stop();

            yield break;
        }

        IEnumerator MaybeGSphere()
        {
            Dev.Where();

            runAudioSource.Stop();

            if(sphereRange.objectIsInRange)
            {
                float randomChoice = GameRNG.Randf();
                if(randomChoice > chanceToThrow)
                {
                    currentState = SphereAnticG();
                }
                else
                {
                    currentState = CanThrow();
                }
            }
            else
            {
                currentState = CanThrow();
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
                currentState = MoveChoiceB();
            }
            else
            {
                //we can throw!
                currentState = MoveChoiceA();
            }

            yield break;
        }

        //"can throw"
        IEnumerator MoveChoiceA()
        {
            Dev.Where();

            currentState = ThrowAntic();

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
                        currentState = SetADash();
                    }
                    if(num == 1)
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        currentState = SetSphereA();
                    }
                    if(num == 2)
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        currentState = GDashAntic();
                    }
                    if(num == 3)
                    {
                        msThrow = 0;
                        ctThrow = 1;
                        currentState = ThrowAntic();
                    }
                }
                //else, randomly pick a skill to use
                else if(randomWeightedIndex == 0 && ctAirDash < 2)
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow = 0;
                    currentState = SetADash();
                    flag = true;
                }
                else if(randomWeightedIndex == 1 && ctASphere < 1)
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    ctThrow = 0;
                    currentState = SetSphereA();
                    flag = true;
                }
                else if(randomWeightedIndex == 2 && ctGDash < 2)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    ctThrow = 0;
                    currentState = GDashAntic();
                    flag = true;
                }
                else if(randomWeightedIndex == 3 && ctThrow < 1)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow += 1;
                    currentState = ThrowAntic();
                    flag = true;
                }
            }

            yield break;
        }

        //"cannot throw"
        IEnumerator MoveChoiceB()
        {
            Dev.Where();

            currentState = SetSphereA();

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
                        currentState = SetADash();
                    }
                    if(num == 1)
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        currentState = SetSphereA();
                    }
                    if(num == 2)
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        currentState = GDashAntic();
                    }
                }
                //else, randomly pick a skill to use
                else if(randomWeightedIndex == 0 && ctAirDash < 2)
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    currentState = SetADash();
                    flag = true;
                }
                else if(randomWeightedIndex == 1 && ctASphere < 1)
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    currentState = SetSphereA();
                    flag = true;
                }
                else if(randomWeightedIndex == 2 && ctGDash < 2)
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    currentState = GDashAntic();
                    flag = true;
                }
            }

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
            FaceObject(hero.gameObject);

            //stop moving
            body.velocity = Vector2.zero;
                        
            PlayOneShotRandom(hornetAttackYells);

            //play throwing animation
            //wait here until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Throw Antic");

            currentState = MaybeLock();

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
                currentState = LockR();
            }
            else if(angleToTarget <= 180f)
            {
                currentState = LockL();
            }
            else if(angleToTarget <= 270f)
            {
                currentState = LockUL();
            }
            else if(angleToTarget <= 360f)
            {
                currentState = LockUR();
            }
            else
            {
                currentState = Throw();
            }

            yield break;
        }

        IEnumerator LockL()
        {
            Dev.Where();

            nextThrowAngle = 180f;
            currentState = Throw();

            yield break;
        }

        IEnumerator LockR()
        {
            Dev.Where();

            nextThrowAngle = 0f;
            currentState = Throw();

            yield break;
        }

        IEnumerator LockUL()
        {
            Dev.Where();

            nextThrowAngle = 180f;
            currentState = Throw();

            yield break;
        }

        IEnumerator LockUR()
        {
            Dev.Where();

            nextThrowAngle = 0f;
            currentState = Throw();

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

            //start throwing the needle
            needle.Play(owner);

            //put the needle tink on the needle
            needleTink.SetParent(needle.transform);

            //start the throw animation
            tk2dAnimator.Play("Throw");

            //wait one frame before ending
            yield return new WaitForEndOfFrame();

            currentState = Thrown();

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

            currentState = ThrowRecover();

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

            currentState = Escalation();

            yield break;
        }

        IEnumerator SetADash()
        {
            Dev.Where();

            airDashPause = GameRNG.Rand(airDashPauseMin, airDashPauseMax);
            willSphere = false;

            currentState = JumpAntic();

            yield break;
        }

        IEnumerator JumpAntic()
        {
            Dev.Where();

            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            body.velocity = Vector2.zero;
                        
            FaceObject(HeroController.instance.gameObject);

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Jump Antic");

            currentState = AimJump();

            yield break;
        }

        IEnumerator AimJump()
        {
            Dev.Where();

            if(willSphere)
            {
                currentState = AimSphereJump();
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
                    currentState = ReAim();
                }
                else
                {
                    currentState = Jump();
                }
            }

            yield break;
        }

        IEnumerator Jump()
        {
            Dev.Where();
            //TODO
            PlayOneShotRandom(hornetJumpYells);
            PlayOneShot(hornetJumpSFX);

            tk2dAnimator.Play("Jump");

            //TODO: this seems weird, see how it turns out
            body.velocity = new Vector2(jumpPoint, jumpVelocityY);

            currentState = InAir();

            yield break;
        }

        IEnumerator InAir()
        {
            Dev.Where();

            //change collision check directions for jumping
            checkUp = false;
            checkDown = true;
            checkLeft = false;
            checkRight = false;

            float startHeight = owner.transform.position.y;

            float waitTimer = airDashPause;
            while(waitTimer > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if(bottomHit)
                {
                    currentState = Land();
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            if(waitTimer <= 0f)
            {
                currentState = ADashAntic();
            }
            else if(!bottomHit)
            {
                while(!bottomHit)
                {
                    if(body.velocity.y > 0f && Mathf.Abs(owner.transform.position.y - startHeight) > minAirSphereHeight)
                    {
                        currentState = MaybeDoSphere();
                        break;
                    }

                    yield return new WaitForEndOfFrame();

                    //did we hit a wall? end evade timer early
                    if(bottomHit)
                    {
                        currentState = Land();
                        break;
                    }
                }
            }

            //restore collision check directions
            checkUp = false;
            checkDown = false;
            checkLeft = true;
            checkRight = true;

            yield break;
        }

        //TODO: this looks redundant, look into changing all re-aims to just call AirJump() again
        IEnumerator ReAim()
        {
            Dev.Where();
            
            currentState = AimJump();

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

            currentState = Escalation();

            yield break;
        }

        IEnumerator ADashAntic()
        {
            Dev.Where();

            if(aDashRange.objectIsInRange)
            {
                HeroController hero = HeroController.instance;
                float angleToTarget = GetAngleToTarget(hero.gameObject, 0f, -.5f);

                bodyCollider.offset = new Vector2(1.1f, -.9f);
                bodyCollider.size = new Vector2(1.2f, 1.4f);

                body.gravityScale = 0f;

                PlayOneShotRandom(hornetAGDashYells);

                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation("A Dash Antic");

                currentState = Fire();
            }
            else
            {
                currentState = InAir();
            }
            
            yield break;
        }

        IEnumerator Fire()
        {
            Dev.Where();

            PlayOneShot(hornetDashSFX);
            
            //TODO: check what SetBoxColliderTrigger is doing here
            //it's getting a collider and enabling/disabling a trigger.... see which one and which value

            GameObject hero = HeroController.instance.gameObject;

            //TODO: check the spread and speed on FireAtTarget
            Vector2 pos = owner.transform.position;
            Vector2 fireVelocity = GetVelocityToTarget(pos, pos, hero.transform.position, airFireSpeed, 0f);

            body.velocity = fireVelocity;

            //TODO: check what SetVelocityAsAngle is doing and fill these in
            float altFireSpeed = 10f;
            float altAngle = 0f;
            Vector2 otherVelocity = GetVelocityFromSpeedAndAngle(altFireSpeed, altAngle);
            
            bodyCollider.offset = new Vector2(.1f, 0f);
            bodyCollider.size = new Vector2(1.5f, 1.0f);

            hitADash.gameObject.SetActive(true);

            //TODO: check to see if this is really applying to the owner
            owner.transform.localScale = owner.transform.localScale.SetX(1f);
            owner.transform.localScale = owner.transform.localScale.SetY(1f);
            
            //TODO: check FaceAngle to see what objects this is using....
            FaceAngle(hero, 0f);

            //TODO: check GetRotation to see what values/objects this is using....
            Vector3 eulerAngles = owner.transform.eulerAngles;
            float zAngle = eulerAngles.z;

            if(zAngle > 90f && zAngle < 270f)
            {
                currentState = FiringR();
            }
            else
            {
                currentState = FiringL();
            }

            yield break;
        }

        IEnumerator FiringL()
        {
            Dev.Where();

            returnXScale = 1f;

            currentState = ADash();

            yield break;
        }

        IEnumerator FiringR()
        {
            Dev.Where();

            owner.transform.localScale = owner.transform.localScale.SetY(-1f);

            returnXScale = -1f;

            currentState = ADash();

            yield break;
        }

        IEnumerator ADash()
        {
            Dev.Where();

            aDashEffect.Play(owner);

            PlayOneShot(hornetDashSFX);

            DoEnemyKillShakeEffect();

            airDashPause = 999;

            Vector3 position = owner.transform.position;
            Vector3 down = Vector3.down;
            Vector3 up = Vector3.up;
            Vector3 left = Vector3.up;
            Vector3 right = Vector3.up;
            float nearDist = 1f;

            bool earlyOut = false;

            currentState = LandY();

            tk2dAnimator.Play("A Dash");

            //TODO: check to see if these FloatCompare's are EveryFrame (and also check the GetPosition)
            while(!earlyOut)
            {
                float closeToSurface = .55f;
                if(!earlyOut)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, down, nearDist, 1 << 8);
                    if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.y - position.y) < closeToSurface)
                    {
                        currentState = LandY();
                    }
                }
                if(!earlyOut)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, up, nearDist, 1 << 8);
                    if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.y - position.y) < closeToSurface)
                    {
                        currentState = HitRoof();
                    }
                }
                if(!earlyOut)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, left, nearDist, 1 << 8);
                    if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.x - position.x) < closeToSurface)
                    {
                        currentState = WallL();
                    }
                }
                if(!earlyOut)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(position, right, nearDist, 1 << 8);
                    if(raycastHit2D2.collider != null && Mathf.Abs(raycastHit2D2.collider.transform.position.x - position.x) < closeToSurface)
                    {
                        currentState = WallR();
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        void DoWallLand(float xScale)
        {
            PlayOneShot(hornetWallLandSFX);

            owner.transform.rotation = Quaternion.identity;

            body.velocity = Vector2.zero;

            owner.transform.localScale = owner.transform.localScale.SetX(xScale);

            //TODO: check SetPosition to see what object name it's setting here....

            hitADash.gameObject.SetActive(false);

            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);
        }

        IEnumerator WallL()
        {
            Dev.Where();

            DoWallLand(1f);

            yield return PlayAndWaitForEndOfAnimation("Wall Impact");

            currentState = JumpR();

            yield break;
        }

        IEnumerator WallR()
        {
            Dev.Where();

            DoWallLand(-1f);

            yield return PlayAndWaitForEndOfAnimation("Wall Impact");

            currentState = JumpL();

            yield break;
        }

        void DoShortJump(float xDirection)
        {
            float xScale = -1f * Mathf.Sign(xDirection);

            body.velocity = new Vector2(jumpDistance * xDirection, jumpVelocityY * .5f);

            tk2dAnimator.Play("Jump");

            body.gravityScale = normShortJumpGravity2DScale;

            owner.transform.localScale = owner.transform.localScale.SetX(xScale);
        }

        IEnumerator JumpL()
        {
            Dev.Where();

            DoShortJump(-1f);

            currentState = InAir();

            yield break;
        }

        IEnumerator JumpR()
        {
            Dev.Where();

            DoShortJump(1f);

            currentState = InAir();

            yield break;
        }

        IEnumerator LandY()
        {
            Dev.Where();

            //TODO: check the "SetPosition" action to see what object it's trying to set

            //TODO: check the SetScale action to see what it's trying to set

            hitADash.gameObject.SetActive(false);

            currentState = HardLand();

            yield break;
        }

        IEnumerator HitRoof()
        {
            Dev.Where();

            //TODO: check the SetScale action to see what it's trying to set

            hitADash.gameObject.SetActive(false);

            //TODO: check the "SetPosition" action to see what object it's trying to set

            //TODO: check the SetVelocity2d to see the name of the object/value it's trying to set

            body.velocity = Vector2.zero;

            //TODO: check SetBoxColliderTrigger to see what it's trying to set

            body.gravityScale = normShortJumpGravity2DScale;
            
            bodyCollider.offset = new Vector2(.1f, -0.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            owner.transform.rotation = Quaternion.identity;
            
            currentState = InAir();

            yield break;
        }

        IEnumerator HardLand()
        {
            Dev.Where();

            //TODO: check PlayParticleEmitter the particle system and the emit value
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
                //TODO: add the "IsNone" check to the DecelerateXY printing

                //TODO: check what SetVelocity2d is doing here

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


            currentState = Escalation();

            yield break;
        }

        IEnumerator MaybeDoSphere()
        {
            Dev.Where();

            willSphere = false;

            if(aSphereRange.objectIsInRange)
            {
                currentState = SphereAnticA();
            }
            else
            {
                currentState = InAir();
            }

            yield break;
        }

        IEnumerator SphereAnticA()
        {
            Dev.Where();

            willSphere = false;

            body.gravityScale = 0f;

            //TODO: check DecelerateV2 to see what it's doing

            FaceObject(HeroController.instance.gameObject);
            
            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Sphere Antic A");

            blockingAnimationIsPlaying = true;

            PlayOneShotRandom(hornetAttackYells);

            float deceleration = .8f;
            for(;;)
            {
                //TODO: add the "IsNone" check to the DecelerateV2 printing

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

            currentState = SphereA();

            yield break;
        }

        IEnumerator SphereA()
        {
            Dev.Where();

            PlayOneShot(hornetSphereSFX);

            sphereBall.Play(owner);
            flashEffect.Play(owner);

            DoEnemyKillShakeEffect();

            tk2dAnimator.Play("Sphere Attack");

            //TODO: move to variables
            float waitTime = 1f;
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

            //TODO
            currentState = SphereRecoverA();

            yield break;
        }

        IEnumerator SphereRecoverA()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation("Sphere Recover A");

            sphereBall.Stop();

            currentState = SphereAEnd();

            yield break;
        }

        IEnumerator SphereAEnd()
        {
            Dev.Where();

            body.gravityScale = normGravity2DScale;

            tk2dAnimator.Play("Fall");

            currentState = InAir();

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

            currentState = Jump();

            yield break;
        }

        IEnumerator SetJumpOnly()
        {
            Dev.Where();

            airDashPause = 999f;

            willSphere = false;

            currentState = JumpAntic();

            yield break;
        }

        IEnumerator SetSphereA()
        {
            Dev.Where();

            willSphere = true;

            airDashPause = 999f;

            currentState = JumpAntic();

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
            
            currentState = GDash();

            yield break;
        }

        IEnumerator GDash()
        {
            Dev.Where();

            tk2dAnimator.Play("G Dash");
            
            PlayOneShot(hornetDashSFX);

            DoEnemyKillShakeEffect();

            gDashEffect.Play(owner);

            bodyCollider.offset = new Vector2(0.1f, -.8f);
            bodyCollider.size = new Vector2(1.6f, 1.5f);

            hitGDash.gameObject.SetActive(true);

            //TODO: see what get scale is doing here

            //TODO: see what FloatOperator is doing here (-25?)

            //TODO: see what BoolTest with Multiply is doing here?

            body.velocity = new Vector2(-25f * owner.transform.localScale.x,0f);

            float waitTimer = .35f;
            while(waitTimer > 0f)
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end dash timer early
                if(rightHit || leftHit)
                {
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            currentState = GDashRecover1();

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

            currentState = GDashRecover2();

            yield break;
        }

        IEnumerator GDashRecover2()
        {
            Dev.Where();

            tk2dAnimator.Play("G Dash Recover2");

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

            currentState = Escalation();

            yield break;
        }

        IEnumerator SphereAnticG()
        {
            Dev.Where();
            
            body.velocity = Vector2.zero;
            
            bodyCollider.offset = new Vector2(0.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);
            
            FaceObject(HeroController.instance.gameObject);

            PlayOneShotRandom(hornetAttackYells);

            yield return PlayAndWaitForEndOfAnimation("Sphere Antic G");

            currentState = Sphere();

            yield break;
        }

        IEnumerator Sphere()
        {
            Dev.Where();
            PlayOneShot(hornetSphereSFX);

            sphereBall.Play(owner);
            flashEffect.Play(owner);

            DoEnemyKillShakeEffect();

            tk2dAnimator.Play("Sphere Attack");

            //TODO: move the wait value into a variable
            yield return new WaitForSeconds(1f);

            currentState = SphereRecover();

            yield break;
        }

        IEnumerator SphereRecover()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation("Sphere Recover G");

            sphereBall.Stop();

            currentState = Escalation();

            yield break;
        }

        IEnumerator EvadeAntic()
        {
            Dev.Where();

            runAudioSource.Stop();

            if(evadeRange.objectIsInRange)
            {
                //put her evade on cooldown
                float randomDelay = GameRNG.Rand(evadeCooldownMin, evadeCooldownMax);
                evadeRange.DisableEvadeForTime(randomDelay);

                //stop her moving
                body.velocity = Vector2.zero;

                //make her face you
                HeroController hero = HeroController.instance;
                FaceObject(hero.gameObject);

                //animate the evade-anticipation                
                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation("Evade Antic");

                currentState = Evade();
            }
            else
            {
                currentState = MaybeGSphere();
            }

            yield break;
        }

        IEnumerator Evade()
        {
            Dev.Where();

            PlayOneShotRandom(hornetLaughs);
            PlayOneShot(hornetSmallJumpSFX);

            tk2dAnimator.Play("Evade");

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

            currentState = EvadeLand();

            yield break;
        }

        IEnumerator EvadeLand()
        {
            Dev.Where();

            //animate the evade-landing            
            body.velocity = Vector2.zero;

            PlayOneShot(hornetGroundLandSFX);

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation("Evade Land");

            currentState = AfterEvade();

            yield break;
        }

        IEnumerator AfterEvade()
        {
            Dev.Where();

            bool attack = GameRNG.CoinToss();
            if(attack)
            {
                currentState = MaybeGSphere();
            }
            else
            {
                currentState = Idle();
            }

            yield break;
        }

        IEnumerator DmgResponse()
        {
            Dev.Where();

            runAudioSource.Stop();

            int choice = GameRNG.WeightedRand(DmgResponseChoices.Values.ToList());
            currentState = DmgResponseChoices.Keys.ToList()[choice].Invoke();

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

            currentState = MaybeGSphere();

            yield break;
        }


        IEnumerator RefightReady()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            tk2dAnimator.Play("Idle");

            //wait for player to get close
            while(!refightRange.objectIsInRange)
            {
                yield return new WaitForEndOfFrame();
            }

            currentState = RefightWake();

            yield break;
        }

        //TODO: state seems redundant, look into removing
        IEnumerator RefightWake()
        {
            Dev.Where();

            //TODO: activate all children? this is probably a state machine thing that we don't need anymore

            currentState = Flourish();

            yield break;
        }

        //TODO: hook something up to cause a transition to here
        IEnumerator StunStart()
        {
            Dev.Where();
            //TODO
            currentState = StunAir();

            yield break;
        }

        IEnumerator StunAir()
        {
            Dev.Where();
            //TODO
            currentState = StunLand();

            yield break;
        }

        IEnumerator StunLand()
        {
            Dev.Where();
            //TODO
            currentState = StunRecover();

            yield break;
        }

        IEnumerator StunRecover()
        {
            Dev.Where();
            //TODO
            currentState = SetJumpOnly();

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

            currentState = Idle();

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
            if(collision.gameObject.layer == 8)
            {
                CheckTouching(8);
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

        void FaceAngle(GameObject target, float offset)
        {
            Vector2 velocity = body.velocity;
            float z = Mathf.Atan2(velocity.y, velocity.x) * 57.2957764f + offset;
            target.transform.localEulerAngles = new Vector3(0f, 0f, z);
        }

        IEnumerator PlayAndWaitForEndOfAnimation(string animation)
        {
            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Flourish");

            blockingAnimationIsPlaying = true;

            for(;;)
            {
                if(!blockingAnimationIsPlaying)
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        void OnAnimationComplete(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
        {
            Dev.Where();            
            blockingAnimationIsPlaying = false;
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
                        this.bottomHit = true;
                        //TODO: call a callback here
                        break;
                    }
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

        DirectionSet DoCheckDirection(GameObject target)
        {
            DirectionSet direction = new DirectionSet();
            float num = owner.transform.position.x;
            float num2 = owner.transform.position.y;
            float num3 = target.transform.position.x;
            float num4 = target.transform.position.y;

            direction.right = (num < num3);
            direction.left = (num > num3);
            direction.above = (num2 < num4);
            direction.below = (num2 > num4);

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
            if(GameObject.Find("Needle") != null)
                needle = GameObject.Find("Needle").AddComponent<Needle>();

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
            foreach( PlayMakerFSM p in owner.GetComponentsInChildren<PlayMakerFSM>() )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerUnity2DProxy p in owner.GetComponentsInChildren<PlayMakerUnity2DProxy>() )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFixedUpdate p in owner.GetComponentsInChildren<PlayMakerFixedUpdate>() )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( DeactivateIfPlayerdataTrue p in owner.GetComponentsInChildren<DeactivateIfPlayerdataTrue>() )
            {
                GameObject.DestroyImmediate( p );
            }            
#endif
        }

        //end helpers /////////////////////////////
    }//end class
}//end namespace
