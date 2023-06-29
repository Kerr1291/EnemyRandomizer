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
using UnityEngine.Audio;

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
                if (gameObject != null && gameObject.IsBattleEnemy())
                {
                    StateMachine.Value.RegisterEnemyDeath(gameObject.GetComponent<SpawnedObjectControl>());
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
            if (StateMachine != null && StateMachine.Value != null)
            {
                if (!StateMachine.Value.battleStarted && StateMachine.Value.useBoxToForceStart)
                {
                    bool isColo = BattleManager.Instance.Value.gameObject.scene.name.Contains("Room_Colosseum_");
                    if (!isColo)
                    {
                        if (StateMachine.Value.IsHeroInBattleArea())
                        {
                            StateMachine.Value.ForceBattleStart();
                            return;
                        }
                    }
                }

                var currentScene = StateMachine.Value.SceneName;

                if (currentScene == "Mines_32")
                {
                    if (StateMachine.Value.battleStarted)
                    {
                        return;
                    }

                    if (HeroController.instance.transform.position.x < 38f)
                    {
                        gameObject.GetDirectChildren().ForEach(x => x.SafeSetActive(true));
                    }
                }
                if (currentScene == "Ruins1_09")
                {
                    if (!StateMachine.Value.battleStarted)
                    {
                        return;
                    }

                    var hms = GameObject.FindObjectsOfType<HealthManager>().Length;
                    if (hms <= 0)
                    {
                        BattleStateMachine.OpenGates(false);
                    }
                }
                else if (currentScene == "Crossroads_09")
                {
                    if (StateMachine.Value.battleStarted ||
                        GameManager.instance.playerData.mawlekDefeated)
                    {
                        return;
                    }

                    if (HeroController.instance.transform.position.x < 72f &&
                        HeroController.instance.transform.position.x > 50f)
                    {
                        BattleStateMachine.CloseGates(true);
                        GameObjectExtensions.FindObjectsOfType<HealthManager>().ToList().ForEach(x =>
                        {
                            x.gameObject.SafeSetActive(true);
                            var setz = x.GetComponent<SetZ>();
                            if (setz != null)
                            {
                                GameObject.Destroy(setz);
                                x.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                            }

                            if (x.gameObject.GetComponent<MeshRenderer>() != null)
                            {
                                x.gameObject.GetComponent<MeshRenderer>().enabled = true;
                            }

                            if (x.gameObject.GetComponent<Collider2D>() != null)
                            {
                                x.gameObject.GetComponent<Collider2D>().enabled = true;
                            }
                        });
                    }
                    else if (currentScene == "Dream_02_Mage_Lord")
                    {
                        if (HeroController.instance.transform.position.y > 23f)
                        {
                            {
                                var gate = GameObject.Find("Dream Gate Phase 2");
                                if (gate != null)
                                    gate.SafeSetActive(false);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (1)");
                                if (gate != null)
                                    gate.SafeSetActive(false);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (2)");
                                if (gate != null)
                                    gate.SafeSetActive(false);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (3)");
                                if (gate != null)
                                    gate.SafeSetActive(false);
                            }
                        }
                        else
                        {
                            {
                                var gate = GameObject.Find("Dream Gate Phase 2");
                                if (gate != null)
                                    gate.SafeSetActive(true);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (1)");
                                if (gate != null)
                                    gate.SafeSetActive(true);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (2)");
                                if (gate != null)
                                    gate.SafeSetActive(true);
                            }

                            {
                                var gate = GameObject.Find("Dream Gate Phase 2 (3)");
                                if (gate != null)
                                    gate.SafeSetActive(true);
                            }
                        }
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
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
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

        public static void StopZoteTheme()
        {
            var musicControl2 = GameObject.Find("Atmos Cave Wind");
            if (musicControl2 == null)
                return;

            var globalAudioSource2 = musicControl2.GetComponent<AudioSource>();

            if (globalAudioSource2 != null)
            {
                globalAudioSource2.Stop();

                if (globalAudioSource2.outputAudioMixerGroup != null && globalAudioSource2.outputAudioMixerGroup.audioMixer != null)
                {
                    var normal = globalAudioSource2.outputAudioMixerGroup.audioMixer.FindSnapshot("Silence");
                    if(normal != null)
                        normal.TransitionTo(1f);
                }
            }

            if (zmusic == null)
                return;

            {
                zmusic.StopMusic();
            }
        }

        public static void SilenceMusic()
        {
            var musicControl = GameObject.Find("AudioManager");
            if (musicControl == null)
                return;

            var globalAudioSource = musicControl.GetComponentInChildren<AudioSource>(true);

            if (globalAudioSource != null)
            {
                globalAudioSource.Stop();

                var normal = globalAudioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Silence");
                normal.TransitionTo(1f);
            }
        }

        public static void DoSpecialSceneFixes(GameObject sceneObject)
        {
            Scene currentScene = sceneObject.scene;
        }

        public static bool startedZoteTheme = false;
        public static void DoSceneCheck(GameObject sceneObject)
        {
            BattleManager.DidSceneCheck = true;
            if (SpawnedObjectControl.VERBOSE_DEBUG)
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

            try
            {
                if (EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
                {
                    //allow zote theme to play on these maps
                    if (currentScene.name != "Ruins1_27"
                        && currentScene.name != "Fungus3_50"
                        && currentScene.name != "Room_Colosseum_Bronze"
                        && currentScene.name != "Room_Colosseum_Silver"
                        && currentScene.name != "Room_Colosseum_Gold")
                    {
                        if (startedZoteTheme)
                            StopZoteTheme();
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Caught exception when trying to stop the zote theme? {e.Message}\n{e.StackTrace}"); }


            if(currentScene.name == "Fungus1_20_v02" || currentScene.name == "Fungus1_32")
            {
                //prevent zote dead in enemy rando
                if(GameManager.instance.playerData.zoteDead)
                {
                    GameManager.instance.playerData.zoteDead = false;
                }
            }


            //TODO: move this into a better spot for generating our custom content
            //make our custom fountain
            if (currentScene.name == "Ruins1_27")
            {
                if (EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
                {
                    startedZoteTheme = true;
                    var center = GameObject.Find("_0083_fountain");
                    var back = GameObject.Find("_0082_fountain");
                    var right = GameObject.Find("_0092_fountain");
                    var left = GameObject.Find("_0092_fountain (1)");

                    //TODO: update core position and rotation a bit
                    var core = EnemyRandomizerDatabase.CustomSpawnWithLogic(center.transform.position.ToVec2() + Vector2.up * 4f, "gg_blue_core", null, true);

                    //TODO: remove roots
                    //var coreRoots = core.FindGameObjectsNameContainsInChildren("_root");
                    //coreRoots.ForEach(x => GameObject.Destroy(x));
                    //var dcmain = core.FindGameObjectNameContainsInChildren("dreamcatcher_main");

                    //if (dcmain != null)
                    //    GameObject.Destroy(dcmain);

                    //var newCenter = EnemyRandomizerDatabase.CustomSpawnWithLogic(center.transform.position, "GG_Statue_Gorb", null, true);
                    //var newBack = EnemyRandomizerDatabase.CustomSpawnWithLogic(back.transform.position, "Knight_v01", null, true);
                    var newRight = EnemyRandomizerDatabase.CustomSpawnWithLogic(right.transform.position, "GG_Statue_Zote", null, true);
                    var newLeft = EnemyRandomizerDatabase.CustomSpawnWithLogic(left.transform.position, "GG_Statue_GreyPrince", null, true);//TODO: remove the trophy/phase markers on the statue

                    var plaq = newLeft.FindGameObjectNameContainsInChildren("Plaque");
                    if (plaq != null)
                        GameObject.Destroy(plaq);

                    var pdtrue = newLeft.GetComponent<DeactivateIfPlayerdataTrue>();
                    if (pdtrue != null)
                    {
                        GameObject.Destroy(pdtrue);
                    }

                    var floor_effect = EnemyRandomizerDatabase.CustomSpawnWithLogic(new Vector3(29.4165f, 4.0255f, -.5f), "dream_beam_animation", null, true);


                    GameObject.Destroy(center);
                    GameObject.Destroy(back);
                    GameObject.Destroy(right);
                    GameObject.Destroy(left);

                    PlayZoteTheme(false, 0.4f);

                    newLeft.SafeSetActive(true);
                    newRight.SafeSetActive(true);

                    //TODO: change fountain text
                    //TODO: add challenge prompt to enter a unqiue enemy rando boss / make it warp you somewhere?
                }
            }
            else if (currentScene.name == "Waterways_12_boss" || currentScene.name == "Waterways_12")
            {
                //make safe floors to keep the boss sub from falling into water/acid
                var p0 = new Vector3(22f, 3.5548f, 0);
                var p1 = new Vector3(19f, 3.5548f, 0);
                var p2 = new Vector3(17f, 3.5548f, 0);
                var p3 = new Vector3(15f, 3.5548f, 0);

                var p4 = new Vector3(7f, 3.5548f, 0);
                var p5 = new Vector3(5f, 3.5548f, 0);

                var p6 = new Vector3(32f, 3.5548f, 0);
                var p7 = new Vector3(34f, 3.5548f, 0);
                var p8 = new Vector3(36f, 3.5548f, 0);
                var p9 = new Vector3(38f, 3.5548f, 0);
                var p10 = new Vector3(40f, 3.5548f, 0);
                var pboss = new Vector3(19.0291f, 21.7803f, 0f);

                var plats = new List<Vector3>() {
                p0,
                p1,
                p2,
                p3,
                p4,
                p5,
                p6,
                p7,
                p8,
                p9,
                p10,
                pboss,
                };

                var plat = GameObject.Find("plat_float_05");

                foreach (var p in plats)
                {
                    var pnew = GameObject.Instantiate(plat);
                    pnew.transform.position = p;
                }
            }
            else if (currentScene.name == "Fungus3_50")
            {
                if (EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
                {
                    startedZoteTheme = true;
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
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
                            Dev.Log("spawning zotes");
                        var lazyFliers = GameObjectExtensions.FindObjectsOfType<PlayMakerFSM>().Where(x => x.name.Contains("Lazy Flyer"));
                        foreach (var flyer in lazyFliers)
                        {
                            if (SpawnedObjectControl.VERBOSE_DEBUG)
                                Dev.Log("found one, making it zote " + flyer);
                            var newThing = SpawnerExtensions.SpawnEntityAt("Zote Crew Fat", flyer.transform.position, null, true, false);

                            if (SpawnedObjectControl.VERBOSE_DEBUG)
                                Dev.Log("spawned " + newThing);
                            var poob = newThing.GetComponent<PreventOutOfBounds>();
                            if (poob != null)
                                GameObject.Destroy(poob);
                            var sz = newThing.GetComponent<SetZ>();
                            if (sz != null)
                                GameObject.Destroy(sz);
                            newThing.transform.position = new Vector3(newThing.transform.position.x, newThing.transform.position.y, 12.5f + UnityEngine.Random.Range(0f, 5f));

                            if (SpawnedObjectControl.VERBOSE_DEBUG) Dev.Log("doing set z");
                            newThing.SafeSetActive(true);

                            if (SpawnedObjectControl.VERBOSE_DEBUG) Dev.Log("activating it");
                            SpawnerExtensions.DestroyObject(flyer.gameObject, true);

                            if (SpawnedObjectControl.VERBOSE_DEBUG) Dev.Log("destroying old object");
                        }
                    }
                    catch (Exception e) { Dev.Log($"Error??? \n{e.Message}\n{e.StackTrace}"); }

                    try
                    {

                        if (SpawnedObjectControl.VERBOSE_DEBUG) Dev.Log("spawning zote music");
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

            if (scene.name == "Room_Colosseum_Bronze")
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
            //else if (scene.name == "Crossroads_08")
            //{
            //    StateMachine.Value = new Crossroads_08Arena();
            //}
            //else if (scene.name == "Crossroads_22")
            //{
            //    StateMachine.Value = new Crossroads_22Arena();
            //}
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



