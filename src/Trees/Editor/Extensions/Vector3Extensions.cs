using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class Vector3Extensions
    {
        public static float TriangleArea(this Vector3 O, Vector3 A, Vector3 B)
        {
            return Mathf.Abs(((A.x - B.x) * (O.y - A.y)) - ((A.x - O.x) * (B.y - A.y))) * 0.5f;
        }

        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }
    }
}