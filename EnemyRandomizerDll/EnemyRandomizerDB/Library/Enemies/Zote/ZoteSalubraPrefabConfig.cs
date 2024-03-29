﻿using System.Collections;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System;

namespace EnemyRandomizerMod
{
    public class ZoteSalubraControl : DefaultSpawnedEnemyControl
    {
        public float startYPos;
        public float maxSuck = 10f;

        public GameObject ptSuck;
        public GameObject ptSend;

        IEnumerator chasing;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            ptSuck = gameObject.FindGameObjectInDirectChildren("Pt Suck");
            ptSend = gameObject.FindGameObjectInDirectChildren("Pt Send");

            var init = control.GetState("Init");
            init.DisableAction(4);

            var appear = control.GetState("Appear");
            var appearSound = appear.GetAction<AudioPlayerOneShotSingle>(5);
            var appearAnim = appear.GetAction<Tk2dPlayAnimationWithEvents>(1);
            appear.RemoveTransition("RETRY");

            control.OverrideState( "Retry", () => { });

            control.OverrideState( "Appear", () =>
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = true;

                var xpos = gameObject.transform.position.x;
                control.FsmVariables.GetFsmFloat("X Pos").Value = xpos;

                float ypos = gameObject.transform.position.y;
                control.FsmVariables.GetFsmFloat("Y Pos").Value = ypos;

                startYPos = ypos;
                gameObject.transform.position = new Vector3(xpos, ypos, 0f);
                gameObject.GetComponent<Collider2D>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().enabled = true;

                if (chasing != null)
                {
                    StopCoroutine(chasing);
                }

                chasing = SpawnerExtensions.DistanceFlyChase(gameObject, HeroController.instance.gameObject, 8f, 0.5f, 6f, 2f);
                StartCoroutine(chasing);
            });

            appear.InsertAction(appearSound, 0);
            appear.InsertAction(appearAnim, 0);

            var idle = control.GetState("Idle");
            idle.GetFirstActionOfType<GhostMovement>().Enabled = false;

            //idle.InsertCustomAction(() => {

            //    var gm = idle.GetFirstActionOfType<GhostMovement>();
            //    gm.xPosMin = edgeL;
            //    gm.xPosMax = edgeR;
            //    gm.yPosMin = floorY;
            //    gm.yPosMax = roofY;

            //}, 0);


            var sucking = control.GetState("Sucking");
            sucking.GetFirstActionOfType<GhostMovement>().Enabled = false;

            //sucking.InsertCustomAction(() => {

            //    control.FsmVariables.GetFsmVector2("Hero Pos").Value = heroPos2d;

            //    var gm = idle.GetFirstActionOfType<GhostMovement>();
            //    gm.xPosMin = edgeL;
            //    gm.xPosMax = edgeR;
            //    gm.yPosMin = floorY;
            //    gm.yPosMax = roofY;

            //}, 0);

            var suck = control.GetState("Suck");

            suck.InsertCustomAction(() => {

                if(gameObject.DistanceToPlayer() > maxSuck)
                {
                    ptSuck.SetActive(false);
                    ptSend.SetActive(false);
                    control.SendEvent("FINISHED");
                }
                else
                {
                    ptSuck.SetActive(true);
                    ptSend.SetActive(true);
                }

            }, 0);

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            control.ChangeTransition("Death", "FINISHED", "DestroyGO");

            var death = control.GetState("Death");
            death.DisableAction(0);
            death.DisableAction(2);
            death.DisableAction(3);
            death.DisableAction(10);
            death.DisableAction(11);

            control.OverrideState("Dead", () => { });

            this.InsertHiddenState(control, "Init", "FINISHED", "Appear");
            //this.AddResetToStateOnHide(control, "Init");

        }
    }

    public class ZoteSalubraSpawner : DefaultSpawner<ZoteSalubraControl>
    {
    }

    public class ZoteSalubraPrefabConfig : DefaultPrefabConfig
    {
    }















}
