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
        static bool DEBUG_WARN_IF_NOT_FOUND = true;

        public EnemyRandomizerDatabase database;
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

        public EnemyRandomizerDatabase GetCurrentDatabase()
        {
            return database;
        }

        public void OnModEnabled()
        {
            EnemyRandomizerDatabase.GetDatabase -= GetCurrentDatabase;
            EnemyRandomizerDatabase.GetDatabase += GetCurrentDatabase;
            SetLogic(currentLogic);
        }

        public void OnModDisabled()
        {
            if (currentLogic != null && IsInGameScene())
                currentLogic.Disable();

            EnemyRandomizerDatabase.GetDatabase -= GetCurrentDatabase;
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

        public static List<string> DestroyOnLoadAlways = new List<string>()
        {
            "Fly Spawn",
            "Hatcher Spawn",
        };

        public void OnEnemyLoaded(GameObject oldEnemy)
        {
            if(DestroyOnLoadAlways.Any(x => oldEnemy.GetSceneHierarchyPath().Contains(x)))
            {
                if (oldEnemy.name.Contains("Fly"))
                {
                    //TEST: see if this removes the count from the battle scene manager
                    var battleManagedObject = oldEnemy.AddComponent<BattleManagedObject>();
                    battleManagedObject.Setup(oldEnemy);
                    BattleManager.StateMachine.Value.RegisterEnemyDeath(battleManagedObject);
                }
                GameObject.Destroy(oldEnemy);
                return;
            }

            if (currentLogic != null)
            {
                //special hack to avoid having custom spawned enemies replaced
                if (EnemyRandomizer.bypassNextRandomizeEnemy)
                {
                    var olde = oldEnemy.GetOrAddComponent<ManagedObject>();
                    olde.Setup(oldEnemy);
                    olde.replaced = true;//flag as replaced anyway
                    EnemyRandomizer.bypassNextRandomizeEnemy = false;
                }

                string key = EnemyRandomizerDatabase.ToDatabaseKey(oldEnemy.name);
                if (!string.IsNullOrEmpty(key) && database.Enemies.ContainsKey(key))
                {
                    ObjectMetadata info = new ObjectMetadata();
                    info.Setup(oldEnemy);

                    if (info.RandoObject != null)
                    {
                        //don't re-replace this
                        if (info.RandoObject.replaced)
                            return;

                        //about to be replaced....
                        info.RandoObject.replaced = true;
                    }
                    else// if (managedObject == null)
                    {
                        bool isBattleManagedEnemy = info.IsBattleEnemy;

                        if (isBattleManagedEnemy)
                        {
                            //don't replce these directly on load
                            var battleManagedObject = oldEnemy.AddComponent<BattleManagedObject>();
                            battleManagedObject.Setup(oldEnemy);
                            OnEnemyLoaded(oldEnemy);
                            return;
                        }
                    }

                    GameObject newEnemy = currentLogic.ReplaceEnemy(oldEnemy);
                    ManagedObject newManagedObject = null;
                    if (info.BattleRandoObject == null)
                    {
                        newManagedObject = newEnemy.AddComponent<ManagedObject>();
                    }
                    else
                    {
                        newManagedObject = newEnemy.AddComponent<BattleManagedObject>();
                    }

                    newManagedObject.Setup(oldEnemy);

                    if (newEnemy != oldEnemy)
                    {
                        //don't invoke the old enemy's ondestroy stuff- just pretend it never existed....
                        if (oldEnemy.activeSelf)
                            oldEnemy.SetActive(false);
                        GameObject.Destroy(oldEnemy);
                    }
                }
                else
                {
                    if (DEBUG_WARN_IF_NOT_FOUND)
                        Dev.LogError($"ERROR: {key} was not found in the database when searching for {oldEnemy.name}!");
                }
            }
        }

        public GameObject SpawnPooledObject(GameObject original)
        {
            if (currentLogic != null && original.GetComponent<ManagedObject>() == null)
            {
                string key = EnemyRandomizerDatabase.ToDatabaseKey(original.name);
                if (!string.IsNullOrEmpty(key) && database.Effects.ContainsKey(key))
                {
                    GameObject newEffect = currentLogic.ReplacePooledObject(original);
                    var mo = newEffect.AddComponent<ManagedObject>();
                    mo.Setup(original);
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
                if (!string.IsNullOrEmpty(key) && database.Hazards.ContainsKey(key))
                {
                    GameObject newHazard = currentLogic.ReplaceHazardObject(potentialHazard.gameObject);
                    var mo = newHazard.AddComponent<ManagedObject>();
                    mo.Setup(newHazard);
                    if (newHazard != potentialHazard.gameObject)
                    {
                        GameObject.Destroy(potentialHazard.gameObject);
                    }
                }
            }
        }

        public void OnBeforeSceneLoad()
        {
            //TODO: infom logics?
        }

        public void OnEnemyDeathEvent(GameObject deathEventOwner)
        {
            //might want to notify the loaded logic here?
        }

        public bool OnPersistentBoolItemLoaded(PersistentBoolItem item)
        {
            if (currentLogic != null && item.GetComponent<ManagedObject>() == null)
            {
                string key = EnemyRandomizerDatabase.ToDatabaseKey(item.name);
                if (!string.IsNullOrEmpty(key) && database.Objects.ContainsKey(key))
                {
                    var customData = currentLogic.ReplacePersistentBoolItemData(item);
                    var mo = item.gameObject.AddComponent<ManagedObject>();
                    mo.Setup(item.gameObject);
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
                    if (!string.IsNullOrEmpty(key) && database.Objects.ContainsKey(key))
                    {
                        var customData = currentLogic.ReplacePersistentBoolItemSetMyID(item);
                        var mo = item.gameObject.AddComponent<ManagedObject>();
                        mo.Setup(item.gameObject);
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

            //might want to notify the loaded logics?
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

        public bool IsInGameScene()
        {
            return GameManager.instance.IsGameplayScene() && !GameManager.instance.IsCinematicScene();
        }
    }
}
