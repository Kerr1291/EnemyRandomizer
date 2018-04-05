using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class Vector3Extensions
    {
        public static void Clamp( this Vector3 value, Vector3 min, Vector3 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
            value.z = Mathf.Clamp( value.z, min.z, max.z );
        }

        public static void Clamp01( this Vector3 value )
        {
            value.x = Mathf.Clamp( value.x, Vector3.zero.x, Vector3.one.x );
            value.y = Mathf.Clamp( value.y, Vector3.zero.y, Vector3.one.y );
            value.z = Mathf.Clamp( value.z, Vector3.zero.z, Vector3.one.z );
        }

        public static void RotateToLocalSpace( this Vector3 input, Transform localSpace )
        {
            Vector4 p = input;
            Quaternion pq = new Quaternion(p.x, p.y, p.z, 0);
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            input = new Vector3(pq.x, pq.y, pq.z);
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
            t.x = Mathnv.Sign( v.x );
            t.y = Mathnv.Sign( v.y );
            t.z = Mathnv.Sign( v.z );
            return t;
        }

        public static void ToInt( this Vector3 v )
        {
            v.x = (int)( v.x );
            v.y = (int)( v.y );
            v.z = (int)( v.z );
        }
    }
}
