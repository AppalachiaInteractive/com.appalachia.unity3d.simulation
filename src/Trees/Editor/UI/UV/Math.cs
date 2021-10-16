using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    /// <summary>
    ///     A collection of math functions that are useful when working with 3d meshes.
    /// </summary>
    public static class Math
    {
        /// <summary>
        ///     Epsilon to use when comparing vertex positions for equality.
        /// </summary>
        private const float k_FltCompareEpsilon = .0001f;

        /// <summary>
        /// The minimum distance a handle must move on an axis before considering that axis as engaged.
        /// </summary>
        internal const float handleEpsilon = .0001f;

        /// <summary>
        ///     Compares two Vector2 values component-wise, allowing for a margin of error.
        /// </summary>
        /// <param name="a">First Vector2 value.</param>
        /// <param name="b">Second Vector2 value.</param>
        /// <param name="delta">The maximum difference between components allowed.</param>
        /// <returns>True if a and b components are respectively within delta distance of one another.</returns>
        internal static bool Approx2(this Vector2 a, Vector2 b, float delta = k_FltCompareEpsilon)
        {
            return (Mathf.Abs(a.x - b.x) < delta) && (Mathf.Abs(a.y - b.y) < delta);
        }

        /// <summary>
        ///     Compares two float values component-wise, allowing for a margin of error.
        /// </summary>
        /// <param name="a">First float value.</param>
        /// <param name="b">Second float value.</param>
        /// <param name="delta">The maximum difference between components allowed.</param>
        /// <returns>True if a and b components are respectively within delta distance of one another.</returns>
        internal static bool Approx(this float a, float b, float delta = k_FltCompareEpsilon)
        {
            return Mathf.Abs(b - a) < Mathf.Abs(delta);
        }
    }
}
