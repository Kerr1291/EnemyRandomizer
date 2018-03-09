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

using nv;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        RNG replacementRNG;
        
        //set to false then the seed will be based on the type of enemy we're going to randomize
        //this will make each enemy type randomize into the same kind of enemy
        //if set to true, it also disables roomRNG and all enemies will be totally randomized
        bool chaosRNG = false;
        public bool ChaosRNG {
            get {
                return chaosRNG;
            }
            set {
                if( RandomizerReady && Settings != null )
                    Settings.RNGChaosMode = value;
                if( GlobalSettings != null )
                    GlobalSettings.RNGChaosMode = value;
                chaosRNG = value;
            }
        }
        
        //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
        //this will cause enemy types within the same room to be randomized the same
        //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
        bool roomRNG = true;
        public bool RoomRNG {
            get {
                return roomRNG;
            }
            set {
                if( RandomizerReady && Settings != null )
                    Settings.RNGRoomMode = value;
                if( GlobalSettings != null )
                    GlobalSettings.RNGRoomMode = value;
                roomRNG = value;
            }
        }
        
        //if enabled, this will NOT skip disabled game objects while looking for things to randomize
        //as a result, you may end up with a lot more enemies in some areas...
        //bool randomizeDisabledEnemies = false;
        //public bool RandomizeDisabledEnemies {
        //    get {
        //        return randomizeDisabledEnemies;
        //    }
        //    set {
        //        if( RandomizerReady && Settings != null )
        //            Settings.RandomizeDisabledEnemies = value;
        //        if( GlobalSettings != null )
        //            GlobalSettings.RandomizeDisabledEnemies = value;
        //        randomizeDisabledEnemies = value;
        //    }
        //}

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


        float baseRestartDelay = 1f;
        float nextRestartDelay = 1f;
        float restartDelay = 0f;

        void RestoreLogic()
        {
            if( randomEnemyLocator != null )
                randomEnemyLocator.Reset();
            randomEnemyLocator = new nv.Contractor();

            replacementController.Reset();
        }

        void CheckAndAddBattleControls( GameObject go )
        {
            if( battleControls.Contains( go ) )
                return;

            if( FSMUtility.ContainsFSM( go, "Battle Control" ) )
            {
                Log( "Found battle control on object: " + go.name );
                battleControls.Add( go );
            }

            //foreach( Component c in go.GetComponents<Component>() )
            //{
            //    PlayMakerFSM pfsm = c as PlayMakerFSM;
            //    if(pfsm.FsmName == "Battle Control")
            //    {
            //        Log( "Found battle control on object: " + go.name );
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
                Log( "Found battle gate control on object: " + go.name );
                go.SetActive( false );
            }
        }

        void UpdateBattleControls()
        {
            //Log( "A" );
            for(int i = 0; i < battleControls.Count; )
            {
                //Log( "B" );
                //remove any controls that go null (like from a scene change)
                if( battleControls[i] == null )
                {
                    battleControls.RemoveAt( i );
                    i = 0;
                    continue;
                }

                //Log( "C" );
                PersistentBoolItem pBoolItem = battleControls[i].GetComponent<PersistentBoolItem>();

                //Log( "D" );
                //does this battle control have a persistent bool? then we want to make sure it's not set
                if( pBoolItem != null && pBoolItem.persistentBoolData != null )
                {
                    //Log( "pBoolItem.persistentBoolData.activated " + pBoolItem.persistentBoolData.activated);

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

                //Log( "E" );
                //ok, so the battle control hasn't been run or completed yet, we need to manually monitor it
                BoxCollider2D collider = battleControls[i].GetComponent<BoxCollider2D>();
                Bounds localBounds;

                //Log( "F" );
                if( collider == null )
                {
                    Log( "Creating out own bounds to test" );
                    localBounds = new Bounds( battleControls[ i ].transform.position, new Vector3( 28f, 24f, 10f ) );
                }
                else
                {
                    Log( "Using provided bounds..." );
                    localBounds = collider.bounds;
                }

                //Log( "G" );

                //Log( "H" );
                //add some Z size to the bounds

                float width = Mathf.Max(28f,localBounds.extents.x);
                float height = Mathf.Max(24f,localBounds.extents.y);

                localBounds.extents = new Vector3( width, height, 10f );

                //Log( "I" );
                Vector3 heroPos = HeroController.instance.transform.position;

                //Log( "J" );
                //is the hero in the battle scene? if not, no point in checking things
                if( !localBounds.Contains( heroPos ) )
                {
                    Log( "Hero outside the bounds of our battle control, don't monitor" );
                    Log( "Hero: " + heroPos );
                    Log( "Bounds Center: " + localBounds.center );
                    Log( "Bounds Extents: " + localBounds.extents );
                    //DebugPrintObjectTree( battleControls[ i ], true );
                }
                else
                {
                    //Log( "K" );
                    //see if any rando enemies are inside the area, if they are, we don't set next
                    bool setNext = true;
                    //Log( "L" );
                    foreach( var pair in replacements )
                    {
                        //Log( "M" );
                        if( pair.replacement == null )
                            continue;

                        //Log( "N" );
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

                    //Log( "O" );

                    if( setNext )
                    {
                        Log( "Sending NEXT event to " + battleControls[ i ].name );
                        //get the battle control
                        //Log( "Q" );
                        PlayMakerFSM pfsm = FSMUtility.LocateFSM( battleControls[i], "Battle Control" );

                        if(pfsm != null)
                        {
                            pfsm.SendEvent( "NEXT" );
                        }
                    }
                }
                
                ++i;
            }
        }

        //entry point into the replacement logic, started on each scene transition
        void StartRandomEnemyLocator( Scene from, Scene to )
        {
            //"disable" the randomizer when we enter the title screen, it's enabled when a new game is started or a game is loaded
            if( to.name == "Menu_Title" )
                DisableEnemyRandomizer();

            Log( "Transitioning FROM [" + from.name + "] TO [" + to.name + "]" );
            if( !RandomizerReady )
                return;

            //ignore randomizing on scenes that aren't normal game world scenes
            //if( to.buildIndex != 7 && (to.buildIndex <= 36 || to.buildIndex > 362) )
            //    return;
            if( to.buildIndex < 4 )
                return;

            replacements.Clear();
            pairsToRemove.Clear();

            replacementController.OnUpdate = ControlReplacementRoot;
            replacementController.Looping = true;
            replacementController.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
            replacementController.Start();

            currentScene = to.name;
            
            if( ShouldSkipRandomizingScene(to.name) )
            {
                Log( "Skipping randomization of scene..." );
                randomEnemyLocator.Reset();
                return;
            }

            InitReplacementLogicForScene( to.name );

            randomEnemyLocator.Reset();
                        
            Log( "Starting the replacer which will search the scene for enemies and randomize them!" );
            randomizerReplacer = DoLocateAndRandomizeEnemies();

            restartDelay = 0f;

            //TODO: remove me
            //Time.timeScale = 2f;

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

        List<ReplacementPair> pairsToRemove = new List<ReplacementPair>();

        void InitReplacementLogicForScene( string scene )
        {
            if( replacementRNG == null )
            {
                //TODO: move to a better place, print all the loaded prefabs on load
                foreach( GameObject go in loadedEnemyPrefabs )
                {
                    DebugPrintObjectTree( go, true );
                }

                //only really matters if chaosRNG is enabled...
                if( loadedBaseSeed >= 0 )
                    replacementRNG = new RNG( loadedBaseSeed );
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
                    Log( "end of iterator or iterator became null" );
                    randomEnemyLocator.Reset();
                }

                if( randomizerReplacer != null && ( randomizerReplacer.Current as bool? ) == false )
                {
                    Log( "iterator returned false" );
                    randomizerReplacer = null;
                }
            }
            catch(Exception e)
            {
                Log( "Replacer hit an exception: " + e.Message );
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
                            Log( "Scene " + i + " has a null root game object! Skipping scene..." );
                            break;
                        }

                        if( rootGameObject.name == ModRoot.name )
                        {
                            continue;
                        }

                        int counter = 0;
                        foreach( Transform t in rootGameObject.GetComponentsInChildren<Transform>( true ) )
                        {
                            if( t.gameObject.name.Contains( "SceneBorder" ) && !sceneBoundry.Contains( t.gameObject ) )
                                sceneBoundry.Add( t.gameObject );

                            //CheckAndDisableSoftlockGates( t.gameObject );
                            CheckAndAddBattleControls( t.gameObject );

                            counter++;
                            string name = t.gameObject.name;

                            if( counter % 100 == 0 )
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

                Log( "Bounds created with dimensions" );
                Log( "min "+sceneBounds.min );
                Log( "max " + sceneBounds.max );
                Log( "center " + sceneBounds.center );
                Log( "extents " + sceneBounds.extents );

                calculateBounds = false;
            }

            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                //iterate over the loaded game objects
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();

                foreach( GameObject rootGameObject in rootGameObjects )
                {
                    //and their children
                    if( rootGameObject == null )
                    {
                        Log( "Scene "+i+" has a null root game object! Skipping scene..." );
                        break; 
                    }

                    //skip our mod root
                    if( rootGameObject.name == ModRoot.name )
                        continue;

                    int counter = 0;
                    foreach( Transform t in rootGameObject.GetComponentsInChildren<Transform>( true ) )
                    {
                        counter++;
                        string name = t.gameObject.name;

                        if( counter % 100 == 0 )
                            yield return true;

                        //don't replace null/destroyed game objects
                        if( t == null || t.gameObject == null )
                            continue;

                        if( !sceneBounds.Contains( t.position ) )
                        {
                            //Log( "Skipping " + t.gameObject.name + " Because it is outside the bounds. " + t.position );
                            continue;
                        }
                        
                        //don't replace inactive game objects
                        if( !t.gameObject.activeInHierarchy )
                            continue;                        

                        if( SkipRandomizeEnemy( name ) )
                            continue;

                        //skip child components of randomized enemies
                        foreach( Transform p in t.GetComponentsInParent<Transform>( true ) )
                        {
                            if( p.name.Contains( "Rando" ) )
                                continue;
                        }

                        GameObject potentialEnemy = t.gameObject;

                        bool isRandoEnemy = IsRandoEnemyType(potentialEnemy);
                        if( isRandoEnemy )
                            RandomizeEnemy( potentialEnemy );
                    }

                    yield return true;
                }
            }

            //Log( "Updating battle controls" );

            yield return null;

            UpdateBattleControls();

            //Log( "Printing list" );
            ////if(needsList)
            //{
            //    List<GameObject> xList = allObjs.Select(x=>x).OrderBy(x=>x.transform.position.x).ToList();
            //    List<GameObject> yList = allObjs.Select(x=>x).OrderBy(x=>x.transform.position.y).ToList();

            //    foreach(var g in xList)
            //    {
            //        Log( "X: " + g.transform.position.x + " :::: " + g.name);
            //    }

            //    foreach( var g in yList )
            //    {
            //        Log( "Y: " + g.transform.position.y + " :::: " + g.name );
            //    }

            //    needsList = false;
            //}

            randomizerReplacer = null;
            yield return false;
        }

        void OnLoadObjectCollider( GameObject potentialEnemy )
        {
            if( !RandomizerReady )
                return;

            bool isRandoEnemy = IsRandomizerEnemy(potentialEnemy);
            if( isRandoEnemy )
                RandomizeEnemy( potentialEnemy );
        }

        void RandomizeEnemy( GameObject enemy )
        {
            //this failsafe is needed here in the case where we have exceptional things that should NOT be randomized
            if( SkipRandomizeEnemy( enemy.name ) )
            {
                //Log( "Exceptional case found in SkipRandomizeEnemy() -- Skipping randomization for: " + enemy.name );
                return;
            }

            Log( "Randomizing: " + enemy.name );

            if( simulateReplacement )
            {
                Log( "Sim mode enabled, not replacing the enemy, but we'll flag it like we did!" );
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
            //where we'll place the new enemy in the scene

            //TODO: testing in combination with other logic, like the destroy stuff
            //oldEnemy.SetActive( false );

            GameObject newEnemy = InstantiateEnemy(replacementPrefab);

            //temporary, origianl name used to configure the enemy
            newEnemy.name = loadedEnemyPrefabNames[ prefabIndex ];

            ScaleRandomizedEnemy( newEnemy );
            RotateRandomizedEnemy( newEnemy, oldEnemy );
            PositionRandomizedEnemy( newEnemy, oldEnemy );

            //must happen after position
            NameRandomizedEnemy( newEnemy, prefabIndex );

            newEnemy.SetActive( true );

            //TODO: test new idea: put replaced enemies in a "box of doom"
            //when tied enemy is kiled, kill the replaced enemy in the box
            //NEW: TESTING THIS FUNCTIONALITY
            //GameObject.Destroy( oldEnemy );

            //DebugPrintObjectTree( oldEnemy, true );

            oldEnemy.gameObject.name = "Rando Replaced Enemy: " + oldEnemy.gameObject.name;

            Log( "Adding replacement pair: "+ oldEnemy.gameObject.name +" replaced by "+ newEnemy.gameObject.name );
            replacements.Add( new ReplacementPair() { original = oldEnemy, replacement = newEnemy } );

            //hide the old enemy for now
            oldEnemy.SetActive( false );
        }

        GameObject InstantiateEnemy( GameObject prefab )
        {
            //where we'll place the new enemy in the scene
            GameObject newEnemyRoot = GameObject.Find("_Enemies");

            GameObject newEnemy = UnityEngine.Object.Instantiate(prefab) as GameObject;

            newEnemy.transform.SetParent( newEnemyRoot.transform );
            
            newEnemy.tag = prefab.tag;

            return newEnemy;
        }

        void NameRandomizedEnemy( GameObject newEnemy, int prefabIndex )
        {
            newEnemy.name = randoEnemyNamePrefix + loadedEnemyPrefabNames[ prefabIndex ]; //gameObject.name; //if we put the game object's name here it'll re-randomize itself (whoops)
        }

        void ScaleRandomizedEnemy( GameObject newEnemy )
        {
            //TODO as a fun factor option, try scaling the new enemy?
            if( newEnemy.name.Contains( "Mawlek Turret" ) )
                newEnemy.transform.localScale = newEnemy.transform.localScale * .6f;
        }

        void RotateRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            //TODO adjust the rotation to take into account the new monster type and/or size            
            newEnemy.transform.rotation = oldEnemy.transform.rotation;            

            if(oldEnemy.name.Contains( "Mantis Flyer Child" ) )
            {
                newEnemy.transform.rotation = Quaternion.identity;
            }

            //if they're ceiling droppers, flip them the opposite direction
            if( newEnemy.name.Contains( "Ceiling Dropper" ) )
            {
                Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
                newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
            }

            //if they're the grass ambush things, flip them opposite (find out the name of the enemy)

            //if they're wall flying mantis, don't rotate the replacement
        }
        
        void PositionRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            //TODO adjust the position to take into account the new monster type and/or size
            newEnemy.transform.position = oldEnemy.transform.position;

            Vector3 positionOffset = Vector3.zero;

            int flags = GetTypeFlags(newEnemy);
            if( ( flags & FLAGS.GROUND ) > 0 )
            {
                Vector3 toGround = GetVectorToGround(newEnemy);
                Vector3 onGround = GetPointOnGround(newEnemy);

                newEnemy.transform.position = onGround;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                positionOffset = new Vector3( 0f, collider.size.y, 0f );
            }

            if( ( flags & FLAGS.WALL ) > 0 || ( flags & FLAGS.CRAWLER ) > 0 )
            {
                Vector2 originalUp = oldEnemy.transform.up.normalized;

                Vector2 ePos = newEnemy.transform.position;
                Vector2 upOffset = ePos + originalUp * 10f;

                Vector2 originalDown = -originalUp;

                Vector3 toSurface = GetVectorTo(ePos,originalDown,50f);

                //Log( "CRAWLER/WALL: ToSurface: " + toSurface );

                Vector2 finalDir = toSurface.normalized;
                Vector3 onGround = GetPointOn(ePos,finalDir, 50f);

                newEnemy.transform.position = onGround;

                BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                if( newEnemy.name.Contains( "Plant Trap") )
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

                if( ( flags & FLAGS.CRAWLER ) > 0 )
                {
                    positionOffset = originalUp * 1f;
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

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Log( "GetPointOnGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        return v.point;
                    }
                }
            }
            else
            {
                Log( "GetPointOnGround:: RaycastHit2D is null! " );
            }

            return Vector3.zero;
        }

        Vector3 GetPointOn( GameObject entitiy, Vector2 dir, float max )
        {
            return GetPointOn( entitiy.transform.position, dir, max );
        }

        Vector3 GetPointOn( Vector2 origin, Vector2 dir, float max )
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin,direction,max, Physics2D.AllLayers);

            if( toGround != null )
            {
                foreach( var v in toGround )
                {
                    Log( "GetPointOn:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        return v.point;
                    }
                }
            }
            else
            {
                Log( "GetPointOn:: RaycastHit2D is null! " );
            }

            return Vector3.one * max;
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
                    Log( "GetVectorToGround:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Log( "GetVectorToGround:: RaycastHit2D is null! " );
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
                    Log( "GetVectorTo:: RaycastHit2D hit object: " + v.collider.gameObject.name );
                    if( v.collider.gameObject.name.Contains( "Chunk" ) )
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Log( "GetVectorTo:: RaycastHit2D is null! " );
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
            static public int HARD = 64;
            static public int CRAWLER = 128;
        }

        int GetTypeFlags( string enemy )
        {
            bool isGround = IsExactlyInList(enemy, EnemyRandoData.groundEnemyTypeNames);
            bool isFlying = IsExactlyInList(enemy, EnemyRandoData.flyerEnemyTypeNames);
            bool isSmall = IsExactlyInList(enemy, EnemyRandoData.smallEnemyTypeNames);
            bool isMed = IsExactlyInList(enemy, EnemyRandoData.mediumEnemyTypeNames);
            bool isLarge = IsExactlyInList(enemy, EnemyRandoData.bigEnemyTypeNames);
            bool isWall = IsExactlyInList(enemy, EnemyRandoData.wallEnemyTypeNames);
            //bool isHard = IsExactlyInList(enemy, EnemyRandoData.hardEnemyTypeNames);
            bool isCrawler = IsExactlyInList(enemy, EnemyRandoData.crawlerEnemyTypeNames);

            int flags = 0;
            flags |= ( isGround ? 1 : 0 ) << 0;
            flags |= ( isFlying ? 1 : 0 ) << 1;
            flags |= ( isSmall ? 1 : 0 ) << 2;
            flags |= ( isMed ? 1 : 0 ) << 3;
            flags |= ( isLarge ? 1 : 0 ) << 4;
            flags |= ( isWall ? 1 : 0 ) << 5;
            //flags |= ( isHard ? 1 : 0 ) << 6;
            flags |= ( isCrawler ? 1 : 0 ) << 7;

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

        GameObject GetRandomEnemyReplacement( GameObject enemy, ref int randomReplacementIndex )
        {
            string enemyName = enemy.name;
            string trimmedName = TrimEnemyNameToBeLoaded(enemyName);
            int enemyFlags = GetTypeFlags(trimmedName);
            
            //if not set, enemy replacements will be completely random
            if( !chaosRNG )
            {
                //set the seed based on the type of enemy we're going to randomize
                //this "should" make each enemy type randomize into the same kind of enemy
                int stringHashValue = trimmedName.GetHashCode();
                replacementRNG.Seed = stringHashValue + loadedBaseSeed;
                
                //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
                //this will cause enemy types within the same room to be randomized the same
                //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
                if( roomRNG )
                {
                    int sceneHashValue = currentScene.GetHashCode();
                    replacementRNG.Seed = stringHashValue + loadedBaseSeed + sceneHashValue;

                }

                Log( "Settings seed to " + replacementRNG.Seed );
            }

            int emergencyAbortCounter = 0;
            int emergencyAbortCounterMax = 100000;

            Log( "loadedEnemyPrefabs.Count = " + loadedEnemyPrefabs.Count );

            //search for a compatible replacement
            int randomReplacement = -1;
            while( randomReplacement < 0 )
            {
                //int temp = UnityEngine.Random.Range(0, loadedEnemyPrefabs.Count);
                
                int temp = replacementRNG.Rand(loadedEnemyPrefabs.Count-1);

                GameObject tempPrefab = loadedEnemyPrefabs[temp];
                string tempName = loadedEnemyPrefabNames[temp];


                Log( "Attempted replacement index: " + temp + " which is " + tempName + " with prefab name " + tempPrefab.name );

                int tempFlags = GetTypeFlags(tempName);
                bool isValid = false;

                if( HasSameType( enemyFlags, tempFlags ) )
                {
                    if( HasSameSize( enemyFlags, tempFlags ) )
                    {
                        isValid = true;
                        Log( "Replacement is VALID." );
                    }                    
                }

                if( enemy.transform.up.y < 0f && tempName == "Mawlek Turret" )
                {
                    isValid = false;
                    Log( "(wrong type of mawlek turret)." );
                }

                if( enemy.transform.up.y > 0f && tempName == "Mawlek Turret Ceiling" )
                {
                    isValid = false;
                    Log( "(wrong type of mawlek turret)." );
                }
                
                if( isValid )
                    randomReplacement = temp;
                else
                    Log( "Replacement is INVALID." );

                emergencyAbortCounter++;

                if( emergencyAbortCounter > emergencyAbortCounterMax )
                {
                    Log( "ERROR: COULD NOT MATCH OR FIND A REPLACEMENT FOR " + enemy.name );
                    //basically stop trying to randomize this scene....
                    nextRestartDelay = 200000f;
                    break;
                }
            }

            randomReplacementIndex = randomReplacement;

            GameObject prefab = loadedEnemyPrefabs[randomReplacement];
            Log( "Spawning rando monster: " + prefab.name + " from index " + randomReplacement + " out of " + loadedEnemyPrefabs.Count + " to replace " + enemy.name );
            return prefab;
        }



        void ControlReplacementRoot()
        {
            Vector3 somewhereOffInSpace = Vector3.one * 50000f;
            if( replacements.Count > 0 )
            {
                foreach( ReplacementPair p in replacements )
                {
                    if( p.replacement == null 
                        || p.replacement.gameObject == null 
                        || p.replacement.gameObject.activeInHierarchy == false 
                        || ( FSMUtility.ContainsFSM( p.replacement, "health_manager_enemy" ) 
                             && ( FSMUtility.LocateFSM( p.replacement, "health_manager_enemy" ).ActiveStateName.Contains( "Corpse" ) 
                               || FSMUtility.LocateFSM( p.replacement, "health_manager_enemy" ).ActiveStateName.Contains( "Death" ) 
                               )
                           )
                       )
                    {
                        pairsToRemove.Add( p );
                        if( p.original != null )
                            Log( "replacement died, removing original: " + p.original.name );
                        else
                            Log( "replacement died, removing original (object is null now) " );
                    }
                    else
                    {
                        if( p.original != null && !simulateReplacement )
                        {
                            p.original.SetActive( false );
                            p.original.transform.position = somewhereOffInSpace;
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
                        Log( "Sending kill to: " + p.original.name );
                        p.original.GetEnemyFSM().SendEvent( "INSTA KILL" );
                    }
                    replacements.Remove( p );
                }
                pairsToRemove.Clear();
            }
        }
    }
}
