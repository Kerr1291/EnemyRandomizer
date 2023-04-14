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

namespace EnemyRandomizerMod
{



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

            //this.OverrideState(control, "Defeat", () =>
            //{
            //    this.thisMetadata.EnemyHealthManager.hasSpecialDeath = false;
            //    this.thisMetadata.EnemyHealthManager.SetSendKilledToObject(null);
            //    this.thisMetadata.EnemyHealthManager.Die(null, AttackTypes.Generic, true);
            //});
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MenderBugSpawner : DefaultSpawner<MenderBugControl> { }

    public class MenderBugPrefabConfig : DefaultPrefabConfig<MenderBugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlockerControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var fsm = gameObject.LocateMyFSM("Blocker Control");

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
            setgoa.gameObject = EnemyRandomizerDatabase.GetDatabase().Enemies["Roller"].prefab;

            //have it skip the roller assign state
            fsm.ChangeTransition("Fire", "FINISHED", "Shot Anim End");
        }
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class BlockerSpawner : DefaultSpawner<BlockerControl> { }

    public class BlockerPrefabConfig : DefaultPrefabConfig<BlockerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////  TODO: fix like hatcher
    public class ZombieHiveControl : DefaultSpawnedEnemyControl
    {
        public int maxBabies = 3;
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;
        public bool dieChildrenOnDeath = true;

        public PlayMakerFSM FSM { get; set; }

        public List<GameObject> children = new List<GameObject>();

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

            if (isVanilla)
                maxBabies = vanillaMaxBabies;

            FSM = gameObject.LocateMyFSM("Hive Zombie");

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
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Hatchling", null);
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
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Hatchling", null);
                children.Add(child);
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value = child;
            }, 0); ;
            spot2.InsertCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 4);
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }

        protected override void OnDestroy()
        {
            if (dieChildrenOnDeath)
            {
                children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    var hm = x.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        hm.Die(null, AttackTypes.Generic, true);
                    }
                });
            }
        }

        protected virtual void Update()
        {
            if (children == null)
                return;

            for (int i = 0; i < children.Count;)
            {
                if (i >= children.Count)
                    break;

                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    continue;
                }
                else
                {
                    var hm = children[i].GetComponent<HealthManager>();
                    if (hm.hp <= 0 || hm.isDead)
                    {
                        children.RemoveAt(i);
                        continue;
                    }
                }

                ++i;
            }
        }
    }

    public class ZombieHiveSpawner : DefaultSpawner<ZombieHiveControl> { }

    public class ZombieHivePrefabConfig : DefaultPrefabConfig<ZombieHiveControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider1Control : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            control = gameObject.LocateMyFSM("Chase");

            //change the start transition to just begin the spawn antics
            control.ChangeTransition("Check Battle", "BATTLE", "Wait 2");
            control.RemoveTransition("Battle Inert", "BATTLE START");
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieSpider1Spawner : DefaultSpawner<ZombieSpider1Control> { }

    public class ZombieSpider1PrefabConfig : DefaultPrefabConfig<ZombieSpider1Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    









    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class ZombieSpider2Control : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            control = gameObject.LocateMyFSM("Chase");

            //change the start transition to just begin the spawn antics
            control.ChangeTransition("Check Battle", "BATTLE", "Wait 2");
            control.RemoveTransition("Battle Inert", "BATTLE START");
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
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

        public override string FSMHiddenStateName => "MOSS_HIDDEN";

        public bool didInit = false;
        public bool isVisible = false;
        public bool isRunning = false;

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> MossyFloatsRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => MossyFloatsRefs;

        public float edgeL;
        public float edgeR;
        public float aggroGroundYSize;
        public float aggroGroundXSize;
        public Vector2 sizeOfAggroArea;
        public Vector2 centerOfAggroArea;
        public UnityEngine.Bounds aggroBounds;

        protected override bool HeroInAggroRange()
        {
            if (aggroBounds == null)
                return false;

            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var down = gameObject.transform.position.Fire2DRayGlobal(Vector2.down, 500f);
            var floor = down.collider;
            var floorsize = floor.bounds.size;

            var left = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 500f);
            var right = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 500f);

            var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

            var wallL = (left.point - pos2d).magnitude;
            var wallR = (right.point - pos2d).magnitude;

            edgeL = wallL < floorsize.x / 2f ? wallL : floorsize.x / 2f;
            edgeR = wallR < floorsize.x / 2f ? wallR : floorsize.x / 2f;

            //is it too big for the platform? then shrink it
            if (edgeR - edgeL < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
            {
                float ratio = (edgeR - edgeL) / (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale);
                thisMetadata.ApplySizeScale(ratio * .5f);
            }

            //for the aggro range, allow the full y size without scaling
            aggroGroundYSize = thisMetadata.ObjectSize.y;
            aggroGroundXSize = edgeR - edgeL;
            centerOfAggroArea = new Vector2(edgeL + edgeR / 2f, down.point.y + aggroGroundYSize / 2f);
            sizeOfAggroArea = new Vector2(aggroGroundXSize, aggroGroundYSize);

            aggroBounds = new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

            MossyFloatsRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"X Max" , x => edgeR},
                {"X Min" , x => edgeL},
                {"Start Y" , x => (HeroController.instance.transform.position + new Vector3(0f, 1f, 0f)).Fire2DRayGlobal(Vector2.down, 50f).point.y},
                {"Start X" , x => centerOfAggroArea.x },
            };

            //just skip this state
            var hs = control.GetState("Hidden");
            hs.DisableAction(1);
            hs.DisableAction(2);
            hs.AddCustomAction(() =>
            {
                control.SendEvent("IN RANGE");
            });

            //use our custom hidden aggro range logic
            this.InsertHiddenState(control, "Init", "FINISHED", "Hidden");
            control.GetState(FSMHiddenStateName).InsertCustomAction(() =>
            {
                //reset our flags at the start of being outside aggro range
                isVisible = false;
                isRunning = false;
            }, 0);

            //change the manual reset states to point to our custom hidden state
            control.ChangeTransition("Play Range", "FINISHED", FSMHiddenStateName);
            control.ChangeTransition("Hero Beyond?", "CANCEL", FSMHiddenStateName);

            DisableSendEvents(control,
                ("Emerge", 1)
                );

            var eRight = control.GetState("Emerge Right");
            var eLeft = control.GetState("Emerge Left");

            //configure emerge right
            {
                eRight.DisableAction(0);
                eRight.DisableAction(1);
                eRight.DisableAction(3);
                eRight.DisableAction(4);
                eRight.DisableAction(5);
                eRight.DisableAction(9);

                eRight.AddCustomAction(() =>
                {
                    //too close to edge? do emerge left
                    float dist = edgeR - HeroX;
                    if (dist < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
                    {
                        control.SendEvent("LEFT");
                        return;
                    }

                    //pick a random spot between hero and edge
                    RNG rng = new RNG();
                    rng.Reset();
                    float spawnX = rng.Rand(HeroX, edgeR - (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale));

                    control.FsmVariables.GetFsmFloat("Appear X").Value = spawnX;

                    control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(-1f, 0f);
                    control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(-6.5f, -.5f);

                    control.SendEvent("FINISHED");
                });
            }

            //configure emerge left
            {
                eLeft.DisableAction(1);
                eLeft.DisableAction(2);
                eLeft.DisableAction(3);
                eLeft.DisableAction(4);
                eLeft.DisableAction(5);
                eLeft.DisableAction(8);

                eLeft.AddCustomAction(() =>
                {
                    //too close to edge? do emerge left
                    float dist = HeroX - edgeL;
                    if (dist < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
                    {
                        control.SendEvent("RIGHT");
                        return;
                    }

                    //pick a random spot between hero and edge
                    RNG rng = new RNG();
                    rng.Reset();
                    float spawnX = rng.Rand(edgeL + (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale), HeroX);

                    control.FsmVariables.GetFsmFloat("Appear X").Value = spawnX;

                    control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(1f, 0f);
                    control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(6.5f, -.5f);

                    control.SendEvent("FINISHED");
                });
            }

            //we calculated the emerge position using world space, so make sure the action uses world space
            control.GetState("Emerge").GetAction<SetPosition>(5).space = Space.World;
            control.AddState("Emerge").InsertCustomAction(() => isVisible = true, 0);

            //mark this flag for our custom hide state
            control.GetState("Init").AddCustomAction(() => didInit = true);

            //add this state to choose the correct "hiding" animation
            var customHide = control.AddState("CUSTOM_HIDE");

            customHide.AddTransition("NEEDS_INIT", "Init");
            customHide.AddTransition("DO_INVIS_HIDE", FSMHiddenStateName);
            customHide.AddTransition("DO_SUBMERGE", "Submerge");
            customHide.AddTransition("DO_DIG", "Dig Start");

            customHide.AddCustomAction(() =>
            {
                if (!didInit)
                {
                    control.SendEvent("NEEDS_INIT");
                    return;
                }
                else if (isVisible && !isRunning)
                {
                    control.SendEvent("DO_SUBMERGE");
                    return;
                }
                else if (isVisible && isRunning)
                {
                    control.SendEvent("DO_DIG");
                    return;
                }
                else
                {
                    control.SendEvent("DO_INVIS_HIDE");
                }
            });

            this.AddResetToStateOnHide(control, "CUSTOM_HIDE");

            control.GetState("Submerge CD").GetAction<SetPosition>(6).space = Space.World;
        }

        protected override void UpdateCustomRefs()
        {
            base.UpdateCustomRefs();
            UpdateRefs(control, MossyFloatsRefs);
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
