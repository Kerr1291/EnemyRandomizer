using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Satchel;

namespace EnemyRandomizerMod
{
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class OrdealZotelingControl : FSMAreaControlEnemy
    {
        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body != null)
                body.isKinematic = false;

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = other.DefaultHP;
            thisMetadata.EnemyHealthManager.SetGeoMedium(geoRNG.Rand(0, 5));
            thisMetadata.EnemyHealthManager.SetGeoSmall(geoRNG.Rand(1, 10));

            var init = control.GetState("Init");
            init.DisableAction(0);
            init.DisableAction(1);

            var ball = control.GetState("Ball");
            ball.DisableAction(1);
            control.FsmVariables.GetFsmFloat("X Pos").Value = pos2d.x;
            ball.GetAction<SetPosition>(2).y.Value = pos2d.y;
            ball.DisableAction(15);

            var dr = control.GetState("Dir");
            dr.DisableAction(0);
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

            //var reset = control.GetState("Reset");
            //reset.RemoveTransition("FINISHED");

            var death = control.GetState("Die");
            death.DisableAction(0);
            death.DisableAction(1);
            death.DisableAction(3);
            death.AddCustomAction(() => { GameObject.Destroy(gameObject); });

            this.OverrideState(control, "Reset", () => { GameObject.Destroy(gameObject); });
            this.OverrideState(control, "Respawn Pause", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Ball");
            //this.AddResetToStateOnHide(control, "Init");
        }
    }



    public class OrdealZotelingSpawner : DefaultSpawner<OrdealZotelingControl> { }

    public class OrdealZotelingPrefabConfig : DefaultPrefabConfig<OrdealZotelingControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////




}
