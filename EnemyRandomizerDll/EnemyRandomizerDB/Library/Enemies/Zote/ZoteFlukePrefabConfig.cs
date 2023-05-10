﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Linq;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System;

namespace EnemyRandomizerMod
{
    public class ZoteFlukeControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.Geo = 21;

            var init = control.GetState("Init");
            init.DisableAction(2);

            this.OverrideState(control, "Pos", () => {
                transform.position = thisMetadata.ObjectPosition;
            });

            var climb = control.GetState("Climb");
            //climb.DisableAction(0);

            climb.InsertCustomAction(() => {

                //control.FsmVariables.GetFsmVector2("Hero Pos").Value = heroPos2d;

                var gm = climb.GetFirstActionOfType<GhostMovement>();
                gm.xPosMin = edgeL;
                gm.xPosMax = edgeR;
                gm.yPosMin = floorY;
                gm.yPosMax = roofY;

            }, 0);

            climb.DisableAction(3);
            climb.AddCustomAction(() => { control.SendEvent("END"); });

            var suck = control.AddState("Suck");
            climb.ChangeTransition("END", "Suck");
            suck.AddTransition("FINISHED", "Climb");
            suck.AddCustomAction(() => {

                var dist = (heroPos2d - pos2d).magnitude;
                if(dist < 5f)
                {
                    HeroController.instance.TakeMPQuick(1);
                }

            });
            suck.AddAction(new Wait() { time = 0.5f, finishEvent = new FsmEvent("FINISHED") });

            var death = control.GetState("Death");
            death.DisableAction(4);
            death.DisableAction(5);
            death.DisableAction(6);
            death.AddCustomAction(() => { GameObject.Destroy(gameObject); });

            this.OverrideState(control, "Sleep", () => { GameObject.Destroy(gameObject); });
            this.OverrideState(control, "Sleeping", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Pos");
            this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class ZoteFlukeSpawner : DefaultSpawner<ZoteFlukeControl>
    {
    }
    public class ZoteFlukePrefabConfig : DefaultPrefabConfig<ZoteFlukeControl>
    {
    }


}
