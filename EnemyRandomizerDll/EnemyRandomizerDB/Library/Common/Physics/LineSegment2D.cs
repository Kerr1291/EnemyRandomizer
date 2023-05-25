using UnityEngine;

namespace EnemyRandomizerMod
{
    public partial class PreventInsideWalls
    {
        public struct LineSegment2D
        {
            public Vector2 a;
            public Vector2 b;

            public Vector2 dir => b - a;
        }
    }
}