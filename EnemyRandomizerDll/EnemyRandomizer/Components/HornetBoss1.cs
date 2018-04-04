using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;

namespace EnemyRandomizerMod
{
    //TODO: move the wake components into their own classes....
    public class HornetBoss1 : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject owner;
        public HealthManager healthManager;
        public Rigidbody2D body;
        public MeshRenderer meshRenderer;

        //Dictionary = FSM and the current state it's in
        public Dictionary<PlayMakerFSM,string> fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

        //print debug bounding boxes and debug log info?
        public bool debug = true;

        private void OnDisable()
        {
            if( debug )
                Dev.Log( "DebugFSMS DebugOnWake was disabled, likely because the enemy died " );

            //final FSM info....
            foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
            {
                if( p == null )
                    continue;

                if( !fsmsOnObject.ContainsKey( p ) )
                {
                    fsmsOnObject.Add( p, p.ActiveStateName );
                    if( debug )
                        Dev.Log( "DebugFSMS :::: Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
                }
                else if( p.ActiveStateName != fsmsOnObject[ p ] )
                {
                    if( debug )
                        Dev.Log( "DebugFSMS :::: " + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[ p ] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ( ( p.Fsm != null && p.Fsm.LastTransition != null ) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE" ) + "]" );
                    fsmsOnObject[ p ] = p.ActiveStateName;
                }
            }
        }

        IEnumerator DebugFSMS()
        {
            fsmsOnObject = new Dictionary<PlayMakerFSM, string>();

            foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
            {
                fsmsOnObject.Add( p, p.ActiveStateName );
                if( debug )
                    Dev.Log( "Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
            }

            while( true )
            {
                if( owner == null )
                    yield break;

                foreach( var p in owner.GetComponentsInChildren<PlayMakerFSM>() )
                {
                    if( p == null )
                        continue;

                    if( !fsmsOnObject.ContainsKey( p ) )
                    {
                        fsmsOnObject.Add( p, p.ActiveStateName );
                        if( debug )
                            Dev.Log( "Added FSM for " + owner.name + " had the fsm [" + p.FsmName + "] with initial state [" + p.ActiveStateName + "]" );
                    }
                    else if( p.ActiveStateName != fsmsOnObject[ p ] )
                    {
                        if( debug )
                            Dev.Log( "" + owner.name + " had the fsm [" + p.FsmName + "] change FROM state [" + fsmsOnObject[ p ] + "] TO state [" + p.ActiveStateName + "] on EVENT [" + ( ( p.Fsm != null && p.Fsm.LastTransition != null ) ? p.Fsm.LastTransition.EventName : "GAME OBJECT AWAKE" ) + "]" );
                        fsmsOnObject[ p ] = p.ActiveStateName;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator Start()
        {
            owner = gameObject;
            collider = GetComponent<BoxCollider2D>();
            healthManager = GetComponent<HealthManager>();
            body = GetComponent<Rigidbody2D>();
            meshRenderer = GetComponent<MeshRenderer>();

            //remove her playmaker fsm for her AI
            //PlayMakerFSM deleteFSM = owner.GetMatchingFSMComponent("Control","G Dash","Tk2dPlayAnimation");
            ////remove the persistant bool check item
            //if( deleteFSM != null )
            //{
            //    GameObject.Destroy( deleteFSM );
            //}

            StartCoroutine( DebugFSMS() );
            StartCoroutine( MainAILoop() );

            yield break;
        }

        IEnumerator MainAILoop()
        {
            currentState = Init();

            for( ; ; )
            {
                if( owner == null )
                    yield break;

                yield return currentState;

                //TODO: remove as the states get implemented
                yield return new WaitForEndOfFrame();
            }
        }



        IEnumerator currentState = null;


        IEnumerator Init()
        {
            body.gravityScale = 1.5f;

            currentState = Inert();

            yield break;
        }


        IEnumerator Inert()
        {
            int test = GameManager.instance.playerData.GetInt("hornetGreenpath");

            if(test >= 4)
            {
                currentState = Wake();
            }
            else
            {
                currentState = Inert();
            }

            yield break;
        }

        IEnumerator RefightReady()
        {
            //TODO

            yield break;
        }

        IEnumerator Wake()
        {
            body.isKinematic = false;
            collider.enabled = true;
            meshRenderer.enabled = true;
            transform.localScale.SetX( -1f );
            collider.offset = new Vector2( .1f, -.3f );
            collider.size = new Vector2( .9f, 2.6f );

            currentState = Flourish();

            yield break;
        }

        IEnumerator Flourish()
        {


            yield break;
        }

        IEnumerator FaceObject(GameObject objectToFace)
        {


            yield break;
        }
    }
}







//force-send an event on this state if everything matches?
//if( !string.IsNullOrEmpty( sendWakeEventsOnState ) && fsmName == p.FsmName && sendWakeEventsOnState == p.ActiveStateName )
//{
//    if( p != null && wakeEvents != null )
//    {
//        foreach( string s in wakeEvents )
//        {
//            p.SendEvent( s );
//        }
//    }
//}




//private void OnTriggerEnter2D( Collider2D collision )
//{
//    if( monitorFSMStates )
//        return;

//    bool isPlayer = false;

//    foreach( Transform t in collision.gameObject.GetComponentsInParent<Transform>() )
//    {
//        if( t.gameObject == HeroController.instance.gameObject )
//        {
//            isPlayer = true;
//            break;
//        }
//    }

//    if( !isPlayer )
//    {
//        Dev.Log( "Something not the player entered us!" );
//        return;
//    }

//    Dev.Log( "Player entered our wake area! " );

//    if( !string.IsNullOrEmpty( fsmName ) )
//    {
//        PlayMakerFSM fsm = FSMUtility.LocateFSM( owner, fsmName );

//        if( fsm != null && wakeEvents != null )
//        {
//            foreach( string s in wakeEvents )
//            {
//                Dev.Log( "Sending event! " + s );
//                fsm.SendEvent( s );
//            }
//        }
//        else
//        {
//            Dev.Log( "Could not find FSM!" );
//        }
//    }

//    //remove this after waking up the enemy
//    Destroy( gameObject );
//}