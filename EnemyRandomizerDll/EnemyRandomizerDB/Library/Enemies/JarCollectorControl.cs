using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JarCollectorControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

        public GameObject spawnerPlaceholder;
        public GameObject buzzers;
        public GameObject spitters;
        public GameObject rollers;

        public GameObject jarPrefab;
        public SpawnObjectFromGlobalPool spawnAction;

        public PrefabObject buzzer;
        public PrefabObject spitter;
        public PrefabObject roller;

        public int buzzerHP = 15;
        public int spitterHP = 10;
        public int rollerHP = 5;

        public RNG rng;

        public List<(PrefabObject, int)> possibleSpawns;

        public int CurrentEnemies { get; set; }
        public int MaxEnemies { get; set; }

        protected virtual void SetupSpawnerPlaceholders()
        {
            if(spawnerPlaceholder == null)
            {
                //generate the placeholder for the init state
                spawnerPlaceholder = new GameObject("Spawner Placeholder");
                spawnerPlaceholder.transform.SetParent(transform);
                spawnerPlaceholder.SetActive(false);

                buzzers = new GameObject("Buzzers");
                spitters = new GameObject("Spitters");
                rollers = new GameObject("Rollers");

                buzzers.transform.SetParent(spawnerPlaceholder.transform);
                spitters.transform.SetParent(spawnerPlaceholder.transform);
                rollers.transform.SetParent(spawnerPlaceholder.transform);

                control.FsmVariables.GetFsmGameObject("Top Pool").Value = spawnerPlaceholder;
                control.FsmVariables.GetFsmGameObject("Buzzers").Value = buzzers;
                control.FsmVariables.GetFsmGameObject("Spitters").Value = spitters;
                control.FsmVariables.GetFsmGameObject("Rollers").Value = rollers;

                var db = EnemyRandomizerDatabase.GetDatabase();
                buzzer = db.Enemies["Buzzer"];
                spitter = db.Enemies["Spitter"];
                roller = db.Enemies["Roller"];

                possibleSpawns = new List<(PrefabObject, int)>()
                {
                    (buzzer, buzzerHP),
                    (spitter, spitterHP),
                    (roller, rollerHP)
                };

                rng = new RNG();
                rng.Reset();
            }
        }

        protected virtual void SetMaxEnemies(int max)
        {
            MaxEnemies = max;
            control.FsmVariables.GetFsmInt("Enemies Max").Value = max;
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt((float)defaultHP / 10f);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            SetupSpawnerPlaceholders();

            SetMaxEnemies(4);
            CurrentEnemies = 0;

            var init = control.GetState("Init");
            init.DisableAction(10);
            init.DisableAction(11);
            init.DisableAction(12);
            init.DisableAction(13);

            DisableSendEvents(control, 
                ("Start Land", 2)
                );

            var onDeath = control.GetState("Death Start");
            onDeath.DisableAction(0);

            control.ChangeTransition("Start Land", "FINISHED", "Roar End");

            var ec = control.GetState("Enemy Count");
            ec.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmInt("Current Enemies").Value = CurrentEnemies;
            }, 0);

            //remove fly away anim
            control.ChangeTransition("Jump Antic", "FINISHED", "Summon?");

            var summon = control.GetState("Summon?");
            summon.DisableAction(0);
            ec.InsertCustomAction(() =>
            {
                control.FsmVariables.GetFsmInt("Boss Tag Count").Value = CurrentEnemies;
            }, 0);

            //remove fly return anim
            control.ChangeTransition("Summon?", "CANCEL", "Hop Start Antic");

            //after spawn we'll be lunging, so return here
            control.ChangeTransition("Spawn", "FINISHED", "Lunge Swipe");

            var spawn = control.GetState("Spawn");
            jarPrefab = spawn.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;

            spawn.DisableAction(1);
            spawn.DisableAction(2);
            spawn.DisableAction(3);
            spawn.DisableAction(4);
            spawn.DisableAction(5);
            spawn.DisableAction(6);
            spawn.DisableAction(7);
            spawn.RemoveTransition("REPOS");

            spawnAction = new SpawnObjectFromGlobalPool();
            spawnAction.gameObject.Value = jarPrefab;
            spawnAction.spawnPoint.Value = gameObject;
            spawnAction.position.Value = Vector3.zero; //offset from boss (might update later)

            spawn.AddAction(spawnAction);
            spawn.AddCustomAction(() =>
            {
                var go = spawnAction.storeObject.Value;
                var jar = go.GetComponent<SpawnJarControl>();
                var selectedSpawn = possibleSpawns.GetRandomElementFromList(rng);
                jar.SetEnemySpawn(selectedSpawn.Item1.prefab, selectedSpawn.Item2);
                FlingUtils.FlingObject(new FlingUtils.SelfConfig
                {
                    Object = go,
                    SpeedMin = 20f,
                    SpeedMax = 30f,
                    AngleMin = 70f,
                    AngleMax = 110f                    
                }, gameObject.transform, Vector3.zero);
                var body = jar.GetComponent<Rigidbody2D>();
                if (body != null)
                    body.angularVelocity = 15f;
            });
            spawn.AddCustomAction(() => control.SendEvent("FINISHED"));

            this.InsertHiddenState(control, "Init", "FINISHED", "Start Fall");
            this.AddResetToStateOnHide(control, "Init");

            var fsm = gameObject.LocateMyFSM("Death");
            fsm.ChangeTransition("Init", "DEATH", "Fall");

            var phase = gameObject.LocateMyFSM("Phase Control");
            GameObject.Destroy(phase);
        }
    }

    public class JarCollectorSpawner : DefaultSpawner<JarCollectorControl> { }

    public class JarCollectorPrefabConfig : DefaultPrefabConfig<JarCollectorControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
