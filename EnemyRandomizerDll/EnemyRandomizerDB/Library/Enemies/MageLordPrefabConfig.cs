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
using HutongGames.PlayMaker.Actions;
namespace EnemyRandomizerMod
{
    public class MageLordControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mage Lord";

        protected override bool ControlCameraLocks => true;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            
            //setup default 'next event' state
            control.FsmVariables.GetFsmString("Next Event").Value = "IDLE";
        }

        protected override void BuildArena(Vector3 spawnPoint)
        {
            QuakeYPos = 0f;
            KnightQuakeYPos = 0f;
            base.BuildArena(spawnPoint);
            DrawArea();
        }

        List<GameObject> debugRenderers = new List<GameObject>();

        protected virtual void DrawArea()
        {
            ClearDebug();
            var pos = new Vector3(xR.Mid, yR.Mid);
            var size = new Vector3(xR.Size, yR.Size);
            UnityEngine.Bounds b = new UnityEngine.Bounds(pos, size);
            debugRenderers = b.CreateBoxOfLineRenderers(Color.cyan);
            debugRenderers.ForEach(x => x.transform.SetParent(transform.parent,false));
        }

        protected virtual void ClearDebug()
        {
            debugRenderers.ForEach(x => GameObject.Destroy(x));
            debugRenderers = new List<GameObject>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearDebug();
        }

        public static float ShockwaveYPos = 3.23f;
        public static float QuakeYPos = 2.15f;
        public static float KnightQuakeYPos = .44f;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
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
        }

        protected override void Show()
        {
            base.Show();
            Dev.Log("Showing");
        }

        protected override void Hide()
        {
            base.Show();
            Dev.Log("Hiding");
        }
    }

    public class MageLordSpawner : DefaultSpawner<MageLordControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);

            var badFSM = go.LocateMyFSM("Destroy If Defeated");
            GameObject.Destroy(badFSM);

            return go;
        }
    }

    public class MageLordPrefabConfig : DefaultPrefabConfig<MageLordControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Mage Lord");
                fsm.ChangeTransition("Init", "FINISHED", "Teleport");
                fsm.ChangeTransition("Init", "GG BOSS", "Teleport");
            }
        }
    }
}

