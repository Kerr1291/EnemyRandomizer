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
}
