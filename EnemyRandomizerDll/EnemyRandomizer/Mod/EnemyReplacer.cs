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
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class EnemyReplacer
    {
        public static bool VVERBOSE_LOGGING = false;
        public static bool VERBOSE_LOGGING = false;

        public EnemyRandomizerDatabase database;
        public HashSet<IRandomizerLogic> loadedLogics = new HashSet<IRandomizerLogic>();
        public HashSet<IRandomizerLogic> previousLogic = new HashSet<IRandomizerLogic>();
        public Dictionary<GameObject, IEnumerator> pendingLoads = new Dictionary<GameObject, IEnumerator>();

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
            bool loadedMoreLogics = false;
            Dev.Log("Getting all known logics");
            //setup/construct all logics
            foreach (var logic in logics.Select(x => x.Value))
            {
                //Dev.Log($"loadedLogics.Contains{logic.Name} = {loadedLogics.Contains(logic)}");
                //Dev.Log($"previouslyLoadedLogics.Contains{logic.Name} = {previouslyLoadedLogics.Contains(logic.Name)}");
                //Dev.Log($"logic.Enabled{logic.Name} = {logic.Enabled}");

                //has already been loaded
                if (loadedLogics.Contains(logic) && previouslyLoadedLogics.Contains(logic.Name))
                {
                    //if we're not in a game scene, the call to EnableLogic won't do much, but call it anyways just in case we are...
                    if(!logic.Enabled)
                    {
                        EnableLogic(logic);
                    }

                    continue;
                }

                loadedMoreLogics = true;

                Dev.Log("Preparing " + logic.Name);
                logic.Setup(database);

                //if this is the first time a logic is loaded
                var hasBeenLoadedBefore = logic.Settings.GetOption("__HAS_BEEN_LOADED_BEFORE__");
                if (!hasBeenLoadedBefore.value)
                {
                    Dev.Log("Creating " + logic.Name);
                    hasBeenLoadedBefore.value = true;

                    //check if it should be enabled by default
                    if (logic.EnableByDefault)
                    {
                        Dev.Log("Default enabling " + logic.Name);
                        //and enable it
                        loadedLogics.Add(logic);
                        EnableLogic(logic);
                    }
                }
                else
                {
                    if (previouslyLoadedLogics.Contains(logic.Name))
                    {
                        Dev.Log("Enabling " + logic.Name);
                        EnableLogic(logic);
                    }
                    else
                    {
                        //leave as is
                    }
                }
            }

            //no change
            if(!loadedMoreLogics)
            {
                Dev.Log("Nothing new to load");
                return loadedLogics.ToList();
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

            SpawnerExtensions.SetupBoundsReactives();

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
            return OnObjectLoaded(original);
        }

        public GameObject RandomizeHazard(DamageHero originalHazard)
        {
            return OnObjectLoaded(originalHazard.gameObject);
        }

        public GameObject RandomizeEffect(GameObject original)
        {
            return OnObjectLoaded(original);
        }

        protected virtual void RemovePendingLoad(GameObject loadedObjectToProcess)
        {
            var pendingLoad = pendingLoads.FirstOrDefault(x => x.Key.GetSceneHierarchyPath() == loadedObjectToProcess.GetSceneHierarchyPath());
            if (pendingLoad.Key != null)
            {
                if (VERBOSE_LOGGING)
                {
                    Dev.Log($"[{loadedObjectToProcess}]: Removing pending load with key {pendingLoad.Key}.");
                }
                pendingLoads.Remove(pendingLoad.Key);
            }
        }

        protected virtual bool OnObjectLoadedBypassed(GameObject loadedObjectToProcess)
        {
            if (EnemyRandomizer.DoReplacementBypassCheck())
            {
                if (VERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Replacement bypass is enabled.");

                //should not be null..
                var thisMetaData = ObjectMetadata.Get(loadedObjectToProcess);
                var originalMetaData = ObjectMetadata.GetOriginal(loadedObjectToProcess);

                if (VERBOSE_LOGGING)
                    Dev.Log($"[THIS:{thisMetaData}, ORIGINAL:{originalMetaData}]: Bypass checking metas...");

                var soc = loadedObjectToProcess.GetComponent<SpawnedObjectControl>();
                bool willSetup = false;
                if (soc == null)
                {
                    if (VERBOSE_LOGGING)
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: No spawned object control on the object");
                    soc = loadedObjectToProcess.AddComponent<SpawnedObjectControl>();
                    willSetup = true;
                }
                else
                {
                    if (soc.thisMetadata == null)
                        willSetup = true;
                }

                if(willSetup)
                    soc.Setup(null);

                if (!EnemyRandomizer.HasCustomBypassReplacement())
                {
                    if (soc != null)
                    {
                        loadedObjectToProcess.FinalizeReplacement(null);
                    }

                    //if (soc != null)
                    //{
                    //    soc.thisMetadata = thisMetaData == null ? new ObjectMetadata(loadedObjectToProcess) : thisMetaData;
                    //    soc.originialMetadata = originalMetaData == null ? new ObjectMetadata(loadedObjectToProcess) : originalMetaData;

                        //    //apply the position logic
                        //    soc.SetPositionOnSpawn();
                        //    soc.MarkLoaded();
                        //}
                }
                else
                {
                    string customBypassReplacement = EnemyRandomizer.GetCustomBypassReplacement();
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Replacement bypass set to {customBypassReplacement}. Will attempt to replace object with this custom object.");
                    }
                    var validReplacements = new List<PrefabObject>() { database.Objects[customBypassReplacement] };

                    GameObject replacementObject = null;
                    try
                    {
                        replacementObject = TryGetReplaceObject(loadedObjectToProcess, validReplacements);
                        if(replacementObject != null)
                        {
                            replacementObject.FinalizeReplacement(loadedObjectToProcess);
                        }
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"[{customBypassReplacement}]: Error attempting custom replacement.");
                    }
                }

                return true;
            }

            return false;
        }

        public GameObject OnObjectLoaded(GameObject loadedObjectToProcess)
        {
            if (loadedObjectToProcess == null)
            {
                if (VERBOSE_LOGGING)
                {
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because object is now null.");
                    Dev.Log("[][][]===========================================================================[][][]");
                }
                RemovePendingLoad(loadedObjectToProcess);
                return loadedObjectToProcess;
            }

            if (OnObjectLoadedBypassed(loadedObjectToProcess))
            {
                if(VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because OnObjectLoadedBypassed is true.");

                return loadedObjectToProcess;
            }

            //don't process these object types
            var objectType = loadedObjectToProcess.ObjectType();
            if (objectType == PrefabObject.PrefabType.None || objectType == PrefabObject.PrefabType.Other)
            {
                if (VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because ObjectType() is None or Other.");
                return loadedObjectToProcess;
            }

            //skip if not a rando object
            if (!loadedObjectToProcess.IsDatabaseObject())
            {
                if (VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because IsDatabaseObject() is false.");
                return loadedObjectToProcess;
            }

            //for now, lets entirely mute/skip global objects like this
            if (loadedObjectToProcess.SceneName() == "DontDestroyOnLoad")
            {
                if (VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because the object is located in the DontDestroyOnLoad scene.");
                return loadedObjectToProcess;
            }

            //do nothing if this replaced something already
            if (loadedObjectToProcess.HasReplacedAnObject())
            {
                if (VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because this HasReplacedAnObject().");
                return loadedObjectToProcess;
            }

            bool isReplacementLogicLoaded = loadedLogics.Any(x => x.WillReplaceType(objectType));
            bool isModificationLogicLoaded = loadedLogics.Any(x => x.WillModifyType(objectType));

            //no logic is loaded that will have any meaningful effect on this game object
            if (!isReplacementLogicLoaded && !isModificationLogicLoaded)
            {
                if (VVERBOSE_LOGGING)
                    Dev.Log($"[{loadedObjectToProcess}]: Cancelling OnObjectLoaded processing because no enabled logic will modify or replace this object.");
                return loadedObjectToProcess;
            }

            if(VERBOSE_LOGGING)
                Dev.Log("[][][]================VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV==================[][][]");

            if (VERBOSE_LOGGING && loadedObjectToProcess.IsAFreshObject())
                Dev.Log($"[{loadedObjectToProcess.SceneName()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Fresh object loaded.");
            
            if (VERBOSE_LOGGING && loadedObjectToProcess.IsASpawnedObject())
                Dev.Log($"[{ObjectMetadata.Get(loadedObjectToProcess)}]: Spawned object loaded.");

            RemovePendingLoad(loadedObjectToProcess);

            if (!WillProcessObject(loadedObjectToProcess))
            {
                if (VERBOSE_LOGGING)
                {
                    Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: WillProcessObject() returned false: will not process this object now.");
                    Dev.Log("[][][]===========================================================================[][][]");
                }

                return loadedObjectToProcess;
            }

            bool skipForLogic = SkipForLogic(loadedObjectToProcess);
            GameObject replacementObject = null;
            if (isReplacementLogicLoaded && !skipForLogic)
            {
                //Get Possible Replacements============================
                List<PrefabObject> validReplacements = GetValidReplacements(loadedObjectToProcess);

                //Create A Replacement============================
                if ((validReplacements != null && validReplacements.Count > 0))
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Will attempt to replace object.");
                    }
                    replacementObject = TryGetReplaceObject(loadedObjectToProcess, validReplacements);
                }
                else
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: No valid objects found to replace {loadedObjectToProcess}.");
                    }
                }
            }

            //Modify The Replacement or Loaded Object============================
            var objectToModifyAndActivate = replacementObject == null ? loadedObjectToProcess : replacementObject;
            var objectToSourceAndDestroy = replacementObject != null ? loadedObjectToProcess : replacementObject;

            if (!skipForLogic)
            {
                if (isModificationLogicLoaded)
                {
                    try
                    {
                        ModifyObject(objectToModifyAndActivate, objectToSourceAndDestroy);
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"{objectToModifyAndActivate.GetSceneHierarchyPath()}: Fatal error modifying object [{objectToModifyAndActivate.GetSceneHierarchyPath()}] with intention to replace this object [{objectToSourceAndDestroy}] -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
                    }
                }

                if (objectToSourceAndDestroy != null)
                {
                    try
                    {
                        FixForLogic(objectToModifyAndActivate, objectToSourceAndDestroy);
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"{objectToModifyAndActivate}: Fatal error fixing object for logic -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
                    }
                }
            }

            //Activate and The Replacement or Loaded Object and Remove The Original============================
            objectToModifyAndActivate.FinalizeReplacement(objectToSourceAndDestroy);

            if (VERBOSE_LOGGING)
            {
                Dev.Log("[][][]===========================================================================[][][]");
            }
            return objectToModifyAndActivate;
        }

        private GameObject TryGetReplaceObject(GameObject loadedObjectToProcess, List<PrefabObject> validReplacements)
        {
            GameObject replacementObject;
            RNG rng = new RNG(EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
            try
            {
                //give the logics a chance to configure the rng
                rng = GetRNG(loadedObjectToProcess, rng);
            }
            catch (Exception e)
            {
                Dev.LogError($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Error configuring the random number generator -- default will be used -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

            replacementObject = null;
            try
            {
                if (GetObjectReplacement(loadedObjectToProcess, validReplacements, rng, out replacementObject))
                {
                    if (VERBOSE_LOGGING)
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Object will be replaced by {replacementObject} .");
                }
                else
                {
                    if (VERBOSE_LOGGING)
                        Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Object was not replaced by any module.");
                }

            }
            catch (Exception e)
            {
                Dev.LogError($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Fatal error replacing object -- ERROR:{e.Message} STACKTRACE:{e.StackTrace}]");
            }

            return replacementObject;
        }

        protected bool WillProcessObject(GameObject objectToProcess)
        {
            bool canProcess = CanProcessNow(objectToProcess);

            if (!canProcess && EnemyRandomizer.DoReplacementBypassCheck(true))
            {
                if (VERBOSE_LOGGING)
                    Dev.LogWarning($"{objectToProcess}: Replacement bypass invoked, forcing this object to be process-able...");
                return true;
            }

            if (!canProcess)
            {
                if (objectToProcess.IsInvalidSceneObject())
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.Log($"[{objectToProcess}]: Destroying invalid object.");
                    }

                    SpawnerExtensions.DestroyObject(objectToProcess);
                    return false;
                }

                if (objectToProcess.IsTemporarilyInactive())
                {
                    if (VERBOSE_LOGGING)
                    {
                        if(objectToProcess.IsInALoadingScene())
                        {
                            Dev.Log($"[{objectToProcess}]: This object is in a scene that is not yet loaded {objectToProcess.SceneName()}.");
                        }

                        if(!objectToProcess.IsVisible())
                        {
                            Dev.Log($"[{objectToProcess}]: This object is not visible yet.");
                        }

                        Dev.Log($"[{objectToProcess}]: Cannot process object yet. Queuing for activation later.");
                    }

                    var loader = OnObjectLoadedAndActive(objectToProcess);
                    GameManager.instance.StartCoroutine(loader);
                    pendingLoads.Add(objectToProcess, loader);

                    //going to return original -- can't process the object yet
                }
                else
                {
                    if (VERBOSE_LOGGING)
                    {
                        Dev.LogWarning($"[{objectToProcess}]: This object will not be processed at all.");

                        if (VERBOSE_LOGGING && objectToProcess == null)
                        {
                            Dev.LogWarning($"[{objectToProcess}]: objectToProcess is null.");
                        }

                        if (VERBOSE_LOGGING && !objectToProcess.IsInAValidScene())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: NOT InAValidScene {objectToProcess.SceneName()}.");
                        }

                        if (VERBOSE_LOGGING && objectToProcess.IsInALoadingScene())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: IsInALoadingScene {objectToProcess.SceneName()}.");
                        }

                        if (VERBOSE_LOGGING && !objectToProcess.IsDatabaseObject())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: NOT a DatabaseObject.");
                        }

                        if (VERBOSE_LOGGING && objectToProcess.HasReplacedAnObject())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: This has replaced an object already");
                        }

                        if (VERBOSE_LOGGING && objectToProcess.IsDisabledBySavedGameState())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: IsDisabledBySavedGameState.");
                        }

                        if (VERBOSE_LOGGING && objectToProcess.IsVisible())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: Is Visible.");
                        }

                        if (VERBOSE_LOGGING && !objectToProcess.IsVisible())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: NOT Visible.");
                        }

                        if (VERBOSE_LOGGING && objectToProcess.IsInvalidSceneObject())
                        {
                            Dev.LogWarning($"[{objectToProcess}]: IsInvalidSceneObject.");
                        }

                        if (VERBOSE_LOGGING)
                        {
                            Dev.LogWarning($"[{objectToProcess}]: Will NOT process object.");
                            //objectToProcess.Dump();
                            Dev.LogError("TODO: implement new object dumping stats here....");
                        }
                    }
                    //else
                    //{
                    //    if (VERBOSE_LOGGING)
                    //        Dev.LogWarning($"[{objectToProcess}]: Will not process object.");
                    //}
                }
            }
            else
            {
                bool doesLogicAllowProcessing = true;
                foreach (var logic in loadedLogics)
                {
                    try
                    {
                        //TODO: rename CanProcessObject
                        doesLogicAllowProcessing = logic.CanReplaceObject(objectToProcess);
                        if (!doesLogicAllowProcessing)
                        {
                            if (VERBOSE_LOGGING)
                                Dev.LogWarning($"{objectToProcess}: The logic {logic} does not allow this to be processed.");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Error trying to check logic {logic.Name} to see if {objectToProcess.ObjectName()} can be replaced... ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                    }
                }
                canProcess = doesLogicAllowProcessing;
            }

            return canProcess;
        }

        protected IEnumerator OnObjectLoadedAndActive(GameObject loadedObjectToProcess)
        {
            string debugHeader = string.Empty;

            if(VERBOSE_LOGGING)
                debugHeader = $"{loadedObjectToProcess.SceneName()}, {loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}";

            yield return null;
            if(loadedObjectToProcess == null || loadedObjectToProcess.IsDisabledBySavedGameState())
            {
                if (VERBOSE_LOGGING && loadedObjectToProcess == null)
                    Dev.LogWarning($"The object is null.");

                if (VERBOSE_LOGGING && loadedObjectToProcess != null && loadedObjectToProcess.IsDisabledBySavedGameState())
                    Dev.LogWarning($"[{debugHeader}]: The object is disabled by game state.");
                yield break;
            }

            if (GetBlackBorders() == null || GetBlackBorders().Value == null || GetBlackBorders().Value.Count <= 0)
            {
                if (VERBOSE_LOGGING)
                    Dev.LogWarning($"[{debugHeader}]: Blocking load until the scene is finished loading (the black boarders have not yet been created).");
                yield return new WaitUntil(() => GetBlackBorders() != null && GetBlackBorders().Value != null && GetBlackBorders().Value.Count > 0);
                if (VERBOSE_LOGGING)
                    Dev.LogWarning($"[{debugHeader}]: Resuming load.");
            }

            if (loadedObjectToProcess != null && !loadedObjectToProcess.CanProcessObject())
            {
                if (VERBOSE_LOGGING)
                    Dev.LogWarning($"[{debugHeader}]: The object cannot be processed yet...");
                yield return new WaitUntil(() => loadedObjectToProcess == null || !loadedObjectToProcess.IsValid() || loadedObjectToProcess.CanProcessObject());
            }

            bool canProcessObject = loadedObjectToProcess != null && loadedObjectToProcess.CanProcessObject();

            if (VERBOSE_LOGGING && loadedObjectToProcess == null)
                Dev.LogWarning($"[{debugHeader}]: The object became null while in queue.");

            if (VERBOSE_LOGGING && loadedObjectToProcess != null && !loadedObjectToProcess.IsValid())
                Dev.LogWarning($"[{debugHeader}]: The object invalid.");

            if (VERBOSE_LOGGING && loadedObjectToProcess != null && loadedObjectToProcess.IsDisabledBySavedGameState())
                Dev.LogWarning($"[{debugHeader}]: The object became disabled by game state while in queue.");

            if (VERBOSE_LOGGING && loadedObjectToProcess != null && canProcessObject)
                Dev.LogWarning($"[{debugHeader}]: The object can now be processed.");

            if (VERBOSE_LOGGING && loadedObjectToProcess != null && !loadedObjectToProcess.IsInAValidScene())
                Dev.LogWarning($"[{debugHeader}]: The object's scene became invalid or unloaded.");

            if (loadedObjectToProcess == null || !canProcessObject)
            {
                if (VERBOSE_LOGGING && loadedObjectToProcess == null)
                    Dev.LogWarning($"[{debugHeader}]: The object became null while in queue and so CANNOT not be processed.");
                else if (VERBOSE_LOGGING && !canProcessObject)
                    Dev.LogWarning($"[{debugHeader}]: The object ultimately CANNOT be processed.");

                pendingLoads.Remove(loadedObjectToProcess);
                yield break;
            }

            DelayedProcess(loadedObjectToProcess);
        }

        protected virtual void DelayedProcess(GameObject loadedObjectToProcess)
        {
            if (loadedObjectToProcess != null)
            {
                try
                {
                    if (VERBOSE_LOGGING)
                        Dev.LogWarning($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Finally invoking the delayed call to OnObjectLoaded({loadedObjectToProcess})");
                    var result = OnObjectLoaded(loadedObjectToProcess);
                    if (VERBOSE_LOGGING)
                        Dev.LogWarning($"[{loadedObjectToProcess}]: Object processing complete and generated {result} removing from pending load...");
                }
                catch (Exception e)
                {
                    Dev.LogError($"Caught exception in DelayedProcess of {loadedObjectToProcess} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            pendingLoads.Remove(loadedObjectToProcess);
        }

        protected bool CanProcessNow(GameObject objectToProcess)
        {
            if (loadedLogics == null || loadedLogics.Count <= 0)
            {
                if(VERBOSE_LOGGING)
                    Dev.LogWarning($"No logics are loaded. Cannot process anything!");

                return false;
            }

            bool canProcessNow = true;
            try
            {
                if (!objectToProcess.CanProcessObject())
                {
                    if (VERBOSE_LOGGING)
                        Dev.LogWarning($"{objectToProcess}: objectToProcess.CanProcessObject == false : This object cannot be processed now.");
                    canProcessNow = false;
                }
            }
            catch (Exception e)
            {
                canProcessNow = false;

                if (VERBOSE_LOGGING && !objectToProcess.IsInvalidSceneObject() && objectToProcess.IsDatabaseObject())
                {
                    Dev.LogError($"Error trying to check if this object can be processed {objectToProcess} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");

                    Dev.LogError("TODO: here's where we'd dump a bunch of stats but..... this need to be rewritten- put that off until I hit this need");
                    //objectToProcess.Dump();
                }
            }

            return canProcessNow;
        }

        public List<PrefabObject> GetValidReplacements(GameObject loadedObjectToProcess)
        {
            var objectType = loadedObjectToProcess.ObjectType();
            List<PrefabObject> validReplacements = loadedObjectToProcess.GetObjectTypeCollection();

            foreach (var logic in loadedLogics.Where(x => x.WillFilterType(objectType)))
            {
                try
                {
                    validReplacements = logic.GetValidReplacements(loadedObjectToProcess, validReplacements);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to load valid replacements in logic {logic.Name} using data from {loadedObjectToProcess.ObjectName()} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }

            return validReplacements;
        }

        protected RNG GetRNG(GameObject original, RNG defaultRNG)
        {
            RNG rng = defaultRNG;

            foreach (var logic in loadedLogics.Where(x => x.WillModifyRNG()))
            {
                try
                {
                    rng = logic.GetRNG(original, rng, EnemyRandomizer.PlayerSettings.enemyRandomizerSeed);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Error trying to load RNG in logic {logic.Name} using data from {original.ObjectName()} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            return rng;
        }

        protected bool GetObjectReplacement(GameObject loadedObjectToProcess, List<PrefabObject> validReplacements, RNG rng, out GameObject newObject)
        {
            var objectType = loadedObjectToProcess.ObjectType();
            GameObject currentPotentialReplacement = null;
            foreach (var logic in loadedLogics.Where(x => x.WillReplaceType(objectType)))
            {
                try
                {
                    currentPotentialReplacement = logic.GetReplacement(currentPotentialReplacement, loadedObjectToProcess, validReplacements, rng);
                }
                catch (Exception e)
                {
                    Dev.LogError($"[GetObjectReplacement ERROR]: Error trying to replace object in logic [{logic.Name}] using data from {loadedObjectToProcess} ; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
            newObject = currentPotentialReplacement;
            return newObject != null;
        }

        protected void ModifyObject(GameObject objectToModify, GameObject objectToReplace)
        {
            var objectType = objectToModify.ObjectType();
            foreach (var logic in loadedLogics.Where(x => x.WillModifyType(objectType)))
            {
                try
                {
                    logic.ModifyObject(objectToModify, objectToReplace);
                }
                catch (Exception e)
                {
                    Dev.LogError($"[ModifyObject ERROR]: Error trying to modify object {objectToModify} in logic [{logic.Name}] using data from {objectToReplace} \nERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Check for a special case for this object to prevent logical softlocks or other unique reasons
        /// </summary>
        protected bool SkipForLogic(GameObject loadedObjectToProcess)
        {
            bool result = false;
            try
            {
                result = loadedObjectToProcess.SkipForLogic();
            }
            catch (Exception e)
            {
                Dev.LogError($"Error trying to check object for skip logic: {loadedObjectToProcess}; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }
            if(result)
            {
                if (VERBOSE_LOGGING)
                {
                    Dev.Log($"[{loadedObjectToProcess.ObjectType()}, {loadedObjectToProcess.GetSceneHierarchyPath()}]: Logic skip set. Will NOT attempt to replace object.");
                }
            }
            return result;
        }

        /// <summary>
        /// Apply special fixes to this enemy to prevent logical softlocks or other critical issues
        /// </summary>
        protected void FixForLogic(GameObject newReplacementObject, GameObject oldOriginalObject)
        {
            try
            {
                newReplacementObject.FixForLogic(oldOriginalObject);
            }
            catch (Exception e)
            {
                Dev.LogError($"Error trying to fix object {newReplacementObject} which would replace {oldOriginalObject} for softlock prevention logic; ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
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




//HasReplacedAnObject now does this check, so it's redundant
//if(ObjectMetadata.Get(loadedObjectToProcess) != null && ObjectMetadata.GetOriginal(loadedObjectToProcess) != null)
//{
//    if (loadedObjectToProcess.name.Contains("]["))
//    {
//        Dev.LogError($"[{ObjectMetadata.Get(loadedObjectToProcess)}]: THIS WAS RANDOMIZED AND SHOULD NOT BE PROCESSED AGAIN!!! DUMPING ALL INFO");
//        ObjectMetadata.Get(loadedObjectToProcess).Dump();
//        throw new InvalidOperationException($"[{ObjectMetadata.Get(loadedObjectToProcess)},{ObjectMetadata.GetOriginal(loadedObjectToProcess)}] THIS WAS RANDOMIZED AND SHOULD NOT BE PROCESSED AGAIN!!!");
//    }
//}

//var thisMetaData = ObjectMetadata.Get(loadedObjectToProcess);
//var originalMetaData = ObjectMetadata.GetOriginal(loadedObjectToProcess);

//if (thisMetaData != null && originalMetaData != null)
//{
//    if (VERBOSE_LOGGING)
//    {
//        Dev.Log($"[{thisMetaData}]: Skipping enemy randomizer processing because this object has already replaced {originalMetaData}.");
//        Dev.Log("[][][]===========================================================================[][][]");
//    }

//    return loadedObjectToProcess;
//}