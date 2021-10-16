using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core.Geometry;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class TreeVertexExtensions
    {
        public static Vector3 AveragePosition(this IEnumerable<TreeVertex> vertices)
        {
            return vertices.Select(v => v.position).Average();
        }
        
        
        public static Vector3 AverageNormal(this IEnumerable<TreeVertex> vertices)
        {
            return vertices.Select(v => v.normal).Average();
        }
        
        
        public static Vector3 AveragePosition(this IList<TreeVertex> vertices, int start, int end)
        {
            return vertices.Skip(start).Take(end - start).AveragePosition();
        }
        
        
        public static Vector3 AverageNormal(this IList<TreeVertex> vertices, int start, int end)
        {
            return vertices.Skip(start).Take(end - start).AverageNormal();
        }
    }
}
