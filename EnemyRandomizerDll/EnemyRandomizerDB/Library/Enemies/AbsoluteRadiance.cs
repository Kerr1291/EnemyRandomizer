using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EnemyRandomizerMod.Futils;

namespace EnemyRandomizerMod
{
    public class AbsoluteRadianceControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        public Rect bounds;

        public PlayMakerFSM commands;

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
            base.UpdateRefs(commands, CommandFloatRefs);
        }

        protected override void BuildArena(Vector3 spawnPoint)
        {
            base.BuildArena(spawnPoint);
            bounds = new Rect(spawnPoint.x, spawnPoint.y, xR.Size, yR.Size);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            commands = gameObject.LocateMyFSM("Attack Commands");
        }

        protected virtual void OnEnable()
        {
            BuildArena(gameObject.transform.position);
        }

        protected override IEnumerator Start()
        {
            GameObject comb = commands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
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

            commands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            commands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            commands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
            control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

            bool replaced = false;
            var actions = control.GetState("Climb Plats").Actions;
            control.GetState("Climb Plats").Actions =
                actions.Select(x =>
                {
                    if (!replaced && x == actions.First())
                    {
                        replaced = true;
                        return new CustomFsmAction(() => Destroy(gameObject));
                    }
                    return x;
                }).ToArray();

            if (!HeroInAggroRange())
                Hide();

            for (; ; )
            {
                UpdateHeroRefs();

                if (HeroInAggroRange())
                    Show();
                else
                    Hide();

                if(control.ActiveStateName == "Intro End")
                    control.SetState("Arena 1 Idle");

                yield return new WaitForSeconds(1f);
            }
        }

        protected override void Show()
        {
            base.Show();
            commands.SetState("Init");
            control.SetState("Init");
        }
    }

    public class AbsoluteRadianceSpawner : DefaultSpawner<AbsoluteRadianceControl>
    {
    }

    public class AbsoluteRadiancePrefabConfig : DefaultPrefabConfig<AbsoluteRadianceControl>
    {
    }
}
