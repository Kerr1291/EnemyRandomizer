using System.Collections;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using nv;

namespace EnemyRandomizerMod
{
    public class SuperDashHandler : GameStateMachine
    {
        public SpriteRenderer spriteRenderer;
        public Animator unityAnimator;
        public tk2dSpriteAnimator tk2dAnimator;
        public MeshRenderer meshRenderer;
        public GameObject SDCharge;
        public AudioSource audioSource;

        public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        public Dictionary<string, ParticleSystem> particleSystems = new Dictionary<string, ParticleSystem>();
        public Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();

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
            spriteRenderer = GetComponent<SpriteRenderer>();
            unityAnimator = GetComponent<Animator>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            meshRenderer = GetComponent<MeshRenderer>();
            audioSource = GetComponent<AudioSource>();

            GameObject go = GameObject.Find( "SD Charge" );
            if( go != null )
                SDCharge = go;;

            gameObject.PrintSceneHierarchyTree();// gameObject.name );
            game = GameManager.instance;
            inputHandler = game.GetComponent<InputHandler>();
            hero = HeroController.instance;
        }

        protected override void Update()
        {
            if( !game.isPaused && hero.acceptingInput && !PlayerData.instance.GetBool( "atBench" ) )
            {
                wasPressed = inputHandler.inputActions.superDash.WasPressed;
                wasReleased = inputHandler.inputActions.superDash.WasReleased;
                isPressed = inputHandler.inputActions.superDash.IsPressed;
                isNotPressed = !inputHandler.inputActions.superDash.IsPressed;
            }
        }

        protected override void RemoveDeprecatedComponents()
        {
            Destroy( HeroController.instance.superDash );
        }

        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources(); 

            yield return GetValueFromAction<AudioClip, AudioPlay>( gameObject, "Superdash", "Ground Charge", "oneShotClip", SetStateMachineValue );
        }

        protected override IEnumerator Init()
        {
            yield return base.Init();


            nextState = Inactive;

            yield break;
        }

        protected virtual IEnumerator Inactive()
        {
            Dev.Where();
            //dunno, play with these here
            hero.RegainControl();
            hero.SetCState( "freezeCharge", false );
            while( !wasPressed && !isPressed )
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = CheckCanSuperDash;

            yield break;
        }

        protected virtual IEnumerator CheckCanSuperDash()
        {
            Dev.Where();

            //testing
            if( true || hero.CanSuperDash() )
                nextState = RelinquishControl;
            else
                nextState = Inactive;

            yield break;
        }

        protected virtual IEnumerator RelinquishControl()
        {
            Dev.Where();

            hero.RelinquishControl();

            nextState = CheckOnGround;

            yield break;
        }

        protected virtual IEnumerator RegainControl()
        {
            Dev.Where();

            hero.RegainControl();

            nextState = Inactive;

            yield break;
        }

        protected virtual IEnumerator CheckOnGround()
        {
            Dev.Where();

            if( hero.cState.onGround )
                nextState = GroundCharge;
            else
                nextState = WallCharge;
            yield break;
        }

        protected virtual IEnumerator GroundCharge()
        {
            Dev.Where();

            chargeTimer = 0.8f;
            PlayAnimation( "SD Charge Ground" );
            PlayMakerFSM.BroadcastEvent( "SUPERDASH CHARGING G" );
            hero.SetCState( "freezeCharge", true );
            DoCameraEffect( "RumblingFocus" );
            PlayMakerFSM.BroadcastEvent( "FocusRumble" );

            SDCharge.GetComponent<MeshRenderer>().enabled = true;

            PlayOneShot( audioSource, audioClips["test"] ); //???

            //send event to "Charge Effect"
            SendEventToFirstFSM( SDCharge, "UP" );

            float waitTimer = chargeTimer;
            while( waitTimer > 0 )
            {
                yield return new WaitForEndOfFrame();
                if( !(isPressed ))
                {
                    nextState = ChargeCancelGround;
                    break;
                }
                waitTimer -= Time.deltaTime;
            }

            if( ( isPressed ) )
            {
                nextState = GroundCharged;

                PlayAnimation( SDCharge.GetComponent<tk2dSpriteAnimator>(), "Charge Effect", 0 );
                PlayAnimation( "SD Fx Charge" );
            }

            yield break;
        }

        protected virtual IEnumerator GroundCharged()
        {
            Dev.Where();

            while( ( isPressed ) )
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = Direction;

            yield break;
        }

        protected virtual IEnumerator Direction()
        {
            Dev.Where();

            nextState = GLeft;

            nextState = GRight;

            yield break;
        }

        protected virtual IEnumerator WallCharge()
        {
            Dev.Where();


            nextState = CheckOnGround;

            yield break;
        }

        protected virtual IEnumerator DashStart()
        {
            Dev.Where();


            nextState = CheckOnGround;

            yield break;
        }

        protected virtual IEnumerator Dashing()
        {
            Dev.Where();


            nextState = CheckOnGround;

            yield break;
        }
        
        protected virtual IEnumerator HitWall()
        {
            Dev.Where();


            nextState = CheckOnGround;

            yield break;
        }

        protected virtual IEnumerator AirCancel()
        {
            Dev.Where();


            nextState = RegainControl;

            yield break;
        }

        protected virtual IEnumerator Right()
        {
            Dev.Where();


            nextState = DashStart;

            yield break;
        }

        protected virtual IEnumerator Left()
        {
            Dev.Where();


            nextState = DashStart;

            yield break;
        }

        protected virtual IEnumerator Cancelable()
        {
            Dev.Where();


            nextState = AirCancel;
            nextState = HitWall;
            nextState = AirCancel;

            yield break;
        }

        protected virtual IEnumerator ChargeCancelGround()
        {
            Dev.Where();


            nextState = RegainControl;

            yield break;
        }

        protected virtual IEnumerator ChargeCancelWall()
        {
            Dev.Where();


            nextState = RegainControl;

            yield break;
        }

        protected virtual IEnumerator WallCharged()
        {
            Dev.Where();


            nextState = DirectionWall;

            yield break;
        }

        protected virtual IEnumerator DirectionWall()
        {
            Dev.Where();

            nextState = Right;
            nextState = Left;

            yield break;
        }

        protected virtual IEnumerator GRight()
        {
            Dev.Where();

            nextState = Right;

            yield break;
        }

        protected virtual IEnumerator GLeft()
        {
            Dev.Where();

            nextState = Left;

            yield break;
        }

        //switch to this state when we enter from a transition
        protected virtual IEnumerator EnterSuperDash()
        {
            Dev.Where();

            nextState = EnterL;
            nextState = EnterR;

            yield break;
        }

        protected virtual IEnumerator EnterL()
        {
            Dev.Where();


            nextState = EnterVelocity;

            yield break;
        }

        protected virtual IEnumerator EnterR()
        {
            Dev.Where();


            nextState = EnterVelocity;

            yield break;
        }

        protected virtual IEnumerator EnterVelocity()
        {
            Dev.Where();


            nextState = Dashing;

            yield break;
        }

        protected virtual void PlayAnimation( string animation )
        {
            PlayAnimation( tk2dAnimator, animation );
        }

        protected virtual void PlayAnimation( tk2dSpriteAnimationClip animation )
        {
            tk2dAnimator.Play( animation );
        }

        public HeroController hero;
        public GameManager game;
        public InputHandler inputHandler;
        public tk2dSpriteAnimationClip groundChargeClip;
        public MeshRenderer chargeEffectMesh;
        public bool wasPressed;
        public bool wasReleased;
        public bool isPressed;
        public bool isNotPressed;
        public float chargeTimer;

        protected virtual void SetStateMachineValue<T>( T value )
        {
            if( value as AudioClip != null )
            {
                var v = value as AudioClip;
                SetStateMachineValue( audioClips, v.name, v );
            }
            else if( value as ParticleSystem != null )
            {
                var v = value as ParticleSystem;
                SetStateMachineValue( particleSystems, v.name, v );
            }
            else if( value as GameObject != null )
            {
                var v = value as GameObject;
                SetStateMachineValue( gameObjects, v.name, v );
            }
            else
            {
                if( value != null )
                {
                    Dev.Log( "Warning: No handler defined for SetStateMachineValue for type " + value.GetType().Name );
                }
                else
                {
                    Dev.Log( "Warning: value is null!" );
                }
            }
        }

        void SetStateMachineValue<D, T>( D dictionary, string name, T value )
            where D : Dictionary<string, T>
            where T : class
        {
            if( value == null )
            {
                Dev.Log( "Warning: " + name + " is null!" );
                return;
            }

            Dev.Log( "Added: " + name + " to dictionary of " + dictionary.GetType().Name + "!" );
            dictionary.Add( name, value );
        }
    }
}