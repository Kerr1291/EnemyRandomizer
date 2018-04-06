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
    
    public class ThrowEffect : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject owner;
        IEnumerator currentState = null;

        public void Play(GameObject parent)
        {
            owner = parent;
            tk2dAnimator = owner.GetComponent<tk2dSpriteAnimator>();

            gameObject.SetActive(true);

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
        public float throwDistance = 10f;
        public bool checkUp = false;
        public bool checkDown = false;
        public bool checkLeft = true;
        public bool checkRight = true;

        //variables used by the state machine that the states set
        Func<IEnumerator> onAnimationCompleteNextState = null;
        float airDashPause;
        float nextThrowAngle;
        bool topHit = false;
        bool rightHit = false;
        bool bottomHit = false;
        bool leftHit = false;
        bool canStunRightNow = true;

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
                yield return new WaitForEndOfFrame();
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
            owner.transform.localScale = owner.transform.localScale.SetX(-1f);
            bodyCollider.offset = new Vector2(.1f, -.3f);
            bodyCollider.size = new Vector2(.9f, 2.6f);

            currentState = Flourish();

            yield break;
        }

        IEnumerator Flourish()
        {
            Dev.Where();

            ShowBossTitle();

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
                //todo: move these into min/max variables
                float randomDelay = GameRNG.Rand(.5f, .75f);
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
            int nextState = GameRNG.Rand(0, 1);
            if(nextState == 1)
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
            //TODO: Get the volume from the play state (add it to the component print types)

            //TODO: this negative seems wrong, test and see what it does
            //TODO: move the 8 into a variable
            float runSpeed = -8f;
            float xVel = owner.transform.localScale.x * runSpeed;

            tk2dAnimator.Play("Run");
            body.velocity = new Vector2(xVel, 0f);

            //todo: move these into variables: RunWaitMin and RunWaitMax
            float randomDelay = GameRNG.Rand(.5f, 1f);
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
                //todo: move these into min/max variables
                float randomChoice = GameRNG.Randf();
                if(randomChoice > .8f)
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

            int maxMissADash = 5;
            int maxMissASphere = 7;
            int maxMissGDash = 5;
            int maxMissThrow = 3;

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
                        currentState = ADash();
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
                    currentState = ADash();
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
                        currentState = ADash();
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
                    currentState = ADash();
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

        IEnumerator Escalation()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator ADash()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator SetSphereA()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator GDashAntic()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator SphereAnticG()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator DmgResponse()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        IEnumerator EvadeAntic()
        {
            Dev.Where();
            //TODO

            yield break;
        }


        IEnumerator RefightReady()
        {
            Dev.Where();
            //TODO

            yield break;
        }

        public void DoEnemyKillShakeEffect()
        {
            //TODO: find out the real name of this gameobject
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

        void ShowBossTitle()
        {
            //show hornet title
            if(areaTitleObject != null)
            {
                areaTitleObject.SetActive(true);
#if UNITY_EDITOR
#else
                PlayMakerFSM fsm = FSMUtility.GetFSM(areaTitleObject);
                if(fsm != null)
                {
                    FSMUtility.SetString(areaTitleObject, "Area Event", "HORNET");
                }
                else
                {
                    Dev.Log(areaTitleObject.name + " has no PlayMakerFSM!");
                    areaTitleObject.PrintSceneHierarchyTree(true);
                }
#endif
            }
            else
            {
                Dev.Log(areaTitleObject + " is null! Cannot show the boss title.");
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

        List<Vector2> topRays;
        List<Vector2> rightRays;
        List<Vector2> bottomRays;
        List<Vector2> leftRays;

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

        static IEnumerator GetAudioPlayerOneShotClipsFromFSM(GameObject go, string fsmName, string stateName, Action<List<AudioClip>> onAudioPlayerOneShotLoaded)
        {
            GameObject copy = GameObject.Instantiate(go) as GameObject;
            copy.SetActive(true);

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onAudioPlayerOneShotLoaded(null);
#else
            var audioPlayerOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShot>(stateName, fsmName);
            
            //this is a prefab
            var clips = setGameObject.audioClips.ToList();

            yield return new WaitForEndOfFrame();
            
            //send the clips out
            onAudioPlayerOneShotLoaded(clips);
#endif
            GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        static IEnumerator GetGameObjectFromFSM(GameObject go, string fsmName, string stateName, Action<GameObject> onGameObjectLoaded)
        {
            GameObject copy = GameObject.Instantiate(go) as GameObject;
            copy.SetActive(true);

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onGameObjectLoaded(null);
#else
            var setGameObject = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.SetGameObject>(stateName, fsmName);
            
            //this is a prefab
            var prefab = setGameObject.gameObject.Value;
            
            //so spawn one
            var spawnedCopy = GameObject.Instantiate(prefab) as GameObject;

            yield return new WaitForEndOfFrame();
            
            //send the loaded object out
            onGameObjectLoaded(spawnedCopy);
#endif
            GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        static IEnumerator GetAudioSourceObjectFromFSM(GameObject go, string fsmName, string stateName, Action<AudioSource> onSourceLoaded)
        {
            GameObject copy = GameObject.Instantiate(go) as GameObject;
            copy.SetActive(true);

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
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

            yield return new WaitForEndOfFrame();
            
            //send the loaded object out
            onSourceLoaded(audioSource);
#endif
            GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        static IEnumerator GetAudioClipFromAudioPlaySimpleInFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            GameObject copy = GameObject.Instantiate(go) as GameObject;
            copy.SetActive(true);

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onClipLoaded(null);
#else
            var audioPlaySimple = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlaySimple>(stateName, fsmName);
            
            yield return new WaitForEndOfFrame();

            var clip = audioPlaySimple.oneShotClip.Value as AudioClip;

            //send the loaded clip out
            onClipLoaded(clip);
#endif
            GameObject.Destroy(copy);

            //let stuff get destroyed
            yield return new WaitForEndOfFrame();

            yield break;
        }

        static IEnumerator GetAudioClipFromFSM(GameObject go, string fsmName, string stateName, Action<AudioClip> onClipLoaded)
        {
            GameObject copy = GameObject.Instantiate(go) as GameObject;
            copy.SetActive(true);

            //wait a few frames for the fsm to set up stuff
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
            onClipLoaded(null);
#else
            var audioOneShot = copy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle>(stateName, fsmName);
            
            //this is a prefab
            var aPlayer = audioOneShot.audioPlayer.Value;
            
            //so spawn one
            var spawnedCopy = GameObject.Instantiate(aPlayer) as GameObject;
            var audioSource = spawnedCopy.GetComponent<AudioSource>();

            yield return new WaitForEndOfFrame();

            var clip = audioSource.clip;

            //send the loaded clip out
            onClipLoaded(clip);
            
            GameObject.Destroy(spawnedCopy);
#endif
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
        static UnityEngine.Audio.AudioMixerSnapshot GetSnapshotFromFSM(GameObject go, string fsmName, string stateName)
        {
            var snapshot = go.GetFSMActionOnState<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>("Flourish", "Control");
            var mixerSnapshot = snapshot.snapshot.Value as UnityEngine.Audio.AudioMixerSnapshot;
            return mixerSnapshot;
#endif
        }

#if UNITY_EDITOR
        static object GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
            return null;
#else
        static MusicCue GetMusicCueFromFSM(GameObject go, string fsmName, string stateName)
        {
            var musicCue = go.GetFSMActionOnState<ApplyMusicCue>(stateName, fsmName);
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
#endif
        }

        IEnumerator ExtractReferencesFromPlayMakerFSMs()
        {
            //load resources for the boss

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

        void SetAreaTitleReference(GameObject areaTitle)
        {
            if(areaTitle == null)
            {
                Dev.Log("Warning: Area Title GameObject failed to load and is null!");
                return;
            }

            //TODO: find out what this should be parented to
            areaTitleObject = areaTitle;
            areaTitleObject.SetActive(false);
        }

        void RemoveDeprecatedComponents()
        {
#if UNITY_EDITOR
#else
            //remove her playmaker fsm for her main AI
            {
                PlayMakerFSM deleteFSM = owner.GetMatchingFSMComponent("Control", "G Dash", "Tk2dPlayAnimation");
            
                if(deleteFSM != null)
                {
                    GameObject.Destroy(deleteFSM);
                }
            }

            //remove her playmaker fsm for her evade range
            {
                PlayMakerFSM deleteFSM = evadeRange.gameObject.GetMatchingFSMComponent("FSM", "Detect", "Trigger2dEvent");
            
                if(deleteFSM != null)
                {
                    GameObject.Destroy(deleteFSM);
                }
                PlayMakerUnity2DProxy proxy = evadeRange.gameObject.GetComponent<PlayMakerUnity2DProxy>();
                if(proxy != null)
                {
                    GameObject.Destroy(proxy);
                }
            }
            //remove her playmaker fsm for her sphere range
            {
                PlayMakerFSM deleteFSM = sphereRange.gameObject.GetMatchingFSMComponent("FSM", "Detect", "Trigger2dEvent");
            
                if(deleteFSM != null)
                {
                    GameObject.Destroy(deleteFSM);
                }
                PlayMakerUnity2DProxy proxy = sphereRange.gameObject.GetComponent<PlayMakerUnity2DProxy>();
                if(proxy != null)
                {
                    GameObject.Destroy(proxy);
                }
            }
            
            //remove the playmaker fsm for her throwEffect
            {
                PlayMakerFSM deleteFSM = throwEffect.gameObject.GetMatchingFSMComponent("FSM", "Wait", "Wait");
            
                if(deleteFSM != null)
                {
                    GameObject.Destroy(deleteFSM);
                }
            }
            
            //remove the playmaker fsm for her Needle
            {
                PlayMakerFSM deleteFSM = needle.gameObject.GetMatchingFSMComponent("Control", "Out", "Wait");
            
                if(deleteFSM != null)
                {
                    GameObject.Destroy(deleteFSM);
                }
                PlayMakerFixedUpdate proxy = needle.gameObject.GetComponent<PlayMakerFixedUpdate>();
                if(proxy != null)
                {
                    GameObject.Destroy(proxy);
                }
            }
#endif
            //TODO: remove stun control
        }

        //end helpers /////////////////////////////
    }//end class
}//end namespace
