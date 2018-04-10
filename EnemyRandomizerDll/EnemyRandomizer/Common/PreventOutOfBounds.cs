using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class PreventOutOfBounds : MonoBehaviour
    {
        Vector3 previousLocation;
        Rigidbody2D body;
        BoxCollider2D bodyCollider;

        private void OnEnable()
        {
            previousLocation = transform.position;
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<BoxCollider2D>();
        }

        private void LateUpdate()
        {
            Vector3 currentLocation = transform.position;
            Vector3 movementVector = (currentLocation - previousLocation);
            float distance = movementVector.magnitude;

            if( distance <= Mathf.Epsilon )
                return;

            var result = Physics2D.Raycast( previousLocation, movementVector.normalized, distance, 1 << 8 );
            if( result.collider != null )
            {
                //somehow we passed through a wall, fix it
                //Dev.Log( "Out of bounds prevention triggered!" );

                Vector3 collisionPoint = result.point;
                Vector3 collisionNormal = result.normal;

                Vector3 size = bodyCollider.size;

                transform.position = collisionPoint + new Vector3( collisionNormal.x * size.x * 0.5f, collisionNormal.y * size.y * 0.5f );
                previousLocation = transform.position;
            }
            else
            {
                previousLocation = currentLocation;
            }
        }
    }
}
