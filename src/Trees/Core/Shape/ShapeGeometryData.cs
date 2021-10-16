using System;
using System.Collections.Generic;

namespace Appalachia.Simulation.Trees.Core.Shape
{
    [Serializable]
    public class ShapeGeometryData
    {
        public int modelTriangleEnd;
        public int modelTriangleStart;
        public int modelVertexEnd;
        public int modelVertexStart;


        public List<int> actualVertices = new List<int>();
        public List<int> actualTriangles = new List<int>();

        public ShapeGeometryData Clone()
        {
            var verts = new List<int>();
            var tris = new List<int>();
            
            foreach (var vert in actualVertices)
            {
                verts.Add(vert);
            }

            foreach (var tri in actualTriangles)
            {
                tris.Add(tri);
            }
            return new ShapeGeometryData
            {
                modelTriangleEnd = modelTriangleEnd,
                modelTriangleStart = modelTriangleStart,
                modelVertexEnd = modelVertexEnd,
                modelVertexStart = modelVertexStart,
                actualVertices = verts,
                actualTriangles = tris
            };
        }

        public void Reset()
        {
            modelTriangleEnd = 0;
            modelTriangleStart = 0;
            modelVertexEnd = 0;
            modelVertexStart = 0;
            actualTriangles.Clear();
            actualVertices.Clear();
        }
    }

}