﻿using HutongGames.PlayMaker.Actions;
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
    public class MushroomBrawlerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Shroom Brawler";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Facing Right?", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");
        }

        protected override void SetDefaultPosition()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class MushroomBrawlerSpawner : DefaultSpawner<MushroomBrawlerControl> { }

    public class MushroomBrawlerPrefabConfig : DefaultPrefabConfig<MushroomBrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MenderBugControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mender Bug Ctrl";

        public override void Setup(ObjectMetadata other)
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
            this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class MenderBugSpawner : DefaultSpawner<MenderBugControl> { }

    public class MenderBugPrefabConfig : DefaultPrefabConfig<MenderBugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlockerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Blocker Control";

        public override bool dieChildrenOnDeath => true;

        public override int maxBabies => 2; 

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var fsm = control;

            //allow players without spells to kill blockers
            int level = GameManager.instance.GetPlayerDataInt("fireballLevel");
            if (level <= 0)
            {
                thisMetadata.EnemyHealthManager.InvincibleFromDirection = -1;

                //disable the invincible state
                var init = fsm.GetState("Init");
                init.DisableAction(6);

                //disable the invincible state
                var close = fsm.GetState("Close");
                close.DisableAction(0);
            }

            //ignore the checks, allow it to spawn rollers all the time
            this.OverrideState(fsm, "Can Roller?", () =>
            {
                //TODO: actual logic for spawning rollers, for now, some rng
                RNG rng = new RNG();
                rng.Reset();

                bool result = rng.Randf() > .5f;
                if (result)
                    fsm.SendEvent("FINISHED");
                else
                    fsm.SendEvent("GOOP");
            });

            //link the shot to a roller prefab
            var roller = fsm.GetState("Roller");
            var setgoa = roller.GetFirstActionOfType<SetGameObject>();

            //TODO: fix up their FSMs to work correctly with the activate and track method
            setgoa.gameObject = EnemyRandomizerDatabase.GetDatabase().Spawn("Roller", null);

            //have it skip the roller assign state
            fsm.ChangeTransition("Fire", "FINISHED", "Shot Anim End");
        }

        protected override void SetDefaultPosition()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class BlockerSpawner : DefaultSpawner<BlockerControl> { }

    public class BlockerPrefabConfig : DefaultPrefabConfig<BlockerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////  
    public class ZombieHiveControl : FSMAreaControlEnemy
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;

        public override int maxBabies => isVanilla ? vanillaMaxBabies : 3;

        public override string FSMName => "Hive Zombie";

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

            var FSM = control;

            var init = FSM.GetState("Init");
            init.DisableAction(1);

            var hatchedAmount = FSM.GetState("Hatched Amount");
            hatchedAmount.DisableAction(0);
            hatchedAmount.AddCustomAction(() => {
                if (children.Count >= maxBabies)
                    FSM.SendEvent("CANCEL");
                else 
                    FSM.SendEvent("FINISHED");
            });

            var spot1 = FSM.GetState("Spot 1");
            spot1.DisableAction(1);
            spot1.DisableAction(3);
            spot1.DisableAction(4);
            spot1.DisableAction(8);
            spot1.InsertCustomAction(() => {
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Bee Hatchling Ambient", null);
                children.Add(child);

                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value = child;
            },0); ;
            spot1.InsertCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 4);

            var spot2 = FSM.GetState("Spot 2");
            spot2.DisableAction(1);
            spot2.DisableAction(3);
            spot2.DisableAction(4);
            spot2.DisableAction(8);
            spot2.InsertCustomAction(() => {
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Bee Hatchling Ambient", null);
                children.Add(child);
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value = child;
            }, 0); ;
            spot2.InsertCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 4);
        }
    }

    public class ZombieHiveSpawner : DefaultSpawner<ZombieHiveControl> { }

    public class ZombieHivePrefabConfig : DefaultPrefabConfig<ZombieHiveControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider1Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Chase";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var inert = control.GetState("Inert");
            inert.AddCustomAction(() => { control.SetState("Init"); });

            //change the start transition to just begin the spawn antics
            control.ChangeTransition("Check Battle", "BATTLE", "Wait 2");
            control.RemoveTransition("Battle Inert", "BATTLE START");
        }

        protected override void SetDefaultPosition()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class ZombieSpider1Spawner : DefaultSpawner<ZombieSpider1Control> { }

    public class ZombieSpider1PrefabConfig : DefaultPrefabConfig<ZombieSpider1Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    









    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider2Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Chase";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var inert = control.GetState("Inert");
            inert.AddCustomAction(() => { control.SetState("Init"); });

            //change the start transition to just begin the spawn antics
            control.ChangeTransition("Check Battle", "BATTLE", "Wait 2");
            control.RemoveTransition("Battle Inert", "BATTLE START");
        }

        protected override void SetDefaultPosition()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class ZombieSpider2Spawner : DefaultSpawner<ZombieSpider2Control> { }
    public class ZombieSpider2PrefabConfig : DefaultPrefabConfig<ZombieSpider2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossChargerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mossy Control";

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

            var fsm = gameObject.LocateMyFSM("FSM");
            if (fsm != null)
                Destroy(fsm);

            floorSpawn = SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.down, 200f);

            //can't spawn here, just explode
            if(floorSpawn.collider == null)
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
                if(HeroInAggroRange())
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

            var emerge = control.GetState("Emerge");
            DisableActions(emerge, 1, 2, 3, 4, 5);
            emerge.InsertCustomAction(() => {
                transform.position = emergePoint;
            }, 0);

            var submergeCD = control.GetState("Submerge CD");
            DisableActions(submergeCD, 6);
        }
    }


    public class MossChargerSpawner : DefaultSpawner<MossChargerControl> { }

    public class MossChargerPrefabConfig : DefaultPrefabConfig<MossChargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////   



    public class MawlekTurretControl : DefaultSpawnedEnemyControl
    {
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
