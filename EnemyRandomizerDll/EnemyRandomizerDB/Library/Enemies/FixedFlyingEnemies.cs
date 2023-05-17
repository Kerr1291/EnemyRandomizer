using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using UniRx;

namespace EnemyRandomizerMod
{
    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class WhitePalaceFlyControl : DefaultSpawnedEnemyControl
    {
        public override string spawnEntityOnDeath => isZapFly ? null : customOnDeathEffect;
        public string customOnDeathEffect = "Electro Zap";

        public int chanceToBeZapFly = 1;
        public int chanceToBeMaxZapFly = 10;
        public bool isZapFly;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            isZapFly = RollProbability(out _, chanceToBeZapFly, chanceToBeMaxZapFly);

            if(isZapFly)
            {
                thisMetadata.Sprite.color = Color.cyan * .4f;
            }

            CurrentHP = 1;
        }

        /// <summary>
        /// This needs to be set each frame to make the palace fly killable
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (thisMetadata == null)
                return;

            if (CurrentHP > 1)
                CurrentHP = 1;
        }
    }

    public class WhitePalaceFlySpawner : DefaultSpawner<WhitePalaceFlyControl> { }

    public class WhitePalaceFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HatcherControl : DefaultSpawnedEnemyControl
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;

        public override int maxBabies => isVanilla ? vanillaMaxBabies : 3;
        public override bool dieChildrenOnDeath => true;

        public override string FSMName => "Hatcher";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = thisMetadata.Source == other.Source;
            }

            var init = control.GetState("Initiate");
            init.DisableAction(2);

            var hatchedMaxCheck = control.GetState("Hatched Max Check");
            hatchedMaxCheck.DisableAction(1);
            hatchedMaxCheck.InsertCustomAction(() => {
                control.FsmVariables.GetFsmInt("Cage Children").Value = maxBabies - children.Count; }, 0);

            var fire = control.GetState("Fire");
            fire.DisableAction(1);
            fire.DisableAction(2);
            fire.DisableAction(6);
            fire.DisableAction(11);
            fire.InsertCustomAction(() => {
                ActivateAndTrackSpawnedObject(control.FsmVariables.GetFsmGameObject("Shot").Value);
            }, 6);
            fire.InsertCustomAction(() => {
                var child = SpawnAndTrackChild("Fly", transform.position, false);
                control.FsmVariables.GetFsmGameObject("Shot").Value = child;
            }, 0);
            fire.AddCustomAction(() => { control.SendEvent("WAIT"); });
        }
    }

    public class HatcherSpawner : DefaultSpawner<HatcherControl> { }

    public class HatcherPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class CentipedeHatcherControl : DefaultSpawnedEnemyControl
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;
        public override int maxBabies => isVanilla ? vanillaMaxBabies : 3;
        public override bool dieChildrenOnDeath => true;

        public override string FSMName => "Hatcher";

        public PlayMakerFSM FSM { get; set; }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = thisMetadata.Source == other.Source;
            }

            FSM = gameObject.LocateMyFSM("Centipede Hatcher");

            var init = FSM.GetState("Init");
            init.DisableAction(1);
            //init.DisableAction(3);

            var getCentipede = FSM.GetState("Get Centipede");
            getCentipede.DisableAction(0);
            getCentipede.DisableAction(1);
            getCentipede.DisableAction(2);
            getCentipede.DisableAction(3);
            getCentipede.AddCustomAction(() => { 
                if(children.Count >= maxBabies)
                    FSM.SendEvent("EXHAUSTED"); 
            });
            getCentipede.AddCustomAction(() => {
                var child = SpawnAndTrackChild("Baby Centipede",transform.position,false);
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value = child;
            });
            getCentipede.AddCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.transform.SetParent(transform);
            });
            getCentipede.AddCustomAction(() => {
                    FSM.SendEvent("FINISHED");
            });

            var birth = FSM.GetState("Birth");
            birth.DisableAction(1);
            birth.DisableAction(7);
            birth.DisableAction(15);//disable the reversing the scale
            birth.InsertCustomAction(() => {
                ActivateAndTrackSpawnedObject(FSM.FsmVariables.GetFsmGameObject("Hatchling").Value);
                //FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 6);

            var checkBirths = FSM.GetState("Check Births");
            birth.DisableAction(0);
            checkBirths.InsertCustomAction(() => {
                if (children.Count >= maxBabies)
                    FSM.SendEvent("HATCHED MAX");
            }, 0);
        }
    }

    public class CentipedeHatcherSpawner : DefaultSpawner<CentipedeHatcherControl> { }

    public class CentipedeHatcherPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mage";

        protected override bool ControlCameraLocks => false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            DisableActions(wake, 1, 2, 3);
            wake.InsertCustomAction(() => {
                var pos = GetRandomPositionInLOSofSelf(5f, 30f, 1f, 1f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            OverrideState(control, "Select Target", () =>
            {
                var pos = GetRandomPositionInLOSofSelf(5f, 20f, 1f, 4f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });

            control.ChangeTransition("Tele Away", "FINISHED", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            //this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class MageSpawner : DefaultSpawner<MageControl> { }

    public class MagePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ElectricMageControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Electric Mage";

        public float hpRatioTriggerNerf = 2f;
        public override float aggroRange => 20f;

        protected override bool ControlCameraLocks => false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            DisableActions(wake, 0, 1, 2);
            wake.InsertCustomAction(() => {
                var pos = GetRandomPositionInLOSofSelf(5f, 30f, 1f, 1f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            OverrideState(control, "Select Target", () =>
            {
                var pos = GetRandomPositionInLOSofSelf(5f, 20f, 1f, 4f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });


            var gen = control.GetState("Gen");
            gen.InsertCustomAction(() => {
                if (!HeroInAggroRange())
                    control.SendEvent("END");
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            //this.AddResetToStateOnHide(control, "Init");
        }

        protected override void ScaleHP()
        {
            base.ScaleHP();
            if (originialMetadata == null)
                return;

            defaultScaledMaxHP = defaultScaledMaxHP / 2;
            if (defaultScaledMaxHP <= 0)
                defaultScaledMaxHP = 1;

            CurrentHP = defaultScaledMaxHP;
        }
    }

    public class ElectricMageSpawner : DefaultSpawner<ElectricMageControl> { }

    public class ElectricMagePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class LaserTurretFramesControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override bool takesSpecialCharmDamage => true;
        public override bool takesSpecialSpellDamage => true;

        //GameObject hitCrystals;
        DamageEnemies damageEnemies;

        public void Splat()
        {
            var hitEffect = SpawnEntityAt("Hit Crystals", transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            damageEnemies = gameObject.GetOrAddComponent<DamageEnemies>();
            damageEnemies.damageDealt = 10;
            damageEnemies.direction = 90f; //TODO: make sure this isn't backwards
            damageEnemies.magnitudeMult = 1f;
            damageEnemies.moveDirection = false;
            damageEnemies.circleDirection = false;
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Generic;
            damageEnemies.specialType = 0;

            var hm = gameObject.GetOrAddComponent<HealthManager>();
            hm.hp = 64;//TODO:
            hm.OnDeath += Splat;
            hm.IsInvincible = true;
            thisMetadata.Setup(gameObject, thisMetadata.DB); //not needed?
            Geo = 7;
            thisMetadata.MRenderer.enabled = true;
            thisMetadata.Collider.enabled = true;

            var dir = GetRandomDirectionFromSelf();
            thisMetadata.PhysicsBody.velocity = dir * 13f;

            float angle = SpawnerExtensions.RotateToDirection(dir);
            thisMetadata.Rotation = angle;
        }

        protected override void SetDefaultPosition()
        {
            gameObject.StickToClosestSurface(200f, false);
        }
    }

    public class LaserTurretFramesSpawner : DefaultSpawner<LaserTurretFramesControl> { }

    public class LaserTurretFramesPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class BeeDropperControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        ParticleSystem ptBurst;
        GameObject splat;


        protected virtual void Splat()
        {
            splat.SafeSetActive(true);
            thisMetadata.MRenderer.enabled = false;
            thisMetadata.Collider.enabled = false;
            ptBurst.Emit(0);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            RemoveFSM("Recoil");
            RemoveFSM("Control");

            ptBurst = gameObject.FindGameObjectInDirectChildren("Pt Burst").GetComponent<ParticleSystem>();
            splat = gameObject.FindGameObjectInChildrenWithName("Splat");

            var hm = gameObject.GetOrAddComponent<HealthManager>();
            hm.hp = 10;//TODO:
            
            hm.OnDeath += Splat;
            thisMetadata.Setup(gameObject, thisMetadata.DB); //not needed?
            Geo = 11;
            thisMetadata.PhysicsBody.isKinematic = false;
            thisMetadata.PhysicsBody.gravityScale = 0f;
            thisMetadata.MRenderer.enabled = true;
            thisMetadata.Collider.enabled = true;

            var dir = GetRandomDirectionFromSelf();
            thisMetadata.PhysicsBody.velocity = dir * 13f;

            float angle = SpawnerExtensions.RotateToDirection(dir);
            thisMetadata.Rotation = angle;
        }

        public float nearToWall = .5f;
        public float rayRange = 10f;
        public float fastRayDistance = 5f;
        public bool inFastCheckMode = false;
        public float slowCheckTimer = 1f;
        float timer;
        int bounceCD = 5;
        int bounceiscd = 0;

        protected override void Update()
        {
            base.Update();

            if(thisMetadata != null && thisMetadata.PhysicsBody != null && thisMetadata.PhysicsBody.velocity.magnitude < 1f)
            {
                thisMetadata.PhysicsBody.velocity = new Vector2(1f, 1f);
            }

            if(inFastCheckMode)
            {
                if (bounceiscd > 0)
                {
                    bounceiscd--;
                    return;
                }

                timer = 0f;
                var result = CheckForNearbyWalls();
                if (result != null && result.Value.collider != null)
                {
                    if (result.Value.distance > fastRayDistance)
                        inFastCheckMode = false;
                    else
                    {
                        if(result.Value.distance < nearToWall)
                        {
                            thisMetadata.PhysicsBody.velocity = BounceReflect(thisMetadata.PhysicsBody.velocity, result.Value.normal);
                            float angle = SpawnerExtensions.RotateToDirection(thisMetadata.PhysicsBody.velocity.normalized);
                            thisMetadata.Rotation = angle;
                            bounceiscd = bounceCD;
                        }
                    }   
                }
            }   
            else
            {
                timer += Time.deltaTime;
                if (timer > slowCheckTimer)
                {
                    timer = 0f;
                    var result = CheckForNearbyWalls();
                    if(result != null && result.Value.collider != null)
                    {
                        if (result.Value.distance < fastRayDistance)
                            inFastCheckMode = true;
                    }                        
                }    
            }
        }

        protected virtual RaycastHit2D? CheckForNearbyWalls()
        {
            RaycastHit2D? closest = SpawnerExtensions.GetOctagonalRays(gameObject, rayRange).Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }
    }

    public class BeeDropperSpawner : DefaultSpawner<BeeDropperControl> { }

    public class BeeDropperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
