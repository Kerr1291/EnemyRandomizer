using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{    public static class Vector3Extensions
    {
        public static Vector3 Min(this IEnumerable<Vector3> set)
        {
            Vector3 min = new Vector3(set.Select(y => y.x).Min(), set.Select(y => y.y).Min(), set.Select(y => y.z).Min());
            return min;
        }

        public static Vector3 Max(this IEnumerable<Vector3> set)
        {
            Vector3 max = new Vector3(set.Select(y => y.x).Max(), set.Select(y => y.y).Max(), set.Select(y => y.z).Max());
            return max;
        }

        public static Vector3 Clamp( this Vector3 value, Vector3 min, Vector3 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
            value.z = Mathf.Clamp( value.z, min.z, max.z );
            return value;
        }

        public static Vector3 Clamp01( this Vector3 value )
        {
            value.x = Mathf.Clamp( value.x, Vector3.zero.x, Vector3.one.x );
            value.y = Mathf.Clamp( value.y, Vector3.zero.y, Vector3.one.y );
            value.z = Mathf.Clamp( value.z, Vector3.zero.z, Vector3.one.z );
            return value;
        }

        public static Vector3 RotateToLocalSpace( this Vector3 input, Transform localSpace )
        {
            Vector4 p = input;
            Quaternion pq = new Quaternion(p.x, p.y, p.z, 0);
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            input = new Vector3(pq.x, pq.y, pq.z);
            return input;
        }


        public static Vector3 Set( this Vector3 v, int componentIndex, float value )
        {
            v[ componentIndex ] = value;
            return v;
        }

        public static Vector3 SetX( this Vector3 v, float value )
        {
            v[ 0 ] = value;
            return v;
        }

        public static Vector3 SetY( this Vector3 v, float value )
        {
            v[ 1 ] = value;
            return v;
        }

        public static Vector3 SetZ( this Vector3 v, float value )
        {
            v[ 2 ] = value;
            return v;
        }

        public static Vector3 Sign( this Vector3 v )
        {
            Vector3 t = Vector3.zero;
            t.x = Mathf.Sign( v.x );
            t.y = Mathf.Sign( v.y );
            t.z = Mathf.Sign( v.z );
            return t;
        }

        public static Vector3Int ToInt( this Vector3 v )
        {
            return Vector3Int.FloorToInt(v);
        }
    }
}
