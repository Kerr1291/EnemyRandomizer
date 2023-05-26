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


        public static PlayMakerFSM FSM { get { return Instance.Value == null ? null : Instance.Value.GetComponent<PlayMakerFSM>(); } }
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
                //var bmo = gameObject.GetComponent<BattleManagedObject>();
                //if (bmo != null)
                if(gameObject != null && gameObject.IsBattleEnemy())
                {
                    StateMachine.Value.RegisterEnemyDeath(gameObject.GetComponent<SpawnedObjectControl>());
                    //bmo.myWave.RegisterEnemyDeath(bmo);
                }
            }
        }

        public static bool DidSceneCheck;

        public static void DoSceneCheck(GameObject sceneObject)
        {
            Scene currentScene = sceneObject.scene;

            var roots = currentScene.GetRootGameObjects();

            //in this scene the battle manager isn't a root game object...
            if(currentScene.name == "Ruins1_09")
            {
                var found = GameObject.Find("Battle Scene");

                if (found)
                {
                    LoadFromFSM(found.GetComponent<PlayMakerFSM>());
                }
            }
            else
            {
                var found = roots.FirstOrDefault(x => IsBattleManager(x));

                if (found)
                {
                    LoadFromFSM(found.GetComponent<PlayMakerFSM>());
                }
            }

            //TODO: move this into a better spot for generating our custom content
            //make our custom fountain
            if(currentScene.name == "Ruins1_27")
            {
                var center = GameObject.Find("_0083_fountain");
                var back = GameObject.Find("_0082_fountain");
                var right = GameObject.Find("_0092_fountain");
                var left = GameObject.Find("_0092_fountain (1)");

                //TODO: update core position and rotation a bit
                var core = EnemyRandomizerDatabase.CustomSpawnWithLogic(center.transform.position.ToVec2() + Vector2.up * 5f, "gg_blue_core", null, true);
                //var newCenter = EnemyRandomizerDatabase.CustomSpawnWithLogic(center.transform.position, "GG_Statue_Gorb", null, true);
                //var newBack = EnemyRandomizerDatabase.CustomSpawnWithLogic(back.transform.position, "Knight_v01", null, true);
                var newRight = EnemyRandomizerDatabase.CustomSpawnWithLogic(right.transform.position, "GG_Statue_Zote", null, true);
                var newLeft = EnemyRandomizerDatabase.CustomSpawnWithLogic(left.transform.position, "GG_Statue_GreyPrince", null, true);//TODO: remove the trophy/phase markers on the statue
                var floor_effect = EnemyRandomizerDatabase.CustomSpawnWithLogic(SpawnerExtensions.GetRayOn(center.transform.position, Vector2.down, 50f).point + Vector2.down, "dream_beam_animation", null, true);

                GameObject.Destroy(center);
                GameObject.Destroy(back);
                GameObject.Destroy(right);
                GameObject.Destroy(left);
                
                //TODO: change fountain text
                //TODO: add challenge prompt to enter a unqiue enemy rando boss / make it warp you somewhere?
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



