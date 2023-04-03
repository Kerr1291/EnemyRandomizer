using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Linq;
using UnityEngine;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{

    public class ZoteBalloonControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public Vector3 spawnLocation;

        public float aggroRange = 25f;
        public float xRange = 115.34f - 91.11f;
        public float yRange = 11.41f - 8.26f;

        PlayMakerFSM fsm;

        FsmFloat xpos;
        FsmFloat ypos;

        public bool hasSpawned;

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
            hasSpawned = false;
            fsm = gameObject.LocateMyFSM("Control");
            var posState = fsm.GetState("Set Pos");
            {
                var setPosition = posState.Actions.FirstOrDefault(x => typeof(SetPosition).IsAssignableFrom(x.GetType()));

                xpos = setPosition.GetFieldValue<FsmFloat>("x");
                ypos = setPosition.GetFieldValue<FsmFloat>("y");

                //check how much room the thowmp has
                var left = gameObject.GetPointOn(Vector2.left, xRange * .5f);
                var right = gameObject.GetPointOn(Vector2.right, xRange * .5f);

                var top = gameObject.GetPointOn(Vector2.up, yRange * .5f);
                var bot = gameObject.GetPointOn(Vector2.down, yRange * .5f);

                xRange = right.x - left.x;
                yRange = top.y - bot.y;
                aggroRange = xRange * 2f;
            }

            //check distance and move us into idle if player is far from spawn
            var hero = HeroController.instance;
            while (gameObject.SafeIsActive())
            {
                if(thisMetadata.EnemyHealthManager.hp <= 2)
                {
                    Destroy(gameObject);
                    yield break;
                }

                if ((hero.transform.position - spawnLocation).magnitude < aggroRange)
                {
                    if (fsm.ActiveStateName == "Dormant")
                    {
                        var rng = (new RNG(hero.transform.position.x.GetHashCode()));
                        xpos.Value = spawnLocation.x + rng.Rand(-xRange * .5f, xRange * .5f);
                        ypos.Value = spawnLocation.y + rng.Rand(-yRange * .5f, yRange * .5f);

                        hasSpawned = true;
                        fsm.SendEvent("BALLOON SPAWN");
                    }
                }
                else
                {
                    if (hasSpawned)
                        fsm.SendEvent("PLAYER_FAR");
                }

                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }

    public class ZoteBalloonSpawner : DefaultSpawner<ZoteBalloonControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZoteBalloonControl>();
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

    public class ZoteBalloonPrefabConfig : DefaultPrefabConfig<ZoteBalloonControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Control");

                //remove the transitions related to chain spawning zotes for the event
                fsm.RemoveTransition("Reset", "FINISHED");
                //fsm.RemoveTransition("Respawn Pause", "FINISHED");

                //remove the states that were also part of that
                fsm.AddGlobalTransition("PLAYER_FAR", "Set Pos");
                //fsm.Fsm.RemoveState("Respawn Pause");

                var appearState = fsm.GetState("Set Pos");
                appearState.Actions = appearState.Actions.Where(x =>
                {
                    return !typeof(RandomFloat).IsAssignableFrom(x.GetType());
                }).ToArray();
            }
        }
    }





}
