using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    /// <summary>
    ///     Snapping functions and ProGrids compatibility.
    /// </summary>
    internal static class Snapping
    {
        /// <summary>
        ///     Round value to nearest snpVal increment.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="snpVal"></param>
        /// <returns></returns>
        public static Vector3 SnapValue(Vector3 vertex, float snpVal)
        {
            // snapValue is a global setting that comes from ProGrids
            return new Vector3(
                snpVal * Mathf.Round(vertex.x / snpVal),
                snpVal * Mathf.Round(vertex.y / snpVal),
                snpVal * Mathf.Round(vertex.z / snpVal)
            );
        }

        /// <summary>
        ///     Round value to nearest snpVal increment.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="snpVal"></param>
        /// <returns></returns>
        public static float SnapValue(float val, float snpVal)
        {
            if (snpVal < Mathf.Epsilon)
            {
                return val;
            }

            return snpVal * Mathf.Round(val / snpVal);
        }
    }
}
