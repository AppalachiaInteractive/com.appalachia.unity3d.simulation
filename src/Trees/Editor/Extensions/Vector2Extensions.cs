using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class Vector2Extensions
    {
        public static float Cross(this Vector2 O, Vector2 A, Vector2 B)
        {
            return ((A.x - O.x) * (B.y - O.y)) - ((A.y - O.y) * (B.x - O.x));
        }

        public static float TriangleArea(this Vector2 O, Vector2 A, Vector2 B)
        {
            return Mathf.Abs(((A.x - B.x) * (O.y - A.y)) - ((A.x - O.x) * (B.y - A.y))) * 0.5f;
        }
    }
}