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
    public class ZotelingControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var getLevel = control.GetState("Get Level");
            getLevel.DisableAction(0);
            getLevel.DisableAction(5);

            var init = control.GetState("Init");
            init.DisableAction(0);
            init.DisableAction(5);
            init.ChangeTransition("FINISHED", "Ball");

            var ball = control.GetState("Ball");
            ball.DisableAction(5);
            ball.DisableAction(7);
            ball.AddCustomAction(() => {
                control.SendEvent("FINISHED");
            });

            var die = control.GetState("Die");
            die.DisableAction(2);

            control.ChangeTransition("Die", "FINISHED", "DestroyGO");

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body != null)
                body.isKinematic = false;

            this.InsertHiddenState(control, "Get Level", "FINISHED", "Init");
            this.AddResetToStateOnHide(control, "Get Level");
        }
    }

    public class ZotelingSpawner : DefaultSpawner<ZotelingControl> { }

    public class ZotelingPrefabConfig : DefaultPrefabConfig<ZotelingControl> { }
}