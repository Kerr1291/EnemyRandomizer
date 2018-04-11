using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

using nv;

#if UNITY_EDITOR
using nv.Tests;
#else
using TMPro;
#endif

namespace EnemyRandomizerMod
{
    public class HornetBoss : EnemySM
    {
        //components used by the boss
        public AudioSource runAudioSource;
        public GameObject areaTitleObject;
        public PolygonCollider2D hitADash;
        public PolygonCollider2D hitGDash;
        public ParticleSystem dustHardLand;
        
        public ThrowEffect throwEffect;
        public ADashEffect aDashEffect;
        public GDashEffect gDashEffect;
        public SphereBall sphereBall;
        public FlashEffect flashEffect;
        public StunController stunControl;

        public RangeCheck runAwayCheck;
        public RangeCheck sphereRange;
        public RangeCheck aSphereRange;
        public RangeCheck refightRange;
        public RangeCheck aDashRange;
        public EvadeRange evadeRange;

        public GameObject hornetCorpse;
        public GameObject stunEffectPrefab;

        //hornet's projectile weapon & the tink effect that goes with it
        public Needle needle;
        public NeedleTink needleTink;

        //use for some sound effects
        public AudioSource actorAudioSource;

        //audio clips for hornet's various moves
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
        public List<AudioClip> hornetStunYells; 
        public AudioClip hornetDialogueSFX;

        public tk2dSpriteAnimationClip hornetPointClip;
        public tk2dSpriteAnimationClip hornetSoftLandClip;
        public tk2dSpriteAnimationClip hornetJumpFullClip;
        
        public MusicCue fightMusic;
        public UnityEngine.Audio.AudioMixerSnapshot fightMusicSnapshot;
        
        public bool checkPlayerData = false;

#if UNITY_EDITOR
        //stuff used by the testing framework
        public bool tempDone = false;
#else
#endif

        //variables used by the state machine that we set here
        public float runSpeed = 8f;
        public float evadeJumpAwaySpeed = 22f;
        public float evadeJumpAwayTimeLength = .25f;
        public float throwDistance = 12f;
        public float throwMaxTravelTime = .8f;
        public float throwWindUpTime = .03f;
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
        public float stunAirXVelocity = 10f;
        public float stunAirYVelocity = 20f;

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
        public float stunTime = 3f;

        public float esRunWaitMin = .35f;
        public float esRunWaitMax = .75f;
        public float esIdleWaitMin = .1f;
        public float esIdleWaitMax = .4f;
        public float esEvadeCooldownMin = .5f;
        public float esEvadeCooldownMax = 1f;
        public float esDmgIdleWaitMin = .05f;
        public float esDmgIdleWaitMax = .2f;
        public float esAirDashPauseMin = .05f;
        public float esAirDashPauseMax = .2f;

        public int maxMissADash = 5;
        public int maxMissASphere = 7;
        public int maxMissGDash = 5;
        public int maxMissThrow = 3;

        public int maxChosenADash = 2;
        public int maxChosenASphere = 1;
        public int maxChosenGDash = 2;
        public int maxChosenThrow = 1;

        //TODO: convert to a weighted table type
        protected Dictionary<Func<IEnumerator>, float> dmgResponseChoices;
        public Dictionary<Func<IEnumerator>, float> DmgResponseChoices {
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

        protected float airDashPause;
        protected float jumpPoint;
        protected float runWaitMin;
        protected float runWaitMax;
        protected float idleWaitMin;
        protected float idleWaitMax;
        protected float evadeCooldownMin;
        protected float evadeCooldownMax;
        protected float dmgIdleWaitMin;
        protected float dmgIdleWaitMax;
        protected float airDashPauseMin;
        protected float airDashPauseMax;
        protected float returnXScale;

        protected bool escalated = false;
        protected bool willSphere = false;

        protected int ctIdle = 0;
        protected int ctRun = 0;
        protected int ctAirDash = 0;
        protected int ctASphere = 0;
        protected int ctGDash = 0;
        protected int ctThrow = 0;
        protected int msAirdash = 0;
        protected int msASphere = 0;
        protected int msGDash = 0;
        protected int msThrow = 0;

        protected Vector2 gDashVelocity;
        protected Vector2 aDashVelocity;
        protected Ray throwRay;
        protected RaycastHit2D throwRaycast;

        protected GameObject dialogueManager;

        public override bool Running {
            get {
                if(checkPlayerData)
                {
                    if( GameManager.instance.playerData.GetBool( "hornet1Defeated" ) )
                        return false;
                }

                return healthManager.hp > 0 && !healthManager.isDead;
            }

            set {
                gameObject.SetActive( value );
            }
        }

        protected virtual void SetFightGates( bool closed )
        {
            if( closed )
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

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            runAudioSource = GetComponent<AudioSource>();

            SetupBossReferences();

            //original setup logic that was duplicated in both wake states
            gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            hitGDash.offset = new Vector2( .15f, .3f );
            hitADash.offset = new Vector2( -.2f, .175f );

            //NOTE: I added this to try fixing a problem with hornet ocassionally ending up in walls. I think it might help, so I'm leaving it.
            body.interpolation = RigidbodyInterpolation2D.Extrapolate;
            
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
        

        protected override IEnumerator Init()
        {
            yield return base.Init();

            body.gravityScale = normGravity2DScale;

            nextState = Inert;

            yield break;
        }


        protected virtual IEnumerator Inert()
        {
            Dev.Where(); 
            //TODO: move this check into a helper function and replace constants with variables
            int test = GameManager.instance.playerData.GetInt("hornetGreenpath");

            if( test >= 4 )
            {
                nextState = RefightReady;
            }
            else
            {
                //make use of the refight range to trigger the first encounter
                while( !refightRange.ObjectIsInRange )
                {
                    yield return new WaitForEndOfFrame();
                }

                nextState = FirstEncounter;
            }

            yield break;
        }

        protected virtual IEnumerator FirstEncounter()
        {
            Dev.Where();
            meshRenderer.enabled = true;

            FlipScale( gameObject );

            PlayAnimation( "Idle" );
            PlayAnimation( hornetPointClip );

            HeroController.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if( checkPlayerData )
            {
                GameManager.instance.playerData.SetInt( "hornetGreenpath", 4 );
            }
            GameManager.instance.playerData.SetBool( "disablePause", true );

            HeroController.instance.RelinquishControl();

            nextState = BoxUp;

            yield break;
        }

        protected virtual IEnumerator BoxUp()
        {
            Dev.Where();
            
            SendEventToFirstFSM(dialogueManager, "BOX UP");

            ShowBossTitle( this, areaTitleObject, -1f, "", "", "", "HORNET", "", "" );

            yield return new WaitForSeconds( .3f );

            nextState = Dialogue;

            yield break;
        }

        TextMeshPro GetDialogueTextMesh()
        {
            DialogueBox dialogue = dialogueManager.GetComponentInChildren<DialogueBox>();

            FieldInfo fi = dialogue.GetType().GetField("textMesh", BindingFlags.NonPublic|BindingFlags.Instance);
            TextMeshPro tm = fi.GetValue(dialogue) as TextMeshPro;
            return tm;
        }

        protected virtual IEnumerator Dialogue()
        {
            Dev.Where();

            dialogueManager.GetComponentInChildren<DialogueBox>().StartConversation( "HORNET_GREENPATH", "Hornet" );
            
            DialogueBox dialogue = dialogueManager.GetComponentInChildren<DialogueBox>();
            TextMeshPro tmp = GetDialogueTextMesh();
            
            PlayOneShot( hornetDialogueSFX );

            //TODO: make a helper function to edit the text pages
            //tmp.text = "Whomst. " + tmp.text;
            
            //wait for the player to finish the dialogue
            while( dialogue.currentPage <= tmp.textInfo.pageCount )
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = BoxDown;

            yield break;
        }

        protected virtual IEnumerator BoxDown()
        {
            Dev.Where();

            HideBossTitle( areaTitleObject );

            SendEventToFirstFSM(dialogueManager, "BOX DOWN");

            nextState = SetHeroActive;

            yield break;
        }

        protected virtual IEnumerator SetHeroActive()
        {
            Dev.Where();

            HeroController.instance.RegainControl();
            GameManager.instance.playerData.SetBool( "disablePause", false );
            nextState = LeapAntic;

            yield break;
        }

        protected virtual IEnumerator LeapAntic()
        {
            Dev.Where();

            SetFightGates( true );

            PlayOneShot( hornetJumpYells[ 0 ] );

            yield return PlayAndWaitForEndOfAnimation( "Evade Antic" );

            nextState = LeapBack;

            yield break;
        }

        protected virtual IEnumerator LeapBack()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );

            float xScale = gameObject.transform.localScale.x;
            float jumpAwaySpeed = xScale * evadeJumpAwaySpeed;

            body.velocity = new Vector2( jumpAwaySpeed, 0f );

            PlayAnimation( "Evade" );

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

            nextState = CinematicLand;

            yield break;
        }

        protected virtual IEnumerator CinematicLand()
        {
            Dev.Where();

            PlayAnimation( hornetSoftLandClip );

            yield return new WaitForSeconds( hornetSoftLandClip.Duration );

            nextState = Flourish;

            yield break;
        }

        //the start of the fight!
        protected virtual IEnumerator Flourish()
        {
            Dev.Where();

            PlayOneShot( hornetYell );

            PlayBossMusic( fightMusicSnapshot, fightMusic );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Flourish" );

            nextState = Idle;

            yield break;
        }

        protected virtual IEnumerator Idle()
        {
            Dev.Where();

            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            airDashPause = 999f;

            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            PlayAnimation( "Idle" );

            body.velocity = Vector2.zero;

            if( evadeRange.ObjectIsInRange )
            {
                nextState = EvadeAntic;
            }
            else
            {
                //use a while loop to yield that way other events may force a transition 
                float randomDelay = GameRNG.Rand(idleWaitMin, idleWaitMax);
                while( randomDelay > 0f )
                {
                    yield return new WaitForEndOfFrame();

                    KeepXVelocityZero();

                    //did something hit us?
                    if( wasHitRecently )
                    {
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

                bool flag = false;
                while( !flag )
                {
                    //TODO: create a weighted table type that has max hit/miss settings
                    int randomWeightedIndex = GameRNG.Rand(0, nextStates.Count);
                    if( randomWeightedIndex == 0 && ctIdle < 2 )
                    {
                        ctIdle += 1;
                        ctRun = 0;
                        nextState = nextStates[ 0 ];
                        flag = true;
                    }
                    else if( randomWeightedIndex == 1 && ctRun < 2 )
                    {
                        ctIdle = 0;
                        ctRun += 1;
                        nextState = nextStates[ 1 ];
                        flag = true;
                    }
                }
            }

            yield break;
        }

        protected virtual IEnumerator MaybeFlip()
        {
            Dev.Where();

            //50/50 chance to flip
            bool shouldFlip = GameRNG.CoinToss();
            if( shouldFlip )
            {
                FlipScale( gameObject );
            }

            nextState = RunAway;

            yield break;
        }

        protected virtual IEnumerator RunAway()
        {
            Dev.Where();

            if( runAwayCheck.ObjectIsInRange )
            {
                //face the knight
                GameObject hero = HeroController.instance.gameObject;
                if( hero.transform.position.x > gameObject.transform.position.x )
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
                else
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

                //then flip the other way
                FlipScale(gameObject);
            }

            nextState = RunAntic;

            yield break;
        }

        protected virtual IEnumerator RunAntic()
        {
            Dev.Where();

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Evade Antic" );

            nextState = Run;

            yield break;
        }

        protected virtual IEnumerator Run()
        {
            Dev.Where();

            runAudioSource.Play();

            float xVel = gameObject.transform.localScale.x * -runSpeed;

            PlayAnimation( "Run" );

            body.velocity = new Vector2( xVel, 0f );

            float randomDelay = GameRNG.Rand(runWaitMin, runWaitMax);
            while( randomDelay > 0f )
            {
                yield return new WaitForEndOfFrame();

                body.velocity = new Vector2( xVel, 0f );

                //did something hit us?
                if( wasHitRecently )
                {
                    nextState = DmgResponse;
                    break;
                }

                if( evadeRange.ObjectIsInRange )
                {
                    nextState = EvadeAntic;
                    break;
                }

                if( ( body.velocity.x > 0f && rightHit ) || ( body.velocity.x < 0f && leftHit ) )
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

        protected virtual IEnumerator MaybeGSphere()
        {
            Dev.Where();

            runAudioSource.Stop();

            if( sphereRange.ObjectIsInRange )
            {
                float randomChoice = GameRNG.Randf();
                if( randomChoice > chanceToThrow )
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

        protected virtual IEnumerator CanThrow()
        {
            Dev.Where();

            Vector3 currentPosition = gameObject.transform.position;

            Vector2 throwOrigin = currentPosition;
            Vector2 throwDirection = Vector2.left;

            //TODO: custom enhancement, get the unity vector direction to the hero and throw along that line
            HeroController hero = HeroController.instance;
            var direction = GetDirectionToTarget(gameObject, hero.gameObject);

            if( direction.right )
            {
                throwDirection = Vector2.right;
            }

            throwRay = new Ray( throwOrigin, throwDirection );
            throwRaycast = Physics2D.Raycast( throwOrigin, throwDirection, throwDistance, 1 << 8 );
            if( throwRaycast.collider != null )
            {
                //TODO: alter this code so that we can throw, but make it shorter and/or have hornet grapple
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
        protected virtual IEnumerator MoveChoiceA()
        {
            Dev.Where();

            bool flag = false;
            bool flag2 = false;
            int num = 0;
            while( !flag )
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0, 4);
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
                        nextState = SetADash;
                    }
                    if( num == 1 )
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        nextState = SetSphereA;
                    }
                    if( num == 2 )
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        nextState = GDashAntic;
                    }
                    if( num == 3 )
                    {
                        msThrow = 0;
                        ctThrow = 1;
                        nextState = ThrowAntic;
                    }
                }
                //else, randomly pick a skill to use
                else if( randomWeightedIndex == 0 && ctAirDash < maxChosenADash )
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow = 0;
                    nextState = SetADash;
                    flag = true;
                }
                else if( randomWeightedIndex == 1 && ctASphere < maxChosenASphere )
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    ctThrow = 0;
                    nextState = SetSphereA;
                    flag = true;
                }
                else if( randomWeightedIndex == 2 && ctGDash < maxChosenGDash )
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash += 1;
                    ctThrow = 0;
                    nextState = GDashAntic;
                    flag = true;
                }
                else if( randomWeightedIndex == 3 && ctThrow < maxChosenThrow )
                {
                    ctAirDash = 0;
                    ctASphere = 0;
                    ctGDash = 0;
                    ctThrow += 1;
                    nextState = ThrowAntic;
                    flag = true;
                }
            }

            if( nextState == null )
                nextState = ThrowAntic;

            yield break;
        }

        //"cannot throw"
        protected virtual IEnumerator MoveChoiceB()
        {
            Dev.Where();

            bool flag = false;
            bool flag2 = false;
            int num = 0;
            while( !flag )
            {
                //have any of our abilities not been used enough? then we're forced to use one
                int randomWeightedIndex = GameRNG.Rand(0, 3);
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
                        nextState = SetADash;
                    }
                    if( num == 1 )
                    {
                        msASphere = 0;
                        ctASphere = 1;
                        nextState = SetSphereA;
                    }
                    if( num == 2 )
                    {
                        msGDash = 0;
                        ctGDash = 1;
                        nextState = GDashAntic;
                    }
                }
                //else, randomly pick a skill to use
                else if( randomWeightedIndex == 0 && ctAirDash < maxChosenADash )
                {
                    ctAirDash += 1;
                    ctASphere = 0;
                    ctGDash = 0;
                    nextState = SetADash;
                    flag = true;
                }
                else if( randomWeightedIndex == 1 && ctASphere < maxChosenASphere )
                {
                    ctAirDash = 0;
                    ctASphere += 1;
                    ctGDash = 0;
                    nextState = SetSphereA;
                    flag = true;
                }
                else if( randomWeightedIndex == 2 && ctGDash < maxChosenGDash )
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

        protected virtual IEnumerator ThrowAntic()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;

            //disable stun control
            stunControl.isSuspended = true;

            //change our collider size to match the throw attack
            bodyCollider.offset = new Vector2( .5f, -.3f );
            bodyCollider.size = new Vector2( 1.2f, 2.6f );
            //bodyCollider.offset = new Vector2(1f, -.3f);
            //bodyCollider.size = new Vector2(1f, 2.6f);

            //face the hero
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            //stop moving
            body.velocity = Vector2.zero;

            PlayOneShotRandom( hornetAttackYells );

            //play throwing animation
            //wait here until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Throw Antic", KeepXVelocityZero );

            nextState = Throw;

            yield break;
        }

        protected virtual IEnumerator Throw()
        {
            Dev.Where();

            //play the throw sound effect
            PlayOneShot( hornetThrowSFX );

            //start the throw effect
            throwEffect.Play( gameObject );

            //shake the camera a bit
            DoEnemyKillShakeEffect();

            //change our collider size to match the during-throw attack
            bodyCollider.offset = new Vector2( .1f, -1.0f );
            bodyCollider.size = new Vector2( 1.4f, 1.2f );

            //start throwing the needle
            needle.Play( gameObject, throwWindUpTime, throwMaxTravelTime, throwRay, throwDistance );

            //put the needle tink on the needle
            needleTink.SetParent( needle.transform );

            //start the throw animation
            PlayAnimation( "Throw" );

            //wait one frame before ending
            yield return new WaitForEndOfFrame();

            nextState = Thrown;

            yield break;
        }

        protected virtual IEnumerator Thrown()
        {
            Dev.Where();

            //wait while the needle does its thing (boomerang effect)
            while( needle.isAnimating )
            {
                yield return new WaitForEndOfFrame();
            }

            nextState = ThrowRecover;

            yield break;
        }

        protected virtual IEnumerator ThrowRecover()
        {
            Dev.Where();

            //play catch sound
            PlayOneShot( hornetCatchSFX );

            //remove tink effect
            needleTink.SetParent( null );

            //allow stunning again
            stunControl.isSuspended = false;

            nextState = Escalation;

            yield break;
        }

        protected virtual IEnumerator SetADash()
        {
            Dev.Where();

            airDashPause = GameRNG.Rand( airDashPauseMin, airDashPauseMax );
            willSphere = false;

            nextState = JumpAntic;

            yield break;
        }

        protected virtual IEnumerator JumpAntic()
        {
            Dev.Where();

            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            body.velocity = Vector2.zero;

            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Jump Antic", KeepXVelocityZero );

            nextState = AimJump;

            yield break;
        }

        protected virtual IEnumerator AimJump()
        {
            Dev.Where();

            if( willSphere )
            {
                nextState = AimSphereJump;
            }
            else
            {
                //TODO: enchancement: make hornet jump at/near/away from the player

                Vector3 currentPosition = gameObject.transform.position;
                Vector2 jumpOrigin = currentPosition;
                Vector2 jumpDirectionL = Vector2.left;
                Vector2 jumpDirectionR = Vector2.right;

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

                jumpPoint = GameRNG.Rand( xMin, xMax );

                //if it's too close, don't jump
                if( Mathf.Abs( jumpPoint - currentPosition.x ) < 2.5f )
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

        protected virtual IEnumerator Jump()
        {
            Dev.Where();

            PlayOneShotRandom( hornetJumpYells );
            PlayOneShot( hornetJumpSFX );

            PlayAnimation( "Jump" );

            //TODO: this seems weird, see how it turns out
            body.velocity = new Vector2( jumpPoint, jumpVelocityY );

            nextState = InAir;

            yield break;
        }

        protected virtual IEnumerator InAir()
        {
            Dev.Where();

            float startHeight = gameObject.transform.position.y;

            //change collision check directions for jumping
            EnableCollisionsInDirection( false, true, false, false );

            Dev.Log( "Enabled downward collisions" );

            float airDashTimer = airDashPause;
            for(; ; )
            {
                yield return new WaitForEndOfFrame();

                bool withinSphereHeightRange = Mathf.Abs(gameObject.transform.position.y - startHeight) < minAirSphereHeight;
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
                if( bottomHit )
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
        protected virtual IEnumerator ReAim()
        {
            Dev.Where();

            nextState = AimJump;

            yield break;
        }

        protected virtual IEnumerator Land()
        {
            Dev.Where();

            PlayOneShot( hornetLandSFX );

            body.gravityScale = normGravity2DScale;

            bodyCollider.offset = new Vector2( .1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            gameObject.transform.rotation = Quaternion.identity;

            gameObject.transform.localScale = gameObject.transform.localScale.SetY( 1f );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Land" );

            nextState = Escalation;

            yield break;
        }

        protected virtual IEnumerator ADashAntic()
        {
            Dev.Where();

            if( aDashRange.ObjectIsInRange )
            {
                GameObject hero = HeroController.instance.gameObject;
                if( hero.transform.position.x > gameObject.transform.position.x )
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
                else
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

                bodyCollider.offset = new Vector2( -0.2f, -.7f );
                bodyCollider.size = new Vector2( 1.2f, 1.4f );
                //bodyCollider.offset = new Vector2(1.1f, -.9f);
                //bodyCollider.size = new Vector2(1.2f, 1.4f);

                body.velocity = Vector2.zero;

                body.gravityScale = 0f;

                PlayOneShotRandom( hornetAGDashYells );

                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation( "A Dash Antic" );

                nextState = Fire;
            }
            else
            {
                nextState = InAir;
            }

            yield break;
        }

        protected virtual IEnumerator Fire()
        {
            Dev.Where();

            PlayOneShot( hornetDashSFX );

            hitADash.enabled = true;

            GameObject hero = HeroController.instance.gameObject;

            float angleToTarget = GetAngleToTarget( gameObject, hero, 0f, -.5f);

            Vector2 pos = gameObject.transform.position;
            Vector2 fireVelocity = GetVelocityToTarget(pos, new Vector3(0f,-5f * gameObject.transform.localScale.x,0f), hero.transform.position, airFireSpeed, 0f);

            body.velocity = fireVelocity;

            Vector2 otherVelocity = GetVelocityFromSpeedAndAngle(airFireSpeed, angleToTarget);

            body.velocity = otherVelocity;
            aDashVelocity = body.velocity;

            bodyCollider.offset = new Vector2( .1f, 0f );
            bodyCollider.size = new Vector2( 1.5f, 1.0f );

            hitADash.gameObject.SetActive( true );

            Vector3 directionToHero = hero.transform.position - (Vector3)pos;
            if( directionToHero != Vector3.zero )
            {
                float angle = Mathf.Atan2(directionToHero.y, directionToHero.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis( angle + 180f, Vector3.forward );
            }

            Vector3 eulerAngles = gameObject.transform.eulerAngles;
            float zAngle = eulerAngles.z;

            if( zAngle > 90f && zAngle < 270f )
            {
                nextState = FiringR;
            }
            else
            {
                nextState = FiringL;
            }

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );
            gameObject.transform.localScale = gameObject.transform.localScale.SetY( 1f );

            yield break;
        }

        protected virtual IEnumerator FiringL()
        {
            Dev.Where();

            returnXScale = 1f;

            ClearPreviousCollisions();

            //change collision check directions for air dashing
            EnableCollisionsInDirection( true, true, true, false );

            nextState = ADash;

            yield break;
        }

        protected virtual IEnumerator FiringR()
        {
            Dev.Where();

            returnXScale = -1f;

            gameObject.transform.localScale = gameObject.transform.localScale.SetY( -1f );

            ClearPreviousCollisions();

            //change collision check directions for air dashing
            EnableCollisionsInDirection( true, true, false, true );

            nextState = ADash;

            yield break;
        }

        protected virtual IEnumerator ADash()
        {
            Dev.Where();

            aDashEffect.Play( gameObject );

            PlayOneShot( hornetDashSFX );

            DoEnemyKillShakeEffect();

            airDashPause = 999;

            PlayAnimation( "A Dash" );

            while( !bottomHit && !rightHit && !leftHit && !topHit )
            {
                yield return new WaitForEndOfFrame();

                //lock the velocity for the duration of the dash
                body.velocity = aDashVelocity;

                //added this in to keep hornet from clipping into walls
                var nextFrame = GetCollisionAlongCurrentVelocity(8,Time.deltaTime * 4f);

                Dev.Log( "next frame collisions (local checking):" + bottomHit + " " + rightHit + " " + leftHit + " " + topHit );
                Dev.Log( "next frame collisions (my checking):" + nextFrame.below + " " + nextFrame.above + " " + nextFrame.left + " " + nextFrame.right );

                //did we hit a wall? end evade timer early
                if( bottomHit || nextFrame.below )
                {
                    nextState = LandY;
                    break;
                }
                if( topHit || nextFrame.above )
                {
                    nextState = HitRoof;
                    break;
                }
                if( leftHit || nextFrame.left )
                {
                    nextState = WallL;
                    break;
                }
                if( rightHit || nextFrame.right )
                {
                    nextState = WallR;
                    break;
                }
            }

            //restore collision check directions
            EnableCollisionsInDirection( false, false, true, true );

            if( nextState == null )
                nextState = LandY;

            yield break;
        }

        protected virtual void DoWallLand( float xScale )
        {
            PlayOneShot( hornetWallLandSFX );

            gameObject.transform.rotation = Quaternion.identity;

            body.velocity = Vector2.zero;

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( xScale );
            gameObject.transform.localScale = gameObject.transform.localScale.SetY( 1f );

            hitADash.gameObject.SetActive( false );
            hitADash.enabled = false;

            bodyCollider.offset = new Vector2( .1f, -0.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );
        }

        protected virtual IEnumerator WallL()
        {
            Dev.Where();

            DoWallLand( 1f );

            yield return PlayAndWaitForEndOfAnimation( "Wall Impact" );

            nextState = JumpR;

            yield break;
        }

        protected virtual IEnumerator WallR()
        {
            Dev.Where();

            DoWallLand( -1f );

            yield return PlayAndWaitForEndOfAnimation( "Wall Impact" );

            nextState = JumpL;

            yield break;
        }

        protected virtual void DoShortJump( float xDirection )
        {
            float xScale = -1f * Mathf.Sign(xDirection);

            body.velocity = new Vector2( jumpDistance * xDirection, jumpVelocityY * .5f );

            PlayAnimation( "Jump" );

            body.gravityScale = normShortJumpGravity2DScale;

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( xScale );
        }

        protected virtual IEnumerator JumpL()
        {
            Dev.Where();

            DoShortJump( -1f );

            nextState = InAir;

            yield break;
        }

        protected virtual IEnumerator JumpR()
        {
            Dev.Where();

            DoShortJump( 1f );

            nextState = InAir;

            yield break;
        }

        protected virtual IEnumerator LandY()
        {
            Dev.Where();

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( returnXScale );

            hitADash.gameObject.SetActive( false );

            nextState = HardLand;

            yield break;
        }

        protected virtual IEnumerator HitRoof()
        {
            Dev.Where();

            gameObject.transform.localScale = gameObject.transform.localScale.SetX( returnXScale );

            hitADash.gameObject.SetActive( false );

            body.velocity = Vector2.zero;

            hitADash.enabled = false;

            body.gravityScale = normShortJumpGravity2DScale;

            bodyCollider.offset = new Vector2( .1f, -0.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            gameObject.transform.rotation = Quaternion.identity;

            nextState = InAir;

            yield break;
        }

        protected virtual IEnumerator HardLand()
        {
            Dev.Where();

            dustHardLand.Play();

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Hard Land" );

            BlockingAnimationIsPlaying = true;

            body.gravityScale = normGravity2DScale;

            bodyCollider.offset = new Vector2( .1f, -0.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            gameObject.transform.rotation = Quaternion.identity;

            gameObject.transform.localScale = gameObject.transform.localScale.SetY( 1f );

            float decelerationX = .8f;
            for(; ; )
            {
                Vector2 velocity = body.velocity;
                if( velocity.x < 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x > 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                else if( velocity.x > 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x < 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                if( velocity.x < 0.001f && velocity.x > -0.001f )
                {
                    velocity.x = 0f;
                }
                body.velocity = velocity;

                if( !BlockingAnimationIsPlaying )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }


            nextState = Escalation;

            yield break;
        }

        protected virtual IEnumerator MaybeDoSphere()
        {
            Dev.Where();

            willSphere = false;

            if( aSphereRange.ObjectIsInRange )
            {
                nextState = SphereAnticA;
            }
            else
            {
                nextState = InAir;
            }

            yield break;
        }

        protected virtual IEnumerator SphereAnticA()
        {
            Dev.Where();

            willSphere = false;

            body.gravityScale = 0f;

            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "Sphere Antic A" );

            BlockingAnimationIsPlaying = true;

            PlayOneShotRandom( hornetAttackYells );

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
                if( velocity.x < 0.001f && velocity.x > -0.001f )
                {
                    velocity.x = 0f;
                }
                if( velocity.y < 0.001f && velocity.y > -0.001f )
                {
                    velocity.y = 0f;
                }
                body.velocity = velocity;

                if( !BlockingAnimationIsPlaying )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = SphereA;

            yield break;
        }

        protected virtual IEnumerator SphereA()
        {
            Dev.Where();

            PlayOneShot( hornetSphereSFX );

            sphereBall.Play( gameObject, aSphereTime, .8f, aSphereSize );
            flashEffect.Play( gameObject );

            DoEnemyKillShakeEffect();

            PlayAnimation( "Sphere Attack" );

            //TODO: move to variables
            float waitTime = aSphereTime;
            float deceleration = .8f;
            while( waitTime > 0f )
            {
                waitTime -= Time.fixedDeltaTime;

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
                if( velocity.x < 0.001f && velocity.x > -0.001f )
                {
                    velocity.x = 0f;
                }
                if( velocity.y < 0.001f && velocity.y > -0.001f )
                {
                    velocity.y = 0f;
                }
                body.velocity = velocity;

                yield return new WaitForFixedUpdate();
            }

            nextState = SphereRecoverA;

            yield break;
        }

        protected virtual IEnumerator SphereRecoverA()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation( "Sphere Recover A" );

            sphereBall.Stop();

            nextState = SphereAEnd;

            yield break;
        }

        protected virtual IEnumerator SphereAEnd()
        {
            Dev.Where();

            body.gravityScale = normGravity2DScale;

            PlayAnimation( "Fall" );

            nextState = InAir;

            yield break;
        }

        protected virtual IEnumerator AimSphereJump()
        {
            Dev.Where();

            //TODO: enchancement: make hornet jump at/near/away from the player

            Vector3 currentPosition = gameObject.transform.position;
            Vector2 jumpOrigin = currentPosition;
            Vector2 jumpDirectionL = Vector2.left;
            Vector2 jumpDirectionR = Vector2.right;

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

            jumpPoint = GameRNG.Rand( xMin, xMax );

            nextState = Jump;

            yield break;
        }

        protected virtual IEnumerator SetJumpOnly()
        {
            Dev.Where();

            airDashPause = 999f;

            willSphere = false;

            nextState = JumpAntic;

            yield break;
        }

        protected virtual IEnumerator SetSphereA()
        {
            Dev.Where();

            willSphere = true;

            airDashPause = 999f;

            nextState = JumpAntic;

            yield break;
        }

        protected virtual IEnumerator GDashAntic()
        {
            Dev.Where();

            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            bodyCollider.offset = new Vector2( 0.5f, -.9f );
            bodyCollider.size = new Vector2( 1.2f, 1.4f );
            //bodyCollider.offset = new Vector2(1.1f, -.9f);
            //bodyCollider.size = new Vector2(1.2f, 1.4f);

            body.velocity = Vector2.zero;

            PlayOneShotRandom( hornetAGDashYells );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "G Dash Antic", KeepXVelocityZero );

            nextState = GDash;

            yield break;
        }

        protected virtual IEnumerator GDash()
        {
            Dev.Where();

            PlayAnimation( "G Dash" );

            PlayOneShot( hornetDashSFX );

            DoEnemyKillShakeEffect();

            ClearPreviousCollisions();

            gDashEffect.Play( gameObject );
            hitGDash.gameObject.SetActive( true );

            bodyCollider.offset = new Vector2( 0.1f, -.8f );
            bodyCollider.size = new Vector2( 1.6f, 1.5f );

            hitGDash.gameObject.SetActive( true );

            float xScale = gameObject.transform.localScale.x;

            body.velocity = new Vector2( -gDashSpeed * xScale, 0f );
            gDashVelocity = body.velocity;

            float waitTimer = maxGDashTime;
            while( waitTimer > 0f )
            {
                yield return new WaitForEndOfFrame();

                //lock the velocity for the duration of the dash
                body.velocity = gDashVelocity;

                //did we hit a wall? then end the dash.
                if( ( body.velocity.x > 0f && rightHit ) || ( body.velocity.x < 0f && leftHit ) )
                {
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            nextState = GDashRecover1;

            yield break;
        }

        protected virtual IEnumerator GDashRecover1()
        {
            Dev.Where();

            hitGDash.gameObject.SetActive( false );

            bodyCollider.offset = new Vector2( 1.1f, -0.9f );
            bodyCollider.size = new Vector2( 1.2f, 1.4f );

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "G Dash Recover1" );

            BlockingAnimationIsPlaying = true;

            //TODO: move into a variable
            float decelerationX = .77f;
            for(; ; )
            {
                Vector2 velocity = body.velocity;
                if( velocity.x < 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x > 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                else if( velocity.x > 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x < 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                if( velocity.x < 0.001f && velocity.x > -0.001f )
                {
                    velocity.x = 0f;
                }
                body.velocity = velocity;

                if( !BlockingAnimationIsPlaying )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = GDashRecover2;

            yield break;
        }

        protected virtual IEnumerator GDashRecover2()
        {
            Dev.Where();

            bodyCollider.offset = new Vector2( .1f, -0.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            tk2dAnimator.AnimationCompleted = OnAnimationComplete;
            tk2dAnimator.Play( "G Dash Recover2" );

            BlockingAnimationIsPlaying = true;

            //TODO: move into a variable
            float decelerationX = .75f; 
            for(; ; )
            {
                Vector2 velocity = body.velocity;
                if( velocity.x < 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x > 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                else if( velocity.x > 0f )
                {
                    velocity.x *= decelerationX;
                    if( velocity.x < 0f )
                    {
                        velocity.x = 0f;
                    }
                }
                if( velocity.x < 0.001f && velocity.x > -0.001f )
                {
                    velocity.x = 0f;
                }
                body.velocity = velocity;

                if( !BlockingAnimationIsPlaying )
                    break;
                else
                    yield return new WaitForFixedUpdate();
            }

            nextState = Escalation;

            yield break;
        }

        protected virtual IEnumerator SphereAnticG()
        {
            Dev.Where();

            body.velocity = Vector2.zero;

            bodyCollider.offset = new Vector2( 0.1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            PlayOneShotRandom( hornetAttackYells );

            yield return PlayAndWaitForEndOfAnimation( "Sphere Antic G", KeepXVelocityZero );

            nextState = Sphere;

            yield break;
        }

        protected virtual IEnumerator Sphere()
        {
            Dev.Where();
            PlayOneShot( hornetSphereSFX );

            //TODO: move the start value into a variable
            sphereBall.Play( gameObject, gSphereTime, .8f, gSphereSize );
            flashEffect.Play( gameObject );

            DoEnemyKillShakeEffect();

            PlayAnimation( "Sphere Attack" );

            yield return new WaitForSeconds( gSphereTime );

            nextState = SphereRecover;

            yield break;
        }

        protected virtual IEnumerator SphereRecover()
        {
            Dev.Where();

            yield return PlayAndWaitForEndOfAnimation( "Sphere Recover G" );

            sphereBall.Stop();

            nextState = Escalation;

            yield break;
        }

        protected virtual IEnumerator EvadeAntic()
        {
            Dev.Where();

            runAudioSource.Stop();

            if( evadeRange.ObjectIsInRange )
            {
                //put her evade on cooldown
                float randomDelay = GameRNG.Rand(evadeCooldownMin, evadeCooldownMax);
                evadeRange.DisableEvadeForTime( randomDelay );

                //stop her moving
                body.velocity = Vector2.zero;

                //make her face you
                GameObject hero = HeroController.instance.gameObject;
                if( hero.transform.position.x > gameObject.transform.position.x )
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
                else
                    gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

                //animate the evade-anticipation                
                //play until the callback fires and changes our state
                yield return PlayAndWaitForEndOfAnimation( "Evade Antic", KeepXVelocityZero );

                nextState = Evade;
            }
            else
            {
                nextState = MaybeGSphere;
            }

            yield break;
        }

        protected virtual IEnumerator Evade()
        {
            Dev.Where();

            PlayOneShotRandom( hornetLaughs );
            PlayOneShot( hornetSmallJumpSFX );

            PlayAnimation( "Evade" );

            float xScale = gameObject.transform.localScale.x;
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

            nextState = EvadeLand;

            yield break;
        }

        protected virtual IEnumerator EvadeLand()
        {
            Dev.Where();

            //animate the evade-landing            
            body.velocity = Vector2.zero;

            PlayOneShot( hornetGroundLandSFX );

            //play until the callback fires and changes our state
            yield return PlayAndWaitForEndOfAnimation( "Land", KeepXVelocityZero );

            nextState = AfterEvade;

            yield break;
        }

        protected virtual IEnumerator AfterEvade()
        {
            Dev.Where();

            bool attack = GameRNG.CoinToss();
            if( attack )
            {
                nextState = MaybeGSphere;
            }
            else
            {
                nextState = Idle;
            }

            yield break;
        }

        protected virtual IEnumerator DmgResponse()
        {
            Dev.Where();

            wasHitRecently = false;
            runAudioSource.Stop();

            int choice = GameRNG.WeightedRand(DmgResponseChoices.Values.ToList());
            nextState = DmgResponseChoices.Keys.ToList()[ choice ];

            yield break;
        }

        protected virtual IEnumerator DmgIdle()
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

            nextState = MaybeGSphere;

            yield break;
        }


        protected virtual IEnumerator RefightReady()
        {
            Dev.Where();
            body.isKinematic = false;
            bodyCollider.enabled = true;
            meshRenderer.enabled = true;

            PlayAnimation( "Idle" );

            //wait for player to get close
            while( !refightRange.ObjectIsInRange )
            {
                yield return new WaitForEndOfFrame();
            }

            //close the gates
            SetFightGates( true );

            ShowBossTitle( this, areaTitleObject, 2f, "", "", "", "HORNET", "", "" );

            nextState = Flourish;

            yield break;
        }

        //called by the stun controller when a stun happens
        protected virtual void OnStun()
        {
            Dev.Where();
            BlockingAnimationIsPlaying = false;
            currentState = null;
            overrideNextState = StunStart;
        }

        //transition here when the next state is set to this by OnStun
        protected virtual IEnumerator StunStart()
        {
            Dev.Where();

            //play stun yell
            PlayOneShotRandom( hornetStunYells );

            //reset collider
            bodyCollider.offset = new Vector2( 0.1f, -.3f );
            bodyCollider.size = new Vector2( .9f, 2.6f );

            //stop ball
            sphereBall.Stop();

            //create stun effect
            //NOTE: this could probably be replaced by GameObject.Instantiate, but for now we're following hollow knight's way of doing things
            stunEffectPrefab.Spawn( Vector3.zero, Quaternion.identity );

            //reset gravity
            body.gravityScale = 1.5f;

            //reset rotation
            gameObject.transform.rotation = Quaternion.identity;

            //reset Y scale
            gameObject.transform.localScale = gameObject.transform.localScale.SetY( 1f );

            //face hero
            GameObject hero = HeroController.instance.gameObject;
            if( hero.transform.position.x > gameObject.transform.position.x )
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( -1f );
            else
                gameObject.transform.localScale = gameObject.transform.localScale.SetX( 1f );

            float stunAirVelocity = gameObject.transform.localScale.x * stunAirXVelocity;

            PlayAnimation( "Stun Air" );

            //launch the boss
            body.velocity = new Vector2( stunAirVelocity, stunAirYVelocity );

            //disable the attacks
            needle.Stop();
            hitADash.gameObject.SetActive( false );
            hitGDash.gameObject.SetActive( false );
            needleTink.SetParent( null );

            wasHitRecently = false;

            nextState = StunAir;

            yield break;
        }

        protected virtual IEnumerator StunAir()
        {
            Dev.Where();

            //change collision check directions for jumping
            EnableCollisionsInDirection( false, true, false, false );

            for(; ; )
            {
                yield return new WaitForEndOfFrame();

                //did we hit a wall? end evade timer early
                if( bottomHit )
                {
                    nextState = StunLand;
                    break;
                }
            }

            //restore collision check directions
            EnableCollisionsInDirection( false, false, true, true );

            yield break;
        }

        protected virtual IEnumerator StunLand()
        {
            Dev.Where();

            PlayAnimation(  "Stun" );

            float waitTimer = stunTime;
            while( waitTimer > 0f )
            {
                yield return new WaitForEndOfFrame();

                //lock the velocity for the duration of the stun
                body.velocity = Vector2.zero;

                //did we hit a wall? then end the dash.
                if( wasHitRecently )
                {
                    break;
                }

                waitTimer -= Time.deltaTime;
            }

            wasHitRecently = false;

            nextState = StunRecover;

            yield break;
        }

        protected virtual IEnumerator StunRecover()
        {
            Dev.Where();

            nextState = SetJumpOnly;

            yield break;
        }

        protected virtual IEnumerator Escalation()
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

                evadeCooldownMin = esEvadeCooldownMin;
                evadeCooldownMax = esEvadeCooldownMax;

                dmgIdleWaitMin = esDmgIdleWaitMin;
                dmgIdleWaitMax = esDmgIdleWaitMax;

                airDashPauseMin = esAirDashPauseMin;
                airDashPauseMax = esAirDashPauseMax;
            }

            nextState = Idle;

            yield break;
        }

        protected virtual void KeepXVelocityZero()
        {
            body.velocity = new Vector2( 0f, body.velocity.y );
        }
        
        protected virtual void PlayAnimation( string animation )
        {
            PlayAnimation( tk2dAnimator, animation );
        }

        protected virtual void PlayAnimation( string animation, int frame )
        {
            PlayAnimation( tk2dAnimator, animation, frame );
        }

        protected virtual void PlayAnimation( tk2dSpriteAnimationClip animation )
        {
            tk2dAnimator.Play( animation );
        }

        protected virtual IEnumerator PlayAndWaitForEndOfAnimation( string animation, Action doWhileWaiting = null )
        {
            yield return PlayAndWaitForEndOfAnimation( tk2dAnimator, animation, doWhileWaiting );
        }

        protected virtual void PlayOneShotRandom( List<AudioClip> clips )
        {
            PlayOneShotRandom( actorAudioSource, clips );
        }

        protected virtual void PlayOneShot( AudioClip clip )
        {
            PlayOneShot( actorAudioSource, clip );
        }

        //Setup functions///////////////////////////////////////////////////////////////////////////

        protected virtual void SetupBossReferences()
        {
            if( gameObject.FindGameObjectInChildren( "Refight Range" ) != null )
                refightRange = gameObject.FindGameObjectInChildren( "Refight Range" ).AddComponent<RangeCheck>();
            if( gameObject.FindGameObjectInChildren( "Evade Range" ) != null )
                evadeRange = gameObject.FindGameObjectInChildren( "Evade Range" ).AddComponent<EvadeRange>();
            if( gameObject.FindGameObjectInChildren( "Sphere Range" ) != null )
                sphereRange = gameObject.FindGameObjectInChildren( "Sphere Range" ).AddComponent<RangeCheck>();
            if( gameObject.FindGameObjectInChildren( "A Sphere Range" ) != null )
                aSphereRange = gameObject.FindGameObjectInChildren( "A Sphere Range" ).AddComponent<RangeCheck>();
            if( gameObject.FindGameObjectInChildren( "A Dash Range" ) != null )
                aDashRange = gameObject.FindGameObjectInChildren( "A Dash Range" ).AddComponent<RangeCheck>();
            if( gameObject.FindGameObjectInChildren( "Hit ADash" ) != null )
                hitADash = gameObject.FindGameObjectInChildren( "Hit ADash" ).GetComponent<PolygonCollider2D>();
            if( gameObject.FindGameObjectInChildren( "Hit GDash" ) != null )
                hitGDash = gameObject.FindGameObjectInChildren( "Hit GDash" ).GetComponent<PolygonCollider2D>();
            if( gameObject.FindGameObjectInChildren( "Dust HardLand" ) != null )
                dustHardLand = gameObject.FindGameObjectInChildren( "Dust HardLand" ).GetComponent<ParticleSystem>();
            if( gameObject.FindGameObjectInChildren( "Run Away Check" ) != null )
                runAwayCheck = gameObject.FindGameObjectInChildren( "Run Away Check" ).AddComponent<RangeCheck>();


            if( gameObject.FindGameObjectInChildren( "Throw Effect" ) != null )
                throwEffect = gameObject.FindGameObjectInChildren( "Throw Effect" ).AddComponent<ThrowEffect>();
            if( gameObject.FindGameObjectInChildren( "A Dash Effect" ) != null )
                aDashEffect = gameObject.FindGameObjectInChildren( "A Dash Effect" ).AddComponent<ADashEffect>();
            if( gameObject.FindGameObjectInChildren( "G Dash Effect" ) != null )
                gDashEffect = gameObject.FindGameObjectInChildren( "G Dash Effect" ).AddComponent<GDashEffect>();
            if( gameObject.FindGameObjectInChildren( "Sphere Ball" ) != null )
                sphereBall = gameObject.FindGameObjectInChildren( "Sphere Ball" ).AddComponent<SphereBall>();
            if( gameObject.FindGameObjectInChildren( "Flash Effect" ) != null )
                flashEffect = gameObject.FindGameObjectInChildren( "Flash Effect" ).AddComponent<FlashEffect>();

            //TODO: replace this with a load from the effects database
            if( UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Needle" ) != null )
                needle = UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Needle" ).AddComponent<Needle>();

            if( GameObject.Find( "Needle Tink" ) != null )
                needleTink = GameObject.Find( "Needle Tink" ).AddComponent<NeedleTink>();

            if( UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Corpse Hornet 1(Clone)" ) != null )
            {
                hornetCorpse = UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Corpse Hornet 1(Clone)" );
#if UNITY_EDITOR
#else
                hornetCorpse.PrintSceneHierarchyTree( hornetCorpse.name );
#endif
                hornetCorpse.AddComponent<HornetCorpse>();
            }
            gameObject.AddComponent<PreventOutOfBounds>();
            stunControl = gameObject.AddComponent<StunController>();
            stunControl.onStun += OnStun;

            //gameObject.AddComponent<DebugColliders>();
            //needle.gameObject.AddComponent<DebugColliders>();
            //needleTink.gameObject.AddComponent<DebugColliders>();
            //HeroController.instance.gameObject.AddComponent<DebugColliders>();            
        }

        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources();

            //load resources for the boss
            string bossFSMName = "Control";

            yield return GetAudioPlayRandomClipsFromFSM( gameObject, bossFSMName, "Stun Start", SetHornetStunYells );

            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Sphere", SetHornetSphereSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Wall L", SetHornetWallLandSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Fire", SetHornetDashSFX );
            yield return GetAudioPlayerOneShotClipsFromFSM( gameObject, bossFSMName, "ADash Antic", SetHornetAGDashYells );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Land", SetHornetLandSFX );
            yield return GetAudioPlayerOneShotClipsFromFSM( gameObject, bossFSMName, "Jump", SetHornetJumpYells );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Jump", SetHornetJumpSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Evade Land", SetHornetGroundLandSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Evade", SetHornetSmallJumpSFX );
            yield return GetAudioPlayerOneShotClipsFromFSM( gameObject, bossFSMName, "Evade", SetHornetLaughs );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Throw Recover", SetHornetCatchSFX );
            yield return GetAudioClipFromAudioPlaySimpleInFSM( gameObject, bossFSMName, "Throw", SetHornetThrowSFX );
            yield return GetAudioPlayerOneShotClipsFromFSM( gameObject, bossFSMName, "Throw Antic", SetHornetAttackYells );
            yield return GetGameObjectFromFSM( gameObject, bossFSMName, "Flourish", SetAreaTitleReference );
            yield return GetGameObjectFromSpawnObjectFromGlobalPoolInFSM( gameObject, bossFSMName, "Stun Start", SetStunEffect, false );
            yield return GetAudioSourceFromAudioPlayerOneShotSingleInFSM( gameObject, bossFSMName, "Flourish", SetActorAudioSource );
            yield return GetAudioClipFromAudioPlayerOneShotSingleInFSM( gameObject, bossFSMName, "Flourish", SetHornetYell );
            
            fightMusic = GetMusicCueFromFSM( gameObject, bossFSMName, "Flourish" );
            fightMusicSnapshot = GetSnapshotFromTransitionToAudioSnapshotInFSM( gameObject, bossFSMName, "Flourish" );

            //load additional resources from other things

            //load the required references for a first encounter
            GameObject encounter = UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04_boss" ).FindGameObject( "Hornet Infected Knight Encounter" );

            if( encounter != null )
            {
                //System.IO.StreamWriter file = null;
                //file = new System.IO.StreamWriter( Application.dataPath + "/Managed/Mods/" + encounter.name );
                //encounter.PrintSceneHierarchyTree( true, file );
                //file.Close();
                string introFSMName = "Encounter";

                tk2dSpriteAnimator otherAnim = encounter.GetComponent<tk2dSpriteAnimator>();
                hornetPointClip = otherAnim.GetClipByName( "Point" );
                hornetSoftLandClip = otherAnim.GetClipByName( "Soft Land" );
                hornetJumpFullClip = otherAnim.GetClipByName( "Jump Full" );

                yield return GetAudioClipFromAudioPlaySimpleInFSM( encounter, introFSMName, "Dialogue", SetHornetDialogueSFX );
                yield return GetGameObjectFromSendEvent( encounter, introFSMName, "Box Up", SetDialogueManager, false );

                //and remove it when done
                GameObject.DestroyImmediate( encounter );
            }

            GameObject item = UnityEngine.SceneManagement.SceneManager.GetSceneByName( "Fungus1_04" ).FindGameObject( "Shiny Item" );
            if( item != null )
            {
#if UNITY_EDITOR
#else
                item.PrintSceneHierarchyTree( item.name );
#endif
                item.AddComponent<ShinyItem>();
            }
            //TODO: get "Grass Escape" game object for a particle effect

            yield break;
        }

        protected override void RemoveDeprecatedComponents()
        {
            //preserve the fsms on the corpse so that their data may be extracted by the corpse components
            if( hornetCorpse != null )
            {
                hornetCorpse.transform.SetParent( null );
            }

            base.RemoveDeprecatedComponents();

            if( hornetCorpse != null )
            {
                hornetCorpse.transform.SetParent( transform );
            }
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

        void SetDialogueManager( GameObject dialogueManagerObject )
        {
            if( dialogueManagerObject == null )
            {
                Dev.Log( "Warning: Stun Effect GameObject failed to load and is null!" );
                return;
            }

            dialogueManager = dialogueManagerObject;
        }

        void SetHornetDialogueSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet sphere sfx clip is null!" );
                return;
            }

            hornetDialogueSFX = clip;
        }

        void SetHornetStunYells( List<AudioClip> clips )
        {
            if( clips == null )
            {
                Dev.Log( "Warning: hornet stun yells are null clips!" );
                return;
            }

            hornetStunYells = clips;
        }

        void SetHornetSphereSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet sphere sfx clip is null!" );
                return;
            }

            hornetSphereSFX = clip;
        }

        void SetHornetWallLandSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet wall land sfx clip is null!" );
                return;
            }

            hornetWallLandSFX = clip;
        }

        void SetHornetDashSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet dash sfx clip is null!" );
                return;
            }

            hornetDashSFX = clip;
        }

        void SetHornetAGDashYells( List<AudioClip> clips )
        {
            if( clips == null )
            {
                Dev.Log( "Warning: hornet ag dash yells are null clips!" );
                return;
            }

            hornetAGDashYells = clips;
        }

        void SetHornetLandSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet land sfx clip is null!" );
                return;
            }

            hornetLandSFX = clip;
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

        void SetHornetThrowSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet throw sfx clip is null!" );
                return;
            }

            hornetThrowSFX = clip;
        }

        void SetHornetCatchSFX( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet catch sfx clip is null!" );
                return;
            }

            hornetCatchSFX = clip;
        }

        void SetHornetYell( AudioClip clip )
        {
            if( clip == null )
            {
                Dev.Log( "Warning: hornet yell clip is null!" );
                return;
            }

            hornetYell = clip;
        }

        void SetHornetAttackYells( List<AudioClip> clips )
        {
            if( clips == null )
            {
                Dev.Log( "Warning: hornet throw yells are null clips!" );
                return;
            }

            hornetAttackYells = clips;
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

        void SetStunEffect( GameObject stunEffect )
        {
            if( stunEffect == null )
            {
                Dev.Log( "Warning: Stun Effect GameObject failed to load and is null!" );
                return;
            }

            stunEffectPrefab = stunEffect;
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
        //end helpers /////////////////////////////
    }//end class
}//end namespace
