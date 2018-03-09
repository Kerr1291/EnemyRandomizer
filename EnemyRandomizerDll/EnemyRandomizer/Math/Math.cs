using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nv;

namespace nv
{
    public static class Mathnv
    {
        public static int GetFirstFlippedBitIndex( byte b )
        {
            for( int i = 0; i < 8; ++i )
            {
                bool r = ((1 << i) & b) == 0;
                if( !r )
                    return i;
            }
            return -1;
        }

        public static int GetFirstFlippedBitIndex( int b )
        {
            for( int i = 0; i < 32; ++i )
            {
                bool r = ((1 << i) & b) == 0;
                if( !r )
                    return i;
            }
            return -1;
        }

        //can't remember the link, but taken from stack overflow or a unity article
        public static bool FastApproximately( float a, float b, float threshold )
        {
            return ( ( a - b ) < 0 ? ( ( a - b ) * -1 ) : ( a - b ) ) <= threshold;
        }

        public static Vector2 Clamp( Vector2 value, Vector2 min, Vector2 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );

            return value;
        }

        public static Vector2 Clamp01( Vector2 value )
        {
            value.x = Mathf.Clamp( value.x, Vector2.zero.x, Vector2.one.x );
            value.y = Mathf.Clamp( value.y, Vector2.zero.y, Vector2.one.y );

            return value;
        }

        public static Vector3 Clamp( Vector3 value, Vector3 min, Vector3 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
            value.z = Mathf.Clamp( value.z, min.z, max.z );

            return value;
        }

        public static Vector3 Clamp01( Vector3 value )
        {
            value.x = Mathf.Clamp( value.x, Vector3.zero.x, Vector3.one.x );
            value.y = Mathf.Clamp( value.y, Vector3.zero.y, Vector3.one.y );
            value.z = Mathf.Clamp( value.z, Vector3.zero.z, Vector3.one.z );

            return value;
        }

        public static Vector3 RotateToLocalSpace( Vector3 input, Transform localSpace )
        {
            Vector4 p = input;
            Quaternion pq = new Quaternion(p.x, p.y, p.z, 0);
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            Vector3 vout = new Vector3(pq.x, pq.y, pq.z);
            return vout;
        }


        public static void SetVectorComponent( ref Vector3 v, int component, float value )
        {
            v[ component ] = value;
        }

        public static int Sign( int v )
        {
            if( v < 0 )
                v = -1;
            else if( v > 0 )
                v = 1;
            else
                v = 0;
            return v;
        }

        public static float Sign( float v )
        {
            if( v < 0.0f )
                v = -1.0f;
            else if( v > 0.0f )
                v = 1.0f;
            else
                v = 0.0f;
            return v;
        }

        public static bool Contains( Vector2 pos, Vector2 min, Vector2 max )
        {
            if( pos.x < min.x )
                return false;
            if( pos.x >= max.x )
                return false;
            if( pos.y < min.y )
                return false;
            if( pos.y >= max.y )
                return false;
            return true;
        }

        public static bool Contains( int x, int y, Vector2 min, Vector2 max )
        {
            if( x < (int)min.x )
                return false;
            if( x >= (int)max.x )
                return false;
            if( y < (int)min.y )
                return false;
            if( y >= (int)max.y )
                return false;
            return true;
        }

        public static Vector2 RectTopLeft( Rect input )
        {
            return input.position;
        }

        public static Vector2 RectBottomRight( Rect input )
        {
            return new Vector2( input.xMax, input.yMax );
        }

        public static void Swap<T>( ref T lhs, ref T rhs )
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void Sort2<T>( ref T out_val0, ref T out_val1 ) where T : System.IComparable<T>
        {
            if( out_val0.CompareTo( out_val1 ) > 0 )
                Swap( ref out_val0, ref out_val1 );
        }

        public static void Clamp( ref Vector2 top_left, ref Vector2 bottom_right, Vector2 pos, Vector2 max_dimensions )
        {
            Sort2( ref top_left.x, ref bottom_right.x );
            Sort2( ref top_left.y, ref bottom_right.y );

            top_left.x = Mathf.Clamp( top_left.x, pos.x, pos.x + max_dimensions.x );
            top_left.y = Mathf.Clamp( top_left.y, pos.y, pos.y + max_dimensions.y );

            bottom_right.x = Mathf.Clamp( bottom_right.x, pos.x, pos.x + max_dimensions.x );
            bottom_right.y = Mathf.Clamp( bottom_right.y, pos.y, pos.y + max_dimensions.y );
        }

        public static void Clamp( ref Rect area, Vector2 pos, Vector2 max_dimensions )
        {
            area.x = Mathf.Max( area.x, pos.x );
            area.y = Mathf.Max( area.y, pos.y );
            area.width = Mathf.Min( area.xMax, pos.x + max_dimensions.x ) - area.x;
            area.height = Mathf.Min( area.yMax, pos.y + max_dimensions.y ) - area.y;
        }

        public static void Clamp( ref Rect area, Rect min_max )
        {
            area.x = Mathf.Max( area.x, min_max.x );
            area.y = Mathf.Max( area.y, min_max.y );
            area.width = Mathf.Min( area.xMax, min_max.xMax ) - area.x;
            area.height = Mathf.Min( area.yMax, min_max.yMax ) - area.y;
        }

        public static Rect Clamp( Rect area, Rect min_max )
        {
            area.x = Mathf.Max( area.x, min_max.x );
            area.y = Mathf.Max( area.y, min_max.y );
            area.width = Mathf.Min( area.xMax, min_max.xMax ) - area.x;
            area.height = Mathf.Min( area.yMax, min_max.yMax ) - area.y;
            return area;
        }

    }
}
