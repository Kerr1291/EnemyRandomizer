﻿using UnityEngine.SceneManagement;
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
    public class WhiteDefenderControl : FSMBossAreaControl
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

            this.InsertHiddenState(control, "Init", "FINISHED", "Will Evade?");
            this.AddResetToStateOnHide(control, "Init");
        }
    }



    public class WhiteDefenderSpawner : DefaultSpawner<WhiteDefenderControl> { }

    public class WhiteDefenderPrefabConfig : DefaultPrefabConfig<WhiteDefenderControl> { }
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
    ///











    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RadianceControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

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

        protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> CommandFloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Orb Max X", x => x.xR.Max - 1},
                { "Orb Max Y", x => x.yR.Max - 1},
                { "Orb Min X", x => x.xR.Min + 1},
                { "Orb Min Y", x => x.yR.Min + 3},
            };
        }

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "A1 X Max", x => x.xR.Max - 2},
                { "A1 X Min", x => x.xR.Min + 2},
            };
        }

        protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
        {
            base.UpdateRefs(fsm, refs);
            base.UpdateRefs(attackCommands, CommandFloatRefs);
        }

        protected virtual void OnEnable()
        {
            BuildArena();
        }

        protected override IEnumerator Start()
        {
            GameObject comb = attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
            comb.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.006f);

            PlayMakerFSM combControl = comb.LocateMyFSM("Control");
            combControl.GetFirstActionOfType<SetPosition>("TL").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("TR").x = bounds.xMax;
            combControl.GetFirstActionOfType<RandomFloat>("Top").min = bounds.center.x - 1;
            combControl.GetFirstActionOfType<RandomFloat>("Top").max = bounds.center.x + 1;
            combControl.GetFirstActionOfType<SetPosition>("Top").y = bounds.yMax;
            combControl.GetFirstActionOfType<SetPosition>("L").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("L").y = bounds.center.y;
            combControl.GetFirstActionOfType<SetPosition>("R").x = bounds.xMax;
            combControl.GetFirstActionOfType<SetPosition>("R").y = bounds.center.y;

            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
            control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

            AddResetToStateOnHide(control, "Init");

            var climbPlatsState = control.GetState("Climb Plats1");
            climbPlatsState.Actions = new FsmStateAction[] {
                new CustomFsmAction(() => Destroy(gameObject))
            };

            if (!HeroInAggroRange())
                Hide();

            yield return UpdateAggroRange();
        }

        protected override bool HeroInAggroRange()
        {
            var size = new Vector2(30f, 30f);
            var center = new Vector2(transform.position.x, transform.position.y);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            return (dist.sqrMagnitude < size.sqrMagnitude);
        }

        protected override void Show()
        {
            base.Show();

            if (control.ActiveStateName == "Hidden")
            {
                control.SendEvent("SHOW");
                attackCommands.SetState("Init");
            }
        }
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
    public class MegaMossChargerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mossy Control";

        public override string FSMHiddenStateName => "MOSS_HIDDEN";

        public bool didInit = false;
        public bool isVisible = false;
        public bool isRunning = false;
        public bool isLeaping = false;

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
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        protected override void SetupBossAsNormalEnemy()
        {
            base.SetupBossAsNormalEnemy();

            //disable player data bool related things
            var control = gameObject.LocateMyFSM("Mossy Control");
            var initState = control.GetState("Init");
            initState.DisableAction(28);
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP * 2;
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
            if (edgeR - edgeL < (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale * 2f))
            {
                float ratio = (edgeR - edgeL) / (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale);
                thisMetadata.ApplySizeScale(Mathf.Max(ratio * .25f, .1f));
            }

            //for the aggro range, allow the full y size without scaling
            aggroGroundYSize = thisMetadata.ObjectSize.y * 5f;
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

            var initState = control.GetState("Init");
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
            hs.DisableAction(0);
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
                isLeaping = false;
            }, 0);

            //change the manual reset states to point to our custom hidden state
            control.ChangeTransition("Play Range", "FINISHED", FSMHiddenStateName);
            control.ChangeTransition("Hero Beyond?", "CANCEL", FSMHiddenStateName);

            DisableSendEvents(control,
                ("Emerge", 0),
                ("Leap Start", 0),
                ("Land", 4)
                );

            var leapStart = control.GetState("Leap Start");
            var eRight = control.GetState("Emerge Right");
            var eLeft = control.GetState("Emerge Left");

            eRight.ChangeTransition("FINISHED", "Attack Choice");
            eLeft.ChangeTransition("FINISHED", "Attack Choice");

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


            {
                leapStart.GetAction<SetPosition>(5).space = Space.World;
                leapStart.AddTransition("RECHOOSE", "Left or Right?");

                leapStart.InsertCustomAction(() =>
                {
                    //impossible leap
                    float leapSize = (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale * 3);
                    if (edgeL + leapSize > edgeR - leapSize)
                    {
                        control.SendEvent("RECHOOSE");
                        return;
                    }

                    isLeaping = true;
                }, 0);
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
            customHide.AddTransition("DO_SUBMERGE", "Submerge 1");
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

            var inAir = control.GetState("In Air");
            {
                inAir.DisableAction(1);

                var collisionAction = new HutongGames.PlayMaker.Actions.DetectCollisonDown();
                collisionAction.collision = PlayMakerUnity2d.Collision2DType.OnCollisionEnter2D;
                collisionAction.collideTag = "";
                collisionAction.sendEvent = new FsmEvent("LAND");

                inAir.AddAction(collisionAction);
            }

            var land = control.GetState("Land");
            {
                land.InsertCustomAction(() =>
                {
                    isLeaping = false;
                }, 0);
            }
        }

        protected override void Hide()
        {
            //can't hide when leaping
            if (isLeaping)
                return;

            base.Hide();
        }

        protected override void UpdateCustomRefs()
        {
            base.UpdateCustomRefs();
            UpdateRefs(control, MossyFloatsRefs);
        }
    }


    public class MegaMossChargerSpawner : DefaultSpawner<MegaMossChargerControl> { }

    public class MegaMossChargerPrefabConfig : DefaultPrefabConfig<MegaMossChargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   


    public class MawlekBodyControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mawklek Control";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => new Dictionary<string, Func<FSMAreaControlEnemy, float>>();

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }

        protected override IEnumerator Start()
        {
            control.SetState("Init");
            control.GetState("Wake Land").AddCustomAction(() => control.SetState("Start"));

            yield return new WaitWhile(() => control.ActiveStateName != "Dormant");

            yield return base.Start();
        }

        protected override void Show()
        {
            base.Show();
            control.SendEvent("WAKE");
        }

        protected override void Hide()
        {
            base.Hide();
        }
    }

    public class MawlekBodySpawner : DefaultSpawner<MawlekBodyControl>
    {
    }

    public class MawlekBodyPrefabConfig : DefaultPrefabConfig<MawlekBodyControl>
    {
    }



    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GhostWarriorMarmuControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";
    }

    public class GhostWarriorMarmuSpawner : DefaultSpawner<GhostWarriorMarmuControl>
    {
    }

    public class GhostWarriorMarmuPrefabConfig : DefaultPrefabConfig<GhostWarriorMarmuControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Set Ghost PD Int");
                GameObject.Destroy(fsm);
            }

            {
                var fsm = p.prefab.LocateMyFSM("FSM");
                GameObject.Destroy(fsm);
            }
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JarCollectorControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

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

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt((float)defaultHP / 10f);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

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

            control.ChangeTransition("Start Land", "FINISHED", "Roar End");

            var ec = control.GetState("Enemy Count");
            ec.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
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
            spawnAction.gameObject.Value = jarPrefab;
            spawnAction.spawnPoint.Value = gameObject;
            spawnAction.position.Value = Vector3.zero; //offset from boss (might update later)

            spawn.AddAction(spawnAction);
            spawn.AddCustomAction(() =>
            {
                var go = spawnAction.storeObject.Value;
                var jar = go.GetComponent<SpawnJarControl>();
                var selectedSpawn = possibleSpawns.GetRandomElementFromList(rng);
                jar.SetEnemySpawn(selectedSpawn.Item1.prefab, selectedSpawn.Item2);
                FlingUtils.FlingObject(new FlingUtils.SelfConfig
                {
                    Object = go,
                    SpeedMin = 20f,
                    SpeedMax = 30f,
                    AngleMin = 70f,
                    AngleMax = 110f
                }, gameObject.transform, Vector3.zero);
                var body = jar.GetComponent<Rigidbody2D>();
                if (body != null)
                    body.angularVelocity = 15f;
            });
            spawn.AddCustomAction(() => control.SendEvent("FINISHED"));

            this.InsertHiddenState(control, "Init", "FINISHED", "Start Fall");
            this.AddResetToStateOnHide(control, "Init");

            var fsm = gameObject.LocateMyFSM("Death");
            fsm.ChangeTransition("Init", "DEATH", "Fall");

            var phase = gameObject.LocateMyFSM("Phase Control");
            GameObject.Destroy(phase);
        }
    }

    public class JarCollectorSpawner : DefaultSpawner<JarCollectorControl> { }

    public class JarCollectorPrefabConfig : DefaultPrefabConfig<JarCollectorControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class InfectedKnightControl : DefaultSpawnedEnemyControl
    {
        Range xR = new Range(17.36f, 36.46f);
        Range yR = new Range(32.16f, 37.42f);

        public float XMIN { get => spawnPos.x - xR.Min; }
        public float XMAX { get => spawnPos.x + xR.Max; }
        public float YMIN { get => spawnPos.y - yR.Min; }
        public float YMAX { get => spawnPos.y + yR.Max; }

        public PlayMakerFSM control;
        public PlayMakerFSM balloonFSM;
        public Vector3 spawnPos;

        float dist { get => (HeroController.instance.transform.position - transform.position).magnitude; }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        IEnumerator Start()
        {
            if (control == null)
                yield break;

            control.enabled = false;

            while (dist > 50f)
            {
                yield return new WaitForEndOfFrame();
            }

            transform.position = transform.position - new Vector3(0f, -4f * transform.localScale.y, 0f);

            control.enabled = true;

            if (balloonFSM == null)
                yield break;

            var spawn = balloonFSM.GetState("Spawn");

            var origin = spawnPos;
            for (; ; )
            {
                spawnPos = transform.position;

                control.Fsm.GetFsmFloat("Air Dash Height").Value = YMIN + 3;
                control.Fsm.GetFsmFloat("Left X").Value = XMIN;
                control.Fsm.GetFsmFloat("Min Dstab Height").Value = YMIN + 5;
                control.Fsm.GetFsmFloat("Right X").Value = XMAX;

                control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").min = origin.x - 1;
                control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").max = origin.x + 1;
                control.GetFirstActionOfType<SetPosition>("Set Pos").x = transform.position.x;
                control.GetFirstActionOfType<SetPosition>("Set Pos").y = transform.position.y;

                balloonFSM.Fsm.Variables.GetFsmFloat("X Max").Value = XMAX;
                balloonFSM.Fsm.Variables.GetFsmFloat("X Min").Value = XMIN;
                balloonFSM.Fsm.Variables.GetFsmFloat("Y Max").Value = YMAX;
                balloonFSM.Fsm.Variables.GetFsmFloat("Y Min").Value = YMIN;
                yield return new WaitForSeconds(5f);
            }
        }
    }

    public class InfectedKnightSpawner : DefaultSpawner<InfectedKnightControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var ik = go.GetComponent<InfectedKnightControl>();
            ik.control = go.LocateMyFSM("IK Control");
            ik.spawnPos = source.ObjectPosition;

            if (source.IsBoss)
            {
                var fsm = go.LocateMyFSM("Spawn Balloon");
                ik.balloonFSM = fsm;
            }

            return go;
        }
    }


    public class InfectedKnightPrefabConfig : DefaultPrefabConfig<InfectedKnightControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("IK Control");
                fsm.ChangeTransition("Init", "FINISHED", "Idle");
                fsm.ChangeTransition("Init", "ACTIVE", "Idle");
            }
        }
    }
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

            thisMetadata.EnemyHealthManager.hp = other.MaxHP * 2 + 1;

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
            this.AddResetToStateOnHide(control, "Init");

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Right X" , x => edgeR},
                {"Left X" , x => edgeL},
            };
        }

        protected override bool HeroInAggroRange()
        {
            return (heroPos2d - pos2d).magnitude < 100f;
        }
    }

    public class GreyPrinceSpawner : DefaultSpawner<GreyPrinceControl>
    {
    }

    public class GreyPrincePrefabConfig : DefaultPrefabConfig<GreyPrinceControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class GiantFlyControl : MonoBehaviour
    {
        //default
        public static int babiesToSpawn = 7;

        static string MODHOOK_BeforeSceneLoad(string sceneName)
        {
            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            return sceneName;
        }

        void OnEnable()
        {
            ModHooks.BeforeSceneLoadHook -= MODHOOK_BeforeSceneLoad;
            ModHooks.BeforeSceneLoadHook += MODHOOK_BeforeSceneLoad;
            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        }

        public void SelfSpawnBabies()
        {
            SpawnBabies(gameObject);
        }

        static void SpawnBabies(GameObject owner)
        {
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
                        }
                        else
                        {
                            Dev.Log("trying to spawn via string");
                            result = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
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

    internal class GiantFlyPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;
            var control = p.prefab.AddComponent<GiantFlyControl>();
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Big Buzzer";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            DisableSendEvents(control,
                ("Roar Left", 0),
                ("Roar Right", 0)
                );

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                var left = db.Spawn("Buzzer", null);
                var right = db.Spawn("Buzzer", null);

                var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                var leftSpawn = pos2d + Vector2.left * 20f;
                var rightSpawn = pos2d + Vector2.right * 20f;

                var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                left.transform.position = leftShorter;
                right.transform.position = rightShorter;

                right.SetActive(true);
                left.SetActive(true);

                control.SendEvent("FINISHED");
            });

            control.GetState("Idle").InsertCustomAction(() =>
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }, 0);
        }

        protected virtual void Update()
        {
            if (gameObject.activeInHierarchy && control.enabled)
            {
                //TODO: temp hack to try and fix their colliders being broken
                gameObject.GetComponents<Collider2D>().ToList().ForEach(x => x.enabled = true);
            }
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt(defaultHP / 2f);
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
    public class GiantBuzzerColControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Big Buzzer";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt(previousHP * 2f);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            DisableSendEvents(control,
                ("Roar Left", 0),
                ("Roar Right", 0)
                );

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                var left = db.Spawn("Buzzer", null);
                var right = db.Spawn("Buzzer", null);

                var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                var leftSpawn = pos2d + Vector2.left * 20f;
                var rightSpawn = pos2d + Vector2.right * 20f;

                var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                left.transform.position = leftShorter;
                right.transform.position = rightShorter;

                right.SetActive(true);
                left.SetActive(true);

                control.SendEvent("FINISHED");
            });

            summon.RemoveTransition("GG BOSS");

            control.ChangeTransition("Init", "FINISHED", "Idle");
            control.ChangeTransition("Init", "GG BOSS", "Idle");

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");

            control.GetState("Idle").InsertCustomAction(() =>
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }, 0);
        }


        protected virtual void Update()
        {
            if (gameObject.activeInHierarchy && control.enabled)
            {
                //TODO: temp hack to try and fix their colliders being broken
                gameObject.GetComponents<Collider2D>().ToList().ForEach(x => x.enabled = true);
            }
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
    public class FalseKnightDreamControl : FSMAreaControlEnemy
    {
        public override string FSMName => "FalseyControl";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
            };
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected override IEnumerator Start()
        {
            control.GetFirstActionOfType<SetPosition>("Dormant").y = yR.Max;
            control.GetFirstActionOfType<GGCheckIfBossScene>("Dormant").regularSceneEvent = new FsmEvent("BATTLE START");

            //skip the music and title activation state
            control.ChangeTransition("Check", "JUMP", "Idle");
            control.ChangeTransition("Init", "FINISHED", "Dormant");

            //make it go to the death anim right away
            control.GetFirstActionOfType<IntCompare>("Check GG").integer2.Value = 0;
            control.GetFirstActionOfType<GGCheckIfBossScene>("Check GG").regularSceneEvent = new FsmEvent("GG BOSS");

            control.GetFirstActionOfType<GGCheckIfBossScene>("Dream Return").regularSceneEvent = new FsmEvent("GG BOSS");

            yield return base.Start();
        }

        protected override void Show()
        {
            base.Show();

            if (control.ActiveStateName == "Dormant")
                control.SendEvent("BATTLE START");
        }
    }


    public class FalseKnightDreamSpawner : DefaultSpawner<FalseKnightNewControl>
    {
    }

    public class FalseKnightDreamPrefabConfig : DefaultPrefabConfig<FalseKnightNewControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class FalseKnightNewControl : FSMAreaControlEnemy
    {
        public override string FSMName => "FalseyControl";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
            };
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected override IEnumerator Start()
        {
            control.GetFirstActionOfType<SetPosition>("Dormant").y = yR.Max;
            control.GetFirstActionOfType<GGCheckIfBossScene>("Dormant").regularSceneEvent = new FsmEvent("BATTLE START");

            //skip the music and title activation state
            control.ChangeTransition("Check", "JUMP", "First Idle");

            //make it go to the death anim right away
            control.GetFirstActionOfType<IntCompare>("Check If GG").integer2.Value = 0;
            control.GetFirstActionOfType<GGCheckIfBossScene>("Check If GG").regularSceneEvent = new FsmEvent("GG BOSS");

            control.GetFirstActionOfType<GGCheckIfBossScene>("Open Map Shop and Journal").regularSceneEvent = new FsmEvent("FINISHED");
            control.GetFirstActionOfType<GGCheckIfBossScene>("Boss Death Sting").regularSceneEvent = new FsmEvent("FINISHED");
            control.GetFirstActionOfType<GGCheckIfBossScene>("Boss Death Sting").regularSceneEvent = new FsmEvent("FINISHED");

            yield return base.Start();
        }
    }

    public class FalseKnightNewSpawner : DefaultSpawner<FalseKnightNewControl>
    {
    }

    public class FalseKnightNewPrefabConfig : DefaultPrefabConfig<FalseKnightNewControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   
    public class AbsoluteRadianceControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

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

        protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> CommandFloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Orb Max X", x => x.xR.Max - 1},
                { "Orb Max Y", x => x.yR.Max - 1},
                { "Orb Min X", x => x.xR.Min + 1},
                { "Orb Min Y", x => x.yR.Min + 3},
            };
        }

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "A1 X Max", x => x.xR.Max - 2},
                { "A1 X Min", x => x.xR.Min + 2},
            };
        }

        protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
        {
            base.UpdateRefs(fsm, refs);
            base.UpdateRefs(attackCommands, CommandFloatRefs);
        }

        protected virtual void OnEnable()
        {
            BuildArena();
        }

        protected override IEnumerator Start()
        {
            GameObject comb = attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
            comb.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.006f);

            PlayMakerFSM combControl = comb.LocateMyFSM("Control");
            combControl.GetFirstActionOfType<SetPosition>("TL").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("TR").x = bounds.xMax;
            combControl.GetFirstActionOfType<RandomFloat>("Top").min = bounds.center.x - 1;
            combControl.GetFirstActionOfType<RandomFloat>("Top").max = bounds.center.x + 1;
            combControl.GetFirstActionOfType<SetPosition>("Top").y = bounds.yMax;
            combControl.GetFirstActionOfType<SetPosition>("L").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("L").y = bounds.center.y;
            combControl.GetFirstActionOfType<SetPosition>("R").x = bounds.xMax;
            combControl.GetFirstActionOfType<SetPosition>("R").y = bounds.center.y;

            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
            control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

            AddResetToStateOnHide(control, "Init");

            var climbPlatsState = control.GetState("Climb Plats1");
            climbPlatsState.Actions = new FsmStateAction[] {
                new CustomFsmAction(() => Destroy(gameObject))
            };

            if (!HeroInAggroRange())
                Hide();

            yield return UpdateAggroRange();
        }

        protected override bool HeroInAggroRange()
        {
            var size = new Vector2(30f, 30f);
            var center = new Vector2(transform.position.x, transform.position.y);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            return (dist.sqrMagnitude < size.sqrMagnitude);
        }

        protected override void Show()
        {
            base.Show();

            if (control.ActiveStateName == "Hidden")
            {
                control.SendEvent("SHOW");
                attackCommands.SetState("Init");
            }
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





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////
}