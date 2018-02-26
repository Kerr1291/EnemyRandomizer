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

        //TODO: allow a user configurable option for this
        //set to false then the seed will be based on the type of enemy we're going to randomize
        //this will make each enemy type randomize into the same kind of enemy
        //if set to true, it also disables roomRNG and all enemies will be totally randomized
        bool chaosRNG = false;
        bool ChaosRNG {
            get {
                return chaosRNG;
            }
            set {
                if( randomizerReady && Settings != null )
                    Settings.RNGChaosMode = value;
                if( GlobalSettings != null )
                    GlobalSettings.RNGChaosMode = value;
                chaosRNG = value;
            }
        }

        //TODO: allow a user configurable option for this
        //if roomRNG is enabled, then we will also offset the seed based on the room's hash code
        //this will cause enemy types within the same room to be randomized the same
        //Example: all Spitters could be randomized into Flys in one room, and Fat Flys in another
        bool roomRNG = true;
        bool RoomRNG {
            get {
                return roomRNG;
            }
            set {
                if( randomizerReady && Settings != null )
                    Settings.RNGRoomMode = value;
                if( GlobalSettings != null )
                    GlobalSettings.RNGRoomMode = value;
                roomRNG = value;
            }
        }

        //TODO: allow a user configurable option for this
        //if enabled, this will NOT skip disabled game objects while looking for things to randomize
        //as a result, you may end up with a lot more enemies in some areas...
        bool randomizeDisabledEnemies = false;
        bool RandomizeDisabledEnemies {
            get {
                return randomizeDisabledEnemies;
            }
            set {
                if( randomizerReady && Settings != null )
                    Settings.RandomizeDisabledEnemies = value;
                if( GlobalSettings != null )
                    GlobalSettings.RandomizeDisabledEnemies = value;
                randomizeDisabledEnemies = value;
            }
        }

        string currentScene = "";
        string randoEnemyNamePrefix = "Rando Enemy: ";
        nv.Contractor randomEnemyLocator = new nv.Contractor();
        IEnumerator randomizerReplacer = null;

        float baseRestartDelay = 1f;
        float nextRestartDelay = 1f;
        float restartDelay = 0f;

        void RestoreLogic()
        {
            if( randomEnemyLocator != null )
                randomEnemyLocator.Reset();
            randomEnemyLocator = new nv.Contractor();
        }

        //entry point into the replacement logic, started on each scene transition
        void StartRandomEnemyLocator( Scene from, Scene to )
        {
            //"disable" the randomizer when we enter the title screen, it's enabled when a new game is started or a game is loaded
            if( to.name == "Menu_Title" )
                DisableEnemyRandomizer();

            Log( "Transitioning FROM [" + from.name + "] TO [" + to.name + "]" );
            if( !randomizerReady )
                return;

            //ignore randomizing on scenes that aren't normal game world scenes
            if( to.buildIndex != 7 && (to.buildIndex <= 36 || to.buildIndex > 362) )
                return;

            currentScene = to.name;

            InitReplacementLogicForScene( to.name );

            randomEnemyLocator.Reset();
                        
            Log( "Starting the replacer which will search the scene for enemies and randomize them!" );
            randomizerReplacer = DoLocateAndRandomizeEnemies();

            restartDelay = 0f;
            nextRestartDelay = baseRestartDelay;

            randomEnemyLocator.OnUpdate = LocateAndRandomizeEnemies;
            randomEnemyLocator.Looping = true;
            randomEnemyLocator.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
            
            randomEnemyLocator.Start();
        }

        void InitReplacementLogicForScene( string scene )
        {
            if( replacementRNG == null )
            {
                //only really matters if chaosRNG is enabled...
                if( loadedBaseSeed >= 0 )
                    replacementRNG = new RNG( loadedBaseSeed );
                else
                    replacementRNG = new RNG();
            }
        }

        void LocateAndRandomizeEnemies()
        {
            if( randomizerReplacer != null && !randomizerReplacer.MoveNext() )
            {
                Log( "end of iterator or iterator became null" );
                randomEnemyLocator.Reset();
            }

            if( randomizerReplacer != null && ( randomizerReplacer.Current as bool? ) == false )
            {
                Log( "iterator returned false" );
                //randomEnemyLocator.Reset();
                randomizerReplacer = null;
            }

            if( randomizerReplacer == null )
            {
                if( restartDelay <= 0 )
                {
                    restartDelay = nextRestartDelay;
                    nextRestartDelay = nextRestartDelay * 2f;
                    //restart iterator, every time it restarts, lets turn up the cooldown on restarting
                    randomizerReplacer = DoLocateAndRandomizeEnemies();
                }
                else
                {
                    restartDelay -= Time.deltaTime;
                }
            }
        }

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

            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                //iterate over the loaded game objects
                GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();

                //bool printed = false;
                foreach( GameObject rootGameObject in rootGameObjects )
                {
                    //and their children
                    if( rootGameObject == null )
                    {
                        Log( "Scene "+i+" has a null root game object! Skipping scene..." );
                        break; 
                    }

                    //if(!printed)
                    //{
                    //    printed = true;
                    //    DebugPrintAllObjects( UnityEngine.SceneManagement.SceneManager.GetSceneAt( i ).name );
                    //}

                    if( rootGameObject.name == ModRoot.name )
                    {
                        continue;
                    }

                    //Log( "searching root: " + rootGameObject.name );

                    int counter = 0;
                    foreach( Transform t in rootGameObject.GetComponentsInChildren<Transform>( true ) )
                    {
                        counter++;
                        string name = t.gameObject.name;

                        if( counter % 100 == 0 )
                            yield return true;

                        //Log( "potential rando enemy Name = " + name );

                        //don't replace null/destroyed game objects
                        if( !randomizeDisabledEnemies )
                        {
                            if( t == null || t.gameObject == null )
                                continue;
                        }

                        //don't replace inactive game objects
                        if( !t.gameObject.activeInHierarchy )
                            continue;

                        if( SkipLoadingGameObject( name ) )
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
            
            randomizerReplacer = null;
            yield return false;
        }

        void OnLoadObjectCollider( GameObject potentialEnemy )
        {
            if( !randomizerReady )
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

            int randomReplacementIndex = 0;

            GameObject replacement = GetRandomEnemyReplacement(enemy, ref randomReplacementIndex);
            ReplaceEnemy( enemy, replacement, randomReplacementIndex );
        }

        void ReplaceEnemy( GameObject oldEnemy, GameObject replacementPrefab, int prefabIndex )
        {
            //where we'll place the new enemy in the scene
            oldEnemy.SetActive( false );

            GameObject newEnemy = InstantiateEnemy(replacementPrefab);

            //temporary, origianl name used to configure the enemy
            newEnemy.name = loadedEnemyPrefabNames[ prefabIndex ];

            ScaleRandomizedEnemy( newEnemy );
            RotateRandomizedEnemy( newEnemy, oldEnemy );
            PositionRandomizedEnemy( newEnemy, oldEnemy );

            //must happen after position
            NameRandomizedEnemy( newEnemy, prefabIndex );

            newEnemy.SetActive( true );

            //NEW: TESTING THIS FUNCTIONALITY
            GameObject.Destroy( oldEnemy );
        }

        GameObject InstantiateEnemy( GameObject prefab )
        {
            //where we'll place the new enemy in the scene
            GameObject newEnemyRoot = GameObject.Find("_Enemies");

            GameObject newEnemy = UnityEngine.Object.Instantiate(prefab) as GameObject;

            newEnemy.transform.SetParent( newEnemyRoot.transform );

            //TODO: generate an interesting or unique name?
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

            newEnemy.transform.position = newEnemy.transform.position + new Vector3( positionOffset.x, positionOffset.y, positionOffset.z );

            //if(newEnemy.name.Contains("Lesser Mawlek"))
            //{

            //}
        }
        //    public class mything
        //    {
        //        public StateFlags flags = 0;

        //        public void SetFlags()
        //        {
        //            flags |= ( isGround ? StateFlags.Ground : 0 );
        //        }

        //        public void CheckFlags()
        //        {
        //            if( flags.HasFlag( StateFlags.Ground & StateFlags.Small ) )
        //        //burp
        //}
        //    }
        //TODO: finish tomorrow, cast a ray to the ground and use it in position randomized enemy to place them on the ground
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

        void RotateRandomizedEnemy( GameObject newEnemy, GameObject oldEnemy )
        {
            //TODO adjust the rotation to take into account the new monster type and/or size
            newEnemy.transform.rotation = oldEnemy.transform.rotation;
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
        }

        int GetTypeFlags( string enemy )
        {
            bool isGround = IsExactlyInList(enemy, EnemyRandoData.groundEnemyTypeNames);
            bool isFlying = IsExactlyInList(enemy, EnemyRandoData.flyerEnemyTypeNames);
            bool isSmall = IsExactlyInList(enemy, EnemyRandoData.smallEnemyTypeNames);
            bool isMed = IsExactlyInList(enemy, EnemyRandoData.mediumEnemyTypeNames);
            bool isLarge = IsExactlyInList(enemy, EnemyRandoData.bigEnemyTypeNames);
            bool isWall = IsExactlyInList(enemy, EnemyRandoData.wallEnemyTypeNames);
            bool isHard = IsExactlyInList(enemy, EnemyRandoData.hardEnemyTypeNames);

            int flags = 0;
            flags |= ( isGround ? 1 : 0 ) << 0;
            flags |= ( isFlying ? 1 : 0 ) << 1;
            flags |= ( isSmall ? 1 : 0 ) << 2;
            flags |= ( isMed ? 1 : 0 ) << 3;
            flags |= ( isLarge ? 1 : 0 ) << 4;
            flags |= ( isWall ? 1 : 0 ) << 5;
            flags |= ( isHard ? 1 : 0 ) << 6;

            return flags;
        }

        int GetTypeFlags( GameObject enemy )
        {
            int flags = GetTypeFlags(enemy.name);

            return flags;
        }

        bool HasSameType( int flagsA, int flagsB )
        {
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
            }

            //search for a compatible replacement
            int randomReplacement = -1;
            while( randomReplacement < 0 )
            {
                //int temp = UnityEngine.Random.Range(0, loadedEnemyPrefabs.Count);
                int temp = replacementRNG.Rand(loadedEnemyPrefabs.Count-1);

                GameObject tempPrefab = loadedEnemyPrefabs[temp];
                string tempName = loadedEnemyPrefabNames[temp];

                int tempFlags = GetTypeFlags(tempName);
                bool isValid = false;

                if( HasSameType( enemyFlags, tempFlags ) )
                {
                    if( HasSameSize( enemyFlags, tempFlags ) )
                        isValid = true;
                }

                if( ( enemyFlags & FLAGS.WALL ) > 0 && ( tempFlags & FLAGS.WALL ) > 0 )
                {
                    isValid = true;
                }

                if( isValid )
                    randomReplacement = temp;
            }

            randomReplacementIndex = randomReplacement;

            GameObject prefab = loadedEnemyPrefabs[randomReplacement];
            Log( "Spawning rando monster: " + prefab.name + " from index " + randomReplacement + " out of " + loadedEnemyPrefabs.Count + " to replace " + enemy.name );
            return prefab;
        }
    }
}
