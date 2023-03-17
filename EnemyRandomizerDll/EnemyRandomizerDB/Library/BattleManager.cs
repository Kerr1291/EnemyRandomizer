using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

using UniRx;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public class BattleManager : MonoBehaviour
    {
        //TODO extract and customize these managers
        public static List<string> battleControllers = new List<string>()
                {
                    "Battle Scene Ore",
                    "Battle Start",
                    "Battle Scene",
                    "Battle Scene v2",
                    "Battle Music",
                    "Mantis Battle",
                    "Lurker Control",
                    "Battle Control",
                    "Grimm Holder",
                    "Grub Scene",
                    "Boss Scene Controller",
                    "Colosseum Manager",
                };

        public static bool IsBattleManager(GameObject gameObject)
        {
            return battleControllers.Contains(gameObject.name);
        }


        public static PlayMakerFSM FSM { get { return Instance.Value.GetComponent<PlayMakerFSM>(); } }
        public static ReactiveProperty<BattleManager> Instance { get; protected set; }
        public static ReactiveProperty<BattleStateMachine> StateMachine { get; protected set; }

        public static void Init()
        { 
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>();
            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();
        }

        public static void OnEnemyDeathEvent(GameObject gameObject)
        {
            if (Instance.HasValue && Instance.Value != null)
            {
                var bmo = gameObject.GetComponent<BattleManagedObject>();
                if (bmo != null)
                {
                    StateMachine.Value.RegisterEnemyDeath(bmo);
                    //bmo.myWave.RegisterEnemyDeath(bmo);
                }
            }
        }

        public static bool DidSceneCheck;

        public static void DoSceneCheck(GameObject sceneObject)
        {
            Scene currentScene = sceneObject.scene;

            var roots = currentScene.GetRootGameObjects();
            var found = roots.FirstOrDefault(x => IsBattleManager(x));

            if (found)
            {
                LoadFromFSM(found.GetComponent<PlayMakerFSM>());
            }

            BattleManager.DidSceneCheck = true;
        }

        public static void LoadFromFSM(PlayMakerFSM fsm)
        {
            if (Instance.HasValue && Instance.Value != null)
                return;

            bool isValid = BattleManager.IsBattleManager(fsm.gameObject);

            if (isValid)
            {
                var pbi = fsm.GetComponent<PersistentBoolItem>();
                if (pbi == null)
                {
                    //TODO: strip the player data bool set from hive knight's fsm...
                    if (fsm.gameObject.scene.name == "Hive_05")
                    {
                        //hive knight has a different way of checking persistance
                        bool result = GameManager.instance.GetPlayerDataBool("killedHiveKnight");
                        if (result)
                            return;
                    }
                }
                else
                {
                    //we found it, but it's done so we don't need to do anything
                    if (pbi != null && pbi.persistentBoolData.activated)
                        return;
                }

                //attach our own
                Instance.Value = fsm.gameObject.AddComponent<BattleManager>();

                //set it up
                Instance.Value.Setup(fsm.gameObject.scene, fsm);
            }
        }

        protected virtual void Awake()
        {
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>(this);
            else
                Instance.Value = this;

            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();
        }

        protected virtual void OnDestroy()
        {
            Instance.Value = null;
            if (StateMachine.Value != null)
                StateMachine.Value.Dispose();
            StateMachine.Value = null;
        }

        public void Setup(Scene scene, PlayMakerFSM fsm)
        {
            if (Instance == null)
                Instance = new ReactiveProperty<BattleManager>(this);

            if (StateMachine == null)
                StateMachine = new ReactiveProperty<BattleStateMachine>();

            if (Instance.Value != this)
                Instance.Value = this;

            StateMachine.Value = new BattleStateMachine(scene, fsm);
            //BattleStateMachine.Create(scene, fsm, item);
            //StateMachine.Value.Build(fsm);
        }

        public void Clear()
        {
            if (StateMachine.Value != null)
                StateMachine.Value.Dispose();
            StateMachine.Value = null;
        }

        protected virtual void Cleanup()
        {
            Instance.Value = null;
            Clear();
            Destroy(this);
        }
    }
}



