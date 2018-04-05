﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;

namespace EnemyRandomizerMod
{
    [RequireComponent( typeof( BoxCollider2D ) )]
    public class EvadeRange : MonoBehaviour
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

    [RequireComponent( typeof( CircleCollider2D ) )]
    public class SphereRange : MonoBehaviour
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


    [RequireComponent( typeof( BoxCollider2D ) )]
    [RequireComponent( typeof( Rigidbody2D ) )]
    [RequireComponent( typeof( MeshRenderer ) )]
    [RequireComponent( typeof( MeshFilter ) )]
    [RequireComponent( typeof( AudioSource ) )]
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
        public PlayMakerFSM audioManagerFSM;
        public AudioSource runAudioSource;

        //TODO: change to false and have this set by an outside controller
        public bool wake = true;

        //TODO: set when hit
        public bool wasHitRecently;

        //stuff used by the testing framework
        public bool tempDone = false;

        //variables used by the state machine that we set here
        public float throwDistance = 10f;
        public bool checkUp = false;
        public bool checkDown = false;
        public bool checkLeft = true;
        public bool checkRight = true;

        //variables used by the state machine that the states set
        Func<IEnumerator> onAnimationCompleteNextState = null;
        float airDashPause;
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

        private IEnumerator Start()
        {
            owner = gameObject;
            bodyCollider = GetComponent<BoxCollider2D>();
            body = GetComponent<Rigidbody2D>();
            meshRenderer = GetComponent<MeshRenderer>();
            runAudioSource = GetComponent<AudioSource>();

            healthManager = GetComponent<HealthManager>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            evadeRange = gameObject.FindGameObjectInChildren( "Evade Range" ).GetComponent<EvadeRange>();
            sphereRange = gameObject.FindGameObjectInChildren( "Sphere Range" ).GetComponent<SphereRange>();

            //TODO, FOR TESTING, REMOVE ME
            //healthManager = gameObject.AddComponent<HealthManager>();
            //tk2dAnimator = gameObject.AddComponent<tk2dSpriteAnimator>();
            //evadeRange = gameObject.AddComponent<EvadeRange>();
            //sphereRange = gameObject.AddComponent<SphereRange>();


            //TODO: replace with the LocateFSM script
            audioManagerFSM = GameManager.instance.AudioManager.GetComponent<PlayMakerFSM>();

            //remove her playmaker fsm for her main AI
            //PlayMakerFSM deleteFSM = owner.GetMatchingFSMComponent("Control","G Dash","Tk2dPlayAnimation");
            //{
            //if( deleteFSM != null )
            //{
            //    GameObject.Destroy( deleteFSM );
            //}
            //}
            //remove her playmaker fsm for her evade range
            //PlayMakerFSM deleteFSM = evadeRange.gameObject.GetMatchingFSMComponent("FSM","Detect","Trigger2dEvent");
            //{
            //if( deleteFSM != null )
            //{
            //    GameObject.Destroy( deleteFSM );
            //}
            //PlayMakerUnity2DProxy proxy = evadeRange.gameObject.GetComponent<PlayMakerUnity2DProxy>();
            //if( proxy != null )
            //{
            //    GameObject.Destroy( proxy );
            //}
            //}
            //remove her playmaker fsm for her sphere range
            //PlayMakerFSM deleteFSM = sphereRange.gameObject.GetMatchingFSMComponent("FSM","Detect","Trigger2dEvent");
            //{
            //if( deleteFSM != null )
            //{
            //    GameObject.Destroy( deleteFSM );
            //}
            //PlayMakerUnity2DProxy proxy = sphereRange.gameObject.GetComponent<PlayMakerUnity2DProxy>();
            //if( proxy != null )
            //{
            //    GameObject.Destroy( proxy );
            //}
            //}

            //TODO: remove stun control

            StartCoroutine( MainAILoop() );

            yield break;
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Init();

            for(; ; )
            {
                if( owner == null )
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

            if( test >= 4 )
            {
                currentState = RefightReady();
            }
            else
            {
                while( !wake )
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
            transform.localScale = transform.localScale.SetX( -1f );
            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            currentState = Flourish();

            yield break;
        }

        IEnumerator Flourish()
        {
            Dev.Where();

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Flourish" );

            if( audioManagerFSM != null )
                audioManagerFSM.SendEvent( "BOSS HORNET" );

            onAnimationCompleteNextState = Idle;

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

        IEnumerator Idle()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;
            FaceObject( hero.gameObject, false, false );

            airDashPause = 999f;

            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            tk2dAnimator.Play( "Idle" );

            body.velocity = Vector2.zero;

            if( evadeRange.objectIsInRange )
            {
                currentState = EvadeAntic();
            }
            else
            {
                //use a while loop to yield that way other events may force a transition 
                //todo: move these into min/max variables
                float randomDelay = GameRNG.Rand(.5f, .75f);
                while( randomDelay > 0f )
                {
                    yield return new WaitForEndOfFrame();

                    //did something hit us?
                    if( wasHitRecently )
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
                while( !flag )
                {
                    int randomWeightedIndex = GameRNG.Rand(0,nextStates.Count-1);
                    if( randomWeightedIndex == 0 && ctIdle < 2 )
                    {
                        ctIdle += 1;
                        ctRun = 0;
                        currentState = nextStates[ 0 ].Invoke();
                        flag = true;
                    }
                    else if( randomWeightedIndex == 1 && ctRun < 2 )
                    {
                        ctIdle = 0;
                        ctRun += 1;
                        currentState = nextStates[ 1 ].Invoke();
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
            if( nextState == 1 )
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
            FaceObject( hero.gameObject, false, false );

            //then flip the other way
            FlipScale();

            currentState = RunAntic();

            yield break;
        }

        IEnumerator RunAntic()
        {
            Dev.Where();

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Evade Antic" );

            onAnimationCompleteNextState = Run;

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

        IEnumerator Run()
        {
            Dev.Where();

            runAudioSource.Play();
            //TODO: Get the volume from the play state (add it to the component print types)

            //TODO: this negative seems wrong, test and see what it does
            //TODO: move the 8 into a variable
            float runSpeed = -8f;
            float xVel = transform.localScale.x * runSpeed;

            tk2dAnimator.Play( "Run" );
            body.velocity = new Vector2( xVel, 0f );

            //todo: move these into variables: RunWaitMin and RunWaitMax
            float randomDelay = GameRNG.Rand(.5f, 1f);
            while( randomDelay > 0f )
            {
                yield return new WaitForEndOfFrame();

                //did something hit us?
                if( wasHitRecently )
                {
                    currentState = DmgResponse();
                    break;
                }

                if( evadeRange.objectIsInRange )
                {
                    currentState = EvadeAntic();
                    break;
                }

                if( rightHit || leftHit )
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

            if( sphereRange.objectIsInRange )
            {
                //todo: move these into min/max variables
                float randomChoice = GameRNG.Randf();
                if( randomChoice > .8f )
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

            Vector3 currentPosition = transform.position;

            Vector2 throwOrigin = currentPosition;
            Vector2 throwDirection = Vector2.left;

            //TODO: custom enhancement, get the unity vector direction to the hero and throw along that line
            HeroController hero = HeroController.instance;
            var direction = DoCheckDirection( hero.gameObject );

            if(direction.right)
            {
                throwDirection = Vector2.right;
            }

            RaycastHit2D raycastHit2D2 = Physics2D.Raycast(throwOrigin, throwDirection, throwDistance, 1 << 8);
            if( raycastHit2D2.collider != null )
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
            while( !flag )
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0,3);
                if( ctAirDash >= maxMissADash )
                {
                    flag2 = true;
                    num = 0;
                }
                if( ctASphere >= maxMissASphere )
                {
                    flag2 = true;
                    num = 1;
                }
                if( ctGDash >= maxMissGDash )
                {
                    flag2 = true;
                    num = 2;
                }
                if( ctThrow >= maxMissThrow )
                {
                    flag2 = true;
                    num = 2;
                }

                //were we forced to use a skill? if so, record that and move on
                if( flag2 )
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

                    if( num == 0 )
                    {
                        msAirdash = 0;
                        ctAirDash = 1;
                        currentState = ADash();
                    }
                    if( num == 1 )
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        currentState = SetSphereA();
                    }
                    if( num == 2 )
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        currentState = GDashAntic();
                    }
                    if( num == 3 )
                    {
                        msThrow = 0;
                        ctThrow = 1;
                        currentState = ThrowAntic();
                    }
                }
                //else, randomly pick a skill to use
                else if( randomWeightedIndex == 0 && ctAirDash < 2 )
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow = 0;
                    currentState = ADash();
                    flag = true;
                }
                else if( randomWeightedIndex == 1 && ctASphere < 1 )
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    ctThrow = 0;
                    currentState = SetSphereA();
                    flag = true;
                }
                else if( randomWeightedIndex == 2 && ctGDash < 2 )
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    ctThrow = 0;
                    currentState = GDashAntic();
                    flag = true;
                }
                else if( randomWeightedIndex == 3 && ctThrow < 1 )
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
            while( !flag )
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0,2);
                if( ctAirDash >= maxMissADash )
                {
                    flag2 = true;
                    num = 0;
                }
                if( ctASphere >= maxMissASphere )
                {
                    flag2 = true;
                    num = 1;
                }
                if( ctGDash >= maxMissGDash )
                {
                    flag2 = true;
                    num = 2;
                }

                //were we forced to use a skill? if so, record that and move on
                if( flag2 )
                {
                    flag = true;

                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;

                    msAirdash += 1;
                    msASphere += 1;
                    msGDash += 1;
                    
                    if( num == 0 )
                    {
                        msAirdash = 0;
                        ctAirDash = 1;
                        currentState = ADash();
                    }
                    if( num == 1 )
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        currentState = SetSphereA();
                    }
                    if( num == 2 )
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        currentState = GDashAntic();
                    }
                }
                //else, randomly pick a skill to use
                else if( randomWeightedIndex == 0 && ctAirDash < 2 )
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    currentState = ADash();
                    flag = true;
                }
                else if( randomWeightedIndex == 1 && ctASphere < 1 )
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    currentState = SetSphereA();
                    flag = true;
                }
                else if( randomWeightedIndex == 2 && ctGDash < 2 )
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
            float angleToTarget = GetAngleToTarget(hero.gameObject,0f,0f);

            canStunRightNow = false;

            bodyCollider.offset = new Vector2( 1f, -.3f );
            bodyCollider.size = new Vector2( 1f, 2.6f );

            FaceObject( hero.gameObject );

            body.velocity = Vector2.zero;

            

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



        void OnCollisionStay2D( Collision2D collision )
        {
            if( collision.gameObject.layer == 8 )
            {
                CheckTouching( 8 );
            }
        }

        void FlipScale()
        {
            transform.localScale = transform.localScale.SetX( -transform.localScale.x );
        }

        void FaceObject( GameObject objectToFace, bool spriteFacesRight = false, bool resetFrame = false, string playNewAnimation = "" )
        {
            Dev.Where();

            Vector3 localScale = transform.localScale;
            float xScale = localScale.x;

            if( transform.position.x < objectToFace.transform.position.x )
            {
                if( spriteFacesRight )
                {
                    if( localScale.x != xScale )
                    {
                        localScale.x = xScale;
                        if( resetFrame )
                        {
                            this.tk2dAnimator.PlayFromFrame( 0 );
                        }
                        if( !string.IsNullOrEmpty( playNewAnimation ) )
                        {
                            this.tk2dAnimator.Play( playNewAnimation );
                        }
                    }
                }
                else if( localScale.x != -xScale )
                {
                    localScale.x = -xScale;
                    if( resetFrame )
                    {
                        this.tk2dAnimator.PlayFromFrame( 0 );
                    }
                    if( !string.IsNullOrEmpty( playNewAnimation ) )
                    {
                        this.tk2dAnimator.Play( playNewAnimation );
                    }
                }
            }
            else if( spriteFacesRight )
            {
                if( localScale.x != -xScale )
                {
                    localScale.x = -xScale;
                    if( resetFrame )
                    {
                        this.tk2dAnimator.PlayFromFrame( 0 );
                    }
                    if( !string.IsNullOrEmpty( playNewAnimation ) )
                    {
                        this.tk2dAnimator.Play( playNewAnimation );
                    }
                }
            }
            else if( localScale.x != xScale )
            {
                localScale.x = xScale;
                if( resetFrame )
                {
                    this.tk2dAnimator.PlayFromFrame( 0 );
                }
                if( !string.IsNullOrEmpty( playNewAnimation ) )
                {
                    this.tk2dAnimator.Play( playNewAnimation );
                }
            }
            transform.localScale = localScale;
        }

        void OnAnimationComplete( tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip )
        {
            Dev.Where();
            int intData = -1;
            tk2dSpriteAnimationClip[] array = (sprite.Library == null) ? null : sprite.Library.clips;
            if( array != null )
            {
                for( int i = 0; i < array.Length; i++ )
                {
                    if( array[ i ] == clip )
                    {
                        intData = i;
                        break;
                    }
                }
            }

            if( onAnimationCompleteNextState != null )
            {
                currentState = onAnimationCompleteNextState();
                Dev.Log( "setting next state" );
            }

            onAnimationCompleteNextState = null;
        }

        private const float RAYCAST_LENGTH = 0.08f;

        private List<Vector2> topRays;
        private List<Vector2> rightRays;
        private List<Vector2> bottomRays;
        private List<Vector2> leftRays;

        private void CheckTouching( LayerMask layer )
        {
            if( this.checkUp )
            {
                this.topRays.Clear();
                this.topRays.Add( new Vector2( this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y ) );
                this.topRays.Add( new Vector2( this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.max.y ) );
                this.topRays.Add( this.bodyCollider.bounds.max );
                this.topHit = false;
                for( int i = 0; i < 3; i++ )
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(this.topRays[i], Vector2.up, 0.08f, 1 << layer);
                    if( raycastHit2D.collider != null )
                    {
                        this.topHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if( this.checkRight )
            {
                this.rightRays.Clear();
                this.rightRays.Add( this.bodyCollider.bounds.max );
                this.rightRays.Add( new Vector2( this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.center.y ) );
                this.rightRays.Add( new Vector2( this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y ) );
                this.rightHit = false;
                for( int j = 0; j < 3; j++ )
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.rightRays[j], Vector2.right, 0.08f, 1 << layer);
                    if( raycastHit2D2.collider != null )
                    {
                        this.rightHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if( this.checkDown )
            {
                this.bottomRays.Clear();
                this.bottomRays.Add( new Vector2( this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y ) );
                this.bottomRays.Add( new Vector2( this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.min.y ) );
                this.bottomRays.Add( this.bodyCollider.bounds.min );
                this.bottomHit = false;
                for( int k = 0; k < 3; k++ )
                {
                    RaycastHit2D raycastHit2D3 = Physics2D.Raycast(this.bottomRays[k], -Vector2.up, 0.08f, 1 << layer);
                    if( raycastHit2D3.collider != null )
                    {
                        this.bottomHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if( this.checkLeft )
            {
                this.leftRays.Clear();
                this.leftRays.Add( this.bodyCollider.bounds.min );
                this.leftRays.Add( new Vector2( this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.center.y ) );
                this.leftRays.Add( new Vector2( this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y ) );
                this.leftHit = false;
                for( int l = 0; l < 3; l++ )
                {
                    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(this.leftRays[l], -Vector2.right, 0.08f, 1 << layer);
                    if( raycastHit2D4.collider != null )
                    {
                        this.leftHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
        }//end CheckTouching

        public struct DirectionSet
        {
            public bool above;
            public bool below;
            public bool right;
            public bool left;
        }

        private DirectionSet DoCheckDirection(GameObject target)
        {
            DirectionSet direction = new DirectionSet();
            float num = transform.position.x;
            float num2 = transform.position.y;
            float num3 = target.transform.position.x;
            float num4 = target.transform.position.y;

            direction.right = ( num < num3 );
            direction.left = ( num > num3 );
            direction.above = ( num2 < num4 );
            direction.below = ( num2 > num4 );
            
            return direction;
        }



        private float GetAngleToTarget( GameObject target, float offsetX, float offsetY )
        {
            float num = target.transform.position.y + offsetY - transform.position.y;
            float num2 = target.transform.position.x + offsetX - transform.position.x;
            float num3;
            for( num3 = Mathf.Atan2( num, num2 ) * 57.2957764f; num3 < 0f; num3 += 360f )
            {
            }
            return num3;
        }
        //end helpers /////////////////////////////
    }//end class
}







//force-send an event on this state if everything matches?
//if( !string.IsNullOrEmpty( sendWakeEventsOnState ) && fsmName == p.FsmName && sendWakeEventsOnState == p.ActiveStateName )
//{
//    if( p != null && wakeEvents != null )
//    {
//        foreach( string s in wakeEvents )
//        {
//            p.SendEvent( s );
//        }
//    }
//}




//private void OnTriggerEnter2D( Collider2D collision )
//{
//    if( monitorFSMStates )
//        return;

//    bool isPlayer = false;

//    foreach( Transform t in collision.gameObject.GetComponentsInParent<Transform>() )
//    {
//        if( t.gameObject == HeroController.instance.gameObject )
//        {
//            isPlayer = true;
//            break;
//        }
//    }

//    if( !isPlayer )
//    {
//        Dev.Log( "Something not the player entered us!" );
//        return;
//    }

//    Dev.Log( "Player entered our wake area! " );

//    if( !string.IsNullOrEmpty( fsmName ) )
//    {
//        PlayMakerFSM fsm = FSMUtility.LocateFSM( owner, fsmName );

//        if( fsm != null && wakeEvents != null )
//        {
//            foreach( string s in wakeEvents )
//            {
//                Dev.Log( "Sending event! " + s );
//                fsm.SendEvent( s );
//            }
//        }
//        else
//        {
//            Dev.Log( "Could not find FSM!" );
//        }
//    }

//    //remove this after waking up the enemy
//    Destroy( gameObject );
//}