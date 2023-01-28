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
using UniRx;
using System;



/* 
 * FIX SPAWN ISSUE:
 * cyrstal laser bug still in ground
 * shrumal ogre in ground too?
 * all 6 FLY enemies didn't spawn from gruz mother (and didn't randomize) -- SOLVED: bosses were set to use the GG versions in the enemy rando port. need to fix that
 * PREVENT white palace fly from replacing arena enemies (and from being replaced in white palace by things that can be killed).....
 * EGG SAC -- needs to copy the item out of it onto the replacement
 * new enemies that drop items need them removed if their replacement didn't drop them
 * 
 * FIX BEHAVIOUR ISSUE:
 * Mawlek Turret: see if i can randomize the projectile velocity when placed in a smaller room so the spray gets around more
 * 
 */

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod, ITogglableMod, IGlobalSettings<EnemyRandomizerSettings>, ILocalSettings<EnemyRandomizerPlayerSettings>
    {
        public static EnemyRandomizer Instance
        {
            get
            {
                if (instance == null)
                    instance = new EnemyRandomizer();
                return instance;
            }
        }
        static EnemyRandomizer instance;

        //public static string ENEMY_RANDO_PREFIX = "RANDOSKIP";

        //DONT USE THIS FOR ENEMY REPLACEMENTS
        public static RNG pRNG = new RNG();

        static SmartRoutine debugInputRoutine = new SmartRoutine();
        static SmartRoutine noClipRoutine = new SmartRoutine();
        static string debugRecentHit = "";
        public UnityEngine.Bounds sceneBounds;
        List<GameObject> sceneBoundry = new List<GameObject>();
        //List<GameObject> battleControls = new List<GameObject>();
        bool disabledRoarEffectOnPlayer = false;

        //NOTE: call this from GetPreloadNames() because that's the first method to execute....
        public virtual void PreInitialize()
        {
            pRNG.Reset();

            //enable debugging
            Dev.Logger.GuiLoggingEnabled = true;
            DevLogger.Instance.ShowSlider();

            //the specific type here doesn't matter, as long as it's from the same assembly as enemy types
            //we want to register
            RandomizerEnemyFactory.RegisterValidTypesInAssembly(typeof(EnemyRandomizer));

            //load data from XML
            LoadEnemyData();
            LoadArenaData();
            LoadExclusionData();

            RegisterCallbacks();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (instance == null)
                instance = this;

            base.Initialize(preloadedObjects);

            //local helper function
            GameObject GetPrefabObject(EnemyData data)
            {
                string sceneName = data.sceneName;
                string scenePath = data.gameObjectPath;

                if (!preloadedObjects.TryGetValue(sceneName, out var sceneObjects))
                    return null;

                if (!sceneObjects.TryGetValue(scenePath, out GameObject prefabObject))
                    return null;

                return prefabObject;
            }

            //hide the debug window
            var logHider = Observable.Timer(TimeSpan.FromSeconds(2f)).DoOnCompleted(() =>
            {
                DevLogger.Instance.Show(false);
                DevLogger.Instance.Show(false);
            }).Subscribe();

            //load the enemy prefabs
            try
            {
                //load the enemy game objects
                randomizerEnemies.enemyData.ForEach(thisEnemy =>
                {
                    //does the loaded enemy data contain a configuration type name and is it valid?
                    //if not, use the default loader
                    //else, use the custom loader
                    thisEnemy.loadedEnemy = (string.IsNullOrEmpty(thisEnemy.configurationTypeName) || !RandomizerEnemyFactory.IsValid(thisEnemy.configurationTypeName)) ?
                            RandomizerEnemyFactory.Create<DefaultEnemy>(thisEnemy, randomizerEnemies.enemyData, GetPrefabObject(thisEnemy)) :
                            RandomizerEnemyFactory.Create(thisEnemy.configurationTypeName, thisEnemy, randomizerEnemies.enemyData, GetPrefabObject(thisEnemy));
                });
            }
            catch (Exception e)
            {
                logHider.Dispose();
                //DevLogger.Instance.Show(true);

                Dev.LogError("Failed to preload enemies for allow for randomization. Error: " + e.Message);
                Dev.LogError("Stacktrace: " + e.StackTrace);
            }

        }


        void RegisterCallbacks()
        {
            ModHooks.AfterSavegameLoadHook -= MODHOOK_LoadFromSave;
            ModHooks.AfterSavegameLoadHook += MODHOOK_LoadFromSave;

            ModHooks.NewGameHook -= MODHOOK_LoadFromNewGame;
            ModHooks.NewGameHook += MODHOOK_LoadFromNewGame;

            ModHooks.OnEnableEnemyHook -= MODHOOK_RandomizeEnemyOnEnable;
            ModHooks.OnEnableEnemyHook += MODHOOK_RandomizeEnemyOnEnable;

            On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);
            On.UIManager.UIClosePauseMenu += new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.DrawBlackBordersHook -= OnSceneBoardersCreated;
            ModHooks.DrawBlackBordersHook += OnSceneBoardersCreated;

            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
            ModHooks.SlashHitHook += DebugPrintObjectOnHit;
        }

        void UnRegisterCallbacks()
        {
            ModHooks.AfterSavegameLoadHook -= MODHOOK_LoadFromSave;

            ModHooks.NewGameHook -= MODHOOK_LoadFromNewGame;

            ModHooks.OnEnableEnemyHook -= MODHOOK_RandomizeEnemyOnEnable;

            On.UIManager.UIClosePauseMenu -= new On.UIManager.hook_UIClosePauseMenu(SetNoClip);

            ModHooks.DrawBlackBordersHook -= OnSceneBoardersCreated;

            ModHooks.SlashHitHook -= DebugPrintObjectOnHit;
        }

        ///Revert all changes the mod has made
        public void Unload()
        {
            UnRegisterCallbacks();
        }

        void MODHOOK_LoadFromSave(SaveGameData data)
        {
            disabledRoarEffectOnPlayer = false;

            //TODO: setup the seed
        }

        //Call from New Game
        void MODHOOK_LoadFromNewGame()
        {
            disabledRoarEffectOnPlayer = false;

            //TODO: setup the seed
        }

        public bool MODHOOK_RandomizeEnemyOnEnable(GameObject oldEnemy, bool isAlreadyDead)
        {
            if (isAlreadyDead)
                return isAlreadyDead;

            if (oldEnemy.IsRandomizerEnemy())
                return isAlreadyDead;

            if (oldEnemy.IsVisible())
                ReplaceEnemy(oldEnemy);
            else
                oldEnemy.AddComponent<RandomizeWhenVisible>();

            return isAlreadyDead;
        }


        /// <summary>
        /// Returns the objects to preload in order for the mod to work.
        /// </summary>
        /// <returns>A List of tuples containing scene name, object name</returns>
        public override List<(string, string)> GetPreloadNames()
        {
            PreInitialize();
            return GetPreloadDataFromXML(randomizerEnemies);
        }

        /// <summary>
        /// Route base class override here to allow for our return type to contain a more descriptive return tuple
        /// </summary>
        public static List<(string SceneName, string GameObjectPath)> GetPreloadDataFromXML(ISceneDataProvider sceneDataProvider)
        {
            return sceneDataProvider.GetSceneDataList();
        }

        ////NOTE: Executes AFTER get preload names
        //public override (string, Func<IEnumerator>)[] PreloadSceneHooks()
        //{
        //    return base.PreloadSceneHooks();
        //}

        public static void SetDebugInput(bool enabled)
        {
            if (enabled)
                debugInputRoutine = new SmartRoutine(DebugInput());
            else
                debugInputRoutine.Reset();
        }

        static void DebugPrintObjectOnHit(Collider2D otherCollider, GameObject gameObject)
        {
            //Dev.Where();
            if (otherCollider.gameObject.name != debugRecentHit)
            {
                Dev.Log("Hero at " + HeroController.instance.transform.position + " HIT: " + otherCollider.gameObject.name + " at (" + otherCollider.gameObject.transform.position + ")");
                debugRecentHit = otherCollider.gameObject.name;
            }
        }

        static void SetNoClip(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        {
            orig.Invoke(self);
            bool noClip = EnemyRandomizer.GlobalSettings.NoClip;
            if (!noClip)
                noClipRoutine.Reset();
            else
                noClipRoutine = new SmartRoutine(DoNoClip());
        }

        static IEnumerator DoNoClip()
        {
            Vector3 noclipPos = HeroController.instance.gameObject.transform.position;
            while (EnemyRandomizer.GlobalSettings.NoClip)
            {
                yield return null;

                if (HeroController.instance == null || HeroController.instance.gameObject == null || !HeroController.instance.gameObject.activeInHierarchy)
                    continue;

                if (EnemyRandomizer.GlobalSettings.NoClip)
                {
                    if (GameManager.instance.inputHandler.inputActions.left.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x - Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.right.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x + Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.up.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x, noclipPos.y + Time.deltaTime * 20f, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.down.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x, noclipPos.y - Time.deltaTime * 20f, noclipPos.z);
                    }

                    if (HeroController.instance.transitionState.ToString() == "WAITING_TO_TRANSITION")
                    {
                        HeroController.instance.gameObject.transform.position = noclipPos;
                    }
                    else
                    {
                        noclipPos = HeroController.instance.gameObject.transform.position;
                    }
                }
            }
        }

        static IEnumerator DebugInput()
        {
            for (; ; )
            {
                yield return new WaitForEndOfFrame();
                if (UnityEngine.Input.GetKeyDown(KeyCode.O))
                {
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                    {
                        Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        bool status = s.IsValid();
                        if (status)
                        {
                            string outputPath = Application.dataPath + "/Managed/Mods/EnemyRandomizer/" + s.name;
                            Dev.Log("Dumping Loaded Scene to " + outputPath);
                            s.PrintHierarchy(outputPath, true);
                        }
                    }
                }
            }

            yield break;
        }


        public bool IsInBounds(GameObject target)
        {
            return (sceneBounds.Contains(target.transform.position));
        }

        public void OnSceneBoardersCreated(List<GameObject> borders)
        {
            if (GameManager.instance.IsMenuScene())
                return;

            //TODO: have this reset when reloading a save
            if (!disabledRoarEffectOnPlayer)
            {
                var roarfsm = HeroController.instance.GetComponentsInChildren<PlayMakerFSM>(true).Where(x => x.FsmName.Contains("Roar Lock")).First();
                roarfsm.ChangeTransition("Roar Allowed?", "FINISHED", "Regain Control");
                disabledRoarEffectOnPlayer = true;
            }

            sceneBoundry.Clear();
            sceneBoundry.AddRange(borders);

            List<GameObject> xList = sceneBoundry.Select(x => x).OrderBy(x => x.transform.position.x).ToList();
            List<GameObject> yList = sceneBoundry.Select(x => x).OrderBy(x => x.transform.position.y).ToList();

            sceneBounds = new UnityEngine.Bounds
            {
                min = new Vector3(xList[0].transform.position.x, yList[0].transform.position.y, -10f),
                max = new Vector3(xList[xList.Count - 1].transform.position.x, yList[yList.Count - 1].transform.position.y, 10f)
            };
        }

        public static GameObject DebugSpawnEnemy(string enemyName, bool forcePlaceOnGround = false)
        {
            try
            {
                if (EnemyRandomizer.Instance.EnemyDataMap.TryGetValue(enemyName, out EnemyData data))
                {
                    var pos = HeroController.instance.transform.position + Vector3.right * 5f;
                    var enemy = data.loadedEnemy.Instantiate();
                    if(forcePlaceOnGround || data.loadedEnemy.IsFlyer)
                        pos = Mathnv.GetPointOn(pos, Vector2.down, 500f, EnemyRandomizer.IsSurfaceOrPlatform);                    
                    enemy.SetupRandomizerComponents(data.loadedEnemy, null, null);
                    enemy.transform.position = pos;
                    enemy.SetActive(true);
                    return enemy;
                }
            }
            catch(Exception e)
            {
                Dev.LogError("Error: " + e.Message);
            }

            return null;
        }

        public static GameObject DebugReplaceEnemy(string enemyName, GameObject enemyToReplace)
        {
            try 
            {
                if (EnemyRandomizer.Instance.EnemyDataMap.TryGetValue(enemyName, out EnemyData data))
                {
                    string trimmedName = enemyToReplace.name.TrimGameObjectName();
                    if (EnemyRandomizer.Instance.EnemyDataMap.TryGetValue(trimmedName, out EnemyData replaceData))
                    {
                        enemyToReplace.SetActive(false);
                        var enemy = data.loadedEnemy.Instantiate();
                        enemy.SetupRandomizerComponents(data.loadedEnemy, enemyToReplace, replaceData);
                        enemy.SetActive(true);
                        return enemy;
                    }
                }
            }
            catch(Exception e)
            {
                Dev.LogError("Error: " + e.Message);
            }

            return null;
        }

        public static void DebugSpawnThenReplaceEnemy(string spawnName, string replaceName)
        {
            try
            { 
                DebugReplaceEnemy(replaceName, DebugSpawnEnemy(spawnName));
            }
            catch(Exception e)
            {
                Dev.LogError("Error: " + e.Message);
            }
        }

        //TODO: load these from an xml file
        public static bool IsSurfaceOrPlatform(GameObject gameObject)
        {
            //First process skips or exclusions
            List<string> groundOrPlatformName = new List<string>()
            {
                "Chunk",
                "Platform",
                "plat_",
                "Roof"
            };

            return groundOrPlatformName.Any(x => gameObject.name.Contains(x));
        }
    }

    public class RandomizeWhenVisible : MonoBehaviour
    {
        IEnumerator Start()
        {
            Dev.Log("Waiting to randomize " + gameObject);
            if (gameObject == null)
                yield break;

            Collider2D collider = gameObject.GetComponent<Collider2D>();
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (collider == null && renderer == null)
                yield break;

            yield return new WaitUntil(() => {
                if (collider != null && renderer == null)
                    return collider.enabled;
                else if (collider == null && renderer != null)
                    return renderer.enabled;
                else //if (collider != null && renderer != null)
                    return collider.enabled && renderer.enabled;
            });

            Dev.Log("Randomizng newly activated " + gameObject);
            EnemyRandomizer.Instance.ReplaceEnemy(gameObject);
            GameObject.Destroy(this);
        }
    }


    //EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Crawler");
    //EnemyRandomizerMod.EnemyRandomizer.DebugSpawnThenReplaceEnemy("Crawler","Roller");

}
