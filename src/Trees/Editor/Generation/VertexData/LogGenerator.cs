using System.Linq;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Settings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.VertexData
{
    public static class LogGenerator
    {
        public static void ApplyMeshLogData(
            IShapeRead shapes,
            IHierarchyRead hierarchies,
            LODGenerationOutput output,
            VertexSettings log,
            BaseSeed seed)
        {
            using (BUILD_TIME.LOG_GEN.ApplyMeshLogData.Auto())
            {
                var maxHierarchyDepth = 8;

                hierarchies.RecurseHierarchies(
                    data => { maxHierarchyDepth = Mathf.Max(maxHierarchyDepth, data.hierarchyDepth); }
                );

                var minX = output.vertices.Min(v => v.position.x);
                var minY = output.vertices.Min(v => v.position.y);
                var minZ = output.vertices.Min(v => v.position.z);

                var offset = new Vector3(minX, minY, minZ);

                float Contrast(float contrast, float value)
                {
                    var x = 259f / 255f;
                    
                    var factor = (x * (contrast + 1f)) / (1f * (x - contrast));
                    var outval = (factor * (value - .5f)) + .5f;

                    return outval;
                }

                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    var pos = offset + vertex.position;

                    vertex.log.noise1 = seed.Noise3(pos, log.noise1Scale, log.noise1Offset);
                    vertex.log.noise2 = seed.Noise3(pos, log.noise2Scale, log.noise2Offset);

                    vertex.log.noise1 = Contrast(log.noise1Contrast, vertex.log.noise1);
                    vertex.log.noise2 = Contrast(log.noise2Contrast, vertex.log.noise2);
                    
                    output.vertices[index] = vertex;
                }
            }
        }
    }
}