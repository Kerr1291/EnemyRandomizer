using System.Collections;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class ZoteSalubraControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;
        public Vector3 spawnLocation;

        public float aggroRange = 25f;
        public float xRange;
        public float yRange;
        public float maxxRange;
        public float maxyRange;

        PlayMakerFSM fsm;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
            spawnLocation = transform.position;
        }


        IEnumerator Start()
        {
            Dev.Where();
            fsm = gameObject.LocateMyFSM("Control");
            var idleState = fsm.GetState("Idle");
            {
                var ghostMovement = idleState.Actions.FirstOrDefault(x => typeof(GhostMovement).IsAssignableFrom(x.GetType()));

                FsmFloat xmin = ghostMovement.GetFieldValue<FsmFloat>("xPosMin");
                FsmFloat ymin = ghostMovement.GetFieldValue<FsmFloat>("yPosMin");
                FsmFloat xmax = ghostMovement.GetFieldValue<FsmFloat>("xPosMax");
                FsmFloat ymax = ghostMovement.GetFieldValue<FsmFloat>("yPosMax");

                xRange = xmax.Value - xmin.Value;
                yRange = ymax.Value - ymin.Value;

                //check if the ghost has less room to move
                var left = gameObject.GetPointOn(Vector2.left, xRange * .5f);
                var right = gameObject.GetPointOn(Vector2.right, xRange * .5f);

                xRange = right.x - left.x;

                xmin.Value = spawnLocation.x - xRange * .5f;
                ymin.Value = spawnLocation.y - yRange * .5f;
                xmax.Value = spawnLocation.x + xRange * .5f;
                ymax.Value = spawnLocation.y + yRange * .5f;

                //set the ghost to patrol around its new location
                //ghostMovement.SetFieldValue<FsmFloat>("xPosMin", spawnLocation.x - xRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("yPosMin", spawnLocation.y - yRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("xPosMax", spawnLocation.y + xRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("yPosMax", spawnLocation.y + yRange * .5f);
            }

            var suckingState = fsm.GetState("Sucking");
            {
                var ghostMovement = suckingState.Actions.FirstOrDefault(x => typeof(GhostMovement).IsAssignableFrom(x.GetType()));

                FsmFloat xmin = ghostMovement.GetFieldValue<FsmFloat>("xPosMin");
                FsmFloat ymin = ghostMovement.GetFieldValue<FsmFloat>("yPosMin");
                FsmFloat xmax = ghostMovement.GetFieldValue<FsmFloat>("xPosMax");
                FsmFloat ymax = ghostMovement.GetFieldValue<FsmFloat>("yPosMax");

                xRange = xmax.Value - xmin.Value;
                yRange = ymax.Value - ymin.Value;

                var left = gameObject.GetPointOn(Vector2.left, xRange * .5f);
                var right = gameObject.GetPointOn(Vector2.right, xRange * .5f);

                xRange = right.x - left.x;
                xRange = Mathf.Min(maxyRange, xRange);

                xmin.Value = spawnLocation.x - xRange * .5f;
                ymin.Value = spawnLocation.y - yRange * .5f;
                xmax.Value = spawnLocation.x + xRange * .5f;
                ymax.Value = spawnLocation.y + yRange * .5f;

                //set the ghost to patrol around its new location
                //ghostMovement.SetFieldValue<FsmFloat>("xPosMin", spawnLocation.x - xRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("yPosMin", spawnLocation.y - yRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("xPosMax", spawnLocation.y + xRange * .5f);
                //ghostMovement.SetFieldValue<FsmFloat>("yPosMax", spawnLocation.y + yRange * .5f);
            }

            //check distance and move us into idle if player is far from spawn
            var hero = HeroController.instance;
            while (gameObject.SafeIsActive())
            {
                if ((hero.transform.position - spawnLocation).magnitude < aggroRange)
                {
                    //keep the ghost near the player once active
                    {
                        FsmFloat xmin = suckingState.GetFieldValue<FsmFloat>("xPosMin");
                        FsmFloat ymin = suckingState.GetFieldValue<FsmFloat>("yPosMin");
                        FsmFloat xmax = suckingState.GetFieldValue<FsmFloat>("xPosMax");
                        FsmFloat ymax = suckingState.GetFieldValue<FsmFloat>("yPosMax");

                        var heroPos = hero.transform.position;
                        xmin.Value = heroPos.x - xRange * .5f;
                        ymin.Value = heroPos.y - yRange * .5f;
                        xmax.Value = heroPos.x + xRange * .5f;
                        ymax.Value = heroPos.y + yRange * .5f;
                    }

                    if (fsm.ActiveStateName == "Idle")
                        fsm.SendEvent("PLAYER_NEAR");
                }
                else
                {

                    if (fsm.ActiveStateName == "Sucking")
                        fsm.SendEvent("PLAYER_FAR");
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ZoteSalubraSpawner : DefaultSpawner<ZoteSalubraControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZoteSalubraControl>();
            fsm.control = go.LocateMyFSM("Control");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
    }

    public class ZoteSalubraPrefabConfig : DefaultPrefabConfig<ZoteSalubraControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");

                //remove the transitions related to chain spawning zotes for the event
                fsm.RemoveTransition("Dormant", "START");
                fsm.RemoveTransition("Dead", "FINISHED");

                fsm.RemoveTransition("Appear", "RETRY");
                fsm.RemoveTransition("Retry", "FINISHED");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Init", "FINISHED", "Appear");

                //remove the states that are not needed
                //fsm.Fsm.RemoveState("Dormant");
                //fsm.Fsm.RemoveState("Retry");

                var appearState = fsm.GetState("Appear");
                appearState.Actions = appearState.Actions.Where(x =>
                {
                    return
                       !typeof(SetCircleCollider).IsAssignableFrom(x.GetType())
                    && !typeof(RandomFloat).IsAssignableFrom(x.GetType())
                    && !typeof(GetXDistance).IsAssignableFrom(x.GetType())
                    && !typeof(FloatCompare).IsAssignableFrom(x.GetType())
                    && !typeof(SetPosition).IsAssignableFrom(x.GetType());
                }).ToArray();

                var idleState = fsm.GetState("Idle");
                appearState.Actions = appearState.Actions.Where(x =>
                {
                    return !typeof(Wait).IsAssignableFrom(x.GetType());
                }).ToArray();

                fsm.AddTransition("Idle", "PLAYER_NEAR", "Sucking");
                fsm.AddTransition("Sucking", "PLAYER_FAR", "Idle");
                fsm.RemoveTransition("Idle", "FINISHED");
            }
        }
    }















}
