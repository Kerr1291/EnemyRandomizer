using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyRandomizerMod
{
    public static class IComparableExtensions
    {
        public static bool IsBetween<T>( this T cmp, T lower, T upper, bool inclusiveOfUpperValue = true ) where T : IComparable<T>
        {
            if( inclusiveOfUpperValue )
            {
                return cmp.CompareTo( lower ) >= 0 && cmp.CompareTo( upper ) <= 0;
            }
            return cmp.CompareTo( lower ) >= 0 && cmp.CompareTo( upper ) < 0;
        }

        public static T ConstrainBetween<T>( this T cmp, T minValue, T maxValue ) where T : IComparable<T>
        {
            if( cmp.CompareTo( minValue ) < 0 )
            {
                return minValue;
            }
            if( cmp.CompareTo( maxValue ) > 0 )
            {
                return maxValue;
            }
            return cmp;
        }
    }
}
