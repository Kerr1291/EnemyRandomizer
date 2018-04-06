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
            if( IsOnCooldown )
                return;

            StartCoroutine( EnableEvadeAfterTime( disableTime ) );
        }

        IEnumerator EnableEvadeAfterTime(float time)
        {
            IsOnCooldown = true;
            bodyCollider = GetComponent<BoxCollider2D>();
            bodyCollider.enabled = false;
            yield return new WaitForSeconds( time );
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

    [RequireComponent( typeof( BoxCollider2D ) )]
    public class RefightRange : MonoBehaviour
    {
        public bool objectIsInRange;

        public void OnTriggerEnter2D( Collider2D collisionInfo )
        {
            objectIsInRange = true;
        }

        public void OnTriggerExit2D( Collider2D collisionInfo )
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

        public tk2dSpriteAnimator tk2dAnimator;
        public HealthManager healthManager;
        public EvadeRange evadeRange;
        public SphereRange sphereRange;
        public ThrowEffect throwEffect;
        public AudioSource runAudioSource;
        public RefightRange refightRange;

        //hornet's projectile weapon & the tink effect that goes with it
        public Needle needle;
        public NeedleTink needleTink;

        //use for some sound effects
        public AudioSource actorAudioSource;
        public GameObject areaTitleObject;

        //used to play the boss background music
        //TODO: uncomment in real build and remove the "object" version
        public AudioClip hornetYell;
        public List<AudioClip> hornetThrowYells;
        public AudioClip hornetThrowSFX;
        public AudioClip hornetCatchSFX;
        public List<AudioClip> hornetLaughs;
        public AudioClip hornetSmallJumpSFX;
        public AudioClip hornetGroundLandSFX;
        public AudioClip hornetJumpSFX;
        public List<AudioClip> hornetJumpYells;
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

        Dictionary< Func<IEnumerator>, float > dmgResponseChoices;
        public Dictionary< Func<IEnumerator>, float > DmgResponseChoices {
            get {
                if( dmgResponseChoices == null )
                {
                    dmgResponseChoices = new Dictionary<Func<IEnumerator>, float>();
                    dmgResponseChoices.Add( EvadeAntic, .3f );
                    dmgResponseChoices.Add( SetJumpOnly, .15f );
                    dmgResponseChoices.Add( MaybeGSphere, .15f );
                    dmgResponseChoices.Add( DmgIdle, .4f );
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
        Func<IEnumerator> onAnimationCompleteNextState = null;
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
            body.gravityScale = 1.5f;

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

        IEnumerator Flourish()
        {
            Dev.Where();

            //original setup logic that was duplicated in both wake states
            //TODO: move this setup logic into a function
            owner.transform.localScale = owner.transform.localScale.SetX( -1f );
            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

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

            //close the gates
            SetFightGates( true );
            //end original setup logic

            ShowBossTitle(3f,"","","","HORNET");

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Flourish");

            PlayOneShot(hornetYell);

            PlayBossMusic();

            onAnimationCompleteNextState = Idle;

            //play until the callback fires and changes our state
            for(;;)
            {
                if(onAnimationCompleteNextState == null)
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }

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

            //face the knight
            HeroController hero = HeroController.instance;
            FaceObject(hero.gameObject, false, false);

            //then flip the other way
            FlipScale();

            currentState = RunAntic();

            yield break;
        }

        IEnumerator RunAntic()
        {
            Dev.Where();

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Evade Antic");

            onAnimationCompleteNextState = Run;

            //play until the callback fires and changes our state
            for(;;)
            {
                if(onAnimationCompleteNextState == null)
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }

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

            //play throwing animation
            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play("Throw Antic");

            //setup the next state to fire after the animation is complete
            onAnimationCompleteNextState = MaybeLock;

            PlayRandomOneShot(hornetThrowYells);

            //wait here until the callback fires and changes our state
            for(;;)
            {
                if(onAnimationCompleteNextState == null)
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }

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

            airDashPause = GameRNG.Rand( airDashPauseMin, airDashPauseMax );
            willSphere = false;

            currentState = JumpAntic();

            yield break;
        }

        IEnumerator JumpAntic()
        {
            Dev.Where();

            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            body.velocity = Vector2.zero;

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Jump Antic" );

            onAnimationCompleteNextState = AimJump;
            
            FaceObject( HeroController.instance.gameObject );

            //play until the callback fires and changes our state
            for(; ; )
            {
                if( onAnimationCompleteNextState == null )
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        IEnumerator AimJump()
        {
            Dev.Where();

            if( willSphere )
            {
                currentState = AimSphereJump();
            }
            else
            {
                //TODO: enchancement: make hornet jump at/near/away from the player

                Vector3 currentPosition = owner.transform.position;
                Vector2 jumpOrigin = currentPosition;
                Vector2 jumpDirectionL = Vector2.left;
                Vector2 jumpDirectionR = Vector2.left;

                float xMin = -jumpDistance;
                float xMax = jumpDistance;

                //get max L x jump distance
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionL, jumpDistance, 1 << 8);
                    if( raycastHit2D2.collider != null )
                    {
                        xMin = raycastHit2D2.transform.position.x;
                    }
                }

                //get max R x jump distance
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(jumpOrigin, jumpDirectionR, jumpDistance, 1 << 8);
                    if( raycastHit2D2.collider != null )
                    {
                        xMax = raycastHit2D2.transform.position.x;
                    }
                }

                jumpPoint = GameRNG.Rand(xMin,xMax);

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
            PlayRandomOneShot( hornetJumpYells );
            PlayOneShot( hornetJumpSFX );

            tk2dAnimator.Play( "Jump" );

            //TODO: this seems weird, see how it turns out
            body.velocity = new Vector2( jumpPoint, jumpVelocityY );

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
            while( waitTimer > 0f )
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if( bottomHit )
                {
                    currentState = Land();
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            if( waitTimer <= 0f )
            {
                currentState = ADashAntic();
            }
            else if( !bottomHit )
            {
                while( !bottomHit )
                {
                    if(body.velocity.y > 0f && Mathf.Abs(owner.transform.position.y - startHeight) > minAirSphereHeight)
                    {
                        currentState = MaybeDoSphere();
                        break;
                    }

                    yield return new WaitForEndOfFrame();

                    //did we hit a wall? end evade timer early
                    if( bottomHit )
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

        IEnumerator ReAim()
        {
            Dev.Where();
            //TODO
            currentState = AimJump();

            yield break;
        }

        IEnumerator Land()
        {
            Dev.Where();
            //TODO
            currentState = Escalation();

            yield break;
        }

        IEnumerator ADashAntic()
        {
            Dev.Where();
            //TODO
            currentState = Fire();
            currentState = InAir();

            yield break;
        }

        IEnumerator Fire()
        {
            Dev.Where();
            //TODO
            currentState = FiringL();
            currentState = FiringR();

            yield break;
        }

        IEnumerator FiringL()
        {
            Dev.Where();
            //TODO
            currentState = ADash();

            yield break;
        }

        IEnumerator FiringR()
        {
            Dev.Where();
            //TODO
            currentState = ADash();

            yield break;
        }

        IEnumerator ADash()
        {
            Dev.Where();
            //TODO
            currentState = WallL();
            currentState = WallR();
            currentState = LandY();
            currentState = HitRoof();

            yield break;
        }

        IEnumerator WallL()
        {
            Dev.Where();
            //TODO
            currentState = JumpR();

            yield break;
        }

        IEnumerator WallR()
        {
            Dev.Where();
            //TODO
            currentState = JumpL();

            yield break;
        }

        IEnumerator JumpL()
        {
            Dev.Where();
            //TODO
            currentState = InAir();

            yield break;
        }

        IEnumerator JumpR()
        {
            Dev.Where();
            //TODO
            currentState = InAir();

            yield break;
        }

        IEnumerator LandY()
        {
            Dev.Where();
            //TODO
            currentState = HardLand();

            yield break;
        }

        IEnumerator HitRoof()
        {
            Dev.Where();
            //TODO
            currentState = InAir();

            yield break;
        }

        IEnumerator HardLand()
        {
            Dev.Where();
            //TODO
            currentState = Escalation();

            yield break;
        }

        IEnumerator MaybeDoSphere()
        {
            Dev.Where();
            //TODO
            currentState = SphereAnticA();
            currentState = InAir();

            yield break;
        }

        IEnumerator SphereAnticA()
        {
            Dev.Where();
            //TODO
            currentState = SphereA();

            yield break;
        }

        IEnumerator SphereA()
        {
            Dev.Where();
            //TODO
            currentState = SphereRecoverA();

            yield break;
        }

        IEnumerator SphereRecoverA()
        {
            Dev.Where();
            //TODO
            currentState = SphereAEnd();

            yield break;
        }

        IEnumerator SphereAEnd()
        {
            Dev.Where();
            //TODO
            currentState = InAir();

            yield break;
        }

        IEnumerator AimSphereJump()
        {
            Dev.Where();
            //TODO
            currentState = Jump();

            yield break;
        }

        IEnumerator SetJumpOnly()
        {
            Dev.Where();
            //TODO
            currentState = JumpAntic();

            yield break;
        }

        IEnumerator SetSphereA()
        {
            Dev.Where();
            //TODO
            currentState = JumpAntic();

            yield break;
        }

        IEnumerator GDashAntic()
        {
            Dev.Where();
            //TODO
            currentState = GDash();

            yield break;
        }

        IEnumerator GDash()
        {
            Dev.Where();
            //TODO
            currentState = GDashRecover1();

            yield break;
        }

        IEnumerator GDashRecover1()
        {
            Dev.Where();
            //TODO
            currentState = GDashRecover2();

            yield break;
        }

        IEnumerator GDashRecover2()
        {
            Dev.Where();
            //TODO
            currentState = Escalation();

            yield break;
        }

        IEnumerator SphereAnticG()
        {
            Dev.Where();
            //TODO
            currentState = Sphere();

            yield break;
        }

        IEnumerator Sphere()
        {
            Dev.Where();
            //TODO
            currentState = SphereRecover();

            yield break;
        }

        IEnumerator SphereRecover()
        {
            Dev.Where();
            //TODO
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
                evadeRange.DisableEvadeForTime( randomDelay );

                //stop her moving
                body.velocity = Vector2.zero;

                //make her face you
                HeroController hero = HeroController.instance;
                FaceObject( hero.gameObject );

                //animate the evade-anticipation
                tk2dAnimator.AnimationCompleted = OnAnimationComplete;
                tk2dAnimator.Play( "Evade Antic" );

                onAnimationCompleteNextState = Evade;

                //play until the callback fires and changes our state
                for(; ; )
                {
                    if( onAnimationCompleteNextState == null )
                        break;
                    else
                        yield return new WaitForEndOfFrame();
                }
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

            PlayRandomOneShot( hornetLaughs );
            PlayOneShot( hornetSmallJumpSFX );

            tk2dAnimator.Play( "Evade" );

            float xScale = owner.transform.localScale.x;
            float jumpAwaySpeed = xScale * evadeJumpAwaySpeed;

            body.velocity = new Vector2( jumpAwaySpeed, 0f );
            float waitTimer = evadeJumpAwayTimeLength;
            while( waitTimer > 0f )
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if( rightHit || leftHit )
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
            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Evade Land" );

            onAnimationCompleteNextState = AfterEvade;

            body.velocity = Vector2.zero;

            PlayOneShot( hornetGroundLandSFX );

            //play until the callback fires and changes our state
            for(; ; )
            {
                if( onAnimationCompleteNextState == null )
                    break;
                else
                    yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        IEnumerator AfterEvade()
        {
            Dev.Where();

            bool attack = GameRNG.CoinToss();
            if( attack )
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

            int choice = GameRNG.WeightedRand( DmgResponseChoices.Values.ToList() );
            currentState = DmgResponseChoices.Keys.ToList()[ choice ].Invoke();

            yield break;
        }

        IEnumerator DmgIdle()
        {
            Dev.Where();

            float randomDelay = GameRNG.Rand(dmgIdleWaitMin, dmgIdleWaitMax);
            while( randomDelay > 0f )
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if( rightHit || leftHit )
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

            tk2dAnimator.Play( "Idle" );

            //wait for player to get close
            while( !refightRange.objectIsInRange )
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
            if( !escalated && hpRemainingPercent < escalationHPPercentage )
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

        public void PlayRandomOneShot(List<AudioClip> clip)
        {
            if(actorAudioSource != null && clip != null && clip.Count > 0)
            {
                AudioClip randomClip = clip.GetRandomElementFromList();
                actorAudioSource.PlayOneShot(randomClip);
            }
        }

        void ShowBossTitle( float hideInSeconds, string largeMain = "", string largeSuper = "", string largeSub = "", string smallMain = "", string smallSuper = "", string smallSub = "" )
        {
            //no point in doing this
            if( hideInSeconds <= 0f )
                hideInSeconds = 0f;

            //show hornet title
            if( areaTitleObject != null )
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
                Dev.Log( areaTitleObject + " is null! Cannot show the boss title." );
            }
        }

        IEnumerator HideBossTitleAfter( float time )
        {
            yield return new WaitForSeconds( time );
            HideBossTitle();
            yield return new WaitForSeconds( 3f );
            areaTitleObject.SetActive( false );
        }

        void HideBossTitle()
        {
            //show hornet title
            if( areaTitleObject != null )
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
                Dev.Log( areaTitleObject + " is null! Cannot hide the boss title." );
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

        void OnAnimationComplete(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
        {
            Dev.Where();
            int intData = -1;
            tk2dSpriteAnimationClip[] array = (sprite.Library == null) ? null : sprite.Library.clips;
            if(array != null)
            {
                for(int i = 0; i < array.Length; i++)
                {
                    if(array[i] == clip)
                    {
                        intData = i;
                        break;
                    }
                }
            }

            if(onAnimationCompleteNextState != null)
            {
                currentState = onAnimationCompleteNextState();
                Dev.Log("setting next state");
            }

            onAnimationCompleteNextState = null;
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

        float GetAngleToTarget(GameObject target, float offsetX, float offsetY)
        {
            float num = target.transform.position.y + offsetY - transform.position.y;
            float num2 = target.transform.position.x + offsetX - transform.position.x;
            float num3;
            for(num3 = Mathf.Atan2(num, num2) * 57.2957764f; num3 < 0f; num3 += 360f)
            {
            }
            return num3;
        }

        public static IEnumerator GetAudioPlayerOneShotClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayerOneShotLoaded)
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
            onAudioPlayerOneShotLoaded(null);
#else
            var audioPlayerOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>(stateName, fsmName);
            
            //this is a prefab
            var clips = audioPlayerOneShot.audioClips.ToList();
            
            //send the clips out
            onAudioPlayerOneShotLoaded(clips);
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
            var setGameObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetGameObject>(stateName, fsmName);            

            //this is a prefab
            var prefab = setGameObject.gameObject.Value;
            
            //so spawn one
            var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

            //send the loaded object out
            onGameObjectLoaded(spawnedCopy);
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
            if( !go.activeInHierarchy )
            {
                copy = GameObject.Instantiate( go ) as GameObject;
                copy.SetActive( true );
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

            if( copy != go )
                GameObject.Destroy( copy );

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioClipFromAudioPlaySimpleInFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
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
            onClipLoaded(null);
#else
            var audioPlaySimple = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlaySimple>(stateName, fsmName);

            var clip = audioPlaySimple.oneShotClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
#endif
            if( copy != go )
                GameObject.Destroy( copy );

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        public static IEnumerator GetAudioClipFromFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            Dev.Where();
            GameObject copy = go;
            if( !go.activeInHierarchy )
            {
                copy = GameObject.Instantiate( go ) as GameObject;
                copy.SetActive( true );
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
            if( copy != go )
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
            if(gameObject.FindGameObjectInChildren("Throw Effect") != null)
                throwEffect = gameObject.FindGameObjectInChildren("Throw Effect").AddComponent<ThrowEffect>();
            
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

            yield return GetAudioPlayerOneShotClipsFromFSM( owner, "Control", "Jump", SetHornetJumpYells );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( owner, "Control", "Jump", SetHornetJumpSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( owner, "Control", "Evade Land", SetHornetGroundLandSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( owner, "Control", "Evade", SetHornetSmallJumpSFX );
            yield return GetAudioPlayerOneShotClipsFromFSM( owner, "Control", "Evade", SetHornetLaughs );
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, "Control", "Throw Recover", SetHornetCatchSFX);
            yield return GetAudioClipFromAudioPlaySimpleInFSM(owner, "Control", "Throw", SetHornetThrowSFX);
            yield return GetAudioPlayerOneShotClipsFromFSM(owner, "Control", "Throw Antic", SetHornetThrowYells);
            yield return GetGameObjectFromFSM(owner, "Control", "Flourish", SetAreaTitleReference);
            yield return GetAudioSourceObjectFromFSM(owner, "Control", "Flourish", SetActorAudioSource);
            yield return GetAudioClipFromFSM(owner, "Control", "Flourish", SetHornetYell);
            fightMusic = GetMusicCueFromFSM(owner, "Control", "Flourish");
            fightMusicSnapshot = GetSnapshotFromFSM(owner, "Control", "Flourish");
            
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

        void SetHornetJumpSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet jump sfx clip is null!" );
                return;
            }

            hornetJumpSFX = clip;
        }

        void SetHornetJumpYells( List<AudioClip> clips )
        {
            if( clips == null )
            {
                Dev.Log( "Warning: hornet jump yells are null clips!" );
                return;
            }

            hornetJumpYells = clips;
        }

        void SetHornetSmallJumpSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet small jump sfx clip is null!" );
                return;
            }

            hornetSmallJumpSFX = clip;
        }

        void SetHornetGroundLandSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet ground land sfx clip is null!" );
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

        void SetHornetThrowYells(List<AudioClip> clips)
        {
            if(clips == null)
            {
                Dev.Log("Warning: hornet throw yells are null clips!");
                return;
            }

            hornetThrowYells = clips;
        }

        void SetHornetLaughs( List<AudioClip> clips )
        {
            if( clips == null )
            {
                Dev.Log( "Warning: hornet laughs are null clips!" );
                return;
            }

            hornetLaughs = clips;
        }

        void SetAreaTitleReference( GameObject areaTitle )
        {
            if( areaTitle == null )
            {
                Dev.Log( "Warning: Area Title GameObject failed to load and is null!" );
                return;
            }

            AreaTitle title = areaTitle.GetComponent<AreaTitle>();

            foreach( PlayMakerFSM p in areaTitle.GetComponentsInChildren<PlayMakerFSM>() )
            {
                GameObject.DestroyImmediate( p );
            }

            GameObject.DestroyImmediate( title );
            
            areaTitleObject = areaTitle;
            areaTitleObject.SetActive( false );
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
