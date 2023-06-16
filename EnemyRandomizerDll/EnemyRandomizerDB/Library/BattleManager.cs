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
using Satchel;

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
                    "Grimm Scene",
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

        public static bool AggroAllBossesNow = true;
        public static bool BlackKnightArenaDoneEarly = false;
        public static int BlackKnightArenaKillCounter = 0;
        public static int BlackKnightsActiveCounter = 0;
        public static int BlackKnightCounter = 0;
        public static bool DidSceneCheck;

        public void Update()
        {
            //special logic for this
            if(StateMachine != null && StateMachine.Value != null)
            {
                var currentScene = StateMachine.Value.SceneName;

                if (currentScene == "Mines_32")
                {
                    if(StateMachine.Value.battleStarted)
                    {
                        return;
                    }

                    if(HeroController.instance.transform.position.x < 38f)
                    {
                        gameObject.GetDirectChildren().ForEach(x => x.SafeSetActive(true));
                    }
                }
            }
        }

        public static ZoteMusicControl zmusic;
        public static void PlayZoteTheme(bool gramaphone = false, float volume = .8f)
        {
            if (zmusic == null)
            {
                GameObject music = SpawnerExtensions.SpawnEntityAt("Zote Music", HeroController.instance.transform.position, null, true, false);
                zmusic = music.GetComponent<ZoteMusicControl>();
            }

            if (gramaphone)
            {
                var gc = GameObject.Find("Gramophone Control");
                if (gc != null)
                {
                    var g = gc.FindGameObjectInDirectChildren("gramaphone");
                    if (g != null)
                    {
                        Dev.Log("playing zote music");
                        zmusic.PlayMusic(volume, g);
                        if (zmusic.audioSource != null)
                        {
                            var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Normal - Gramaphone");
                            normal.TransitionTo(2f);
                        }
                    }
                }
            }
            else
            {
                zmusic.PlayMusic(volume);
                if (zmusic.audioSource != null)
                {
                    var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Action");
                    normal.TransitionTo(2f);
                }
            }
        }

        public static void DoSceneCheck(GameObject sceneObject)
        {
            BattleManager.DidSceneCheck = true;
            Dev.Log("Checking scene " + sceneObject.SceneName());

            Scene currentScene = sceneObject.scene;

            BlackKnightsActiveCounter = 0;
            BlackKnightArenaKillCounter = 0;
            BlackKnightCounter = 0;
            BlackKnightArenaDoneEarly = false;
            AggroAllBossesNow = false;

            var roots = currentScene.GetRootGameObjects();

                //in this scene the battle manager isn't a root game object...
            if (currentScene.name == "Ruins1_09")
            {
                var found = GameObject.Find("Battle Scene");

                if (found)
                {
                    LoadFromFSM(found.GetComponent<PlayMakerFSM>());
                }
            }
            //for abyss 19 we want a slightly different controller
            else if (currentScene.name == "Abyss_19")
            {
                var found = GameObject.Find("infected_door");

                if (found)
                {
                    LoadFromFSM(found.LocateMyFSM("Control"));
                }
            }
            else if (currentScene.name == "Grimm_Main_Tent" || currentScene.name == "Grimm_Main_Tent_boss")
            {
                //grimm's stuff is a bit different, so use special cases to get the correct FSM
                var found = GameObject.Find("Grimm Scene");

                if (found)
                {
                    LoadFromFSM(found.LocateMyFSM("Initial Scene"));
                }
            }
            else if (currentScene.name == "Grimm_Nightmare")
            {
                var found = GameObject.Find("Grimm Control");

                if (found)
                {
                    LoadFromFSM(found.LocateMyFSM("Control"));
                }
            }
            else if (currentScene.name == "GG_Lurker")
            {
                var found = GameObject.Find("Lurker Control");

                if (found)
                {
                    LoadFromFSM(found.LocateMyFSM("Battle Start"));
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

                var coreRoots = core.FindGameObjectsNameContainsInChildren("_root");
                coreRoots.ForEach(x => GameObject.Destroy(x));
                var dcmain = core.FindGameObjectNameContainsInChildren("dreamcatcher_main");
                
                if(dcmain != null)
                    GameObject.Destroy(dcmain);

                //var newCenter = EnemyRandomizerDatabase.CustomSpawnWithLogic(center.transform.position, "GG_Statue_Gorb", null, true);
                //var newBack = EnemyRandomizerDatabase.CustomSpawnWithLogic(back.transform.position, "Knight_v01", null, true);
                var newRight = EnemyRandomizerDatabase.CustomSpawnWithLogic(right.transform.position, "GG_Statue_Zote", null, true);
                var newLeft = EnemyRandomizerDatabase.CustomSpawnWithLogic(left.transform.position, "GG_Statue_GreyPrince", null, true);//TODO: remove the trophy/phase markers on the statue

                var plaq = newLeft.FindGameObjectNameContainsInChildren("Plaque");
                if(plaq != null)
                    GameObject.Destroy(plaq);

                var pdtrue = newLeft.GetComponent<DeactivateIfPlayerdataTrue>();
                if(pdtrue != null)
                {
                    GameObject.DestroyImmediate(pdtrue);
                }

                var floor_effect = EnemyRandomizerDatabase.CustomSpawnWithLogic(new Vector3(29.4165f, 4.0255f, -.5f), "dream_beam_animation", null, true);


                GameObject.Destroy(center);
                GameObject.Destroy(back);
                GameObject.Destroy(right);
                GameObject.Destroy(left);

                PlayZoteTheme(false, 0.4f);

                //TODO: change fountain text
                //TODO: add challenge prompt to enter a unqiue enemy rando boss / make it warp you somewhere?
            }
            else if (currentScene.name == "Fungus3_50")
            {
                try
                {
                    GameObject go = new GameObject("Wall");
                    go.transform.position = new Vector3(1f, 110f);
                    var collider = go.GetOrAddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1f, 30f);
                }
                catch (Exception e) { Dev.Log($"Error??? \n{e.Message}\n{e.StackTrace}"); }

                try
                {
                    Dev.Log("spawning zotes");
                    var lazyFliers = GameObjectExtensions.FindObjectsOfType<PlayMakerFSM>().Where(x => x.name.Contains("Lazy Flyer"));
                    foreach (var flyer in lazyFliers)
                    {
                        Dev.Log("found one, making it zote " + flyer);
                        var newThing = SpawnerExtensions.SpawnEntityAt("Zote Crew Fat", flyer.transform.position, null, true, false);
                        Dev.Log("spawned " + newThing);
                        var poob = newThing.GetComponent<PreventOutOfBounds>();
                        if (poob != null)
                            GameObject.Destroy(poob);
                        var sz = newThing.GetComponent<SetZ>();
                        if (sz != null)
                            GameObject.Destroy(sz);
                        newThing.transform.position = new Vector3(newThing.transform.position.x, newThing.transform.position.y, 12.5f + UnityEngine.Random.Range(0f, 5f));
                        Dev.Log("doing set z");
                        newThing.SafeSetActive(true);
                        Dev.Log("activating it");
                        SpawnerExtensions.DestroyObject(flyer.gameObject, true);
                        Dev.Log("destroying old object");
                    }
                }
                catch (Exception e) { Dev.Log($"Error??? \n{e.Message}\n{e.StackTrace}"); }

                try
                {
                    Dev.Log("spawning zote music");
                    var gc = GameObject.Find("Gramaphone Control");
                    if (gc != null)
                    {
                        var g = gc.FindGameObjectInDirectChildren("gramaphone");
                        if (g != null)
                        {
                            var fsm = g.LocateMyFSM("Music Control");
                            var play = fsm.GetState("Play");
                            play.DisableAction(0);
                            play.InsertCustomAction(() => { PlayZoteTheme(true, 0.4f); }, 0);
                        }
                    }
                }
                catch (Exception e) { Dev.Log($"Error??? \n{e.Message}\n{e.StackTrace}"); }
            }

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

            if(scene.name == "Room_Colosseum_Bronze")
            {
                StateMachine.Value = new ColoBronze();
            }
            else if (scene.name == "Room_Colosseum_Silver")
            {
                StateMachine.Value = new ColoSilver();
            }
            else if (scene.name == "Room_Colosseum_Gold")
            {
                StateMachine.Value = new ColoGold();
            }
            else
            {
                StateMachine.Value = new BattleStateMachine();
            }

            StateMachine.Value.Setup(scene, fsm);

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



