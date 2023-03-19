using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
using System;
using HutongGames.PlayMaker;
using Modding;
using System.Linq;
using HutongGames.PlayMaker.Actions;

namespace EnemyRandomizerMod
{
    public class GhostWarriorMarmuControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public Range xR;
        public Range yR;

        public void BuildArena(Vector3 spawnPoint)
        {
            gameObject.transform.position = spawnPoint;
            var hits = gameObject.GetNearestSurfaces(500f);
            xR = new Range(hits[Vector2.left].point.x, hits[Vector2.right].point.x);
            yR = new Range(hits[Vector2.down].point.y, hits[Vector2.up].point.y);

            
            UpdateRefs();
        }

        public Vector3 SpawnPoint;

        public Dictionary<string, Func<GhostWarriorMarmuControl, float>> FloatRefs = new Dictionary<string, Func<GhostWarriorMarmuControl, float>>()
        {
            { "Tele X", x => x.xR.RandomValuef(new RNG(HeroController.instance.transform.position.x.GetHashCode()))},
            { "Tele X Min", x => x.xR.Min},
            { "Tele X Max", x => x.xR.Max},
            { "Tele Y Max", x => x.yR.Max},
            { "Tele Y Min", x => x.yR.Min},
            { "Tele Y", x => x.yR.RandomValuef(new RNG(HeroController.instance.transform.position.y.GetHashCode()))},
        };

        public void UpdateRefs()
        {
            for (int i = 0; i < control.FsmVariables.FloatVariables.Length; ++i)
            {
                var v = control.FsmVariables.FloatVariables[i];
                if (FloatRefs.TryGetValue(v.Name, out var kvp))
                {
                    v.Value = kvp.Invoke(this);
                }
            }
        }

        public void UpdateHeroRefs()
        {
            //control.FsmVariables.GetFsmFloat("Hero X").Value = FloatRefs["Hero X"].Invoke(this);
            //control.FsmVariables.GetFsmFloat("Hero Y").Value = FloatRefs["Hero Y"].Invoke(this);
        }

        public float HeroX { get => HeroController.instance.transform.position.x; }
        public float HeroY { get => HeroController.instance.transform.position.y; }

        public float XMIN { get => xR.Min; }
        public float XMAX { get => xR.Max; }
        public float YMIN { get => yR.Min; }
        public float YMAX { get => yR.Max; }

        public float MidX { get => xR.Mid; }

        protected virtual void Hide()
        {
            if (!control.enabled)
                return;

            control.enabled = false;
        }

        protected virtual void Show()
        {
            if (control.enabled)
                return;

            control.enabled = true;
            BuildArena(SpawnPoint);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }

        IEnumerator Start()
        {
            BuildArena(SpawnPoint);
            if (control == null)
                yield break;

            control.enabled = false;

            for (; ; )
            {
                UpdateHeroRefs();

                if (HeroX < XMIN)
                    Hide();

                else if (HeroY < YMIN)
                    Hide();

                else if (HeroX > XMAX)
                    Hide();

                else if (HeroX > YMAX)
                    Hide();

                else
                    Show();

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public class GhostWarriorMarmuSpawner : DefaultSpawner<GhostWarriorMarmuControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<GhostWarriorMarmuControl>();
            fsm.control = go.LocateMyFSM("Control");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                var hm = go.GetComponent<HealthManager>();
                hm.hp = source.MaxHP;
            }

            return go;
        }
    }


    public class GhostWarriorMarmuPrefabConfig : DefaultPrefabConfig<GhostWarriorMarmuControl>
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
