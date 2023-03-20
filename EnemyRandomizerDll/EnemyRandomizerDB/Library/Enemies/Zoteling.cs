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



    public class ZotelingControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body != null)
                body.isKinematic = false;
        }

        protected virtual void OnEnable()
        {
        }
    }

    public class ZotelingSpawner : DefaultSpawner<ZotelingControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZotelingControl>();
            fsm.control = go.LocateMyFSM("Control");

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


    public class ZotelingPrefabConfig : DefaultPrefabConfig<ZotelingControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");

                //remove the transitions related to chain spawning zotes for the event
                fsm.RemoveTransition("Dormant", "SPAWN");
                //fsm.RemoveTransition("Die", "FINISHED");
                //fsm.RemoveTransition("Respawn Pause", "SPAWN");
                fsm.RemoveTransition("Ball", "FINISHED");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Init", "FINISHED", "Choice");
                fsm.ChangeTransition("Reset", "FINISHED", "Dormant");

                //remove the states that were also part of that
                //fsm.Fsm.RemoveState("Dormant");
                //fsm.Fsm.RemoveState("Reset");
                //fsm.Fsm.RemoveState("Respawn Pause");
                //fsm.Fsm.RemoveState("Ball");
            }
        }
    }
}