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
using Satchel;
using Satchel.Futils;
namespace EnemyRandomizerMod
{
    public class ZotelingControl : FSMAreaControlEnemy
    {
        public override string spawnEntityOnDeath => "Item Get Effect R";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            SetupLevel();
            SetupDormant();
            SetupBall();
            SetupTheRest();


            this.InsertHiddenState(control, "Get Level", "FINISHED", "Init");
        }

        void SetupLevel()
        {

            control.OverrideState( "Get Level", () => {
                control.FsmVariables.GetFsmInt("Level").Value = 1;
                control.FsmVariables.GetFsmInt("Damage").Value = 1;
                control.SendEvent("FINISHED");
            });
        }

        void SetupDormant()
        {
            control.OverrideState( "Dormant", () => {
                control.SendEvent("SPAWN");
            });

        }

        void SetupBall()
        {

            var ball = control.GetState("Ball");
            ball.DisableAction(5);
            ball.DisableAction(7);
            ball.AddCustomAction(() => {
                control.SendEvent("FINISHED");
            });
        }

        void SetupTheRest()
        {

            control.OverrideState( "Reset", () => { Destroy(gameObject); });

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body != null)
                body.isKinematic = false;
        }
    }

    public class ZotelingSpawner : DefaultSpawner<ZotelingControl> { }

    public class ZotelingPrefabConfig : DefaultPrefabConfig { }
}