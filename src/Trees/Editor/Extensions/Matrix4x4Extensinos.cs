using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class Matrix4x4Extensinos
    {
        public static Matrix4x4 LerpTo(this Matrix4x4 from, Matrix4x4 to, float time)
        {
            var matrix = new Matrix4x4();
            
            for (var i = 0; i < 16; i++) 
            {
                matrix[i] = Mathf.Lerp(from[i], to[i], time);
            }

            return matrix;
        }
    }
}
