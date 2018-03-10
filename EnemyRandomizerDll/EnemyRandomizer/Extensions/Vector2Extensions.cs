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


        public static Vector2 Sign( this Vector2 v )
        {
            Vector3 t = Vector3.zero;
            t.x = Mathnv.Sign( v.x );
            t.y = Mathnv.Sign( v.y );
            return t;
        }

        public static void Set( this Vector2 v, int componentIndex, float value )
        {
            v[ componentIndex ] = value;
        }

        public static void SetX( this Vector2 v, float value )
        {
            v[ 0 ] = value;
        }

        public static void SetY( this Vector2 v, float value )
        {
            v[ 1 ] = value;
        }

        public static void ToInt( this Vector2 v )
        {
            v.x = (int)( v.x );
            v.y = (int)( v.y );
        }

        public static Vector3 VectorXZ( this Vector2 v )
        {
            return new Vector3( v.x, 0.0f, v.y );
        }

        public static Vector3 VectorXZ( this Vector2 v, float y )
        {
            return new Vector3( v.x, y, v.y );
        }
    }
}
