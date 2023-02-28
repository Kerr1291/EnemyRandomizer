using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Linq;

namespace EnemyRandomizerMod
{

    public class EnemyReplacer
    {
        //TODO extract and customize these managers
        static List<string> battleControllers = new List<string>()
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

        public EnemyRandomizerDatabase database;
        public BattleManager battleManager;
        public IRandomizerLogic currentLogic;

        public List<(string, string)> GetPreloadNames(string databaseFilePath)
        {
            database = LoadDatabase(databaseFilePath);
            if (database == null)
                return new List<(string, string)>();
            return database.GetPreloadNames();
        }

        public void Setup(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            database.Initialize(preloadedObjects);
            database.Finalize(null);
        }

        public void OnModEnabled()
        {
            SetLogic(currentLogic);
        }

        public void OnModDisabled()
        {
            if (currentLogic != null && IsInGameScene())
                currentLogic.Disable();
        }

        public void SetLogic(IRandomizerLogic newLogic)
        {
            if (currentLogic != null && newLogic != currentLogic && IsInGameScene())
            {
                currentLogic.Disable();
            }

            currentLogic = newLogic;
            EnemyRandomizer.GlobalSettings.currentLogic = currentLogic != null ? currentLogic.Name : string.Empty;

            if (null != currentLogic && IsInGameScene())
            {
                currentLogic.Enable();
            }
        }

        public void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            GameManager.instance.StartCoroutine(MakePlayerImmuneToRoar());
            GameManager.instance.StartCoroutine(EnableLogicOnGameLoad(settings));
        }

        IEnumerator MakePlayerImmuneToRoar()
        {
            yield return new WaitUntil(() => IsInGameScene());

            var roarfsm = HeroController.instance.GetComponentsInChildren<PlayMakerFSM>(true).Where(x => x.FsmName.Contains("Roar Lock")).First();
            roarfsm.ChangeTransition("Roar Allowed?", "FINISHED", "Regain Control");
        }

        IEnumerator EnableLogicOnGameLoad(EnemyRandomizerPlayerSettings settings)
        {
            yield return new WaitUntil(() => IsInGameScene());
            if (currentLogic != null)
            {
                currentLogic.OnStartGame(settings);
                SetLogic(currentLogic);
            }
            yield break;
        }

        public void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
            if (currentLogic != null)
            {
                currentLogic.OnSaveGame(settings);
            }
        }

        public static (string, string)[] SpecialBattleManagedEnemies = new (string, string)[]
        {
            (@"Crossroads_4", @"Giant Fly"),
            (@"Crossroads_4", @"_Enemies/Fly Spawn"),
        };

        public void OnEnemyLoaded(GameObject oldEnemy)
        {
            if (currentLogic != null)
            {
                //special hack to avoid having custom spawned enemies replaced
                if (EnemyRandomizer.bypassNextRandomizeEnemy)
                {
                    var olde = oldEnemy.GetOrAddComponent<ManagedObject>();
                    olde.Setup(this, oldEnemy);
                    olde.replaced = true;//flag replaced anyway
                    EnemyRandomizer.bypassNextRandomizeEnemy = false;
                }

                string key = EnemyRandomizerDatabase.ToDatabaseKey(oldEnemy.name);
                if (database.Enemies.ContainsKey(key))
                {
                    var managedObject = oldEnemy.GetComponent<ManagedObject>();
                    var battleManagedObject = oldEnemy.GetComponent<BattleManagedObject>();

                    if (managedObject != null)
                    {
                        //don't re-replace this
                        if (managedObject.replaced)
                            return;

                        //about to be replaced....
                        managedObject.replaced = true;
                    }
                    else// if (managedObject == null)
                    {
                        string path = oldEnemy.GetSceneHierarchyPath();
                        bool isBattleManagedEnemy = path.Split('/').Any(x => battleControllers.Any(y => x.Contains(y)));

                        if (!isBattleManagedEnemy)
                        {
                            var oldHm = oldEnemy.GetComponent<HealthManager>();
                            var battleScene = oldHm.GetFieldValue<GameObject>("battleScene");
                            if (oldHm != null && battleScene != null)
                            {
                                isBattleManagedEnemy = true;
                                //TODO: test
                                oldHm.SetBattleScene(null);
                                oldEnemy.SetActive(false);
                            }

                            if (!isBattleManagedEnemy
                                && SpecialBattleManagedEnemies.Any(x => x.Item1 == oldEnemy.scene.name && path.Contains(x.Item2)))
                            {
                                isBattleManagedEnemy = true;
                                oldEnemy.SetActive(false);
                            }
                        }

                        if (isBattleManagedEnemy)
                        {
                            //don't replce these directly on load
                            battleManagedObject = oldEnemy.AddComponent<BattleManagedObject>();
                            battleManagedObject.Setup(this, oldEnemy);
                            managedObject = battleManagedObject;

                            //wait to replace later
                            return;
                        }
                    }

                    GameObject newEnemy = currentLogic.ReplaceEnemy(oldEnemy);
                    ManagedObject newManagedObject = null;
                    if (battleManagedObject == null)
                    {
                        newManagedObject = newEnemy.AddComponent<ManagedObject>();
                    }
                    else
                    {
                        newManagedObject = newEnemy.AddComponent<BattleManagedObject>();
                    }

                    newManagedObject.Setup(this, oldEnemy);

                    if (newEnemy != oldEnemy)
                    {
                        //don't invoke the old enemy's ondestroy stuff- just pretend it never existed....
                        if (oldEnemy.activeSelf)
                            oldEnemy.SetActive(false);
                        GameObject.Destroy(oldEnemy);
                    }
                }
            }
        }

        public GameObject SpawnPooledObject(GameObject original)
        {
            if (currentLogic != null && original.GetComponent<ManagedObject>() == null)
            {
                string key = EnemyRandomizerDatabase.ToDatabaseKey(original.name);
                if (database.Effects.ContainsKey(key))
                {
                    GameObject newEffect = currentLogic.ReplacePooledObject(original);
                    var mo = newEffect.AddComponent<ManagedObject>();
                    mo.Setup(this, original);
                    if (newEffect != original)
                    {
                        GameObject.Destroy(original);
                    }
                    return newEffect;
                }
            }

            return original;
        }

        public void OnDamageHeroEnabled(DamageHero potentialHazard)
        {
            if (currentLogic != null && potentialHazard.GetComponent<ManagedObject>() == null)
            {
                string key = EnemyRandomizerDatabase.ToDatabaseKey(potentialHazard.name);
                if (database.Hazards.ContainsKey(key))
                {
                    GameObject newHazard = currentLogic.ReplaceHazardObject(potentialHazard.gameObject);
                    var mo = newHazard.AddComponent<ManagedObject>();
                    mo.Setup(this, newHazard);
                    if (newHazard != potentialHazard.gameObject)
                    {
                        GameObject.Destroy(potentialHazard.gameObject);
                    }
                }
            }
        }

        public void OnBeforeSceneLoad()
        {
            ClearBattleManagerBeforeSceneLoad();
        }

        public void OnEnemyDeathEvent(GameObject deathEventOwner)
        {
            if (battleManager != null)
            {
                var bmo = deathEventOwner.GetComponent<BattleManagedObject>();
                if (bmo != null)
                {
                    battleManager.RegisterEnemyDeath(bmo);
                }
            }
        }

        public bool OnPersistentBoolItemLoaded(PersistentBoolItem item)
        {
            if (currentLogic != null && item.GetComponent<ManagedObject>() == null)
            {
                string key = EnemyRandomizerDatabase.ToDatabaseKey(item.name);
                if (database.Objects.ContainsKey(key))
                {
                    var customData = currentLogic.ReplacePersistentBoolItemData(item);
                    var mo = item.gameObject.AddComponent<ManagedObject>();
                    mo.Setup(this, item.gameObject);
                    if (customData != null)
                    {
                        //new data
                        if (item.persistentBoolData != customData)
                        {
                            item.persistentBoolData = customData;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool OnPersistentBoolItemSetMyID(PersistentBoolItem item)
        {
            if (currentLogic != null && item.GetComponent<ManagedObject>() == null)
            {
                if (item.persistentBoolData != null)
                {
                    string key = EnemyRandomizerDatabase.ToDatabaseKey(item.name);
                    if (database.Objects.ContainsKey(key))
                    {
                        var customData = currentLogic.ReplacePersistentBoolItemSetMyID(item);
                        var mo = item.gameObject.AddComponent<ManagedObject>();
                        mo.Setup(this, item.gameObject);
                        if (( string.IsNullOrEmpty(customData.ID) && string.IsNullOrEmpty(customData.SceneName)))
                            return false;

                        if (customData.ID != item.persistentBoolData.id ||
                            customData.SceneName != item.persistentBoolData.sceneName)
                        {
                            if(!string.IsNullOrEmpty(customData.ID))
                                item.persistentBoolData.id = customData.ID;

                            if (!string.IsNullOrEmpty(customData.SceneName))
                                item.persistentBoolData.sceneName = customData.SceneName;

                            return true;
                            //item.persistentBoolData.sceneName = SceneName;// GameManager.GetBaseSceneName("custom");
                        }
                    }
                }
            }

            return false;
        }

        public void OnPlaymakerFSMEnabled(PlayMakerFSM fsm)
        {
            if (battleManager != null)
                return;

            bool isValid = IsBattleManager(fsm.gameObject);
            if (isValid)
            {
                //we found it, but it's done so we don't need to do anything
                var pbi = fsm.GetComponent<PersistentBoolItem>();
                if (pbi != null && pbi.persistentBoolData.activated)
                    return;

                //attach our own
                battleManager = fsm.gameObject.AddComponent<BattleManager>();

                //set it up
                battleManager.Setup(this, fsm, pbi);
            }
        }

        EnemyRandomizerDatabase LoadDatabase(string fileName)
        {
            Dev.Where();
            EnemyRandomizerDatabase newDB = null;
            try
            {
                Dev.Log("loading data");
                newDB = EnemyRandomizerDatabase.Create(Path.GetFileName(fileName));
                if (newDB == null || newDB.scenes.Count <= 0)
                {
                    Dev.Log($"Failed to load database file {Path.GetFileName(fileName)} from mod directory");
                    return null;
                }

                Dev.Log("Loaded entries: " + newDB.scenes.Count);
                return newDB;
            }
            catch (System.Exception e)
            {
                Dev.LogError("Failed to load database. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// Invoked after scene change, so everything should be gone anyway
        /// </summary>
        void ClearBattleManagerBeforeSceneLoad()
        {
            battleManager = null;
        }

        public bool IsInGameScene()
        {
            return GameManager.instance.IsGameplayScene() && !GameManager.instance.IsCinematicScene();
        }
    }
}
