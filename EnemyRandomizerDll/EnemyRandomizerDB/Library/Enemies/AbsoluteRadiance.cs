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
    public class AbsoluteRadianceControl : DefaultSpawnedEnemyControl
    {
        public Range xR;
        public Range yR;
        public Rect bounds;

        public PlayMakerFSM control;
        public PlayMakerFSM commands;

        public void BuildArena(Vector3 spawnPoint)
        {
            gameObject.transform.position = spawnPoint;
            var hits = gameObject.GetNearestSurfaces(500f);
            xR = new Range(hits[Vector2.left].point.x, hits[Vector2.right].point.x);
            yR = new Range(hits[Vector2.down].point.y, hits[Vector2.up].point.y);
            bounds = new Rect(spawnPoint.x, spawnPoint.y, xR.Size, yR.Size);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
            BuildArena(gameObject.transform.position);
        }

        private IEnumerator Start()
        {
            commands.Fsm.GetFsmFloat("Orb Max X").Value = bounds.xMax - 1;
            commands.Fsm.GetFsmFloat("Orb Max Y").Value = bounds.yMax - 1;
            commands.Fsm.GetFsmFloat("Orb Min X").Value = bounds.xMin + 1;
            commands.Fsm.GetFsmFloat("Orb Min Y").Value = bounds.yMin + 3;

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

            control.Fsm.GetFsmFloat("A1 X Max").Value = bounds.xMax - 2;
            control.Fsm.GetFsmFloat("A1 X Min").Value = bounds.xMin + 2;

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


            commands.SetState("Init");
            control.SetState("Init");

            yield return new WaitUntil(() => control.ActiveStateName == "Intro End");

            control.SetState("Arena 1 Idle");
        }
    }

    public class AbsoluteRadianceSpawner : DefaultSpawner<AbsoluteRadianceControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<AbsoluteRadianceControl>();
            fsm.control = go.LocateMyFSM("Control");
            fsm.commands = go.LocateMyFSM("Attack Commands");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
    }
    public class AbsoluteRadiancePrefabConfig : DefaultPrefabConfig<AbsoluteRadianceControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");
            }
        }
    }
}
