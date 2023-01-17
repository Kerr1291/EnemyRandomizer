using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class Vector2Extensions
    {
        public static Vector3 ToVector3( this Vector2 value, float z = 0f )
        {
            Vector3 t = Vector3.zero;
            t.x = value.x;
            t.y = value.y;
            t.z = z;
            return t;
        }

        public static Vector2 Clamp( this Vector2 value, Vector2 min, Vector2 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
            return value;
        }

        public static Vector2 Clamp01( this Vector2 value )
        {
            value.x = Mathf.Clamp( value.x, Vector2.zero.x, Vector2.one.x );
            value.y = Mathf.Clamp( value.y, Vector2.zero.y, Vector2.one.y );
            return value;
        }

        ///NOTE: untested, please test
        public static Vector2 RotateToLocalSpace( this Vector2 input, Transform localSpace )
        {
            float angle = Mathf.Atan2(localSpace.forward.y, localSpace.forward.x) * Mathf.Rad2Deg;
            Quaternion pq = Quaternion.AngleAxis( angle, Vector3.forward );
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            input = new Vector3( pq.x, pq.y );
            return input;
        }


        public static Vector2 Sign( this Vector2 v )
        {
            Vector3 t = Vector3.zero;
            t.x = Mathf.Sign( v.x );
            t.y = Mathf.Sign( v.y );
            return t;
        }

        public static Vector2 Set( this Vector2 v, int componentIndex, float value )
        {
            v[ componentIndex ] = value;
            return v;
        }

        public static Vector2 SetX( this Vector2 v, float value )
        {
            v[ 0 ] = value;
            return v;
        }

        public static Vector2 SetY( this Vector2 v, float value )
        {
            v[ 1 ] = value;
            return v;
        }

        public static Vector2Int ToInt( this Vector2 v )
        {
            return Vector2Int.FloorToInt(v);
        }

        public static Vector3 VectorXZ( this Vector2 v )
        {
            return new Vector3( v.x, 0.0f, v.y );
        }

        public static Vector3 VectorXZ( this Vector2 v, float y )
        {
            return new Vector3( v.x, y, v.y );
        }

        public static Vector2Int ScaleBy( this Vector2Int v, float s )
        {
            return Vector2Int.FloorToInt(new Vector2(v.x, v.y) * s);
        }

        public static Vector2Int AddScalar(this Vector2Int v, int s)
        {
            return v + Vector2Int.one * s;
        }

        public static Vector2Int DivideBy(this Vector2Int v, float s)
        {
            return Vector2Int.FloorToInt(new Vector2(v.x, v.y) / s);
        }
    }
}
