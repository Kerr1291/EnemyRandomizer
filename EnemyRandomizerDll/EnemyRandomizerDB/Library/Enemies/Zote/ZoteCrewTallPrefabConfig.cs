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
    public class ZoteCrewTallControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => 1f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            Geo = 8;

            var init = control.GetState("Init");
            init.DisableAction(10);

            var mult = control.GetState("Multiply");
            mult.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Deaths").Value = 0;
            }, 0);

            var spawnAntic = control.GetState("Spawn Antic");
            spawnAntic.DisableAction(0);
            spawnAntic.DisableAction(2);
            spawnAntic.DisableAction(6);
            spawnAntic.DisableAction(7);

            var activated = control.GetState("Activate");
            activated.DisableAction(2);
            activated.DisableAction(3);
            activated.GetAction<SetDamageHeroAmount>(4).damageDealt = 1;

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            var reset = control.GetState("Death Reset");
            reset.RemoveTransition("FINISHED");

            var death = control.GetState("Death");
            death.DisableAction(0);
            death.DisableAction(2);
            death.DisableAction(3);
            death.DisableAction(4);
            death.DisableAction(5);
            death.DisableAction(9);
            death.AddCustomAction(() => { GameObject.Destroy(gameObject); });

            control.OverrideState("Death Reset", () => { GameObject.Destroy(gameObject); });

            control.AddTimeoutAction(control.GetState("In Air"), "LAND", 4f);

            this.InsertHiddenState(control, "Init", "FINISHED", "Multiply");
            //this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class ZoteCrewTallSpawner : DefaultSpawner<ZoteCrewTallControl> { }
    public class ZoteCrewTallPrefabConfig : DefaultPrefabConfig { }
}
