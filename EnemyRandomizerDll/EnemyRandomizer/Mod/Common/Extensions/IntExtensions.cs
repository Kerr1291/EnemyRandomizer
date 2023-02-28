using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public static class IntExtensions
    {
        public static string ToHexString(this int val)
        {
            return val.ToString("X");
        }

        public static int GetFirstFlippedBitIndex(this int b)
        {
            for(int i = 0; i < 32; ++i)
            {
                bool r = ((1 << i) & b) == 0;
                if(!r)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Mathematical modulus, different from the % operation that returns the remainder.
        /// This performs a "wrap around" of the given value assuming the range [0, mod)
        /// Define mod 0 to return the value unmodified
        /// </summary>
        public static int Modulus(this int value, int mod)
        {
            if(value > 0)
                return (value % mod);
            else if(value < 0)
                return (value % mod + mod) % mod;
            else
                return value;
        }
    }
}