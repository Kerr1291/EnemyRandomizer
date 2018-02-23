using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;


/*
 * Top TODOs:
 * 
 * NEXT: make enemies randomized selection be based on a seed so it's always the same
 * 
 * try something with this to kill enemies
            //FSMUtility.LocateFSM( enemy, "health_manager_enemy" ).SetState( "Decrement Health" );
 * 
 * */

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : Mod<EnemyRandomizerSettings>, ITogglableMod
    {
        bool randomizerReady = false;

        public static EnemyRandomizer Instance { get; private set; }

        string recentHit = "";
        string fullVersionName = "0.0.1";

        Dictionary<string, List<string>> enemyTypes = new Dictionary<string, List<string>>();
        List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        List<string> loadedEnemyPrefabNames = new List<string>();
        List<string> uniqueEnemyTypes = new List<string>();

        GameObject modRoot;
        GameObject ModRoot {
            get {
                if( modRoot == null )
                {
                    modRoot = new GameObject( "RandoRoot" );
                    GameObject.DontDestroyOnLoad( modRoot );
                }
                return modRoot;
            }
            set {
                if( modRoot != null && value != modRoot )
                {
                    GameObject.Destroy( modRoot );
                }
                modRoot = value;
            }
        }

        void EnableEnemyRandomizerFromSave( int id )
        {
            EnableEnemyRandomizer();
        }

        void EnableEnemyRandomizer()
        {
            if( databaseGenerated )
                randomizerReady = true;
        }

        public override void Initialize()
        {
            if(Instance != null)
            {
                Log("Warning: EnemyRandomizer is a singleton. Trying to create more than one may cause issues!");
                return;
            }

            Instance = this;

            Log("Enemy Randomizer Mod initializing!");

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ToggleBuildRandoDatabaseUI;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartRandomEnemyLocator;
            

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.ColliderCreateHook += OnLoadObjectCollider;

            ModHooks.Instance.SavegameLoadHook -= EnableEnemyRandomizerFromSave;
            ModHooks.Instance.SavegameLoadHook += EnableEnemyRandomizerFromSave;

            ModHooks.Instance.NewGameHook -= EnableEnemyRandomizer;
            ModHooks.Instance.NewGameHook += EnableEnemyRandomizer;

            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;
            ModHooks.Instance.SlashHitHook += Debug_PrintObjectOnHit;

            //ModHooks.Instance.DashPressedHook += StartLoad; //used for testing
        }

        public void Unload()
        {
            Instance = null;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ToggleBuildRandoDatabaseUI;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.SavegameLoadHook -= EnableEnemyRandomizerFromSave;
            ModHooks.Instance.NewGameHook -= EnableEnemyRandomizer;
            ModHooks.Instance.SlashHitHook -= Debug_PrintObjectOnHit;

            ModRoot = null;

            if( menu != null )
            {
                GameObject.Destroy( menu );
            }

            Restore();
        }

        //TODO: split the restore function into functions for each part
        void Restore()
        {
            currentDatabaseIndex = 0;
            menu = null;
            databaseGenerated = false;
            isLoadingDatabase = false;
            loadCount = 0;
            enemyTypes = new Dictionary<string, List<string>>();
            loadedEnemyPrefabs = new List<GameObject>();
            loadedEnemyPrefabNames = new List<string>();
            uniqueEnemyTypes = new List<string>();
            databaseLoader = new nv.Contractor();
            loadingBar = null;
            randomizerReady = false;
            randomEnemyLocator = new nv.Contractor();
            databaseLoadProgress = 0f;
            recentHit = "";
        }

        public override string GetVersion()
        {
            if( fullVersionName.Length < 2 )
            {
                //try
                //{
                //    GithubVersionHelper helper = new GithubVersionHelper("Kerr1291/EnemyRandomizer");
                //    fullVersionName = "0.0.1 (Github version: " + helper.GetVersion() + ")";
                //}
                //catch(Exception e)
                //{
                //    fullVersionName = "0.0.1";
                //}
            }
            return fullVersionName;
        }

        public override bool IsCurrent()
        {
            try
            {
                //GithubVersionHelper helper = null;
                //try
                //{
                //    helper = new GithubVersionHelper("Kerr1291/EnemyRandomizer");
                //    Log( "Github = " + helper.GetVersion() );
                //}
                //catch( Exception e )
                //{
                //    helper = null;
                //}

                return true;
                //return helper == null || GetVersion().StartsWith( helper.GetVersion() );
            }
            catch( Exception )
            {
                return true;
            }
        }

    }
}
