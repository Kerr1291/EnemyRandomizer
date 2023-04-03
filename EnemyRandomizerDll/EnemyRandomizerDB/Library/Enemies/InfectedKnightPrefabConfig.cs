using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;
namespace EnemyRandomizerMod
{
    public class InfectedKnightControl : DefaultSpawnedEnemyControl
    {
        Range xR = new Range(17.36f, 36.46f);
        Range yR = new Range(32.16f, 37.42f);

        public float XMIN { get => spawnPos.x - xR.Min; }
        public float XMAX { get => spawnPos.x + xR.Max; }
        public float YMIN { get => spawnPos.y - yR.Min; }
        public float YMAX { get => spawnPos.y + yR.Max; }

        public PlayMakerFSM control;
        public PlayMakerFSM balloonFSM;
        public Vector3 spawnPos;

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

            while(dist > 50f)
            {
                yield return new WaitForEndOfFrame();
            }

            transform.position = transform.position - new Vector3(0f, -4f * transform.localScale.y, 0f);

            control.enabled = true;

            if (balloonFSM == null)
                yield break;

            var spawn = balloonFSM.GetState("Spawn");

            var origin = spawnPos;
            for (; ; )
            {
                spawnPos = transform.position;
                
                control.Fsm.GetFsmFloat("Air Dash Height").Value = YMIN + 3;
                control.Fsm.GetFsmFloat("Left X").Value = XMIN;
                control.Fsm.GetFsmFloat("Min Dstab Height").Value = YMIN + 5;
                control.Fsm.GetFsmFloat("Right X").Value = XMAX;

                control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").min = origin.x - 1;
                control.GetFirstActionOfType<RandomFloat>("Aim Jump 2").max = origin.x + 1;
                control.GetFirstActionOfType<SetPosition>("Set Pos").x = transform.position.x;
                control.GetFirstActionOfType<SetPosition>("Set Pos").y = transform.position.y;

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
            ik.spawnPos = source.ObjectPosition;

            if (source.IsBoss)
            {
                var fsm = go.LocateMyFSM("Spawn Balloon");
                ik.balloonFSM = fsm;
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

