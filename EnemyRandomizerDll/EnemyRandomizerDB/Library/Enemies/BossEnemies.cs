using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
using HutongGames.PlayMaker;
using Modding;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;
using UniRx;

namespace EnemyRandomizerMod
{
    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MantisTraitorLordControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mantis";

        public GameObject megaMantisTallSlash;

        public override float spawnPositionOffset => 1f;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //enable this after activating/repositioning traitor lord
            this.InsertHiddenState(control, "Init", "FINISHED", "Fall");

            //remove all journal stuff from the boss fsm
            control.OverrideState("Journal", () =>{ });
            

            var fall = control.GetState("Fall");
            fall.DisableAction(1);
            fall.DisableAction(2);
            fall.DisableAction(3);
            fall.DisableAction(4);
            fall.DisableAction(5);
            fall.DisableAction(6);
            fall.DisableAction(8);
            fall.DisableAction(10);
            fall.AddCustomAction(() => { control.SendEvent("LAND"); });

            var introLand = control.GetState("Intro Land");
            introLand.DisableAction(0);
            introLand.DisableAction(1);
            introLand.DisableAction(3);
            introLand.DisableAction(6);
            introLand.ChangeTransition("FINISHED", "Active");

            var active = control.GetState("Active");
            active.DisableAction(4);
            active.DisableAction(5);
            active.DisableAction(6);

            var roarRecover = control.GetState("Roar Recover");
            roarRecover.DisableAction(1);

            var idle = control.GetState("Idle");
            idle.DisableAction(0);
            idle.InsertCustomAction(() => {  
            if(EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            },0);

            var waves = control.GetState("Waves");

            //extract wave prefab
            megaMantisTallSlash = waves.GetAction<SpawnObjectFromGlobalPool>(0).gameObject.Value;
            waves.DisableAction(0);
            waves.DisableAction(3);

            waves.InsertCustomAction(() => {
                var s = megaMantisTallSlash.Spawn(transform.position, Quaternion.identity);
                control.FsmVariables.GetFsmGameObject("Projectile").Value = s;
                if (!PlayerData.instance.canShadowDash && PlayerData.instance.quakeLevel <= 0)
                {
                    s.transform.localScale = new Vector3(.1f, .1f, .1f);
                }
            }, 3);
            waves.InsertCustomAction(() => {
                var s = megaMantisTallSlash.Spawn(transform.position, Quaternion.identity);
                control.FsmVariables.GetFsmGameObject("Projectile").Value = s;
                if (!PlayerData.instance.canShadowDash && PlayerData.instance.quakeLevel <= 0)
                {
                    s.transform.localScale = new Vector3(.1f, .1f, .1f);
                }
            }, 0);
        }
    }

    public class MantisTraitorLordSpawner : DefaultSpawner<MantisTraitorLordControl> { }

    public class MantisTraitorLordPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MimicSpiderControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mimic Spider";
        public bool skipToIdle = false;

        public override bool preventInsideWallsAfterPositioning => false;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public float customTransformMinAliveTime = 1f;
        public float customTransformAggroRange = 6f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Trans 1");

            var constrainx = gameObject.LocateMyFSM("constrain_x");
            Destroy(constrainx);

            var init = control.GetState("Init");
            init.DisableAction(1);
            init.DisableAction(2);

            var trans1 = control.GetState("Trans 1");
            trans1.DisableAction(0);
            trans1.DisableAction(1);
            trans1.DisableAction(3);
            trans1.DisableAction(4);
            trans1.DisableAction(5);
            trans1.DisableAction(7);
            trans1.DisableAction(8);
            trans1.DisableAction(9);
            trans1.DisableAction(10);
            //trans1.DisableAction(11);
            trans1.GetAction<Wait>(12).time = 0.1f;
            trans1.DisableAction(14);
            trans1.InsertCustomAction(() => {
                if (skipToIdle)
                    control.SetState("Idle");
            }, 0);

            var trans2 = control.GetState("Trans 2");
            trans2.GetAction<Wait>(2).time = 0.1f;

            var trans3 = control.GetState("Trans 3");
            trans3.GetAction<Wait>(2).time = 0.1f;

            var trans4 = control.GetState("Trans 4");
            trans4.GetAction<Wait>(1).time = 0.1f;

            var trans5 = control.GetState("Trans 5");
            trans5.GetAction<Wait>(1).time = 0.1f;

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(0);
            roarEnd.DisableAction(2);

            var roarInit = control.GetState("Roar Init");            
            roarInit.DisableAction(0);
            roarInit.DisableAction(1);
            roarInit.DisableAction(2);
            roarInit.DisableAction(3);
            roarInit.DisableAction(4);
            roarInit.DisableAction(5);
            roarInit.ChangeTransition("FINISHED", "Idle");
            roarInit.InsertCustomAction(() => { skipToIdle = true; }, 0);

            var encountered = control.GetState("Encountered");
            control.OverrideState("Encountered", () => { control.SetState("Init"); });

            var idle = control.GetState("Idle");
            idle.DisableAction(3);
            idle.DisableAction(5);
            idle.RemoveTransition("ROAR");

            var aimJump = control.GetState("Aim Jump");
            aimJump.DisableAction(1);
            aimJump.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Target X").Value = heroPos2d.x;
            },0);

            var roofJump = control.GetState("Roof Jump?");
            roofJump.ChangeTransition("ROOF SPIT", "Idle");
        }

        protected override void CheckControlInCustomHiddenState()
        {
            if (customTransformMinAliveTime > 0)
            {
                customTransformMinAliveTime -= Time.deltaTime;
                return;
            }

            if (gameObject.CanSeePlayer() && gameObject.DistanceToPlayer() < customTransformAggroRange)
            {
                base.CheckControlInCustomHiddenState();
            }
        }
    }

    public class MimicSpiderSpawner : DefaultSpawner<MimicSpiderControl> { }

    public class MimicSpiderPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////













    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class LostKinControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM balloonFSM;

        public override string FSMName => "IK Control";

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            balloonFSM = gameObject.LocateMyFSM("Spawn Balloon");

            var pause = control.GetState("Pause");
            pause.DisableAction(1);
            pause.AddCustomAction(() => { control.SendEvent("FINISHED"); });

            var waiting = control.GetState("Waiting");
            waiting.DisableAction(2);
            waiting.DisableAction(3);
            waiting.DisableAction(4);
            waiting.DisableAction(5);
            waiting.AddCustomAction(() => { control.SendEvent("BATTLE START"); });

            var closeGates = control.GetState("Close Gates");
            closeGates.DisableAction(0);
            closeGates.DisableAction(3);

            var setx = control.GetState("Set X");
            setx.DisableAction(0);
            setx.DisableAction(2);

            var introFall = control.GetState("Intro Fall");
            introFall.DisableAction(2);
            introFall.AddCustomAction(() => { control.SendEvent("LAND"); });

            control.ChangeTransition("Intro Land", "FINISHED", "First Counter");
            this.InsertHiddenState(control, "Intro Land", "FINISHED", "First Counter");

            var inAir2 = control.GetState("In Air 2");
            inAir2.DisableAction(3);
            inAir2.AddAction(new Wait() { time = 3f, finishEvent = new FsmEvent("LAND") });

            var inAir = control.GetState("In Air");
            inAir.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Min Dstab Height").Value = floorY + 6f;
                control.FsmVariables.GetFsmFloat("Air Dash Height").Value = floorY + 3f;
            }, 0);

            var inert = balloonFSM.GetState("Inert");
            inert.AddCustomAction(() => { balloonFSM.SendEvent("START SPAWN"); });

            var stop = balloonFSM.GetState("Stop");
            inert.AddCustomAction(() => { balloonFSM.SendEvent("START SPAWN"); });

            balloonFSM.OverrideState("Spawn", () =>
            {
                if (ChildController.AtMaxChildren)
                {
                    balloonFSM.SendEvent("STOP SPAWN");
                }

                if(gameObject.CanSeePlayer())
                {
                    var randomDir = UnityEngine.Random.insideUnitCircle;
                    var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 12f);

                    if(spawnRay.distance < 5f)
                    {
                        balloonFSM.SendEvent("CANCEL");
                    }
                    else
                    {
                        var spawnPoint = spawnRay.point - spawnRay.normal;
                        ChildController.SpawnAndTrackChild("Parasite Balloon", spawnPoint);
                        balloonFSM.SendEvent("FINISHED");
                    }
                }
                else
                {
                    balloonFSM.SendEvent("CANCEL");
                }
            });
        }
    }


    public class LostKinSpawner : DefaultSpawner<LostKinControl> { }


    public class LostKinPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordPhase2Control : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mage Lord 2";

        protected override bool DisableCameraLocks => true;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public float teleAboveHeroHeight = 10f;
        public Vector2 teleQuakePoint;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var pause = control.GetState("Pause");
            pause.ChangeTransition("FINISHED", "Init");

            this.InsertHiddenState(control, "Pause", "FINISHED", "Init");

            var init = control.GetState("Init");
            init.DisableAction(6);
            init.AddCustomAction(() => { control.SendEvent("PHASE 2"); });
            init.ChangeTransition("PHASE 2", "Tele Quake");

            var teleQuake  = control.GetState("Tele Quake");
            teleQuake.DisableAction(3);
            teleQuake.DisableAction(4);
            teleQuake.DisableAction(5);
            teleQuake.InsertCustomAction(() => {

                var aboveHero = gameObject.GetTeleportPositionAbovePlayer(teleAboveHeroHeight - 1f, teleAboveHeroHeight + 1f);
                teleQuakePoint = aboveHero + Vector2.down * 2f;
                var dist = (aboveHero - heroPos2d).magnitude;
                if (dist < teleAboveHeroHeight * 0.5f)
                {
                    control.SendEvent("CANCEL");
                }

            }, 3);

            //var shiftq = control.GetState("Shift?");
            //shiftq.DisableAction(1);
            //shiftq.DisableAction(2);
            //shiftq.DisableAction(3);

            var teleportQ = control.GetState("TeleportQ");
            teleportQ.DisableAction(0);
            teleportQ.DisableAction(1);
            teleportQ.DisableAction(2);
            teleportQ.DisableAction(3);
            teleportQ.DisableAction(7);
            teleportQ.InsertCustomAction(() => {

                control.FsmVariables.GetFsmGameObject("Tele Out Anim").Value.transform.position = transform.position;
                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = teleQuakePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = teleQuakePoint;
            },0);

            var quakeLand = control.GetState("Quake Land");
            quakeLand.DisableAction(1);
            quakeLand.DisableAction(9);
            quakeLand.InsertCustomAction(() =>
            {
                gameObject.StickToGroundX();
            }, 0);

            var teleAway = control.GetState("Tele Away");
            teleAway.DisableAction(1);
            teleAway.DisableAction(2);

            var awayValidq = control.GetState("Away Valid?");
            awayValidq.DisableAction(0);
            awayValidq.DisableAction(1);
            awayValidq.DisableAction(3);
            awayValidq.DisableAction(4);
            awayValidq.DisableAction(5);
            awayValidq.DisableAction(6);
            awayValidq.AddCustomAction(() => { control.SendEvent("FINISHED"); });

            var teleport = control.GetState("Teleport");
            teleport.DisableAction(0);
            teleport.DisableAction(2);
            teleport.DisableAction(3);
            teleport.DisableAction(4);
            teleport.InsertCustomAction(() => {

                Vector3 telePoint = gameObject.GetRandomPositionInLOSofPlayer(3f, 20f, 2f);

                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;
            }, 0);

            control.OverrideState("Fireball Pos", () => { control.SendEvent("FINISHED"); });

            var orbSummon = control.GetState("Orb Summon");
            orbSummon.DisableAction(0);
            orbSummon.InsertCustomAction(() => {

                var telePoint = gameObject.GetRandomPositionInLOSofSelf(1f, 4f, 0f);
                control.FsmVariables.GetFsmVector3("Fireball Pos").Value = telePoint;
            }, 0);

            var spawnFireball = control.GetState("Spawn Fireball");
            spawnFireball.DisableAction(0);

            var teleOut = control.GetState("Tele Out");
            teleOut.DisableAction(3);
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);

            GetComponent<PreventOutOfBounds>().onBoundCollision -= ForceDownward;
            GetComponent<PreventOutOfBounds>().onBoundCollision += ForceDownward;
        }

        protected virtual void ForceDownward(RaycastHit2D r, GameObject a, GameObject b)
        {
            if (control.ActiveStateName == "Quake Down")
            {
                if (pos2d.y > heroPos2d.y)
                {
                    var poob = gameObject.GetComponent<PreventOutOfBounds>();
                    if (poob != null)
                    {
                        poob.ForcePosition(pos2d + Vector2.down * 2f);
                    }
                }
            }
        }
    }

    public class MageLordPhase2Spawner : DefaultSpawner<MageLordPhase2Control> { }

    public class MageLordPhase2PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mage Lord";

        protected override bool DisableCameraLocks => true;
        public override bool preventOutOfBoundsAfterPositioning => true;

        public float teleAboveHeroHeight = 12f;
        public float chargeHeight => gameObject.GetOriginalObjectSize().y * this.SizeScale + 0.2f;
        public float spinnerHeight => gameObject.GetOriginalObjectSize().y * this.SizeScale + 0.2f + 2f;

        public bool isTeleCharge = false;
        public Vector2 teleQuakePoint;
        public Vector2 teleChargePoint;
        public Vector2 teleChargePoint2;

        public float highSpinnerSpeed = 9.5f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var destroyIfDefeated = gameObject.LocateMyFSM("Destroy If Defeated");
            if (destroyIfDefeated != null)
                GameObject.Destroy(destroyIfDefeated);

            try
            {
                var init = control.GetState("Init");
                init.DisableAction(1);
                init.DisableAction(4);
                init.DisableAction(5);
                init.AddCustomAction(() => { control.SendEvent("FINISHED"); });
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING INIT -- {e.Message} {e.StackTrace}"); }

            try
            {
                var sleep = control.GetState("Sleep");
                sleep.DisableAction(0);

                this.InsertHiddenState(control, "Init", "FINISHED", "Teleport In");

                var teleportIn = control.GetState("Teleport In");
                teleportIn.DisableAction(3);
                teleportIn.DisableAction(4);
                teleportIn.DisableAction(5);
                teleportIn.DisableAction(6);
                teleportIn.DisableAction(7);
                teleportIn.InsertCustomAction(() =>
                {
                    var telePoint = gameObject.GetRandomPositionInLOSofSelf(2f, 20f, 10f);

                    control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                    transform.position = telePoint;
                    control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;

                }, 0);
                teleportIn.ChangeTransition("FINISHED", "Set Idle Timer");
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING SLEEP TELE IN -- {e.Message} {e.StackTrace}"); }

            try
            {
                var teleShoot = control.GetState("Tele Shoot");
                teleShoot.DisableAction(1);
                teleShoot.DisableAction(2);

                var shootValidq = control.GetState("Shoot Valid?");
                shootValidq.DisableAction(0);
                shootValidq.DisableAction(1);
                shootValidq.DisableAction(3);
                shootValidq.DisableAction(4);
                shootValidq.DisableAction(5);
                shootValidq.DisableAction(6);
                shootValidq.AddCustomAction(() => { control.SendEvent("FINISHED"); });


                var teleAway = control.GetState("Tele Away");
                teleAway.DisableAction(1);
                teleAway.DisableAction(2);

                var awayValidq = control.GetState("Away Valid?");
                awayValidq.DisableAction(0);
                awayValidq.DisableAction(1);
                awayValidq.DisableAction(3);
                awayValidq.DisableAction(4);
                awayValidq.DisableAction(5);
                awayValidq.DisableAction(6);
                awayValidq.AddCustomAction(() => { control.SendEvent("FINISHED"); });

                var teleport = control.GetState("Teleport");
                teleport.DisableAction(4);
                teleport.DisableAction(5);
                teleport.DisableAction(6);
                teleport.DisableAction(7);
                //teleport.DisableAction(11);
                teleport.InsertCustomAction(() =>
                {
                    Vector3 telePoint;
                    if (isTeleCharge)
                    {
                        telePoint = teleChargePoint;
                        isTeleCharge = false;
                    }
                    else
                    {
                        telePoint = gameObject.GetRandomPositionInLOSofSelf(2f, 20f, 2f);
                    }

                    control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                    transform.position = telePoint;
                    control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;
                }, 0);



                var teleCharge = control.GetState("Tele Charge");
                teleCharge.DisableActions(1,2,3,4,5,6,7,8);
                teleCharge.AddCustomAction(() =>
                {
                    isTeleCharge = true;
                    var left = gameObject.GetHorizontalTeleportPositionFromPlayer(false, chargeHeight);
                    var right = gameObject.GetHorizontalTeleportPositionFromPlayer(true, chargeHeight);

                    float leftd = (left - heroPos2d).magnitude;
                    float rightd = (right - heroPos2d).magnitude;

                    if(leftd > rightd)
                    {
                        teleChargePoint = left;
                        teleChargePoint2 = right;
                        control.FsmVariables.GetFsmBool("High Spinner Right").Value = true;
                        gameObject.SetSpriteDirection(false);
                    }
                    else
                    {
                        control.FsmVariables.GetFsmBool("High Spinner Left").Value = true;
                        teleChargePoint = right;
                        teleChargePoint2 = left;
                        gameObject.SetSpriteDirection(true);
                    }
                });

                var teleBotY = control.GetState("Tele Bot Y");
                teleBotY.DisableAction(0);

                var teleTopY = control.GetState("Tele Top Y");
                teleBotY.DisableAction(0);



                var teleSpinnerX = control.GetState("Tele Spinner X");
                teleSpinnerX.DisableActions(1, 3, 4, 5, 6, 7, 8, 10);
                teleSpinnerX.AddCustomAction(() =>
                {
                    isTeleCharge = true;
                    var left = gameObject.GetHorizontalTeleportPositionFromPlayer(false, spinnerHeight);
                    var right = gameObject.GetHorizontalTeleportPositionFromPlayer(true, spinnerHeight);

                    float leftd = (left - heroPos2d).magnitude;
                    float rightd = (right - heroPos2d).magnitude;

                    if (leftd > rightd)
                    {
                        teleChargePoint = left;
                        teleChargePoint2 = right;
                        control.FsmVariables.GetFsmBool("High Spinner Right").Value = true;
                        gameObject.SetSpriteDirection(false);
                    }
                    else
                    {
                        control.FsmVariables.GetFsmBool("High Spinner Left").Value = true;
                        teleChargePoint = right;
                        teleChargePoint2 = left;
                        gameObject.SetSpriteDirection(true);
                    }
                });


                var hsRight = control.GetState("HS Right");
                hsRight.DisableActions(0,1,2);
                hsRight.AddCustomAction(() =>
                {
                    PhysicsBody.velocity = new Vector2(highSpinnerSpeed, 0f);
                    StartCoroutine(SendFinishedOnXPositionOrDelay(teleChargePoint2.x));
                });


                var hsLeft = control.GetState("HS Left");
                hsLeft.DisableActions(0, 1, 2, 3, 4);
                hsLeft.AddCustomAction(() =>
                {
                    PhysicsBody.velocity = new Vector2(-highSpinnerSpeed, 0f);
                    StartCoroutine(SendFinishedOnXPositionOrDelay(teleChargePoint2.x));
                });

                var rePos = control.GetState("Re Pos");
                rePos.DisableActions(0);
                rePos.AddCustomAction(() =>
                {
                    transform.position = gameObject.GetTeleportPositionAbovePlayer(5f, 35f, 2f) + Vector2.down * 2f;
                });
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING TELE AWAY VALID TELE -- {e.Message} {e.StackTrace}"); }

            try
            {
                var shot = control.GetState("Shot");
                shot.DisableAction(2);

                var teleQuake = control.GetState("Tele Quake");
                teleQuake.DisableAction(2);
                teleQuake.DisableAction(3);
                teleQuake.DisableAction(4);
                teleQuake.InsertCustomAction(() =>
                {
                    var aboveHero = gameObject.GetTeleportPositionAbovePlayer(teleAboveHeroHeight - 1f, teleAboveHeroHeight + 1f);
                    //teleQuakePoint = aboveHero;
                    teleQuakePoint = aboveHero + Vector2.down * 2f;
                    var dist = (aboveHero - heroPos2d).magnitude;
                    if (dist < teleAboveHeroHeight * 0.5f)
                    {
                        control.SendEvent("CANCEL");
                    }
                }, 2);

                var teleportQ = control.GetState("TeleportQ");
                teleportQ.DisableAction(0);
                teleportQ.DisableAction(3);
                teleportQ.DisableAction(4);
                teleportQ.DisableAction(5);
                teleportQ.DisableAction(9);
                teleportQ.InsertCustomAction(() =>
                {
                    control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                    transform.position = teleQuakePoint;
                    control.FsmVariables.GetFsmVector3("Teleport Point").Value = teleQuakePoint;
                }, 0);

                //var quakeDown = control.GetState("Quake Down");
                //quakeDown.DisableAction(6);
                //quakeDown.DisableAction(7);
                //quakeDown.AddCustomAction(() =>
                //{
                //    StartCoroutine(SendFinishedOnGroundOrDelay());
                //});

                var quakeLand = control.GetState("Quake Land");
                quakeLand.DisableAction(3);
                quakeLand.DisableAction(13);
                quakeLand.InsertCustomAction(() =>
                {
                    gameObject.StickToGroundX();
                }, 0);

                var teleOut = control.GetState("HS Tele Out");
                teleOut.DisableAction(2);
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING QUAKE Q DOWN LAND -- {e.Message} {e.StackTrace}"); }



            control.AddTimeoutAction(control.GetState("Charge Left"), "FINISHED", 1f);
            control.AddTimeoutAction(control.GetState("Charge Right"), "FINISHED", 1f);
            control.AddTimeoutAction(control.GetState("HS Ret Left"), "FINISHED", 0.5f);
            control.AddTimeoutAction(control.GetState("Hs Ret Right"), "FINISHED", 0.5f);
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);
            GetComponent<PreventOutOfBounds>().onBoundCollision -= OnCollision;
            GetComponent<PreventOutOfBounds>().onBoundCollision += OnCollision;
        }

        protected virtual void OnCollision(RaycastHit2D r, GameObject a, GameObject b)
        {
            if (control.ActiveStateName == "Quake Down")
            {
                if (pos2d.y > heroPos2d.y)
                {
                    var poob = gameObject.GetComponent<PreventOutOfBounds>();
                    if (poob != null)
                    {
                        poob.ForcePosition(pos2d + Vector2.down * 2f);
                    }
                }
            }

            if( control.ActiveStateName == "Charge Left"   ||
                control.ActiveStateName == "Charge Right"   ||
                control.ActiveStateName == "HS Left" ||
                control.ActiveStateName == "HS Ret Left" ||
                control.ActiveStateName == "Hs Ret Right" ||
                control.ActiveStateName == "HS Right"   )
            {
                control.SendEvent("FINISHED");
            }
        }

        IEnumerator SendFinishedOnXPositionOrDelay(float endPosX, float timeout = 3f)
        {
            for (; ; )
            {
                if (timeout <= 0)
                    break;

                float vel = PhysicsBody.velocity.x;

                if (vel < 0 && pos2d.x < endPosX)
                    break;

                if (vel > 0 && pos2d.x > endPosX)
                    break;

                timeout -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (control.ActiveStateName == "HS Right" || control.ActiveStateName == "HS Left")
                control.SendEvent("FINISHED");

            yield break;
        }


        IEnumerator SendFinishedOnGroundOrDelay()
        {
            bool poobGround = false;
            void HitSurface(RaycastHit2D r, GameObject a, GameObject b)
            {
                poobGround = true;
            }

            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            if(poob != null)
            {
                poob.onBoundCollision += HitSurface;
            }

            float timeout = 2f;
            float toGround = 1f;
            var rayToGround = SpawnerExtensions.GetGroundX(gameObject);
            if (rayToGround.distance >= 1f)
            {
                for (; ; )
                {
                    if (timeout <= 0)
                        break;

                    rayToGround = SpawnerExtensions.GetGroundX(gameObject);
                    if (rayToGround.distance < toGround || poobGround)
                        break;

                    timeout -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (control.ActiveStateName == "Quake Down")
                control.SendEvent("FINISHED");

            if (poob != null)
            {
                poob.onBoundCollision -= HitSurface;
            }

            yield break;
        }
    }

    public class MageLordSpawner : DefaultSpawner<MageLordControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class MageLordPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class WhiteDefenderControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Dung Defender";
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;

        public float peakEruptHeight = 12f;
        public float eruptStartOffset = 3f;
        public float defaultHP;
        public float defaultRageHP=350f;
        public float rageTriggerRatio;
        public float tooFarToSlamDist = 7f;
        public float tooCloseToWallDist = 5f;
        public float buriedYOffset = 3f;
        public float tunnelTooCloseToWall = 2f;
        public float tunnelTurnTowardPlayerDist = 10f;
        public float airDiveHeight = 15.4f;
        public float pillarHeight = 10.68f;
        public Vector2 buriedOrigin;
        public Vector2 eruptOrigin;

        protected GameObject pooBallW;
        protected Vector3 pooThrowOffset;

        public GameObject burrowEffectClone;
        public GameObject burrowEffectParticles;

        public void PlayBurrowEffect()
        {
            var mr = burrowEffectClone.GetComponent<MeshRenderer>();
            mr.enabled = true;

            var sprite = burrowEffectClone.GetComponent<tk2dSpriteAnimator>();
            sprite.PlayFromFrame(0);
            sprite.Play("Burrow Effect");

            burrowEffectParticles.GetComponent<ParticleSystem>().Emit(0);

            //TODO: finish this
            //TODO
            //TODO
            //TODO
            //TODO
            //TODO
            //TODO
            //TODO
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var constrainX = gameObject.LocateMyFSM("Constrain X");
            if(constrainX != null)
            {
                Destroy(constrainX);
            }

            var originalBurrowEffect = gameObject.GetDirectChildren().FirstOrDefault(x => x.name.Contains("Burrow Effect"));

            var burrowEffect = originalBurrowEffect.LocateMyFSM("Burrow Effect");
            if (burrowEffect != null)
                Destroy(burrowEffect);

            var keepY = originalBurrowEffect.LocateMyFSM("Keep Y");
            if (keepY != null)
                Destroy(keepY);

            burrowEffectClone = GameObject.Instantiate(originalBurrowEffect);
            burrowEffectClone.transform.parent = transform;
            Destroy(originalBurrowEffect);
            burrowEffectClone.SafeSetActive(false);

            burrowEffectParticles = burrowEffectClone.FindGameObjectInChildrenWithName("Particles");

            defaultHP = gameObject.OriginalPrefabHP();
            rageTriggerRatio = defaultHP / defaultRageHP;

            this.InsertHiddenState(control, "Init 2", "FINISHED", "Wake");

            var init = control.GetState("Init");
            init.DisableActions(1, 2);

            var wake = control.GetState("Wake");
            wake.DisableActions(1, 7, 9);
            wake.InsertCustomAction(() => {
                PositionBoss(.3f);
                burrowEffect.gameObject.StickToGroundX(-.5f);
                burrowEffect.transform.position -= new Vector3(0f, 0.2f * SizeScale, 0f);
                PlayBurrowEffect();
                //gameObject.transform.position -= new Vector3(0f, -2f * SizeScale, 0f);
            }, 5);

            var eruptOutFirst2 = control.GetState("Erupt Out First 2");
            eruptOutFirst2.DisableActions(0, 1, 6, 8, 11, 16);
            eruptOutFirst2.InsertCustomAction(() => CalculateAndSetupErupt(), 8);
            eruptOutFirst2.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First 2", "END")); });

            var introFall = control.GetState("Intro Fall");
            introFall.DisableActions(0);
            control.AddTimeoutAction(introFall, "LAND", 2f);
            //introFall.AddCustomAction(() => { control.StartCoroutine(TimeoutState("Intro Fall", "LAND", 2f)); });

            var introLand = control.GetState("Intro Land");
            introLand.InsertCustomAction(() => { UpdateBurrowEffect(); }, 0);

            var introRoar = control.GetState("Intro Roar");
            introRoar.DisableActions(1, 2, 6, 7, 12, 13, 14, 15, 16, 17, 18);

            var music = control.GetState("Music");
            music.DisableActions(0, 1, 2);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableActions(0, 1);

            var rageRoar = control.GetState("Rage Roar");
            rageRoar.DisableActions(2, 3, 4, 5);

            var roarq = control.GetState("Roar?");
            roarq.DisableActions(5, 6, 10, 11, 12, 13, 14, 15, 16);

            var rageIn = control.GetState("Rage In");
            rageIn.DisableActions(3, 7);

            var diveIn2 = control.GetState("Dive In 2");
            diveIn2.DisableActions(3);

            var underground = control.GetState("Underground");
            underground.DisableActions(0);
            underground.InsertCustomAction(() => CalculateAndSetupBuried(), 0);

            var tunnelingL = control.GetState("Tunneling L");
            tunnelingL.DisableActions(4, 7, 8, 9);
            tunnelingL.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling L", true)));

            var tunnelingR = control.GetState("Tunneling R");
            tunnelingR.DisableActions(0, 4, 7, 8, 9);
            tunnelingR.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling R", false)));

            var eruptAntic = control.GetState("Erupt Antic");
            eruptAntic.DisableActions(3);

            var eruptAnticR = control.GetState("Erupt Antic R");
            eruptAnticR.DisableActions(2);

            var eruptOutFirst = control.GetState("Erupt Out First");
            eruptOutFirst.DisableActions(1, 3, 6, 10);
            eruptOutFirst.InsertCustomAction(() => CalculateAndSetupErupt(), 3);
            eruptOutFirst.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First", "END")); });

            var eruptOut = control.GetState("Erupt Out");
            eruptOut.DisableActions(2, 5, 9, 14);
            eruptOut.InsertCustomAction(() => CalculateAndSetupErupt(), 5);
            eruptOut.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out", "END")); });

            var eruptFall = control.GetState("Erupt Fall");
            eruptFall.DisableActions(0);
            control.AddTimeoutAction(eruptFall, "LAND", 2f);
            //eruptFall.AddCustomAction(() => { StartCoroutine(TimeoutState("Erupt Fall", "LAND", 2f)); });

            var eruptLand = control.GetState("Erupt Land");
            eruptLand.DisableActions(4);
            eruptLand.InsertCustomAction(() => { gameObject.transform.position = eruptOrigin; }, 0);

            var throw1 = control.GetState("Throw 1");
            pooBallW = throw1.GetAction<SpawnObjectFromGlobalPool>(1).gameObject.Value;
            throw1.DisableActions(1);
            throw1.AddCustomAction(() => {
                pooThrowOffset = 1.5f * Vector2.right * (transform.localScale.x < 0 ? -1f : 1f);
                var dungBall = pooBallW.Spawn(transform.position + pooThrowOffset, Quaternion.identity);
                //var dungBall = EnemyRandomizerDatabase.GetDatabase().Spawn("Dung Ball Large", null);
                control.FsmVariables.GetFsmGameObject("Dung Ball").Value = dungBall;
                //dungBall.transform.position = transform.position;
                dungBall.SafeSetActive(true);
            });

            var rjInAir = control.GetState("RJ In Air");
            rjInAir.DisableActions(7);
            rjInAir.InsertCustomAction(() => StartCoroutine(CalculateAirDiveHeight()), 7);

            var airDive = control.GetState("Air Dive");
            airDive.DisableActions(8);
            airDive.AddCustomAction(() => StartCoroutine(CheckEndAirDive()));

            var pillar = control.GetState("Pillar");
            pillar.InsertCustomAction(() => {
                var floor = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 3.5f, Vector2.down, float.MaxValue);
                pillar.GetActions<SetPosition>().ToList().ForEach(x => x.y.Value = floor.point.y + pillarHeight);
            }, 0);

            var dolphDir = control.GetState("Dolph Dir");
            dolphDir.DisableAction(5);
            dolphDir.InsertCustomAction(() => {
                if (pos2d.x < heroPos2d.x)
                    control.SendEvent("FINISHED");
            }, 5);

            control.OverrideState( "Set Min X", () => { control.SendEvent("FINISHED"); });
            control.OverrideState( "Set Max X", () => { control.SendEvent("FINISHED"); });


            var rageq = control.GetState("Rage?");
            control.OverrideState( "Rage?", () => {
                if (control.FsmVariables.GetFsmBool("Raged").Value)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    if (CurrentHPf / MaxHPf < rageTriggerRatio)
                    {
                        control.FsmVariables.GetFsmBool("Raged").Value = true;
                        control.SendEvent("RAGE");
                    }
                    else
                    {
                        control.SendEvent("FINISHED");
                    }
                }
            });

            var groundslamq = control.GetState("Ground Slam?");
            control.OverrideState( "Ground Slam?", () => {

                var left = SpawnerExtensions.GetRayOn(pos2d + Vector2.up, Vector2.left, float.MaxValue);
                var right = SpawnerExtensions.GetRayOn(pos2d + Vector2.up, Vector2.right, float.MaxValue);

                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if (gameObject.DistanceToPlayer() > tooFarToSlamDist)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    bool tooCloseToRight = right.distance < tooCloseToWallDist;
                    bool tooCloseToLeft = left.distance < tooCloseToWallDist;

                    if (tooCloseToLeft && isHeroLeft)
                    {
                        control.SendEvent("FINISHED");
                    }
                    else if (tooCloseToRight && !isHeroLeft)
                    {
                        control.SendEvent("FINISHED");
                    }
                    else
                    {
                        RNG rng = new RNG();
                        rng.Reset();

                        bool doSlam = rng.Randf() < .2f;
                        if (doSlam)
                        {
                            control.SendEvent("GROUND SLAM");
                        }
                        else
                        {
                            control.SendEvent("FINISHED");
                        }
                    }
                }
            });
        }

        protected virtual IEnumerator CheckEndAirDive()
        {
            //var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();

            var downRay = SpawnerExtensions.GetRayOn(gameObject, Vector2.down, float.MaxValue);

            float timeout = 4f;
            float endY = downRay.point.y;

            while (control.ActiveStateName == "Air Dive")
            {
                timeout -= Time.deltaTime;
                if (pos2d.y <= endY)
                {
                    control.SendEvent("LAND");
                    break;
                }

                if (timeout <= 0f)
                {
                    control.SendEvent("LAND");
                    break;
                }

                var platRay = SpawnerExtensions.GetRayOn(gameObject, Vector2.down, 0.5f);
                if(platRay.collider != null)
                {
                    control.SendEvent("LAND");
                    break;
                }

                yield return new WaitForEndOfFrame();
            }

            //Destroy(poob);

            yield break;
        }

        protected virtual IEnumerator CalculateAirDiveHeight()
        {
            while (control.ActiveStateName == "RJ In Air")
            {
                var hitRay = SpawnerExtensions.GetRayOn(gameObject,Vector2.down,float.MaxValue);
                if (hitRay.collider != null && hitRay.distance > airDiveHeight)
                {
                    control.FsmVariables.GetFsmBool("Air Dive Height").Value = true;
                }
                else
                {
                    control.FsmVariables.GetFsmBool("Air Dive Height").Value = false;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual void CalculateAndSetupBuried()
        {
            Vector2 groundPos = transform.position;
            Vector2 buriedPos = groundPos - new Vector2(0f, buriedYOffset * transform.localScale.y);
            buriedOrigin = buriedPos;
            control.FsmVariables.GetFsmFloat("Buried Y").Value = buriedPos.y;
            transform.position = new Vector3(buriedPos.x, buriedPos.y, transform.position.z);
        }

        protected virtual void CalculateAndSetupErupt()
        {
            Vector2 eruptStart = pos2d + new Vector2(0f, eruptStartOffset * transform.localScale.y);
            eruptOrigin = eruptStart;
            control.FsmVariables.GetFsmFloat("Erupt Y").Value = eruptStart.y;
            Vector2 eruptMax = eruptStart + Vector2.up * peakEruptHeight;
            var upRay = SpawnerExtensions.GetRayOn(eruptStart, Vector2.up, peakEruptHeight + 1f);
            if (upRay.distance < peakEruptHeight)
            {
                eruptMax = upRay.point;
            }
            control.FsmVariables.GetFsmFloat("Erupt Peak Y").Value = eruptMax.y;
            transform.position = new Vector3(eruptStart.x, eruptStart.y, transform.position.z);
        }

        protected virtual IEnumerator StartAndCheckEruptPeak(string currentState, string endEvent)
        {
            float eruptMax = control.FsmVariables.GetFsmFloat("Erupt Peak Y").Value;

            while (control.ActiveStateName == currentState)
            {
                var hitRay = SpawnerExtensions.GetRoofX(gameObject);
                if (hitRay.collider != null || pos2d.y >= eruptMax || PhysicsBody.velocity.y < 0)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual IEnumerator StartAndCheckTunnelingState(string currentState, bool isMovingLeft)
        {
            while (control.ActiveStateName == currentState)
            {
                var left = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 3.5f, Vector2.left, float.MaxValue);
                var right = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 3.5f, Vector2.right, float.MaxValue);

                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if(left.distance < tunnelTooCloseToWall && isMovingLeft)
                {
                    control.SendEvent("RIGHT");
                    break;
                }
                else if(right.distance < tunnelTooCloseToWall && !isMovingLeft)
                {
                    control.SendEvent("LEFT");
                    break;
                }
                else if(gameObject.DistanceToPlayer() > tunnelTurnTowardPlayerDist)
                {
                    if ((isMovingLeft && !isHeroLeft) || (!isMovingLeft && isMovingLeft))
                    {
                        control.SendEvent("TURN");
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected override void SetCustomPositionOnSpawn()
        {            
            gameObject.StickToGroundX(.3f);
            UpdateBurrowEffect();
        }

        protected virtual void PositionBoss(float extraOffset)
        {
            gameObject.StickToGroundX(extraOffset);
            UpdateBurrowEffect();
        }

        protected virtual void UpdateBurrowEffect(float extraOffsetScale = -.5f)
        {
            //Vector2 groundPos = transform.position;
            //Vector2 buriedPos = groundPos + new Vector2(0f, buriedYOffset * transform.localScale.y * extraOffsetScale);
            burrowEffectClone.StickToGroundX(extraOffsetScale);
            //burrowEffectClone.FsmVariables.GetFsmFloat("Ground Y").Value = buriedPos.y;
        }
    }

    public class WhiteDefenderSpawner : DefaultSpawner<WhiteDefenderControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class WhiteDefenderPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




















    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HiveKnightControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        int p1HP;
        int p2HP => (int)(p1HP * .4f);
        int p3HP => (int)(p2HP * .6f);

        public override void Setup(GameObject other)
        {
            Dev.Log($"Begin Setup for {gameObject} with {other}");
            base.Setup(other);
            Dev.Log($"Enter Hive Knight Setup for {gameObject} with {other}");

            SetupCorpse();
            SetupPhase1HP();
            SetupRoar();
            SetupL();
            SetupR();

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
        }

        private void SetupPhase1HP()
        {
            if (p1HP <= 0)
                p1HP = CurrentHP;
        }

        private void SetupCorpse()
        {
            var c = gameObject.GetCorpseObject();
            if (c != null)
            {
                c.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }
        }

        private void SetupR()
        {
            var aimR = control.GetState("Aim R");
            aimR.DisableAction(3);
            aimR.AddCustomAction(() =>
            {
                if (SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 10f).distance < 8f)
                    control.SendEvent("L");
                else
                    control.SendEvent("FINISHED");
            });
        }

        private void SetupL()
        {
            var aimL = control.GetState("Aim L");
            aimL.DisableAction(3);
            aimL.AddCustomAction(() =>
            {
                if (SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 10f).distance < 8f)
                    control.SendEvent("R");
                else
                    control.SendEvent("FINISHED");
            });
        }

        protected virtual void SetupRoar()
        {
            Dev.Log($"control = {control}");
            Dev.Log($"fsm = {control.FsmStates}");
            Dev.Log($"len = {control.FsmStates.Length}");
            Dev.Log($"ror = {control.GetState("Roar")}");

            control.FsmStates.ToList().ForEach(x => Dev.Log($"HIVE KNIGHT STATE {x.Name}"));
            FsmState roar = control.GetState("Roar");
            AddedBeeToRoar(roar);
            AddedBeeToRoar(roar);
            AddedBeeToRoar(roar);
            AddedBeeToRoar(roar);
            roar.AddCustomAction(() => { control.SendEvent("FINISHED"); });
        }

        protected virtual void AddedBeeToRoar(FsmState roar)
        {
            roar.AddAction(new Wait() { time = 0.25f });
            roar.AddCustomAction(() => {
                var bee = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position + Vector3.up, "Bee Dropper", null, true);
                var beeBody = bee.GetComponent<Rigidbody2D>();
                if (beeBody != null)
                {
                    beeBody.velocity = UnityEngine.Random.insideUnitCircle * 5f;
                    if (beeBody.velocity.y < 0) { beeBody.velocity = new Vector2(beeBody.velocity.x, -beeBody.velocity.y); }
                }
            });
        }


        public override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            var result = base.GetStartingMaxHP(objectThatWillBeReplaced);
            p1HP = CurrentHP;
            return result;
        }
    }

    public class HiveKnightSpawner : DefaultSpawner<HiveKnightControl> { }

    public class HiveKnightPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrimmBossControl : DefaultSpawnedEnemyControl
    {
        public override bool explodeOnDeath => true;

        public override string FSMName => "Control";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var cx = gameObject.LocateMyFSM("constrain_x");
            if (cx != null)
                Destroy(cx);
            var cy = gameObject.LocateMyFSM("Constrain Y");
            if (cy != null)
                Destroy(cy);

            //control.ChangeTransition("Init", "FINISHED", "GG Bow");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Explode", "FINISHED", "Send Death Event");

            var balloonPos = control.GetState("Balloon Pos");
            balloonPos.DisableAction(0);
            balloonPos.InsertCustomAction(() =>
            {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(1f, 30f, 2f);
                transform.position = pos;
            }, 0);


            var moveChoice = control.GetState("Move Choice");
            moveChoice.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Ground Y").Value = SpawnerExtensions.GetGroundRay(HeroController.instance.gameObject).point.y;
                control.FsmVariables.GetFsmFloat("AD Min X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("AD Max X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
                control.FsmVariables.GetFsmFloat("AD Mid X").Value = heroPosWithOffset.x;
                control.FsmVariables.GetFsmFloat("Min X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("Max X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
            }, 0);


            var adFire = control.GetState("AD Fire");
            control.AddTimeoutAction(adFire, "LAND", 1f);

            var adEdge = control.GetState("AD Edge");
            control.AddTimeoutAction(adFire, "LAND", 1f);

            this.InsertHiddenState(control, "Init", "FINISHED", "GG Bow");

            var ggbow = control.GetState("GG Bow");
            ggbow.ChangeTransition("FINISHED", "Tele Out");
            ggbow.ChangeTransition("TOOK DAMAGE", "Tele Out");
        }
    }

    public class GrimmBossSpawner : DefaultSpawner<GrimmBossControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class GrimmBossPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class NightmareGrimmBossControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override bool preventOutOfBoundsAfterPositioning => true;

        protected virtual void SetupJunk()
        {
            var cx = gameObject.LocateMyFSM("constrain_x");
            if (cx != null)
                Destroy(cx);
            var cy = gameObject.LocateMyFSM("Constrain Y");
            if (cy != null)
                Destroy(cy);
        }

        protected virtual void ChangeTransitions()
        {
            //control.ChangeTransition("Set Balloon HP", "FINISHED", "Tele Out");
            InsertHiddenState(control, "Set Balloon HP", "FINISHED", "Tele Out");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Start", "FINISHED", "Death Explode");
            control.AddCustomAction("Dormant", () => { control.SendEvent("WAKE"); });
        }

        protected virtual void FixDeath()
        {
            var deathStart = control.GetState("Death Start");
            deathStart.DisableActions(6, 7, 20);

            var deathExplode = control.GetState("Death Start");
            deathExplode.DisableActions(2, 4);

            control.OverrideState("Send NPC Event", () => { Destroy(gameObject); });
        }

        protected virtual void FixMoves()
        {
            var balloonPos = control.GetState("Balloon Pos");
            balloonPos.DisableAction(0);
            balloonPos.InsertCustomAction(() =>
            {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(1f, 30f, 2f);
                transform.position = pos;
            }, 0);


            var moveChoice = control.GetState("Move Choice");
            moveChoice.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmFloat("Ground Y").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.down, 20f).point.y;
                control.FsmVariables.GetFsmFloat("AD Min X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("AD Max X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
                control.FsmVariables.GetFsmFloat("AD Mid X").Value = heroPosWithOffset.x;
                control.FsmVariables.GetFsmFloat("Min X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("Max X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
            }, 0);


            var adFire = control.GetState("AD Fire");
            control.AddTimeoutAction(adFire, "LAND", 1f);

            var adEdge = control.GetState("AD Edge");
            control.AddTimeoutAction(adEdge, "LAND", 1f);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            try
            {
                SetupJunk();

                ChangeTransitions();

                FixDeath();

                FixMoves();
            }
            catch(Exception e)
            {
                Dev.LogError($"Error setting up NKG's FSM: MESSAGE:{e.Message} STACKTRACE:{ e.StackTrace}");
            }
        }
    }

    public class NightmareGrimmBossSpawner : DefaultSpawner<NightmareGrimmBossControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class NightmareGrimmBossPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HollowKnightBossControl : DefaultSpawnedEnemyControl
    {
        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if(GameManager.instance.GetCurrentMapZone() == "FINAL_BOSS")
            {
                //do nothing
                return;
            }

            var phaseControl = gameObject.LocateMyFSM("Phase Control");
            if (phaseControl != null)
                Destroy(phaseControl);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            EnemyHealthManager.hasSpecialDeath = false;

            control.AddTimeoutAction(control.GetState("Dstab Air"), "LAND", 1f);

            //TEMP
            control.OverrideState( "Long Roar End", () => Destroy(gameObject));

            var movetest = control.GetState("Move test");
            movetest.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("PuppetSlam Y").Value = SpawnerExtensions.GetGroundRay(HeroController.instance.gameObject).point.y;
                control.FsmVariables.GetFsmFloat("Left X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("Right X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
                control.FsmVariables.GetFsmFloat("TeleRange Min").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 20f).point.x;
                control.FsmVariables.GetFsmFloat("TeleRange Max").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 20f).point.x;
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Init Idle");
        }
    }

    public class HollowKnightBossSpawner : DefaultSpawner<HollowKnightBossControl> { }

    public class HollowKnightBossPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HKPrimeControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            control.GetState("Intro 1").GetFirstActionOfType<Wait>().time = 0.25f;
            control.GetState("Intro 2").GetFirstActionOfType<Wait>().time = 0.25f;
            control.GetState("Intro 3").GetFirstActionOfType<Wait>().time = 0.25f;

            var intro2 = control.GetState("Intro 2");
            intro2.DisableAction(6);

            var intro3 = control.GetState("Intro 3");
            intro3.DisableAction(2);
            intro3.DisableAction(7);

            var intro4 = control.GetState("Intro 4");
            intro4.DisableAction(5);

            var intro5 = control.GetState("Intro 5");
            intro5.DisableAction(2);

            control.ChangeTransition("Intro 6", "FINISHED", "Intro Roar End");

            this.InsertHiddenState(control, "Pause", "FINISHED", "Init");

            var phaseq = control.GetState("Phase ?");
            control.OverrideState("Phase ?", () => {
                if (CurrentHPf > MaxHPf / 2)
                    control.SendEvent("PHASE1");
                else if (CurrentHP > MaxHPf / 4)
                    control.SendEvent("PHASE2");
                else //if (CurrentHP > thisMetadata.DefaultHP / 2)
                    control.SendEvent("PHASE3");
            });

            var telein = control.GetState("Tele In");
            telein.DisableAction(0);
            telein.InsertCustomAction(() => {
                float teleX = control.FsmVariables.GetFsmFloat("Tele X").Value - 40f;
                bool isPositive = teleX > 0f;
                teleX = Mathf.Abs(teleX);

                Vector2 telePos;

                if(isPositive)
                {
                    //go right
                    var teleXRayRight = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, teleX);

                    var groundNearTele = SpawnerExtensions.GetRayOn(teleXRayRight.point, Vector2.down, 50f);

                    telePos = new Vector2(teleXRayRight.point.x - 1f, groundNearTele.point.y) + Vector2.up;
                }
                else
                {
                    //go left
                    var teleXRayLeft = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, teleX);

                    var groundNearTele = SpawnerExtensions.GetRayOn(teleXRayLeft.point, Vector2.down, 50f);

                    telePos = new Vector2(teleXRayLeft.point.x + 1f, groundNearTele.point.y) + Vector2.up;
                }

                transform.position = telePos;
            }, 0);

            var idleStance = control.GetState("Idle Stance");
            idleStance.AddCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Plume Y").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.down, 50f).point.y;
                control.FsmVariables.GetFsmFloat("Right X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 50f).point.x;
                control.FsmVariables.GetFsmFloat("Left X").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 50f).point.x;
                control.FsmVariables.GetFsmFloat("TeleRange Max").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 100f).point.x - 4f;
                control.FsmVariables.GetFsmFloat("TeleRange Min").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 100f).point.x + 4f;
                control.FsmVariables.GetFsmFloat("Stun Land Y").Value = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.down, 50f).point.y;
            });

            var stunAir = control.GetState("Stun Air");
           control.AddTimeoutAction(stunAir, "LAND", 2f);

            var dstabAir = control.GetState("Dstab Air");
           control.AddTimeoutAction(dstabAir, "LAND", 2f);

            var stompDown = control.GetState("Stomp Down");
           control.AddTimeoutAction(stompDown, "LAND", 2f);

            var inAir = control.GetState("In Air");
           control.AddTimeoutAction(inAir, "LAND", 2f);
        }
    }

    public class HKPrimeSpawner : DefaultSpawner<HKPrimeControl> { }

    public class HKPrimePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PaleLurkerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Lurker Control";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var hpscaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpscaler != null)
                Destroy(hpscaler);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            //TODO: make lurker balls killable

            this.InsertHiddenState(control, "Init", "FINISHED", "Get High");
        }
    }

    public class PaleLurkerSpawner : DefaultSpawner<PaleLurkerControl> { }

    public class PaleLurkerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class OroControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "nailmaster";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            control.ChangeTransition("Death Start", "FINISHED", "Explode");

            var explode = control.GetState("Explode");
            explode.InsertCustomAction(() => {
                if (EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
        }
    }

    public class OroSpawner : DefaultSpawner<OroControl> { }

    public class OroPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MatoControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "nailmaster";


        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");

            control.ChangeTransition("Death Start", "FINISHED", "Explode");

            var explode = control.GetState("Explode");
            explode.InsertCustomAction(() => {
                if (EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);
        }
    }

    public class MatoSpawner : DefaultSpawner<MatoControl> { }

    public class MatoPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SheoBossControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "nailmaster_sheo";

        public override bool useCustomPositonOnSpawn => false;

        public override float spawnPositionOffset => 0.5f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            this.InsertHiddenState(control, "Set Paint HP", "FINISHED", "Idle");
        }
    }

    public class SheoBossSpawner : DefaultSpawner<SheoBossControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class SheoBossPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlyBossControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";


        public override void Setup(GameObject other)
        {
            base.Setup(other);            
            this.InsertHiddenState(control, "Phase HP", "FINISHED", "Idle");
            control.ChangeTransition("Death Reset", "FINISHED", "Explosion");
        }
    }

    public class SlyBossSpawner : DefaultSpawner<SlyBossControl> 
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class SlyBossPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetNoskControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Hornet Nosk";

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                if (EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
                else
                {
                    control.FsmVariables.GetFsmFloat("X Min").Value = SpawnerExtensions.GetRayOn(pos2d, Vector2.left, 50f).point.x;
                    control.FsmVariables.GetFsmFloat("X Max").Value = SpawnerExtensions.GetRayOn(pos2d, Vector2.right, 50f).point.x;
                    control.FsmVariables.GetFsmFloat("Y Min").Value = SpawnerExtensions.GetRayOn(pos2d, Vector2.down, 50f).point.y;
                    control.FsmVariables.GetFsmFloat("Y Max").Value = SpawnerExtensions.GetRayOn(pos2d, Vector2.up, 50f).point.y;
                    control.FsmVariables.GetFsmFloat("Swoop Height").Value = SpawnerExtensions.GetRayOn(pos2d, Vector2.down, 50f).point.y + 2f;
                }
            }, 0);


            var swoopRise = control.GetState("Swoop Rise");
            control.AddTimeoutAction(swoopRise, "FINISHED", 0.5f);

            var chooseAttack2 = control.GetState("Choose Attack 2");
            chooseAttack2.ChangeTransition("ROOF", "Shift Down?");

            var shiftDownq = control.GetState("Shift Down?");
            shiftDownq.DisableAction(1);
            shiftDownq.DisableAction(2);
            shiftDownq.AddCustomAction(() => {
                float toRoof = Mathf.Abs(roofY - pos2d.y);
                if (toRoof <= 2f)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    PhysicsBody.velocity = new Vector2(0, -20f);
                    control.SendEvent("FINISHED");
                }                
            });

            var hp = control.GetState("HP");
            control.OverrideState("HP", () => {
                control.FsmVariables.GetFsmInt("HP").Value = MaxHP;
                control.FsmVariables.GetFsmInt("Half HP").Value = (MaxHP / 2) > 0 ? (MaxHP / 2) : 1;
            });

            this.InsertHiddenState(control, "HP", "FINISHED", "Idle");
        }
    }

    public class HornetNoskSpawner : DefaultSpawner<HornetNoskControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class HornetNoskPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DungDefenderControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Dung Defender";
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;

        public float peakEruptHeight = 12f;
        public float eruptStartOffset = 3f;
        public float defaultHP;
        public float defaultRageHP = 600f;
        public float rageTriggerRatio;
        public float tooFarToSlamDist = 7f;
        public float tooCloseToWallDist = 5f;
        public float buriedYOffset = 2.5f;
        public float tunnelTooCloseToWall = 2f;
        public float tunnelTurnTowardPlayerDist = 10f;
        public float airDiveHeight = 15.4f;
        public float pillarHeight = 10.68f;
        public Vector2 buriedOrigin;
        public Vector2 eruptOrigin;
        protected PlayMakerFSM burrowEffect;
        protected PlayMakerFSM keepY;
        protected Vector3 pooThrowOffset;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            defaultHP = gameObject.OriginalPrefabHP();
            rageTriggerRatio = defaultHP / defaultRageHP;

            burrowEffect = gameObject.GetDirectChildren().FirstOrDefault(x => x.name.Contains("Burrow Effect")).LocateMyFSM("Burrow Effect");
            //keepY = gameObject.GetDirectChildren().FirstOrDefault(x => x.name.Contains("Burrow Effect")).LocateMyFSM("Keep Y");

            keepY = gameObject.GetDirectChildren().FirstOrDefault(x => x.name.Contains("Burrow Effect")).LocateMyFSM("Keep Y");
            if (keepY != null)
            {
                Destroy(keepY);
            }
            //var keepy_init = keepY.GetState("Init");
            //keepy_init.DisableAction(3);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.DisableActions(6);
            wake.InsertCustomAction(() => {
                PositionBoss(.3f);
                burrowEffect.gameObject.StickToGroundX(0f);
                //burrowEffect.transform.position -= new Vector3(0f, 1f * SizeScale, 0f);
            }, 2);

            var quakedOut = control.GetState("Quaked Out");
            quakedOut.DisableActions(1, 4, 9, 11);
            quakedOut.InsertCustomAction(() => CalculateAndSetupErupt(), 1);

            var rageq = control.GetState("Rage?");
            control.OverrideState( "Rage?", () => {
                if (control.FsmVariables.GetFsmBool("Raged").Value)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    if (CurrentHPf / MaxHPf < rageTriggerRatio)
                    {
                        control.FsmVariables.GetFsmBool("Raged").Value = true;
                        control.SendEvent("RAGE");
                    }
                    else
                    {
                        control.SendEvent("FINISHED");
                    }
                }
            });

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableActions(3, 4, 5);

            var rageRoar = control.GetState("Rage Roar");
            rageRoar.DisableActions(2, 3);

            var diveIn2 = control.GetState("Dive In 2");
            diveIn2.DisableActions(3);

            var underground = control.GetState("Underground");
            underground.DisableActions(0);
            underground.InsertCustomAction(() => CalculateAndSetupBuried(), 0);

            var rageIn = control.GetState("Rage In");
            rageIn.DisableActions(2, 6);

            var tunnelingL = control.GetState("Tunneling L");
            tunnelingL.DisableActions(3, 4, 7, 8, 9);
            tunnelingL.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling L", true)));

            var tunnelingR = control.GetState("Tunneling R");
            tunnelingR.DisableActions(0, 4, 7, 8, 9);
            tunnelingR.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling R", false)));

            var eruptAntic = control.GetState("Erupt Antic");
            eruptAntic.DisableActions(3);

            var eruptAnticR = control.GetState("Erupt Antic R");
            eruptAnticR.DisableActions(2);

            var eruptOutFirst = control.GetState("Erupt Out First");
            eruptOutFirst.DisableActions(1, 3, 6, 11);
            eruptOutFirst.InsertCustomAction(() => CalculateAndSetupErupt(), 3);
            eruptOutFirst.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First", "END")); });

            var eruptOut = control.GetState("Erupt Out");
            eruptOut.DisableActions(2, 5, 9, 14);
            eruptOut.InsertCustomAction(() => CalculateAndSetupErupt(), 5);
            eruptOut.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out", "END")); });

            var eruptFall = control.GetState("Erupt Fall");
            eruptFall.DisableActions(0);
            control.AddTimeoutAction(eruptFall, "LAND", 2f);
            //eruptFall.AddCustomAction(() => { StartCoroutine(TimeoutState("Erupt Fall", "LAND", 2f)); });

            var eruptLand = control.GetState("Erupt Land");
            eruptLand.DisableActions(4);

            var roarq = control.GetState("Roar?");
            roarq.DisableActions(4, 5, 11, 12, 13, 14, 15);

            var throw1 = control.GetState("Throw 1");
            throw1.DisableActions(1);
            throw1.AddCustomAction(() => {
                pooThrowOffset = pos2d + 1.5f * Vector2.right * (transform.localScale.x < 0 ? -1f : 1f);
                var dungBall = SpawnerExtensions.SpawnEntityAt("Dung Ball Large", pooThrowOffset, null, false);
                dungBall.ScaleObject(SizeScale);
                dungBall.ScaleAudio(SizeScale);
                control.FsmVariables.GetFsmGameObject("Dung Ball").Value = dungBall;
                dungBall.SafeSetActive(true);
            });

            var dolphDir = control.GetState("Dolph Dir");
            dolphDir.DisableAction(5);
            dolphDir.InsertCustomAction(() => {
                if (pos2d.x < heroPos2d.x)
                    control.SendEvent("FINISHED");
            }, 5);

            control.OverrideState( "Set Min X", () => { control.SendEvent("FINISHED"); });
            control.OverrideState( "Set Max X", () => { control.SendEvent("FINISHED"); });
        }

        protected virtual IEnumerator CheckEndAirDive()
        {
            //var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();

            var downRay = SpawnerExtensions.GetRayOn(gameObject, Vector2.down, float.MaxValue);

            float timeout = 4f;
            float endY = downRay.point.y;

            while (control.ActiveStateName == "Air Dive")
            {
                timeout -= Time.deltaTime;
                if (pos2d.y <= endY)
                {
                    control.SendEvent("LAND");
                    break;
                }

                if (timeout <= 0f)
                {
                    control.SendEvent("LAND");
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            //Destroy(poob);

            yield break;
        }

        protected virtual IEnumerator CalculateAirDiveHeight()
        {
            while (control.ActiveStateName == "RJ In Air")
            {
                var hitRay = SpawnerExtensions.GetRayOn(gameObject, Vector2.down, float.MaxValue);
                if (hitRay.collider != null && hitRay.distance > airDiveHeight)
                {
                    control.FsmVariables.GetFsmBool("Air Dive Height").Value = true;
                }
                else
                {
                    control.FsmVariables.GetFsmBool("Air Dive Height").Value = false;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual void CalculateAndSetupBuried()
        {
            Vector2 groundPos = transform.position;
            Vector2 buriedPos = groundPos - new Vector2(0f, buriedYOffset * transform.localScale.y);
            buriedOrigin = buriedPos;
            control.FsmVariables.GetFsmFloat("Buried Y").Value = buriedPos.y;
            transform.position = new Vector3(buriedPos.x, buriedPos.y, transform.position.z);
        }

        protected virtual void CalculateAndSetupErupt()
        {
            Vector2 eruptStart = pos2d + new Vector2(0f, eruptStartOffset * transform.localScale.y);
            eruptOrigin = eruptStart;
            control.FsmVariables.GetFsmFloat("Erupt Y").Value = eruptStart.y;
            Vector2 eruptMax = eruptStart + Vector2.up * peakEruptHeight;
            var upRay = SpawnerExtensions.GetRayOn(eruptStart, Vector2.up, peakEruptHeight + 1f);
            if (upRay.distance < peakEruptHeight)
            {
                eruptMax = upRay.point;
            }
            control.FsmVariables.GetFsmFloat("Erupt Peak Y").Value = eruptMax.y;
            transform.position = new Vector3(eruptStart.x, eruptStart.y, transform.position.z);
        }

        protected virtual IEnumerator StartAndCheckEruptPeak(string currentState, string endEvent)
        {
            float eruptMax = control.FsmVariables.GetFsmFloat("Erupt Peak Y").Value;

            while (control.ActiveStateName == currentState)
            {
                var hitRay = SpawnerExtensions.GetRoofX(gameObject);
                if (hitRay.collider != null || pos2d.y >= eruptMax || PhysicsBody.velocity.y < 0)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual IEnumerator StartAndCheckTunnelingState(string currentState, bool isMovingLeft)
        {
            while (control.ActiveStateName == currentState)
            {
                var left = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 3.5f, Vector2.left, float.MaxValue);
                var right = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 3.5f, Vector2.right, float.MaxValue);

                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if (left.distance < tunnelTooCloseToWall && isMovingLeft)
                {
                    control.SendEvent("RIGHT");
                    break;
                }
                else if (right.distance < tunnelTooCloseToWall && !isMovingLeft)
                {
                    control.SendEvent("LEFT");
                    break;
                }
                else if (gameObject.DistanceToPlayer() > tunnelTurnTowardPlayerDist)
                {
                    if ((isMovingLeft && !isHeroLeft) || (!isMovingLeft && isMovingLeft))
                    {
                        control.SendEvent("TURN");
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }


        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.StickToGroundX(.3f);
            UpdateBurrowEffect();
        }

        protected virtual void PositionBoss(float extraOffset)
        {
            gameObject.StickToGroundX(extraOffset);
            UpdateBurrowEffect();
        }

        protected virtual void UpdateBurrowEffect(float extraOffsetScale = -1f)
        {
            Vector2 groundPos = transform.position;
            Vector2 buriedPos = groundPos - new Vector2(0f, buriedYOffset * transform.localScale.y * extraOffsetScale);
            burrowEffect.FsmVariables.GetFsmFloat("Ground Y").Value = buriedPos.y;
        }

        public float customTransformMinAliveTime = 1f;
        public float customTransformAggroRange = 20f;

        protected override void CheckControlInCustomHiddenState()
        {
            if (customTransformMinAliveTime > 0)
            {
                customTransformMinAliveTime -= Time.deltaTime;
                return;
            }

            if (gameObject.CanSeePlayer() && gameObject.DistanceToPlayer() < customTransformAggroRange)
            {
                base.CheckControlInCustomHiddenState();
            }
        }
    }

    public class DungDefenderSpawner : DefaultSpawner<DungDefenderControl>
    {
        public override bool corpseRemovedByEffect => true;
    }

    public class DungDefenderPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorGalienControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        public virtual void OnEnable()
        {
            var hammer = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Hammer", null, true);
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }

            if(hammer != null)
            {
                hammer.ScaleObject(SizeScale);
                hammer.ScaleAudio(SizeScale);
            }

            hammer.SafeSetActive(true);

            chasing = SpawnerExtensions.DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 0.3f, 7f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            {
                var corpse = gameObject.GetCorpseObject();
                if(corpse != null)
                {
                    var fsm = corpse.LocateMyFSM("Control");
                    if (fsm != null)
                    {
                        var music = fsm.GetState("Music");
                        if(music != null)
                        {
                            try
                            {
                                music.DisableActions(0, 1, 2);
                            }
                            catch (Exception) { }//don't care
                        }
                    }
                }
            }

            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }

            //slightly change the ghost movement, no more teleports for now and also chase the player 
            //{
            //    var attack = control.GetState("Warp In");
            //    attack.InsertCustomAction(() => {

            //        var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            //        EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            //    }, 0);

            //    control.OverrideState("Hover", () => { 
            //    });
            //    control.GetState("Hover").AddAction(new Wait() { time = 10f });
            //}

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");

            var summon = gameObject.LocateMyFSM("Summon Minis");
            var summona1 = summon.GetState("Summon Antic");
            summona1.InsertCustomAction(() => {
                var hammer = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
                if (hammer != null)
                {
                    hammer.ScaleObject(SizeScale);
                    hammer.ScaleAudio(SizeScale);
                }
            },0);
            summon.AddTimeoutAction(summona1, "SUMMON", 1f);

            var summona2 = summon.GetState("Summon Antic 2");
            summona2.InsertCustomAction(() => {
                var hammer = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
                if (hammer != null)
                {
                    hammer.ScaleObject(SizeScale);
                    hammer.ScaleAudio(SizeScale);
                }
            }, 0);
            summon.AddTimeoutAction(summona2, "SUMMON", 1f);
        }

        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GhostWarriorGalienSpawner : DefaultSpawner<GhostWarriorGalienControl> { }

    public class GhostWarriorGalienPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorXeroControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        public GameObject sword1;
        public GameObject sword2;
        public GameObject sword3;
        public GameObject sword4;

        public virtual void OnEnable()
        {
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = gameObject.DistanceFlyChase(HeroController.instance.gameObject, 5f, .25f, 4f);
            StartCoroutine(chasing);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            sword1 = gameObject.SpawnEntity("Sword 1", false);
            sword2 = gameObject.SpawnEntity("Sword 2", false);
            sword3 = gameObject.SpawnEntity("Sword 3", false);
            sword4 = gameObject.SpawnEntity("Sword 4", false);

            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Y Limit");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }


            {
                //control.ChangeTransition("Warp In", "FINISHED", "Hover");
                //control.OverrideState("Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var attacking = gameObject.LocateMyFSM("Attacking");
                var init = attacking.GetState("Init");
                init.AddCustomAction(() => {
                    attacking.FsmVariables.GetFsmGameObject("Sword 1").Value = sword1;
                    attacking.FsmVariables.GetFsmGameObject("Sword 2").Value = sword2;
                    attacking.FsmVariables.GetFsmGameObject("Sword 3").Value = sword3;
                    attacking.FsmVariables.GetFsmGameObject("Sword 4").Value = sword4;
                });

                attacking.ChangeTransition("Wait", "FINISHED", "Antic");

                var a_init = attacking.GetState("Init");
                a_init.AddCustomAction(() => { attacking.SendEvent("READY"); });
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
    }

    public class GhostWarriorXeroSpawner : DefaultSpawner<GhostWarriorXeroControl> { }

    public class GhostWarriorXeroPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorHuControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";
        public virtual string ATTACKFSM => "Attacking";

        public GameObject ringHolder;

        public virtual void OnEnable()
        {
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = gameObject.DistanceFlyChase(HeroController.instance.gameObject, 5f, 0.3f, 8f, 2f);
            StartCoroutine(chasing);
        }

        protected virtual void ConfigureRingPositions()
        {
            foreach (var fsm in ringHolder.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                fsm.ChangeTransition("Init", "FINISHED", "Antic");
            }
        }

        protected virtual void SetRingPositions(Vector3 ringRootPos)
        {
            ringHolder.transform.position = ringRootPos;
            foreach (var fsm in ringHolder.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                var downRay = SpawnerExtensions.GetRayOn(fsm.transform.position, Vector2.down, 20f);
                var down = fsm.GetState("Down");
                var fc = down.GetFirstActionOfType<FloatCompare>();
                fc.float2 = downRay.point.y + .3f;

                var land = fsm.GetState("Land");
                var spos = land.GetFirstActionOfType<SetPosition>();
                spos.y = downRay.point.y;

                var reset = fsm.GetState("Reset");
                var spos2 = reset.GetFirstActionOfType<SetPosition>();
                spos2.y = ringRootPos.y;
            }
        }

        IEnumerator chasing;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            ringHolder = gameObject.SpawnEntity("Ring Holder", false);            
            ConfigureRingPositions();


            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                //control.ChangeTransition("Warp In", "FINISHED", "Hover");
                //control.OverrideState("Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var attacking = gameObject.LocateMyFSM("Attacking");
                var init = attacking.GetState("Init");
                init.AddCustomAction(() => {
                    attacking.FsmVariables.GetFsmGameObject("Ring Holder").Value = ringHolder;
                });

                var wait = attacking.GetState("Wait");
                wait.DisableActions(0, 1);
                wait.AddAction(new Wait() { time = 1.5f });
                wait.AddCustomAction(() => { attacking.SendEvent("FINISHED"); });

                var placeRings = attacking.GetState("Place Rings");
                placeRings.AddCustomAction(() => {
                    SetRingPositions(heroPos2d);
                });

                attacking.ChangeTransition("Choice 2", "MEGA", "Choice");

                var a_init = attacking.GetState("Init");
                a_init.AddCustomAction(() => { attacking.SendEvent("READY"); });
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GhostWarriorHuSpawner : DefaultSpawner<GhostWarriorHuControl> { }

    public class GhostWarriorHuPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorSlugControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        public virtual void OnEnable()
        {
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = gameObject.DistanceFlyChase(HeroController.instance.gameObject, 5f, 0.3f, 5f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Distance Attack");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                //control.ChangeTransition("Warp In", "FINISHED", "Hover");
                //control.OverrideState("Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var attacking = gameObject.LocateMyFSM("Attacking");
                attacking.ChangeTransition("Wait", "FINISHED", "Antic");
                var a_init = attacking.GetState("Init");
                a_init.AddCustomAction(() => { attacking.SendEvent("READY"); });

            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GhostWarriorSlugSpawner : DefaultSpawner<GhostWarriorSlugControl> { }

    public class GhostWarriorSlugPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorNoEyesControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;


        public virtual void OnEnable()
        {
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = gameObject.DistanceFlyChase(HeroController.instance.gameObject, 5f, 10f, 2f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }

            //slightly change the ghost movement, no more teleports for now and also chase the player 
            //{
            //    control.OverrideState("Hover", () => {
            //    });
            //    control.GetState("Hover").AddAction(new Wait() { time = 10f });
            //}

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GhostWarriorNoEyesSpawner : DefaultSpawner<GhostWarriorNoEyesControl> { }

    public class GhostWarriorNoEyesPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorMarkothControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        public virtual void OnEnable()
        {
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = gameObject.DistanceFlyChase(HeroController.instance.gameObject, 5f, 0.3f, 5f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            {
                var fsm = gameObject.LocateMyFSM("FSM");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                if (fsm != null)
                    Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("Movement");
                if (fsm != null)
                    Destroy(fsm);
            }

            //slightly change the ghost movement, no more teleports for now and also chase the player 
            {
                //control.ChangeTransition("Warp In", "FINISHED", "Hover");
                //control.OverrideState("Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var shieldAttack = gameObject.LocateMyFSM("Shield Attack");
                var init = shieldAttack.GetState("Init");
                init.AddCustomAction(() => {

                    var shield = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shield", null, false);
                    shield.transform.parent = transform;
                    shield.ScaleObject(SizeScale);
                    shield.SetActive(true);

                    shieldAttack.FsmVariables.GetFsmGameObject("Shield 1").Value = shield;
                });

                var attacking = gameObject.LocateMyFSM("Attacking");
                var nail = attacking.GetState("Nail");
                nail.AddCustomAction(() => {

                    var nailObject = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shot Markoth Nail", null, false);
                    nailObject.transform.parent = transform;
                    nailObject.ScaleObject(SizeScale);
                    nailObject.SetActive(true);
                });

                var a_init = attacking.GetState("Init");
                a_init.AddCustomAction(() => { attacking.SendEvent("READY"); });
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }


        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GhostWarriorMarkothSpawner : DefaultSpawner<GhostWarriorMarkothControl> { }

    public class GhostWarriorMarkothPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishGGControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mega Jellyfish";


        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            //{
            //    //{"Dolphin Max X" , x => edgeR},
            //    //{"Dolphin Min X" , x => edgeL},
            //    //{"Max X" , x => edgeR},
            //    //{"Min X" , x => edgeL},
            //    //{"Erupt Y" , x => floorY},
            //    //{"Buried Y" , x => floorY - 3f},
            //    ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
            //    ////{"Left Pos" , x => edgeL},
            //    ////{"Right Pos" , x => edgeR},
            //};

            this.InsertHiddenState(control, "Init", "FINISHED", "Start");
            //this.AddResetToStateOnHide(control, "Init");

            this.EnemyHealthManager.IsInvincible = false;

            control.GetState("Recover").DisableAction(2);
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class JellyfishGGSpawner : DefaultSpawner<JellyfishGGControl> { }

    public class JellyfishGGPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaJellyfishControl : JellyfishGGControl
    {
        public override string FSMName => "Mega Jellyfish";
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class MegaJellyfishSpawner : DefaultSpawner<MegaJellyfishControl> { }

    public class MegaJellyfishPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeMotherControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Fluke Mother";

        public AudioPlayerOneShotSingle squirtA;
        public AudioPlayerOneShotSingle squirtB;

        public AudioSource audio;

        public override bool useCustomPositonOnSpawn => true;

        public GameObject lastSpawnedObject;
        public GameObject lastSpawnedObject2;

        protected virtual void SetupChildrenStuff()
        {
            ChildController.maxChildren = 8;

            var spawn2 = control.GetState("Spawn 2");

            squirtA = spawn2.GetAction<AudioPlayerOneShotSingle>(9);
            squirtB = spawn2.GetAction<AudioPlayerOneShotSingle>(10);
            audio = GetComponent<AudioSource>();
        }

        protected virtual void SetupInitStates()
        {
            var init = control.GetState("Init");
            init.DisableAction(2);
            init.DisableAction(3);
            init.DisableAction(5);
            init.DisableAction(6);
            init.DisableAction(7);
            init.DisableAction(8);

            var gg = control.GetState("GG?");
            gg.RemoveTransition("GG BOSS");
            gg.AddCustomAction(() => { control.SendEvent("FINISHED"); });

            var idle = control.GetState("Idle");
            idle.DisableAction(4);

            var playIdle = control.GetState("Play Idle");
            playIdle.DisableAction(3);
        }

        protected virtual void SetupRoar()
        {
            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);
            roarStart.DisableAction(4);
            roarStart.DisableAction(7);
            roarStart.DisableAction(8);
            roarStart.DisableAction(9);
            roarStart.DisableAction(10);
            roarStart.DisableAction(11);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(0);
            roarEnd.DisableAction(1);
        }

        protected virtual void SetupRage()
        {
            var rage = control.GetState("Rage");
            rage.DisableAction(0);

            rage.AddAction(new Wait() { time = 3f });

            rage.AddCustomAction(() =>
            {
                if (ChildController.AtMaxChildren)
                {
                    control.SendEvent("SPAWN");
                }
                else
                {
                    audio.PlayOneShot(squirtA.audioClip.Value as AudioClip);
                    audio.PlayOneShot(squirtB.audioClip.Value as AudioClip);

                    var spawn = gameObject.GetRandomPositionInLOSofSelf(0f, 3f, 1f, false, false);
                    var spawn2 = gameObject.GetRandomPositionInLOSofSelf(0f, 3f, 1f, false, false);

                    lastSpawnedObject = SpawnChildForEnemySpawner(spawn, false, "Fluke Fly");
                    lastSpawnedObject2 = SpawnChildForEnemySpawner(spawn2, false, "Fluke Fly");

                    control.SendEvent("SPAWN");
                }
            });
        }

        protected virtual void SetupSpawn()
        {
            var selectPoint = control.GetState("Select Point");
            selectPoint.DisableActions(0, 1, 2);

            var selectPoint2 = control.GetState("Select Point 2");
            selectPoint2.DisableActions(0, 1, 2);

            var spawn = control.GetState("Spawn");
            control.OverrideState("Spawn", () => {

                if (!ChildController.AtMaxChildren)
                {
                    lastSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(lastSpawnedObject);
                    lastSpawnedObject2 = ChildController.ActivateAndTrackSpawnedObject(lastSpawnedObject2);
                }
                control.SendEvent("FINISHED");
            });
            spawn.ChangeTransition("FINISHED", "Rage");
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var corpse = gameObject.GetCorpseObject();
            if(corpse != null)
            {
                var cc = corpse.LocateMyFSM("Corpse Control");
                var init = cc.GetState("Init");
                init.DisableActions(5);
            }

            SetupChildrenStuff();

            SetupInitStates();

            SetupRoar();

            SetupRage();

            SetupSpawn();
        }


        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.StickToRoof(rotateToPlaceOnRoof: false);
        }
    }

    public class FlukeMotherSpawner : DefaultSpawner<FlukeMotherControl> { }

    public class FlukeMotherPrefabConfig : DefaultPrefabConfig
    {
        public static AudioClip squirtA;
        public static AudioClip squirtB;

        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            var control = p.prefab.LocateMyFSM("Fluke Mother");

            var spawn2 = control.GetState("Spawn 2");

            if (squirtA == null || squirtB == null)
            {
                squirtA = spawn2.GetAction<AudioPlayerOneShotSingle>(9).audioClip.Value as AudioClip;
                squirtB = spawn2.GetAction<AudioPlayerOneShotSingle>(10).audioClip.Value as AudioClip;
            }
        }
    }

    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss1Control : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        //values taken from hornet's FSM
        //for this rewrite I probably won't use them all, but since I collected the data
        //I'll keep it here anyway for now
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

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            var deactive = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
            if(deactive != null)
            {
                Destroy(deactive);
            }

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("WAKE");
            control.OverrideState( "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(4);
            refightReady.DisableAction(6);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");

            var refightWake = control.GetState("Refight Wake");
            refightWake.DisableAction(0);
            refightWake.DisableAction(1);
            refightWake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(2);
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);

            var escalation = control.GetState("Escalation");
            control.OverrideState( "Escalation", () =>
            {
                var esc = control.FsmVariables.GetFsmBool("Escalated");
                if(esc.Value)
                {
                    control.SendEvent("FINISHED");
                    return;
                }

                float hpPercent = CurrentHPf / MaxHPf;
                if(hpPercent <= escalationHPPercentage)
                {
                    control.FsmVariables.GetFsmFloat("Run Wait Min").Value = esRunWaitMin;
                    control.FsmVariables.GetFsmFloat("Run Wait Max").Value = esRunWaitMax;
                    control.FsmVariables.GetFsmFloat("Idle Wait Min").Value = esIdleWaitMin;
                    control.FsmVariables.GetFsmFloat("Idle Wait Max").Value = esIdleWaitMax;

                    esc.Value = true;
                }

                control.SendEvent("FINISHED");
            });

            var canThrowq = control.GetState("Can Throw?");
            control.OverrideState( "Can Throw?", () =>
            {
                var left = SpawnerExtensions.GetLeftRay(gameObject);
                var right = SpawnerExtensions.GetRightRay(gameObject);
                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if(isHeroLeft && left.distance < throwDistance)
                {
                    control.SendEvent("CAN THROW");
                }
                else if(!isHeroLeft && right.distance < throwDistance)
                {
                    control.SendEvent("CAN THROW");
                }
                else
                {
                    control.SendEvent("CANT THROW");
                }
            });

            var throws = control.GetState("Throw");
            throws.DisableAction(5);
            throws.InsertCustomAction(() => {

                var off = control.FsmVariables.GetFsmVector3("Self Pos").Value;
                off.y += -.5f * SizeScale;
            },5);

            var thrown = control.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });

            var aimJump = control.GetState("Aim Jump");
            control.OverrideState( "Aim Jump", () =>
            {
                bool isHeroLeft = heroPos2d.x < pos2d.x;

                RNG rng = new RNG();
                rng.Reset();

                if (isHeroLeft)
                {
                    float leftJumpX = rng.Rand(-2.5f, -0.5f);
                    control.FsmVariables.GetFsmFloat("Jump X").Value = leftJumpX;
                }
                else
                {
                    float rightJumpX = rng.Rand(0.5f, 2.5f);
                    control.FsmVariables.GetFsmFloat("Jump X").Value = rightJumpX;
                }

                control.SendEvent("FINISHED");
            });

            var inAir = control.GetState("In Air");
            inAir.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Sphere Y").Value = floorY + minAirSphereHeight;
            },0);

            var fire = control.GetState("Fire");
            fire.InsertCustomAction(() => {
                var setScale = fire.GetFirstActionOfType<SetScale>();
                setScale.x = SizeScale;
                setScale.y = SizeScale;
            },0);

            var firingr = control.GetState("Firing R");
            firingr.InsertCustomAction(() => {
                var sfv = firingr.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = -SizeScale;
            }, 0);

            var firingl = control.GetState("Firing L");
            firingl.InsertCustomAction(() => {
                var sfv = firingl.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = SizeScale;
            }, 0);

            var adash = control.GetState("A Dash");
            adash.DisableAction(2);
            adash.DisableAction(5);
            adash.DisableAction(6);
            adash.DisableAction(7);
            adash.DisableAction(8);
            adash.AddCustomAction(() => { StartCoroutine(CheckEndAirDash()); });

            var landy = control.GetState("Land Y");
            landy.DisableAction(0);
            landy.AddCustomAction(() => { SetCustomPositionOnSpawn(); });

            var hardland = control.GetState("Hard Land");
            hardland.InsertCustomAction(() => {
                var setScale = hardland.GetFirstActionOfType<SetScale>();
                setScale.y = SizeScale;
            }, 0);

            var hitRoof = control.GetState("Hit Roof");
            hitRoof.DisableAction(2);
            hitRoof.InsertCustomAction(() => {
                var setScale = hitRoof.GetFirstActionOfType<SetScale>();
                setScale.x = control.FsmVariables.GetFsmFloat("Return X Scale").Value;
                setScale.y = SizeScale;
            }, 0);

            var wallL = control.GetState("Wall L");
            wallL.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = SizeScale;
                setScale.y = SizeScale;
                StickToWall();
            }, 0);

            var wallR = control.GetState("Wall R");
            wallR.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = -SizeScale;
                setScale.y = SizeScale;
                StickToWall();
            }, 0);
        }

        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.StickToGround(1f);
        }

        protected virtual void StickToWall()
        {
            gameObject.StickToClosestSurfaceWithoutRotation(2f, .33f);
        }

        protected virtual IEnumerator ThrowAbortTimer()
        {
            yield return new WaitForSeconds(throwMaxTravelTime);
            if (control.ActiveStateName == "Thrown")
                control.SendEvent("NEEDLE RETURN");
            yield break;
        }

        protected virtual IEnumerator CheckEndAirDash()
        {
            float timeout = 5f;
            while(control.ActiveStateName == "A Dash")
            {
                timeout -= Time.deltaTime;
                var cardinalRays = SpawnerExtensions.GetCardinalRays(gameObject, 1f);
                var hitRay = cardinalRays.FirstOrDefault(x => x.collider != null);
                if(hitRay.collider != null)
                {
                    if(Mathf.Abs(hitRay.normal.x) > Mathf.Abs(hitRay.normal.y))
                    {

                        if (hitRay.normal.x > 0)
                            control.SendEvent("WALL L");
                        else// (hitRay.normal.x < 0)
                            control.SendEvent("WALL R");
                        break;
                    }
                    else
                    {
                        if (hitRay.normal.y > 0)
                            control.SendEvent("LAND");
                        else// (hitRay.normal.y < 0)
                            control.SendEvent("ROOF");
                        break;
                    }
                }
                if (timeout <= 0f)
                {
                    control.SendEvent("ROOF");
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class HornetBoss1Spawner : DefaultSpawner<HornetBoss1Control> { }

    public class HornetBoss1PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss2Control : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public float escalationHPPercentage = .7f;
        public float throwDistance = 12f;
        public float minAirSphereHeight = 5f;
        public float throwMaxTravelTime = .8f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            ChildController.spawnEntityOnChildDeath = "Gas Explosion Recycle L";

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                var cfsm = corpse.LocateMyFSM("Control");
                {
                    if(cfsm != null)
                    {
                        cfsm.GetState("Set PD").DisableActions(0);
                        cfsm.GetState("Blow").DisableActions(5);
                    }
                }

                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            var deactive = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
            if (deactive != null)
            {
                Destroy(deactive);
            }

            var badfsm = gameObject.LocateMyFSM("destroy_if_playerdatabool");
            if (badfsm != null)
            {
                Destroy(badfsm);
            }

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("BATTLE START");
            control.OverrideState( "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(4);
            refightReady.DisableAction(5);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");

            var refightWake = control.GetState("Refight Wake");
            refightWake.DisableAction(0);
            refightWake.DisableAction(1);
            refightWake.DisableAction(5);
            refightWake.DisableAction(7);
            refightWake.DisableAction(11);
            refightWake.DisableAction(12);
            refightWake.DisableAction(13);
            refightWake.DisableAction(14);
            refightWake.DisableAction(15);
            refightWake.DisableAction(16);
            refightWake.DisableAction(17);
            refightWake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);
            flourish.DisableAction(6);

            var escalation = control.GetState("Escalation");
            control.OverrideState( "Escalation", () =>
            {
                var esc = control.FsmVariables.GetFsmBool("Escalated");
                if (esc.Value)
                {
                    control.SendEvent("FINISHED");
                    return;
                }

                //if hornet gets scaled to a normal enemy with low hp, just enable the escalation right away
                float hpPercent = CurrentHPf / MaxHPf;
                if (hpPercent <= escalationHPPercentage || EnemyHealthManager.hp < 100)
                {
                    control.FsmVariables.GetFsmBool("Can Barb").Value = true;

                    esc.Value = true;
                }

                control.SendEvent("FINISHED");
            });

            var canThrowq = control.GetState("Can Throw?");
            control.OverrideState( "Can Throw?", () =>
            {
                var left = SpawnerExtensions.GetLeftRay(gameObject);
                var right = SpawnerExtensions.GetRightRay(gameObject);
                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if (isHeroLeft && left.distance < throwDistance)
                {
                    control.SendEvent("CAN THROW");
                }
                else if (!isHeroLeft && right.distance < throwDistance)
                {
                    control.SendEvent("CAN THROW");
                }
                else
                {
                    control.SendEvent("CANT THROW");
                }
            });

            var throws = control.GetState("Throw");
            throws.DisableAction(3);
            throws.InsertCustomAction(() => {

                var off = control.FsmVariables.GetFsmVector3("Self Pos").Value;
                off.y += -.5f * SizeScale;
            }, 5);

            var thrown = control.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });

            var aimJump = control.GetState("Aim Jump");
            control.OverrideState( "Aim Jump", () =>
            {
                bool isHeroLeft = heroPos2d.x < pos2d.x;

                RNG rng = new RNG();
                rng.Reset();

                if (isHeroLeft)
                {
                    float leftJumpX = rng.Rand(-2.5f, -0.5f);
                    control.FsmVariables.GetFsmFloat("Jump X").Value = leftJumpX;
                }
                else
                {
                    float rightJumpX = rng.Rand(0.5f, 2.5f);
                    control.FsmVariables.GetFsmFloat("Jump X").Value = rightJumpX;
                }

                control.SendEvent("FINISHED");
            });

            var inAir = control.GetState("In Air");
            inAir.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Sphere Y").Value = floorY + minAirSphereHeight;
            }, 0);

            var fire = control.GetState("Fire");
            fire.InsertCustomAction(() => {
                var setScale = fire.GetFirstActionOfType<SetScale>();
                setScale.x = SizeScale;
                setScale.y = SizeScale;
            }, 0);

            var firingr = control.GetState("Firing R");
            firingr.InsertCustomAction(() => {
                var sfv = firingr.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = -SizeScale;
            }, 0);

            var firingl = control.GetState("Firing L");
            firingl.InsertCustomAction(() => {
                var sfv = firingl.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = SizeScale;
            }, 0);

            var adash = control.GetState("A Dash");
            adash.DisableAction(1);
            adash.DisableAction(4);
            adash.DisableAction(5);
            adash.DisableAction(6);
            adash.DisableAction(7);
            adash.AddCustomAction(() => { StartCoroutine(CheckEndAirDash()); });

            var landy = control.GetState("Land Y");
            landy.DisableAction(0);
            landy.AddCustomAction(() => { SetCustomPositionOnSpawn(); });

            var hardland = control.GetState("Hard Land");
            hardland.InsertCustomAction(() => {
                var setScale = hardland.GetFirstActionOfType<SetScale>();
                setScale.y = SizeScale;
            }, 0);

            var hitRoof = control.GetState("Hit Roof");
            hitRoof.DisableAction(2);
            hitRoof.InsertCustomAction(() => {
                var setScale = hitRoof.GetFirstActionOfType<SetScale>();
                setScale.x = control.FsmVariables.GetFsmFloat("Return X Scale").Value;
                setScale.y = SizeScale;
            }, 0);

            var wallL = control.GetState("Wall L");
            wallL.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = SizeScale;
                setScale.y = SizeScale;
                StickToWall();
            }, 0);

            var wallR = control.GetState("Wall R");
            wallR.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = -SizeScale;
                setScale.y = SizeScale;
                StickToWall();
            }, 0);

            var barbq = control.GetState("Barb?");
            barbq.DisableAction(1);
            barbq.DisableAction(2);
            barbq.InsertCustomAction(() => {
                if (ChildController.AtMaxChildren)
                    control.SendEvent("FINISHED");
            },0);

            var barbthrow = control.GetState("Barb Throw");
            barbthrow.DisableAction(1);
            barbthrow.InsertCustomAction(() => {

                RNG rng = new RNG();
                rng.Reset();

                var toplayer = gameObject.DistanceToPlayer();
                toplayer = Mathf.Max(7f, toplayer);
                var spawnRange = rng.Rand(toplayer - 4f, toplayer + 4f);
                int tries = 0;
                RaycastHit2D spawnRay = default;
                for (tries = 0; tries < 10; ++tries)
                {
                    var randomDir = UnityEngine.Random.insideUnitCircle;
                    spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, spawnRange);
                    if (spawnRay.collider == null)
                        break;
                }

                var spawnPoint = spawnRay.point;

                var newBarb = gameObject.SpawnEntity("Hornet Barb");

                newBarb.ScaleObject(SizeScale);
                newBarb.transform.position = spawnPoint;
               ChildController.ActivateAndTrackSpawnedObject(newBarb);

                StartCoroutine(SendSpikeToBarbs());
            }, 0);
        }

        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.StickToGround(1f);
        }

        protected virtual void StickToWall()
        {
            gameObject.StickToClosestSurfaceWithoutRotation(2f, .33f);
        }

        protected virtual IEnumerator ThrowAbortTimer()
        {
            yield return new WaitForSeconds(throwMaxTravelTime);
            if (control.ActiveStateName == "Thrown")
                control.SendEvent("NEEDLE RETURN");
            yield break;
        }

        protected virtual IEnumerator SendSpikeToBarbs()
        {
            yield return new WaitForSeconds(1f);
            ChildController.GetChildrenControllers<HornetBarbControl>().ToList().ForEach(x => x.ActivateBarb());
            yield break;
        }

        protected virtual IEnumerator CheckEndAirDash()
        {
            float timeout = 5f;
            while (control.ActiveStateName == "A Dash")
            {
                timeout -= Time.deltaTime;
                var cardinalRays = SpawnerExtensions.GetCardinalRays(gameObject, 1f);
                var hitRay = cardinalRays.FirstOrDefault(x => x.collider != null);
                if (hitRay.collider != null)
                {
                    if (Mathf.Abs(hitRay.normal.x) > Mathf.Abs(hitRay.normal.y))
                    {

                        if (hitRay.normal.x > 0)
                            control.SendEvent("WALL L");
                        else// (hitRay.normal.x < 0)
                            control.SendEvent("WALL R");
                        break;
                    }
                    else
                    {
                        if (hitRay.normal.y > 0)
                            control.SendEvent("LAND");
                        else// (hitRay.normal.y < 0)
                            control.SendEvent("ROOF");
                        break;
                    }
                }
                if(timeout <= 0f)
                {
                    control.SendEvent("ROOF");
                    break;
                }    
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class HornetBoss2Spawner : DefaultSpawner<HornetBoss2Control> { }

    public class HornetBoss2PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaZombieBeamMinerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Beam Miner";

        protected override bool DisableCameraLocks => true;

        public override bool preventOutOfBoundsAfterPositioning => true;


        protected Tk2dPlayAnimation sleepAnim;

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            var tinker = gameObject.GetComponentInChildren<TinkEffect>(true);
            if (tinker != null)
            {
                GameObject.Destroy(tinker);
            }

            var land = control.GetState("Land");
            land.DisableAction(2);

            var sleep = control.GetState("Sleep");
            sleepAnim = sleep.GetFirstActionOfType<Tk2dPlayAnimation>();

            var deparents = control.GetState("Deparents");
            deparents.AddAction(sleepAnim);

            this.InsertHiddenState(control, "Deparents", "FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.DisableAction(2);
            wake.DisableAction(3);
            wake.ChangeTransition("FINISHED", "Idle");

            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                EnemyHealthManager.IsInvincible = false;
            },0);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound

            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(1);
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);
        }
    }

    public class MegaZombieBeamMinerSpawner : DefaultSpawner<MegaZombieBeamMinerControl> { }

    public class MegaZombieBeamMinerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerRematchControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Beam Miner";
        protected override bool DisableCameraLocks => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            var tinker = gameObject.GetComponentInChildren<TinkEffect>(true);
            if(tinker != null)
            {
                GameObject.Destroy(tinker);
            }


            var land = control.GetState("Land");
            land.DisableAction(2);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");
            wake.DisableAction(1);
            wake.DisableAction(2);
            wake.DisableAction(3);

            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                EnemyHealthManager.IsInvincible = false;
            }, 0);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound
            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(1);
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);

            if (other != null && !other.IsBoss())
                roarEnd.GetFirstActionOfType<SetDamageHeroAmount>().damageDealt = 1;
        }
    }

    public class ZombieBeamMinerRematchSpawner : DefaultSpawner<ZombieBeamMinerRematchControl> { }

    public class ZombieBeamMinerRematchPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordControl : MageLordControl { }

    public class DreamMageLordSpawner : DefaultSpawner<DreamMageLordControl> { }

    public class DreamMageLordPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordPhase2Control : MageLordPhase2Control { }

    public class DreamMageLordPhase2Spawner : DefaultSpawner<DreamMageLordPhase2Control> { }

    public class DreamMageLordPhase2PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///














    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaMossChargerControl : InGroundEnemyControl
    {
        public override string FSMName => "Mossy Control";

        protected override string FSMHiddenStateName => "MOSS_HIDDEN";

        public override float spawnPositionOffset => 1f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var initState = control.GetState("Init");
            initState.DisableAction(28);

            initState.RemoveTransition("GG BOSS");
            initState.RemoveTransition("SUBMERGE");
            initState.DisableAction(27);
            initState.DisableAction(26);
            initState.DisableAction(25);
            initState.DisableAction(24);
            initState.DisableAction(23);
            initState.DisableAction(22);
            initState.DisableAction(21);


            //just skip this state
            var hs = control.GetState("Hidden");
            hs.DisableAction(1);
            hs.DisableAction(2);
            hs.AddCustomAction(() =>
            {
                control.SendEvent("IN RANGE");
            });

            var heroBeyondq = control.GetState("Hero Beyond?");
            control.OverrideState("Hero Beyond?", () =>
            {
                if (HeroInAggroRange())
                    control.SendEvent("FINISHED");
                else
                    control.SendEvent("CANCEL");
            });

            control.OverrideState("Emerge Right", () => {
                if (heroPos2d.x > floorCenter)
                {
                    control.SendEvent("LEFT");
                    return;
                }
                control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(-6.5f, -0.5f);
                control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(-1f, 0f);
                control.FsmVariables.GetFsmFloat("Current Charge Speed").Value = -15f;
                control.FsmVariables.GetFsmFloat("Emerge Speed").Value = -4f;
                emergePoint = new Vector3(floorsize * .25f + floorCenter, floorSpawn.point.y, 0f);
                gameObject.SetXScaleSign(true);
                control.SendEvent("FINISHED");
            });

            control.OverrideState("Emerge Left", () => {
                if (heroPos2d.x < floorCenter)
                {
                    control.SendEvent("RIGHT");
                    return;
                }
                control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(6.5f, -0.5f);
                control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(1f, 0f);
                control.FsmVariables.GetFsmFloat("Current Charge Speed").Value = 15f;
                control.FsmVariables.GetFsmFloat("Emerge Speed").Value = 4f;
                emergePoint = new Vector3(floorCenter - floorsize * .25f, floorSpawn.point.y, 0f);
                gameObject.SetXScaleSign(false);
                control.SendEvent("FINISHED");
            });

            var eRight = control.GetState("Emerge Right");
            var eLeft = control.GetState("Emerge Left");

            eRight.ChangeTransition("FINISHED", "Attack Choice");
            eLeft.ChangeTransition("FINISHED", "Attack Choice");

            var leapStart = control.GetState("Leap Start");
            {
                leapStart.GetAction<SetPosition>(5).space = Space.World;
                leapStart.AddTransition("RECHOOSE", "Left or Right?");

                leapStart.InsertCustomAction(() =>
                {
                    //impossible leap
                    float leapSize = (gameObject.GetOriginalObjectSize().x * this.SizeScale * 3);
                    if ((wallLeft.point.x + leapSize > pos2d.x) || (wallRight.point.x - leapSize < pos2d.x))
                    {
                        control.SendEvent("RECHOOSE");
                        return;
                    }
                }, 0);
            }

            var emerge = control.GetState("Emerge");
            emerge.DisableActions(0, 6);
            emerge.InsertCustomAction(() => {
                transform.position = emergePoint.ToVec2() + Vector2.up * 1f;
            }, 0);

            var submergeCD = control.GetState("Submerge CD");
            submergeCD.DisableActions(6);

            var inAir = control.GetState("In Air");
            {
                inAir.DisableAction(1);

                var collisionAction = new HutongGames.PlayMaker.Actions.DetectCollisonDown();
                collisionAction.collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
                collisionAction.collideTag = "";
                collisionAction.sendEvent = new FsmEvent("LAND");

                inAir.AddAction(collisionAction);
               control.AddTimeoutAction(inAir, "LAND", 2f);
            }

            this.InsertHiddenState(control, "Init", "FINISHED", "Hidden");
        }
    }


    public class MegaMossChargerSpawner : DefaultSpawner<MegaMossChargerControl> { }

    public class MegaMossChargerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   


    public class MawlekBodyControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mawlek Control";

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var init = control.GetState("Init");
            init.DisableAction(1);
            init.DisableAction(14);

            init.ChangeTransition("FINISHED", "Start");

            this.InsertHiddenState(control, "Start", "FINISHED", "Idle");

            control.AddTimeoutAction(control.GetState("Wake In Air"), "LANDED", 1f);
            control.AddTimeoutAction(control.GetState("In Air"), "LANDED", 1f);
            control.AddTimeoutAction(control.GetState("In Air 2"), "LANDED", 1f);

            control.GetState("Start").InsertCustomAction(() =>
            {
                PhysicsBody.gravityScale = 3f;
            }, 0);
        }

        protected override void SetDefaultPosition()
        {
            base.SetDefaultPosition();
            control.FsmVariables.GetFsmFloat("Start X").Value = transform.position.x;
            control.FsmVariables.GetFsmFloat("Start Y").Value = transform.position.y;
        }
    }

    public class MawlekBodySpawner : DefaultSpawner<MawlekBodyControl> { }

    public class MawlekBodyPrefabConfig : DefaultPrefabConfig { }



    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GhostWarriorMarmuControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        protected override void SetCustomPositionOnSpawn()
        {
            {
                var fsm = gameObject.LocateMyFSM("Set Ghost PD Int");
                GameObject.Destroy(fsm);
            }

            {
                var fsm = gameObject.LocateMyFSM("FSM");
                GameObject.Destroy(fsm);
            }

            //keep marmu from teleporting into walls and stuff
            var setPos = control.GetState("Set Pos");
            setPos.DisableAction(0);
            setPos.DisableAction(1);
            setPos.DisableAction(5);
            setPos.DisableAction(6);
            setPos.InsertCustomAction(() => {

                var telepos =gameObject.GetRandomPositionInLOSofSelf(1f, 40f, 2f);
                control.FsmVariables.GetFsmFloat("Tele X").Value = telepos.x;
                control.FsmVariables.GetFsmFloat("Tele Y").Value = telepos.y;

            },0);

            //keep marmu from teleporting into walls and stuff
            var setPos2 = control.GetState("Set Pos 2");
            setPos.DisableAction(0);
            setPos.DisableAction(1);
            setPos.DisableAction(5);
            setPos.DisableAction(6);
            setPos.InsertCustomAction(() => {

                var telepos =gameObject.GetRandomPositionInLOSofSelf(1f, 40f, 2f);
                control.FsmVariables.GetFsmFloat("Tele X").Value = telepos.x;
                control.FsmVariables.GetFsmFloat("Tele Y").Value = telepos.y;

            }, 0);
        }

        public override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if (objectThatWillBeReplaced == null)
                return base.GetStartingMaxHP(objectThatWillBeReplaced);

            if(objectThatWillBeReplaced.IsBoss())
                return base.GetStartingMaxHP(objectThatWillBeReplaced);

            //maps get insane when there's lots of these.... so scale down their HP really fast if there's more than 1
            var result = base.GetStartingMaxHP(objectThatWillBeReplaced);
            float hpScale = GameObject.FindObjectsOfType<GhostWarriorMarmuControl>().Length;

            if (hpScale < 1f)
                hpScale = 2f;
            else if (hpScale > 1f)
                hpScale *= 4f;

            float curHP = result;
            float newHP = curHP / hpScale;

            return Mathf.Clamp(Mathf.FloorToInt(newHP), 1, Mathf.FloorToInt(curHP));
        }
    }

    public class GhostWarriorMarmuSpawner : DefaultSpawner<GhostWarriorMarmuControl>
    {
    }

    public class GhostWarriorMarmuPrefabConfig : DefaultPrefabConfig
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    ////
    public class JarCollectorControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        //public GameObject spawnerPlaceholder;
        //public GameObject buzzers;
        //public GameObject spitters;
        //public GameObject rollers;

        public GameObject jarPrefab;
        //public SpawnObjectFromGlobalPool spawnAction;

        //public PrefabObject buzzer;
        //public PrefabObject spitter;
        //public PrefabObject roller;

        //public int buzzerHP = 15;
        //public int spitterHP = 10;
        //public int rollerHP = 5;

        //public RNG rng;

        //public List<(PrefabObject, int)> possibleSpawns;

        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => true;

        public override float spawnPositionOffset => 1f;

        public Vector2 throwOffset;

        //protected virtual void SetupSpawnerPlaceholders()
        //{
        //    if (spawnerPlaceholder == null)
        //    {
        //        //generate the placeholder for the init state
        //        spawnerPlaceholder = new GameObject("Spawner Placeholder");
        //        spawnerPlaceholder.transform.SetParent(transform);
        //        spawnerPlaceholder.SetActive(false);

        //        buzzers = new GameObject("Buzzers");
        //        spitters = new GameObject("Spitters");
        //        rollers = new GameObject("Rollers");

        //        buzzers.transform.SetParent(spawnerPlaceholder.transform);
        //        spitters.transform.SetParent(spawnerPlaceholder.transform);
        //        rollers.transform.SetParent(spawnerPlaceholder.transform);

        //        control.FsmVariables.GetFsmGameObject("Top Pool").Value = spawnerPlaceholder;
        //        control.FsmVariables.GetFsmGameObject("Buzzers").Value = buzzers;
        //        control.FsmVariables.GetFsmGameObject("Spitters").Value = spitters;
        //        control.FsmVariables.GetFsmGameObject("Rollers").Value = rollers;

        //        var db = EnemyRandomizerDatabase.GetDatabase();
        //        buzzer = db.Enemies["Buzzer"];
        //        spitter = db.Enemies["Spitter"];
        //        roller = db.Enemies["Roller"];

        //        possibleSpawns = new List<(PrefabObject, int)>()
        //        {
        //            (buzzer, buzzerHP),
        //            (spitter, spitterHP),
        //            (roller, rollerHP)
        //        };

        //        rng = new RNG();
        //        rng.Reset();
        //    }
        //}

        public GameObject lastSpawned;

        protected virtual void SetMaxEnemies(int max)
        {
            ChildController.maxChildren = max;
            control.FsmVariables.GetFsmInt("Enemies Max").Value = max;
        }

        //protected virtual void OnSpawned(SpawnEffect self, GameObject spawned)
        //{
        //    lastSpawned = ChildController.ActivateAndTrackSpawnedObject(spawned);
        //    self.onSpawn -= OnSpawned;
        //}

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            var hpscaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpscaler != null)
                Destroy(hpscaler);

            var corpse = gameObject.GetCorpseObject();
            if (corpse != null)
            {
                corpse.AddCorpseRemoverWithEffect(gameObject, "Death Explode Boss");
            }

            SetMaxEnemies(4);

            var init = control.GetState("Init");
            init.DisableAction(10);
            init.DisableAction(11);
            init.DisableAction(12);
            init.DisableAction(13);

            var startLand = control.GetState("Start Land");
            startLand.DisableAction(2);

            var onDeath = control.GetState("Death Start");
            onDeath.DisableAction(0);
            onDeath.InsertCustomAction(() => {
                if (EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);

            control.ChangeTransition("Start Land", "FINISHED", "Roar End");

            var ec = control.GetState("Enemy Count");
            ec.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmInt("Current Enemies").Value = ChildController.Children.Count;
            }, 0);

            //remove fly away anim
            control.ChangeTransition("Jump Antic", "FINISHED", "Summon?");

            var summon = control.GetState("Summon?");
            summon.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmInt("Boss Tag Count").Value = ChildController.Children.Count;
            }, 0);

            //remove fly return anim
            control.ChangeTransition("Summon?", "CANCEL", "Hop Start Antic");

            //after spawn we'll be lunging, so return here
            control.ChangeTransition("Spawn", "FINISHED", "Lunge Swipe");

            var spawn = control.GetState("Spawn");
            jarPrefab = spawn.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;

            spawn.DisableAction(1);
            spawn.DisableAction(2);
            spawn.DisableAction(3);
            spawn.DisableAction(4);
            spawn.DisableAction(5);
            spawn.DisableAction(6);
            spawn.DisableAction(7);
            spawn.RemoveTransition("REPOS");

            //spawnAction = new SpawnObjectFromGlobalPool();
            //spawnAction.storeObject = new FsmGameObject();

            //spawnAction.gameObject = new FsmGameObject();
            //spawnAction.gameObject.Value = jarPrefab;

            //spawnAction.spawnPoint = new FsmGameObject();
            //spawnAction.spawnPoint.Value = gameObject;

            //spawnAction.position = new FsmVector3();
            //spawnAction.position.Value = Vector3.zero; //offset from boss (might update later)

            //spawn.AddAction(spawnAction);
            spawn.AddCustomAction(() =>
            {
                //var go = GameObject.Instantiate(spawnAction.storeObject.Value);
                var go = GameObject.Instantiate(jarPrefab);

                if (go != null)
                {
                    float dist = 1.5f;
                    var throwDir = gameObject.GetRandomDirectionFromSelf(true);

                    bool rightThrow = transform.localScale.x > 0;

                    //make sure the throw is "forwards"
                    if (throwDir.x < 0 && transform.localScale.x > 0)
                        throwDir.x = -throwDir.x;
                    else if (throwDir.x > 0 && transform.localScale.x < 0)
                        throwDir.x = -throwDir.x;

                    var throwPoint = throwDir * dist + pos2d + throwOffset;
                    go.transform.position = throwPoint;

                    var jar = go.GetComponent<SpawnJarControl>();
                    if (jar == null)
                        Dev.LogError("NO JAR COMPONENT FOUND!");

                    try
                    {
                        //don't use the component
                        jar.enabled = false;

                        ParticleSystem ps = jar.gameObject.AddParticleEffect_TorchShadeEmissions().GetComponent<ParticleSystem>();
                        ps.emissionRate = ps.emissionRate * 4f;


                        var body = jar.GetComponent<Rigidbody2D>();
                        if (body != null)
                        {
                            body.velocity = throwDir * 60f;
                            body.drag = 0f;
                            body.gravityScale = 2f;
                            body.angularVelocity = rightThrow ? -400f : 400f;
                            body.freezeRotation = false;
                            body.angularDrag = 0f;
                        }

                        var col = jar.GetComponent<CircleCollider2D>();
                        if(col != null)
                            col.enabled = true;

                        go.SetActive(true);
                        jar.GetComponent<SpriteRenderer>().enabled = true;
                        jar.transform.localScale = jar.transform.localScale * 0.5f;

                        var dustTrail = jar.dustTrail;

                        dustTrail.Play();

                        var jpoob = jar.gameObject.GetOrAddComponent<PreventOutOfBounds>();
                        jpoob.onBoundCollision += BreakJar;

                        transform.SetPositionZ(0.01f);
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Error flinging custom collector jar! ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                    }
                }
                else
                {
                    Dev.LogError("NO JAR PREFAB FOUND!");
                }
            });

            spawn.AddCustomAction(() => control.SendEvent("FINISHED"));

            this.InsertHiddenState(control, "Init", "FINISHED", "Start Fall");

            try
            { 
                var fsm = gameObject.LocateMyFSM("Death");
                fsm.ChangeTransition("Init", "DEATH", "Fall");

                var phase = gameObject.LocateMyFSM("Phase Control");
                GameObject.Destroy(phase);
            }
            catch (Exception e)
            {
                Dev.LogError($"Error fixing additional fsms! ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        protected virtual void BreakJar(RaycastHit2D r, GameObject a, GameObject b)
        {
            var jar = a.GetComponent<SpawnJarControl>();
            if(jar == null)
                jar = a.GetComponent<SpawnJarControl>();

            var dustTrail = jar.dustTrail;
            var ptBreakS = jar.ptBreakS;
            var ptBreakL = jar.ptBreakL;
            var strikeNailR = jar.strikeNailR;

            dustTrail.Stop();

            ptBreakS.Play();
            ptBreakL.Play();
            strikeNailR.Spawn(jar.transform.position);

            var jpoob = jar.gameObject.GetOrAddComponent<PreventOutOfBounds>();
            jpoob.onBoundCollision -= BreakJar;

            lastSpawned = SpawnChildForEnemySpawner(jar.transform.position, false, "Spitter");
            lastSpawned = ChildController.ActivateAndTrackSpawnedObject(lastSpawned);

            var soc = lastSpawned.GetComponent<DefaultSpawnedEnemyControl>();
            if (soc != null)
            {
                soc.defaultScaledMaxHP = SpawnerExtensions.GetObjectPrefab("Roller").prefab.GetEnemyHealthManager().hp;
                soc.CurrentHP = soc.defaultScaledMaxHP;
            }

            SpawnerExtensions.SpawnEntityAt("Pt Break", jar.transform.position, null, true);

            jar.GetComponent<SpriteRenderer>().enabled = false;
            jar.GetComponent<CircleCollider2D>().enabled = false;
            jar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            jar.GetComponent<Rigidbody2D>().angularVelocity = 0f;

            jar.breakSound.SpawnAndPlayOneShot(jar.audioSourcePrefab, jar.transform.position);

            GameObject.Destroy(jar.gameObject, 2f);
        }

        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class JarCollectorSpawner : DefaultSpawner<JarCollectorControl> { }

    public class JarCollectorPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class InfectedKnightControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM balloonFSM;

        public override string FSMName => "IK Control";


        public override void Setup(GameObject other)
        {
            base.Setup(other);

            ChildController.maxChildren = 3;

            var fsm = gameObject.LocateMyFSM("FSM");
            if (fsm != null)
                GameObject.Destroy(fsm);

            balloonFSM = gameObject.LocateMyFSM("Spawn Balloon");

            var pause = control.GetState("Pause");
            pause.DisableAction(1);
            pause.AddCustomAction(() => { control.SendEvent("FINISHED"); });

            var waiting = control.GetState("Waiting");
            waiting.DisableAction(2);
            waiting.DisableAction(3);
            waiting.DisableAction(4);
            waiting.DisableAction(5);
            waiting.AddCustomAction(() => { control.SendEvent("BATTLE START"); });

            var closeGates = control.GetState("Close Gates");
            closeGates.DisableAction(0);
            closeGates.DisableAction(3);

            var setx = control.GetState("Set X");
            setx.DisableAction(0);
            setx.DisableAction(2);

            var introFall = control.GetState("Intro Fall");
            introFall.DisableAction(2);
            introFall.AddCustomAction(() => {

                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                control.SendEvent("LAND"); });

            control.ChangeTransition("Intro Land", "FINISHED", "First Counter");
            this.InsertHiddenState(control, "Intro Land", "FINISHED", "First Counter");

            var inAir2 = control.GetState("In Air 2");
            inAir2.DisableAction(3);
            inAir2.AddAction(new Wait() { time = 3f, finishEvent = new FsmEvent("LAND") });

            var inAir = control.GetState("In Air");
            inAir.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Min Dstab Height").Value = floorY + 6f;
                control.FsmVariables.GetFsmFloat("Air Dash Height").Value = floorY + 3f;
            }, 0);

            var inert = balloonFSM.GetState("Inert");
            inert.AddCustomAction(() => { balloonFSM.SendEvent("START SPAWN"); });

            var stop = balloonFSM.GetState("Stop");
            inert.AddCustomAction(() => { balloonFSM.SendEvent("START SPAWN"); });

            balloonFSM.OverrideState("Spawn", () =>
            {
                if (ChildController.AtMaxChildren)
                {
                    balloonFSM.SendEvent("STOP SPAWN");
                }

                if (gameObject.CanSeePlayer())
                {
                    var randomDir = UnityEngine.Random.insideUnitCircle;
                    var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 12f);

                    if (spawnRay.distance < 5f)
                    {
                        balloonFSM.SendEvent("CANCEL");
                    }
                    else
                    {

                        var spawnPoint = spawnRay.point - spawnRay.normal;
                        ChildController.SpawnAndTrackChild("Parasite Balloon", spawnPoint);
                        balloonFSM.SendEvent("FINISHED");
                    }
                }
                else
                {
                    balloonFSM.SendEvent("CANCEL");
                }
            });
        }

        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.StickToGround(-1f);
        }
    }

    public class InfectedKnightSpawner : DefaultSpawner<InfectedKnightControl> { }


    public class InfectedKnightPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GreyPrinceControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public float customTransformMinAliveTime = 1f;
        public float customTransformAggroRange = 10f;

        public int NumGPZ => GameObject.FindObjectsOfType<GreyPrinceControl>().Length;

        public override void Setup(GameObject other)
        {
            base.Setup(other);


            var setJumps = control.GetState("Set Jumps");
            setJumps.InsertCustomAction(() => { 
            if (NumGPZ > 2)
            {
                //not allowed to jump if there's too many GPZ
                control.SetState("Idle Start");
            }
            }, 0);

            var landWaves = control.GetState("Land Waves");
            landWaves.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Shockwave Y").Value = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 0.5f, Vector2.down, 10).point.y;
            }, 0);

            {
                var slashWavesR = control.GetState("Slash Waves R");
                slashWavesR.InsertCustomAction(() => {
                    control.FsmVariables.GetFsmFloat("Shockwave Y").Value = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 0.5f, Vector2.down, 10).point.y;
                }, 0);
            }

            {
                var slashWavesL = control.GetState("Slash Waves L");
                slashWavesL.InsertCustomAction(() => {
                    control.FsmVariables.GetFsmFloat("Shockwave Y").Value = SpawnerExtensions.GetRayOn(pos2d + Vector2.up * 0.5f, Vector2.down, 10).point.y;
                }, 0);
            }

            var e1 = control.GetState("Enter 1");
            e1.AddCustomAction(() => { control.FsmVariables.GetFsmInt("Level").Value = 1; });//force level to be 1 so this doesn't get out of hand...

            GameObject.Destroy(gameObject.LocateMyFSM("Constrain X"));

            control.GetState("Level 1").DisableActions(0);
            control.GetState("Level 2").DisableActions(0);
            control.GetState("Level 3").DisableActions(0);
            control.GetState("4+").DisableActions(0);

            var fsm = control;
            //remove the transitions related to chain spawning zotes for the event
            fsm.RemoveTransition("Dormant", "ZOTE APPEAR");
            fsm.RemoveTransition("Dormant", "GG BOSS");
            fsm.RemoveTransition("GG Pause", "FINISHED");

            //change the start transition to just begin the spawn antics
            fsm.ChangeTransition("Level 1", "FINISHED", "Enter 1");
            fsm.ChangeTransition("Level 2", "FINISHED", "Enter 1");
            fsm.ChangeTransition("Level 3", "FINISHED", "Enter 1");
            fsm.ChangeTransition("4+", "FINISHED", "Enter 1");

            fsm.ChangeTransition("Enter 3", "FINISHED", "Roar End");

            this.InsertHiddenState(control, "Init", "FINISHED", "Level Check");

            var idle = control.GetState("Idle Start");
            idle.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Right X").Value = edgeR;
                control.FsmVariables.GetFsmFloat("Left X").Value = edgeL;
            },0);
        }

        protected override void CheckControlInCustomHiddenState()
        {
            if (customTransformMinAliveTime > 0)
            {
                customTransformMinAliveTime -= Time.deltaTime;
                return;
            }

            if (gameObject.CanSeePlayer() && gameObject.DistanceToPlayer() < customTransformAggroRange)
            {
                base.CheckControlInCustomHiddenState();
            }
        }
    }

    public class GreyPrinceSpawner : DefaultSpawner<GreyPrinceControl> { }

    public class GreyPrincePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GiantFlyControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Big Fly Control";

        public static int babiesToSpawn = 6;

        public override bool preventOutOfBoundsAfterPositioning => true;

        
        IEnumerator WaitForCorpse(GameObject go)
        {
            for(; ; )
            {
                if (go == null)
                    break;

                if (!go.scene.IsValid())
                    yield break;

                
                yield return null;
            }

            for (; ; )
            {
                if (go == null)
                {
                    var found = GameObject.FindObjectsOfType<PlayMakerFSM>().Where(x => x.name == "burster");
                    foreach (var f in found)
                    {
                        var spawn = f.GetState("Spawn Flies 2");
                        if (spawn.Actions.Length <= 3)
                        {
                            spawn.AddCustomAction(() =>
                            {
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                                SpawnerExtensions.SpawnEnemyForEnemySpawner(f.transform.position, true, "Fly");
                            });
                            yield break;
                        }
                    }
                }
                yield return null;
            }
        }


        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (thisMetadata != originialMetadata)
            {
                var init = control.GetState("Init");
                init.ChangeTransition("FINISHED", "Wake");
                init.DisableAction(0);
                init.DisableAction(2);
                init.DisableAction(6);

                var wake = control.GetState("Wake");
                wake.DisableAction(0);
                wake.DisableAction(1);
                wake.DisableAction(2);
                wake.DisableAction(3);
                wake.DisableAction(5);
                wake.DisableAction(6);

                var fly = control.GetState("Fly");
                fly.DisableAction(5);
                fly.DisableAction(6);
                fly.DisableAction(7);

                GameManager.instance.StartCoroutine(WaitForCorpse(gameObject));
            }
        }

        //public void SelfSpawnBabies()
        //{
        //    SpawnBabies(gameObject);
        //}

        //static void SpawnBabies(GameObject owner)
        //{
        //    try
        //    {
        //        if (EnemyRandomizerDatabase.GetDatabase != null)
        //        {
        //            for (int i = 0; i < babiesToSpawn; ++i)
        //            {
        //                GameObject result = null;
        //                if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
        //                {
        //                    Dev.Log("trying to spawn via prefab " + src.prefabName);
        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn(src);
        //                }
        //                else
        //                {
        //                    Dev.Log("trying to spawn via string");
        //                    result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly");
        //                }

        //                Dev.Log("result = " + result);
        //                Dev.Log("self.Owner = " + owner);
        //                if (result != null && owner != null)
        //                {
        //                    result.transform.position = owner.transform.position;
        //                    result.SafeSetActive(true);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Dev.LogError($"Caught exception trying to spawn a custom fly! {e.Message} STACKTRACE:{e.StackTrace}");
        //    }
        //}

        //static string MODHOOK_BeforeSceneLoad(string sceneName)
        //{
        //    ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
        //    On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
        //    return sceneName;
        //}
        //public virtual void OnEnable()
        //{
        //    ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
        //    ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;
        //    On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
        //    On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        //}
        //static void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        //{
        //    orig(self);

        //    if (string.Equals(self.Name, "Spawn Flies 2"))
        //    {
        //        SpawnBabies(self.Fsm.Owner.gameObject);
        //    }
        //}
    }

    public class GiantFlySpawner : DefaultSpawner<GiantFlyControl>
    {
    }

    public class GiantFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Big Buzzer";

        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            ChildController.maxChildren = 4;

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                if (!ChildController.AtMaxChildren)
                {

                    var leftMax = gameObject.GetLeftX().point;
                    var rightMax = gameObject.GetRightX().point;

                    var leftSpawn = pos2d + Vector2.left * 20f;
                    var rightSpawn = pos2d + Vector2.right * 20f;

                    var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                    var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                    ChildController.SpawnAndTrackChild("Buzzer", leftShorter);
                    ChildController.SpawnAndTrackChild("Buzzer", rightShorter);
                }

                control.SendEvent("FINISHED");
            });

            control.GetState("Idle").InsertCustomAction(() =>
            {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                gameObject.GetComponentsInChildren<Collider2D>(true).ToList().ForEach(x => x.enabled = true);
            }, 0);


            this.InsertHiddenState(control, "Hanging", "TAKE DAMAGE", "Unfurl");
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }


    public class GiantBuzzerSpawner : DefaultSpawner<GiantBuzzerControl>
    {
        public override GameObject Spawn(PrefabObject p, GameObject source)
        {
            int buzzersInScene = GameObject.FindObjectsOfType<GiantBuzzerControl>().Length;

            //change the spawn to be the col buzzer if zote has been rescued
            if (GameManager.instance.GetPlayerDataBool("zoteRescuedBuzzer") ||
                GameManager.instance.GetPlayerDataInt("zoteDeathPos") == 0 ||
                buzzersInScene > 0)
            {
                Dev.Log("Spawning alternate buzzer boss");
                return base.Spawn(defaultDatabase.Enemies["Giant Buzzer Col"], source);
            }
            else
            {
                Dev.Log("Spawning default buzzer boss");
                return base.Spawn(p, source);
            }
        }
    }

    public class GiantBuzzerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerColControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Big Buzzer";
        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            ChildController.maxChildren = 4;

            var r = control.GetState("Roar Left");
            var l = control.GetState("Roar Right");
            r.DisableAction(0);
            l.DisableAction(0);

            var init = control.GetState("Init");
            control.ChangeTransition("Init", "GG BOSS", "Idle");


            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.DisableAction(4);
            summon.AddCustomAction(() =>
            {
                if (!ChildController.AtMaxChildren)
                {

                    var leftMax = gameObject.GetLeftX().point;
                    var rightMax = gameObject.GetRightX().point;

                    var leftSpawn = pos2d + Vector2.left * 20f;
                    var rightSpawn = pos2d + Vector2.right * 20f;

                    var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                    var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                    ChildController.SpawnAndTrackChild("Buzzer", leftShorter);
                    ChildController.SpawnAndTrackChild("Buzzer", rightShorter);
                }

                control.SendEvent("FINISHED");
            });

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");

            control.GetState("Idle").InsertCustomAction(() =>
            {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                gameObject.GetComponentsInChildren<Collider2D>(true).ToList().ForEach(x => x.enabled = true);
            }, 0);
        }

        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class GiantBuzzerColSpawner : DefaultSpawner<GiantBuzzerColControl> { }

    public class GiantBuzzerColPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class FalseKnightDreamControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "FalseyControl";

        public Vector3 originalScale;

        public bool hasSetupYet = false;

        public override bool preventOutOfBoundsAfterPositioning => true;
        public override bool preventInsideWallsAfterPositioning => false;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //CurrentHP = 40;

            //{
            //    var checkHP = gameObject.LocateMyFSM("Check Health");
            //    if (checkHP != null)
            //        Destroy(checkHP);
            //}

           control.AddTimeoutAction(control.GetState("Rise"), "FALL", 2f);
           control.AddTimeoutAction(control.GetState("Fall"), "FALL", 1f);

           control.AddTimeoutAction(control.GetState("Rise 2"), "FALL", 2f);
           control.AddTimeoutAction(control.GetState("Fall 2"), "FALL", 1f);

           control.AddTimeoutAction(control.GetState("S Rise"), "FALL", 2f);
           control.AddTimeoutAction(control.GetState("S Fall"), "FALL", 1f);

           control.AddTimeoutAction(control.GetState("JA Rise"), "FALL", 2f);
           control.AddTimeoutAction(control.GetState("JA Fall"), "FALL", 1f);

            this.InsertHiddenState(control, "Init", "FINISHED", "Start Fall");

            var init = control.GetState("Init");
            init.DisableAction(1);
            init.DisableAction(2);
            init.DisableAction(9);
            init.DisableAction(10);
            init.DisableAction(11);
            init.DisableAction(13);
            init.DisableAction(14);
            init.DisableAction(15);
            init.DisableAction(18);
            init.DisableAction(19);

            var startFall = control.GetState("Start Fall");
            startFall.DisableAction(1);
            startFall.DisableAction(2);
            startFall.DisableAction(3);
            startFall.DisableAction(4);
            startFall.DisableAction(5);
            startFall.DisableAction(6);
            startFall.DisableAction(7);
            startFall.DisableAction(13);
            startFall.DisableAction(14);
            startFall.DisableAction(16);
            startFall.DisableAction(17);

            startFall.ChangeTransition("FALL", "State 1");

            var state1 = control.GetState("State 1");
            state1.DisableAction(1);
            state1.DisableAction(4);
            state1.AddCustomAction(() => { hasSetupYet = true; });

            control.ChangeTransition("Check", "JUMP", "First Idle");

            var jump = control.GetState("Jump");
            jump.DisableAction(4);

            jump.AddCustomAction(() => { hasSetupYet = true; });

            originalScale = transform.localScale;

            var turnr = control.GetState("Turn R");
            turnr.DisableAction(5);
            turnr.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                transform.localScale = new Vector3(originalScale.x * -1.3f * SizeScale, originalScale.y * SizeScale, originalScale.z * SizeScale);
                hasSetupYet = true;
            }, 5);

            var turnl = control.GetState("Turn L");
            turnl.DisableAction(5);
            turnl.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                transform.localScale = new Vector3(originalScale.x * 1.3f * SizeScale, originalScale.y * SizeScale, originalScale.z * SizeScale);
                hasSetupYet = true;
            }, 5);

            var checkDirection = control.GetState("Check Direction");
            checkDirection.RemoveTransition("TURN L");
            checkDirection.RemoveTransition("TURN R");
            checkDirection.RemoveTransition("FINISHED");
            checkDirection.RemoveTransition("CANCEL");
            control.OverrideState( "Check Direction", () => {
                if(!hasSetupYet)
                {
                    control.SetState("Init");
                    return;
                }
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                Destroy(gameObject);
            });
        }
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }


    public class FalseKnightDreamSpawner : DefaultSpawner<FalseKnightDreamControl> { }

    public class FalseKnightDreamPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class FalseKnightNewControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "FalseyControl";

        public Vector3 originalScale;
        public bool hasSetupYet = false;

        public override bool preventOutOfBoundsAfterPositioning => true;
        public override bool preventInsideWallsAfterPositioning => false;

        public override float spawnPositionOffset => 1.23f;

        public override bool useCustomPositonOnSpawn => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            control.AddTimeoutAction(control.GetState("JA Hit"), "LAND", 1f);
            control.AddTimeoutAction(control.GetState("JA Hit 2"), "LAND", 1f);

            control.AddTimeoutAction(control.GetState("Rise"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("Fall"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("Rise 2"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("Fall 2"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("S Rise"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("S Fall"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("JA Rise"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("JA Fall"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("JA Rise 2"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("JA Fall 2"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("Start Fall"), "FALL", 1f);

            control.AddTimeoutAction(control.GetState("Esc Rise"), "FALL", 2f);
            control.AddTimeoutAction(control.GetState("Esc Fall"), "FALL", 1f);

            this.InsertHiddenState(control, "Init", "FINISHED", "Start Fall");

            var init = control.GetState("Init");
            init.DisableAction(1);
            init.DisableAction(2);
            init.DisableAction(9);
            init.DisableAction(10);
            init.DisableAction(12);
            init.DisableAction(13);
            init.DisableAction(14);

            var startFall = control.GetState("Start Fall");
            startFall.DisableAction(1);
            startFall.DisableAction(2);
            startFall.DisableAction(3);
            startFall.DisableAction(4);
            startFall.DisableAction(5);
            startFall.DisableAction(10);
            startFall.DisableAction(11);
            startFall.DisableAction(12);
            startFall.DisableAction(13);
            startFall.InsertCustomAction(() => {

                var startPoint = gameObject.GetTeleportPositionAboveSelf(3f);
                var poob = gameObject.GetComponent<PreventOutOfBounds>();
                if(poob != null)
                {
                    poob.ForcePosition(startPoint);
                }
            },0);

            startFall.ChangeTransition("FALL", "State 1");

            var sAttackRecover = control.GetState("S Attack Recover");
            sAttackRecover.DisableActions(3);
            sAttackRecover.InsertCustomAction(() => {
                float x = control.FsmVariables.GetFsmFloat("Shockwave X Origin").Value;
                control.FsmVariables.GetFsmVector3("Shockwave Origin").Value = new Vector3(x, floorY, -.1f);
            }, 3);

            var state1 = control.GetState("State 1");
            state1.DisableAction(1);
            state1.DisableAction(4);

            control.ChangeTransition("Check", "JUMP", "First Idle");

            var idle = control.GetState("Idle");
            idle.DisableAction(0);
            idle.ChangeTransition("ESCAPED", "Init");
            idle.AddCustomAction(() => { hasSetupYet = true; });

            var jump = control.GetState("Jump");
            jump.DisableAction(6);
            jump.AddCustomAction(() => { hasSetupYet = true; });

            originalScale = transform.localScale;

            var turnr = control.GetState("Turn R");
            turnr.DisableAction(5);
            turnr.InsertCustomAction(() => {
                transform.localScale = new Vector3(originalScale.x * -1.3f * SizeScale, originalScale.y * SizeScale, originalScale.z * SizeScale);
                hasSetupYet = true;
            }, 5);

            var turnl = control.GetState("Turn L");
            turnl.DisableAction(5);
            turnl.InsertCustomAction(() => {
                transform.localScale = new Vector3(originalScale.x * 1.3f * SizeScale, originalScale.y * SizeScale, originalScale.z * SizeScale);
                hasSetupYet = true;
            }, 5);

            var checkDirection = control.GetState("Check Direction");
            checkDirection.RemoveTransition("TURN L");
            checkDirection.RemoveTransition("TURN R");
            checkDirection.RemoveTransition("FINISHED");
            control.OverrideState( "Check Direction", () => {
                if (!hasSetupYet)
                {
                    control.SetState("Init");
                    return;
                }
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                Destroy(gameObject);
            });
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);
            gameObject.FindGameObjectInDirectChildren("Head").SafeSetActive(false);


            GetComponent<PreventOutOfBounds>().onBoundCollision -= LandNow;
            GetComponent<PreventOutOfBounds>().onBoundCollision += LandNow;
        }

        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.transform.position = gameObject.transform.position.ToVec2() + Vector2.up * gameObject.GetOriginalObjectSize().y * this.SizeScale * .5f;
        }


        protected virtual void LandNow(RaycastHit2D r, GameObject a, GameObject b)
        {
            if (control.ActiveStateName == "Rise" ||
                control.ActiveStateName == "Rise 2" ||
                control.ActiveStateName == "S Rise" ||
                control.ActiveStateName == "JA Rise" ||
                control.ActiveStateName == "JA Rise 2" ||
                control.ActiveStateName == "Esc Rise")
            {

                //var isUp = gameObject.GetRoofRay().distance < 5f;
                //if (isUp)
                {
                    PhysicsBody.velocity = Vector2.down * 2f;
                    var poob = gameObject.GetComponent<PreventOutOfBounds>();
                    if (poob != null)
                    {
                        poob.ForcePosition(pos2d + Vector2.down * 2f);
                    }

                    //var fallPoint = gameObject.GetTeleportPositionAboveSelf(0f);
                    //if (poob != null)
                    //{
                    //    poob.ForcePosition(fallPoint);
                    //}
                }

                control.SendEvent("FALL");
            }


            if (control.ActiveStateName == "Fall" ||
                control.ActiveStateName == "Fall 2" ||
                control.ActiveStateName == "S Fall" ||
                control.ActiveStateName == "JA Fall" ||
                control.ActiveStateName == "JA Fall 2" ||
                control.ActiveStateName == "Esc Fall")
            {

                //var isUp = gameObject.GetRoofRay().distance < 5f;
                //if (isUp)
                {
                    PhysicsBody.velocity = Vector2.down * 20f;
                    var poob = gameObject.GetComponent<PreventOutOfBounds>();
                    if (poob != null)
                    {
                        poob.ForcePosition(pos2d + Vector2.down * 2f);
                    }

                    var fallPoint = gameObject.GetTeleportPositionAboveSelf(0f);
                    if (poob != null)
                    {
                        poob.ForcePosition(fallPoint);
                    }
                }

                control.SendEvent("FALL");
            }
        }
    }


    public class FalseKnightNewSpawner : DefaultSpawner<FalseKnightNewControl> { }

    public class FalseKnightNewPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class AbsoluteRadianceControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (GameManager.instance.GetCurrentMapZone() == "FINAL_BOSS")
            {
                //do nothing
                return;
            }

            GameObject comb = attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
            comb.transform.position = new Vector3(transform.position.x, transform.position.y, 0.006f);

            PlayMakerFSM combControl = comb.LocateMyFSM("Control");
            combControl.GetFirstActionOfType<SetPosition>("TL").x = edgeL;
            combControl.GetFirstActionOfType<SetPosition>("TR").x = edgeR;
            combControl.GetFirstActionOfType<RandomFloat>("Top").min = pos2d.x - 1;
            combControl.GetFirstActionOfType<RandomFloat>("Top").max = pos2d.x + 1;
            combControl.GetFirstActionOfType<SetPosition>("Top").y = roofY;
            combControl.GetFirstActionOfType<SetPosition>("L").x = edgeL;
            combControl.GetFirstActionOfType<SetPosition>("L").y = pos2d.y;
            combControl.GetFirstActionOfType<SetPosition>("R").x = edgeR;
            combControl.GetFirstActionOfType<SetPosition>("R").y = pos2d.y;

            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
            control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

            //AddResetToStateOnHide(control, "Init");

            var climbPlatsState = control.GetState("Climb Plats1");
            climbPlatsState.Actions = new FsmStateAction[] {
                new CustomFsmAction(() => Destroy(gameObject))
            };

            if (SizeScale >= 1f)
            {
                gameObject.ScaleObject(SizeScale * 0.25f);
            }

            if (teleport == null)
                teleport = gameObject.LocateMyFSM("Teleport");

            if (control == null)
                control = gameObject.LocateMyFSM("Control");

            if (attackCommands == null)
                attackCommands = gameObject.LocateMyFSM("Attack Commands");

            //disable a variety of camera shake actions

            try
            {
                control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Rage1"); }

            try
            {
                control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Stun1"); }

            try
            {
                control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Tendrils1"); }

            try
            {
                //clip off both camera shakes
                control.GetState("Stun1 Roar").Actions = control.GetState("Stun1 Roar").Actions.Take(control.GetState("Stun1 Roar").Actions.Length - 2).ToArray();
            }
            catch (Exception e) { Dev.Log("error in Stun1"); }

            try
            {
                control.GetState("Stun1 Out").GetAction<ActivateGameObject>(8).activate = false;
                control.GetState("Stun1 Out").GetAction<SendEventByName>(9).sendEvent = string.Empty;
            }
            catch (Exception e) { Dev.Log("error in Stun1 out"); }


            //reduce this non-boss radiance to spawn only 1 or 2 shots
            attackCommands.ChangeRandomIntRange("Orb Antic", 1, 2);

            //disable enemy kill shake commands that make the camera shake
            attackCommands.DisableActions(
                  ("EB 1", 3)
                , ("EB 2", 4)
                , ("EB 3", 4)
                , ("EB 7", 3)
                , ("EB 8", 3)
                , ("EB 9", 3)
                , ("Spawn Fireball", 0)
                , ("Aim", 2)
                );

            var orbPrefab = attackCommands.GetState("Spawn Fireball").GetAction<SpawnObjectFromGlobalPool>(1).gameObject.Value;
            orbPrefab.transform.localScale = orbPrefab.transform.localScale * 0.4f;

            if (orbPrefab.GetComponent<DamageHero>() != null)
                orbPrefab.GetComponent<DamageHero>().damageDealt = 1;

            //grab attacks
            List<GameObject> attacks = new List<GameObject>() {
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst1").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst2").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst3").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Glow").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Self").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Shot Charge").Value,
            };

            //reduce damage and shrink attacks
            attacks.Select(x => x.GetComponent<DamageHero>()).Where(x => x != null)
                .ToList().ForEach(x => { x.damageDealt = 1; x.transform.localScale = new Vector3(.4f, .4f, 1f); });

            control.ChangeTransition("Intro End", "FINISHED", "Arena 1 Idle");

            //disable shaking from teleport
            teleport.GetState("Arrive").GetAction<SendEventByName>(5).sendEvent = string.Empty;

            //add aggro radius controls to teleport
            InsertHiddenState(teleport, "Music?", "FINISHED", "Arrive");

            control.RemoveAction("First Tele", 3); //remove big shake

            InsertHiddenState(control, "First Tele", "TELEPORTED", "Intro Recover", createNewPreTransitionEvent: true);

            //special behaviour for abs rad
            //mute the init sfx
            control.SetAudioOneShotVolume("Set Arena 1");
            control.SetAudioOneShotVolume("First Tele");
        }

        protected override void Update()
        {
            base.Update();
            if(teleport != null && teleport.ActiveStateName == FSMHiddenStateName)
            {
                teleport.SendEvent("SHOW");
            }

            this.UpdateRefs(attackCommands, CommandFloatRefs);
            this.UpdateRefs(control, FloatRefs);
        }

        protected virtual Dictionary<string, Func<SpawnedObjectControl, float>> CommandFloatRefs
        {
            get => new Dictionary<string, Func<SpawnedObjectControl, float>>()
            {
                { "Orb Max X", x => x.edgeR - 1},
                { "Orb Max Y", x => x.roofY - 1},
                { "Orb Min X", x => x.edgeL + 1},
                { "Orb Min Y", x => x.floorY + 3},
            };
        }

        protected virtual Dictionary<string, Func<SpawnedObjectControl, float>> FloatRefs
        {
            get => new Dictionary<string, Func<SpawnedObjectControl, float>>()
            {
                { "A1 X Max", x => x.edgeR - 2},
                { "A1 X Min", x => x.edgeL + 2},
            };
        }

        //protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> refs)
        //{
        //    base.UpdateRefs(fsm, refs);
        //    base.UpdateRefs(attackCommands, CommandFloatRefs);
        //}
        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class AbsoluteRadianceSpawner : DefaultSpawner<AbsoluteRadianceControl>
    {
    }

    public class AbsoluteRadiancePrefabConfig : DefaultPrefabConfig
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LobsterControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        protected override bool DisableCameraLocks => false;

        public override float spawnPositionOffset => 1f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");

            //add a random timeout to force the lobster out of an infinite roll
            var rcCharging = control.GetState("RC Charging");
            control.AddTimeoutAction(rcCharging, "WALL", 2f);

            var rcAir = control.GetState("RC Air");
            control.AddTimeoutAction(rcAir, "LAND", 2f);
        }
    }

    public class LobsterSpawner : DefaultSpawner<LobsterControl> { }

    public class LobsterPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LancerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var init = control.GetState("Init");
            init.DisableAction(6);

            control.OverrideState( "Defeat", () =>
            {
                if (EnemyHealthManager.hp <= 0)
                {
                    gameObject.KillObjectNow();
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            });

            this.InsertHiddenState(control, "Init", "FINISHED", "First Aim");
        }
    }

    public class LancerSpawner : DefaultSpawner<LancerControl>
    {
        public override bool corpseRemovedByEffect => true;
        public override string corpseRemoveEffectName => "Death Explode Boss";
    }

    public class LancerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageKnightControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mage Knight";

        public override bool useCustomPositonOnSpawn => true;

        public float spawnOffset = 1f;
        public override float spawnPositionOffset => spawnOffset;

        protected override bool DisableCameraLocks => false;

        public bool canFakeout = false;
        public bool willFakeout = false;
        public bool didFakeout = false;

        public float chargeHeight => gameObject.GetOriginalObjectSize().y * this.SizeScale + 1f;

        public Vector2 teleQuakePoint;
        public Vector2 teleChargePoint;
        public Vector2 teleChargePoint2;

        float sideTeleRange = 8f;
        float teleAboveHeroHeight = 8f;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            canFakeout = SpawnerExtensions.RollProbability(out _, 2, 12);

            control.OverrideState("Up Tele Aim", () => {

                var aboveHero = gameObject.GetTeleportPositionAbovePlayer(teleAboveHeroHeight - 1f, teleAboveHeroHeight + 1f);
                teleQuakePoint = aboveHero;

                var dist = (aboveHero - heroPos2d).magnitude;
                if (dist < teleAboveHeroHeight * 0.5f)
                {
                    control.SendEvent("CANCEL");
                }
            });

            var upTele = control.GetState("Up Tele");
            upTele.DisableActions(4);
            upTele.InsertCustomAction(() => {
                control.FsmVariables.GetFsmVector3("Tele Out").Value = transform.position;
                transform.position = teleQuakePoint;

                var poob = gameObject.GetComponent<PreventOutOfBounds>();
                if (poob != null)
                {
                    poob.ForcePosition(teleQuakePoint);
                }
            }, 4);

            var stompAir = control.GetState("Stomp Air");
            if (canFakeout)
            {
                stompAir.AddTransition("FAKEOUT", "Tele Antic");
            }

            stompAir.InsertCustomAction(() => {
                willFakeout = false;
                float timeout = 0.7f;
                if (canFakeout && !didFakeout)
                {
                    willFakeout = SpawnerExtensions.RollProbability(out _, 1, 4);
                    timeout = 0.15f;
                    didFakeout = true;
                    control.StartTimeoutState("Stomp Air", "FAKEOUT", timeout);
                }
                else
                {
                    didFakeout = false;
                    control.StartTimeoutState("Stomp Air", "LANDED", timeout);
                }
                }, 2);

            var teleChoice = control.GetState("Tele Choice");
            teleChoice.InsertCustomAction(() => {
                if (willFakeout)
                {
                    willFakeout = false;
                    control.SendEvent("STOMP");
                }
            },0);


            var sideTeleAim = control.GetState("Side Tele Aim");
            sideTeleAim.InsertCustomAction(() =>
            {
                var left = gameObject.GetHorizontalTeleportPositionFromPlayer(false, chargeHeight, sideTeleRange - 1f, sideTeleRange + 1f);
                var right = gameObject.GetHorizontalTeleportPositionFromPlayer(true, chargeHeight, sideTeleRange - 1f, sideTeleRange + 1f);

                float leftd = (left - heroPos2d).magnitude;
                float rightd = (right - heroPos2d).magnitude;

                if (leftd < 3f && rightd < 3f)
                {
                    control.SendEvent("CANCEL");
                }
                else
                {
                    if (leftd > rightd)
                    {
                        teleChargePoint = left;
                        teleChargePoint2 = right;
                        control.SendEvent("R");
                    }
                    else
                    {
                        control.SendEvent("L");
                        teleChargePoint = right;
                        teleChargePoint2 = left;
                    }
                }
            }, 0);
            sideTeleAim.ChangeTransition("L", "Side Tele");
            sideTeleAim.ChangeTransition("R", "Side Tele");


            var stompRecover = control.GetState("Stomp Recover");
            stompRecover.InsertCustomAction(() => {
                PhysicsBody.gravityScale = 1f;
            }, 0);

            var cancelFrame = control.GetState("Cancel Frame");
            cancelFrame.InsertCustomAction(() => {
                PhysicsBody.gravityScale = 1f;
            }, 0);

            var sideTele = control.GetState("Side Tele");
            sideTele.DisableAction(6);
            sideTele.InsertCustomAction(() => {

                Vector3 telePoint = teleChargePoint;
                transform.position = telePoint;

            }, 6);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
        }

        protected override void SetCustomPositionOnSpawn()
        {
        }
    }

    public class MageKnightSpawner : DefaultSpawner<MageKnightControl> { }

    public class MageKnightPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///










    /////////////////////////////////////////////////////////////////////////////
    ///// (Oblobbles)
    public class MegaFatBeeControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "fat fly bounce";

        public PlayMakerFSM FSMattack { get; set; }

        public Vector2 spawnPos;
        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (other != null)
            {
                spawnPos = other.transform.position;
            }
            else
            {
                gameObject.transform.position = spawnPos;
            }

            FSMattack = gameObject.LocateMyFSM("Fatty Fly Attack");

            var init = control.GetState("Initialise");
            init.DisableAction(2);

            //skip the swoop in animation
            control.ChangeTransition("Initialise", "FINISHED", "Activate");
            var activate = control.GetState("Fly 2");
            activate.InsertCustomAction(() => {
                var wallLeft = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.left, 100f);
                var wallRight = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.right, 100f);
                control.FsmVariables.GetFsmFloat("X Max").Value = wallRight.point.x;
                control.FsmVariables.GetFsmFloat("X Min").Value = wallLeft.point.x;
            }, 0);
        }

        protected override void SetCustomPositionOnSpawn()
        {
            gameObject.transform.position = spawnPos;
            gameObject.GetOrAddComponent<PreventOutOfBounds>();
        }
    }

    public class MegaFatBeeSpawner : DefaultSpawner<MegaFatBeeControl> { }

    public class MegaFatBeePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlackKnightControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Black Knight";

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override float spawnPositionOffset => 0.65f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init Facing", "FINISHED", "Bugs In");
            control.ChangeTransition("Bugs In End", "FINISHED", "Roar End");

            control.GetState("Roar End").InsertCustomAction(() =>
            {
                UnFreeze();
            }, 0);

            control.GetState("Cloud Stop").DisableAction(3);
            control.GetState("Cloud Stop").DisableAction(4);

            control.AddTimeoutAction(control.GetState("Antic Air"), "LAND", 1f);
            control.AddTimeoutAction(control.GetState("Jump Air"), "LAND", 1f);
            control.AddTimeoutAction(control.GetState("Bounce Air"), "LAND", 1f);
            control.AddTimeoutAction(control.GetState("Charge"), "LAND", 1f);
        }

        protected virtual void LandNow(RaycastHit2D r, GameObject a, GameObject b)
        {
            if(control.ActiveStateName == "Antic Air" ||
                control.ActiveStateName == "Jump Air" ||
                control.ActiveStateName == "Bounce Air")
            {
                control.SendEvent("LAND");
            }
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);

            PhysicsBody.isKinematic = false;

            GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
            GetComponent<PreventOutOfBounds>().onBoundCollision += Freeze;

            GetComponent<PreventOutOfBounds>().onBoundCollision -= LandNow;
            GetComponent<PreventOutOfBounds>().onBoundCollision += LandNow;
        }

        protected virtual void Freeze(RaycastHit2D r, GameObject a, GameObject b)
        {
            var pl = gameObject.GetOrAddComponent<PositionLocker>();
            pl.positionLock = transform.position;
        }

        protected virtual void UnFreeze()
        {
            var locker = gameObject.GetComponent<PositionLocker>();
            if (locker != null)
            {
                GameObject.Destroy(locker);
            }
            if (GetComponent<PreventOutOfBounds>() != null)
            {
                GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
            }
        }
    }

    public class BlackKnightSpawner : DefaultSpawner<BlackKnightControl> { }

    public class BlackKnightPrefabConfig : DefaultPrefabConfig { }

    /////
    //////////////////////////////////////////////////////////////////////////////












    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RadianceControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

        //protected override void SetupBossAsNormalEnemy()
        //{
        //    base.SetupBossAsNormalEnemy();

        //    if (SizeScale >= 1f)
        //    {
        //        thisMetadata.ApplySizeScale(SizeScale * 0.25f);
        //    }

        //    if (teleport == null)
        //        teleport = gameObject.LocateMyFSM("Teleport");

        //    if (control == null)
        //        control = gameObject.LocateMyFSM("Control");

        //    if (attackCommands == null)
        //        attackCommands = gameObject.LocateMyFSM("Attack Commands");

        //    //disable a variety of camera shake actions

        //    try
        //    {
        //        control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
        //        control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
        //    }
        //    catch (Exception e) { Dev.Log("error in Rage1"); }

        //    try
        //    {
        //        control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
        //        control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
        //    }
        //    catch (Exception e) { Dev.Log("error in Stun1"); }

        //    try
        //    {
        //        control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
        //        control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
        //    }
        //    catch (Exception e) { Dev.Log("error in Tendrils1"); }

        //    try
        //    {
        //        //clip off both camera shakes
        //        control.GetState("Stun1 Roar").Actions = control.GetState("Stun1 Roar").Actions.Take(control.GetState("Stun1 Roar").Actions.Length - 2).ToArray();
        //    }
        //    catch (Exception e) { Dev.Log("error in Stun1"); }

        //    try
        //    {
        //        control.GetState("Stun1 Out").GetAction<ActivateGameObject>(8).activate = false;
        //        control.GetState("Stun1 Out").GetAction<SendEventByName>(9).sendEvent = string.Empty;
        //    }
        //    catch (Exception e) { Dev.Log("error in Stun1 out"); }


        //    //reduce this non-boss radiance to spawn only 1 or 2 shots
        //    ChangeRandomIntRange(attackCommands, "Orb Antic", 1, 2);

        //    //disable enemy kill shake commands that make the camera shake
        //    DisableSendEvents(attackCommands
        //        , ("EB 1", 3)
        //        , ("EB 2", 4)
        //        , ("EB 3", 4)
        //        , ("EB 7", 3)
        //        , ("EB 8", 3)
        //        , ("EB 9", 3)
        //        , ("Spawn Fireball", 0)
        //        , ("Aim", 2)
        //        );

        //    var orbPrefab = attackCommands.GetState("Spawn Fireball").GetAction<SpawnObjectFromGlobalPool>(1).gameObject.Value;
        //    orbPrefab.transform.localScale = orbPrefab.transform.localScale * 0.4f;

        //    if (orbPrefab.GetComponent<DamageHero>() != null)
        //        orbPrefab.GetComponent<DamageHero>().damageDealt = 1;

        //    //grab attacks
        //    List<GameObject> attacks = new List<GameObject>() {
        //    attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst1").Value,
        //    attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst2").Value,
        //    attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst3").Value,
        //    attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Glow").Value,
        //    attackCommands.FsmVariables.GetFsmGameObject("Self").Value,
        //    attackCommands.FsmVariables.GetFsmGameObject("Shot Charge").Value,
        //    };

        //    //reduce damage and shrink attacks
        //    attacks.Select(x => x.GetComponent<DamageHero>()).Where(x => x != null)
        //        .ToList().ForEach(x => { x.damageDealt = 1; x.transform.localScale = new Vector3(.4f, .4f, 1f); });

        //    control.ChangeTransition("Intro End", "FINISHED", "Arena 1 Idle");

        //    //disable shaking from teleport
        //    teleport.GetState("Arrive").GetAction<SendEventByName>(5).sendEvent = string.Empty;

        //    //add aggro radius controls to teleport
        //    InsertHiddenState(teleport, "Music?", "FINISHED", "Arrive");

        //    control.RemoveAction("First Tele", 3); //remove big shake

        //    InsertHiddenState(control, "First Tele", "TELEPORTED", "Intro Recover", createNewPreTransitionEvent: true);

        //    //special behaviour for abs rad
        //    if (FSMsUsingHiddenStates.Contains(control))
        //        FSMsUsingHiddenStates.Remove(control);

        //    //mute the init sfx
        //    SetAudioOneShotVolume(control, "Set Arena 1");
        //    SetAudioOneShotVolume(control, "First Tele");
        //}

        //protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> CommandFloatRefs
        //{
        //    get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
        //    {
        //        //{ "Orb Max X", x => x.xR.Max - 1},
        //        //{ "Orb Max Y", x => x.yR.Max - 1},
        //        //{ "Orb Min X", x => x.xR.Min + 1},
        //        //{ "Orb Min Y", x => x.yR.Min + 3},
        //    };
        //}

        ////protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        ////{
        ////    get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
        ////    {
        ////        //{ "A1 X Max", x => x.xR.Max - 2},
        ////        //{ "A1 X Min", x => x.xR.Min + 2},
        ////    };
        ////}

        //protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
        //{
        //    base.UpdateRefs(fsm, refs);
        //    base.UpdateRefs(attackCommands, CommandFloatRefs);
        //}

        //protected virtual void OnEnable()
        //{
        //    BuildArena();
        //}

        //protected override IEnumerator Start()
        //{
        //    GameObject comb = attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
        //    comb.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.006f);

        //    PlayMakerFSM combControl = comb.LocateMyFSM("Control");
        //    combControl.GetFirstActionOfType<SetPosition>("TL").x = bounds.xMin;
        //    combControl.GetFirstActionOfType<SetPosition>("TR").x = bounds.xMax;
        //    combControl.GetFirstActionOfType<RandomFloat>("Top").min = bounds.center.x - 1;
        //    combControl.GetFirstActionOfType<RandomFloat>("Top").max = bounds.center.x + 1;
        //    combControl.GetFirstActionOfType<SetPosition>("Top").y = bounds.yMax;
        //    combControl.GetFirstActionOfType<SetPosition>("L").x = bounds.xMin;
        //    combControl.GetFirstActionOfType<SetPosition>("L").y = bounds.center.y;
        //    combControl.GetFirstActionOfType<SetPosition>("R").x = bounds.xMax;
        //    combControl.GetFirstActionOfType<SetPosition>("R").y = bounds.center.y;

        //    attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
        //    attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
        //    attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

        //    control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
        //    control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
        //    control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
        //    control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
        //    control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
        //    control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

        //    AddResetToStateOnHide(control, "Init");

        //    var climbPlatsState = control.GetState("Climb Plats1");
        //    climbPlatsState.Actions = new FsmStateAction[] {
        //        new CustomFsmAction(() => Destroy(gameObject))
        //    };

        //    if (!HeroInAggroRange())
        //        Hide();

        //    yield return UpdateAggroRange();
        //}

        //protected override bool HeroInAggroRange()
        //{
        //    var size = new Vector2(30f, 30f);
        //    var center = new Vector2(transform.position.x, transform.position.y);
        //    var herop = new Vector2(HeroX, HeroY);
        //    var dist = herop - center;
        //    return (dist.sqrMagnitude < size.sqrMagnitude);
        //}

        //protected override void Show()
        //{
        //    base.Show();

        //    if (control.ActiveStateName == "Hidden")
        //    {
        //        control.SendEvent("SHOW");
        //        attackCommands.SetState("Init");
        //    }
        //}
    }

    public class RadianceSpawner : DefaultSpawner<RadianceControl>
    {
    }

    public class RadiancePrefabConfig : DefaultPrefabConfig
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///


}




//        protected override void SetupCustomDebugArea()
//        {
//#if DEBUG

//            //radius
//            debugColliders.customLineCollections.Add(Color.red,
//                DebugColliders.GetPointsFromCollider(Vector2.one, centerOfAggroArea, sizeOfAggroArea.magnitude).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

//            //distance
//            debugColliders.customLineCollections.Add(Color.magenta, new List<Vector3>() {
//            heroPos2d, pos2d, heroPos2d
//            });

//            //bounds
//            debugColliders.customLineCollections.Add(Color.blue, debugColliders.GetPointsFromCollider(aggroBounds, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

//            Vector2 min = new Vector2(edgeL, floorY);
//            Vector2 max = new Vector2(edgeR, roofY);
//            var rect = new Rect();
//            rect = rect.SetMinMax(min, max);

//            //arena bounds
//            debugColliders.customLineCollections.Add(new Color(255, 255, 0), debugColliders.GetPointsFromCollider(rect, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

//            var down = heroPos2d.FireRayGlobal(Vector2.down, 50f).point;
//            var up = heroPos2d.FireRayGlobal(Vector2.up, 200f).point;
//            var left = heroPos2d.FireRayGlobal(Vector2.left, 100f).point;
//            var right = heroPos2d.FireRayGlobal(Vector2.right, 100f).point;

//            //floory
//            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
//            down, pos2d, down
//            });

//            //roofy
//            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
//            up, pos2d, up
//            });

//            //left
//            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
//            left, pos2d, left
//            });

//            //right
//            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
//            right, pos2d, right
//            });
//#endif
//        }




//var selectedSpawn = possibleSpawns.GetRandomElementFromList(rng);
//var thingToSpawn = EnemyRandomizerDatabase.GetDatabase().Spawn(selectedSpawn.Item1);
//jar.SetEnemySpawn(selectedSpawn.Item1.prefab, selectedSpawn.Item2);
//var spawner = jar.gameObject.GetOrAddComponent<SpawnEffectOnDestroy>();
//spawner.allowRandomization = true;
//spawner.isSpawnerEnemy = true;
//spawner.effectToSpawn = null;
//spawner.activateOnSpawn = false;
//spawner.onSpawn = OnSpawned;

//var smashEffect = jar.gameObject.AddComponent<SpawnEffectOnDestroy>();
//smashEffect.effectToSpawn = "Pt Break";

//spawner.effectToSpawn = selectedSpawn.Item1.prefab.name;
//spawner.setHealthOnSpawn = selectedSpawn.Item2;







//var whiteScreenEffectfsm = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid");
//if (whiteScreenEffectfsm != null)
//{
//    var whiteScreenEffect = whiteScreenEffectfsm.gameObject;
//    if (whiteScreenEffect != null)
//    {
//        Destroy(whiteScreenEffect);
//    }
//}
//else
//{
//    Dev.LogError("Could not find white screen child object on Zote Boss!");
//}

//var corpse = gameObject.GetCorpseObject();
//if (corpse != null)
//{
//    var white2 = corpse.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid");
//    if (white2 != null && white2.gameObject != null)
//        Destroy(white2.gameObject);

//    var corpseFSM = corpse.LocateMyFSM("Control");
//    if (corpseFSM != null)
//    {
//        var init = corpseFSM.GetState("Init");
//        init.DisableActions(0, 1, 7, 8, 9, 10, 11, 15);

//        var inAir = corpseFSM.GetState("In Air");
//        inAir.DisableActions(0);
//        corpseFSM.AddTimeoutAction(inAir, "LAND", 1f);
//        //inAir.AddTimeoutAction("LAND", 1f);

//        var burst = corpseFSM.GetState("Burst");
//        burst.DisableAction(5);
//        burst.ChangeTransition("FINISHED", "End");

//        var notify = corpseFSM.GetState("Notify");
//        notify.DisableAction(0);
//        notify.AddCustomAction(() => { control.SendEvent("CORPSE END"); });

//        var end = corpseFSM.GetState("End");
//        end.DisableActions(0, 1, 2, 3);

//        var land = corpseFSM.GetState("Land");
//        land.DisableActions(0, 3, 4, 5, 6, 7, 13);
//    }
//    else
//    {
//        Dev.LogError("corpseFSM not found in Zote Boss");
//    }
//}
//else
//{
//    Debug.LogError("Failed to find corpse on Zote boss!");
//}



//public override void SetupPrefab(PrefabObject p)
//{
//    base.SetupPrefab(p);

//    var Prefab = p.prefab;

//    Dev.Log("getting death effects");
//    var deathEffects = Prefab.GetComponentInChildren<EnemyDeathEffectsUninfected>(true);
//    //var baseDeathEffects = deathEffects as EnemyDeathEffects;

//    deathEffects.doKillFreeze = false;


//    var corpsePrefab = (GameObject)deathEffects.GetType().BaseType.GetField("corpsePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);

//    if (corpsePrefab == null)
//        Dev.LogError("Failed to find corpse prefab zote boss");
//    else
//    {
//        corpsePrefab.AddComponent<GGZoteCorpseFixer>();
//    }
//}