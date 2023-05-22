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
    public class ZoteThwompControl : FSMAreaControlEnemy
    {
        PlayMakerFSM slamEffect;
        PlayMakerFSM enemyCrusher;
        PlayMakerFSM armourHit;

        public float startYPos;
        DamageEnemies damageEnemies;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var de = gameObject.GetComponent<DamageEnemies>();
            if (de != null)
                GameObject.Destroy(de);

            //allow the zote to squash things
            damageEnemies = gameObject.GetOrAddComponent<DamageEnemies>();
            damageEnemies.damageDealt = 10;
            damageEnemies.direction = 90f; //TODO: make sure this isn't backwards
            damageEnemies.magnitudeMult = 1f;
            damageEnemies.moveDirection = false;
            damageEnemies.circleDirection = false;
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Generic;
            damageEnemies.specialType = 0;


            slamEffect = gameObject.FindGameObjectInChildrenWithName("Slam Effect").LocateMyFSM("FSM");
            enemyCrusher = gameObject.FindGameObjectInChildrenWithName("Enemy Crusher").LocateMyFSM("FSM");
            armourHit = gameObject.FindGameObjectInChildrenWithName("Armour Hit").LocateMyFSM("Effect Control");

            slamEffect.ChangeTransition("Set Rotation?", "FINISHED", "Wait");
            slamEffect.ChangeTransition("Wait", "FINISHED", "Destroy");

            //control.ChangeTransition("Init", "FINISHED", "Set Pos");

            control.OverrideState( "Set Pos", () =>
            {
                var telepos = gameObject.GetRandomPositionInLOSofSelf(5, 50, 5f, 5f);

                control.FsmVariables.GetFsmFloat("X Pos").Value = telepos.x;
                //float ypos = roofY - this.thisMetadata.ObjectSize.y * this.thisMetadata.SizeScale;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = telepos.y;
                startYPos = telepos.y;
                gameObject.transform.position = new Vector3(telepos.x, telepos.y, 0f);
                gameObject.GetComponent<Collider2D>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            });

            var rise = control.GetState("Rise");
            rise.DisableAction(5);
            rise.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Y Pos").Value = startYPos;
            }, 5);

            control.OverrideState( "Out", () =>
            {
                var telepos = gameObject.GetRandomPositionInLOSofSelf(5, 50, 5f, 5f);
                control.FsmVariables.GetFsmFloat("X Pos").Value = telepos.x;
                //float ypos = roofY - this.thisMetadata.ObjectSize.y * this.thisMetadata.SizeScale;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = telepos.y;
                startYPos = telepos.y;
                gameObject.transform.position = new Vector3(heroPos2d.x, telepos.y, 0f);
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            });

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            control.ChangeTransition("Break", "FINISHED", "DestroyGO");

            var breakState = control.GetState("Break");
            breakState.DisableAction(1);

            var breakAntic = control.GetState("Break Antic");
            breakAntic.DisableAction(1);

            this.InsertHiddenState(control, "Init", "FINISHED", "Set Pos");
            //this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class ZoteThwompSpawner : DefaultSpawner<ZoteThwompControl> { }

    public class ZoteThwompPrefabConfig : DefaultPrefabConfig { }































}
