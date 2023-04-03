using System.Collections.Generic;
using System.Collections;
using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
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

            if(control.ActiveStateName == "Dormant")
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
}
