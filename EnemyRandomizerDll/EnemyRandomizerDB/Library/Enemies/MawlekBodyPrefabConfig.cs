using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyRandomizerMod
{

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
}
