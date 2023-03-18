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
    public class InfectedKnightControl : DefaultSpawnedEnemyControl
    {
        Range xR = new Range(17.36f, 36.46f);
        Range yR = new Range(32.16f, 37.42f);

        public float XMIN { get => transform.position.x - xR.Min; }
        public float XMAX { get => transform.position.x + xR.Max; }
        public float YMIN { get => transform.position.y - yR.Min; }
        public float YMAX { get => transform.position.y + yR.Max; }

        public PlayMakerFSM control;
        public PlayMakerFSM balloonFSM;


        float dist { get => (HeroController.instance.transform.position - transform.position).magnitude; }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        IEnumerator Start()
        {
            if (control == null)
                yield break;

            control.enabled = false;

            while(dist > 100f)
            {
                yield return new WaitForEndOfFrame();
            }

            control.enabled = true;

            if (balloonFSM == null)
                yield break;

            var spawn = balloonFSM.GetState("Spawn");

            for (; ; )
            {
                balloonFSM.Fsm.Variables.GetFsmFloat("X Max").Value = XMAX;
                balloonFSM.Fsm.Variables.GetFsmFloat("X Min").Value = XMIN;
                balloonFSM.Fsm.Variables.GetFsmFloat("Y Max").Value = YMAX;
                balloonFSM.Fsm.Variables.GetFsmFloat("Y Min").Value = YMIN;
                yield return new WaitForSeconds(5f);
            }
        }
    }

    public class InfectedKnightSpawner : DefaultSpawner<InfectedKnightControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var ik = go.GetComponent<InfectedKnightControl>();
            ik.control = go.LocateMyFSM("IK Control");

            if(source.IsBoss)
            {
                var fsm = go.LocateMyFSM("Spawn Balloon");
                ik.balloonFSM = fsm;
            }
            else
            {
                var hm = go.GetComponent<HealthManager>();
                hm.hp = source.MaxHP;
            }

            return go;
        }
    }


    public class InfectedKnightPrefabConfig : DefaultPrefabConfig<InfectedKnightControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("IK Control");
                fsm.ChangeTransition("Init", "FINISHED", "Idle");
                fsm.ChangeTransition("Init", "ACTIVE", "Idle");
            }
        }
    }
}

