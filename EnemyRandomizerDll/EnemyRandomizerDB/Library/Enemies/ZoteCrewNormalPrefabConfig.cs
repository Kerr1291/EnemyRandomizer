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
    public class ZoteCrewNormalControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = other.MaxHP;
            thisMetadata.EnemyHealthManager.SetGeoSmall(geoRNG.Rand(2, 12));

            var init = control.GetState("Init");
            init.DisableAction(8);

            var spawnAntic = control.GetState("Spawn Antic");
            spawnAntic.DisableAction(0);
            control.FsmVariables.GetFsmFloat("X Pos").Value = pos2d.x;
            spawnAntic.GetAction<SetPosition>(1).y.Value = pos2d.y;
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

            this.OverrideState(control, "Death Reset", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Multiply");
            this.AddResetToStateOnHide(control, "Init");

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Shockwave Y" , x => floorY},
            };
        }

        protected override bool HeroInAggroRange()
        {
            return (heroPos2d - pos2d).magnitude < 50f;
        }
    }

    public class ZoteCrewNormalSpawner : DefaultSpawner<ZoteCrewNormalControl>
    {
    }

    public class ZoteCrewNormalPrefabConfig : DefaultPrefabConfig<ZoteCrewNormalControl>
    {
    }









}
