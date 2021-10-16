using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core.Geometry;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Meshing
{
    [Serializable]
    public class LODGenerationOutput
    {
        [SerializeField] private int _lodLevel;
        public int lodLevel => _lodLevel;
        
        public List<BillboardPlane> billboards;
        
        public List<PrefabVertex> fruits;
        public List<PrefabVertex> knots;
        public List<PrefabVertex> fungi;
        
        [SerializeField] private List<TreeTriangle> _triangles;
        
        public IList<TreeTriangle> triangles => _triangles;
        
        
        [SerializeField] private List<TreeVertex> _vertices;
        public IList<TreeVertex> vertices => _vertices;
        

        private HashSet<int> _materialIDs;

        public HashSet<int> materialIDs
        {
            get
            {
                if (_materialIDs == null)
                {
                    _materialIDs = new HashSet<int>();
                }

                return _materialIDs;
            }
        }

        public int submeshCount => materialIDs.Count;

        public LODGenerationOutput(int lodLevel)
        {
            _lodLevel = lodLevel;
            billboards = new List<BillboardPlane>();
            fruits = new List<PrefabVertex>();
            knots = new List<PrefabVertex>();
            fungi = new List<PrefabVertex>();
            _triangles = new List<TreeTriangle>();
            _vertices = new List<TreeVertex>();
            _materialIDs = new HashSet<int>();
        }

        public void Clear()
        {
            foreach (var billboard in billboards)
            {
                billboard.Reset();
            }
            
            billboards.Clear();
            materialIDs.Clear();
            fruits.Clear();
            fungi.Clear();
            knots.Clear();
            _triangles.Clear();
            _vertices.Clear();
        }

        public void AddVertex(TreeVertex vertex)
        {
            if (float.IsNaN(vertex.position.x) || 
                float.IsNaN(vertex.position.y) ||
                float.IsNaN(vertex.position.z))
            {
                throw new NotSupportedException("Vertex position NaN!  Look for division by 0!");
            }
            if ((vertex.position == Vector3.positiveInfinity) || (vertex.position == Vector3.negativeInfinity))
            {
                throw new NotSupportedException("Bad mesh bounds!");
            }
            if (vertex.position.magnitude > 1000)
            {
                throw new NotSupportedException("Bad mesh bounds!");
            }
            else if (vertex.position.magnitude < -1000)
            {
                throw new NotSupportedException("Bad mesh bounds!");
            }

            _vertices.Add(vertex);
        }

        public void AddTriangle(TreeTriangle triangle)
        {
            materialIDs.Add(triangle.inputMaterialID);
            _triangles.Add(triangle);
        }

        public int VisibleVertexCount()
        {
            return _vertices.Count(v => v.visible);
        }
        
        public int VisibleTriangleCount()
        {
            return _triangles.Count(v => v.visible);
        }

        public Bounds GetBounds()
        {
            var bounds = new Bounds();

            foreach (var vertex in vertices)
            {
                bounds.Encapsulate(vertex.position);
            }
            
            return bounds;
        }
    }
}