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
using UniRx;
using UniRx.Triggers;

namespace EnemyRandomizerMod
{
    public class EnemyReplacer
    {
        public static bool VERBOSE_LOGGING = false;

        public EnemyRandomizerDatabase database;
        public HashSet<IRandomizerLogic> loadedLogics = new HashSet<IRandomizerLogic>();
        public HashSet<IRandomizerLogic> previousLogic = new HashSet<IRandomizerLogic>();
        public Dictionary<ObjectMetadata, IEnumerator> pendingLoads = new Dictionary<ObjectMetadata, IEnumerator>();

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

            foreach (var logic in previousLogic)
            {
                EnableLogic(logic);
            }
        }

        public void OnModDisabled()
        {
            previousLogic = new HashSet<IRandomizerLogic>(loadedLogics);

            foreach (var logic in loadedLogics)
            {
                DisableLogic(logic, false);
            }

            EnemyRandomizerDatabase.GetDatabase -= GetCurrentDatabase;
        }

        public void EnableLogic(IRandomizerLogic newLogic, bool updateSettings = true)
        {
            if (updateSettings)
            {
                if (!EnemyRandomizer.GlobalSettings.loadedLogics.Contains(newLogic.Name))
                    EnemyRandomizer.GlobalSettings.loadedLogics.Add(newLogic.Name);
            }
            loadedLogics.Add(newLogic);
            
            if (IsInGameScene())
            {
                if (!newLogic.OnStartGameWasCalled)
                    newLogic.OnStartGame(EnemyRandomizer.PlayerSettings);

                if(!newLogic.Enabled)
                    newLogic.Enable();
            }
        }

        public void DisableLogic(IRandomizerLogic oldLogic, bool updateSettings = true)
        {
            if (updateSettings)
            {
                if (EnemyRandomizer.GlobalSettings.loadedLogics.Contains(oldLogic.Name))
                    EnemyRandomizer.GlobalSettings.loadedLogics.Remove(oldLogic.Name);
            }
            loadedLogics.Remove(oldLogic);

            if (IsInGameScene())
            {
                if(oldLogic.Enabled)
                    oldLogic.Disable();               
            }
        }

        public void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            foreach (var logic in loadedLogics)
            {
                if (logic.Enabled)
                    logic.Reset();
            }

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

            foreach (var logic in loadedLogics)
            {
                logic.OnStartGame(settings);

                if (!logic.Enabled)
                    logic.Enable();
            }

            yield break;
        }

        public void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
            foreach (var logic in loadedLogics)
            {
                logic.OnSaveGame(settings);
            }
        }

        public void ClearPendingLoads()
        {
            pendingLoads.Where(x => x.Key.EnemyHealthManager != null).ToList().ForEach(x => x.Key.EnemyHealthManager.StopCoroutine(x.Value));
            pendingLoads.Clear();
        }

        public GameObject RandomizeEnemy(GameObject original)
        {
            var metaData = original.ToMetadata(database);
            return OnObjectLoaded(metaData);
        }

        public GameObject RandomizeHazard(DamageHero originalHazard)
        {
            var metaData = originalHazard.gameObject.ToMetadata(database);
            return OnObjectLoaded(metaData);
        }

        public GameObject RandomizeEffect(GameObject original)
        {
            var metaData = original.ToMetadata(database);
            return OnObjectLoaded(metaData);
        }

        public GameObject OnObjectLoaded(ObjectMetadata metaObject)
        {
            if (VERBOSE_LOGGING && metaObject.HasData && !metaObject.IsAReplacementObject)
            {
                Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Unrandomized object loaded.");
            }

            bool canProcess = CanProcessObject(metaObject);

            if (!canProcess)
                return metaObject.Source;

            bool replaceObject = true;

            //create default replacements
            var originalReplacementObjects = metaObject.GetObjectTypeCollection(database);

            var validReplacements = GetValidReplacements(metaObject, originalReplacementObjects);
            if (validReplacements == null || validReplacements.Count <= 0)
                replaceObject = false;

            //create default rng
            RNG rng = new RNG(EnemyRandomizer.PlayerSettings.seed);
            rng = GetRNG(metaObject, rng);

            if (replaceObject)
            {
                if(TryReplaceObject(metaObject, validReplacements, rng, out var newObject))
                {
                    newObject.MarkObjectAsReplacement(metaObject);
                    metaObject = newObject;
                }
            }

            metaObject = ModifyObject(metaObject);

            return metaObject.ActivateSource();
        }

        protected bool CanProcessObject(ObjectMetadata metaObject)
        {
            bool canProcess = CanProcessNow(metaObject);

            if (!canProcess)
            {
                if (metaObject.IsInvalidObject)
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Destroying invalid object.");
                    }

                    metaObject.DestroySource();
                }

                if (metaObject.IsTemporarilyInactive())
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Cannot process object yet. Queuing for activation later.");
                    }

                    var loader = OnObjectLoadedAndActive(metaObject);
                    metaObject.EnemyHealthManager.StartCoroutine(loader);
                    pendingLoads.Add(metaObject, loader);

                    //going to return original -- can't process the object yet
                }
                else
                {
                    //object is inactive, can't process at all, return original
                }

                //if (VERBOSE_LOGGING && metaObject.HasData && !metaObject.IsAReplacementObject)
                //{
                //    Dev.LogWarning($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Cannot process object for some other reason.");
                //}
            }
            else
            {
                if (VERBOSE_LOGGING && metaObject.HasData && !metaObject.IsAReplacementObject)
                {
                    Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Can process object.");
                }
            }

            return canProcess;
        }

        protected IEnumerator OnObjectLoadedAndActive(ObjectMetadata info)
        {
            yield return new WaitUntil(() => info.IsVisibleNow());
            OnObjectLoaded(info);
            pendingLoads.Remove(info);
        }

        protected bool CanProcessNow(ObjectMetadata original)
        {
            if (loadedLogics == null || loadedLogics.Count <= 0)
                return false;

            bool canProcessNow = true;
            try
            {
                if (!original.CanProcessObject())
                    canProcessNow = false;
            }
            catch (Exception e)
            {
                canProcessNow = false;
                Dev.Log($"Error trying to check if this object can be processed {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                
                if(VERBOSE_LOGGING)
                    original.Dump();
            }

            return canProcessNow;
        }

        public List<PrefabObject> GetValidReplacements(ObjectMetadata original, List<PrefabObject> validReplacementObjects)
        {
            List<PrefabObject> validReplacements = validReplacementObjects;

            foreach (var logic in loadedLogics)
            {
                try
                {
                    validReplacements = logic.GetValidReplacements(original, validReplacementObjects);
                }
                catch (Exception e)
                {
                    Dev.Log($"Error trying to load valid replacements in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }

            return validReplacements;
        }

        protected RNG GetRNG(ObjectMetadata original, RNG defaultRNG)
        {
            RNG rng = defaultRNG;

            foreach (var logic in loadedLogics)
            {
                try
                {
                    rng = logic.GetRNG(original, rng, EnemyRandomizer.PlayerSettings.seed);
                    //newObject = logic.ReplaceEnemy(original);
                }
                catch (Exception e)
                {
                    Dev.Log($"Error trying to load RNG in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            return rng;
        }

        protected bool TryReplaceObject(ObjectMetadata original, List<PrefabObject> validReplacements, RNG rng, out ObjectMetadata newObject)
        {
            ObjectMetadata metaObject = original;
            foreach (var logic in loadedLogics)
            {
                try
                {
                    metaObject = logic.GetReplacement(metaObject, validReplacements, rng);
                }
                catch (Exception e)
                {
                    Dev.Log($"Error trying to replace object in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            newObject = metaObject;
            return newObject != original;
        }

        protected ObjectMetadata ModifyObject(ObjectMetadata original)
        {
            ObjectMetadata metaObject = original;
            foreach (var logic in loadedLogics)
            {
                try
                {
                    metaObject = logic.ModifyObject(metaObject);
                }
                catch (Exception e)
                {
                    Dev.Log($"Error trying to modify object in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            return metaObject;
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




        public bool OnPersistentBoolItemLoaded(PersistentBoolItem item)
        {
            PersistentBoolData customData = null;
            foreach (var logic in loadedLogics)
            {
                customData = logic.ReplacePersistentBoolItemData(item);

                //only allow one replacement
                if (customData != null)
                    break;
            }

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

            (string ID, string SceneName) customData = default;
            foreach (var logic in loadedLogics)
            {
                customData = logic.ReplacePersistentBoolItemSetMyID(item);

                //only allow one replacement
                if ((string.IsNullOrEmpty(customData.ID) && string.IsNullOrEmpty(customData.SceneName)))
                    continue;
                else
                    break;
            }

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
    }
}






//protected virtual void RemoveOldObject(GameObject oldEnemy)
//{
//    //don't invoke the old enemy's ondestroy stuff- just pretend it never existed....
//    if (oldEnemy.activeSelf)
//        oldEnemy.SetActive(false);

//    GameObject.Destroy(oldEnemy);
//}

//public GameObject SpawnPooledObject(GameObject original)
//{
//    //let the health manager replacement handle this
//    if (original.GetComponent<HealthManager>() && original.activeSelf)
//        return OnEnemyLoaded(original);

//    //let the hazard replacement handle this
//    if (original.GetComponent<DamageHero>() && original.activeSelf)
//        return OnDamageHeroEnabled(original.GetComponent<DamageHero>());

//    //if it's neither of those things, it's just an effect so continue as usual

//    if (VERBOSE_LOGGING)
//        Dev.Log("Can we replace? " + original);

//    bool error = false;
//    try
//    {
//        if (!ProcessPreReplacement(original))
//        {
//            if (VERBOSE_LOGGING)
//                Dev.Log("Cannot replace " + original);

//            return original;
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to process original pooled object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    if (error)
//        return original;

//    GameObject newEffect = null;
//    try
//    {
//        foreach (var logic in loadedLogics)
//        {
//            //TODO: rename "replace effect object"
//            newEffect = logic.ReplacePooledObject(original);

//            //only allow one replacement
//            if (newEffect != original)
//                break;
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to replace original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    if (error)
//        return original;

//    try
//    {
//        ObjectMetadata oldData = new ObjectMetadata();
//        oldData.Setup(original, database);

//        ObjectMetadata newData = new ObjectMetadata();
//        newData.Setup(newEffect, database);
//        foreach (var logic in loadedLogics)
//        {
//            newData = logic.ModifyPooledObject(newData, oldData);
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to modify new enemy object {newEffect.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    FinalizeReplacement(newEffect, original);

//    return newEffect;
//}


//protected virtual bool SkipReplacement(ObjectMetadata oldEnemy)
//{
//    //special hack to avoid having custom spawned enemies replaced
//    if (EnemyRandomizer.bypassNextRandomizeEnemy)
//    {
//        if (oldEnemy.RandoObject == null)
//        {
//            ManagedObject olde = oldEnemy.Source.GetOrAddComponent<ManagedObject>();
//            olde.Setup(oldEnemy);
//            olde.replaced = true;//flag as replaced anyway
//        }
//        else
//        {
//            oldEnemy.RandoObject.replaced = true;
//        }

//        EnemyRandomizer.bypassNextRandomizeEnemy = false;
//        return true;
//    }
//    return false;
//}

//public GameObject OnDamageHeroEnabled(DamageHero originalHazard)
//{
//    GameObject original = originalHazard.gameObject;
//    //don't invoke this for enemies
//    if (originalHazard.GetComponent<HealthManager>())
//        return original;

//    if (VERBOSE_LOGGING)
//        Dev.Log("Trying to replace hazard " + original);            

//    bool error = false;
//    try
//    {
//        if (!ProcessPreReplacement(original))
//        {
//            if (VERBOSE_LOGGING)
//            {
//                Dev.Log("Cannot replace " + original);
//            }
//            return original;
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to process original hazard object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    if (error)
//        return original;

//    GameObject newHazard = null;
//    try
//    {
//        foreach (var logic in loadedLogics)
//        {
//            newHazard = logic.ReplaceHazardObject(originalHazard.gameObject);

//            //only allow one replacement
//            if (newHazard != original)
//                break;
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to replace original enemy object {original.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    if (error)
//        return original;

//    try
//    {
//        ObjectMetadata oldData = new ObjectMetadata();
//        oldData.Setup(original, database);

//        ObjectMetadata newData = new ObjectMetadata();
//        newData.Setup(newHazard, database);
//        foreach (var logic in loadedLogics)
//        {
//            newData = logic.ModifyHazardObject(newData, oldData);
//        }
//    }
//    catch (Exception e)
//    {
//        error = true;
//        Dev.Log($"Error trying to modify new enemy object {newHazard.name} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
//    }

//    FinalizeReplacement(newHazard, originalHazard.gameObject);

//    return newHazard;
//}