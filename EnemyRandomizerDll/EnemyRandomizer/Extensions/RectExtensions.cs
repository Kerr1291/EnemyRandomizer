using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class RectExtensions
    {
        public static Vector2 TopLeft( this Rect input )
        {
            return input.position;
        }

        public static Vector2 TopRight( this Rect input )
        {
            return new Vector2( input.xMax, input.position.y );
        }

        public static Vector2 BottomRight( this Rect input )
        {
            return new Vector2( input.xMax, input.yMax );
        }

        public static Vector2 BottomLeft( this Rect input )
        {
            return new Vector2( input.position.x, input.yMax );
        }

        public static void Clamp( this Rect area, Vector2 pos, Vector2 extents )
        {
            Mathnv.Clamp( ref area, pos, extents );
        }

        public static void Clamp( this Rect area, Rect min_max )
        {
            Mathnv.Clamp( ref area, min_max );
        }
    }
}
