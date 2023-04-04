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

    public class ZoteBalloonControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = 1;
            thisMetadata.EnemyHealthManager.SetGeoSmall(geoRNG.Rand(1, 5));

            var init = control.GetState("Init");
            init.DisableAction(2);
            init.DisableAction(3);
            init.DisableAction(4);

            var setpos = control.GetState("Set Pos");
            setpos.RemoveTransition("ZERO HP");

            this.OverrideState(control, "Set Pos", () =>
            {
                RNG rng = new RNG();
                rng.Reset();
                gameObject.transform.localScale = new Vector3(rng.Rand(1f, 1.3f)*thisMetadata.SizeScale, rng.Rand(-4f,4f)*thisMetadata.SizeScale, gameObject.transform.localScale.z);

                var xpos = gameObject.transform.position.x;
                control.FsmVariables.GetFsmFloat("Pos X").Value = xpos;

                float ypos = gameObject.transform.position.y;
                control.FsmVariables.GetFsmFloat("Pos Y").Value = ypos;

                startYPos = ypos;
                gameObject.transform.position = new Vector3(xpos, ypos, 0f);
            });

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            var reset = control.GetState("Reset");
            reset.RemoveTransition("FINISHED");

            var death = control.GetState("Die");
            death.DisableAction(4);

            this.OverrideState(control, "Reset", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Spawn Pos");
            //this.AddResetToStateOnHide(control, "Init");

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Right X" , x => edgeR},
                //{"Left X" , x => edgeL},
                //{"TeleRange Max" , x => edgeR},
                //{"TeleRange Min" , x => edgeL},
                //{"PuppetSlam Y" , x => floorY},
            };
        }

        protected override bool HeroInAggroRange()
        {
            return (heroPos2d - pos2d).magnitude < 5f;
        }
    }

    public class ZoteBalloonSpawner : DefaultSpawner<ZoteBalloonControl>
    {
    }

    public class ZoteBalloonPrefabConfig : DefaultPrefabConfig<ZoteBalloonControl>
    {
    }





}
