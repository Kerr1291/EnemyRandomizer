using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public static class RectExtensions
    {
        public static Rect SetMinMax(this Rect input, Vector2 min, Vector2 max)
        {
            Vector2 rMin = Mathnv.Min(min, max);
            Vector2 rMax = Mathnv.Max(min, max);

            Rect r = new Rect((rMax - rMin) * .5f + rMin, rMax - rMin);
            return r;
        }

        public static Vector2 TopLeft(this Rect input, bool flipYAxis = true)
        {
            return new Vector2(input.xMin, flipYAxis ? input.yMin : input.yMax);
        }

        public static Vector2 TopRight(this Rect input, bool flipYAxis = true)
        {
            return new Vector2(input.xMax, flipYAxis ? input.yMin : input.yMax);
        }

        public static Vector2 BottomRight(this Rect input, bool flipYAxis = true)
        {
            return new Vector2(input.xMax, flipYAxis ? input.yMax : input.yMin);
        }

        public static Vector2 BottomLeft(this Rect input, bool flipYAxis = true)
        {
            return new Vector2(input.xMin, flipYAxis ? input.yMax : input.yMin);
        }

        public static void Clamp(this Rect area, Vector2 pos, Vector2 size)
        {
            Mathnv.Clamp(ref area, pos, size);
        }

        public static void Clamp(this Rect area, Rect min_max)
        {
            Mathnv.Clamp(ref area, min_max);
        }

        public static Range GetXRange(this Rect r)
        {
            return new Range(r.xMin, r.xMax);
        }

        public static Range GetYRange(this Rect r)
        {
            return new Range(r.yMin, r.yMax);
        }

        public static bool GetIntersectionRect( this Rect r1, Rect r2, out Rect area )
        {
            area = default( Rect );
            if( r2.Overlaps( r1 ) )
            {
                float num = Mathf.Min( r1.xMax, r2.xMax );
                float num2 = Mathf.Max( r1.xMin, r2.xMin );
                float num3 = Mathf.Min( r1.yMax, r2.yMax );
                float num4 = Mathf.Max( r1.yMin, r2.yMin );
                area.x = Mathf.Min( num, num2 );
                area.y = Mathf.Min( num3, num4 );
                area.width = Mathf.Max( 0f, num - num2 );
                area.height = Mathf.Max( 0f, num3 - num4 );
                return true;
            }
            return false;
        }
    }
}
