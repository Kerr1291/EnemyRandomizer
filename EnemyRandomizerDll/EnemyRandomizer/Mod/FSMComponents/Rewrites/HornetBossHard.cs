using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{
    public class HornetBossHard : HornetBoss
    {
        //needs some testing
        public int hardMaxHP = 800;
        public int hardMaxHPBonus = 400;
        public float evadeSpeedModifier = 4f;
        public float idleWaitModifier = .1f;
        public float evadeRangeOnTimeMod = .1f;
        public float evadeRangeCooldownMod = .1f;
        public float runSpeedMod = 2f;
        public float runWaitMod = .1f;
        public float grappleMaxVelocity = 44f;
        public float grappleTime = .25f;

        public int needleDamage = 2;

        public float throwAnticMod = 2f;
        public float throwWindUpMod = .2f;
        public float throwMaxTravelTimeMod = .6f;

        public float newThrowDistance = 24f;
        public int maxStunCount = 12;

        public override Dictionary<Func<IEnumerator>, float> DmgResponseChoices
        {
            get
            {
                if(dmgResponseChoices == null)
                {
                    dmgResponseChoices = new Dictionary<Func<IEnumerator>, float>();
                    dmgResponseChoices.Add(EvadeAntic, .455f);
                    dmgResponseChoices.Add(SetJumpOnly, .25f);
                    dmgResponseChoices.Add(MaybeGSphere, .25f);
                    dmgResponseChoices.Add(DmgIdle, .05f);
                }
                return dmgResponseChoices;
            }
        }

        public override void ShowTitle(float hideTime)
        {
            ShowBossTitle(this, areaTitleObject, hideTime, "", "", "", "HORNET", "", "THE GUARDIAN");
        }

        protected override IEnumerator Init()
        {
            yield return base.Init();

            maxHP = hardMaxHP + GameRNG.Rand(0, hardMaxHPBonus);

            tk2dAnimator.GetClipByName("Evade Antic").fps *= evadeSpeedModifier;
            evadeJumpAwayTimeLength /= evadeSpeedModifier;
            evadeJumpAwaySpeed *= evadeSpeedModifier;

            idleWaitMin *= idleWaitModifier;
            idleWaitMax *= idleWaitModifier;

            evadeRange.onTimeMin *= evadeRangeOnTimeMod;
            evadeRange.onTimeMax *= evadeRangeOnTimeMod;

            evadeCooldownMin *= evadeRangeCooldownMod;
            evadeCooldownMax *= evadeRangeCooldownMod;

            tk2dAnimator.GetClipByName("Run").fps *= runSpeedMod;
            runSpeed *= runSpeedMod;
            runWaitMin *= runWaitMod;
            runWaitMax *= runWaitMod;
            
            tk2dAnimator.GetClipByName("Throw Antic").fps *= throwAnticMod;
            throwWindUpTime *= throwWindUpMod;
            throwMaxTravelTime *= throwMaxTravelTimeMod;

            throwDistance = newThrowDistance;

            needle.GetComponent<DamageHero>().damageDealt = needleDamage;

            stunControl.maxStuns = maxStunCount;

            yield break;
        }

        protected override void SelectNextStateFromIdle()
        {
            //nothing hit us, choose the next state with 50/50
            List<Func<IEnumerator>> nextStates = new List<Func<IEnumerator>>()
                {
                    MaybeFlip, MaybeGSphere
                };

            if(sphereRange.ObjectIsInRange)
            {
                ctIdle = 0;
                ctRun += 1;
                nextState = nextStates[1];
            }
            else
            {
                ctIdle += 1;
                ctRun = 0;
                nextState = nextStates[0];
            }
        }

        protected override IEnumerator Escalation()
        {
            Dev.Where();

            //see if we're low on hp and should act faster
            float hpRemainingPercent = (float)healthManager.hp / (float)maxHP;
            if(!escalated && hpRemainingPercent < escalationHPPercentage)
            {
                //TODO: not sure yet what I want to escalate....
            }

            nextState = Idle;

            yield break;
        }

        //change the throw to aim at the hero
        protected override IEnumerator CanThrow()
        {
            Dev.Where();

            HeroController hero = HeroController.instance;
            Vector3 currentPosition = gameObject.transform.position;

            Vector2 throwOrigin = currentPosition;

            float lead = GameRNG.Rand(0f, .2f);

            //aim a bit ahead of our hero
            Vector2 target = hero.GetComponent<Rigidbody2D>().velocity * lead + (Vector2)hero.transform.position;

            //clamp the y prediction so we don't throw into the ground
            if(target.y < throwOrigin.y)
            {
                target.y = throwOrigin.y;
            }

            Vector2 throwDirection = (target - throwOrigin).normalized;
            
            throwRay = new Ray(throwOrigin, throwDirection);
            throwRaycast = Physics2D.Raycast(throwOrigin, throwDirection, throwDistance, 1 << 8);
            //Dev.CreateLineRenderer( throwOrigin, throwRaycast.point, Color.white, -2f, .1f );
            //Dev.Log( "ray hit " + throwRaycast.collider.gameObject );

            if( throwRaycast.collider != null && throwRaycast.collider.gameObject != null && throwRaycast.distance < 2f )
            {
                Dev.Log( "Target is too close! Skipping throw." );
                Dev.Log( "" + throwRaycast.point );

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

        protected override void DoThrowNeedle()
        {
            Dev.Where();
            needle.canHitWalls = true;
            needle.Play(gameObject, throwWindUpTime, throwMaxTravelTime, throwRay, throwDistance);
        }

        protected override IEnumerator Thrown()
        {
            Dev.Where();

            //wait while the needle does its thing (boomerang effect)
            while(needle.isAnimating)
            {
                //if( !needle.gameObject.activeInHierarchy )
                //    needle.gameObject.SetActive( true );

                yield return new WaitForEndOfFrame();
            }

            if(needle.HitWall)
            {
                nextState = Grapple;
            }
            else
            {
                nextState = ThrowRecover;
            }

            yield break;
        }


        void OnBoundsCollision( RaycastHit2D ray, GameObject self, GameObject other )
        {
            Dev.Where();
            BoundsExtensions.CreateLineRenderer( transform.position, ray.point, Color.white, -2f, .2f );
            Dev.Log( ""+ray.normal );
            if(ray.normal.x < 0f)
            {
                rightHit = true;
            }
            else
            if( ray.normal.x > 0f )
            {
                leftHit = true;
            }
            else//( Mathf.Abs( ray.normal.y ) > .9f )
            {
                topHit = true;
            }
        }

        protected override void OnCollisionEnter2D( Collision2D collision )
        {
            base.OnCollisionEnter2D( collision );

            if( isGrappling )
            {
                Dev.Where();
                RaycastHit2D raycast = GetRaycastInDirection( transform.position, ( needle.transform.position - transform.position ).normalized );
                if(raycast.distance <= (bodyCollider.size.magnitude * raycast.normal).magnitude)
                    OnBoundsCollision( raycast, gameObject, raycast.collider.gameObject );
                else
                {
                    Dev.Log( "raycast.distance = " + raycast.distance );
                    Dev.Log( "( bodyCollider.size.magnitude * raycast.normal ).magnitude = " + ( bodyCollider.size.magnitude * raycast.normal ).magnitude );
                }
            }
        }

        protected override void OnCollisionStay2D( Collision2D collision )
        {
            base.OnCollisionEnter2D( collision );

            if( isGrappling )
            {
                Dev.Where();
                RaycastHit2D raycast = GetRaycastInDirection( transform.position, ( needle.transform.position - transform.position ).normalized );



                if( raycast.distance <= ( bodyCollider.size.magnitude * raycast.normal ).magnitude )
                    OnBoundsCollision( raycast, gameObject, raycast.collider.gameObject );
                else
                {
                    Dev.Log( "raycast.distance = " + raycast.distance );
                    Dev.Log( "( bodyCollider.size.magnitude * raycast.normal ).magnitude = " + ( bodyCollider.size.magnitude * raycast.normal ).magnitude );
                }
            }
        }

        bool isGrappling = false;

        protected virtual IEnumerator Grapple()
        {
            Dev.Where();

            boundsChecking.onBoundCollision += OnBoundsCollision;

            EnableCollisionsInDirection(true, false, true, true);

            PlayOneShotRandom(hornetJumpYells);

            bodyCollider.offset = new Vector2( -0.2f, -.7f );
            bodyCollider.size = new Vector2( 1.2f, 1.4f );

            //orient hornet at the spot
            //float grappleRotation = GetAngleToTarget(gameObject, needle.gameObject, 0f, 0f);
            //Vector3 grappleRotationVector = new Vector3(0f, 0f, grappleRotation);
            //transform.Rotate(grappleRotationVector);

            Vector3 grapplePoint = needle.transform.position;
            grapplePoint.y = Mathf.Max( transform.position.y, grapplePoint.y );

            Vector3 facing = ( grapplePoint - transform.position ).normalized;
            //facing.x = facing.x;
            transform.right = facing;
            if( facing.x < 0f )
                transform.right = -transform.right;

            //use the air dash zoom effect
            aDashEffect.Play(gameObject);

            PlayOneShot(hornetDashSFX);

            //shake the screen for dramatic effect
            //DoEnemyKillShakeEffect();

            //get the grapple clip
            //tk2dSpriteAnimationClip grappleClip =
            //    hornetCorpse.GetComponent<HornetCorpse>().leaveAnim.GetClipByName("Harpoon Side");

            //play it
            PlayAnimation("A Dash");//TODO: look into getting the grapple clip and playing that here

            //make hornet zoom away
            //get the escape direction and add some distance one so the smoothdamp finishes inside the wall
            float distanceToGrapple = (grapplePoint - transform.position).magnitude + 10f;

            //calculate the new escape point farther away (inside the wall)
            Vector3 projectedGrapplePoint = transform.position + ((grapplePoint - transform.position)).normalized * (distanceToGrapple);
            Vector3 grappleVelocity = Vector3.zero;
            //Dev.CreateLineRenderer( transform.position, grapplePoint, Color.white, -2f, .2f );
            isGrappling = true;
            while(!rightHit && !leftHit && !topHit)
            {
                yield return new WaitForEndOfFrame();

                body.velocity = Vector2.zero;

                //lock the velocity for the duration of the dash
                transform.position = Vector3.SmoothDamp(transform.position, projectedGrapplePoint, ref grappleVelocity, grappleTime, grappleMaxVelocity, Time.deltaTime);
                distanceToGrapple = ( projectedGrapplePoint - transform.position).magnitude;
                //Dev.Log( ""+ transform.position);
                //did we hit a wall? end evade timer early
                if(topHit)
                {
                    nextState = HitRoof;
                    break;
                }
                if(leftHit)
                {
                    nextState = WallL;
                    break;
                }
                if(rightHit)
                {
                    nextState = WallR;
                    break;
                }
            }

            isGrappling = false;
            boundsChecking.onBoundCollision -= OnBoundsCollision;

            //restore collision check directions
            RestorePreviousCollisionDirections();

            //play catch sound
            PlayOneShot(hornetCatchSFX);

            //stop the needle
            needle.Stop();

            //remove tink effect
            needleTink.SetParent(null);

            //allow stunning again
            stunControl.isSuspended = false;

            yield break;
        }
    }
}