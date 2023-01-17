using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public static class ByteExtensions
    {
        public static int GetFirstFlippedBitIndex(this byte b)
        {
            for(int i = 0; i < 8; ++i)
            {
                bool r = ((1 << i) & b) == 0;
                if(!r)
                    return i;
            }
            return -1;
        }
    }
}