using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public static class ColorExtensions
    {
        //Unity 5.2 and onward removed ToHexStringRGB in favor of the ColorUtility class methods
        public static string ColorToHex(this Color color)
        {
#if UNITY_5_1
        return color.ToHexStringRGB();
#else
            return ColorUtility.ToHtmlStringRGB(color);
#endif
        }
    }
}
