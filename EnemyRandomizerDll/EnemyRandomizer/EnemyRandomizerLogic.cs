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
using System.Reflection;

using nv;

namespace EnemyRandomizerMod
{
    //TODO: clean this up.... a lot
    public class EnemyRandomizerLogic
    {
        public static EnemyRandomizerLogic Instance { get; private set; }

        //TODO: check tomorrow
        public const bool RUN_DEBUG_INPUT = false;
        IEnumerator debugInput = null;

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        RNG replacementRNG;

        string currentScene = "";
        string randoEnemyNamePrefix = "Rando Enemy: ";
        nv.Contractor randomEnemyLocator = new nv.Contractor();
        IEnumerator randomizerReplacer = null;

        nv.Contractor replacementController = new nv.Contractor();

        class ReplacementPair
        {
            public GameObject original;
            public GameObject replacement;
        }

        List<ReplacementPair> replacements = new List<ReplacementPair>();

        List<GameObject> battleControls = new List<GameObject>();

        //For debugging, the scene replacer will run its logic without doing anything
        //(Useful for testing without needing to wait through the load times)
        //This is set to true if the game is started without loading the database
        bool simulateReplacement = false;

        float baseRestartDelay = 1f;
        float nextRestartDelay = 1f;
        float restartDelay = 0f;

        List<ReplacementPair> pairsToRemove = new List<ReplacementPair>();

        public EnemyRandomizerLogic( EnemyRandomizerDatabase database )
        {
            this.database = database;
        }

        IEnumerator DebugInput()
        {
            bool printed = false;
            while( true )
            {
                yield return new WaitForEndOfFrame();
                if( !printed && UnityEngine.Input.GetKeyDown( KeyCode.T ) )
                {
                    printed = true;
                    Dev.Log( "=====================================" );
                    Dev.Log( "=====================================" );
                    Dev.Log( "=====================================" );
                    Dev.Log( "Dumping Scene" );
                    for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; )
                    {
                        Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        bool status = s.IsValid();
                        if(status)
                            s.PrintHierarchy( i, null, EnemyRandomizerDatabase.enemyTypeNames );
                    }
                    Dev.Log( "=====================================" );
                }
                if( !printed && UnityEngine.Input.GetKeyDown( KeyCode.Y ) )
                {
                    printed = true;
                    Dev.Log( "=====================================" );
                    Dev.Log( "=====================================" );
                    Dev.Log( "=====================================" );
                    Dev.Log( "Dumping FSM Objects" );
                    foreach( var fsm in GameObject.FindObjectsOfType<PlayMakerFSM>() )
                    {
                        fsm.gameObject.PrintSceneHierarchyTree( true );
                    }
                    Dev.Log( "=====================================" );
                }
                if( printed && UnityEngine.Input.GetKeyDown( KeyCode.U ) )
                {
                    printed = false;
                }
            }

            debugInput = null;
            yield break;
        }

        public void Setup( bool simulateReplacement )
        {
            Dev.Where();
            Instance = this;
            comms = new CommunicationNode();
            comms.EnableNode( this );

            if( RUN_DEBUG_INPUT )
            {
                debugInput = DebugInput();
                ContractorManager.Instance.StartCoroutine( debugInput );
            }

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.ColliderCreateHook += OnLoadObjectCollider;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += StartRandomEnemyLocator;
        }

        public void Unload()
        {
            Dev.Where();

            if( RUN_DEBUG_INPUT )
            {
                ContractorManager.Instance.StopCoroutine( debugInput );
                debugInput = null;
            }

            if( randomEnemyLocator != null )
                randomEnemyLocator.Reset();
            randomEnemyLocator = new nv.Contractor();

            replacementController.Reset();

            comms.DisableNode();
            Instance = null;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
        }

        void CheckAndAddBattleControls( GameObject go )
        {
            if( battleControls.Contains( go ) )
                return;

            if( FSMUtility.ContainsFSM( go, "Battle Control" ) )
            {
                Dev.Log( "Found battle control on object: " + go.name );
                battleControls.Add( go );
            }

            //foreach( Component c in go.GetComponents<Component>() )
            //{
            //    PlayMakerFSM pfsm = c as PlayMakerFSM;
            //    if(pfsm.FsmName == "Battle Control")
            //    {
            //        Dev.Log( "Found battle control on object: " + go.name );
            //        battleControls.Add( go );
            //    }
            //}
        }

        void CheckAndDisableSoftlockGates( GameObject go )
        {
            if( go.activeInHierarchy == false )
                return;

            if( FSMUtility.ContainsFSM( go, "BG Control" ) )
            {
                Dev.Log( "Found battle gate control on object: " + go.name );
                go.SetActive( false );
            }
        }

        void UpdateBattleControls()
        {
            //Dev.Log( "A" );
            for( int i = 0; i < battleControls.Count; )
            {
                //Dev.Log( "B" );
                //remove any controls that go null (like from a scene change)
                if( battleControls[ i ] == null )
                {
                    battleControls.RemoveAt( i );
                    i = 0;
                    continue;
                }

                //Dev.Log( "C" );
                PersistentBoolItem pBoolItem = battleControls[i].GetComponent<PersistentBoolItem>();

                //Dev.Log( "D" );
                //does this battle control have a persistent bool? then we want to make sure it's not set
                if( pBoolItem != null && pBoolItem.persistentBoolData != null )
                {
                    //Dev.Log( "pBoolItem.persistentBoolData.activated " + pBoolItem.persistentBoolData.activated);

                    //ignore battle controls that have been completed and remove them from the list
                    if( pBoolItem.persistentBoolData.activated )
                    {
                        battleControls.RemoveAt( i );
                        i = 0;
                        continue;
                    }
                }

                //average screen size
                //20 width
                //12 high

                //Dev.Log( "E" );
                //ok, so the battle control hasn't been run or completed yet, we need to manually monitor it
                BoxCollider2D collider = battleControls[i].GetComponent<BoxCollider2D>();
                Bounds localBounds;



                //Dev.Log( "F" );
                if( collider == null )
                {
                    //Dev.Log( "Creating out own bounds to test" );
                    localBounds = new Bounds( battleControls[ i ].transform.position, new Vector3( 28f, 24f, 10f ) );
                }
                else
                {
                    //Dev.Log( "Using provided bounds..." );
                    localBounds = collider.bounds;
                }

                //Dev.Log( "G" );

                //Dev.Log( "H" );
                //add some Z size to the bounds

                float width = Mathf.Max(28f,localBounds.extents.x);
                float height = Mathf.Max(24f,localBounds.extents.y);

                localBounds.extents = new Vector3( width, height, 10f );

                //Dev.Log( "I" );
                Vector3 heroPos = HeroController.instance.transform.position;

                //Dev.Log( "J" );

                //TODO: test!!!
                //is the hero in the battle scene? if not, no point in checking things
                if( !localBounds.Contains( heroPos ) )
                {
                    Dev.Log( "Hero outside the bounds of our battle control, don't monitor" );
                    Dev.Log( "Hero: " + heroPos );
                    Dev.Log( "Bounds Center: " + localBounds.center );
                    Dev.Log( "Bounds Extents: " + localBounds.extents );
                    //DebugPrintObjectTree( battleControls[ i ], true );
                }
                else
                {
                    //Dev.Log( "K" );
                    //see if any rando enemies are inside the area, if they are, we don't set next
                    bool setNext = true;
                    //Dev.Log( "L" );
                    foreach( var pair in replacements )
                    {
                        //Dev.Log( "M" );
                        if( pair.replacement == null )
                            continue;

                        bool isColosseum = false;
                        for( int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; ++j )
                        {
                            //iterate over the loaded game objects
                            if( UnityEngine.SceneManagement.SceneManager.GetSceneAt( j ).IsValid() )
                                isColosseum = UnityEngine.SceneManagement.SceneManager.GetSceneAt( j ).name.Contains( "Colosseum" );
                            if( isColosseum )
                                break;
                        }

                        //kill enemies that escape the coloseeum
                        if( isColosseum && !localBounds.Contains( pair.replacement.transform.position ) )
                        {
                            if( pair.replacement.GetEnemyFSM() != null )
                                pair.replacement.GetEnemyFSM().SendEvent( "INSTA KILL" );
                        }

                        //Dev.Log( "N" );
                        if( localBounds.Contains( pair.replacement.transform.position ) )
                        {
                            setNext = false;
                            break;
                        }
                    }

                    //TODO: special giant fly logic? gruz mother
                    //GameObject giantFly = GameObject.Find("Giant Fly");

                    //if(giantFly != null)
                    //{

                    //}

                    //Dev.Log( "O" );


                    //for( int j = 0; j < UnityEngine.SceneManagement.SceneManager.sceneCount; ++j )
                    //{
                    //    //iterate over the loaded game objects
                    //    UnityEngine.SceneManagement.SceneManager.GetSceneAt(j).PrintHierarchy(j,localBounds,EnemyRandomizerDatabase.enemyTypeNames);

                    //}

                    if( setNext )
                    {
                        //Dev.Log( "Sending NEXT notification to battle gates!" );
                        //Dev.Log( "Removing battle gates! " + battleControls[ i ].name );
                        //get the battle control
                        //Dev.Log( "Q" );
                        //GameObject b = battleControls[ i ].FindGameObjectInChildren( "Battle Gate" );
                        //while( b != null )
                        //{
                        //    b = null;
                        //    b = battleControls[ i ].FindGameObjectNameContainsInChildren( "Battle Gate" );
                        //    if( b != null )
                        //    {
                        //        b.name = "DELETED";
                        //        GameObject.DestroyImmediate( b );
                        //    }
                        //}
                        //continue;
                        PlayMakerFSM pfsm = FSMUtility.LocateFSM( battleControls[i], "Battle Control" );

                        //this has an unintended side-effect of causing colosseum 3 to be rushed through.... and it's very fun (and crazy)
                        //so even if i change to another fix for this later, will keep this behavior around for that reason
                        if( pfsm != null )
                        {
                            pfsm.SendEvent( "NEXT" );
                        }
                        //continue;
                    }
                }

                ++i;
            }
        }

        //entry point into the replacement logic, started on each scene transition
        void StartRandomEnemyLocator( Scene from, Scene to )
        {
            //"disable" the randomizer when we enter the title screen, it's enabled when a new game is started or a game is loaded
            if( to.name == Menu.RandomizerMenu.MainMenuSceneName )
                return;

            Dev.Log( "Transitioning FROM [" + from.name + "] TO [" + to.name + "]" );

            //ignore randomizing on the menu/movie intro scenes
            if( to.buildIndex < 4 )
                return;

            Dev.Where();
            replacements.Clear();
            pairsToRemove.Clear();

            replacementController.OnUpdate = ControlReplacementRoot;
            replacementController.Looping = true;
            replacementController.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
            replacementController.Start();

            currentScene = to.name;

            SetupRNGForScene( to.name );

            randomEnemyLocator.Reset();

            Dev.Log( "Starting the replacer which will search the scene for enemies and randomize them!" );
            randomizerReplacer = DoLocateAndRandomizeEnemies();

            restartDelay = 0f;

            //TODO: see if the performance cost here is still OK and refactor out this hack after some more testing
            nextRestartDelay = baseRestartDelay;

            randomEnemyLocator.OnUpdate = LocateAndRandomizeEnemies;
            randomEnemyLocator.Looping = true;
            randomEnemyLocator.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );

            randomEnemyLocator.Start();

            sceneBoundry.Clear();
            //printedScenees.Clear();

            battleControls.Clear();
        }

        void SetupRNGForScene( string scene )
        {
            if( replacementRNG == null )
            {
                //only really matters if chaosRNG is enabled...
                if( EnemyRandomizer.Instance.GameSeed >= 0 )
                    replacementRNG = new RNG( EnemyRandomizer.Instance.GameSeed );
                else
                    replacementRNG = new RNG();
            }
        }

        void LocateAndRandomizeEnemies()
        {
            //This should catch any weird behaviors during the replacement algorithm and prevent it from
            //breaking during a player's game.
            try
            {
                if( randomizerReplacer != null && !randomizerReplacer.MoveNext() )
                {
                    Dev.Log( "end of iterator or iterator became null" );
                    randomEnemyLocator.Reset();
                }

                if( randomizerReplacer != null && ( randomizerReplacer.Current as bool? ) == false )
                {
                    Dev.Log( "iterator returned false" );
                    randomizerReplacer = null;
                }
            }
            catch( Exception e )
            {
                Dev.Log( "Replacer hit an exception: " + e.Message );
                randomizerReplacer = null;
            }

            if( randomizerReplacer == null )
            {
                if( restartDelay <= 0 )
                {
                    restartDelay = nextRestartDelay;
                    nextRestartDelay = nextRestartDelay * 1f;

                    //restart iterator, every time it restarts, lets turn up the cooldown on restarting
                    randomizerReplacer = DoLocateAndRandomizeEnemies();
                }
                else
                {
                    restartDelay -= Time.deltaTime;
                }
            }
        }

        bool calculateBounds = true;
        Bounds sceneBounds;
        List<GameObject> sceneBoundry = new List<GameObject>();


        //List<int> printedScenees = new List<int>();

        IEnumerator DoLocateAndRandomizeEnemies()
        {
            //wait until all scenes are loaded
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; )
            {
                bool status = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isLoaded;
                if( !status )
                {
                    i = 0;
                    yield return null;
                }
                else
                {
                    ++i;
                }
            }

            //TODO: move into a precalculate scene function
            while( sceneBoundry.Count <= 0 )
            {
                calculateBounds = true;
                //iterate over the loaded scenes
                for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
                {
                    Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                    if( !loadedScene.IsValid() )
                        continue;

                    int buildIndex = loadedScene.buildIndex;

                    //if(!printedScenees.Contains(buildIndex))
                    //{
                    //    printedScenees.Add( buildIndex );
                    //    DebugPrintAllObjects( loadedScene.name, i );
                    //}

                    //iterate over the loaded game objects
                    GameObject[] rootGameObjects = loadedScene.GetRootGameObjects();

                    foreach( GameObject rootGameObject in rootGameObjects )
                    {
                        yield return true;
                        //and their children
                        if( rootGameObject == null )
                        {
                            Dev.Log( "Scene " + i + " has a null root game object! Skipping scene..." );
                            break;
                        }

                        if( rootGameObject.name == EnemyRandomizer.Instance.ModRoot.name )
                        {
                            continue;
                        }

                        int counter = 0;
                        foreach( Transform t in rootGameObject.GetComponentsInChildren<Transform>( true ) )
                        {
                            if( t.gameObject == null )
                                break;

                            if( sceneBoundry == null )
                                break;

                            if( t.gameObject.name.Contains( "SceneBorder" ) && !sceneBoundry.Contains( t.gameObject ) )
                                sceneBoundry.Add( t.gameObject );

                            //CheckAndDisableSoftlockGates( t.gameObject );
                            CheckAndAddBattleControls( t.gameObject );

                            counter++;
                            string name = t.gameObject.name;

                            if( counter % 500 == 0 )
                                yield return true;
                        }
                    }
                }
                yield return true;
            }

            if( calculateBounds )
            {
                List<GameObject> xList = sceneBoundry.Select(x=>x).OrderBy(x=>x.transform.position.x).ToList();
                List<GameObject> yList = sceneBoundry.Select(x=>x).OrderBy(x=>x.transform.position.y).ToList();

                sceneBounds = new Bounds
                {
                    min = new Vector3( xList[ 0 ].transform.position.x, yList[ 0 ].transform.position.y, -10f ),
                    max = new Vector3( xList[ xList.Count - 1 ].transform.position.x, yList[ yList.Count - 1 ].transform.position.y, 10f )
                };

                Dev.Log( "Bounds created with dimensions" );
                Dev.Log( "min " + sceneBounds.min );
                Dev.Log( "max " + sceneBounds.max );
                Dev.Log( "center " + sceneBounds.center );
                Dev.Log( "extents " + sceneBounds.extents );

                calculateBounds = false;
            }

            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                //iterate over the loaded game objects
                GameObject[] rootGameObjects = currentScene.GetRootGameObjects();

                foreach( GameObject rootGameObject in rootGameObjects )
                {
                    //and their children
                    if( rootGameObject == null )
                    {
                        Dev.Log( "Scene " + i + " has a null root game object! Skipping scene..." );
                        break;
                    }

                    //skip our mod root
                    if( rootGameObject.name == EnemyRandomizer.Instance.ModRoot.name )
                        continue;

                    int counter = 0;
                    foreach( Transform t in rootGameObject.GetComponentsInChildren<Transform>( true ) )
                    {
                        if( t.gameObject == null )
                            break;

                        counter++;
                        string name = t.gameObject.name;

                        //kill rando enemies that get outside the scene bounds
                        if( name.Contains( "Rando" ) && !name.Contains( "Replaced" ) )
                        {
                            if( !sceneBounds.Contains( t.position ) )
                            {
                                if( t.gameObject.GetEnemyFSM() != null )
                                    t.gameObject.GetEnemyFSM().SendEvent( "INSTA KILL" );
                            }
                            continue;
                        }

                        //TODO: test fix for weird behavior in shrumal ogre arena
                        if( name.Contains( "Cap Hit" ) )
                        {
                            GameObject.Destroy( t.gameObject );
                            continue;
                        }

                        //if( name.Contains( "Battle Gate" ) )
                        //{
                        //    GameObject.Destroy( t.gameObject );
                        //    continue;
                        //}

                        if( counter % 200 == 0 )
                            yield return true;

                        //don't replace null/destroyed game objects
                        if( t == null || t.gameObject == null )
                            continue;

                        if( !sceneBounds.Contains( t.position ) )
                        {
                            //Dev.Log( "Skipping " + t.gameObject.name + " Because it is outside the bounds. " + t.position );
                            continue;
                        }

                        //don't replace inactive game objects
                        if( !t.gameObject.activeInHierarchy )
                            continue;

                        if( name.IsSkipRandomizingString() )
                            continue;

                        //skip child components of randomized enemies
                        foreach( Transform p in t.GetComponentsInParent<Transform>( true ) )
                        {
                            if( p.name.Contains( "Rando" ) )
                                continue;
                        }

                        GameObject potentialEnemy = t.gameObject;

                        bool isRandoEnemy = potentialEnemy.IsRandomizerEnemy(database.loadedEnemyPrefabNames);

                        if( EnemyRandomizerDatabase.USE_TEST_SCENES )
                        {
                            isRandoEnemy = potentialEnemy.IsRandomizerEnemy( EnemyRandomizerDatabase.enemyTypeNames );
                        }

                        if( isRandoEnemy )
                            RandomizeEnemy( potentialEnemy );
                    }

                    yield return true;
                }
            }

            //Dev.Log( "Updating battle controls" );

            yield return null;

            UpdateBattleControls();

            //Dev.Log( "Printing list" );
            ////if(needsList)
            //{
            //    List<GameObject> xList = allObjs.Select(x=>x).OrderBy(x=>x.transform.position.x).ToList();
            //    List<GameObject> yList = allObjs.Select(x=>x).OrderBy(x=>x.transform.position.y).ToList();

            //    foreach(var g in xList)
            //    {
            //        Dev.Log( "X: " + g.transform.position.x + " :::: " + g.name);
            //    }

            //    foreach( var g in yList )
            //    {
            //        Dev.Log( "Y: " + g.transform.position.y + " :::: " + g.name );
            //    }

            //    needsList = false;
            //}

            randomizerReplacer = null;
            yield return false;
        }

        void OnLoadObjectCollider( GameObject potentialEnemy )
        {
            bool isRandoEnemy = potentialEnemy.IsRandomizerEnemy( database.loadedEnemyPrefabNames );

            if( EnemyRandomizerDatabase.USE_TEST_SCENES )
            {
                isRandoEnemy = potentialEnemy.IsRandomizerEnemy( EnemyRandomizerDatabase.enemyTypeNames );
            }

            if( isRandoEnemy )
                RandomizeEnemy( potentialEnemy );
        }

        void RandomizeEnemy( GameObject enemy )
        {
            //this failsafe is needed here in the case where we have exceptional things that should NOT be randomized
            if( enemy.name.IsSkipRandomizingString() )
            {
                //Dev.Log( "Exceptional case found in SkipRandomizeEnemy() -- Skipping randomization for: " + enemy.name );
                return;
            }

            //TODO: refactor this into a function
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if( !loadedScene.IsValid() )
                    continue;

                string sceneName = loadedScene.name;

                if( sceneName.Contains( "Colosseum" ) )
                {
                    //don't randomize these in the colosseum, will cause a softlock
                    if( enemy.name.Contains( "Zote Boss" ) )
                        return;
                    if( enemy.name.Contains( "Baby Centipede" ) )
                        return;
                    if( enemy.name.Contains( "Mantis Traitor Lord" ) )
                        return;
                }
            }

            Dev.Log( "Randomizing: " + enemy.name );

            if( simulateReplacement )
            {
                Dev.Log( "Sim mode enabled, not replacing the enemy, but we'll flag it like we did!" );
                enemy.name += " Rando simulated";
                replacements.Add( new ReplacementPair() { original = enemy, replacement = enemy } );
                return;
            }

            int randomReplacementIndex = 0;

            GameObject replacement = GetRandomEnemyReplacement(enemy, ref randomReplacementIndex);
            ReplaceEnemy( enemy, replacement, randomReplacementIndex );
        }

        void ReplaceEnemy( GameObject oldEnemy, GameObject replacementPrefab, int prefabIndex )
        {
            GameObject newEnemy = InstantiateEnemy(replacementPrefab,oldEnemy);

            //temporary, origianl name used to configure the enemy
            newEnemy.name = database.loadedEnemyPrefabNames[ prefabIndex ];

            //customize the randomized enemy
            ScaleRandomizedEnemy( newEnemy, oldEnemy );
            RotateRandomizedEnemy( newEnemy, oldEnemy );
            PositionRandomizedEnemy( newEnemy, oldEnemy );

            ModifyRandomizedEnemyGeo( newEnemy, oldEnemy );

            FixRandomizedEnemy( newEnemy, oldEnemy );

            //must happen after position
            NameRandomizedEnemy( newEnemy, prefabIndex );

            try
            {
                newEnemy.SetActive( true );
            }
            catch( Exception e )
            {
                Dev.Log( "Exception trying to activate new enemy!" + e.Message );
            }

            InitRandomizedEnemy( newEnemy, oldEnemy );

            //DebugPrintObjectTree( oldEnemy, true );

            oldEnemy.gameObject.name = "Rando Replaced Enemy: " + oldEnemy.gameObject.name;


            //put replaced enemies in a "box of doom"
            //when tied enemy is kiled, kill the replaced enemy in the box
            Dev.Log( "Adding replacement pair: " + oldEnemy.gameObject.name + " replaced by " + newEnemy.gameObject.name );
            replacements.Add( new ReplacementPair() { original = oldEnemy, replacement = newEnemy } );

            //TODO: remove this 
            //newEnemy.PrintSceneHierarchyTree(true);

            //hide the old enemy for now
            try
            {
                oldEnemy.SetActive( false );
            }
            catch( Exception e )
            {
                Dev.Log( "Exception trying to deactivate old enemy!" + e.Message );
            }
        }

        GameObject InstantiateEnemy( GameObject prefab, GameObject oldEnemy )
        {
            //where we'll place the new enemy in the scene
            //GameObject newEnemyRoot = GameObject.Find("_Enemies");

            GameObject newEnemy = UnityEngine.Object.Instantiate(prefab) as GameObject;

            //newEnemy.transform.SetParent( newEnemyRoot.transform );
            newEnemy.transform.SetParent( oldEnemy.transform.parent );

            newEnemy.tag = prefab.tag;

            return newEnemy;
        }

        void NameRandomizedEnemy( GameObject newEnemy, int prefabIndex )
        {
            newEnemy.name = randoEnemyNamePrefix + database.loadedEnemyPrefabNames[ prefabIndex ]; //gameObject.name; //if we put the game object's name here it'll re-randomize itself (whoops)
        }

        void ScaleRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            Vector3 originalNewEnemyScale = newEnemy.transform.localScale;

            //adjust big enemies that replace small enemies
            if( GetTypeSize( GetTypeFlags( newEnemy ) ) == FLAGS.BIG && GetTypeSize( GetTypeFlags( oldEnemy ) ) == FLAGS.SMALL )
            {
                newEnemy.transform.localScale = newEnemy.transform.localScale * .5f;
            }
            if( GetTypeSize( GetTypeFlags( newEnemy ) ) == FLAGS.MED && GetTypeSize( GetTypeFlags( oldEnemy ) ) == FLAGS.SMALL )
            {
                newEnemy.transform.localScale = newEnemy.transform.localScale * .75f;
            }

            if( newEnemy.name.Contains( "Mawlek Turret" ) )
            {
                newEnemy.transform.localScale = originalNewEnemyScale * .6f;
            }
        }

        void InitRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            TryInitPlayMakerFSM( newEnemy );
        }

        public static void TryInitPlayMakerFSM( GameObject newEnemy )
        {
            //wake up the mage knight
            if( newEnemy.name.Contains( "Mage Knight" ) )
            {
                PlayMakerFSM fsm = null;

                foreach( Component c in newEnemy.GetComponents<Component>() )
                {
                    if( c as PlayMakerFSM != null )
                    {
                        if( ( c as PlayMakerFSM ).FsmName == "Mage Knight" )
                        {
                            fsm = ( c as PlayMakerFSM );
                            break;
                        }
                    }
                }

                if( fsm != null )
                {
                    fsm.SendEvent( "FINISHED" );
                    fsm.SendEvent( "WAKE" );
                }
            }
            else if( newEnemy.name.Contains( "Electric Mage" ) )
            {
                PlayMakerFSM fsm = null;

                foreach( Component c in newEnemy.GetComponents<Component>() )
                {
                    if( c as PlayMakerFSM != null )
                    {
                        if( ( c as PlayMakerFSM ).FsmName == "Mage Knight" )
                        {
                            fsm = ( c as PlayMakerFSM );
                            break;
                        }
                    }
                }

                if( fsm != null )
                {
                    fsm.SendEvent( "IN RANGE" );
                    fsm.SendEvent( "FINISHED" );
                    fsm.SendEvent( "WAKE" );
                }
            }
            else if( newEnemy.name.Contains( "Mage" ) )
            {
                PlayMakerFSM fsm = null;

                foreach( Component c in newEnemy.GetComponents<Component>() )
                {
                    if( c as PlayMakerFSM != null )
                    {
                        if( ( c as PlayMakerFSM ).FsmName == "Mage" )
                        {
                            fsm = ( c as PlayMakerFSM );
                            break;
                        }
                    }
                }

                if( fsm != null )
                {
                    fsm.SendEvent( "IN RANGE" );
                    fsm.SendEvent( "WAKE" );
                }
            }


        }

        void FixRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            foreach( Transform t in newEnemy.GetComponentsInChildren<Transform>( true ) )
            {
                foreach( Component c in t.GetComponents<Component>() )
                {
                    TryFixPlayMakerFSM( newEnemy, c );
                }
            }
        }


        public static void TryFixPlayMakerFSM( GameObject newEnemy, Component c )
        {
            if( newEnemy.name.Contains( "Mage" ) && !newEnemy.name.Contains( "Knight" ) )
                return;

            try
            {
                if( c as PlayMakerFSM != null )
                {
                    foreach( var s in ( c as PlayMakerFSM ).FsmStates )
                    {
                        foreach( var a in s.Actions )
                        {
                            HutongGames.PlayMaker.Actions.GetColliderRange gcr = a as HutongGames.PlayMaker.Actions.GetColliderRange;
                            if( gcr != null )
                            {
                                if( gcr.gameObject == null )
                                {
                                    Dev.Log( "TESTING PLAYMAKER FIX GetColliderRange" );
                                    PlayMakerFSM fsmProxy = ( c as PlayMakerFSM );
                                    //if( fsmProxy == null )
                                    //{
                                    //    continue;
                                    //}

                                    //testing...
                                    HutongGames.PlayMaker.FsmOwnerDefault goTarget = new HutongGames.PlayMaker.FsmOwnerDefault();
                                    goTarget.GameObject = fsmProxy.Fsm.GameObject;
                                    goTarget.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;
                                    //HutongGames.PlayMaker.FsmEventTarget fsmEventTarget = new HutongGames.PlayMaker.FsmEventTarget();
                                    //fsmEventTarget.excludeSelf = false;
                                    //fsmEventTarget.target = HutongGames.PlayMaker.FsmEventTarget.EventTarget.GameObject;
                                    //fsmEventTarget.gameObject = goTarget;
                                    //fsmEventTarget.sendToChildren = false;

                                    gcr.gameObject = goTarget;
                                    Dev.Log( "ENDG TESTING PLAYMAKER FIX" );
                                }
                            }

                            HutongGames.PlayMaker.Actions.ChaseObjectV2 doc = a as HutongGames.PlayMaker.Actions.ChaseObjectV2;
                            if( doc != null )
                            {
                                Dev.Log( "TESTING PLAYMAKER FIX ChaseObjectV2" );
                                //if( doc.target != null )
                                //    continue;

                                HutongGames.PlayMaker.FsmGameObject go = new HutongGames.PlayMaker.FsmGameObject(HeroController.instance.proxyFSM.Fsm.GameObject);

                                doc.target = go;
                                Dev.Log( "ENDG TESTING PLAYMAKER FIX" );
                            }


                            HutongGames.PlayMaker.Actions.FaceObject fo = a as HutongGames.PlayMaker.Actions.FaceObject;
                            if( fo != null )
                            {
                                Dev.Log( "TESTING PLAYMAKER FIX FaceObject" );
                                //if( fo.objectA != null )
                                //    continue;

                                PlayMakerFSM fsmProxy = ( c as PlayMakerFSM );
                                HutongGames.PlayMaker.FsmGameObject goa = new HutongGames.PlayMaker.FsmGameObject(fsmProxy.Fsm.GameObject);
                                HutongGames.PlayMaker.FsmGameObject gob = new HutongGames.PlayMaker.FsmGameObject(HeroController.instance.proxyFSM.Fsm.GameObject);

                                fo.objectA = goa;
                                fo.objectB = gob;
                                Dev.Log( "ENDG TESTING PLAYMAKER FIX" );
                            }


                            HutongGames.PlayMaker.Actions.CheckTargetDirection ctd = a as HutongGames.PlayMaker.Actions.CheckTargetDirection;
                            if( ctd != null )
                            {
                                Dev.Log( "TESTING PLAYMAKER FIX CheckTargetDirection" );
                                //if( ctd.target != null )
                                //    continue;

                                PlayMakerFSM fsmProxy = ( c as PlayMakerFSM );
                                HutongGames.PlayMaker.FsmGameObject goa = new HutongGames.PlayMaker.FsmGameObject(fsmProxy.Fsm.GameObject);
                                HutongGames.PlayMaker.FsmGameObject gob = new HutongGames.PlayMaker.FsmGameObject(HeroController.instance.proxyFSM.Fsm.GameObject);

                                HutongGames.PlayMaker.FsmOwnerDefault goTarget = new HutongGames.PlayMaker.FsmOwnerDefault();
                                goTarget.GameObject = goa;
                                goTarget.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;
                                ctd.gameObject = goTarget;
                                ctd.target = gob;
                                Dev.Log( "ENDG TESTING PLAYMAKER FIX" );
                            }
                        }
                    }
                }
            }
            catch( Exception e )
            {
                Dev.Log( "Exception:" + e.Message );
            }
        }

        //TODO: add variables to allow players to adjust the geo rates
        void ModifyRandomizedEnemyGeo( GameObject newEnemy, GameObject oldEnemy )
        {
            if( EnemyRandomizer.Instance.RandomizeGeo )
            {
                int smallGeo = GetTypeSize(GetTypeFlags(newEnemy)) == FLAGS.SMALL ? GameRNG.Rand(0,10) : GameRNG.Rand(0,20);
                int medGeo = GetTypeSize(GetTypeFlags(newEnemy)) == FLAGS.MED ? GameRNG.Rand(0,10) : GameRNG.Rand(0,5);
                int bigGeo = GetTypeSize(GetTypeFlags(newEnemy)) == FLAGS.BIG ? GameRNG.Rand(0,20) : 0;

                if( IsHardEnemy( GetTypeFlags( newEnemy ) ) )
                {
                    smallGeo += GameRNG.Rand( 0, 20 );
                    medGeo += GameRNG.Rand( 0, 10 );
                    bigGeo += GameRNG.Rand( 0, 5 );

                    newEnemy.SetEnemyGeoRates( smallGeo, medGeo, bigGeo );
                }

                newEnemy.SetEnemyGeoRates( smallGeo, medGeo, bigGeo );
            }
            else
            {
                if( IsHardEnemy( GetTypeFlags( newEnemy ) ) )
                {
                    int smallGeo = GameRNG.Rand(0,40);
                    int medGeo = GameRNG.Rand(0,30);
                    int bigGeo = GameRNG.Rand(0,25);

                    newEnemy.SetEnemyGeoRates( smallGeo, medGeo, bigGeo );
                }
                else if( IsColloseumEnemy( GetTypeFlags( newEnemy ) ) )
                {
                    newEnemy.SetEnemyGeoRates( oldEnemy );
                }
                else
                {
                }
            }
        }

        //adjust the rotation to take into account the new monster type and/or size     
        void RotateRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            if( !newEnemy.name.Contains( "Ceiling Dropper" ) )
                newEnemy.transform.rotation = oldEnemy.transform.rotation;

            //if they were a wall flying mantis, don't rotate the replacement
            if( oldEnemy.name.Contains( "Mantis Flyer Child" ) )
            {
                newEnemy.transform.rotation = Quaternion.identity;
            }

            //mosquitos rotate, so spawn replacements with default rotation
            if( oldEnemy.name.Contains( "Mosquito" ) )
            {
                newEnemy.transform.rotation = Quaternion.identity;
            }

            //looks like it's not needed???
            //if( newEnemy.name.Contains( "Crystallised Lazer Bug" ) )
            //{
            //    Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
            //    newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
            //}

            if( oldEnemy.name.Contains( "Moss Walker" ) )
            {
                Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
                newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
            }
        }

        public IEnumerator PositionCorrector( GameObject newEnemy, Vector3 lockedPosition )
        {
            float time = 4f;
            while( time > 0f )
            {
                //player could leave screen while this is happening.. or other strange things could happen
                if( newEnemy == null )
                    yield break;

                newEnemy.transform.position = lockedPosition;

                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        void PositionRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            //adjust the position to take into account the new monster type and/or size
            newEnemy.transform.position = oldEnemy.transform.position;

            Vector3 positionOffset = Vector3.zero;

            //TODO: needs more adjusting to figure out why it doesn't attack sometimes
            if( newEnemy.name.Contains( "Electric Mage" ) )
            {
                Vector3 pos = oldEnemy.transform.position - new Vector3(-2f, -20f,0f);
                ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, pos ) );
            }

            //TODO: needs more work to figure out why it despawns? or teleports sometimes
            if( newEnemy.name.Contains( "Giant Fly Col" ) )
            {
                ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, oldEnemy.transform.position ) );
            }


            int flags = GetTypeFlags(newEnemy);
            if( ( flags & FLAGS.GROUND ) > 0 )
            {
                Vector3 toGround = GetVectorToGround(newEnemy);
                Vector3 onGround = GetPointOnGround(newEnemy);

                newEnemy.transform.position = onGround;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                positionOffset = new Vector3( 0f, collider.size.y, 0f );

                //TODO: TEST, see if this fixes him spawning in the roof
                if( newEnemy.name.Contains( "Mantis Traitor Lord" ) )
                {
                    ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, newEnemy.transform.position + new Vector3( positionOffset.x, positionOffset.y, positionOffset.z ) ) );
                }
            }

            if( ( flags & FLAGS.WALL ) > 0 || ( flags & FLAGS.CRAWLER ) > 0 )
            {
                Vector2 originalUp = oldEnemy.transform.up.normalized;

                Vector2 ePos = newEnemy.transform.position;
                Vector2 upOffset = ePos + originalUp * 10f;

                Vector2 originalDown = -originalUp;

                float projectionDistance = 50f;

                Vector3 toSurface = GetVectorTo(ePos,originalDown,projectionDistance);

                //project the ceiling droppers onto the ceiling
                if( newEnemy.name.Contains( "Ceiling Dropper" ) )
                {
                    projectionDistance = 500f;
                    toSurface = GetVectorTo( ePos, Vector2.up, projectionDistance );
                }

                //Dev.Log( "CRAWLER/WALL: ToSurface: " + toSurface );

                Vector2 finalDir = toSurface.normalized;
                Vector3 onGround = GetPointOn(ePos,finalDir, projectionDistance);

                newEnemy.transform.position = onGround;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                if( newEnemy.name.Contains( "Plant Trap" ) )
                {
                    positionOffset = originalUp * 2f;
                }
                if( collider != null && newEnemy.name.Contains( "Mawlek Turret" ) )
                {
                    positionOffset = originalUp * collider.size.y / 3f;
                }
                if( collider != null && newEnemy.name.Contains( "Mushroom Turret" ) )
                {
                    positionOffset = originalUp * collider.size.y / 10f;
                }
                if( newEnemy.name.Contains( "Plant Turret" ) )
                {
                    positionOffset = originalUp * .7f;
                }
                if( collider != null && newEnemy.name.Contains( "Laser Turret" ) )
                {
                    positionOffset = originalUp * collider.size.y / 10f;

                }
                if( collider != null && newEnemy.name.Contains( "Worm" ) )
                {
                    positionOffset = originalUp * collider.size.y / 3f;
                }

                if( newEnemy.name.Contains( "Ceiling Dropper" ) )
                {
                    //move it down a bit, keeps spawning in roof
                    positionOffset = Vector3.down * 2f;
                }

                if( ( flags & FLAGS.CRAWLER ) > 0 )
                {
                    positionOffset = originalUp * 1f;

                    //TODO: test this, needs to be closer to the ground than the rest
                    if( newEnemy.name.Contains( "Crystallised Lazer Bug" ) )
                    {
                        positionOffset = -finalDir * .25f;
                    }

                    //TODO: test this, needs to be farther from the ground than the rest
                    if( newEnemy.name.Contains( "Mines Crawler" ) )
                    {
                        positionOffset = -finalDir * 1.25f;
                    }
                }
                //DebugCreateLine( onGround, newEnemy.transform.position + new Vector3( positionOffset.x, positionOffset.y, positionOffset.z ), Color.red );
            }

            newEnemy.transform.position = newEnemy.transform.position + new Vector3( positionOffset.x, positionOffset.y, positionOffset.z );

        }

        Vector3 GetPointOnGround( GameObject entitiy )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,5f, Physics2D.AllLayers);

            Vector2 lastGoodPoint = Vector2.zero;

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Dev.Log( "GetPointOnGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) || v.collider.gameObject.name.Contains( "Platform" ) || v.collider.gameObject.name.Contains( "Roof" ) )
                    {
                        return v.point;
                    }
                    else
                    {
                        float newDist = (v.point - origin).magnitude;
                        float oldDist = (lastGoodPoint - origin).magnitude;

                        if( newDist < oldDist )
                        {
                            lastGoodPoint = v.point;
                        }
                    }
                }
            }
            else
            {
                Dev.Log( "GetPointOnGround:: RaycastHit2D is null! " );
            }

            return lastGoodPoint;
        }

        Vector3 GetPointOn( GameObject entitiy, Vector2 dir, float max )
        {
            return GetPointOn( entitiy.transform.position, dir, max );
        }

        Vector3 GetPointOn( Vector2 origin, Vector2 dir, float max )
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,max, Physics2D.AllLayers);

            Vector2 lastGoodPoint = Vector2.zero;

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Dev.Log( "GetPointOn:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) || v.collider.gameObject.name.Contains( "Platform" ) || v.collider.gameObject.name.Contains( "Roof" ) )
                    {
                        return v.point;
                    }
                    else
                    {
                        float newDist = (v.point - origin).magnitude;
                        float oldDist = (lastGoodPoint - origin).magnitude;

                        if( newDist < oldDist )
                        {
                            lastGoodPoint = v.point;
                        }
                    }
                }
            }
            else
            {
                Dev.Log( "GetPointOn:: RaycastHit2D is null! " );
            }

            return lastGoodPoint;
        }

        Vector3 GetVectorToGround( GameObject entitiy )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,5f, Physics2D.AllLayers);

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Dev.Log( "GetVectorToGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Dev.Log( "GetVectorToGround:: RaycastHit2D is null! " );
            }

            return Vector3.zero;
        }

        Vector3 GetVectorTo( GameObject entitiy, Vector2 dir, float max )
        {
            return GetVectorTo( entitiy.transform.position, dir, max );
        }

        Vector3 GetVectorTo( Vector2 origin, Vector2 dir, float max )
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,max, Physics2D.AllLayers);

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Dev.Log( "GetVectorTo:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Dev.Log( "GetVectorTo:: RaycastHit2D is null! " );
            }

            return Vector3.one * max;
        }



        public class FLAGS
        {
            static public int GROUND = 1;
            static public int FLYING = 2;
            static public int SMALL = 4;
            static public int MED = 8;
            static public int BIG = 16;
            static public int WALL = 32;
            static public int COLLOSEUM = 64;
            static public int CRAWLER = 128;
            static public int ARENA_EXCLUDE = 256;
            static public int HARD = 512;
        }

        int GetTypeFlags( string enemy )
        {
            bool isGround = EnemyRandomizerDatabase.groundEnemyTypeNames.Contains( enemy );
            bool isFlying = EnemyRandomizerDatabase.flyerEnemyTypeNames.Contains( enemy );
            bool isSmall = EnemyRandomizerDatabase.smallEnemyTypeNames.Contains( enemy );
            bool isMed = EnemyRandomizerDatabase.mediumEnemyTypeNames.Contains( enemy );
            bool isLarge = EnemyRandomizerDatabase.bigEnemyTypeNames.Contains( enemy );
            bool isWall = EnemyRandomizerDatabase.wallEnemyTypeNames.Contains( enemy );
            bool isColloseum = EnemyRandomizerDatabase.colloseumEnemyTypes.Contains( enemy );
            bool isCrawler = EnemyRandomizerDatabase.crawlerEnemyTypeNames.Contains( enemy );
            bool isArenaExluded = EnemyRandomizerDatabase.excludeFromBattleArenaZones.Contains( enemy );
            bool isHard = EnemyRandomizerDatabase.hardEnemyTypeNames.Contains( enemy );

            int flags = 0;
            flags |= ( isGround ? 1 : 0 ) << 0;
            flags |= ( isFlying ? 1 : 0 ) << 1;
            flags |= ( isSmall ? 1 : 0 ) << 2;
            flags |= ( isMed ? 1 : 0 ) << 3;
            flags |= ( isLarge ? 1 : 0 ) << 4;
            flags |= ( isWall ? 1 : 0 ) << 5;
            flags |= ( isColloseum ? 1 : 0 ) << 6;
            flags |= ( isCrawler ? 1 : 0 ) << 7;
            flags |= ( isArenaExluded ? 1 : 0 ) << 8;
            flags |= ( isHard ? 1 : 0 ) << 9;

            return flags;
        }

        int GetTypeFlags( GameObject enemy )
        {
            int flags = GetTypeFlags(enemy.name);

            return flags;
        }

        bool HasSameType( int flagsA, int flagsB )
        {
            if( ( flagsA & FLAGS.CRAWLER ) > 0 && ( flagsB & FLAGS.CRAWLER ) > 0 )
            {
                return true;
            }
            if( ( flagsA & FLAGS.GROUND ) > 0 && ( flagsB & FLAGS.GROUND ) > 0 )
            {
                return true;
            }
            if( ( flagsA & FLAGS.FLYING ) > 0 && ( flagsB & FLAGS.FLYING ) > 0 )
            {
                return true;
            }
            if( ( flagsA & FLAGS.WALL ) > 0 && ( flagsB & FLAGS.WALL ) > 0 )
            {
                return true;
            }
            return false;
        }

        bool HasSameSize( int flagsA, int flagsB )
        {
            if( ( flagsA & FLAGS.SMALL ) > 0 && ( flagsB & FLAGS.SMALL ) > 0 )
            {
                return true;
            }
            if( ( flagsA & FLAGS.MED ) > 0 && ( flagsB & FLAGS.MED ) > 0 )
            {
                return true;
            }
            if( ( flagsA & FLAGS.BIG ) > 0 && ( flagsB & FLAGS.BIG ) > 0 )
            {
                return true;
            }
            return false;
        }


        int GetTypeSize( int flags )
        {
            if( ( flags & FLAGS.SMALL ) > 0 )
            {
                return FLAGS.SMALL;
            }
            if( ( flags & FLAGS.MED ) > 0 )
            {
                return FLAGS.MED;
            }
            if( ( flags & FLAGS.BIG ) > 0 )
            {
                return FLAGS.BIG;
            }
            return 0;
        }

        bool IsArenaExluded( int flags )
        {
            if( ( flags & FLAGS.ARENA_EXCLUDE ) > 0 )
            {
                return true;
            }
            return false;
        }

        bool IsColloseumEnemy( int flags )
        {
            if( ( flags & FLAGS.COLLOSEUM ) > 0 )
            {
                return true;
            }
            return false;
        }

        bool IsHardEnemy( int flags )
        {
            if( ( flags & FLAGS.HARD ) > 0 )
            {
                return true;
            }
            return false;
        }

        GameObject GetRandomEnemyReplacement( GameObject enemy, ref int randomReplacementIndex )
        {
            string enemyName = enemy.name;
            string trimmedName = enemyName.TrimGameObjectName();
            int enemyFlags = GetTypeFlags(trimmedName);




            //check to see if this enemy is allowed in this scene and do some special logic depending...
            //TODO: refactor this into a function
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if( !loadedScene.IsValid() )
                    continue;

                string sceneName = loadedScene.name;

                if( sceneName.Contains( "Colosseum" ) )
                {
                    if( trimmedName.Contains( "Ceiling Dropper" ) )
                    {
                        //remove wall type
                        enemyFlags &= ~FLAGS.WALL;

                        //add flying type, this will causing ceiling droppers in the colosseum to become random flying enemies
                        //and hopefully avoid a softlock
                        enemyFlags |= FLAGS.FLYING;
                    }
                }
            }



            //if not set, enemy replacements will be completely random
            if( !EnemyRandomizer.Instance.ChaosRNG )
            {
                //set the seed based on the type of enemy we're going to randomize
                //this "should" make each enemy type randomize into the same kind of enemy
                int stringHashValue = trimmedName.GetHashCode();
                replacementRNG.Seed = stringHashValue + EnemyRandomizer.Instance.GameSeed;

                //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
                //this will cause enemy types within the same room to be randomized the same
                //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
                if( EnemyRandomizer.Instance.RoomRNG )
                {
                    int sceneHashValue = currentScene.GetHashCode();
                    replacementRNG.Seed = stringHashValue + EnemyRandomizer.Instance.GameSeed + sceneHashValue;

                }

                Dev.Log( "Settings seed to " + replacementRNG.Seed );
            }

            int emergencyAbortCounter = 0;
            int emergencyAbortCounterMax = 100000;

            Dev.Log( "loadedEnemyPrefabs.Count = " + database.loadedEnemyPrefabs.Count );

            //search for a compatible replacement
            int randomReplacement = -1;
            while( randomReplacement < 0 )
            {
                //int temp = UnityEngine.Random.Range(0, loadedEnemyPrefabs.Count);

                int temp = replacementRNG.Rand( database.loadedEnemyPrefabs.Count-1);

                GameObject tempPrefab = database.loadedEnemyPrefabs[temp];
                string tempName = database.loadedEnemyPrefabNames[temp];

                Dev.Log( "Attempted replacement index: " + temp + " which is " + tempName + " with prefab name " + tempPrefab.name );

                int tempFlags = GetTypeFlags(tempName);
                bool isValid = false;


                if( HasSameType( enemyFlags, tempFlags ) )
                {
                    if( HasSameSize( enemyFlags, tempFlags ) )
                    {
                        isValid = true;
                        Dev.Log( "Replacement is VALID." );
                    }
                }

                if( EnemyRandomizerDatabase.USE_TEST_SCENES )
                    isValid = true;

                //this one never explodes...
                if( tempName == "Ceiling Dropper Col" )
                    isValid = false;

                //if you have the void charm, shade siblings are no longer enemies
                if( HeroController.instance.playerData.royalCharmState > 2 )
                {
                    if( tempName.Contains( "Shade Sibling" ) )
                        isValid = false;
                }

                if( enemy.transform.up.y < 0f && tempName == "Mawlek Turret" )
                {
                    isValid = false;
                    Dev.Log( "(wrong type of mawlek turret)." );
                }

                if( enemy.transform.up.y > 0f && tempName == "Mawlek Turret Ceiling" )
                {
                    isValid = false;
                    Dev.Log( "(wrong type of mawlek turret)." );
                }

                if( battleControls.Count > 0 )
                {
                    if( IsArenaExluded( GetTypeFlags( tempName ) ) )
                    {
                        isValid = false;
                        Dev.Log( "(Enemy not allowed in battle arena zones)." );
                    }
                }

                if( isValid )
                    randomReplacement = temp;
                else
                    Dev.Log( "Replacement is INVALID." );

                emergencyAbortCounter++;

                if( emergencyAbortCounter > emergencyAbortCounterMax )
                {
                    Dev.Log( "ERROR: COULD NOT MATCH OR FIND A REPLACEMENT FOR " + enemy.name );
                    //basically stop trying to randomize this scene....
                    nextRestartDelay = 200000f;
                    break;
                }
            }

            randomReplacementIndex = randomReplacement;

            GameObject prefab = database.loadedEnemyPrefabs[randomReplacement];
            Dev.Log( "Spawning rando monster: " + prefab.name + " from index " + randomReplacement + " out of " + database.loadedEnemyPrefabs.Count + " to replace " + enemy.name + " at " + enemy.transform.position );
            return prefab;
        }


        bool IsEnemyDead( GameObject enemy )
        {
            return enemy == null
                || enemy.activeInHierarchy == false
                || ( enemy.IsGameEnemy() && ( enemy.GetEnemyFSM().ActiveStateName.Contains( "Corpse" ) || enemy.GetEnemyFSM().ActiveStateName.Contains( "Death" ) || ( enemy.GetEnemyFSM().Fsm.Variables.FindFsmInt( "HP" ) != null && FSMUtility.GetInt( enemy.GetEnemyFSM(), "HP" ) <= 0 ) ) );
        }

        void ControlReplacementRoot()
        {
            Vector3 somewhereOffInSpace = Vector3.one * 50000f;
            if( replacements.Count > 0 )
            {
                foreach( ReplacementPair p in replacements )
                {
                    if( p.replacement == null || IsEnemyDead( p.replacement.gameObject ) )
                    // || p.replacement.gameObject == null 
                    // || p.replacement.gameObject.activeInHierarchy == false 
                    // || ( FSMUtility.ContainsFSM( p.replacement, "health_manager_enemy" ) 
                    //      && ( FSMUtility.LocateFSM( p.replacement, "health_manager_enemy" ).ActiveStateName.Contains( "Corpse" ) 
                    //        || FSMUtility.LocateFSM( p.replacement, "health_manager_enemy" ).ActiveStateName.Contains( "Death" ) 
                    //        )
                    //    )
                    //)
                    {
                        pairsToRemove.Add( p );
                        if( p.original != null )
                            Dev.Log( "replacement died, removing original: " + p.original.name );
                        else
                            Dev.Log( "replacement died, removing original (object is null now) " );
                    }
                    else
                    {
                        if( p.original != null && !simulateReplacement )
                        {
                            p.original.SetActive( false );

                            //TODO: later try removing the re-positioning
                            p.original.transform.position = p.replacement.transform.position + somewhereOffInSpace;
                            Rigidbody2D r = p.original.GetComponentInChildren<Rigidbody2D>();
                            if( r != null )
                            {
                                r.isKinematic = true;
                                r.velocity = Vector2.zero;
                            }
                        }
                    }
                }
            }

            if( pairsToRemove.Count > 0 )
            {
                foreach( ReplacementPair p in pairsToRemove )
                {
                    //kill enemy?
                    if( p.original != null )
                    {
                        p.original.SetActive( true );
                        Dev.Log( "Sending kill to: " + p.original.name );
                        p.original.GetEnemyFSM().SendEvent( "INSTA KILL" );
                    }
                    replacements.Remove( p );
                }
                pairsToRemove.Clear();
            }
        }
    }
}
