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

namespace EnemyRandomizerMod
{
    public class FSMBossAreaControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> CustomFloatRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => CustomFloatRefs;

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 16);
        public float floorY => heroPos2d.FireRayGlobal(Vector2.down, 50f).point.y;
        public float roofY => heroPos2d.FireRayGlobal(Vector2.up, 200f).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, 100f).point.y;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, 100f).point.y;

        public Vector2 sizeOfAggroArea = new Vector2(50f, 50f);
        public Vector2 centerOfAggroArea => gameObject.transform.position;
        public UnityEngine.Bounds aggroBounds => new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

        protected override bool HeroInAggroRange()
        {
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        protected override void SetupCustomDebugArea()
        {
            //radius
            debugColliders.customLineCollections.Add(Color.red,
                DebugColliders.GetPointsFromCollider(Vector2.one, centerOfAggroArea, sizeOfAggroArea.magnitude).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

            //distance
            debugColliders.customLineCollections.Add(Color.magenta, new List<Vector3>() {
            heroPos2d, pos2d, heroPos2d
            });

            //bounds
            debugColliders.customLineCollections.Add(Color.blue, debugColliders.GetPointsFromCollider(aggroBounds, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

            Vector2 min = new Vector2(edgeL, floorY);
            Vector2 max = new Vector2(edgeR, roofY);
            var rect = new Rect();
            rect = rect.SetMinMax(min, max);

            //arena bounds
            debugColliders.customLineCollections.Add(new Color(255, 255, 0), debugColliders.GetPointsFromCollider(rect, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());

            var down = heroPos2d.FireRayGlobal(Vector2.down, 50f).point;
            var up = heroPos2d.FireRayGlobal(Vector2.up, 200f).point;
            var left = heroPos2d.FireRayGlobal(Vector2.left, 100f).point;
            var right = heroPos2d.FireRayGlobal(Vector2.right, 100f).point;

            //floory
            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
            down, pos2d, down
            });

            //roofy
            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
            up, pos2d, up
            });

            //left
            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
            left, pos2d, left
            });

            //right
            debugColliders.customLineCollections.Add(Color.green, new List<Vector3>() {
            right, pos2d, right
            });
        }
    }




    /////////////////////////////////////////////////////////////////////////////
    ///// TODO
    public class MantisTraitorLordControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MantisTraitorLordSpawner : DefaultSpawner<MantisTraitorLordControl> { }

    public class MantisTraitorLordPrefabConfig : DefaultPrefabConfig<MantisTraitorLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MimicSpiderControl : DefaultSpawnedEnemyControl { }

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
            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>();
        }

        protected override void SetupNormalEnemyAsBoss()
        {
            base.SetupNormalEnemyAsBoss();
            var fsm = gameObject.LocateMyFSM("Spawn Balloon");
            balloonFSM = fsm;
        }

        protected override void SetupBossAsNormalEnemy()
        {
            base.SetupBossAsNormalEnemy();
        }

        FsmFloat adright;
        FsmFloat leftx;

        FsmFloat mindstabh;
        FsmFloat rightx;

        FsmFloat bxmax;
        FsmFloat bxmin;
        FsmFloat bymax;
        FsmFloat bymin;

        protected override void PreloadRefs()
        {
            {
                var fsm = gameObject.LocateMyFSM("IK Control");
                fsm.ChangeTransition("Init", "FINISHED", "Intro Fall");
                var fallpos = control.GetFirstActionOfType<SetPosition>("Intro Fall");
                fallpos.x.Value = pos2d.x;
                fallpos.y.Value = floorY + 4f;
                fsm.ChangeTransition("Intro Land", "FINISHED", "First Counter");
            }

            adright = control.Fsm.GetFsmFloat("Air Dash Height");
            leftx = control.Fsm.GetFsmFloat("Left X");
            mindstabh = control.Fsm.GetFsmFloat("Min Dstab Height");
            rightx = control.Fsm.GetFsmFloat("Right X");

            if (balloonFSM != null)
            {
                bxmax = balloonFSM.Fsm.Variables.GetFsmFloat("X Max");
                bxmin = balloonFSM.Fsm.Variables.GetFsmFloat("X Min");
                bymax = balloonFSM.Fsm.Variables.GetFsmFloat("Y Max");
                bymin = balloonFSM.Fsm.Variables.GetFsmFloat("Y Min");
            }
        }

        protected override void UpdateCustomRefs()
        {
            adright.Value = YMIN + 3;
            leftx.Value = XMIN;
            mindstabh.Value = YMIN + 5;
            rightx.Value = XMAX;

            control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").min = xR.Mid - 1;
            control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").max = xR.Mid + 1;
            control.GetFirstActionOfType<SetPosition>("Set Pos").x = transform.position.x;
            control.GetFirstActionOfType<SetPosition>("Set Pos").y = transform.position.y;

            if (balloonFSM != null)
            {
                bxmax.Value = XMAX;
                bxmin.Value = XMIN;
                bymax.Value = YMAX;
                bymin.Value = YMIN;
            }
        }
    }


    public class LostKinSpawner : DefaultSpawner<LostKinControl>
    {
    }


    public class LostKinPrefabConfig : DefaultPrefabConfig<LostKinControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordPhase2Control : FSMBossAreaControl
    {
        public override string FSMName => "Mage Lord 2";

        protected override bool ControlCameraLocks => true;

        public static float ShockwaveYPos = 3.23f;
        public static float QuakeYPos = 2.15f;
        public static float KnightQuakeYPos = .44f;

        //public Range xR = new Range(6.98f, 34.75f);
        //public Range yR = new Range(31.05f, 35.85f);

        public Range oxR = new Range(6.98f, 34.75f);
        public Range oyR = new Range(31.05f, 35.85f);

        protected override void BuildArena()
        {
            var hits = gameObject.GetNearestSurfaces(500f);

            oxR = new Range(hits[Vector2.left].point.x + .5f, hits[Vector2.right].point.x - .5f);
            oyR = new Range(hits[Vector2.down].point.y + .5f, hits[Vector2.up].point.y - .5f);

            //don't spawn in the roof
            QuakeYPos = 0f;
            KnightQuakeYPos = 0f;

            base.BuildArena();
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Tele X", x => x.xR.RandomValuef(new RNG(HeroController.instance.transform.position.x.GetHashCode()))},
                { "Left X", x => x.xR.Min},
                { "Right X", x => x.xR.Max},
                { "Top Y", x => x.yR.Max},
                { "Bot Y", x => x.yR.Min},
                { "Ground Y", x => x.yR.Min},
                { "Tele Y", x => x.yR.RandomValuef(new RNG(HeroController.instance.transform.position.y.GetHashCode()))},
                { "Hero Mid X", x => x.xR.Mid},
                { "X Scale", x => x.transform.localScale.x},
                { "Hero X", x => x.HeroX },
                { "Hero Y", x => x.HeroY },
                { "Shockwave Y", x => x.yR.Min + ShockwaveYPos },
                { "Quake Y", x => x.yR.Max + QuakeYPos },
                { "Knight Quake Y Max", x => x.yR.Max + KnightQuakeYPos },

                { "Orb Min X", x => oxR.Min},
                { "Orb Min Y", x => oyR.Min},
                { "Orb Max X", x => oxR.Max},
                { "Orb Max Y", x => oyR.Max},
            };

            //set it up to just instantly start
            {
                var fsm = control;
                fsm.ChangeTransition("Init", "PHASE 2", "Tele Quake");
                var init = fsm.GetState("Init");
                var last = (FloatCompare)init.Actions.Last();
                last.equal = last.lessThan;
                last.greaterThan = last.lessThan;
            }
        }
    }

    public class MageLordPhase2Spawner : DefaultSpawner<MageLordPhase2Control>
    {
    }

    public class MageLordPhase2PrefabConfig : DefaultPrefabConfig<MageLordPhase2Control>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class MageLordControl : FSMBossAreaControl
    {
        public override string FSMName => "Mage Lord";

        protected override bool ControlCameraLocks => true;

        public static float ShockwaveYPos = 3.23f;
        public static float QuakeYPos = 2.15f;
        public static float KnightQuakeYPos = .44f;

        protected override void BuildArena()
        {
            QuakeYPos = 0f;
            KnightQuakeYPos = 0f;

            base.BuildArena();
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Tele X", x => x.xR.RandomValuef(new RNG(HeroController.instance.transform.position.x.GetHashCode()))},
                { "Left X", x => x.xR.Min},
                { "Right X", x => x.xR.Max},
                { "Top Y", x => x.yR.Max},
                { "Bot Y", x => x.yR.Min},
                { "Ground Y", x => x.yR.Min},
                { "Tele Y", x => x.yR.RandomValuef(new RNG(HeroController.instance.transform.position.y.GetHashCode()))},
                { "Hero Mid X", x => x.xR.Mid},
                { "X Scale", x => x.transform.localScale.x},
                { "Hero X", x => x.HeroX },
                { "Hero Y", x => x.HeroY },
                { "Shockwave Y", x => x.yR.Min + ShockwaveYPos },
                { "Quake Y", x => x.yR.Max + QuakeYPos },
                { "Knight Quake Y Max", x => x.yR.Max + KnightQuakeYPos },
            };

            var badFSM = gameObject.LocateMyFSM("Destroy If Defeated");
            GameObject.Destroy(badFSM);

            control.ChangeTransition("Init", "FINISHED", "Teleport");
            control.ChangeTransition("Init", "GG BOSS", "Teleport");

            //setup default 'next event' state
            control.FsmVariables.GetFsmString("Next Event").Value = "IDLE";
        }
    }

    public class MageLordSpawner : DefaultSpawner<MageLordControl>
    {
    }

    public class MageLordPrefabConfig : DefaultPrefabConfig<MageLordControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////
































    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HiveKnightControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);


            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Left X" , x => edgeL},
                {"Right X" , x => edgeR},
                {"Ground Y" , x => floorY},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //control.ChangeTransition("Init", "FINISHED", "GG Bow");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Explode", "FINISHED", "Send Death Event");

            var bpsp = control.GetState("Balloon Pos").GetFirstActionOfType<SetPosition>();
            bpsp.x.Value = HeroX;
            bpsp.x.Value = HeroY + 15f;

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"AD Max X" , x => edgeR},
                {"AD Min X" , x => edgeL},
                {"Max X" , x => edgeR},
                {"Min X" , x => edgeL},
                {"Ground Y" , x => floorY},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "GG Bow");
            this.AddResetToStateOnHide(control, "Init");
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

            //control.ChangeTransition("Set Balloon HP", "FINISHED", "Tele Out");
            control.ChangeTransition("Stun", "FINISHED", "Reformed");
            control.ChangeTransition("Death Start", "FINISHED", "Death Explode");

            var bpsp = control.GetState("Balloon Pos").GetFirstActionOfType<SetPosition>();
            bpsp.x.Value = HeroX;
            bpsp.x.Value = HeroY + 15f;

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"AD Max X" , x => edgeR},
                {"AD Min X" , x => edgeL},
                {"Max X" , x => edgeR},
                {"Min X" , x => edgeL},
                {"Mid Y" , x => MidY},
                {"Ground Y" , x => floorY},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Tele Out");
            this.AddResetToStateOnHide(control, "Init");
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
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);



            //control.ChangeTransition("Set Balloon HP", "FINISHED", "Tele Out");
            //control.ChangeTransition("Stun", "FINISHED", "Reformed");
            //control.ChangeTransition("Death Start", "FINISHED", "Death Explode");

            //TEMP
            this.OverrideState(control, "Long Roar End", () => Destroy(gameObject));

            //var bpsp = control.GetState("Balloon Pos").GetFirstActionOfType<SetPosition>();
            //bpsp.x.Value = HeroX;
            //bpsp.x.Value = HeroY + 15f;

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Right X" , x => edgeR},
                {"Left X" , x => edgeL},
                {"TeleRange Max" , x => edgeR},
                {"TeleRange Min" , x => edgeL},
                {"PuppetSlam Y" , x => floorY},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Init Idle");
            this.AddResetToStateOnHide(control, "Init");
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
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);



            control.ChangeTransition("Intro 6", "FINISHED", "Intro Roar End");
            //control.ChangeTransition("Stun", "FINISHED", "Reformed");
            //control.ChangeTransition("Death Start", "FINISHED", "Death Explode");

            //TEMP
            //this.OverrideState(control, "Long Roar End", () => Destroy(gameObject));

            //var bpsp = control.GetState("Balloon Pos").GetFirstActionOfType<SetPosition>();
            //bpsp.x.Value = HeroX;
            //bpsp.x.Value = HeroY + 15f;

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Right X" , x => edgeR},
                {"Left X" , x => edgeL},
                {"TeleRange Max" , x => edgeR},
                {"TeleRange Min" , x => edgeL},
                {"Plume Y" , x => floorY - 4f},
                {"Stun Land Y" , x => floorY},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Intro 1");
            this.AddResetToStateOnHide(control, "Init");
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



            //control.ChangeTransition("Intro 6", "FINISHED", "Intro Roar End");
            //control.ChangeTransition("Stun", "FINISHED", "Reformed");
            //control.ChangeTransition("Death Start", "FINISHED", "Death Explode");

            //TEMP
            //this.OverrideState(control, "Long Roar End", () => Destroy(gameObject));

            //var bpsp = control.GetState("Balloon Pos").GetFirstActionOfType<SetPosition>();
            //bpsp.x.Value = HeroX;
            //bpsp.x.Value = HeroY + 15f;

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                {"Left Pos" , x => edgeL},
                {"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Get High");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"X Min" , x => edgeL},
                {"X Max" , x => edgeR},
                {"Y Min" , x => floorY},
                {"Y Max" , x => roofY},
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Dolphin Max X" , x => edgeR},
                {"Dolphin Min X" , x => edgeL},
                {"Max X" , x => edgeR},
                {"Min X" , x => edgeL},
                {"Erupt Y" , x => floorY},
                {"Buried Y" , x => floorY - 3f},
                //{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                //{"Left Pos" , x => edgeL},
                //{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Underground");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Warp In");
            this.AddResetToStateOnHide(control, "Init");
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

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Dolphin Max X" , x => edgeR},
                //{"Dolphin Min X" , x => edgeL},
                //{"Max X" , x => edgeR},
                //{"Min X" , x => edgeL},
                //{"Erupt Y" , x => floorY},
                //{"Buried Y" , x => floorY - 3f},
                ////{"Mid Y" , x => edgeL + (edgeR-edgeL)/2f},
                ////{"Left Pos" , x => edgeL},
                ////{"Right Pos" , x => edgeR},
            };

            this.InsertHiddenState(control, "Init", "FINISHED", "Start");
            this.AddResetToStateOnHide(control, "Init");

            this.thisMetadata.EnemyHealthManager.IsInvincible = false;

            control.GetState("Recover").DisableAction(2);
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
    }

    public class MegaJellyfishSpawner : DefaultSpawner<MegaJellyfishControl> { }

    public class MegaJellyfishPrefabConfig : DefaultPrefabConfig<MegaJellyfishControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeMotherControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Fluke Mother";

        public AudioPlayerOneShotSingle squirtA;
        public AudioPlayerOneShotSingle squirtB;

        public UnityEngine.Bounds spawnArea;

        protected override bool HeroInAggroRange()
        {
            return true;
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var surfaces = gameObject.GetNearestSurfaces(500f);

            var w = surfaces[Vector2.right].point.x - surfaces[Vector2.left].point.x;
            var h = surfaces[Vector2.up].point.y - surfaces[Vector2.down].point.y;

            spawnArea = new UnityEngine.Bounds(gameObject.transform.position, new Vector3(w, h, 0f));

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
            var customSpawn = control.AddState("Custom Spawn");

            customSpawn.AddCustomAction(() =>
            {
                var fly = db.Spawn("Fluke Fly", null);

                RNG rng = new RNG();
                rng.Reset();

                var spawn = rng.Rand(spawnArea.min, spawnArea.max);
                fly.transform.position = spawn;

                fly.gameObject.SetActive(true);
            });

            customSpawn.AddAction(squirtA);
            customSpawn.AddAction(squirtB);

            rage.ChangeTransition("SPAWN", "Custom Spawn");
        }
    }

    public class FlukeMotherSpawner : DefaultSpawner<FlukeMotherControl> { }

    public class FlukeMotherPrefabConfig : DefaultPrefabConfig<FlukeMotherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss1Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> HornetFloatRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => HornetFloatRefs;

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 16);
        public float floorY => heroPos2d.FireRayGlobal(Vector2.down, 50f).point.y;
        public float roofY => heroPos2d.FireRayGlobal(Vector2.up, 200f).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, 100f).point.y;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, 100f).point.y;

        //values taken from hornet's FSM
        public float sphereHeight = 33.8f - 27.55f;
        public float minDstabHeight = 33.31f - 27.55f;
        public float airDashHeight = 31.5f - 27.55f;
        public float throwXLoffset = 22.51f - 15.13f;
        public float throwXRoffset = 37.9f - 30.16f;

        public Vector2 sizeOfAggroArea = new Vector2(50f, 50f);
        public Vector2 centerOfAggroArea => gameObject.transform.position;
        public UnityEngine.Bounds aggroBounds => new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

        protected override bool HeroInAggroRange()
        {
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            HornetFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Wall X Left" , x => edgeL},
                {"Wall X Right" , x => edgeR},
                {"Floor Y" , x => floorY},
                {"Roof Y" , x => roofY},
                {"Sphere Y" , x => floorY + sphereHeight},
                {"Air Dash Height" , x => floorY + sphereHeight},
                {"Min Dstab Height" , x => floorY + minDstabHeight},
                {"Throw X L" , x => edgeL + throwXLoffset},
                {"Throw X R" , x => edgeR - throwXRoffset},
            };

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("WAKE");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(4);
            refightReady.DisableAction(5);
            refightReady.DisableAction(6);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            var refightWake = control.GetState("Refight Wake");
            refightWake.DisableAction(0);
            refightWake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(2);
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");
            this.AddResetToStateOnHide(control, "Refight Ready");
        }
    }

    public class HornetBoss1Spawner : DefaultSpawner<HornetBoss1Control> { }

    public class HornetBoss1PrefabConfig : DefaultPrefabConfig<HornetBoss1Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss2Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> HornetFloatRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => HornetFloatRefs;

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 16);
        public float floorY => heroPos2d.FireRayGlobal(Vector2.down, 50f).point.y;
        public float roofY => heroPos2d.FireRayGlobal(Vector2.up, 200f).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, 100f).point.y;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, 100f).point.y;

        //values taken from hornet's FSM
        public float sphereHeight = 33.8f - 27.55f;
        public float minDstabHeight = 33.31f - 27.55f;
        public float airDashHeight = 31.5f - 27.55f;
        public float throwXLoffset = 22.51f - 15.13f;
        public float throwXRoffset = 37.9f - 30.16f;

        public Vector2 sizeOfAggroArea = new Vector2(50f, 50f);
        public Vector2 centerOfAggroArea => gameObject.transform.position;
        public UnityEngine.Bounds aggroBounds => new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

        protected override bool HeroInAggroRange()
        {
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            HornetFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Wall X Left" , x => edgeL},
                {"Wall X Right" , x => edgeR},
                {"Floor Y" , x => floorY},
                {"Roof Y" , x => roofY},
                {"Sphere Y" , x => floorY + sphereHeight},
                {"Air Dash Height" , x => floorY + sphereHeight},
                {"Min Dstab Height" , x => floorY + minDstabHeight},
                {"Throw X L" , x => edgeL + throwXLoffset},
                {"Throw X R" , x => edgeR - throwXRoffset},
            };

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("BATTLE START");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(3);
            refightReady.DisableAction(5);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            refightReady.ChangeTransition("WAKE", "Refight Wake");

            var refightWake = control.GetState("Refight Wake");
            this.OverrideState(control, "Refight Wake", () => control.SendEvent("FINISHED"));
            refightWake.ChangeTransition("FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);
            flourish.DisableAction(6);

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");
            this.AddResetToStateOnHide(control, "Refight Ready");
        }
    }

    public class HornetBoss2Spawner : DefaultSpawner<HornetBoss2Control> { }

    public class HornetBoss2PrefabConfig : DefaultPrefabConfig<HornetBoss2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaZombieBeamMinerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Beam Miner";
        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;


        protected Tk2dPlayAnimation sleepAnim;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );

            var sleep = control.GetState("Sleep");
            sleepAnim = sleep.GetFirstActionOfType<Tk2dPlayAnimation>();

            var deparents = control.GetState("Deparents");
            deparents.AddAction(sleepAnim);

            this.InsertHiddenState(control, "Deparents", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Deparents");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");

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

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP * 2;
        }
    }

    public class MegaZombieBeamMinerSpawner : DefaultSpawner<MegaZombieBeamMinerControl> { }

    public class MegaZombieBeamMinerPrefabConfig : DefaultPrefabConfig<MegaZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerRematchControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Beam Miner";
        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );


            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");
            wake.DisableAction(1);
            wake.DisableAction(2);
            wake.DisableAction(3);

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

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP * 2;
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
}
