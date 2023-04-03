using System.Collections.Generic;
using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;
namespace EnemyRandomizerMod
{
    public class LostKinControl : FSMAreaControlEnemy
    {
        public PlayMakerFSM balloonFSM;

        public override string FSMName => "IK Control";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
            };
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
                fallpos.x.Value = SpawnPoint.x;
                fallpos.y.Value = SpawnPoint.y + 4f;
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
}

