using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

using UnityEngine;
using ModCommon;

//-----------------------------
namespace EnemyRandomizerMod
{
    //public enum ExitDirection
    //{
    //    Up, Down, Left, Right, In
    //}

    //public class Gate
    //{
    //    public string name;
    //    public ExitDirection dir;
    //    public int requirements;
    //}

    //public class Requirements
    //{
    //    static public int None = 0;
    //    static public int Dash = 1;
    //    static public int Wall = 2;
    //    static public int Crystal = 4;
    //    static public int Double = 8;
    //    static public int Acid = 16;
    //    static public int Shade = 32;
    //    static public int Smash = 64;
    //    static public int Impossible = 128;
    //    // 100100
    //}
    
    public class Roguelike
    {
        RNG rng;

        World world;

        //map
        MapNode currentArea = null;
        MapNode previousArea = null;

        WorldInfo.TransitionInfo lastEntrance;
        WorldInfo.TransitionInfo lastExit;

        //Dictionary<string,MapNode> visited = new Dictionary<string, MapNode>();


        /*
         *  //hook into this to start the randomizing of a new scene (sceneload is in game manager)
         *
         * 
    this.sceneLoad.Finish += delegate
    {
        this.$this.sceneLoad = null;
        this.$this.isLoading = false;
        this.$this.waitForManualLevelStart = false;
        info.NotifyFinished();
        this.$this.OnNextLevelReady();
    };
         * 
         * 
         * */

        public IEnumerator Init( int seed )
        {
            Dev.Log( "Loading!" );   
            //TODO: pick a better way to know when it's safe to do this
            yield return new WaitForSeconds( 5f ); 

            while(GameManager.instance.IsNonGameplayScene())
                yield return new WaitForEndOfFrame();

            Dev.Where();

            rng = new RNG( seed );

            ResetPlayerData();

            world = new World();
            currentArea = World.Map[ "Town" ];

            Dev.Log( "Transitioning!" );
            yield return ModCommon.Tools.EnterZone( "Town", "left1", "door"  ); 

            yield return new WaitForSeconds( 2f );

            SetupTown();

            //add the hooks
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChange;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChange;
        }

        void ResetPlayerData()
        {
            //TODO: reset all the flags on playerdata
        }

        void SetupTown()
        {
            //TODO:
            RandomizeTransitionsInCurrentScene();
        }

        void OnSceneChange( Scene from, Scene to )
        {
            foreach(var t in transitionFixers )
            {
                if(t != null)
                    GameManager.instance.StopCoroutine( t );
            }
            transitionFixers.Clear();

            UpdateCurrentArea(to);

            UpdateEntranceToPointToExit();

            RandomizeTransitionsInCurrentScene();
        }

        void UpdateCurrentArea(Scene next)
        {
            previousArea = currentArea;
            currentArea = World.Map[ next.name ];
        }

        void UpdateEntranceToPointToExit()
        {
            lastExit = previousArea.GetTransitionWithName( TransitionPoint.lastEntered );

            lastEntrance = currentArea.GetTransitionWithName( lastExit.DestinationDoorName );

            GameObject transitionObject = GameObject.Find( lastEntrance.DoorName );

            TransitionPoint transition = transitionObject.GetComponent<TransitionPoint>();

            transition.targetScene = lastExit.DestinationSceneName;
            transition.entryPoint = lastExit.DoorName;
        }

        List<IEnumerator> transitionFixers = new List<IEnumerator>();

        void RandomizeTransitionsInCurrentScene()
        {
            foreach(var t in GameObject.FindObjectsOfType<TransitionPoint>())
            {
                Dev.Log( "Found " + t.name + " with transiton to " + t.targetScene + " with entry point " + t.entryPoint );
            }

            foreach(var connection in currentArea.scene.Transitions)
            {
                Dev.Log( "Randomizing " + connection.DoorName );
                GameObject transitionObject = GameObject.Find( connection.DoorName );

                if( transitionObject == null )
                {
                    Dev.Log( "transitionObject is null! " + connection.DoorName );
                    continue;
                }
                Dev.Log( "Found? " + transitionObject );
                Dev.Log( "Found? " + transitionObject.name );

                TransitionPoint transition = transitionObject.GetComponent<TransitionPoint>();

                if( transition == null )
                {
                    Dev.Log( "TransitionPoint is null! " + connection.DoorName );
                    continue;
                }
                Dev.Log( "Transition has " + transition.entryPoint );
                Dev.Log( "Transition goes to " + transition.targetScene );

                var areas = World.Map.Values.ToList();
                var nextArea = areas.GetRandomElementFromList();

                int saver = 0;
                bool selected = false;
                while(!selected)
                {
                    bool hasMatchingDoor = false;
                    string oldEntryPoint = transition.entryPoint;

                    if( nextArea.connections.Count > 1 )
                    {
                        if( transition.entryPoint.Contains( "right" ) )
                        {
                            var selectedTransitions = nextArea.GetTransitionsNamesContaining("right");
                            if( selectedTransitions.Count > 0 )
                            {
                                hasMatchingDoor = true;
                                var chosen = selectedTransitions.GetRandomElementFromList();
                                transition.entryPoint = chosen;
                            }
                        }
                        else if( transition.entryPoint.Contains( "left" ) )
                        {
                            var selectedTransitions = nextArea.GetTransitionsNamesContaining("left");
                            if( selectedTransitions.Count > 0 )
                            {
                                hasMatchingDoor = true;
                                var chosen = selectedTransitions.GetRandomElementFromList();
                                transition.entryPoint = chosen;
                            }
                        }
                        else if( transition.entryPoint.Contains( "top" ) )
                        {
                            var selectedTransitions = nextArea.GetTransitionsNamesContaining("top");
                            if( selectedTransitions.Count > 0 )
                            {
                                hasMatchingDoor = true;
                                var chosen = selectedTransitions.GetRandomElementFromList();
                                transition.entryPoint = chosen;
                            }
                        }
                        else if( transition.entryPoint.Contains( "bot" ) )
                        {
                            var selectedTransitions = nextArea.GetTransitionsNamesContaining("bot");
                            if( selectedTransitions.Count > 0 )
                            {
                                hasMatchingDoor = true;
                                var chosen = selectedTransitions.GetRandomElementFromList();
                                transition.entryPoint = chosen;
                            }
                        }
                        else if( transition.entryPoint.Contains( "door" ) )
                        {
                            var selectedTransitions = nextArea.GetTransitionsNamesContaining("door");
                            if( selectedTransitions.Count > 0 )
                            {
                                hasMatchingDoor = true;
                                var chosen = selectedTransitions.GetRandomElementFromList();
                                transition.entryPoint = chosen;
                            }
                        }
                        else
                        {
                            hasMatchingDoor = false;
                        }

                    }
                    
                    if( hasMatchingDoor )
                    {
                        Dev.Log( oldEntryPoint + " entry point changed to " + transition.entryPoint );
                        selected = true;
                    }
                    else
                    { 
                        nextArea = areas.GetRandomElementFromList();
                    }

                    if( saver > 1000 )
                    { 
                        Dev.LogError( "Saver triggered!" );
                        break;
                    }
                    saver++;
                }

                Dev.Log( transition.targetScene + " transition changed to " + nextArea.scene.SceneName );

                transition.targetScene = nextArea.scene.SceneName;
                IEnumerator fixer = FixTransition(transition,nextArea.scene.SceneName,transition.entryPoint);
                transitionFixers.Add( fixer );
                GameManager.instance.StartCoroutine( fixer ); 
            }
        }

        IEnumerator FixTransition(TransitionPoint gate, string setScene, string setGate)
        {
            for( ; ;)
            {
                if( gate == null )
                    break;

                gate.targetScene = setScene;
                gate.entryPoint = setGate;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
