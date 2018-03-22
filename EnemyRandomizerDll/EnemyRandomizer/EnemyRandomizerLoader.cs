using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

using nv;

namespace EnemyRandomizerMod
{
    //TODO: move the wake components into their own classes....
    public class DebugOnWake : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject owner;

        //the fsm to aim our events at
        public string fsmName;

        //the events to send the fsm
        public List<string> wakeEvents = new List<string>();

        //send the wake events every time we're in this state for the fsmName given
        public string sendWakeEventsOnState;

        //Dictionary = FSM and the current state it's in
        public Dictionary<PlayMakerFSM,string> fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

        //run logic on the FSMs every frame?
        public bool monitorFSMStates = false;

        //print debug bounding boxes and debug log info?
        public bool logFSM = true;

        private void OnDisable()
        {
            if( monitorFSMStates )
            {
                if( logFSM )
                    Dev.Log( "DebugFSMS was DebugOnWake disabled, likely because the enemy died " );

                //final FSM info....
                foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
                {
                    if( p == null )
                        continue;

                    if( !fsmsOnObject.ContainsKey( p ) )
                    {
                        fsmsOnObject.Add( p, p.ActiveStateName );
                        if( logFSM )
                            Dev.Log( "DebugFSMS :::: Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
                    }
                    else if( p.ActiveStateName != fsmsOnObject[ p ] )
                    {
                        if( logFSM )
                            Dev.Log( "DebugFSMS :::: " + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[ p ] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ( ( p.Fsm != null && p.Fsm.LastTransition != null ) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE" ) + "]" );
                        fsmsOnObject[ p ] = p.ActiveStateName;
                    }
                }
            }
        }

        IEnumerator DebugFSMS()
        {
            fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

            foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
            {
                fsmsOnObject.Add( p, p.ActiveStateName );
                if( logFSM )
                    Dev.Log( "Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
            }

            bool IsEnemyDead( GameObject enemy )
            {
                return enemy == null
                    || enemy.activeInHierarchy == false
                    || ( enemy.IsGameEnemy() && ( enemy.GetEnemyFSM().ActiveStateName.Contains( "Corpse" ) || enemy.GetEnemyFSM().ActiveStateName.Contains( "Death" ) || ( enemy.GetEnemyFSM().Fsm.Variables.FindFsmInt( "HP" ) != null && FSMUtility.GetInt( enemy.GetEnemyFSM(), "HP" ) <= 0 ) ) );
            }

            //Dev.Log( "FSMDEBUG :::: Tracking FSMS on " + owner.name );
            while( monitorFSMStates )
            {
                if( owner == null )
                    yield break;

                //if( owner.name.Contains( "Garden" ) && owner.FindGameObjectInChildren( "Attack Range" ).activeInHierarchy )
                //{
                //    //    //this fixes the slash spider!
                //    //    //DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
                //    //    DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
                //    //    d.monitorFSMStates = true;
                //    //    //d.monitorFSMStates = true;
                //    //    //d.sendWakeEventsOnState = "Waiting";
                //    //    //d.logFSM = false;

                //    //    //HutongGames.PlayMaker.Actions.GetColliderRange gcr = modifiedPrefab.GetFSMActionOnState<HutongGames.PlayMaker.Actions.GetColliderRange>("Select Target","Mage");

                //    //    //Dev.Log( "PRINTING!!!" );
                //    //    //PlayMakerFSM fsm = FSMUtility.LocateFSM( modifiedPrefab, "Mage" );
                //    //    //if( fsm != null && gcr != null )
                //    //    //{
                //    //    //    if( gcr.gameObject != null )
                //    //    //    {
                //    //    //        Dev.Log( "DebugFSMS OO -- " + gcr.gameObject.OwnerOption );
                //    //    //        Dev.Log( "DebugFSMS GO -- " + gcr.gameObject.GameObject );
                //    //    //    }
                //    //    //    if( gcr.gameObject.GameObject != null )
                //    //    //    {
                //    //    //        Dev.Log( "DebugFSMS GON -- " + gcr.gameObject.GameObject.Name );
                //    //    //    }
                //    //    //}


                //    //TODO: move all this debug crap into a function 

                //    {
                //        HutongGames.PlayMaker.Actions.FindGameObject y = owner.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.FindGameObject>("Initialise","attack_range_detect");

                //        if( y != null )
                //        {
                //            if( y.objectName != null )
                //                Dev.Log( "DebugFSMS FindGameObject objectName --- " + y.objectName.Value );
                //            if( y.withTag != null )
                //                Dev.Log( "DebugFSMS FindGameObject withTag --- " + y.withTag.Value );
                //            if( y.store != null )
                //                Dev.Log( "DebugFSMS FindGameObject store --- " + y.store.Name );
                //        }
                //    }

                //    {
                //        List<HutongGames.PlayMaker.Actions.GetPosition> y = owner.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.GetPosition>("Raycast","attack_range_detect");

                //        if( y != null )
                //        {
                //            foreach( var yy in y )
                //            {
                //                Dev.Log( "DebugFSMS GetPosition Name --- " + yy.gameObject.GameObject.Name );
                //                Dev.Log( "DebugFSMS GetPosition Value --- " + yy.gameObject.GameObject.Value );
                //            }
                //        }
                //    }

                //    {
                //        List<HutongGames.PlayMaker.Actions.SetFloatValue> y = owner.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetFloatValue>("Raycast","attack_range_detect");

                //        if( y != null )
                //        {
                //            foreach( var yy in y )
                //            {
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue?.Name );
                //            }
                //        }
                //    }

                //    {
                //        List<HutongGames.PlayMaker.Actions.FloatSubtract> y = owner.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.FloatSubtract>("Raycast","attack_range_detect");

                //        if( y != null )
                //        {
                //            foreach( var yy in y )
                //            {
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract?.Name );
                //            }
                //        }
                //    }

                //    {
                //        List<HutongGames.PlayMaker.Actions.SetVector2XY> y = owner.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetVector2XY>("Raycast","attack_range_detect");

                //        if( y != null )
                //        {
                //            foreach( var yy in y )
                //            {
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable.Name );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable?.Value );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value?.Name );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x?.Name );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y );
                //                Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y?.Name );
                //            }
                //        }
                //    }

                //    {
                //        HutongGames.PlayMaker.Actions.RayCast2d x = owner.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.RayCast2d>("Raycast","attack_range_detect");

                //        if( x != null && x.fromGameObject != null && x.fromGameObject.GameObject != null && x.fromGameObject.GameObject.Value != null )
                //        {
                //            //checkFSM = fsm;
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Name );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Value.transform.position );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.fromPosition );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.direction );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.space );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.distance );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.minDepth );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.maxDepth );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.hitEvent );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeDidHit );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitObject );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitPoint );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitNormal );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitDistance );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitFraction );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.repeatInterval );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.layerMask );
                //            Dev.Log( "DebugFSMS RayCast2d --- " + x.invertMask );
                //        }
                //    }










                //    //    //modifiedPrefab.PrintSceneHierarchyTree( true );
                //    //    //UnityEngine.SceneManagement.SceneManager.GetSceneByName( currentlyLoadingScene ).PrintHierarchy();
                //}

                bool isDead = IsEnemyDead(owner);
                //Dev.Log( "Is dead? " + isDead );
                
                //Dev.Log( "Position " + transform.position );

                //Dev.Log( "FSMDEBUG :::: FOREACH ON " + owner.name );
                foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
                {
                    if( p == null )
                        continue;

                    if( !fsmsOnObject.ContainsKey( p ) )
                    {
                        fsmsOnObject.Add( p, p.ActiveStateName );
                        if( logFSM )
                            Dev.Log( "Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
                    }
                    else if( p.ActiveStateName != fsmsOnObject[ p ] )
                    {
                        if( logFSM )
                            Dev.Log( "" + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[ p ] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ( ( p.Fsm != null && p.Fsm.LastTransition != null ) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE" ) + "]" );
                        fsmsOnObject[ p ] = p.ActiveStateName;
                    }

                    //force-send an event on this state if everything matches?
                    if( !string.IsNullOrEmpty( sendWakeEventsOnState ) && fsmName == p.FsmName && sendWakeEventsOnState == p.ActiveStateName )
                    {
                        if( p != null && wakeEvents != null )
                        {
                            foreach( string s in wakeEvents )
                            {
                                p.SendEvent( s );
                            }
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Start()
        {
            while( collider == null && owner == null )
            {
                yield return null;
            }

            if( logFSM )
            {
                List<GameObject> lines = new List<GameObject>();
                lines = Dev.CreateBoxOfLineRenderers( collider.bounds, Color.green, -2.1f, .01f );
                foreach(var go in lines )
                {
                    go.transform.SetParent( owner.transform );
                    go.transform.localPosition = Vector3.zero;
                }
            }

            if(monitorFSMStates)
            {
                StartCoroutine( DebugFSMS() );
            }
        }

        private void OnTriggerEnter2D( Collider2D collision )
        {
            if( monitorFSMStates )
                return;

            bool isPlayer = false;

            foreach( Transform t in collision.gameObject.GetComponentsInParent<Transform>() )
            {
                if( t.gameObject == HeroController.instance.gameObject )
                {
                    isPlayer = true;
                    break;
                }
            }

            if( !isPlayer )
            {
                Dev.Log( "Something not the player entered us!" );
                return;
            }

            Dev.Log( "Player entered our wake area! " );

            if( !string.IsNullOrEmpty( fsmName ) )
            {                
                PlayMakerFSM fsm = FSMUtility.LocateFSM( owner, fsmName );
                
                if( fsm != null && wakeEvents != null )
                {
                    foreach( string s in wakeEvents )
                    {
                        Dev.Log( "Sending event! " + s );
                        fsm.SendEvent( s );
                    }
                }
                else
                {
                    Dev.Log( "Could not find FSM!" );
                }
            }

            //remove this after waking up the enemy
            Destroy( gameObject );
        }
    }


    //parent this to the mage knight
    public class WakeUpMageKnight : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject mageKnight;

        private IEnumerator Start()
        {
            //Dev.Log( "Trying to load WakeUpMageKnight ");
            while( collider == null && mageKnight == null )
            {
                yield return null;
            }
            //Dev.Log( "Created waker for " + mageKnight.name );
            //Dev.Log( "Bounds " + collider.bounds );

            //Dev.CreateBoxOfLineRenderers( collider.bounds, Color.green, -2.1f, .01f );

            //Dev.Log( "Hero is at " + HeroController.instance.transform.position );
            //HeroController.instance.gameObject.PrintSceneHierarchyTree( true );
        }

        private void OnTriggerEnter2D( Collider2D collision )
        {
            bool isPlayer = false;

            foreach( Transform t in collision.gameObject.GetComponentsInParent<Transform>() )
            {
                if( t.gameObject == HeroController.instance.gameObject )
                {
                    isPlayer = true;
                    break;
                }
            }

            if( !isPlayer )
            {
                Dev.Log( "Something not the player entered us!" );
                return;
            }

            Dev.Log( "Player entered our wake area! " );

            PlayMakerFSM fsm = null;

            foreach( Component c in mageKnight.GetComponents<Component>() )
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
            else
            {
                Dev.Log( "Could not find Mage Knight FSM!" );
            }

            //remove this after waking up the enemy
            Destroy( gameObject );
        }
    }


    public class EnemyRandomizerLoader
    {
        public static EnemyRandomizerLoader Instance { get; private set; }

        CommunicationNode comms;

        EnemyRandomizerDatabase database;

        nv.Contractor databaseLoader;

        IEnumerator randomizerSceneProcessor = null;

        public bool DatabaseGenerated { get; private set; }

        int currentDatabaseIndex = 0;

        int loadCount = 0;

        string currentlyLoadingScene = "";

        bool IsLoadingDatabase {
            get {
                return randomizerSceneProcessor != null;
            }
        }            

        float databaseLoadProgress = 0f;
        float DatabaseLoadProgress {
            get {
                return databaseLoadProgress;
            }
            set {
                databaseLoadProgress = value;
                
                comms.Publish( new LoadingProgressEvent() { progress = value } );
            }
        }

        public EnemyRandomizerLoader( EnemyRandomizerDatabase database )
        {
            this.database = database;
        }

        public void Setup()
        {
            Instance = this;

            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;
            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged += PrintNextSceneToLoad;

            comms = new CommunicationNode();
            comms.EnableNode( this );
        }

        public void Unload()
        {
            comms.DisableNode();

            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;

            Instance = null;
        }


        //void PrintNextSceneToLoad( Scene from, Scene to )
        //{
        //    //For debugging
        //    Dev.Log( "Loading Scene [" + to.name + "]" );
        //}


        /// <summary>
        /// Initial workhorse funciton. Load all the enemy types in the game.
        /// </summary>
        /// 
        void LoadSceneData()
        {
            GameObject root = EnemyRandomizer.Instance.ModRoot;

            bool debugOnce = true;
            //iterate over the loaded scenes
            for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
            {
                Scene sceneToLoad = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                currentlyLoadingScene = sceneToLoad.name;
                Dev.Log( "Loading Scene [" + currentlyLoadingScene + "]" );

                GameObject[] rootGameObjects = sceneToLoad.GetRootGameObjects();
                foreach( GameObject go in rootGameObjects )
                {
                    //ignore the mod root
                    if( go.name == root.name )
                        continue;

                    if( sceneToLoad.buildIndex == 244 )
                    {
                        int indexOfEffectType = EnemyRandomizerDatabase.effectObjectNames.IndexOf(go.name);

                        if( indexOfEffectType >= 0 )
                        {
                            GameObject prefab = go;
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //special logic for certain enemies:
                            prefab = ModifyGameObjectPrefab( prefab );

                            //don't actually add this one
                            //database.loadedEffectPrefabs.Add( EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ], prefab );
                            Dev.Log( "Saving special enemy effect: " + prefab.name );
                        }//end if-enemy
                    }

                    //save off teleplanes
                    //if( sceneToLoad.buildIndex == 96 )
                    {
                        if(go.name.Contains( "Teleplanes" ) )
                        {
                            GameObject prefab = go;
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //don't actually add this one, keep it around because the mages need it
                            Dev.Log( "Saving special enemy effect: " + prefab.name );
                            continue;
                        }
                    }

                    bool isInEffectList = database.loadedEffectPrefabs.ContainsKey(go.name);
                    if( isInEffectList )
                        continue;

                    //load beam effects
                    if( sceneToLoad.buildIndex == 241 )
                    {
                        int indexOfEffectType = EnemyRandomizerDatabase.effectObjectNames.IndexOf(go.name);

                        if( indexOfEffectType >= 0 )
                        {
                            GameObject prefab = go;
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //special logic for certain enemies:
                            prefab = ModifyGameObjectPrefab( prefab );

                            database.loadedEffectPrefabs.Add( EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ], prefab );
                            Dev.Log( "Adding enemy effect: " + prefab.name + " to loaded effect list with search string " + EnemyRandomizerDatabase.effectObjectNames[ indexOfEffectType ] );
                        }//end if-enemy
                    }

                    foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
                    {
                        string name = t.gameObject.name;

                        if( name.IsSkipLoadingString() )
                            continue;

                        name = name.TrimGameObjectName();
                        
                        bool isInLoadedList = database.loadedEnemyPrefabNames.Contains(name);
                        if( isInLoadedList )
                            continue;

                        if( debugOnce && name.Contains( "Zombie Beam Miner" ) && !name.Contains( "Mega" ) )
                        {
                            debugOnce = false;
                            //t.gameObject.PrintSceneHierarchyTree( true );
                            sceneToLoad.PrintHierarchy( i );
                        }

                        int indexOfRandomizerEnemyType = EnemyRandomizerDatabase.enemyTypeNames.IndexOf(name);
                        
                        if( indexOfRandomizerEnemyType >= 0 && t.gameObject.IsGameEnemy() )
                        {
                            GameObject prefab = null;
                            if( name.Contains("Zombie Beam Miner") )
                            {
                                prefab = t.gameObject;
                            }
                            if( name ==  "Mage" )
                            {
                                prefab = t.gameObject;
                            }
                            else
                            {
                                prefab = GameObject.Instantiate(t.gameObject);
                            }
                            
                            prefab.SetActive( false );
                            GameObject.DontDestroyOnLoad( prefab );
                            prefab.transform.SetParent( root.transform );

                            //special logic for certain enemies:
                            prefab = ModifyGameObjectPrefab( prefab );

                            database.loadedEnemyPrefabs.Add( prefab );
                            database.loadedEnemyPrefabNames.Add( EnemyRandomizerDatabase.enemyTypeNames[indexOfRandomizerEnemyType] );
                            Dev.Log( "Adding enemy type: " + prefab.name + " to list with search string " + EnemyRandomizerDatabase.enemyTypeNames[ indexOfRandomizerEnemyType ] );                            
                        }//end if-enemy
                    }//end foreach transform in the root game objects
                }//end for each root game object
            }//iterate over all LOADED scenes
        }//end LoadSceneData()

        GameObject ModifyGameObjectPrefab( GameObject randoPrefab )
        {
            GameObject modifiedPrefab = randoPrefab;
            
            //Create a custom "wake up" base game object and put it on the mage knight
            if( modifiedPrefab.name.Contains( "Mage Knight" ) )
            {
                //BoxCollider2D mkCollider = modifiedPrefab.GetComponent<BoxCollider2D>();

                GameObject wakeUpRoot = new GameObject("MK Wake Up Object");
                wakeUpRoot.transform.SetParent( modifiedPrefab.transform );
                wakeUpRoot.transform.localPosition = Vector3.zero;

                wakeUpRoot.layer = 13; //try this
                wakeUpRoot.tag = modifiedPrefab.tag;

                BoxCollider2D box = wakeUpRoot.AddComponent<BoxCollider2D>();
                box.isTrigger = true;
                box.size = new Vector2( 40f, 20f );

                WakeUpMageKnight specialWakeUp = wakeUpRoot.AddComponent<WakeUpMageKnight>();
                specialWakeUp.collider = box;
                specialWakeUp.mageKnight = modifiedPrefab;
            }

            //if( modifiedPrefab.name.Contains( "Garden" ) )
            //{
            //    //    //this fixes the slash spider!
            //    //    //DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
            //    //    DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Mage", new List<string>() { "IN RANGE" }, new Vector2( 40f, 20f ));
            //    //    d.monitorFSMStates = true;
            //    //    //d.monitorFSMStates = true;
            //    //    //d.sendWakeEventsOnState = "Waiting";
            //    //    //d.logFSM = false;

            //    //    //HutongGames.PlayMaker.Actions.GetColliderRange gcr = modifiedPrefab.GetFSMActionOnState<HutongGames.PlayMaker.Actions.GetColliderRange>("Select Target","Mage");

            //    //    //Dev.Log( "PRINTING!!!" );
            //    //    //PlayMakerFSM fsm = FSMUtility.LocateFSM( modifiedPrefab, "Mage" );
            //    //    //if( fsm != null && gcr != null )
            //    //    //{
            //    //    //    if( gcr.gameObject != null )
            //    //    //    {
            //    //    //        Dev.Log( "DebugFSMS OO -- " + gcr.gameObject.OwnerOption );
            //    //    //        Dev.Log( "DebugFSMS GO -- " + gcr.gameObject.GameObject );
            //    //    //    }
            //    //    //    if( gcr.gameObject.GameObject != null )
            //    //    //    {
            //    //    //        Dev.Log( "DebugFSMS GON -- " + gcr.gameObject.GameObject.Name );
            //    //    //    }
            //    //    //}


            //    //TODO: move all this debug crap into a function 

            //    //{
            //    //    HutongGames.PlayMaker.Actions.FindGameObject y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.FindGameObject>("Initialise","attack_range_detect");

            //    //    if( y != null )
            //    //    {
            //    //        if( y.objectName != null )
            //    //            Dev.Log( "DebugFSMS FindGameObject objectName --- " + y.objectName.Value );
            //    //        if( y.withTag != null )
            //    //            Dev.Log( "DebugFSMS FindGameObject withTag --- " + y.withTag.Value );
            //    //        if( y.store != null )
            //    //            Dev.Log( "DebugFSMS FindGameObject store --- " + y.store.Name );
            //    //    }
            //    //}

            //    //{
            //    //    List<HutongGames.PlayMaker.Actions.GetPosition> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.GetPosition>("Raycast","attack_range_detect");

            //    //    if( y != null )
            //    //    {
            //    //        foreach( var yy in y )
            //    //        {
            //    //            Dev.Log( "DebugFSMS GetPosition Name --- " + yy.gameObject.GameObject.Name );
            //    //            Dev.Log( "DebugFSMS GetPosition Value --- " + yy.gameObject.GameObject.Value );
            //    //        }
            //    //    }
            //    //}

            //    //{
            //    //    List<HutongGames.PlayMaker.Actions.SetFloatValue> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetFloatValue>("Raycast","attack_range_detect");

            //    //    if( y != null )
            //    //    {
            //    //        foreach( var yy in y )
            //    //        {
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatValue?.Name );
            //    //        }
            //    //    }
            //    //}

            //    //{
            //    //    List<HutongGames.PlayMaker.Actions.FloatSubtract> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.FloatSubtract>("Raycast","attack_range_detect");

            //    //    if( y != null )
            //    //    {
            //    //        foreach( var yy in y )
            //    //        {
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable.Name );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.floatVariable?.Value );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.subtract?.Name );
            //    //        }
            //    //    }
            //    //}

            //    //{
            //    //    List<HutongGames.PlayMaker.Actions.SetVector2XY> y = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionsOnState<HutongGames.PlayMaker.Actions.SetVector2XY>("Raycast","attack_range_detect");

            //    //    if( y != null )
            //    //    {
            //    //        foreach( var yy in y )
            //    //        {
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable.Name );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Variable?.Value );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.vector2Value?.Name );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.x?.Name );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y );
            //    //            Dev.Log( "DebugFSMS " + yy.GetType().Name + "--- " + yy.y?.Name );
            //    //        }
            //    //    }
            //    //}

            //    //{
            //    //    HutongGames.PlayMaker.Actions.RayCast2d x = modifiedPrefab.FindGameObjectInChildren("Attack Range").GetFSMActionOnState<HutongGames.PlayMaker.Actions.RayCast2d>("Raycast","attack_range_detect");

            //    //    if( x != null )
            //    //    {
            //    //        //checkFSM = fsm;
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Name );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromGameObject.GameObject.Value.transform.position );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.fromPosition );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.direction );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.space );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.distance );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.minDepth );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.maxDepth );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.hitEvent );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeDidHit );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitObject );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitPoint );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitNormal );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitDistance );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.storeHitFraction );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.repeatInterval );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.layerMask );
            //    //        Dev.Log( "DebugFSMS RayCast2d --- " + x.invertMask );
            //    //    }
            //    //}










            //    //    //modifiedPrefab.PrintSceneHierarchyTree( true );
            //    //    //UnityEngine.SceneManagement.SceneManager.GetSceneByName( currentlyLoadingScene ).PrintHierarchy();
            //}

            //remove the "Cam Lock" game object child from the crystal guardian (mega zombie beam miner)
            if( modifiedPrefab.name.Contains( "Mega Zombie Beam Miner" ) )
            {
                modifiedPrefab.PrintSceneHierarchyTree( true );
                GameObject camLock = modifiedPrefab.FindGameObjectInChildren("Cam Lock");
                if( camLock != null )
                {
                    Dev.Log( "Marking Cam Lock for removal!" );
                    GameObject.Destroy( camLock );
                }

                PersistentBoolItem pbi = modifiedPrefab.GetComponent<PersistentBoolItem>();
                if( pbi != null )
                {
                    pbi.semiPersistent = true;
                    //pbi.persistentBoolData.activated = true;
                    //GameObject.Destroy( pbi );
                }


                //find and remove the FSM that kills the boss
                PlayMakerFSM deleteFSM = null;
                for(int i = 0; i < modifiedPrefab.GetComponentsInChildren<PlayMakerFSM>().Length; ++i )
                {
                    PlayMakerFSM fsm = modifiedPrefab.GetComponentsInChildren<PlayMakerFSM>()[i];
                    if(fsm.FsmName == "FSM")
                    {
                        foreach( var s in fsm.FsmStates )
                        {
                            if( s.Name == "Check" )
                            {
                                foreach( var a in s.Actions )
                                {
                                    if(a.GetType().Name == "PlayerDataBoolTest" )
                                    {
                                        deleteFSM = fsm;
                                        break;
                                    }
                                }

                                if( deleteFSM != null )
                                    break;
                            }
                        }
                    }

                    if( deleteFSM != null )
                        break;
                }

                //remove the persistant bool check item
                if( deleteFSM != null )
                {
                    GameObject.Destroy( deleteFSM );
                }
                
                DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Beam Miner", new List<string>() { "FINISHED" } );
                d.monitorFSMStates = true;
                d.sendWakeEventsOnState = "Beam End";
                d.logFSM = false;
            }

            //fix the beam miner FSM from getting stuck
            else if( modifiedPrefab.name.Contains( "Zombie Beam Miner" ) )
            {
                Dev.Log( "DebugFSMS added zombie beam miner" );
                DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Beam Miner", new List<string>() { "FINISHED" } );
                d.monitorFSMStates = true;
                d.sendWakeEventsOnState = "Beam End";
                d.logFSM = false;
            }

            //fix the slash spider from getting stuck
            if( modifiedPrefab.name.Contains( "Slash Spider" ) )
            {
                //this fixes the slash spider!
                DebugOnWake d = AddDebugOnWake(modifiedPrefab, "Slash Spider", new List<string>() { "WAKE" } );
                d.monitorFSMStates = true;
                d.sendWakeEventsOnState = "Waiting";
                d.logFSM = false;
            }
            
            //if( modifiedPrefab.name.Contains( "Garden Zombie" ) )
            //{
            //    //this fixes the slash spider!
            //    //DebugOnWake d = AddDebugOnWake(modifiedPrefab.FindGameObjectInChildren("Attack Range"), "attack_range_detect", new List<string>() { "WAKE" } );
            //    //d.monitorFSMStates = true;
            //    ////d.sendWakeEventsOnState = "Waiting";
            //    ////d.logFSM = false;

            //    //modifiedPrefab.PrintSceneHierarchyTree( true );
            //}

            return modifiedPrefab;
        }

        public void BuildEnemyRandomizerDatabase()
        {
            Dev.Where();
            randomizerSceneProcessor = DoBuildDatabase();

            databaseLoader = new nv.Contractor
            {
                OnUpdate = BuildDatabase,
                Looping = true
            };
            databaseLoader.SetUpdateRate( nv.Contractor.UpdateRateType.Frame );
            databaseLoader.Start();
        }

        public int GetSceneToLoadFromRandomizerData(int databaseIndex)
        {
            return EnemyRandomizerDatabase.EnemyTypeScenes[ databaseIndex ];
        }

        protected virtual void AdditivelyLoadCurrentScene()
        {
            try
            {
                //Dev.Log( "Additively loading scene " + GetSceneToLoadFromRandomizerData(currentDatabaseIndex));
                UnityEngine.SceneManagement.SceneManager.LoadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ), LoadSceneMode.Additive );
                loadCount++;
            }
            catch( Exception e )
            {
                Dev.Log( "Exception from scene: " + e.Message );
                PrintDebugLoadingError();
            }
        }

        protected virtual void IncrementCurrentSceneIndex()
        {
            currentDatabaseIndex += 1;
        }

        protected virtual void ProcessCurrentSceneForDataLoad()
        {
            try
            {
                Dev.Log( "Loading scene data: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                LoadSceneData();

                DatabaseLoadProgress = currentDatabaseIndex / (float)(EnemyRandomizerDatabase.EnemyTypeScenes.Count - 1);
                Dev.Log( "Loading Progress: " + DatabaseLoadProgress );

                Dev.Log( "Unloading scene: " + GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
                UnityEngine.SceneManagement.SceneManager.UnloadScene( GetSceneToLoadFromRandomizerData( currentDatabaseIndex ) );
            }
            catch( Exception e )
            {
                Dev.Log( "Exception from scene: " + e.Message );
                PrintDebugLoadingError();
            }
        }

        protected virtual void CompleteEnemyRandomizerDataLoad()
        {
            Dev.Log( "Loaded data from " + loadCount + " scenes." );
            //PrintDebugLoadingError();

            //TODO:uncomment
            //For debugging: print all the loaded enemies
            //foreach( GameObject go in database.loadedEnemyPrefabs )
            //{
            //    go.PrintSceneHierarchyTree( true );
            //}

            //For debugging: print all the loaded effects
            //foreach( var effect in database.loadedEffectPrefabs )
            //{
            //    effect.Value.PrintSceneHierarchyTree( true );
            //}


            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= PrintNextSceneToLoad;

            DatabaseGenerated = true;
            databaseLoader.Reset();
            randomizerSceneProcessor = null;

            GameManager.instance.LoadFirstScene();
        }

        protected virtual bool IsDoneLoadingRandomizerData()
        {
            return ( currentDatabaseIndex + 1) >= EnemyRandomizerDatabase.EnemyTypeScenes.Count;
        }

        protected virtual void BuildDatabase()
        {
            if( randomizerSceneProcessor != null && !randomizerSceneProcessor.MoveNext() )
            {
                randomizerSceneProcessor.Reset();
            }

            if( randomizerSceneProcessor != null && ( randomizerSceneProcessor.Current as bool? ) == false )
            {
                randomizerSceneProcessor.Reset();
            }
        }

        protected IEnumerator DoBuildDatabase()
        {
            while( !DatabaseGenerated )
            {
                AdditivelyLoadCurrentScene();
                
                //wait until all scenes are loaded
                for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; )
                {
                    bool status = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isLoaded;
                    if(!status)
                    {
                        i = 0;
                        yield return null;
                    }
                    else
                    {
                        ++i;
                    }
                }            

                ProcessCurrentSceneForDataLoad();

                yield return true;

                if( IsDoneLoadingRandomizerData() )
                {
                    CompleteEnemyRandomizerDataLoad();
                }
                else
                {
                    IncrementCurrentSceneIndex();
                }

                yield return true;
            }

            yield return false;
        }

        protected virtual void PrintDebugLoadingError()
        {
            bool printInitial = true;
            foreach( string enemy in EnemyRandomizerDatabase.enemyTypeNames )
            {
                if( database.loadedEnemyPrefabNames.Contains( enemy ) )
                    continue;

                if( printInitial )
                {
                    Dev.Log( "Enemies not loaded so far:" );
                    printInitial = false;
                }

                Dev.Log( "Missing type: " + enemy );
            }
        }



        public DebugOnWake AddDebugOnWake( GameObject enemy, string fsmName = "", List<string> wakeEvents = null, Vector2? customColliderSize = null )
        {
            GameObject wakeUpRoot = new GameObject(enemy.name + " DebugWake Object");
            wakeUpRoot.transform.SetParent( enemy.transform );
            wakeUpRoot.transform.localPosition = Vector3.zero;

            wakeUpRoot.layer = 13; //try this
            wakeUpRoot.tag = enemy.tag;

            BoxCollider2D box = wakeUpRoot.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            //box.size = new Vector2( 10f, 10f );

            if( !customColliderSize.HasValue )
            {
                BoxCollider2D pbox = enemy.GetComponent<BoxCollider2D>();
                if( pbox != null )
                {
                    box.size = pbox.size;
                    box.offset = pbox.offset;
                }
            }
            else
            {
                box.size = customColliderSize.Value;
            }

            DebugOnWake specialWakeUp = wakeUpRoot.AddComponent<DebugOnWake>();
            specialWakeUp.collider = box;
            specialWakeUp.owner = enemy;
            specialWakeUp.fsmName = fsmName;
            specialWakeUp.wakeEvents = wakeEvents;

            return specialWakeUp;
        }
    }
}
