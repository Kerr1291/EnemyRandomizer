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
    public class MushroomBrawlerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Shroom Brawler";

        public override float spawnPositionOffset => 1f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Facing Right?", "FINISHED", "Wake");
        }
    }

    public class MushroomBrawlerSpawner : DefaultSpawner<MushroomBrawlerControl> { }

    public class MushroomBrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MenderBugControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mender Bug Ctrl";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //disable the mender state killed player data
            var killed = control.GetState("Killed");
            killed.DisableAction(0);

            var destroy = control.GetState("Destroy");
            destroy.DisableAction(0);

            var fly = control.GetState("Fly");
            fly.ChangeTransition("DESTROY", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
        }
    }

    public class MenderBugSpawner : DefaultSpawner<MenderBugControl> { }

    public class MenderBugPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlockerControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Blocker Control";

        public override float spawnPositionOffset => 0.66f;

        //TODO: fix this
        public override bool isInvincible => false;// GameManager.instance.GetPlayerDataInt("fireballLevel") > 0;

        public RNG rng = new RNG();
        public GameObject lastSpawnedObject;
        public float closeRange = 2f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            rng.Reset();
            ChildController.maxChildren = 2;

            //allow players without spells to kill blockers
            int level = GameManager.instance.GetPlayerDataInt("fireballLevel");
            if (level <= 0)
            {
                EnemyHealthManager.InvincibleFromDirection = -1;

                var idle = control.GetState("Idle");
                idle.DisableAction(2);
                idle.ChangeTransition("CLOSE", "Attack Choose"); //just make idle go to the attack state

                //disable the invincible state
                var init = control.GetState("Init");
                init.DisableAction(6);

                //disable the invincible state
                var close = control.GetState("Close");
                close.DisableAction(0);
                close.DisableAction(1);

                var open = control.GetState("Open");
                open.DisableAction(1);

                var sleep1 = control.GetState("Sleep 1");
                sleep1.DisableAction(0);
                sleep1.DisableAction(1);
            }
            else
            {
                var idle = control.GetState("Idle");
                idle.DisableAction(2);

                //disable the invincible state
                var close = control.GetState("Close");
                close.DisableAction(1);

                var sleep1 = control.GetState("Sleep 1");
                sleep1.DisableAction(1);
            }

            //ignore the checks, allow it to spawn rollers all the time
            control.OverrideState("Can Roller?", () =>
            {
                if (ChildController.AtMaxChildren)
                {
                    control.SendEvent("GOOP");
                }
                else
                {
                    bool result = rng.CoinToss();
                    if (result)
                        control.SendEvent("FINISHED");
                    else
                        control.SendEvent("GOOP");
                }
            });

            //link the shot to a roller prefab
            var roller = control.GetState("Roller");
            control.OverrideState("Roller", () => {
                control.FsmVariables.GetFsmBool("Rollering").Value = true;
                control.FsmVariables.GetFsmFloat("Spawned Time").Value = 0f;
                lastSpawnedObject = SpawnChildForEnemySpawner(transform.position, false, "Roller", "Projectile");
            });

            var fire = control.GetState("Fire");
            fire.InsertCustomAction(() => {
                lastSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(lastSpawnedObject);
                control.FsmVariables.GetFsmGameObject("Shot Instance").Value = lastSpawnedObject;
            }, 3);

            //have it skip the roller assign state
            control.ChangeTransition("Fire", "FINISHED", "Shot Anim End");

            var shotAnimEnd = control.GetState("Shot Anim End");
            shotAnimEnd.DisableActions(1, 2);
            control.AddCustomAction("Shot Anim End", () =>
            {
                if (isInvincible && gameObject.CanSeePlayer() && gameObject.DistanceToPlayer() < closeRange)
                    control.SendEvent("CLOSE");
                else
                    control.SendEvent("FINISHED");
            });

            var closed = control.GetState("Closed");
            closed.DisableActions(0,1);
            control.AddCustomAction("Closed", () =>
            {
                if (gameObject.CanSeePlayer() && gameObject.DistanceToPlayer() >= closeRange)
                    control.SendEvent("FAR");
                else
                    control.SendEvent("FINISHED");
            });
        }
    }

    public class BlockerSpawner : DefaultSpawner<BlockerControl> { }

    public class BlockerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////  
    public class ZombieHiveControl : DefaultSpawnedEnemyControl
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;

        public override string FSMName => "Hive Zombie";

        public GameObject leftSpawnedObject;
        public GameObject rightSpawnedObject;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            isVanilla = gameObject == other;

            ChildController.maxChildren = isVanilla ? vanillaMaxBabies : 2;

            var init = control.GetState("Init");
            init.DisableAction(1);

            var hatchedAmount = control.GetState("Hatched Amount");
            hatchedAmount.DisableAction(0);
            hatchedAmount.AddCustomAction(() => {
                if (ChildController.AtMaxChildren)
                    control.SendEvent("CANCEL");
                else 
                    control.SendEvent("FINISHED");
            });

            var spot1 = control.GetState("Spot 1");
            spot1.DisableAction(1);
            spot1.DisableAction(3);
            spot1.DisableAction(4);
            spot1.DisableAction(8);
            spot1.InsertCustomAction(() => {
                leftSpawnedObject = SpawnChildForEnemySpawner(transform.position, false, "Bee Hatchling Ambient", "Hatchling");
            },0); ;
            spot1.InsertCustomAction(() => {
                leftSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(leftSpawnedObject);
                control.FsmVariables.GetFsmGameObject("Hatchling").Value = leftSpawnedObject;
            }, 4);

            var spot2 = control.GetState("Spot 2");
            spot2.DisableAction(1);
            spot2.DisableAction(3);
            spot2.DisableAction(4);
            spot2.DisableAction(8);
            spot2.InsertCustomAction(() => {
                rightSpawnedObject = SpawnChildForEnemySpawner(transform.position, false, "Bee Hatchling Ambient", "Hatchling");
            }, 0); ;
            spot2.InsertCustomAction(() => {
                rightSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(rightSpawnedObject);
                control.FsmVariables.GetFsmGameObject("Hatchling").Value = rightSpawnedObject;
            }, 4);
        }
    }

    public class ZombieHiveSpawner : DefaultSpawner<ZombieHiveControl> { }

    public class ZombieHivePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider1Control : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Chase";

        public override float spawnPositionOffset => 1f;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var inert = control.GetState("Inert");
            inert.AddCustomAction(() => { control.SetState("Init"); });

            control.GetState("Check Battle").RemoveTransition("BATTLE");

            control.OverrideState("Check Battle", () => {
                Animator.Play("Death Land");
                PhysicsBody.velocity = Vector2.zero;
                control.SendEvent("FINISHED");
            });

            control.OverrideState("Check", () => {
                control.SendEvent("SPIDER");
            });

            control.GetState("Active").AddCustomAction(() => UnFreeze());

            control.AddTimeoutAction(control.GetState("Hit Ground"), "FINISHED", 1f);
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);
            GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
            GetComponent<PreventOutOfBounds>().onBoundCollision += Freeze;
        }

        protected virtual void Freeze(RaycastHit2D r, GameObject a, GameObject b)
        {
            var pl = gameObject.GetOrAddComponent<PositionLocker>();
            pl.positionLock = transform.position;
        }

        protected virtual void UnFreeze()
        {
            GameObject.Destroy(gameObject.GetComponent<PositionLocker>());
            GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
        }
    }

    public class ZombieSpider1Spawner : DefaultSpawner<ZombieSpider1Control> { }

    public class ZombieSpider1PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    









    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider2Control : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Chase";

        public override float spawnPositionOffset => 1f;

        public override bool preventOutOfBoundsAfterPositioning => true;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var inert = control.GetState("Inert");
            inert.AddCustomAction(() => { control.SetState("Init"); });

            control.GetState("Check Battle").RemoveTransition("BATTLE");

            control.OverrideState("Check Battle", () => {
                Animator.Play("Death Land");
                PhysicsBody.velocity = Vector2.zero;
                control.SendEvent("FINISHED");
            });

            control.OverrideState("Check", () => {
                control.SendEvent("SPIDER");
            });

            control.GetState("Active").AddCustomAction(() => UnFreeze());

            control.AddTimeoutAction(control.GetState("Hit Ground"), "FINISHED", 1f);
        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);
            GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
            GetComponent<PreventOutOfBounds>().onBoundCollision += Freeze;
        }

        protected virtual void Freeze(RaycastHit2D r, GameObject a, GameObject b)
        {
            var pl = gameObject.GetOrAddComponent<PositionLocker>();
            pl.positionLock = transform.position;
        }

        protected virtual void UnFreeze()
        {
            GameObject.Destroy(gameObject.GetComponent<PositionLocker>());
            GetComponent<PreventOutOfBounds>().onBoundCollision -= Freeze;
        }
    }

    public class ZombieSpider2Spawner : DefaultSpawner<ZombieSpider2Control> { }
    public class ZombieSpider2PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossChargerControl : InGroundEnemyControl
    {
        public override string FSMName => "Mossy Control";

        public override float spawnPositionOffset => 1f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

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
                if(HeroInAggroRange())
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

            var emerge = control.GetState("Emerge");
            emerge.DisableActions(1, 2, 3, 4, 5);
            emerge.InsertCustomAction(() => {
                transform.position = emergePoint.ToVec2() + Vector2.up * 1f;
            }, 0);

            var submergeCD = control.GetState("Submerge CD");
            submergeCD.DisableActions(6);
        }
    }


    public class MossChargerSpawner : DefaultSpawner<MossChargerControl> { }

    public class MossChargerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////   



    public class MawlekTurretControl : DefaultSpawnedEnemyControl
    {
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;

        Range upL = new Range(95f, 110f);
        Range upR = new Range(70f, 85f);

        Range leL = new Range(185f, 200f);
        Range leR = new Range(160f, 175f);

        Range dnL = new Range(250f, 265f);
        Range dnR = new Range(275f, 290f);

        Range riL = new Range(5f, 20f);
        Range riR = new Range(340f, 355f);

        //TODO: implement modified shot speed
        //public float shotSpeed = 10f;
        public float spawnDist = 1.4f;

        public bool isFloorTurret = false;

        void Start()
        {
            gameObject.StickToClosestSurface();

            var fsm = gameObject.LocateMyFSM("Mawlek Turret");

            var spawnPos = fsm.FsmVariables.FindFsmVector3("Spawn Pos");

            var left = fsm.GetState("Fire Left");
            var right = fsm.GetState("Fire Right");

            var leftMin = fsm.FsmVariables.FindFsmFloat("Angle Min L");
            var leftMax = fsm.FsmVariables.FindFsmFloat("Angle Max L");
            var rightMin = fsm.FsmVariables.FindFsmFloat("Angle Min R");
            var rightMax = fsm.FsmVariables.FindFsmFloat("Angle Max R");


            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z % 360f;
            if (!isFloorTurret)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
                leftMin.Value = upL.Min;
                leftMax.Value = upL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
                leftMin.Value = leL.Min;
                leftMax.Value = leL.Max;

                rightMin.Value = leR.Min;
                rightMax.Value = leR.Max;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
                leftMin.Value = dnL.Min;
                leftMax.Value = dnL.Max;

                rightMin.Value = dnL.Min;
                rightMax.Value = dnL.Max;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
                leftMin.Value = riL.Min;
                leftMax.Value = riL.Max;

                rightMin.Value = riR.Min;
                rightMax.Value = riR.Max;
            }

            spawnPos.Value = up * spawnDist * transform.localScale.y;
        }
    }

    public class MawlekTurretPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }

    //TODO: remove ceiling version entirely
    public class MawlekTurretCeilingPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            //overwrite the upside-down one since the code now works just fine for the upright one in all cases
            var uprightTurret = p.source.Scene.sceneObjects.FirstOrDefault(x => x.LoadedObject.prefabName == "Mawlek Turret");
            p.prefab = uprightTurret.LoadedObject.prefab;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }



    /////
    //////////////////////////////////////////////////////////////////////////////
}
