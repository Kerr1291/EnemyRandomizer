using UnityEngine;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System;

namespace EnemyRandomizerMod
{

    public class ZoteTurretControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = other.MaxHP;
            thisMetadata.EnemyHealthManager.SetGeoMedium(geoRNG.Rand(1, 5));

            this.OverrideState(control, "Pos", () =>
            {
                control.FsmVariables.GetFsmFloat("X Pos").Value = HeroX;
                float ypos = roofY;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = ypos;
                startYPos = ypos;
                gameObject.transform.position = new Vector3(HeroX, ypos, 0f);
                gameObject.GetComponent<Collider2D>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            });


            var pos = control.GetState("Pos");
            pos.RemoveTransition("RETRY");

            var appear = control.GetState("Appear");
            appear.DisableAction(4);
            appear.DisableAction(5);
            appear.DisableAction(6);

            var idle = control.GetState("Idle");
            idle.RemoveTransition("RETRACT");
            idle.DisableAction(3);
            idle.DisableAction(4);

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            control.ChangeTransition("Die", "FINISHED", "DestroyGO");
            var die = control.GetState("Die");
            die.DisableAction(4);
            die.DisableAction(2);
            die.DisableAction(3);
            die.DisableAction(6);

            //should never happen now
            this.OverrideState(control, "Death Pause", () => { });
            this.OverrideState(control, "Retry Frame", () => { });

            this.InsertHiddenState(control, "Init", "FINISHED", "Pos");
            this.AddResetToStateOnHide(control, "Retract");

            var retract = control.GetState("Retract");
            retract.ChangeTransition("FINISHED", "Init");

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
            return (heroPos2d - pos2d).magnitude < 50f;
        }
    }

    public class ZoteTurretSpawner : DefaultSpawner<ZoteTurretControl>
    {
    }

    public class ZoteTurretPrefabConfig : DefaultPrefabConfig<ZoteTurretControl>
    {
    }
}
