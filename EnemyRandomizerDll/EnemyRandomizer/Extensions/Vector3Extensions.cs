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


        public static Vector3 VectorXZ( int x, int y )
        {
            return new Vector3( x, 0, y );
        }

        public static Vector3 VectorXZ( float x, float y )
        {
            return new Vector3( x, 0f, y );
        }

        public static Vector3 VectorXZ( Vector2 v )
        {
            return new Vector3( v.x, 0.0f, v.y );
        }

        public static Vector3 VectorXZ( Vector2 v, float y )
        {
            return new Vector3( v.x, y, v.y );
        }

        public static Vector3 VectorXZ( Vector3 v )
        {
            return new Vector3( v.x, 0.0f, v.z );
        }

        public static void VectorXZ( ref Vector3 v )
        {
            v.y = 0f;
        }

        public static void XYtoXZ( ref Vector3 v )
        {
            v.z = v.y;
            v.y = 0f;
        }

        public static void XYtoXZ( ref Vector3 v, float y )
        {
            v.z = v.y;
            v.y = y;
        }

        public static void SetVectorComponent( ref Vector3 v, int component, float value )
        {
            v[ component ] = value;
        }
    }
}
