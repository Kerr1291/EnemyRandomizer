using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Linq;
using System;

namespace EnemyRandomizerMod
{
    public class EnemyReplacer
    {
        public static bool VERBOSE_LOGGING = false;

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

        IEnumerator OnEnemyLoadedAndActive(ObjectMetadata info)
        {
            yield return new WaitUntil(() => info.IsVisibleNow());
            OnEnemyLoaded(info.Source);
        }

        public GameObject OnEnemyLoaded(GameObject original)
        {
            if (VERBOSE_LOGGING)
                Dev.Log("Trying to replace " + original);

            bool error = false;
            try
            {
                if (!ProcessPreReplacement(original))
                {
                    ObjectMetadata info = new ObjectMetadata();
                    info.Setup(original, database);

                    if(info.IsTemporarilyInactive())
                    {
                        var hm = original.GetComponent<HealthManager>();
                        hm.StartCoroutine(OnEnemyLoadedAndActive(info));
                    }
                    else
                    {
                        if (VERBOSE_LOGGING)
                            Dev.Log("Cannot replace " + original);
                    }

                    return original;
                }
            }
            catch (Exception e)
            {
                error = true;
                Dev.Log($"Error trying to process original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            GameObject newEnemy = null;
            try
            {
                newEnemy = currentLogic.ReplaceEnemy(original);
            }
            catch (Exception e)
            {
                error = true;
                Dev.Log($"Error trying to replace original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            FinalizeReplacement(newEnemy, original);

            return newEnemy;
        }

        public GameObject SpawnPooledObject(GameObject original)
        {
            //let the health manager replacement handle this
            if (original.GetComponent<HealthManager>() && original.activeSelf)
                return OnEnemyLoaded(original);

            //let the hazard replacement handle this
            if (original.GetComponent<DamageHero>() && original.activeSelf)
                return OnDamageHeroEnabled(original.GetComponent<DamageHero>());

            //if it's neither of those things, it's just an effect so continue as usual

            if (VERBOSE_LOGGING)
                Dev.Log("Can we replace? " + original);

            bool error = false;
            try
            {
                if (!ProcessPreReplacement(original))
                {
                    if (VERBOSE_LOGGING)
                        Dev.Log("Cannot replace " + original);

                    return original;
                }
            }
            catch(Exception e)
            {
                error = true;
                Dev.Log($"Error trying to process original pooled object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            GameObject newEffect = null;
            try
            {
                //TODO: rename "replace effect object"
                newEffect = currentLogic.ReplacePooledObject(original);
            }
            catch (Exception e)
            {
                error = true;
                Dev.Log($"Error trying to replace original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            FinalizeReplacement(newEffect, original);

            return newEffect;
        }

        public GameObject OnDamageHeroEnabled(DamageHero originalHazard)
        {
            GameObject original = originalHazard.gameObject;
            //don't invoke this for enemies
            if (originalHazard.GetComponent<HealthManager>())
                return original;
            
            if (VERBOSE_LOGGING)
                Dev.Log("Trying to replace hazard " + original);            

            bool error = false;
            try
            {
                if (!ProcessPreReplacement(original))
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log("Cannot replace " + original);
                    }
                    return original;
                }
            }
            catch (Exception e)
            {
                error = true;
                Dev.Log($"Error trying to process original hazard object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            GameObject newHazard = null;
            try
            {
                newHazard = currentLogic.ReplaceHazardObject(originalHazard.gameObject);
            }
            catch (Exception e)
            {
                error = true;
                Dev.Log($"Error trying to replace original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            if (error)
                return original;

            FinalizeReplacement(newHazard, originalHazard.gameObject);

            return newHazard;
        }

        public bool OnPersistentBoolItemLoaded(PersistentBoolItem item)
        {
            var customData = currentLogic.ReplacePersistentBoolItemData(item);
            if (customData != null)
            {
                //new data
                if (item.persistentBoolData != customData)
                {
                    item.persistentBoolData = customData;
                    return true;
                }
            }

            return false;
        }

        public bool OnPersistentBoolItemSetMyID(PersistentBoolItem item)
        {
            if (item.persistentBoolData == null)
                return false;

            var customData = currentLogic.ReplacePersistentBoolItemSetMyID(item);

            if ((string.IsNullOrEmpty(customData.ID) && string.IsNullOrEmpty(customData.SceneName)))
                return false;

            if (customData.ID != item.persistentBoolData.id ||
                customData.SceneName != item.persistentBoolData.sceneName)
            {
                if (!string.IsNullOrEmpty(customData.ID))
                    item.persistentBoolData.id = customData.ID;

                if (!string.IsNullOrEmpty(customData.SceneName))
                    item.persistentBoolData.sceneName = customData.SceneName;

                return true;
            }

            return false;
        }

        public void OnPlaymakerFSMEnabled(PlayMakerFSM fsm)
        {
            //might want to notify the loaded logics?
        }

        public void OnBeforeSceneLoad()
        {
            //TODO: infom logics?
        }

        public void OnEnemyDeathEvent(GameObject deathEventOwner)
        {
            //might want to notify the loaded logic here?
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

        protected virtual bool CullBadObjects(ObjectMetadata oldEnemy)
        {
            if (DefaultMetadata.AlwaysDeleteObject.Any(x => oldEnemy.ScenePath.Contains(x)))
            {
                if (oldEnemy.ObjectName.Contains("Fly") && oldEnemy.SceneName == "Crossroads_04")
                {
                    //this seems to correctly decrement the count from the battle manager
                    BattleManager.StateMachine.Value.RegisterEnemyDeath(null);
                }

                GameObject.Destroy(oldEnemy.Source);
                return true;
            }

            return false;
        }

        protected virtual bool SkipReplacement(ObjectMetadata oldEnemy)
        {
            //special hack to avoid having custom spawned enemies replaced
            if (EnemyRandomizer.bypassNextRandomizeEnemy)
            {
                if (oldEnemy.RandoObject == null)
                {
                    ManagedObject olde = oldEnemy.Source.GetOrAddComponent<ManagedObject>();
                    olde.Setup(oldEnemy);
                    olde.replaced = true;//flag as replaced anyway
                }
                else
                {
                    oldEnemy.RandoObject.replaced = true;
                }

                EnemyRandomizer.bypassNextRandomizeEnemy = false;
                return true;
            }
            return false;
        }

        protected virtual void MarkObjectAsReplacement(GameObject newObject, GameObject oldObject)
        {
            ObjectMetadata info = new ObjectMetadata();
            info.Setup(oldObject, database);

            ManagedObject mo = null;
            if (info.IsBattleEnemy)
                mo = newObject.AddComponent<BattleManagedObject>();
            else
                mo = newObject.AddComponent<ManagedObject>();

            mo.Setup(info);
            mo.replaced = true;
        }

        protected virtual bool ProcessPreReplacement(GameObject oldObject)
        {
            if (currentLogic == null)
                return false;

            ObjectMetadata info = new ObjectMetadata();
            info.Setup(oldObject, database);

            if (!info.HasData)
                return false;

            if (CullBadObjects(info))
                return false;

            if (SkipReplacement(info))
                return false;

            if (!info.IsActive)
                return false;

            if (info.IsAReplacementObject)
                return false;

            return true;
        }

        protected virtual void FinalizeReplacement(GameObject newObject, GameObject oldObject)
        {
            MarkObjectAsReplacement(newObject, oldObject);

            if (newObject != oldObject)
            {
                var oedf = oldObject.GetComponent<EnemyDeathEffects>();
                var nedf = newObject.GetComponent<EnemyDeathEffects>();
                if (oedf != null && nedf != null)
                {
                    string oplayerDataName = oedf.GetPlayerDataNameFromDeathEffects();
                    nedf.SetPlayerDataNameFromDeathEffects(oplayerDataName);
                }
                newObject.SafeSetActive(true);
                RemoveOldObject(oldObject);
            }
        }

        protected virtual void RemoveOldObject(GameObject oldEnemy)
        {
            //don't invoke the old enemy's ondestroy stuff- just pretend it never existed....
            if (oldEnemy.activeSelf)
                oldEnemy.SetActive(false);

            GameObject.Destroy(oldEnemy);
        }

        //TODO: temp solution for "bad effects"
        public static List<string> ReplacementEffectsToSkip = new List<string>()
        {
            "tank_fill",
            "tank_full",
            "Bugs Idle",
            "bee_fg_swarm",
            "spider sil left",
            "Butterflies FG",
            "Butterflies BG",
            "wind system",
            "water component",
            "BG_swarm_01",
            "Ruins_Rain",
            "Snore",
            "BG_swarm_02",
            "FG_swarm_02",
            "FG_swarm_01",
            "Particle System FG",
            "Particle System BG",
            "bg_dream",
            "fung_immediate_BG",
            "tank_full",
            "tank_full",
        };
    }
}
