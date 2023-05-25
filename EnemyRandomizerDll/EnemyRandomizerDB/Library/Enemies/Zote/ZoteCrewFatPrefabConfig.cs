using HutongGames.PlayMaker;
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

    public class ZoteCrewFatControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var init = control.GetState("Init");
            init.DisableActions(8);

            var mult = control.GetState("Multiply");
            mult.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Deaths").Value = 0;
            }, 0);

            var spawnAntic = control.GetState("Spawn Antic");
            spawnAntic.DisableAction(0);
            spawnAntic.DisableAction(1);
            //control.FsmVariables.GetFsmFloat("X Pos").Value = pos2d.x;
            //spawnAntic.GetAction<SetPosition>(1).y.Value = pos2d.y;
            spawnAntic.DisableAction(2);
            spawnAntic.DisableAction(6);
            spawnAntic.DisableAction(7);

            var activated = control.GetState("Activate");
            activated.DisableAction(2);
            activated.DisableAction(3);
            activated.GetAction<SetDamageHeroAmount>(4).damageDealt = 1;


            var landWaves = control.GetState("Land Waves");
            landWaves.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Shockwave Y").Value = floorY;
            }, 0);

            var land = control.GetState("Land");
            land.DisableAction(3);

            var dr = control.GetState("Dr");
            dr.DisableAction(1);
            dr.AddCustomAction(() => {

                RNG rng = new RNG();
                rng.Reset();

                bool left = rng.Randf() > .5f;
                if (left)
                    control.SendEvent("L");
                else
                    control.SendEvent("R");
            });

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            var reset = control.GetState("Death Reset");
            reset.RemoveTransition("FINISHED");

            var death = control.GetState("Death");
            death.DisableAction(0);
            death.DisableAction(3);
            death.DisableAction(4);
            death.DisableAction(5);
            death.DisableAction(6);
            death.DisableAction(7);
            death.DisableAction(8);
            death.DisableAction(10);
            death.AddCustomAction(() => { GameObject.Destroy(gameObject); });

            control.OverrideState("Death Reset", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Multiply");
            //this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class ZoteCrewFatSpawner : DefaultSpawner<ZoteCrewFatControl> { }

    public class ZoteCrewFatPrefabConfig : DefaultPrefabConfig { }
}
