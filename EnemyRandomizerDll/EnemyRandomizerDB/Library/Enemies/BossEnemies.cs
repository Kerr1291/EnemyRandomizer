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
    public class MantisTraitorLordControl : FSMBossAreaControl
    {
        public override string FSMName => "Mantis";

        public GameObject megaMantisTallSlash;
        public PreventOutOfBounds poob;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //enable this after activating/repositioning traitor lord
            poob = gameObject.AddComponent<PreventOutOfBounds>();
            poob.enabled = false;

            this.InsertHiddenState(control, "Init", "FINISHED", "Fall");

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
            if(thisMetadata.EnemyHealthManager.hp <= 0)
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

        protected override void Show()
        {
            base.Show();
            poob.enabled = true;
        }

        protected override void SetCustomPositionOnShow()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class MantisTraitorLordSpawner : DefaultSpawner<MantisTraitorLordControl> { }

    public class MantisTraitorLordPrefabConfig : DefaultPrefabConfig<MantisTraitorLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MimicSpiderControl : FSMBossAreaControl
    {
        public override string FSMName => "Mimic Spider";
        public bool skipToIdle = false;

        public override void Setup(ObjectMetadata other)
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
            trans1.DisableAction(11);
            trans1.GetAction<Wait>(12).time = 0.1f;
            trans1.DisableAction(14);
            trans1.InsertCustomAction(() => {
                if (skipToIdle)
                    control.SetState("Idle");
            }, 0);

            var trans2 = control.GetState("Trans 2");
            trans1.GetAction<Wait>(2).time = 0.1f;

            var trans3 = control.GetState("Trans 3");
            trans1.GetAction<Wait>(2).time = 0.1f;

            var trans4 = control.GetState("Trans 4");
            trans1.GetAction<Wait>(1).time = 0.1f;

            var trans5 = control.GetState("Trans 5");
            trans1.GetAction<Wait>(1).time = 0.1f;

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
            this.OverrideState(control, "Encountered", () => { control.SetState("Init"); });

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
    }

    public class MimicSpiderSpawner : DefaultSpawner<MimicSpiderControl> { }

    public class MimicSpiderPrefabConfig : DefaultPrefabConfig<MimicSpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////













    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class LostKinControl : FSMBossAreaControl
    {
        public PlayMakerFSM balloonFSM;

        public override string FSMName => "IK Control";

        public override void Setup(ObjectMetadata other)
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

            this.OverrideState(balloonFSM, "Spawn", () =>
            {
                if (children.Count >= maxBabies)
                {
                    balloonFSM.SendEvent("STOP SPAWN");
                }

                if(CanEnemySeePlayer())
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
                        SpawnAndTrackChild("Parasite Balloon", spawnPoint);
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


    public class LostKinPrefabConfig : DefaultPrefabConfig<LostKinControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordPhase2Control : FSMBossAreaControl
    {
        public override string FSMName => "Mage Lord 2";

        protected override bool ControlCameraLocks => true;

        public override void Setup(ObjectMetadata other)
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

            var shiftq = control.GetState("Shift?");
            shiftq.DisableAction(1);
            shiftq.DisableAction(2);
            shiftq.DisableAction(3);

            var teleportQ = control.GetState("TeleportQ");
            teleportQ.DisableAction(0);
            teleportQ.DisableAction(1);
            teleportQ.DisableAction(2);
            teleportQ.DisableAction(3);
            teleportQ.DisableAction(7);
            teleportQ.InsertCustomAction(() => {
                var aboveHero = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.up, 20f);
                var telePoint = heroPosWithOffset;
                if (aboveHero.collider != null && aboveHero.distance > 2f)
                {
                    float distFromHero = (aboveHero.distance - 2f);
                    if (distFromHero < 6f)
                        distFromHero = 6;

                    telePoint = telePoint + Vector2.up * distFromHero;
                }
                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = transform.position;
            },0);

            var quakeLand = control.GetState("Quake Land");
            quakeLand.DisableAction(1);
            quakeLand.DisableAction(9);
            quakeLand.InsertCustomAction(() =>
            {
                gameObject.StickToGround();
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

                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 12f);
                Vector2 telePoint;

                if (spawnRay.distance > 2f)
                {
                    telePoint = spawnRay.point - spawnRay.normal * 2f;
                }
                else
                {
                    telePoint = spawnRay.point;
                }

                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;
            }, 0);

            this.OverrideState(control, "Fireball Pos", () => { control.SendEvent("FINISHED"); });

            var orbSummon = control.GetState("Orb Summon");
            orbSummon.DisableAction(0);
            orbSummon.InsertCustomAction(() => {

                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 6f);
                Vector2 orbSpawnPoint;

                if (spawnRay.distance > 2f)
                {
                    orbSpawnPoint = spawnRay.point - spawnRay.normal;
                }
                else
                {
                    orbSpawnPoint = spawnRay.point;
                }

                control.FsmVariables.GetFsmVector3("Fireball Pos").Value = orbSpawnPoint;
            }, 0);

            var spawnFireball = control.GetState("Spawn Fireball");
            spawnFireball.DisableAction(0);

            var teleOut = control.GetState("Tele Out");
            teleOut.DisableAction(3);
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class MageLordPhase2Spawner : DefaultSpawner<MageLordPhase2Control> { }

    public class MageLordPhase2PrefabConfig : DefaultPrefabConfig<MageLordPhase2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordControl : FSMBossAreaControl
    {
        public override string FSMName => "Mage Lord";

        protected override bool ControlCameraLocks => true;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
              
            var destroyIfDefeated = gameObject.LocateMyFSM("Destroy If Defeated");
            if(destroyIfDefeated != null)
                GameObject.Destroy(destroyIfDefeated);

            try
            {
                var init = control.GetState("Init");
                init.DisableAction(1);
                init.DisableAction(4);
                init.DisableAction(5);
                init.AddCustomAction(() => { control.SendEvent("FINISHED"); });
            } catch (Exception e) { Dev.LogError($"ERROR CONFIGURING INIT -- {e.Message} {e.StackTrace}"); }

            try
            { 
            var sleep = control.GetState("Sleep");
            sleep.DisableAction(0);
            //sleep.AddCustomAction(() => { control.SendEvent("WAKE"); });
            //sleep.ChangeTransition("FINISHED", "Teleport In");

            this.InsertHiddenState(control, "Init", "FINISHED", "Teleport In");

            var teleportIn = control.GetState("Teleport In");
            teleportIn.DisableAction(3);
            teleportIn.DisableAction(4);
            teleportIn.DisableAction(5);
            teleportIn.DisableAction(6);
            teleportIn.DisableAction(7);
            teleportIn.InsertCustomAction(() => {

                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 12f);
                Vector2 telePoint;

                if (spawnRay.distance > 2f)
                {
                    telePoint = spawnRay.point - spawnRay.normal * 2f;
                }
                else
                {
                    telePoint = spawnRay.point;
                }

                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;
            }, 0);
            teleportIn.ChangeTransition("FINISHED", "Set Idle Timer");
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING SLEEP TELE IN -- {e.Message} {e.StackTrace}"); }

            try { 
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
            teleport.DisableAction(11);
            teleport.InsertCustomAction(() => {

                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(gameObject, randomDir, 12f);
                Vector2 telePoint;

                if (spawnRay.distance > 2f)
                {
                    telePoint = spawnRay.point - spawnRay.normal * 2f;
                }
                else
                {
                    telePoint = spawnRay.point;
                }

                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = telePoint;
            }, 0);
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING TELE AWAY VALID TELE -- {e.Message} {e.StackTrace}"); }
            var shot = control.GetState("Shot");
            shot.DisableAction(2);

            try
            { 
            var teleQuake = control.GetState("Tele Quake");
            teleQuake.DisableAction(2);
            teleQuake.DisableAction(3);
            teleQuake.DisableAction(4);

            var teleportQ = control.GetState("TeleportQ");
            teleportQ.DisableAction(0);
            teleportQ.DisableAction(3);
            teleportQ.DisableAction(4);
            teleportQ.DisableAction(5);
            teleportQ.DisableAction(9);
            teleportQ.InsertCustomAction(() => {
                var telePoint = heroPos2d + SpawnerExtensions.GetRayOn(heroPos2d, Vector2.up, 10f).point;
                control.FsmVariables.GetFsmVector3("Self Pos").Value = transform.position;
                transform.position = telePoint;
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = transform.position;
            }, 0);

            var quakeDown = control.GetState("Quake Down");
            quakeDown.DisableAction(6);
            quakeDown.DisableAction(7);
            quakeDown.AddCustomAction(() =>
            {
                StartCoroutine(SendFinishedOnGroundOrDelay());
            });

            var quakeLand = control.GetState("Quake Land");
            quakeLand.DisableAction(3);
            quakeLand.DisableAction(13);
            quakeLand.InsertCustomAction(() =>
            {
                gameObject.StickToGround();
            }, 0);

            var teleOut = control.GetState("HS Tele Out");
            teleOut.DisableAction(2);
            }
            catch (Exception e) { Dev.LogError($"ERROR CONFIGURING QUAKE Q DOWN LAND -- {e.Message} {e.StackTrace}"); }
        }

        IEnumerator SendFinishedOnGroundOrDelay()
        {
            float timeout = 2f;
            float toGround = 1f;
            var rayToGround = SpawnerExtensions.GetGroundRay(gameObject);
            if (rayToGround.distance >= 1f)
            {
                for (; ; )
                {
                    if (timeout <= 0)
                        break;

                    rayToGround = SpawnerExtensions.GetGroundRay(gameObject);
                    if (rayToGround.distance < toGround)
                        break;

                    timeout -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (control.ActiveStateName == "Quake Down")
                control.SendEvent("FINISHED");

            yield break;
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class MageLordSpawner : DefaultSpawner<MageLordControl> { }

    public class MageLordPrefabConfig : DefaultPrefabConfig<MageLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class WhiteDefenderControl : FSMBossAreaControl
    {
        public override string FSMName => "Dung Defender";

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

        public override void Setup(ObjectMetadata other)
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

            defaultHP = thisMetadata.DefaultHP;
            rageTriggerRatio = defaultHP / defaultRageHP;

            this.InsertHiddenState(control, "Init 2", "FINISHED", "Wake");

            var init = control.GetState("Init");
            DisableActions(init, 1, 2);

            var wake = control.GetState("Wake");
            DisableActions(wake, 1, 7, 9);
            wake.InsertCustomAction(() => {
                PositionBoss(.3f);
                burrowEffect.gameObject.StickToGroundX(-.5f);
                burrowEffect.transform.position -= new Vector3(0f, 0.2f * thisMetadata.SizeScale, 0f);
                PlayBurrowEffect();
                //gameObject.transform.position -= new Vector3(0f, -2f * thisMetadata.SizeScale, 0f);
            }, 5);

            var eruptOutFirst2 = control.GetState("Erupt Out First 2");
            DisableActions(eruptOutFirst2, 0, 1, 6, 8, 11, 16);
            eruptOutFirst2.InsertCustomAction(() => CalculateAndSetupErupt(), 8);
            eruptOutFirst2.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First 2", "END")); });

            var introFall = control.GetState("Intro Fall");
            DisableActions(introFall, 0);
            introFall.AddCustomAction(() => { StartCoroutine(TimeoutState("Intro Fall", "LAND", 2f)); });

            var introLand = control.GetState("Intro Land");
            introLand.InsertCustomAction(() => { UpdateBurrowEffect(); }, 0);

            var introRoar = control.GetState("Intro Roar");
            DisableActions(introRoar, 1,2,6,7,12,13,14,15,16,17,18);

            var music = control.GetState("Music");
            DisableActions(music, 0, 1, 2);

            var roarEnd = control.GetState("Roar End");
            DisableActions(roarEnd, 0, 1);

            var rageRoar = control.GetState("Rage Roar");
            DisableActions(rageRoar, 2, 3, 4, 5);

            var roarq = control.GetState("Roar?");
            DisableActions(roarq, 5, 6, 10, 11, 12, 13, 14, 15, 16);

            var rageIn = control.GetState("Rage In");
            DisableActions(rageIn, 3, 7);

            var diveIn2 = control.GetState("Dive In 2");
            DisableActions(diveIn2, 3);

            var underground = control.GetState("Underground");
            DisableActions(underground, 0);
            underground.InsertCustomAction(() => CalculateAndSetupBuried(), 0);

            var tunnelingL = control.GetState("Tunneling L");
            DisableActions(tunnelingL, 4, 7, 8, 9);
            tunnelingL.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling L", true)));

            var tunnelingR = control.GetState("Tunneling R");
            DisableActions(tunnelingR, 0, 4, 7, 8, 9);
            tunnelingR.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling R", false)));

            var eruptAntic = control.GetState("Erupt Antic");
            DisableActions(eruptAntic, 3);

            var eruptAnticR = control.GetState("Erupt Antic R");
            DisableActions(eruptAnticR, 2);

            var eruptOutFirst = control.GetState("Erupt Out First");
            DisableActions(eruptOutFirst, 1,3,6,10);
            eruptOutFirst.InsertCustomAction(() => CalculateAndSetupErupt(), 3);
            eruptOutFirst.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First", "END")); });

            var eruptOut = control.GetState("Erupt Out");
            DisableActions(eruptOut, 2, 5, 9, 14);
            eruptOut.InsertCustomAction(() => CalculateAndSetupErupt(), 5);
            eruptOut.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out", "END")); });

            var eruptFall = control.GetState("Erupt Fall");
            DisableActions(eruptFall, 0);
            eruptFall.AddCustomAction(() => { StartCoroutine(TimeoutState("Erupt Fall", "LAND", 2f)); });

            var eruptLand = control.GetState("Erupt Land");
            DisableActions(eruptLand, 4);
            eruptLand.InsertCustomAction(() => { gameObject.transform.position = eruptOrigin; }, 0);

            var throw1 = control.GetState("Throw 1");
            pooBallW = throw1.GetAction<SpawnObjectFromGlobalPool>(1).gameObject.Value;
            DisableActions(throw1, 1);
            throw1.AddCustomAction(() => {
                pooThrowOffset = 1.5f * Vector2.right * (transform.localScale.x < 0 ? -1f : 1f);
                var dungBall = pooBallW.Spawn(transform.position + pooThrowOffset, Quaternion.identity);
                //var dungBall = EnemyRandomizerDatabase.GetDatabase().Spawn("Dung Ball Large", null);
                control.FsmVariables.GetFsmGameObject("Dung Ball").Value = dungBall;
                //dungBall.transform.position = transform.position;
                dungBall.SafeSetActive(true);
            });

            var rjInAir = control.GetState("RJ In Air");
            DisableActions(rjInAir, 7);
            rjInAir.InsertCustomAction(() => StartCoroutine(CalculateAirDiveHeight()), 7);

            var airDive = control.GetState("Air Dive");
            DisableActions(airDive, 8);
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

            this.OverrideState(control, "Set Min X", () => { control.SendEvent("FINISHED"); });
            this.OverrideState(control, "Set Max X", () => { control.SendEvent("FINISHED"); });


            var rageq = control.GetState("Rage?");
            this.OverrideState(control, "Rage?", () => {
                if (control.FsmVariables.GetFsmBool("Raged").Value)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    if (thisMetadata.EnemyHealthManager.hp / thisMetadata.DefaultHP < rageTriggerRatio)
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
            this.OverrideState(control, "Ground Slam?", () => {

                var left = SpawnerExtensions.GetRayOn(pos2d + Vector2.up, Vector2.left, float.MaxValue);
                var right = SpawnerExtensions.GetRayOn(pos2d + Vector2.up, Vector2.right, float.MaxValue);

                bool isHeroLeft = heroPos2d.x < pos2d.x;

                if (DistanceToPlayer() > tooFarToSlamDist)
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
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();

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

            Destroy(poob);

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
                if (hitRay.collider != null || pos2d.y >= eruptMax || thisMetadata.PhysicsBody.velocity.y < 0)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        //protected virtual IEnumerator TimeoutState(string currentState, string endEvent, float timeout)
        //{
        //    while (control.ActiveStateName == currentState)
        //    {
        //        timeout -= Time.deltaTime;

        //        if(timeout <= 0f)
        //        {
        //            control.SendEvent(endEvent);
        //            break;
        //        }
        //        yield return new WaitForEndOfFrame();
        //    }

        //    yield break;
        //}

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
                else if(DistanceToPlayer() > tunnelTurnTowardPlayerDist)
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

        protected override void SetCustomPositionOnShow()
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
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var wd = base.Spawn(p, source);
            GameObject corpse = SpawnerExtensions.GetCorpseObject(wd);
            if (corpse != null)
            {
                SpawnerExtensions.AddCorpseRemoverWithEffect(corpse);
            }
            return wd;
        }
    }

    public class WhiteDefenderPrefabConfig : DefaultPrefabConfig<WhiteDefenderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




















    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HiveKnightControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        int p1HP;
        int p2HP => (int)(p1HP * .4f);
        int p3HP => (int)(p2HP * .6f);

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (thisMetadata.Corpse != null)
            {
                var corpseRemover = thisMetadata.Corpse.AddComponent<CorpseRemover>();
                if (corpseRemover != null)
                    corpseRemover.replacementEffect = "Death Explode Boss";
            }

            if (p1HP <= 0)
                p1HP = thisMetadata.CurrentHP;

            var roar = control.GetState("Roar");
            roar.AddCustomAction(() => {
                var bee = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position + Vector3.up, "Bee Dropper", null, true);
                var beeBody = bee.GetComponent<Rigidbody2D>();
                if(beeBody != null)
                {
                    beeBody.velocity = UnityEngine.Random.insideUnitCircle * 5f;
                    if (beeBody.velocity.y < 0) { beeBody.velocity = new Vector2(beeBody.velocity.x, -beeBody.velocity.y); }
                }
            });
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
            roar.AddCustomAction(() => { control.SendEvent("FINISHED"); });


            var aimL = control.GetState("Aim L");
            aimL.DisableAction(3);
            aimL.AddCustomAction(() => {
                if (SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.right, 10f).distance < 8f)
                    control.SendEvent("R");
                else
                    control.SendEvent("FINISHED");
            });

            var aimR = control.GetState("Aim R");
            aimR.DisableAction(3);
            aimR.AddCustomAction(() => {
                if (SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, 10f).distance < 8f)
                    control.SendEvent("L");
                else
                    control.SendEvent("FINISHED");
            });

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
        }

        protected override void ScaleHP()
        {
            base.ScaleHP();
            p1HP = thisMetadata.CurrentHP;
        }
    }

    public class HiveKnightSpawner : DefaultSpawner<HiveKnightControl> { }

    public class HiveKnightPrefabConfig : DefaultPrefabConfig<HiveKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrimmBossControl : FSMBossAreaControl
    {
        public override bool explodeOnDeath => true;

        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if(corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            if (thisMetadata.CurrentHP <= 0)
                thisMetadata.CurrentHP = 800;

            var cx = gameObject.LocateMyFSM("constrain_x");
            if (cx != null)
                Destroy(cx);
            var cy = gameObject.LocateMyFSM("Constrain_Y");
            if (cy != null)
                Destroy(cy);


            //control.ChangeTransition("Init", "FINISHED", "GG Bow");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Explode", "FINISHED", "Send Death Event");

            var balloonPos = control.GetState("Balloon Pos");
            balloonPos.DisableAction(0);
            balloonPos.InsertCustomAction(() =>
            {
                var pos = GetRandomPositionInLOSofPlayer(1f, 30f, 2f, 6f);
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
            AddTimeoutAction(adFire, "LAND", 1f);

            var adEdge = control.GetState("AD Edge");
            AddTimeoutAction(adFire, "LAND", 1f);

            this.InsertHiddenState(control, "Init", "FINISHED", "GG Bow");

            var ggbow = control.GetState("GG Bow");
            ggbow.ChangeTransition("FINISHED", "Tele Out");
            ggbow.ChangeTransition("TOOK DAMAGE", "Tele Out");
        }
    }

    public class GrimmBossSpawner : DefaultSpawner<GrimmBossControl> { }

    public class GrimmBossPrefabConfig : DefaultPrefabConfig<GrimmBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class NightmareGrimmBossControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            if (thisMetadata.CurrentHP <= 0)
                thisMetadata.CurrentHP = 800;

            var cx = gameObject.LocateMyFSM("constrain_x");
            if (cx != null)
                Destroy(cx);
            var cy = gameObject.LocateMyFSM("Constrain_Y");
            if (cy != null)
                Destroy(cy);

            //control.ChangeTransition("Set Balloon HP", "FINISHED", "Tele Out");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Start", "FINISHED", "Death Explode");

            var deathStart = control.GetState("Death Start");
            DisableActions(deathStart, 6, 7, 20);

            var deathExplode = control.GetState("Death Start");
            DisableActions(deathExplode, 2, 4);

            OverrideState(control, "Send NPC Event", () =>{ Destroy(gameObject); });

            var balloonPos = control.GetState("Balloon Pos");
            balloonPos.DisableAction(0);
            balloonPos.InsertCustomAction(() =>
            {
                var pos = GetRandomPositionInLOSofPlayer(1f, 30f, 2f, 6f);
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
            AddTimeoutAction(adFire, "LAND", 1f);

            var adEdge = control.GetState("AD Edge");
            AddTimeoutAction(adFire, "LAND", 1f);

            this.InsertHiddenState(control, "Init", "FINISHED", "Tele Out");
        }
    }

    public class NightmareGrimmBossSpawner : DefaultSpawner<NightmareGrimmBossControl> { }

    public class NightmareGrimmBossPrefabConfig : DefaultPrefabConfig<NightmareGrimmBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HollowKnightBossControl : FSMBossAreaControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var phaseControl = gameObject.LocateMyFSM("Phase Control");
            if (phaseControl != null)
                Destroy(phaseControl);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            //TEMP
            this.OverrideState(control, "Long Roar End", () => Destroy(gameObject));

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

    public class HollowKnightBossPrefabConfig : DefaultPrefabConfig<HollowKnightBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HKPrimeControl : FSMBossAreaControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
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
            OverrideState(control, "Phase ?", () => {
                if (thisMetadata.CurrentHP > thisMetadata.DefaultHP / 2)
                    control.SendEvent("PHASE1");
                else if (thisMetadata.CurrentHP > thisMetadata.DefaultHP / 4)
                    control.SendEvent("PHASE2");
                else //if (thisMetadata.CurrentHP > thisMetadata.DefaultHP / 2)
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

                    telePos = new Vector2(teleXRayRight.point.x, groundNearTele.point.y);
                }
                else
                {
                    //go left
                    var teleXRayLeft = SpawnerExtensions.GetRayOn(heroPosWithOffset, Vector2.left, teleX);

                    var groundNearTele = SpawnerExtensions.GetRayOn(teleXRayLeft.point, Vector2.down, 50f);

                    telePos = new Vector2(teleXRayLeft.point.x, groundNearTele.point.y);
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
            AddTimeoutAction(stunAir, "LAND", 2f);

            var dstabAir = control.GetState("Dstab Air");
            AddTimeoutAction(dstabAir, "LAND", 2f);

            var stompDown = control.GetState("Stomp Down");
            AddTimeoutAction(stompDown, "LAND", 2f);

            var inAir = control.GetState("In Air");
            AddTimeoutAction(inAir, "LAND", 2f);
        }
    }

    public class HKPrimeSpawner : DefaultSpawner<HKPrimeControl> { }

    public class HKPrimePrefabConfig : DefaultPrefabConfig<HKPrimeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PaleLurkerControl : FSMBossAreaControl
    {
        public override string FSMName => "Lurker Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var hpscaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpscaler != null)
                Destroy(hpscaler);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            //TODO: make lurker balls killable

            this.InsertHiddenState(control, "Init", "FINISHED", "Get High");
        }
    }

    public class PaleLurkerSpawner : DefaultSpawner<PaleLurkerControl> { }

    public class PaleLurkerPrefabConfig : DefaultPrefabConfig<PaleLurkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class OroControl : FSMBossAreaControl
    {
        public override string FSMName => "nailmaster";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            control.ChangeTransition("Death Start", "FINISHED", "Explode");

            var explode = control.GetState("Explode");
            explode.InsertCustomAction(() => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
        }
    }

    public class OroSpawner : DefaultSpawner<OroControl> { }

    public class OroPrefabConfig : DefaultPrefabConfig<OroControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MatoControl : FSMBossAreaControl
    {
        public override string FSMName => "nailmaster";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");

            control.ChangeTransition("Death Start", "FINISHED", "Explode");

            var explode = control.GetState("Explode");
            explode.InsertCustomAction(() => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);
        }
    }

    public class MatoSpawner : DefaultSpawner<MatoControl> { }

    public class MatoPrefabConfig : DefaultPrefabConfig<MatoControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SheoBossControl : FSMBossAreaControl
    {
        public override string FSMName => "nailmaster_sheo";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            }, 0);

            this.InsertHiddenState(control, "Set Paint HP", "FINISHED", "Idle");
        }
    }

    public class SheoBossSpawner : DefaultSpawner<SheoBossControl> { }

    public class SheoBossPrefabConfig : DefaultPrefabConfig<SheoBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlyBossControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            var explode = control.GetState("Explosion");
            OverrideState(control, "Explosion", () => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            });
            
            this.InsertHiddenState(control, "Phase HP", "FINISHED", "Idle");
            control.ChangeTransition("Death Reset", "FINISHED", "Explosion");
        }
    }

    public class SlyBossSpawner : DefaultSpawner<SlyBossControl> { }

    public class SlyBossPrefabConfig : DefaultPrefabConfig<SlyBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetNoskControl : FSMBossAreaControl
    {
        public override string FSMName => "Hornet Nosk";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
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

            this.InsertHiddenState(control, "HP", "FINISHED", "Idle");
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class HornetNoskSpawner : DefaultSpawner<HornetNoskControl> { }

    public class HornetNoskPrefabConfig : DefaultPrefabConfig<HornetNoskControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DungDefenderControl : FSMBossAreaControl
    {
        public override string FSMName => "Dung Defender";

        public float peakEruptHeight = 12f;
        public float eruptStartOffset = 3f;
        public float defaultHP;
        public float defaultRageHP = 600f;
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
        protected PlayMakerFSM burrowEffect;
        protected PlayMakerFSM keepY;
        protected Vector3 pooThrowOffset;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            defaultHP = thisMetadata.DefaultHP;
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
            DisableActions(wake, 6);
            wake.InsertCustomAction(() => {
                PositionBoss(.3f);
                burrowEffect.gameObject.StickToGroundX(-.5f);
                burrowEffect.transform.position -= new Vector3(0f, 1f * thisMetadata.SizeScale, 0f);
            }, 2);

            var quakedOut = control.GetState("Quaked Out");
            DisableActions(quakedOut, 1, 4, 9, 11);
            quakedOut.InsertCustomAction(() => CalculateAndSetupErupt(), 1);

            var rageq = control.GetState("Rage?");
            this.OverrideState(control, "Rage?", () => {
                if (control.FsmVariables.GetFsmBool("Raged").Value)
                {
                    control.SendEvent("FINISHED");
                }
                else
                {
                    if (thisMetadata.EnemyHealthManager.hp / thisMetadata.DefaultHP < rageTriggerRatio)
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
            DisableActions(roarEnd, 3,4,5);

            var rageRoar = control.GetState("Rage Roar");
            DisableActions(rageRoar, 2, 3);

            var diveIn2 = control.GetState("Dive In 2");
            DisableActions(diveIn2, 3);

            var underground = control.GetState("Underground");
            DisableActions(underground, 0);
            underground.InsertCustomAction(() => CalculateAndSetupBuried(), 0);

            var rageIn = control.GetState("Rage In");
            DisableActions(rageIn, 2, 6);

            var tunnelingL = control.GetState("Tunneling L");
            DisableActions(tunnelingL, 3, 4, 7, 8, 9);
            tunnelingL.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling L", true)));

            var tunnelingR = control.GetState("Tunneling R");
            DisableActions(tunnelingL, 0, 4, 7, 8, 9);
            tunnelingR.AddCustomAction(() => StartCoroutine(StartAndCheckTunnelingState("Tunneling R", false)));

            var eruptAntic = control.GetState("Erupt Antic");
            DisableActions(eruptAntic, 3);

            var eruptAnticR = control.GetState("Erupt Antic R");
            DisableActions(eruptAnticR, 2);

            var eruptOutFirst = control.GetState("Erupt Out First");
            DisableActions(eruptOutFirst, 1, 3, 6, 11);
            eruptOutFirst.InsertCustomAction(() => CalculateAndSetupErupt(), 3);
            eruptOutFirst.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out First", "END")); });

            var eruptOut = control.GetState("Erupt Out");
            DisableActions(eruptOut, 2, 5, 9, 14);
            eruptOut.InsertCustomAction(() => CalculateAndSetupErupt(), 5);
            eruptOut.AddCustomAction(() => { StartCoroutine(StartAndCheckEruptPeak("Erupt Out", "END")); });

            var eruptFall = control.GetState("Erupt Fall");
            DisableActions(eruptFall, 0);
            eruptFall.AddCustomAction(() => { StartCoroutine(TimeoutState("Erupt Fall", "LAND", 2f)); });

            var eruptLand = control.GetState("Erupt Land");
            DisableActions(eruptLand, 4);

            var roarq = control.GetState("Roar?");
            DisableActions(roarq, 4, 5, 11, 12, 13, 14, 15);

            var throw1 = control.GetState("Throw 1");
            DisableActions(throw1, 1);
            throw1.AddCustomAction(() => {
                pooThrowOffset = 1.5f * Vector2.right * (transform.localScale.x < 0 ? -1f : 1f);
                var dungBall = EnemyRandomizerDatabase.GetDatabase().Spawn("Dung Ball Large", null);
                control.FsmVariables.GetFsmGameObject("Dung Ball").Value = dungBall;
                dungBall.SafeSetActive(true);
            });

            var dolphDir = control.GetState("Dolph Dir");
            dolphDir.DisableAction(5);
            dolphDir.InsertCustomAction(() => {
                if (pos2d.x < heroPos2d.x)
                    control.SendEvent("FINISHED");
            }, 5);

            this.OverrideState(control, "Set Min X", () => { control.SendEvent("FINISHED"); });
            this.OverrideState(control, "Set Max X", () => { control.SendEvent("FINISHED"); });
        }

        protected virtual IEnumerator CheckEndAirDive()
        {
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();

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

            Destroy(poob);

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
                if (hitRay.collider != null || pos2d.y >= eruptMax || thisMetadata.PhysicsBody.velocity.y < 0)
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
                else if (DistanceToPlayer() > tunnelTurnTowardPlayerDist)
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


        protected override void SetCustomPositionOnShow()
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
    }

    public class DungDefenderSpawner : DefaultSpawner<DungDefenderControl> { }

    public class DungDefenderPrefabConfig : DefaultPrefabConfig<DungDefenderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorGalienControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;

            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }

            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 0.3f, 7f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(ObjectMetadata other)
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
            //    var attack = control.GetState("Warp In");
            //    attack.InsertCustomAction(() => {

            //        var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            //        EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            //    }, 0);

            //    OverrideState(control, "Hover", () => { 
            //    });
            //    control.GetState("Hover").AddAction(new Wait() { time = 10f });
            //}

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");

            var summon = gameObject.LocateMyFSM("Summon Minis");
            var summona1 = summon.GetState("Summon Antic");
            summona1.InsertCustomAction(() => {
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            },0);
            TimeoutState(summon, "Summon Antic", "SUMMON", 1f);

            var summona2 = summon.GetState("Summon Antic 2");
            summona2.InsertCustomAction(() => {
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            }, 0);
            TimeoutState(summon, "Summon Antic 2", "SUMMON", 1f);
        }

        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorGalienSpawner : DefaultSpawner<GhostWarriorGalienControl> { }

    public class GhostWarriorGalienPrefabConfig : DefaultPrefabConfig<GhostWarriorGalienControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorXeroControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        public GameObject sword1;
        public GameObject sword2;
        public GameObject sword3;
        public GameObject sword4;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 10f, 9f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            sword1 = thisMetadata.DB.Spawn("Sword 1", null);
            sword2 = thisMetadata.DB.Spawn("Sword 2", null);
            sword3 = thisMetadata.DB.Spawn("Sword 3", null);
            sword4 = thisMetadata.DB.Spawn("Sword 4", null);

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
                //OverrideState(control, "Hover", () => {
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
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorXeroSpawner : DefaultSpawner<GhostWarriorXeroControl> { }

    public class GhostWarriorXeroPrefabConfig : DefaultPrefabConfig<GhostWarriorXeroControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorHuControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";
        public virtual string ATTACKFSM => "Attacking";

        public GameObject ringHolder;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 0.3f, 8f, 2f);
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
                var spos = down.GetFirstActionOfType<SetPosition>();
                spos.y = downRay.point.y;

                var reset = fsm.GetState("Reset");
                var spos2 = down.GetFirstActionOfType<SetPosition>();
                spos2.y = ringRootPos.y;
            }
        }

        IEnumerator chasing;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            ringHolder = thisMetadata.DB.Spawn("Ring Holder", null);            
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
                //OverrideState(control, "Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var attacking = gameObject.LocateMyFSM("Attacking");
                var init = attacking.GetState("Init");
                init.AddCustomAction(() => {
                    attacking.FsmVariables.GetFsmGameObject("Ring Holder").Value = ringHolder;
                });

                var placeRings = attacking.GetState("Place Rings");
                placeRings.AddCustomAction(() => {
                    SetRingPositions(heroPos2d);
                });

                attacking.ChangeTransition("Choice 2", "MEGA", "Choice");
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorHuSpawner : DefaultSpawner<GhostWarriorHuControl> { }

    public class GhostWarriorHuPrefabConfig : DefaultPrefabConfig<GhostWarriorHuControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorSlugControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 0.3f, 5f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(ObjectMetadata other)
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
                //OverrideState(control, "Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var attacking = gameObject.LocateMyFSM("Attacking");
                attacking.ChangeTransition("Wait", "FINISHED", "Antic");
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorSlugSpawner : DefaultSpawner<GhostWarriorSlugControl> { }

    public class GhostWarriorSlugPrefabConfig : DefaultPrefabConfig<GhostWarriorSlugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorNoEyesControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;


        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 10f, 2f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(ObjectMetadata other)
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
            //    OverrideState(control, "Hover", () => {
            //    });
            //    control.GetState("Hover").AddAction(new Wait() { time = 10f });
            //}

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorNoEyesSpawner : DefaultSpawner<GhostWarriorNoEyesControl> { }

    public class GhostWarriorNoEyesPrefabConfig : DefaultPrefabConfig<GhostWarriorNoEyesControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorMarkothControl : FSMBossAreaControl
    {
        public override string FSMName => "Movement";

        IEnumerator chasing;


        protected override void OnEnable()
        {
            base.OnEnable();
            if (thisMetadata == null)
                return;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
            if (chasing != null)
            {
                StopCoroutine(chasing);
            }
            chasing = DistanceFlyChase(gameObject, HeroController.instance.gameObject, 5f, 0.3f, 5f, 2f);
            StartCoroutine(chasing);
        }

        public override void Setup(ObjectMetadata other)
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
                //OverrideState(control, "Hover", () => {
                //});
                //control.GetState("Hover").AddAction(new Wait() { time = 10f });

                var shieldAttack = gameObject.LocateMyFSM("Shield Attack");
                var init = shieldAttack.GetState("Init");
                init.AddCustomAction(() => {

                    var shield = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shield", null, false);
                    shield.transform.parent = transform;
                    ObjectMetadata shieldMeta = new ObjectMetadata();
                    shieldMeta.Setup(shield, EnemyRandomizerDatabase.GetDatabase());
                    shieldMeta.ApplySizeScale(thisMetadata.SizeScale);
                    shield.SetActive(true);

                    shieldAttack.FsmVariables.GetFsmGameObject("Shield 1").Value = shield;
                });

                var attacking = gameObject.LocateMyFSM("Attacking");
                var nail = attacking.GetState("Nail");
                nail.AddCustomAction(() => {

                    var nailObject = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shot Markoth Nail", null, false);
                    nailObject.transform.parent = transform;
                    ObjectMetadata shieldMeta = new ObjectMetadata();
                    shieldMeta.Setup(nailObject, EnemyRandomizerDatabase.GetDatabase());
                    shieldMeta.ApplySizeScale(thisMetadata.SizeScale);
                    nailObject.SetActive(true);
                });
            }

            //this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
        }


        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GhostWarriorMarkothSpawner : DefaultSpawner<GhostWarriorMarkothControl> { }

    public class GhostWarriorMarkothPrefabConfig : DefaultPrefabConfig<GhostWarriorMarkothControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishGGControl : FSMBossAreaControl
    {
        public override string FSMName => "Mega Jellyfish";


        public override void Setup(ObjectMetadata other)
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

            this.thisMetadata.EnemyHealthManager.IsInvincible = false;

            control.GetState("Recover").DisableAction(2);
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class JellyfishGGSpawner : DefaultSpawner<JellyfishGGControl> { }

    public class JellyfishGGPrefabConfig : DefaultPrefabConfig<JellyfishGGControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaJellyfishControl : JellyfishGGControl
    {
        public override string FSMName => "Mega Jellyfish";
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class MegaJellyfishSpawner : DefaultSpawner<MegaJellyfishControl> { }

    public class MegaJellyfishPrefabConfig : DefaultPrefabConfig<MegaJellyfishControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeMotherControl : FSMBossAreaControl
    {
        public override string FSMName => "Fluke Mother";

        public AudioPlayerOneShotSingle squirtA;
        public AudioPlayerOneShotSingle squirtB;

        public override int maxBabies => 8;
        public override bool dieChildrenOnDeath => true;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            var spawn2 = control.GetState("Spawn 2");

            squirtA = spawn2.GetAction<AudioPlayerOneShotSingle>(9);
            squirtB = spawn2.GetAction<AudioPlayerOneShotSingle>(10);

            var init = control.GetState("Init");
            init.DisableAction(2);
            init.DisableAction(3);
            init.DisableAction(5);
            init.DisableAction(7);
            init.DisableAction(8);

            init.RemoveTransition("GG BOSS");

            var idle = control.GetState("Idle");
            var playIdle = control.GetState("Play Idle");

            idle.DisableAction(4);
            idle.DisableAction(3);

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


            var rage = control.GetState("Rage");
            var stateAfterRage = rage.GetTransition("SPAWN").ToState;

            var customSpawn = control.AddState("Custom Spawn");

            customSpawn.AddCustomAction(() =>
            {
                if(children.Count >= maxBabies)
                {
                    control.SendEvent("FINISHED");
                }
            });

            customSpawn.AddAction(squirtA);
            customSpawn.AddAction(squirtB);

            customSpawn.AddCustomAction(() =>
            {

                var fly = db.Spawn("Fluke Fly", null);

                var spawn = GetRandomPositionInLOSofSelf(0f, 2f, 0f, 0f);
                fly.transform.position = spawn;

                ActivateAndTrackSpawnedObject(fly);

                control.SendEvent("FINISHED");
            });


            rage.ChangeTransition("SPAWN", "Custom Spawn");
            customSpawn.AddTransition("FINISHED", stateAfterRage);
        }


        protected override void SetCustomPositionOnShow()
        {
            gameObject.StickToRoof();
        }
    }

    public class FlukeMotherSpawner : DefaultSpawner<FlukeMotherControl> { }

    public class FlukeMotherPrefabConfig : DefaultPrefabConfig<FlukeMotherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss1Control : FSMBossAreaControl
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

        public PreventOutOfBounds poob;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var deactive = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
            if(deactive != null)
            {
                Destroy(deactive);
            }

            //enable this after activating/repositioning hornet
            poob = gameObject.AddComponent<PreventOutOfBounds>();
            poob.enabled = false;

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("WAKE");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

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
            this.OverrideState(control, "Escalation", () =>
            {
                var esc = control.FsmVariables.GetFsmBool("Escalated");
                if(esc.Value)
                {
                    control.SendEvent("FINISHED");
                    return;
                }

                float hpPercent = (float)thisMetadata.EnemyHealthManager.hp / (float)thisMetadata.DefaultHP;
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
            this.OverrideState(control, "Can Throw?", () =>
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
                off.y += -.5f * thisMetadata.SizeScale;
            },5);

            var thrown = control.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });

            var aimJump = control.GetState("Aim Jump");
            this.OverrideState(control, "Aim Jump", () =>
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
                setScale.x = thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
            },0);

            var firingr = control.GetState("Firing R");
            firingr.InsertCustomAction(() => {
                var sfv = firingr.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = -thisMetadata.SizeScale;
            }, 0);

            var firingl = control.GetState("Firing L");
            firingl.InsertCustomAction(() => {
                var sfv = firingl.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = thisMetadata.SizeScale;
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
            landy.AddCustomAction(() => { SetCustomPositionOnShow(); });

            var hardland = control.GetState("Hard Land");
            hardland.InsertCustomAction(() => {
                var setScale = hardland.GetFirstActionOfType<SetScale>();
                setScale.y = thisMetadata.SizeScale;
            }, 0);

            var hitRoof = control.GetState("Hit Roof");
            hitRoof.DisableAction(2);
            hitRoof.InsertCustomAction(() => {
                var setScale = hitRoof.GetFirstActionOfType<SetScale>();
                setScale.x = control.FsmVariables.GetFsmFloat("Return X Scale").Value;
                setScale.y = thisMetadata.SizeScale;
            }, 0);

            var wallL = control.GetState("Wall L");
            wallL.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
                StickToWall();
            }, 0);

            var wallR = control.GetState("Wall R");
            wallR.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = -thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
                StickToWall();
            }, 0);
        }

        protected override void Show()
        {
            base.Show();
            poob.enabled = true;
        }

        protected override void SetCustomPositionOnShow()
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

    public class HornetBoss1PrefabConfig : DefaultPrefabConfig<HornetBoss1Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss2Control : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public float escalationHPPercentage = .7f;
        public float throwDistance = 12f;
        public float minAirSphereHeight = 5f;
        public float throwMaxTravelTime = .8f;
        public override int maxBabies => 5;
        public override bool dieChildrenOnDeath => true;

        public PreventOutOfBounds poob;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

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

            //enable this after activating/repositioning hornet
            poob = gameObject.AddComponent<PreventOutOfBounds>();
            poob.enabled = false;

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("BATTLE START");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

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
            this.OverrideState(control, "Escalation", () =>
            {
                var esc = control.FsmVariables.GetFsmBool("Escalated");
                if (esc.Value)
                {
                    control.SendEvent("FINISHED");
                    return;
                }

                //if hornet gets scaled to a normal enemy with low hp, just enable the escalation right away
                float hpPercent = (float)thisMetadata.EnemyHealthManager.hp / (float)thisMetadata.DefaultHP;
                if (hpPercent <= escalationHPPercentage || thisMetadata.EnemyHealthManager.hp < 100)
                {
                    control.FsmVariables.GetFsmBool("Can Barb").Value = true;

                    esc.Value = true;
                }

                control.SendEvent("FINISHED");
            });

            var canThrowq = control.GetState("Can Throw?");
            this.OverrideState(control, "Can Throw?", () =>
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
                off.y += -.5f * thisMetadata.SizeScale;
            }, 5);

            var thrown = control.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });

            var aimJump = control.GetState("Aim Jump");
            this.OverrideState(control, "Aim Jump", () =>
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
                setScale.x = thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
            }, 0);

            var firingr = control.GetState("Firing R");
            firingr.InsertCustomAction(() => {
                var sfv = firingr.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = -thisMetadata.SizeScale;
            }, 0);

            var firingl = control.GetState("Firing L");
            firingl.InsertCustomAction(() => {
                var sfv = firingl.GetFirstActionOfType<SetFloatValue>();
                sfv.floatValue = thisMetadata.SizeScale;
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
            landy.AddCustomAction(() => { SetCustomPositionOnShow(); });

            var hardland = control.GetState("Hard Land");
            hardland.InsertCustomAction(() => {
                var setScale = hardland.GetFirstActionOfType<SetScale>();
                setScale.y = thisMetadata.SizeScale;
            }, 0);

            var hitRoof = control.GetState("Hit Roof");
            hitRoof.DisableAction(2);
            hitRoof.InsertCustomAction(() => {
                var setScale = hitRoof.GetFirstActionOfType<SetScale>();
                setScale.x = control.FsmVariables.GetFsmFloat("Return X Scale").Value;
                setScale.y = thisMetadata.SizeScale;
            }, 0);

            var wallL = control.GetState("Wall L");
            wallL.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
                StickToWall();
            }, 0);

            var wallR = control.GetState("Wall R");
            wallR.DisableAction(4);
            wallL.InsertCustomAction(() => {
                var setScale = wallL.GetFirstActionOfType<SetScale>();
                setScale.x = -thisMetadata.SizeScale;
                setScale.y = thisMetadata.SizeScale;
                StickToWall();
            }, 0);

            var barbq = control.GetState("Barb?");
            barbq.DisableAction(1);
            barbq.DisableAction(2);
            barbq.InsertCustomAction(() => {
                if (children.Count >= maxBabies)
                    control.SendEvent("FINISHED");
            },0);

            var barbthrow = control.GetState("Barb Throw");
            barbthrow.DisableAction(1);
            barbthrow.InsertCustomAction(() => {

                RNG rng = new RNG();
                rng.Reset();

                var toplayer = DistanceToPlayer();
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

                var newBarb = EnemyRandomizerDatabase.GetDatabase().Spawn("Hornet Barb", null);

                ObjectMetadata barbMeta = new ObjectMetadata();
                barbMeta.Setup(newBarb, EnemyRandomizerDatabase.GetDatabase());
                barbMeta.ApplySizeScale(thisMetadata.SizeScale);
                newBarb.transform.position = spawnPoint;
                ActivateAndTrackSpawnedObject(newBarb);

                StartCoroutine(SendSpikeToBarbs());
            }, 0);
        }

        protected override void Show()
        {
            base.Show();
            poob.enabled = true;
        }

        protected override void SetCustomPositionOnShow()
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
            children.Where(x => x != null).Select(x => x.GetComponent<HornetBarbControl>())
                .ToList().ForEach(x => x.ActivateBarb());
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

        protected virtual void OnDestroy()
        {
            //base.OnDestroy();
            if (dieChildrenOnDeath)
            {
                children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    var boom = EnemyRandomizerDatabase.CustomSpawnWithLogic(x.transform.position, "Gas Explosion Recycle M", null, false);
                    ObjectMetadata boomMeta = new ObjectMetadata();
                    boomMeta.Setup(boom, EnemyRandomizerDatabase.GetDatabase());
                    boomMeta.ApplySizeScale(0.5f);
                    boom.SetActive(true);
                    Destroy(x);
                });
            }
        }
    }

    public class HornetBoss2Spawner : DefaultSpawner<HornetBoss2Control> { }

    public class HornetBoss2PrefabConfig : DefaultPrefabConfig<HornetBoss2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaZombieBeamMinerControl : FSMBossAreaControl
    {
        public override string FSMName => "Beam Miner";

        protected override bool ControlCameraLocks => true;

        protected Tk2dPlayAnimation sleepAnim;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            var tinker = other.Source.GetComponentInChildren<TinkEffect>();
            if (tinker != null)
            {
                GameObject.Destroy(tinker);
            }

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );

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
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                thisMetadata.EnemyHealthManager.IsInvincible = false;
            },0);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound
            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);
        }
    }

    public class MegaZombieBeamMinerSpawner : DefaultSpawner<MegaZombieBeamMinerControl> { }

    public class MegaZombieBeamMinerPrefabConfig : DefaultPrefabConfig<MegaZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerRematchControl : FSMBossAreaControl
    {
        public override string FSMName => "Beam Miner";
        protected override bool ControlCameraLocks => true;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            var tinker = other.Source.GetComponentInChildren<TinkEffect>();
            if(tinker != null)
            {
                GameObject.Destroy(tinker);
            }

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );


            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");
            wake.DisableAction(1);
            wake.DisableAction(2);
            wake.DisableAction(3);

            var idle = control.GetState("Idle");
            idle.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                thisMetadata.EnemyHealthManager.IsInvincible = false;
            }, 0);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound
            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);

            if (!other.IsBoss)
                roarEnd.GetFirstActionOfType<SetDamageHeroAmount>().damageDealt = 1;
        }
    }

    public class ZombieBeamMinerRematchSpawner : DefaultSpawner<ZombieBeamMinerRematchControl> { }

    public class ZombieBeamMinerRematchPrefabConfig : DefaultPrefabConfig<ZombieBeamMinerRematchControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordControl : MageLordControl { }

    public class DreamMageLordSpawner : DefaultSpawner<DreamMageLordControl> { }

    public class DreamMageLordPrefabConfig : DefaultPrefabConfig<DreamMageLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordPhase2Control : MageLordPhase2Control { }

    public class DreamMageLordPhase2Spawner : DefaultSpawner<DreamMageLordPhase2Control> { }

    public class DreamMageLordPhase2PrefabConfig : DefaultPrefabConfig<DreamMageLordPhase2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///














    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaMossChargerControl : FSMBossAreaControl
    {
        public override string FSMName => "Mossy Control";

        protected override string FSMHiddenStateName => "MOSS_HIDDEN";

        public RaycastHit2D floorSpawn;
        public RaycastHit2D floorLeft;
        public RaycastHit2D floorRight;
        public RaycastHit2D wallLeft;
        public RaycastHit2D wallRight;
        public float floorsize;
        public float floorCenter => floorLeft.point.x + floorsize * .5f;
        public Vector3 emergePoint;
        public Vector3 emergeVelocity;

        protected override bool HeroInAggroRange()
        {
            if (heroPos2d.y < floorSpawn.point.y)
                return false;

            if (heroPos2d.y > floorSpawn.point.y + 5f)
                return false;

            if (heroPos2d.x < floorLeft.point.x)
                return false;

            if (heroPos2d.x > floorRight.point.x)
                return false;

            return true;
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var initState = control.GetState("Init");
            initState.DisableAction(28);

            var fsm = gameObject.LocateMyFSM("FSM");
            if (fsm != null)
                Destroy(fsm);

            floorSpawn = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.down, 200f);

            //can't spawn here, just explode
            if (floorSpawn.collider == null)
            {
                thisMetadata.EnemyHealthManager.Die(null, AttackTypes.Generic, true);
                return;
            }

            floorLeft = SpawnerExtensions.GetRayOn(floorSpawn.point - Vector2.one * 0.2f, Vector2.left, 50f);
            floorRight = SpawnerExtensions.GetRayOn(floorSpawn.point - Vector2.one * 0.2f, Vector2.right, 50f);

            floorsize = floorRight.distance + floorLeft.distance;

            wallLeft = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.left, 50f);
            wallRight = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.right, 50f);

            if (floorsize < (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale))
            {
                float ratio = (floorsize) / (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale);
                thisMetadata.ApplySizeScale(ratio * .5f);
            }

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
            OverrideState(control, "Hero Beyond?", () =>
            {
                if (HeroInAggroRange())
                    control.SendEvent("FINISHED");
                else
                    control.SendEvent("CANCEL");
            });

            OverrideState(control, "Emerge Right", () => {
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
                SetXScaleSign(true);
                control.SendEvent("FINISHED");
            });

            OverrideState(control, "Emerge Left", () => {
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
                SetXScaleSign(false);
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
                    float leapSize = (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale * 3);
                    if ((wallLeft.point.x + leapSize > pos2d.x) || (wallRight.point.x - leapSize < pos2d.x))
                    {
                        control.SendEvent("RECHOOSE");
                        return;
                    }
                }, 0);
            }

            var emerge = control.GetState("Emerge");
            DisableActions(emerge, 0, 6);
            emerge.InsertCustomAction(() => {
                transform.position = emergePoint;
            }, 0);

            var submergeCD = control.GetState("Submerge CD");
            DisableActions(submergeCD, 6);

            var inAir = control.GetState("In Air");
            {
                inAir.DisableAction(1);

                var collisionAction = new HutongGames.PlayMaker.Actions.DetectCollisonDown();
                collisionAction.collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
                collisionAction.collideTag = "";
                collisionAction.sendEvent = new FsmEvent("LAND");

                inAir.AddAction(collisionAction);
                AddTimeoutAction(inAir, "LAND", 2f);
            }

            this.InsertHiddenState(control, "Init", "FINISHED", "Hidden");
        }

        protected override void SetCustomPositionOnShow()
        {
            base.SetCustomPositionOnShow();
        }
    }


    public class MegaMossChargerSpawner : DefaultSpawner<MegaMossChargerControl> { }

    public class MegaMossChargerPrefabConfig : DefaultPrefabConfig<MegaMossChargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   


    public class MawlekBodyControl : FSMBossAreaControl
    {
        public override string FSMName => "Mawlek Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);



            thisMetadata.PhysicsBody.gravityScale = 3f;

            var init = control.GetState("Init");
            init.DisableAction(1);
            init.DisableAction(14);

            this.InsertHiddenState(control, "Init", "FINISHED", "Start");
        }

        protected override void SetCustomPositionOnShow()
        {
            base.SetCustomPositionOnShow();
            control.FsmVariables.GetFsmFloat("Start X").Value = transform.position.x;
            control.FsmVariables.GetFsmFloat("Start Y").Value = transform.position.y;
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
        }
    }

    public class MawlekBodySpawner : DefaultSpawner<MawlekBodyControl> { }

    public class MawlekBodyPrefabConfig : DefaultPrefabConfig<MawlekBodyControl> { }



    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GhostWarriorMarmuControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        protected override void SetCustomPositionOnShow()
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

                var telepos = GetRandomPositionInLOSofSelf(1f, 40f, 2f, 5f);
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

                var telepos = GetRandomPositionInLOSofSelf(1f, 40f, 2f, 5f);
                control.FsmVariables.GetFsmFloat("Tele X").Value = telepos.x;
                control.FsmVariables.GetFsmFloat("Tele Y").Value = telepos.y;

            }, 0);
        }
    }

    public class GhostWarriorMarmuSpawner : DefaultSpawner<GhostWarriorMarmuControl>
    {
    }

    public class GhostWarriorMarmuPrefabConfig : DefaultPrefabConfig<GhostWarriorMarmuControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    ////
    public class JarCollectorControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public GameObject spawnerPlaceholder;
        public GameObject buzzers;
        public GameObject spitters;
        public GameObject rollers;

        public GameObject jarPrefab;
        public SpawnObjectFromGlobalPool spawnAction;

        public PrefabObject buzzer;
        public PrefabObject spitter;
        public PrefabObject roller;

        public int buzzerHP = 15;
        public int spitterHP = 10;
        public int rollerHP = 5;

        public RNG rng;

        public List<(PrefabObject, int)> possibleSpawns;

        public int CurrentEnemies { get; set; }
        public int MaxEnemies { get; set; }

        protected virtual void SetupSpawnerPlaceholders()
        {
            if (spawnerPlaceholder == null)
            {
                //generate the placeholder for the init state
                spawnerPlaceholder = new GameObject("Spawner Placeholder");
                spawnerPlaceholder.transform.SetParent(transform);
                spawnerPlaceholder.SetActive(false);

                buzzers = new GameObject("Buzzers");
                spitters = new GameObject("Spitters");
                rollers = new GameObject("Rollers");

                buzzers.transform.SetParent(spawnerPlaceholder.transform);
                spitters.transform.SetParent(spawnerPlaceholder.transform);
                rollers.transform.SetParent(spawnerPlaceholder.transform);

                control.FsmVariables.GetFsmGameObject("Top Pool").Value = spawnerPlaceholder;
                control.FsmVariables.GetFsmGameObject("Buzzers").Value = buzzers;
                control.FsmVariables.GetFsmGameObject("Spitters").Value = spitters;
                control.FsmVariables.GetFsmGameObject("Rollers").Value = rollers;

                var db = EnemyRandomizerDatabase.GetDatabase();
                buzzer = db.Enemies["Buzzer"];
                spitter = db.Enemies["Spitter"];
                roller = db.Enemies["Roller"];

                possibleSpawns = new List<(PrefabObject, int)>()
                {
                    (buzzer, buzzerHP),
                    (spitter, spitterHP),
                    (roller, rollerHP)
                };

                rng = new RNG();
                rng.Reset();
            }
        }

        protected virtual void SetMaxEnemies(int max)
        {
            MaxEnemies = max;
            control.FsmVariables.GetFsmInt("Enemies Max").Value = max;
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            var hpscaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpscaler != null)
                Destroy(hpscaler);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }


            SetupSpawnerPlaceholders();

            SetMaxEnemies(4);
            CurrentEnemies = 0;

            var init = control.GetState("Init");
            init.DisableAction(10);
            init.DisableAction(11);
            init.DisableAction(12);
            init.DisableAction(13);

            DisableSendEvents(control,
                ("Start Land", 2)
                );

            var onDeath = control.GetState("Death Start");
            onDeath.DisableAction(0);
            onDeath.InsertCustomAction(() => {
                if (thisMetadata.EnemyHealthManager.hp <= 0)
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
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                control.FsmVariables.GetFsmInt("Current Enemies").Value = CurrentEnemies;
            }, 0);

            //remove fly away anim
            control.ChangeTransition("Jump Antic", "FINISHED", "Summon?");

            var summon = control.GetState("Summon?");
            summon.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmInt("Boss Tag Count").Value = CurrentEnemies;
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

            spawnAction = new SpawnObjectFromGlobalPool();
            spawnAction.storeObject = new FsmGameObject();

            spawnAction.gameObject = new FsmGameObject();
            spawnAction.gameObject.Value = jarPrefab;

            spawnAction.spawnPoint = new FsmGameObject();
            spawnAction.spawnPoint.Value = gameObject;

            spawnAction.position = new FsmVector3();
            spawnAction.position.Value = Vector3.zero; //offset from boss (might update later)

            spawn.AddAction(spawnAction);
            spawn.AddCustomAction(() =>
            {
                var go = GameObject.Instantiate(spawnAction.storeObject.Value);

                if(go != null)
                {
                    float dist = 1.5f;
                    var throwDir = GetRandomDirectionVectorFromSelf(true);
                    var throwPoint = throwDir * dist + pos2dWithOffset;
                    go.transform.position = throwPoint;

                    var jar = go.GetComponent<SpawnJarControl>();
                    if (jar == null)
                        Dev.LogError("NO JAR COMPONENT FOUND!");

                    try
                    {
                        var selectedSpawn = possibleSpawns.GetRandomElementFromList(rng);
                        var thingToSpawn = thisMetadata.DB.Spawn(selectedSpawn.Item1, null);
                        jar.SetEnemySpawn(selectedSpawn.Item1.prefab, selectedSpawn.Item2);

                        var spawner = jar.gameObject.GetOrAddComponent<SpawnOnDestroyOrDisable>();
                        spawner.spawnEntity = selectedSpawn.Item1.prefab.name;
                        spawner.setHealthOnSpawn = selectedSpawn.Item2;

                        var body = jar.GetComponent<Rigidbody2D>();
                        if (body != null)
                        {
                            body.velocity = throwDir * 15f;
                            body.angularVelocity = 15f;
                        }

                        go.SetActive(true);
                        jar.GetComponent<SpriteRenderer>().enabled = true;
                        jar.transform.localScale = jar.transform.localScale * 0.5f;
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

        protected override void SetCustomPositionOnShow()
        {
            gameObject.StickToGroundX(1f);
        }
    }

    public class JarCollectorSpawner : DefaultSpawner<JarCollectorControl> { }

    public class JarCollectorPrefabConfig : DefaultPrefabConfig<JarCollectorControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class InfectedKnightControl : FSMBossAreaControl
    {
        public PlayMakerFSM balloonFSM;
        public override bool dieChildrenOnDeath => true;
        public override int maxBabies => 3;

        public override string FSMName => "IK Control";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


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

            this.OverrideState(balloonFSM, "Spawn", () =>
            {
                if (children.Count >= maxBabies)
                {
                    balloonFSM.SendEvent("STOP SPAWN");
                }

                if (CanEnemySeePlayer())
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
                        SpawnAndTrackChild("Parasite Balloon", spawnPoint);
                        balloonFSM.SendEvent("FINISHED");
                    }
                }
                else
                {
                    balloonFSM.SendEvent("CANCEL");
                }
            });
        }

        protected override void SetCustomPositionOnShow()
        {
            gameObject.StickToGround(-1f);
        }
    }

    public class InfectedKnightSpawner : DefaultSpawner<InfectedKnightControl> { }


    public class InfectedKnightPrefabConfig : DefaultPrefabConfig<InfectedKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GreyPrinceControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var e1 = control.GetState("Enter 1");
            e1.AddCustomAction(() => { control.FsmVariables.GetFsmInt("Level").Value = 1; });//force level to be 1 so this doesn't get out of hand...

            GameObject.Destroy(gameObject.LocateMyFSM("Constrain X"));

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

            CustomFloatRefs = new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                {"Right X" , x => edgeR},
                {"Left X" , x => edgeL},
            };
        }
    }

    public class GreyPrinceSpawner : DefaultSpawner<GreyPrinceControl> { }

    public class GreyPrincePrefabConfig : DefaultPrefabConfig<GreyPrinceControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GiantFlyControl : FSMBossAreaControl
    {
        public override string FSMName => "Big Fly Control";

        public static int babiesToSpawn = 6;

        static string MODHOOK_BeforeSceneLoad(string sceneName)
        {
            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            return sceneName;
        }

        public override void Setup(ObjectMetadata other)
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
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (thisMetadata == null)
                return;

            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        }

        protected override void SetCustomPositionOnShow()
        {
            var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
        }

        public void SelfSpawnBabies()
        {
            SpawnBabies(gameObject);
        }

        static void SpawnBabies(GameObject owner)
        {
            bool areBattleBabies = false;
            if (BattleManager.Instance.Value != null && BattleManager.FSM != null)
            {
                areBattleBabies = true;
            }

            try
            {
                Dev.Log("has database ref: " + EnemyRandomizerDatabase.GetDatabase.GetInvocationList().Length);
                if (EnemyRandomizerDatabase.GetDatabase != null)
                {
                    for (int i = 0; i < babiesToSpawn; ++i)
                    {
                        GameObject result = null;
                        if (EnemyRandomizerDatabase.GetDatabase().Enemies.TryGetValue("Fly", out var src))
                        {
                            Dev.Log("trying to spawn via prefab " + src.prefabName);
                            result = EnemyRandomizerDatabase.GetDatabase().Spawn(src, null);

                            if (areBattleBabies)
                            {
                                var bmo = result.GetOrAddComponent<BattleManagedObject>();
                                ObjectMetadata metaInfo = new ObjectMetadata();
                                metaInfo.Setup(result, EnemyRandomizerDatabase.GetDatabase());
                                bmo.Setup(metaInfo);
                            }
                        }
                        else
                        {
                            Dev.Log("trying to spawn via string");
                            result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);

                            if (areBattleBabies)
                            {
                                var bmo = result.GetOrAddComponent<BattleManagedObject>();
                                ObjectMetadata metaInfo = new ObjectMetadata();
                                metaInfo.Setup(result, EnemyRandomizerDatabase.GetDatabase());
                                bmo.Setup(metaInfo);
                            }
                        }

                        Dev.Log("result = " + result);
                        Dev.Log("self.Owner = " + owner);
                        if (result != null && owner != null)
                        {
                            result.transform.position = owner.transform.position;
                            result.SetActive(true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception trying to spawn a custom fly! {e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        static void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        {
            orig(self);

            if (string.Equals(self.Name, "Spawn Flies 2"))
            {
                SpawnBabies(self.Fsm.Owner.gameObject);
            }
        }
    }

    public class GiantFlySpawner : DefaultSpawner<GiantFlyControl> { }

    public class GiantFlyPrefabConfig : DefaultPrefabConfig<GiantFlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerControl : FSMBossAreaControl
    {
        public override string FSMName => "Big Buzzer";

        public override bool dieChildrenOnDeath => true;
        public override int maxBabies => 4;
        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                if (children.Count < 4)
                {

                    var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                    var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                    var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                    var leftSpawn = pos2d + Vector2.left * 20f;
                    var rightSpawn = pos2d + Vector2.right * 20f;

                    var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                    var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                    SpawnAndTrackChild("Buzzer", leftShorter);
                    SpawnAndTrackChild("Buzzer", rightShorter);
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
        protected override void SetCustomPositionOnShow()
        {
        }
    }


    public class GiantBuzzerSpawner : DefaultSpawner<GiantBuzzerControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            int buzzersInScene = GameObject.FindObjectsOfType<GiantBuzzerControl>().Length;

            //change the spawn to be the col buzzer if zote has been rescued
            if (GameManager.instance.GetPlayerDataBool("zoteRescuedBuzzer") ||
                GameManager.instance.GetPlayerDataInt("zoteDeathPos") == 0 ||
                buzzersInScene > 0)
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                return base.Spawn(db.Enemies["Giant Buzzer Col"], source);
            }
            else
            {
                return base.Spawn(p, source);
            }
        }
    }

    public class GiantBuzzerPrefabConfig : DefaultPrefabConfig<GiantBuzzerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerColControl : FSMBossAreaControl
    {
        public override bool dieChildrenOnDeath => true;
        public override int maxBabies => 4;

        public override string FSMName => "Big Buzzer";
        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            DisableSendEvents(control,
                ("Roar Left", 0),
                ("Roar Right", 0)
                );

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
                if (children.Count < 4)
                {
                    var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                    var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                    var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                    var leftSpawn = pos2d + Vector2.left * 20f;
                    var rightSpawn = pos2d + Vector2.right * 20f;

                    var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                    var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                    SpawnAndTrackChild("Buzzer", leftShorter);
                    SpawnAndTrackChild("Buzzer", rightShorter);
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

        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class GiantBuzzerColSpawner : DefaultSpawner<GiantBuzzerColControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            return base.Spawn(p, source);
        }
    }

    public class GiantBuzzerColPrefabConfig : DefaultPrefabConfig<GiantBuzzerColControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class FalseKnightDreamControl : FSMBossAreaControl
    {
        public override string FSMName => "FalseyControl";

        public Vector3 originalScale;

        public bool hasSetupYet = false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //thisMetadata.CurrentHP = 40;

            //{
            //    var checkHP = gameObject.LocateMyFSM("Check Health");
            //    if (checkHP != null)
            //        Destroy(checkHP);
            //}

            AddTimeoutAction(control.GetState("Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("Rise 2"), "FALL", 2f);
            AddTimeoutAction(control.GetState("Fall 2"), "FALL", 1f);

            AddTimeoutAction(control.GetState("S Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("S Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("JA Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("JA Fall"), "FALL", 1f);

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
                transform.localScale = new Vector3(originalScale.x * -1.3f * thisMetadata.SizeScale, originalScale.y * thisMetadata.SizeScale, originalScale.z * thisMetadata.SizeScale);
                hasSetupYet = true;
            }, 5);

            var turnl = control.GetState("Turn L");
            turnl.DisableAction(5);
            turnl.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                transform.localScale = new Vector3(originalScale.x * 1.3f * thisMetadata.SizeScale, originalScale.y * thisMetadata.SizeScale, originalScale.z * thisMetadata.SizeScale);
                hasSetupYet = true;
            }, 5);

            var checkDirection = control.GetState("Check Direction");
            checkDirection.RemoveTransition("TURN L");
            checkDirection.RemoveTransition("TURN R");
            checkDirection.RemoveTransition("FINISHED");
            checkDirection.RemoveTransition("CANCEL");
            this.OverrideState(control, "Check Direction", () => {
                if(!hasSetupYet)
                {
                    control.SetState("Init");
                    return;
                }
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                Destroy(gameObject);
            });
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }


    public class FalseKnightDreamSpawner : DefaultSpawner<FalseKnightDreamControl> { }

    public class FalseKnightDreamPrefabConfig : DefaultPrefabConfig<FalseKnightDreamControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class FalseKnightNewControl : FSMBossAreaControl
    {
        public override string FSMName => "FalseyControl";

        public Vector3 originalScale;
        public bool hasSetupYet = false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            AddTimeoutAction(control.GetState("Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("Rise 2"), "FALL", 2f);
            AddTimeoutAction(control.GetState("Fall 2"), "FALL", 1f);

            AddTimeoutAction(control.GetState("S Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("S Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("JA Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("JA Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("JA Rise 2"), "FALL", 2f);
            AddTimeoutAction(control.GetState("JA Fall 2"), "FALL", 1f);

            AddTimeoutAction(control.GetState("Start Fall"), "FALL", 1f);

            AddTimeoutAction(control.GetState("Esc Rise"), "FALL", 2f);
            AddTimeoutAction(control.GetState("Esc Fall"), "FALL", 1f);

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

            startFall.ChangeTransition("FALL", "State 1");

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
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                transform.localScale = new Vector3(originalScale.x * -1.3f * thisMetadata.SizeScale, originalScale.y * thisMetadata.SizeScale, originalScale.z * thisMetadata.SizeScale);
                hasSetupYet = true;
            }, 5);

            var turnl = control.GetState("Turn L");
            turnl.DisableAction(5);
            turnl.InsertCustomAction(() => {
                var poob = gameObject.GetOrAddComponent<PreventOutOfBounds>();
                transform.localScale = new Vector3(originalScale.x * 1.3f * thisMetadata.SizeScale, originalScale.y * thisMetadata.SizeScale, originalScale.z * thisMetadata.SizeScale);
                hasSetupYet = true;
            }, 5);

            var checkDirection = control.GetState("Check Direction");
            checkDirection.RemoveTransition("TURN L");
            checkDirection.RemoveTransition("TURN R");
            checkDirection.RemoveTransition("FINISHED");
            this.OverrideState(control, "Check Direction", () => {
                if (!hasSetupYet)
                {
                    control.SetState("Init");
                    return;
                }
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                Destroy(gameObject);
            });
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }


    public class FalseKnightNewSpawner : DefaultSpawner<FalseKnightNewControl> { }

    public class FalseKnightNewPrefabConfig : DefaultPrefabConfig<FalseKnightNewControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class AbsoluteRadianceControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

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
        }

        protected override void SetupBossAsNormalEnemy()
        {
            base.SetupBossAsNormalEnemy();

            if (thisMetadata.SizeScale >= 1f)
            {
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale * 0.25f);
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
            ChangeRandomIntRange(attackCommands, "Orb Antic", 1, 2);

            //disable enemy kill shake commands that make the camera shake
            DisableSendEvents(attackCommands
                , ("EB 1", 3)
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
            if (FSMsUsingHiddenStates.Contains(control))
                FSMsUsingHiddenStates.Remove(control);

            //mute the init sfx
            SetAudioOneShotVolume(control, "Set Arena 1");
            SetAudioOneShotVolume(control, "First Tele");
        }

        protected virtual Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> CommandFloatRefs
        {
            get => new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                { "Orb Max X", x => x.edgeR - 1},
                { "Orb Max Y", x => x.roofY - 1},
                { "Orb Min X", x => x.edgeL + 1},
                { "Orb Min Y", x => x.floorY + 3},
            };
        }

        protected override Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> FloatRefs
        {
            get => new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                { "A1 X Max", x => x.edgeR - 2},
                { "A1 X Min", x => x.edgeL + 2},
            };
        }

        protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> refs)
        {
            base.UpdateRefs(fsm, refs);
            base.UpdateRefs(attackCommands, CommandFloatRefs);
        }
        protected override void SetCustomPositionOnShow()
        {
        }
    }

    public class AbsoluteRadianceSpawner : DefaultSpawner<AbsoluteRadianceControl>
    {
    }

    public class AbsoluteRadiancePrefabConfig : DefaultPrefabConfig<AbsoluteRadianceControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LobsterControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        protected override bool ControlCameraLocks => false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");

            //add a random timeout to force the lobster out of an infinite roll
            var rcCharging = control.GetState("RC Charging");
            rcCharging.AddCustomAction(() => {
                StartCoroutine(FireAfterTime("RC Charging", "WALL", 2f));
            });

            var rcAir = control.GetState("RC Air");
            rcAir.AddCustomAction(() => {
                StartCoroutine(FireAfterTime("RC Air", "LAND", 2f));
            });
        }

        protected virtual IEnumerator FireAfterTime(string state, string eventName, float t)
        {
            yield return new WaitForSeconds(t);
            if (control.ActiveStateName == state)
                control.SendEvent(eventName);
        }
    }

    public class LobsterSpawner : DefaultSpawner<LobsterControl> { }

    public class LobsterPrefabConfig : DefaultPrefabConfig<LobsterControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LancerControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                var cr = corpse.AddComponent<CorpseRemover>();
                cr.replacementEffect = "Death Explode Boss";
            }

            var init = control.GetState("Init");
            init.DisableAction(6);

            this.InsertHiddenState(control, "Init", "FINISHED", "First Aim");

            this.OverrideState(control, "Defeat", () =>
            {
                this.thisMetadata.EnemyHealthManager.hasSpecialDeath = false;
                this.thisMetadata.EnemyHealthManager.SetSendKilledToObject(null);
                this.thisMetadata.EnemyHealthManager.Die(null, AttackTypes.Generic, true);
                if (thisMetadata.EnemyHealthManager.hp <= 0)
                {
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Death Explode Boss", null, true);
                    Destroy(gameObject);
                }
            });
        }
    }

    public class LancerSpawner : DefaultSpawner<LancerControl> { }

    public class LancerPrefabConfig : DefaultPrefabConfig<LancerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageKnightControl : FSMBossAreaControl
    {
        public override string FSMName => "Mage Knight";

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> FloatRefs =>
            new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                    { "Floor Y",    x => { return heroPos2d.y; } },
                    { "Tele X Max", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "Tele X Min", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.left, 500f).point.x; } },
            };

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
        }
    }

    public class MageKnightSpawner : DefaultSpawner<MageKnightControl> { }

    public class MageKnightPrefabConfig : DefaultPrefabConfig<MageKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///










    /////////////////////////////////////////////////////////////////////////////
    ///// (Oblobbles)
    public class MegaFatBeeControl : FSMBossAreaControl
    {
        public override string FSMName => "fat fly bounce";

        public PlayMakerFSM FSMattack { get; set; }

        public Vector2 spawnPos;
        public override string spawnEntityOnDeath => "Death Explode Boss";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            spawnPos = other.ObjectPosition;

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

        protected override void SetCustomPositionOnShow()
        {
            thisMetadata.ObjectPosition = spawnPos;
            gameObject.GetOrAddComponent<PreventOutOfBounds>();
        }
    }

    public class MegaFatBeeSpawner : DefaultSpawner<MegaFatBeeControl> { }

    public class MegaFatBeePrefabConfig : DefaultPrefabConfig<MegaFatBeeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlackKnightControl : FSMBossAreaControl
    {
        public override string FSMName => "Black Knight";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            control.GetState("Antic Air").AddCustomAction(() => {
                gameObject.GetOrAddComponent<PreventOutOfBounds>();
            });

            AddTimeoutAction(control.GetState("Antic Air"), "LAND", 1f);
            AddTimeoutAction(control.GetState("Jump Air"), "LAND", 1f);
            AddTimeoutAction(control.GetState("Bounce Air"), "LAND", 1f);
            AddTimeoutAction(control.GetState("Charge"), "LAND", 1f);

            this.InsertHiddenState(control, "Init Facing", "FINISHED", "Bugs In");

            control.GetState("Cloud Stop").DisableAction(3);
            control.GetState("Cloud Stop").DisableAction(4);

            control.ChangeTransition("Bugs In End", "FINISHED", "Roar End");
        }
    }

    public class BlackKnightSpawner : DefaultSpawner<BlackKnightControl> { }

    public class BlackKnightPrefabConfig : DefaultPrefabConfig<BlackKnightControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////












    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RadianceControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

        //protected override void SetupBossAsNormalEnemy()
        //{
        //    base.SetupBossAsNormalEnemy();

        //    if (thisMetadata.SizeScale >= 1f)
        //    {
        //        thisMetadata.ApplySizeScale(thisMetadata.SizeScale * 0.25f);
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

    public class RadiancePrefabConfig : DefaultPrefabConfig<RadianceControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///




    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////
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