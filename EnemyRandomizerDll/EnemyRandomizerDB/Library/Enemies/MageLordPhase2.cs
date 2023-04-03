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
    public class MageLordPhase2Control : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public void BuildArena(Vector3 spawnPoint)
        {
            gameObject.transform.position = spawnPoint;
            var hits = gameObject.GetNearestSurfaces(500f);
            xR = new Range(hits[Vector2.left].point.x, hits[Vector2.right].point.x);
            yR = new Range(hits[Vector2.down].point.y, hits[Vector2.up].point.y);

            oxR = new Range(hits[Vector2.left].point.x + .5f, hits[Vector2.right].point.x - .5f);
            oyR = new Range(hits[Vector2.down].point.y + .5f, hits[Vector2.up].point.y - .5f);

            //don't spawn in the roof
            QuakeYPos = 0f;
            KnightQuakeYPos = 0f;

            UpdateRefs();
        }

        public static float ShockwaveYPos = 3.23f;
        public static float QuakeYPos = 2.15f;
        public static float KnightQuakeYPos = .44f;

        public Range xR = new Range(6.98f, 34.75f);
        public Range yR = new Range(31.05f, 35.85f);

        public Range oxR = new Range(6.98f, 34.75f);
        public Range oyR = new Range(31.05f, 35.85f);

        public Vector3 SpawnPoint;

        public Dictionary<string, Func<MageLordPhase2Control, float>> FloatRefs = new Dictionary<string, Func<MageLordPhase2Control, float>>()
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

            { "Orb Min X", x => x.oxR.Min},
            { "Orb Min Y", x => x.oyR.Min},
            { "Orb Max X", x => x.oxR.Max},
            { "Orb Max Y", x => x.oyR.Max},
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
            control.FsmVariables.GetFsmFloat("Hero X").Value = FloatRefs["Hero X"].Invoke(this);
            control.FsmVariables.GetFsmFloat("Hero Y").Value = FloatRefs["Hero Y"].Invoke(this);
        }

        public float HeroX { get => HeroController.instance.transform.position.x; }
        public float HeroY { get => HeroController.instance.transform.position.y; }

        public float XMIN { get => xR.Min; }
        public float XMAX { get => xR.Max; }
        public float YMIN { get => yR.Min; }
        public float YMAX { get => yR.Max; }

        public float MidX { get => xR.Mid; }

        IEnumerable<CameraLockArea> cams;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

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
            UnlockCameras(cams);
            BuildArena(SpawnPoint);
        }

        IEnumerator Start()
        {
            BuildArena(SpawnPoint);
            cams = GetCameraLocksFromScene();
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

        public IEnumerable<CameraLockArea> GetCameraLocksFromScene()
        {
            return gameObject.GetComponentsFromScene<CameraLockArea>();
        }

        public virtual void UnlockCameras(IEnumerable<CameraLockArea> cameraLocks)
        {
            foreach (var c in cameraLocks)
            {
                c.gameObject.SetActive(false);
            }
        }
    }

    public class MageLordPhase2Spawner : DefaultSpawner<MageLordPhase2Control>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<MageLordControl>();
            fsm.control = go.LocateMyFSM("Mage Lord 2");
            fsm.SpawnPoint = source.ObjectPosition;

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

    public class MageLordPhase2PrefabConfig : DefaultPrefabConfig<MageLordPhase2Control>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            //set it up to just instantly start
            {
                var fsm = p.prefab.LocateMyFSM("Mage Lord 2");
                fsm.ChangeTransition("Init", "PHASE 2", "Tele Quake");
                var init = fsm.GetState("Init");
                var last = (FloatCompare)init.Actions.Last();
                last.equal = last.lessThan;
                last.greaterThan = last.lessThan;
            }
        }
    }
}

