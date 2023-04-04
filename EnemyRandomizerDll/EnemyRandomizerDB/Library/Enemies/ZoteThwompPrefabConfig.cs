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
    public class ZoteThwompControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        PlayMakerFSM slamEffect;
        PlayMakerFSM enemyCrusher;
        PlayMakerFSM armourHit;

        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var de = gameObject.GetComponent<DamageEnemies>();
            if (de != null)
                GameObject.Destroy(de);

            slamEffect = gameObject.FindGameObjectInChildrenWithName("Slam Effect").LocateMyFSM("FSM");
            enemyCrusher = gameObject.FindGameObjectInChildrenWithName("Enemy Crusher").LocateMyFSM("FSM");
            armourHit = gameObject.FindGameObjectInChildrenWithName("Armour Hit").LocateMyFSM("Effect Control");

            slamEffect.ChangeTransition("Set Rotation?", "FINISHED", "Wait");
            slamEffect.ChangeTransition("Wait", "FINISHED", "Destroy");

            //control.ChangeTransition("Init", "FINISHED", "Set Pos");

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = other.MaxHP;
            thisMetadata.EnemyHealthManager.SetGeoLarge(geoRNG.Rand(1, 5));

            this.OverrideState(control, "Set Pos", () =>
            {
                control.FsmVariables.GetFsmFloat("X Pos").Value = HeroX;
                float ypos = roofY - this.thisMetadata.ObjectSize.y * this.thisMetadata.SizeScale;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = ypos;
                startYPos = ypos;
                gameObject.transform.position = new Vector3(HeroX, ypos, 0f);
                gameObject.GetComponent<Collider2D>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            });

            var rise = control.GetState("Rise");
            rise.DisableAction(5);
            rise.InsertCustomAction(() => {
                control.FsmVariables.GetFsmFloat("Y Pos").Value = startYPos;
            }, 5);

            this.OverrideState(control, "Out", () =>
            {
                control.FsmVariables.GetFsmFloat("X Pos").Value = HeroX;
                float ypos = roofY - this.thisMetadata.ObjectSize.y * this.thisMetadata.SizeScale;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = ypos;
                startYPos = ypos;
                gameObject.transform.position = new Vector3(HeroX, ypos, 0f);
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
            this.AddResetToStateOnHide(control, "Init");


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
            return (heroPos2d - pos2d).magnitude < 25f;
        }
    }

    public class ZoteThwompSpawner : DefaultSpawner<ZoteThwompControl>
    {
    }

    public class ZoteThwompPrefabConfig : DefaultPrefabConfig<ZoteThwompControl>
    {
    }































}
