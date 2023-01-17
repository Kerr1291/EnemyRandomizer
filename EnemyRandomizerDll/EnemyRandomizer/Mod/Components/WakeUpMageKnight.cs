using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nv;
 

namespace EnemyRandomizerMod
{
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
}
