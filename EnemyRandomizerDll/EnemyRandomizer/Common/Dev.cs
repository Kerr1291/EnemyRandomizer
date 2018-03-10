using UnityEngine;
using System.Collections.Generic;

namespace nv
{
    /// <summary>
    /// Collection of tools, debug or otherwise, to improve the quality of life
    /// </summary>
    public partial class Dev
    {
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
}

