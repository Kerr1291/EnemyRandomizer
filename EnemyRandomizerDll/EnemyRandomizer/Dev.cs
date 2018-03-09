using UnityEngine;
using System.Collections.Generic;

namespace nv
{
    /// <summary>
    /// Collection of tools, debug or otherwise, to improve the quality of life
    /// </summary>
    public partial class Dev
    {
        public static bool IsLayerInMask( LayerMask mask, GameObject go )
        {
            return IsLayerInMask( mask, go.layer );
        }

        public static bool IsLayerInMask( LayerMask mask, int layerID )
        {
            return ( mask.value & ( 1 << layerID ) ) > 0;
        }

        public static T GetRandomElementFromList<T>( List<T> list )
        {
            int index = GameRNG.Rand(0, list.Count);
            return list[ index ];
        }

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

        public static T Instantiate<T>( GameObject obj, Transform parent ) where T : MonoBehaviour
        {
#if UNITY_5_5_OR_NEWER
            //Core: Using Object.Instantiate(Object, Transform) will now use the Object position as the local position by default. 
            //This is a change from the 5.4 behaviour, which used the Object position as world.
            GameObject g = (GameObject)GameObject.Instantiate(obj,parent);
#else
            GameObject g = (GameObject)GameObject.Instantiate(obj,parent,false);
#endif
            return g.GetComponent<T>();
        }

        public static bool FastApproximately( float a, float b, float threshold )
        {
            return ( ( a - b ) < 0 ? ( ( a - b ) * -1 ) : ( a - b ) ) <= threshold;
        }

        public static T GetOrAddComponent<T>( GameObject source ) where T : UnityEngine.Component
        {
            T result = source.GetComponent<T>();
            if( result != null )
                return result;
            result = source.AddComponent<T>();
            return result;
        }

        public static void GetOrAddComponentIfNull<T>( ref T result, GameObject source ) where T : UnityEngine.Component
        {
            if( result != null )
                return;
            result = source.GetComponent<T>();
            if( result != null )
                return;
            result = source.AddComponent<T>();
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

        public static void Clamp( ref Vector2 value, Vector2 min, Vector2 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
        }

        public static void Clamp01( ref Vector2 value )
        {
            value.x = Mathf.Clamp( value.x, Vector2.zero.x, Vector2.one.x );
            value.y = Mathf.Clamp( value.y, Vector2.zero.y, Vector2.one.y );
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

        public static void ClampToInt( ref Vector2 v )
        {
            v.x = (int)( v.x );
            v.y = (int)( v.y );
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

        public static Vector2 Sign( Vector2 v )
        {
            v.x = Sign( v.x );
            v.y = Sign( v.y );
            return v;
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

        public static Vector3 RotateToLocalSpace( Vector3 input, Transform localSpace )
        {
            Vector4 p = input;
            Quaternion pq = new Quaternion(p.x, p.y, p.z, 0);
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            Vector3 vout = new Vector3(pq.x, pq.y, pq.z);
            return vout;
        }

        //Unity 5.2 and onward removed ToHexStringRGB in favor of the ColorUtility class methods
        public static string ColorToHex( Color color )
        {
#if UNITY_5_1
        return color.ToHexStringRGB();
#else
            return ColorUtility.ToHtmlStringRGB( color );
#endif
        }

        public static string HexString( int val )
        {
            return val.ToString( "X" );
        }

        public static string ColorStr( int r, int g, int b )
        {
            return Dev.HexString( r ) + Dev.HexString( g ) + Dev.HexString( b );
        }

        public static string ColorStr( float r, float g, float b )
        {
            return Dev.HexString( (int)( 255.0f * Mathf.Clamp01( r ) ) ) + Dev.HexString( (int)( 255.0f * Mathf.Clamp01( g ) ) ) + Dev.HexString( (int)( 255.0f * Mathf.Clamp01( b ) ) );
        }

        public static Texture2D RectArrayToTexture<T>( T[,] data, System.Func<T,Color> toColor = null )
        {
            Texture2D tex = new Texture2D( (int)data.Length, (int)data.Rank, TextureFormat.ARGB32, false, false );
            tex.filterMode = FilterMode.Point;
            for( int j = 0; j < (int)data.Rank; ++j )
            {
                for( int i = 0; i < (int)data.Length; ++i )
                {
                    if( toColor == null )
                    {
                        if( data[ i, j ] != null )
                        {
                            tex.SetPixel( i, j, Color.red );
                        }
                        else
                        {
                            tex.SetPixel( i, j, Color.black );
                        }
                    }
                    else
                    {
                        tex.SetPixel( i, j, toColor(data[ i, j ]) );
                    }
                }
            }
            tex.Apply();
            return tex;
        }
    }

    public static void DebugCreateLine( Vector2 from, Vector2 to, Color c )
    {
        GameObject lineObj = new GameObject("line: "+from+" -> "+to);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.SetVertexCount( 2 );
        Vector3[] points = new Vector3[]{from,to};
        lr.SetPositions( points );
        lr.SetWidth( .5f, .1f );
        if( lr.GetComponent<Renderer>() )
            lr.GetComponent<Renderer>().material = new Material( Shader.Find( "Diffuse" ) );
        if( lr.GetComponent<Renderer>() )
            lr.GetComponent<Renderer>().material.color = c;
        lr.SetColors( c, c );
    }


}

