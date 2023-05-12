﻿using System.Collections;
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
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class EnemyReplacer
    {
        public static bool VERBOSE_LOGGING = true;

        public EnemyRandomizerDatabase database;
        public HashSet<IRandomizerLogic> loadedLogics = new HashSet<IRandomizerLogic>();
        public HashSet<IRandomizerLogic> previousLogic = new HashSet<IRandomizerLogic>();
        public Dictionary<ObjectMetadata, IEnumerator> pendingLoads = new Dictionary<ObjectMetadata, IEnumerator>();

        public List<(string, string)> GetPreloadNames(string databaseFilePath)
        {
            database = LoadDatabase(databaseFilePath);
            if (database == null)
                return new List<(string, string)>();

            //needed early to use internally during db load
            EnemyRandomizerDatabase.GetDatabase -= GetCurrentDatabase;
            EnemyRandomizerDatabase.GetDatabase += GetCurrentDatabase;

            return database.GetPreloadNames();
        }

        public void Setup(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            database.Initialize(preloadedObjects);
            database.Finalize(null);
        }

        public List<IRandomizerLogic> ConstructLogics(List<string> previouslyLoadedLogics, Dictionary<string, IRandomizerLogic> logics)
        {
            Dev.Log("constructing logics");
            //setup/construct all logics
            foreach (var logic in logics.Select(x => x.Value))
            {
                Dev.Log("Creating " + logic.Name);
                logic.Setup(database);

                //if this is the first time a logic is loaded
                var hasBeenLoadedBefore = logic.Settings.GetOption("__HAS_BEEN_LOADED_BEFORE__");
                if (!hasBeenLoadedBefore.value)
                {
                    hasBeenLoadedBefore.value = true;

                    //check if it should be enabled by default
                    if (logic.EnableByDefault)
                    {
                        //and enable it
                        loadedLogics.Add(logic);
                        EnableLogic(logic);
                    }
                }
                else
                {
                    if (previouslyLoadedLogics.Contains(logic.Name))
                        EnableLogic(logic);
                }
            }

            var enabledLogics = logics.Where(x => loadedLogics.Contains(x.Value)).Select(x => x.Value).ToList();
            EnemyRandomizer.GlobalSettings.loadedLogics = EnemyRandomizer.GlobalSettings.loadedLogics.Concat(enabledLogics.Select(x => x.Name)).Distinct().ToList();

            return enabledLogics;
        }

        public EnemyRandomizerDatabase GetCurrentDatabase()
        {
            return database;
        }

        public ReactiveProperty<List<GameObject>> GetBlackBorders()
        {
            return EnemyRandomizer.BlackBorders;
        }

        public void OnModEnabled()
        {
            EnemyRandomizerDatabase.GetDatabase -= GetCurrentDatabase;
            EnemyRandomizerDatabase.GetDatabase += GetCurrentDatabase;

            EnemyRandomizerDatabase.GetBlackBorders -= GetBlackBorders;
            EnemyRandomizerDatabase.GetBlackBorders += GetBlackBorders;

            EnemyRandomizerDatabase.CustomSpawnWithLogic -= EnemyRandomizer.CustomSpawn;
            EnemyRandomizerDatabase.CustomSpawnWithLogic += EnemyRandomizer.CustomSpawn;

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
            EnemyRandomizerDatabase.GetBlackBorders -= GetBlackBorders;
            EnemyRandomizerDatabase.CustomSpawnWithLogic -= EnemyRandomizer.CustomSpawn;
        }

        public void EnableLogic(IRandomizerLogic newLogic, bool updateSettings = true)
        {
            loadedLogics ??= new HashSet<IRandomizerLogic>();

            if (EnemyRandomizer.GlobalSettings == null)
                throw new NullReferenceException("Error: Global settings is null! (this should never happen)");

            if(EnemyRandomizer.GlobalSettings.loadedLogics == null)
                throw new NullReferenceException("Error: List of loaded modules is null! (this should never happen)");

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
            if (loadedLogics == null)
                loadedLogics = new HashSet<IRandomizerLogic>();

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
            Dev.Log("Game Loaded! Enabling logics...");
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
            Dev.Where();
            pendingLoads.ToList().ForEach(x => GameManager.instance.StopCoroutine(x.Value));
            pendingLoads.Clear();
        }

        public GameObject RandomizeEnemy(GameObject original)
        {
            var metaData = original.ToMetadata(database);

            //special logic for the giant fly boss
            if (original.name.Contains("Giant Fly"))
            {
                var fixedBossControl = original.GetOrAddComponent<GiantFlyControl>();
                fixedBossControl.Setup(metaData);
                metaData.MarkObjectAsReplacement(metaData);
                return original;
            }

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
            if (VERBOSE_LOGGING && metaObject.HasData && !metaObject.IsAReplacementObject && metaObject.Source != null)
            {
                Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Unrandomized object loaded.");
            }

            var pendingLoad = pendingLoads.FirstOrDefault(x => x.Key.ScenePath == metaObject.ScenePath);
            if (pendingLoad.Key != null)
            {
                pendingLoads.Remove(pendingLoad.Key);
            }

            if (metaObject.Source == null)
                return null;

            if (metaObject.IsAReplacementObject)
                return metaObject.Source;

            //TODO: add this to the modules DO THIS NEXT!!!!!!!!!!!!!!!
            bool canProcess = CanProcessObject(metaObject);

            if (!canProcess)
                return metaObject.Source;

            bool replaceObject = true;

            List<PrefabObject> originalReplacementObjects = null;
            List<PrefabObject> validReplacements = null;

            //are we skipping replacement?
            if (EnemyRandomizer.DoReplacementBypassCheck())
            {
                if (EnemyRandomizer.HasCustomBypassReplacement())
                {
                    string customBypassReplacement = EnemyRandomizer.GetCustomBypassReplacement();
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Replacement bypass set to {customBypassReplacement}. Will attempt to replace object with this custom object.");
                    }

                    try
                    {
                        validReplacements = new List<PrefabObject>() { database.Objects[customBypassReplacement] };
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"[{customBypassReplacement}]: Invalid object key used for custom bypass replacement. Cannot use to replace object.");
                        validReplacements = new List<PrefabObject>();
                        replaceObject = false;
                    }
                }
                else
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Replacement bypass set. Will NOT attempt to replace object.");
                    }

                    replaceObject = false;
                }
            }
            else
            {
                //create default replacements
                originalReplacementObjects = metaObject.GetObjectTypeCollection(database);
                validReplacements = originalReplacementObjects;

                if (VERBOSE_LOGGING)
                {
                    Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Will attempt to replace object.");
                }

                validReplacements = GetValidReplacements(metaObject, originalReplacementObjects);
                if (validReplacements == null || validReplacements.Count <= 0)
                    replaceObject = false;
            }

            try
            {
                //create default rng
                RNG rng = new RNG(EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
                rng = GetRNG(metaObject, rng);

                if (replaceObject)
                {
                    if (TryReplaceObject(metaObject, validReplacements, rng, out var newObject))
                    {
                        if (VERBOSE_LOGGING)
                        {
                            Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Object will be replaced by [{newObject.ObjectType}, {newObject.ObjectName}] .");
                        }

                        if(newObject.Source != metaObject.Source)
                            EnemyRandomizerDatabase.OnObjectReplaced?.Invoke((newObject, metaObject));

                        newObject.MarkObjectAsReplacement(metaObject);
                        metaObject = newObject;

                        if (VERBOSE_LOGGING)
                            Dev.Log($"[{newObject.ObjectType}, {newObject.ObjectName}]: Was marked as a replacement for [{metaObject.ObjectType}, {metaObject.ObjectName}] .");
                    }
                }
                else
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Object was not replaced and will be marked to prevent processing by the randomizer again.");
                    }
                    metaObject.MarkObjectAsReplacement(metaObject);
                }
            }
            catch (Exception e)
            {
                Dev.LogError($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Fatal error replacing object -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

            try
            {
                if (VERBOSE_LOGGING)
                    Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Trying to modify....");

                metaObject = ModifyObject(metaObject);
            }
            catch(Exception e)
            {
                Dev.LogError($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Fatal error modifying object -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

            try
            {
                if (VERBOSE_LOGGING)
                    Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Trying to fix logic....");

                FixForLogic(metaObject);
            }
            catch (Exception e)
            {
                Dev.LogError($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Fatal error fixing object for logic -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

            if (VERBOSE_LOGGING)
                Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Trying to finalize and activate....");

            return metaObject.ActivateSource();
        }

        protected bool CanProcessObject(ObjectMetadata metaObject)
        {
            bool canProcess = CanProcessNow(metaObject);

            if (!canProcess && EnemyRandomizer.DoReplacementBypassCheck(true))
                canProcess = true;

            if (!canProcess)
            {
                if (metaObject.IsInvalidObject)
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Destroying invalid object.");
                    }

                    metaObject.DestroySource();
                    return false;
                }

                if (metaObject.isTemporarilyInactive.Value)
                {
                    if (VERBOSE_LOGGING && GetBlackBorders().Value != null && GetBlackBorders().Value.Count > 0)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Cannot process object yet. Queuing for activation later.");
                        //metaObject.Dump();
                    }

                    var loader = OnObjectLoadedAndActive(metaObject);
                    GameManager.instance.StartCoroutine(loader);
                    pendingLoads.Add(metaObject, loader);

                    //going to return original -- can't process the object yet
                }
                else
                {
                    //object is inactive, can't process at all, return original
                    if(VERBOSE_LOGGING)
                    {
                        Dev.LogWarning($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Cannot process inactive object.");
                        //metaObject.Dump();
                    }
                }
            }
            else
            {
                bool result = true;
                foreach (var logic in loadedLogics)
                {
                    try
                    {
                        result = logic.CanReplaceObject(metaObject);
                        if (!result)
                            break;
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Error trying to check logic {logic.Name} to see if {metaObject.ObjectName} can be replaced... ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                    }
                }
            }

            return canProcess;
        }

        protected IEnumerator OnObjectLoadedAndActive(ObjectMetadata info)
        { 
            if(info == null || info.Source == null || info.IsDisabledBySavedGameState)
            {
                yield break;
            }

            yield return new WaitUntil(() => info == null || info.Source == null || (!info.IsDisabledBySavedGameState && info.ActiveSelf && info.renderersVisible.Value && info.InBounds));
            if(info == null || info.Source == null || info.IsDisabledBySavedGameState)
            {
                pendingLoads.Remove(info);
                yield break;
            }
            yield return new WaitUntil(() => GetBlackBorders().Value != null && GetBlackBorders().Value.Count > 0);
            if (info != null)
            {
                OnObjectLoaded(info);
                pendingLoads.Remove(info);
            }
        }

        protected bool CanProcessNow(ObjectMetadata original)
        {
            if (loadedLogics == null || loadedLogics.Count <= 0)
            {
                if(VERBOSE_LOGGING)
                    Dev.LogWarning($"No logics are loaded. Cannot process any objects!");

                return false;
            }

            bool canProcessNow = true;
            try
            {
                if (!original.CanProcessObject)
                    canProcessNow = false;
            }
            catch (Exception e)
            {
                canProcessNow = false;

                if (VERBOSE_LOGGING && !original.IsInvalidObject && original.HasData)
                {
                    Dev.LogError($"Error trying to check if this object can be processed {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                    original.Dump();
                }
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
                    validReplacements = logic.GetValidReplacements(original, validReplacements);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to load valid replacements in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
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
                    rng = logic.GetRNG(original, rng, EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to load RNG in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            return rng;
        }

        protected bool TryReplaceObject(ObjectMetadata original, List<PrefabObject> validReplacements, RNG rng, out ObjectMetadata newObject)
        {
            ObjectMetadata metaObject = null;
            foreach (var logic in loadedLogics)
            {
                try
                {
                    metaObject = logic.GetReplacement(metaObject, original, validReplacements, rng);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to replace object in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            newObject = metaObject;
            return newObject != original;
        }

        protected ObjectMetadata ModifyObject(ObjectMetadata metaObject)
        {
            ObjectMetadata original = metaObject.ObjectThisReplaced;
            foreach (var logic in loadedLogics)
            {
                try
                {
                    metaObject = logic.ModifyObject(metaObject, original);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to modify object in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            return metaObject;
        }

        /// <summary>
        /// Apply special fixes to this enemy to prevent logical softlocks or other critical issues
        /// </summary>
        protected void FixForLogic(ObjectMetadata metaObject)
        {
            try
            {
                database.FixForLogic(metaObject);
            }
            catch (Exception e)
            {
                Dev.LogError($"Error trying to fix object {metaObject} for softlock prevention logic; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
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
                    Dev.LogError($"Failed to load database file {Path.GetFileName(fileName)} from mod directory");
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