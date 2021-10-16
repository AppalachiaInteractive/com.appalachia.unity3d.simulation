using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public static class TangentGenerator
    {
        public static Vector4 CreateTangent(ShapeData shape, Quaternion rot, Vector3 normal)
        {
            var tangent = shape.effectiveMatrix.MultiplyVector(rot * new Vector3(1, 0, 0));
            tangent -= normal * Vector3.Dot(tangent, normal);
            tangent.Normalize();
            return new Vector4(tangent.x, tangent.y, tangent.z, 1);
        }
        
    }
}
