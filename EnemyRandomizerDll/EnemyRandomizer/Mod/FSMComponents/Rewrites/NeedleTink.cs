using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class NeedleTink : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.GetComponent<Collider2D>().offset = new Vector2( 0f, -.3f );
        }

        public void SetParent( Transform t )
        {
            RemoveDeprecatedComponents();
            //if deparenting, hide the parent
            if( t == null )
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
                //if(transform.parent != null)
                //    transform.parent.gameObject.SetActive( false );
            }
            else
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            }

            gameObject.transform.SetParent( t );
            gameObject.transform.localPosition = Vector2.zero;
        }

        protected virtual void RemoveDeprecatedComponents()
        {
            foreach( PlayMakerFSM p in gameObject.GetComponentsInChildren<PlayMakerFSM>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerUnity2DProxy p in gameObject.GetComponentsInChildren<PlayMakerUnity2DProxy>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( PlayMakerFixedUpdate p in gameObject.GetComponentsInChildren<PlayMakerFixedUpdate>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( DeactivateIfPlayerdataTrue p in gameObject.GetComponentsInChildren<DeactivateIfPlayerdataTrue>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTweenFSMEvents p in gameObject.GetComponentsInChildren<iTweenFSMEvents>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in gameObject.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
            foreach( iTween p in gameObject.GetComponentsInChildren<iTween>( true ) )
            {
                GameObject.DestroyImmediate( p );
            }
        }
    }
}
