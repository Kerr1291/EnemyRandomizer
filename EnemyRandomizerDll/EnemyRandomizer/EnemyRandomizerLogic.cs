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
using HutongGames.PlayMaker.Actions;

using Bounds = UnityEngine.Bounds;

using ModCommon;

namespace EnemyRandomizerMod
{
    //TODO: clean this up.... a lot
    public class EnemyRandomizerLogic
    {
        public static EnemyRandomizerLogic Instance { get; private set; }

        //TODO: check tomorrow
        public const bool RUN_DEBUG_INPUT = true;
        IEnumerator debugInput = null;

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        RNG replacementRNG;

        string currentScene = "";
        string randoEnemyNamePrefix = "Rando Enemy: ";
        Contractor randomEnemyLocator = new Contractor();
        IEnumerator randomizerReplacer = null;

        Contractor replacementController = new Contractor();

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

        GameObject memeEnemy;

        public EnemyRandomizerLogic( EnemyRandomizerDatabase database )
        {
            this.database = database;
        }
        
        IEnumerator DebugInput()
        {
            while( true )
            {
                yield return new WaitForEndOfFrame();
                if( UnityEngine.Input.GetKeyDown( KeyCode.O ) )
                {
                    for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
                    {
                        Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        bool status = s.IsValid();
                        if( status )
                        {
                            Dev.Log( "Dumping Scene to " + Application.dataPath + "/Managed/Mods/" + s.name );
                            //s.PrintHierarchy( i, null, EnemyRandomizerDatabase.enemyTypeNames, s.name );
                            s.PrintHierarchy( i, sceneBounds, database.loadedEnemyPrefabNames, s.name );
                        }
                    }
                }
            }

            debugInput = null;
            yield break;
        }

        public void Setup( bool simulateReplacement )
        {
            //TODO: fix this...
            //this.simulateReplacement = simulateReplacement;

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

            ModHooks.Instance.DrawBlackBordersHook -= OnSceneBoardersCreated;
            ModHooks.Instance.DrawBlackBordersHook += OnSceneBoardersCreated;

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
            randomEnemyLocator = new Contractor();

            replacementController.Reset();

            comms.DisableNode();
            Instance = null;

            ModHooks.Instance.ColliderCreateHook -= OnLoadObjectCollider;
            ModHooks.Instance.DrawBlackBordersHook -= OnSceneBoardersCreated;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= StartRandomEnemyLocator;
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
            replacementController.SetUpdateRate( Contractor.UpdateRateType.Frame );
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
            randomEnemyLocator.SetUpdateRate( Contractor.UpdateRateType.Frame );

            randomEnemyLocator.Start();

            sceneBoundry.Clear();
            //printedScenees.Clear();

            battleControls.Clear();

            if( memeEnemy != null )
            {
                GameObject.Destroy( memeEnemy );
                memeEnemy = null;
            }
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

        //bool calculateBounds = true;
        UnityEngine.Bounds sceneBounds;
        List<GameObject> sceneBoundry = new List<GameObject>();


        public void OnSceneBoardersCreated( List<GameObject> borders )
        {
            sceneBoundry.Clear();
            sceneBoundry.AddRange( borders );

            List<GameObject> xList = sceneBoundry.Select(x=>x).OrderBy(x=>x.transform.position.x).ToList();
            List<GameObject> yList = sceneBoundry.Select(x=>x).OrderBy(x=>x.transform.position.y).ToList();

            sceneBounds = new UnityEngine.Bounds
            {
                min = new Vector3( xList[ 0 ].transform.position.x, yList[ 0 ].transform.position.y, -10f ),
                max = new Vector3( xList[ xList.Count - 1 ].transform.position.x, yList[ yList.Count - 1 ].transform.position.y, 10f )
            };

            //Dev.Log( "Bounds created with dimensions" );
            //Dev.Log( "min " + sceneBounds.min );
            //Dev.Log( "max " + sceneBounds.max );
            //Dev.Log( "center " + sceneBounds.center );
            //Dev.Log( "extents " + sceneBounds.extents );
            
            PreProcessScene();
        }

        void PreProcessScene()
        {
            if( EnemyRandomizer.Instance.CustomEnemies )
            {
                if( GameManager.instance.sceneName == "Town" )
                {
                    memeEnemy = MemeController.CreateMemeEnemy();
                }
            }

            //get the battle controls by checking the FSMS 
            foreach( PlayMakerFSM pfsm in GameObject.FindObjectsOfType<PlayMakerFSM>())
            {
                if( pfsm == null )
                    continue;

                if( battleControls.Contains( pfsm.gameObject ) )
                    continue;

                PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(pfsm.gameObject, "Battle Control");
                if( playMakerFSM != null )
                {
                    battleControls.Add( playMakerFSM.gameObject );
                }
            }
        }

        bool modifiedHornet = false;
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

            while( sceneBoundry.Count <= 0 )
            {
                yield return true;
            }


            //TODO: temp, remove once we're done developing her
            if( !modifiedHornet )
            {
                while( GameObject.Find( "Hornet Boss 1" ) == null )
                    yield return new WaitForEndOfFrame();

                GameObject.Find( "Hornet Boss 1" ).AddComponent<HornetBoss>();
                modifiedHornet = true;
            }

            foreach( HealthManager ho in GameObject.FindObjectsOfType<HealthManager>() )
            {
                while(Time.timeScale <= 0f)
                {
                    yield return true;
                }

                //skip child components of randomized enemies
                foreach( Transform p in ho.GetComponentsInParent<Transform>( true ) )
                {
                    if( p.name.Contains( "Rando" ) || p.name.Contains( EnemyRandomizer.Instance.ModRoot.name ) )
                        continue;
                }

                //don't replace null/destroyed game objects
                if( ho == null || ho.gameObject == null )
                    continue;

                //don't replace inactive game objects
                if( !ho.gameObject.activeInHierarchy )
                    continue;

                string name = ho.gameObject.name.TrimGameObjectName();

                if( !sceneBounds.Contains( ho.transform.position ) )
                {
                    //kill rando enemies that get outside the scene bounds
                    //TODO: test, don't kill jar collector
                    if( name.Contains( "Rando" ) && !name.Contains( "Replaced" ) && !name.Contains("Jar Collector") )
                    {
                        Dev.Log( "Sending force kill for out of scene to " + ho.gameObject.name );
                        ho.Die( null, AttackTypes.Splatter, true );
                        continue;
                    }
                    //Dev.Log( "Skipping " + t.gameObject.name + " Because it is outside the bounds. " + t.position );
                    continue;
                }

                if( name.IsSkipRandomizingString() )
                    continue;

                GameObject potentialEnemy = ho.gameObject;

                bool isRandoEnemy = potentialEnemy.IsRandomizerEnemy( database.loadedEnemyPrefabNames );

                if( EnemyRandomizerDatabase.USE_TEST_SCENES )
                {
                    isRandoEnemy = potentialEnemy.IsRandomizerEnemy( database.loadedEnemyPrefabNames );
                }

                if( isRandoEnemy && !potentialEnemy.IsEnemyDead() )
                {
                    RandomizeEnemy( potentialEnemy );
                    yield return true;
                }
            }



            yield return null;

            randomizerReplacer = null;

            yield return false;
        }

        void OnLoadObjectCollider( GameObject potentialEnemy )
        {
            bool isRandoEnemy = potentialEnemy.IsRandomizerEnemy( database.loadedEnemyPrefabNames );

            if( EnemyRandomizerDatabase.USE_TEST_SCENES )
            {
                isRandoEnemy = potentialEnemy.IsRandomizerEnemy( database.loadedEnemyPrefabNames );
            }

            if( isRandoEnemy )
            {
                RandomizeEnemy( potentialEnemy );

                //Dev.Log( "FINISHED RANDOMIZING: " + potentialEnemy.name );
            }
        }
        

        //TODO: change to return by ref?
        public GameObject CreateEnemy(string name, Vector3 position)
        {
            if( simulateReplacement )
            {
                Dev.Log( "Simulating create enemy "+name );
                return null;
            }

            int replacementIndex = 0;

            GameObject enemyPrefab = GetEnemyTypePrefab(name, ref replacementIndex);
            if( enemyPrefab == null )
                return null;

            Dev.Log( "Creating enemy "+name );
            
            GameObject newEnemy = PlaceEnemy( enemyPrefab, enemyPrefab, replacementIndex, position );

            newEnemy.name = "Rando Custom: " + name;

            return newEnemy;
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

            //DebugPrintObjectTree( oldEnemy, true );

            oldEnemy.gameObject.name = "Rando Replaced Enemy: " + oldEnemy.gameObject.name;

            //hook up the old enemy's battle scene to the new enemy replacement
            HealthManager oldHM = oldEnemy.GetComponent<HealthManager>();
            HealthManager newHM = newEnemy.GetComponent<HealthManager>();
            if(oldHM != null && newHM != null)
            {
                FieldInfo fi = oldHM.GetType().GetField("battleScene",BindingFlags.NonPublic|BindingFlags.Instance);
                if( fi != null )
                {
                    GameObject battleScene = fi.GetValue(oldHM) as GameObject;
                    if( battleScene != null )
                        fi.SetValue( newHM, battleScene ); 
                }
            }


            //put replaced enemies in a "box of doom"
            //when tied enemy is kiled, kill the replaced enemy in the box
            Dev.Log( "Adding replacement pair: " + oldEnemy.gameObject.name + " replaced by " + newEnemy.gameObject.name );
            //replacements.Add( new ReplacementPair() { original = oldEnemy, replacement = newEnemy } );

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

            Dev.Log( "New stats for rando monster: " + newEnemy.name + " to replace " + oldEnemy.name + " at " + newEnemy.transform.position + " with rotation " + newEnemy.transform.localRotation.eulerAngles );
        }

        GameObject InstantiateEnemy( GameObject prefab, GameObject oldEnemy )
        {
            //where we'll place the new enemy in the scene
            //GameObject newEnemyRoot = GameObject.Find("_Enemies");

            //Dev.Log( "Loading " + prefab );
            //Dev.Log( "Loading " + prefab.name );
            //GameObject newEnemy = UnityEngine.Object.Instantiate(Resources.Load(prefab.name)) as GameObject;
            GameObject newEnemy = UnityEngine.Object.Instantiate(prefab) as GameObject;
            //UnityEngine.Object.Instantiate(prefab) as GameObject;

            //newEnemy.transform.SetParent( newEnemyRoot.transform );
            newEnemy.transform.SetParent( oldEnemy.transform.parent );

            //newEnemy.tag = prefab.tag;

            return newEnemy;
        }

        void NameRandomizedEnemy( GameObject newEnemy, int prefabIndex )
        {
            newEnemy.name = randoEnemyNamePrefix + database.loadedEnemyPrefabNames[ prefabIndex ]; //gameObject.name; //if we put the game object's name here it'll re-randomize itself (whoops)
        }

        void ScaleRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            Vector3 originalNewEnemyScale = newEnemy.transform.localScale;

            Collider2D newC = newEnemy.GetComponent<Collider2D>();
            Collider2D oldC = oldEnemy.GetComponent<Collider2D>();
            
            //TODO: create a settings bool for silly scaling
            //new scale logic, compare the size of the colliders and adjust the scale by the ratios
            if(false && newC != null && oldC != null)
            {
                //bounds returns null on colliders of uninstantiated and unactivated game objects
                //so we need to determine the type of collider manually by downcasting and then query its bounds by hand
                Vector2 newBounds = Vector2.zero;

                PolygonCollider2D newCPoly = newC as PolygonCollider2D;
                if( newCPoly != null )
                {
                    newBounds = new Vector2( newCPoly.points.Select( x => x.x ).Max() - newCPoly.points.Select(x=>x.x).Min(), newCPoly.points.Select( x => x.y ).Max() - newCPoly.points.Select( x => x.y ).Min());
                }
                CircleCollider2D newCCircle = newC as CircleCollider2D;
                if( newCCircle != null )
                {
                    newBounds = Vector2.one * newCCircle.radius;
                }
                BoxCollider2D newCBox = newC as BoxCollider2D;
                if( newCBox != null)
                {
                    newBounds = newCBox.size;
                }

                Bounds oldBounds = oldC.bounds;

                Dev.Log( oldBounds.size.x + " / " + newBounds.x );
                Dev.Log( oldBounds.size.y + " / " + newBounds.y );

                float xRatio = oldBounds.size.x / newBounds.x;
                float yRatio = oldBounds.size.y / newBounds.y;

                float minBoundsValue = Mathf.Min( xRatio, yRatio );

                Vector3 oldScale = newEnemy.transform.localScale;

                //scale the new enemy to the size of the old enemy
                newEnemy.transform.localScale = new Vector3( newEnemy.transform.localScale.x * minBoundsValue,
                    newEnemy.transform.localScale.y * minBoundsValue, 1f );

                Dev.Log( "Old scale = " + oldScale + " New scale = " + newEnemy.transform.localScale );
            }
            //no two colliders to compare? then do old logic (until it's removed....)
            else
            {
                //adjust big enemies that replace small enemies
                if( ( GetTypeFlags( newEnemy ) & FLAGS.BIG ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.BIG ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 1f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.BIG ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.MED ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * .5f;
                }
                else if( (GetTypeFlags( newEnemy ) & FLAGS.BIG) > 0 && (GetTypeFlags( oldEnemy ) & FLAGS.SMALL) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * .25f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.MED ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.BIG ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 1.5f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.MED ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.MED ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 1f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.MED ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.SMALL ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * .5f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.SMALL ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.BIG ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 2.0f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.SMALL ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.MED ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 1.5f;
                }
                else if( ( GetTypeFlags( newEnemy ) & FLAGS.SMALL ) > 0 && ( GetTypeFlags( oldEnemy ) & FLAGS.SMALL ) > 0 )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * 1f;
                }

                if( newEnemy.name.Contains( "Mawlek Turret" ) )
                {
                    newEnemy.transform.localScale = originalNewEnemyScale * .6f;
                }

                //shrink him!
                if( newEnemy.name.Contains( "Mantis Traitor Lord" ) && ( GetTypeSize( GetTypeFlags( oldEnemy ) ) == FLAGS.SMALL || GetTypeSize( GetTypeFlags( oldEnemy ) ) == FLAGS.MED ) )
                {
                    newEnemy.transform.localScale = newEnemy.transform.localScale * .4f;
                }
            }
        }

        void FixRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            if( newEnemy.name == "Mega Fat Bee" )
            {
                {
                    List<SetPosition> actions = newEnemy.GetFSMActionsOnStates<SetPosition>( new List<string>() { "Swoop In" }, "fat fly bounce" );
                    foreach( var a in actions )
                    {
                        a.vector = newEnemy.transform.position;
                    }
                }
                {
                    List<Translate> actions = newEnemy.GetFSMActionsOnStates<Translate>( new List<string>() { "Swoop In" }, "fat fly bounce" );
                    foreach( var a in actions )
                    {
                        a.vector = Vector3.zero;
                    }
                }
            }

            if( newEnemy.name == "Jar Collector" )
            {
                {//Keep the collector from jumping out of the scene
                    List<FloatCompare> actions = newEnemy.GetFSMActionsOnStates<FloatCompare>( new List<string>() { "Jump" }, "Control" );
                    foreach( var a in actions )
                    {
                        a.float2 = sceneBounds.max.y - 26f;
                    }
                }
                {//Keep the collector from jumping out of the scene
                    List<RandomFloat> actions = newEnemy.GetFSMActionsOnStates<RandomFloat>( new List<string>() { "Spawn", "Return Antic" }, "Control" );
                    foreach( var a in actions )
                    {
                        a.min = newEnemy.transform.position.x - 10f;
                        a.max = newEnemy.transform.position.x + 10f;
                    }
                }
            }

            //TEST
            if(newEnemy.name == "Mage Lord" || newEnemy.name == "Dream Mage Lord")
            {
                {//
                    List<SetFloatValue> actions = newEnemy.GetFSMActionsOnStates<SetFloatValue>( new List<string>() { "Set Entry Point" }, "Mage Lord" );
                    actions[ 0 ].floatValue = newEnemy.transform.position.y;
                    actions[ 1 ].floatValue = newEnemy.transform.position.x;
                    actions[ 2 ].floatValue = newEnemy.transform.position.y;
                    //foreach( var a in actions )
                    //{
                    //    a.floatValue = newEnemy.transform.position.y + 20f;
                    //}
                }
                {//
                    List<FloatCompare> actions = newEnemy.GetFSMActionsOnStates<FloatCompare>( new List<string>() { "Set Entry Point" }, "Mage Lord" );
                    actions[ 0 ].float2 = -100f;
                    //foreach( var a in actions )
                    //{
                    //    a.floatValue = newEnemy.transform.position.y + 20f;
                    //}
                }
                {//Tele Top Y
                    List<SetFloatValue> actions = newEnemy.GetFSMActionsOnStates<SetFloatValue>( new List<string>() { "Tele Top Y" }, "Mage Lord" );
                    foreach( var a in actions )
                    {
                        a.floatValue = newEnemy.transform.position.y + 20f;
                    }
                }
                {//Tele Bot Y
                    List<SetFloatValue> actions = newEnemy.GetFSMActionsOnStates<SetFloatValue>( new List<string>() { "Tele Bot Y" }, "Mage Lord" );
                    foreach( var a in actions )
                    {
                        a.floatValue = newEnemy.transform.position.y - 5f;
                    }
                }
            }

            if( newEnemy.name.Contains("Mawlek Turret") )
            {
                Vector2 up = newEnemy.transform.up.normalized;
                //TODO: make them fling in the direction the mawlek turret is facing
                //{
                //    List<FlingObjectsFromGlobalPool> actions = newEnemy.GetFSMActionsOnStates<FlingObjectsFromGlobalPool>( new List<string>() { "Fire Left" }, "Mawlek Turret" );
                //    foreach( var a in actions )
                //    {
                //        Dev.Log(a.angleMax
                //    }
                //}
                //{
                //    List<FlingObjectsFromGlobalPool> actions = newEnemy.GetFSMActionsOnStates<FlingObjectsFromGlobalPool>( new List<string>() { "Fire Right" }, "Mawlek Turret" );
                //    foreach( var a in actions )
                //    {
                //        a.floatValue = newEnemy.transform.position.y - 5f;
                //    }
                //}
            }


            

            //TODO: store this value off in our mod and set it to true in the scenes that care about it... but for now....
            if(newEnemy.name.Contains( "Mega Zombie Beam Miner" ) ) 
            {
                //fix the targetting source?
                {
                    List<GetPosition> actions = newEnemy.GetFSMActionsOnStates<GetPosition>( new List<string>(){"Aim Left", "Aim Right", "Aim"},"Beam Miner");
                    foreach( var a in actions )
                    {
                        if( a.gameObject.GameObject.Name == "Beam Point R" || a.gameObject.GameObject.Name == "Beam Point L" )
                        {
                            GameObject beamPoint = GameObject.Instantiate( database.loadedEffectPrefabs[ a.gameObject.GameObject.Name ] );
                            HutongGames.PlayMaker.FsmGameObject fsmGO = new HutongGames.PlayMaker.FsmGameObject(beamPoint);

                            HutongGames.PlayMaker.FsmOwnerDefault fsmOwnerDefault = new HutongGames.PlayMaker.FsmOwnerDefault();
                            fsmOwnerDefault.GameObject = fsmGO;
                            fsmOwnerDefault.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;

                            a.gameObject = fsmOwnerDefault;
                        }
                        if( a.gameObject.GameObject.Name == "Beam Origin" )
                        {
                            GameObject beamOrigin = newEnemy.FindGameObjectInChildren("Beam Origin");
                            HutongGames.PlayMaker.FsmGameObject fsmGO = new HutongGames.PlayMaker.FsmGameObject(beamOrigin);

                            HutongGames.PlayMaker.FsmOwnerDefault fsmOwnerDefault = new HutongGames.PlayMaker.FsmOwnerDefault();
                            fsmOwnerDefault.GameObject = fsmGO;
                            fsmOwnerDefault.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;

                            a.gameObject = fsmOwnerDefault;
                        }
                    }
                }
                {
                    List<SetPosition> actions = newEnemy.GetFSMActionsOnStates<SetPosition>( new List<string>(){"Aim Left", "Aim Right", "Aim"},"Beam Miner");
                    foreach( var a in actions )
                    {
                        if( a.gameObject.GameObject.Name == "Beam Ball" )
                        {
                            GameObject beamBall = GameObject.Instantiate( database.loadedEffectPrefabs[ a.gameObject.GameObject.Name ] );
                            HutongGames.PlayMaker.FsmGameObject fsmGO = new HutongGames.PlayMaker.FsmGameObject(beamBall);

                            HutongGames.PlayMaker.FsmOwnerDefault fsmOwnerDefault = new HutongGames.PlayMaker.FsmOwnerDefault();
                            fsmOwnerDefault.GameObject = fsmGO;
                            fsmOwnerDefault.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;

                            a.gameObject = fsmOwnerDefault;
                        }

                        if( a.gameObject.GameObject.Name == "Beam" )
                        {
                            GameObject beam = GameObject.Instantiate( database.loadedEffectPrefabs[ a.gameObject.GameObject.Name ] );
                            HutongGames.PlayMaker.FsmGameObject fsmGO = new HutongGames.PlayMaker.FsmGameObject(beam);

                            HutongGames.PlayMaker.FsmOwnerDefault fsmOwnerDefault = new HutongGames.PlayMaker.FsmOwnerDefault();
                            fsmOwnerDefault.GameObject = fsmGO;
                            fsmOwnerDefault.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;

                            a.gameObject = fsmOwnerDefault;
                        }
                    }
                }
            }
        }


        //TODO: add variables to allow players to adjust the geo rates
        void ModifyRandomizedEnemyGeo( GameObject newEnemy, GameObject oldEnemy )
        {
            if(newEnemy.name == "Bursting Zombie" )
            { 
                int smallGeo = GameRNG.Rand( 0, 5 );
                int medGeo = GameRNG.Rand( 1, 2 );
                int bigGeo = GameRNG.Rand( 0, 1 );

                newEnemy.SetEnemyGeoRates( smallGeo, medGeo, bigGeo );
            }

            if(newEnemy.name == "Lazy Flyer Enemy" )
            {
                int smallGeo = 0;
                int medGeo = 0;
                int bigGeo = GameRNG.Rand( 0, 10 );

                newEnemy.SetEnemyGeoRates( smallGeo, medGeo, bigGeo );
            }


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
                    int smallGeo = GameRNG.Rand(0,20);
                    int medGeo = GameRNG.Rand(0,15);
                    int bigGeo = GameRNG.Rand(0,10);

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
            
            //TODO: after next content patch is out
            if( newEnemy.name.Contains( "Crystallised Lazer Bug" ) )
            {
                //Dev.Log( "Old rotation = " + newEnemy.transform.rotation.eulerAngles );
                //Quaternion rotate = Quaternion.Euler(new Vector3(0f,0f,-180f));
                //newEnemy.transform.rotation = rotate * oldEnemy.transform.rotation;
                //Dev.Log( "New rotation = " + newEnemy.transform.rotation.eulerAngles );
                //( ContractorManager.Instance ).StartCoroutine( RotationCorrector( newEnemy, oldEnemy.transform.rotation, 1.5f ) );
                //newEnemy.PrintSceneHierarchyTree(true);
                //crawler = newEnemy;
            }

            if( newEnemy.name.Contains( "Mines Crawler" ) )
            {
                Quaternion rot180degrees = Quaternion.Euler( -oldEnemy.transform.rotation.eulerAngles );
                newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
                newEnemy.transform.rotation = Quaternion.identity;
            }

            //if( oldEnemy.name.Contains( "Moss Walker" ) )
            //{
            //    //Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
            //    //newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
            //    newEnemy.transform.rotation = Quaternion.identity;
            //}
        }

        public IEnumerator PositionCorrector( GameObject newEnemy, Vector3 lockedPosition, float lockTime = 4f )
        {
            float time = lockTime;
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

        public IEnumerator PositionCorrectorProjection( GameObject newEnemy, Quaternion matchDir, Vector2 origin, Vector2 dir, float max, float startDelay = 0f, float timeout = 4f )
        {
            Dev.Log( "WAITING" );
            yield return new WaitForSeconds( startDelay );
            Dev.Log( "STARTING" );
            float time = timeout;
            while( time > 0f )
            {
                yield return new WaitForEndOfFrame();
                //player could leave screen while this is happening.. or other strange things could happen
                if( newEnemy == null )
                    yield break;

                time -= Time.deltaTime;

                Dev.Log( "Searching for match: "+ newEnemy.transform.rotation.eulerAngles.z +" VS "+ matchDir.eulerAngles.z );
                if( !Mathf.Approximately( newEnemy.transform.rotation.eulerAngles.z, matchDir.eulerAngles.z ) )
                    continue;

                Dev.Log( "found match! Projecting to new pos..." );
                newEnemy.transform.position = GetPointOn(origin,dir,max);
                //Dev.CreateLineRenderer( origin, newEnemy.transform.position, Color.white, -2f );
                break;
            }
            yield break;
        }

        public IEnumerator RotationCorrector( GameObject newEnemy, Quaternion lockedRotation, float lockTime = 4f )
        {
            float time = lockTime;
            while( time > 0f )
            {
                //player could leave screen while this is happening.. or other strange things could happen
                if( newEnemy == null )
                    yield break;

                newEnemy.transform.localRotation = lockedRotation;

                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            yield break;
        }

        //optional startingPos to use to place the enemy intstead of the old enemy's position
        void PositionRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy, Vector3? startingPos = null )
        {
            //adjust the position to take into account the new monster type and/or size
            newEnemy.transform.position = oldEnemy.transform.position;

            if( startingPos != null && startingPos.HasValue )
            {
                newEnemy.transform.position = startingPos.Value;
            }

            Vector3 positionOffset = Vector3.zero;

            //TODO: needs more adjusting to figure out why it doesn't attack sometimes
            //if( newEnemy.name.Contains( "Electric Mage" ) )
            //{
            //    Vector3 pos = oldEnemy.transform.position - new Vector3(-2f, -20f,0f);
            //    ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, pos ) );
            //}

            //TODO: needs more work to figure out why it despawns? or teleports sometimes
            //if( newEnemy.name.Contains( "Giant Fly Col" ) )
            //{
            //    ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, oldEnemy.transform.position ) );
            //}


            int flags = GetTypeFlags(newEnemy);
            if( ( flags & FLAGS.GROUND ) > 0 )
            {
                Vector3 toGround = GetVectorToGround(newEnemy, 50f);
                Vector3 onGround = GetPointOnGround(newEnemy, 50f);

                newEnemy.transform.position = onGround;

                Vector2 scale = newEnemy.transform.localScale;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                positionOffset = new Vector3( 0f, collider.size.y * scale.y, 0f );

                if( newEnemy.name.Contains( "Lobster" ) )
                {
                    positionOffset = positionOffset + (Vector3)(Vector2.up * 2f) * scale.y;
                }
                if( newEnemy.name.Contains( "Blocker" ) )
                {
                    positionOffset = positionOffset + (Vector3)( Vector2.up * -1f ) * scale.y;
                }
                if( newEnemy.name == ( "Moss Knight" ) )
                {
                    positionOffset = positionOffset + (Vector3)( Vector2.up * -1f ) * scale.y;
                }
                if( newEnemy.name == ( "Enemy" ) )
                {
                    positionOffset = positionOffset + (Vector3)( Vector2.up * -0.5f ) * scale.y;
                }

                
                //TODO: TEST, see if this fixes him spawning in the roof
                //if( newEnemy.name.Contains( "Mantis Traitor Lord" ) )
                //{
                //    ( ContractorManager.Instance ).StartCoroutine( PositionCorrector( newEnemy, newEnemy.transform.position + positionOffset ) );
                //}
            }

            bool newEnemyIsMantisFlyerChild = newEnemy.name.Contains( "Mantis Flyer Child" );

            if( newEnemyIsMantisFlyerChild ||( flags & FLAGS.WALL ) > 0 || ( flags & FLAGS.CRAWLER ) > 0 )
            {
                Vector2 scale = newEnemy.transform.localScale;

                Vector2 originalUp = oldEnemy.transform.up.normalized;

                Vector2 ePos = newEnemy.transform.position;
                
                Vector2 upOffset = ePos + originalUp * 5f;

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
                    positionOffset = originalUp * 2f * scale.y;
                }
                if( collider != null && newEnemy.name.Contains( "Mawlek Turret" ) )
                {
                    positionOffset = originalUp * collider.size.y / 3f * scale.y;
                }
                if( collider != null && newEnemy.name.Contains( "Mushroom Turret" ) )
                {
                    positionOffset = (originalUp * .5f) * scale.y;// collider.size.y;
                }
                if( newEnemy.name.Contains( "Plant Turret" ) )
                {
                    positionOffset = originalUp * .7f * scale.y;
                }
                if( collider != null && newEnemy.name.Contains( "Laser Turret" ) )
                {
                    positionOffset = originalUp * collider.size.y / 10f * scale.y;
                }
                if( collider != null && newEnemy.name.Contains( "Worm" ) )
                {
                    positionOffset = originalUp * collider.size.y / 3f * scale.y;
                }

                if( newEnemy.name.Contains( "Ceiling Dropper" ) )
                {
                    //move it down a bit, keeps spawning in roof
                    positionOffset = Vector3.down * 2f * scale.y;
                }

                if( ( flags & FLAGS.CRAWLER ) > 0 )
                {
                    //positionOffset =  * 1f;
                    //BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                    if( collider != null )
                        positionOffset = new Vector3( collider.size.x * originalUp.x * scale.x, collider.size.y * originalUp.y * scale.y, 0f );

                    //TODO: test this, fix this up after next content patch is out
                    //if( newEnemy.name.Contains( "Crystallised Lazer Bug" ) )
                    //{
                    //    //suppposedly 1/2 their Y collider space offset should be 1.25
                    //    //but whatever we set it at, they spawn pretty broken, so spawn them out of the ground a bit so they're still a threat
                    //    positionOffset = -finalDir * 1.0f;
                    //    //( ContractorManager.Instance ).StartCoroutine( PositionCorrectorProjection( newEnemy, newEnemy.transform.rotation, upOffset, finalDir, 10f, 1f, 10f ) );
                    //}

                    //TODO: test this, needs to be farther from the ground than the rest
                    if( newEnemy.name.Contains( "Mines Crawler" ) )
                    {
                        positionOffset = -finalDir * 1.5f * scale.y;
                    }

                    if( newEnemy.name.Contains( "Spider Mini" ) )
                    {
                        positionOffset = Vector3.zero;
                    }

                    //TODO: unknown what value this needs
                    if( newEnemy.name.Contains( "Abyss Crawler" ) )
                    {
                        positionOffset = Vector3.zero;
                    }

                    //TODO: unknown what value this needs
                    if( newEnemy.name.Contains( "Climber" ) )
                    {
                        positionOffset = Vector3.zero;
                    }
                }

                //show adjustment
                //Dev.CreateLineRenderer( onGround, newEnemy.transform.position + positionOffset, Color.red, -1f );
            }

            newEnemy.transform.position = newEnemy.transform.position + positionOffset;

        }

        static bool IsGameSurfaceCollider( Collider2D collider )
        {
            return ( collider.GetComponent<Roof>() != null
                  || collider.gameObject.name.Contains( "Chunk" )
                  || collider.gameObject.name.Contains( "Platform" )
                  || collider.gameObject.name.Contains( "Roof" ) );
        }

        Vector3 GetPointOnGround( GameObject entitiy, float maxProjectionDistance )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction, maxProjectionDistance, Physics2D.AllLayers);

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

        Vector3 GetVectorToGround( GameObject entitiy, float maxProjectionDistance )
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction, maxProjectionDistance, Physics2D.AllLayers);

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

        public GameObject GetEnemyTypePrefab( string enemyTypeName, ref int randomReplacementIndex )
        {
            string enemyName = enemyTypeName;
            string trimmedName = enemyName.TrimGameObjectName();
            int enemyFlags = GetTypeFlags(trimmedName);
            
            //get the enemy's index
            int enemyTypeIndex = database.loadedEnemyPrefabNames.IndexOf( enemyTypeName );

            if( enemyTypeIndex < 0 )
            {
                Dev.Log( "Error: " + enemyTypeName + " not found in loaded enemy types." );
                return null;
            }

            randomReplacementIndex = enemyTypeIndex;

            //use the index to get the prefab
            GameObject enemyTypePrefab = database.loadedEnemyPrefabs[enemyTypeIndex];

            GameObject prefab = database.loadedEnemyPrefabs[enemyTypeIndex];
            Dev.Log( "Getting prefab for monster: " + prefab.name + " from index " + enemyTypeIndex );
            return prefab;
        }


        public GameObject PlaceEnemy( GameObject oldEnemy, GameObject replacementPrefab, int prefabIndex, Vector3 pos )
        {
            GameObject newEnemy = InstantiateEnemy(replacementPrefab,oldEnemy);

            //temporary, origianl name used to configure the enemy
            newEnemy.name = database.loadedEnemyPrefabNames[ prefabIndex ];

            //customize the randomized enemy
            ScaleRandomizedEnemy( newEnemy, oldEnemy );
            RotateRandomizedEnemy( newEnemy, oldEnemy );
            PositionRandomizedEnemy( newEnemy, oldEnemy, pos );

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

            //InitRandomizedEnemy( newEnemy, oldEnemy );
            
            Dev.Log( "New stats for custom rando monster: " + newEnemy.name + " at " + newEnemy.transform.position + " with rotation " + newEnemy.transform.localRotation.eulerAngles );
            return newEnemy;
        }


        GameObject GetRandomEnemyReplacement( GameObject enemy, ref int randomReplacementIndex )
        {
            string enemyName = enemy.name;
            string trimmedName = enemyName.TrimGameObjectName();
            int enemyFlags = GetTypeFlags(trimmedName);




            //check to see if this enemy is allowed in this scene and do some special logic depending...
            //TODO: refactor this into a function
            if( GameManager.instance.GetCurrentMapZone() == "COLOSSEUM" )
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
            int emergencyAbortCounterMax = 100;

            Dev.Log( "loadedEnemyPrefabs.Count = " + database.loadedEnemyPrefabs.Count );

            //search for a compatible replacement
            int randomReplacement = -1;
            while( randomReplacement < 0 )
            {
                //int temp = UnityEngine.Random.Range(0, loadedEnemyPrefabs.Count);

                int temp = replacementRNG.Rand( database.loadedEnemyPrefabs.Count-1);

                GameObject tempPrefab = database.loadedEnemyPrefabs[temp];
                string tempName = database.loadedEnemyPrefabNames[temp];

                Dev.Log( "Attempted replacement index: " + temp + " which is " + tempName );
                
                //this one is broken for now....
                if( tempName.Contains( "Hatcher Baby" ) )
                    continue;

                Dev.Log( " with prefab name " + tempPrefab.name );

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

                if( tempName == "Pigeon" )
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


                if( GameManager.instance.GetCurrentMapZone() == "COLOSSEUM" )
                {
                    if( EnemyRandomizerDatabase.excludeFromColosseum.Contains( tempName ) )
                        isValid = false;
                }
                else
                {
                    if( battleControls.Count > 0 )
                    {
                        if( IsArenaExluded( GetTypeFlags( tempName ) ) )
                        {
                            isValid = false;
                            Dev.Log( "(Enemy not allowed in battle arena zones)." );
                        }
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
            Dev.Log( "Spawning rando monster: " + prefab.name + " from index " + randomReplacement + " out of " + database.loadedEnemyPrefabs.Count + " to replace " + enemy.name );
            return prefab;
        }

        void ControlReplacementRoot()
        {
            Vector3 somewhereOffInSpace = Vector3.one * 50000f;
            if( replacements.Count > 0 )
            {
                foreach( ReplacementPair p in replacements )
                {
                    if( p.replacement == null || p.replacement.gameObject.IsEnemyDead() )
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
                    if( p.original != null )
                    {
                        //enable the enemy so it can die
                        p.original.SetActive( true );

                        Dev.Log( "Calling Die() on: " + p.original.name );
                        p.original.GetEnemyHealthManager().Die( null, AttackTypes.Generic, true );

                        //TODO: update this a bit 
                    }
                    replacements.Remove( p );
                }
                pairsToRemove.Clear();
            }
        }
    }
}