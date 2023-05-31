using UnityEngine;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System;

namespace EnemyRandomizerMod
{

    public class ZoteTurretControl : DefaultSpawnedEnemyControl
    {
        public float startYPos;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            Geo = 12;

            control.OverrideState( "Pos", () =>
            {
                control.FsmVariables.GetFsmFloat("X Pos").Value = pos2d.x;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = pos2d.y;
                startYPos = pos2d.y;
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
            control.OverrideState( "Death Pause", () => { });
            control.OverrideState( "Retry Frame", () => { });

            this.InsertHiddenState(control, "Init", "FINISHED", "Pos");
            //this.AddResetToStateOnHide(control, "Retract");

            var retract = control.GetState("Retract");
            retract.ChangeTransition("FINISHED", "Init");
        }
    }

    public class ZoteTurretSpawner : DefaultSpawner<ZoteTurretControl>
    {
    }

    public class ZoteTurretPrefabConfig : DefaultPrefabConfig
    {
    }
}
