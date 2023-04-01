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

            if (metaData.IsBoss && !EnemyRandomizer.GlobalSettings.RandomizeBosses)
                return original;

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
            if(pendingLoad.Key != null)
            {
                pendingLoads.Remove(pendingLoad.Key);
            }

            if (metaObject.Source == null)
                return null;

            if (metaObject.IsAReplacementObject)
                return metaObject.Source;

            bool canProcess = CanProcessObject(metaObject);

            if (!canProcess)
                return metaObject.Source;

            metaObject.UpdateTransformValues();

            bool replaceObject = true;

            //create default replacements
            var originalReplacementObjects = metaObject.GetObjectTypeCollection(database);

            var validReplacements = GetValidReplacements(metaObject, originalReplacementObjects);
            if (validReplacements == null || validReplacements.Count <= 0)
                replaceObject = false;

            try
            {
                //create default rng
                RNG rng = new RNG(EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
                rng = GetRNG(metaObject, rng);

                if (replaceObject)
                {
                    if(TryReplaceObject(metaObject, validReplacements, rng, out var newObject))
                    {
                        if (VERBOSE_LOGGING)
                        {
                            Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Object will be replaced by [{newObject.ObjectType}, {newObject.ObjectName}] .");
                        }
                        newObject.MarkObjectAsReplacement(metaObject);
                        metaObject = newObject;
                    }
                }

                metaObject = ModifyObject(metaObject);
            }
            catch(Exception e)
            {
                Dev.Log($"[{metaObject.ObjectType}, {metaObject.ObjectName}]: Fatal error randomzing object ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

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
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Destroying invalid object.");
                    }

                    metaObject.DestroySource();
                    return false;
                }

                if (metaObject.IsTemporarilyInactive())// || metaObject.IsBattleInactive())
                {
                    if (VERBOSE_LOGGING && GetBlackBorders().Value != null && GetBlackBorders().Value.Count > 0)
                    {
                        Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Cannot process object yet. Queuing for activation later.");
                        metaObject.Dump();
                    }

                    var loader = OnObjectLoadedAndActive(metaObject);
                    GameManager.instance.StartCoroutine(loader);
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
                    Dev.Log($"[{metaObject.ObjectType}, {metaObject.ScenePath}]: Can process object.");

                    metaObject.Dump();
                }
            }

            return canProcess;
        }

        protected IEnumerator OnObjectLoadedAndActive(ObjectMetadata info)
        { 
            if(info == null)
            {
                yield break;
            }

            yield return new WaitUntil(() => info == null || info.CheckIfIsActiveAndVisible() || info.Source == null);
            if(info == null || info.Source == null)
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
                    validReplacements = logic.GetValidReplacements(original, validReplacements);
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
                    rng = logic.GetRNG(original, rng, EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
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
            ObjectMetadata metaObject = null;
            foreach (var logic in loadedLogics)
            {
                try
                {
                    metaObject = logic.GetReplacement(metaObject, original, validReplacements, rng);
                }
                catch (Exception e)
                {
                    Dev.Log($"Error trying to replace object in logic {logic.Name} using data from {original.ObjectName} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
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

        public static List<string> ReplacementEnemiesToSkip = new List<string>()
        {
            "Dream Mage Lord Phase2",
            "Mage Lord Phase2",
            "Corpse Garden Zombie", //don't spawn this, it's just a corpse
        };

        public static List<string> ReplacementHazardsToSkip = new List<string>()
        {
            "Cave Spikes tile", //small bunch of upward pointing cave spikes
            "Cave Spikes tile(Clone)"
        };

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
            "fung_immediate_BG",  //
            "tank_full", //large rect of acid, LOOPING
            "bg_dream",  //big dream circles that float in the background, LOOPING
            "Bugs Idle", //floaty puffs of glowy moth things, LOOPING
            "Shade Particles", //sprays out shade stuff in an wide upward spray, LOOPING
            "Fire Particles", //emits burning fire particles, LOOPING
            "spawn particles b", //has a serialization error? -- may need fixing/don't destroy on load or something
            "Acid Steam", //has a serialization error?
            "Spre Fizzle", //emits a little upward spray of green particles, LOOPING
            "Dust Land", //emits a crescent of steamy puffs that spray upward a bit originating a bit under the given origin, LOOPING
            "Slash Ball", //emits the huge pale lurker slash AoE, LOOPING
            "Bone", //serialzation error?
            "Particle System", //serialization error?
            "Dust Land Small", //??? seems to be making lots of issues
            "Infected Grass A", //??? seems to be making lots of issues
            //"Dust Trail", // --- likely needs mod to control pooling
        };


        public static List<string> BurstEffects = new List<string>()
        {
            "Roar Feathers",
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