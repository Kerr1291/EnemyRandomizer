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




}
