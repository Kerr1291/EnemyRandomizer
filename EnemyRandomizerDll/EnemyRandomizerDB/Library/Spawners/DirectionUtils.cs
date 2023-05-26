using UnityEngine;

namespace EnemyRandomizerMod
{
    public static class DirectionUtils
    {
        public static int GetCardinalDirection(float degrees)
        {
            return DirectionUtils.NegSafeMod(Mathf.RoundToInt(degrees / 90f), 4);
        }

        public static int NegSafeMod(int val, int len)
        {
            return (val % len + len) % len;
        }

        public static int GetX(int cardinalDirection)
        {
            int num = cardinalDirection % 4;
            if (num == 0)
            {
                return 1;
            }
            if (num != 2)
            {
                return 0;
            }
            return -1;
        }

        public static int GetY(int cardinalDirection)
        {
            int num = cardinalDirection % 4;
            if (num == 1)
            {
                return 1;
            }
            if (num != 3)
            {
                return 0;
            }
            return -1;
        }

        // Token: 0x040009A2 RID: 2466
        public const int Right = 0;

        // Token: 0x040009A3 RID: 2467
        public const int Up = 1;

        // Token: 0x040009A4 RID: 2468
        public const int Left = 2;

        // Token: 0x040009A5 RID: 2469
        public const int Down = 3;
    }
}
