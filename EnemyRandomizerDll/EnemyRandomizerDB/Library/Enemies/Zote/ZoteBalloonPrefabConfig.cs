using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Linq;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System;
using UniRx;

namespace EnemyRandomizerMod
{

    public class ZoteBalloonControl : DefaultSpawnedEnemyControl
    {
        public override bool explodeOnDeath => true;

        public float startYPos;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            EnemyHealthManager.hp = 1;
            EnemyHealthManager.SetGeoSmall(geoRNG.Rand(1, 5));

            var init = control.GetState("Init");
            init.DisableAction(2);
            init.DisableAction(3);
            init.DisableAction(4);

            var setpos = control.GetState("Set Pos");
            setpos.RemoveTransition("ZERO HP");

            control.OverrideState("Set Pos", () =>
            {
                RNG rng = new RNG();
                rng.Reset();
                gameObject.transform.localScale = new Vector3(rng.Rand(1f, 1.3f)*SizeScale, rng.Rand(-4f,4f)*SizeScale, gameObject.transform.localScale.z);

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

            control.OverrideState( "Reset", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Spawn Pos");
            //this.AddResetToStateOnHide(control, "Init");

            //thisMetadata.EnemyHealthManager.OnDeath -= EnemyHealthManager_OnDeath;
            //thisMetadata.EnemyHealthManager.OnDeath += EnemyHealthManager_OnDeath;

            ////if hp changes at all, explode
            //CurrentHP.SkipLatestValueOnSubscribe().Subscribe(x => EnemyHealthManager_OnDeath()).AddTo(disposables);
        }

        //private void EnemyHealthManager_OnDeath()
        //{
        //    disposables.Clear();
        //    SpawnExplosion(gameObject, explosionEffect);
        //    thisMetadata.EnemyHealthManager.OnDeath -= EnemyHealthManager_OnDeath;
        //}

        //protected static void SpawnExplosion(GameObject gameObject, string effect)
        //{
        //    EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, effect, null, true);
        //}
    }

    public class ZoteBalloonSpawner : DefaultSpawner<ZoteBalloonControl>
    {
    }

    public class ZoteBalloonPrefabConfig : DefaultPrefabConfig
    {
    }





}
