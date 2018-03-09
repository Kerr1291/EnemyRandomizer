using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class Vector2Extensions
    {
        public static void Clamp( this Vector2 value, Vector2 min, Vector2 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
        }

        public static void Clamp01( this Vector2 value )
        {
            value.x = Mathf.Clamp( value.x, Vector2.zero.x, Vector2.one.x );
            value.y = Mathf.Clamp( value.y, Vector2.zero.y, Vector2.one.y );
        }

        ///NOTE: untested, please test
        public static void RotateToLocalSpace( this Vector2 input, Transform localSpace )
        {
            float angle = Mathf.Atan2(localSpace.forward.y, localSpace.forward.x) * Mathf.Rad2Deg;
            Quaternion pq = Quaternion.AngleAxis( angle, Vector3.forward );
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            input = new Vector3( pq.x, pq.y );
        }


        public static Vector2 Sign( Vector2 v )
        {
            v.x = Sign( v.x );
            v.y = Sign( v.y );
            return v;
        }

        public static void SetVectorComponent( ref Vector3 v, int component, float value )
        {
            v[ component ] = value;
        }

        public static void ClampToInt( ref Vector2 v )
        {
            v.x = (int)( v.x );
            v.y = (int)( v.y );
        }
    }
}
