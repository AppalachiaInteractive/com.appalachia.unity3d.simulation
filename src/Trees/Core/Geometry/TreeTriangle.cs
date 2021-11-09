using System;
using System.Collections.Generic;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    [Serializable]
    public struct TreeTriangle /*: TreePoolObject<TreeTriangle>*/  : IEquatable<TreeTriangle>
    {
        public TreeTriangle(bool initialize = true)
        {
            visible = true;
            hierarchyID = 0;
            inputMaterialID = 0;
            shapeID = 0;
            type = TreeComponentType.Root;
            context = TreeMaterialUsage.Bark;
            v = new int[3];
            uvScale = Vector2.one;
        }
        
        public bool visible;
        public int hierarchyID;
        public int inputMaterialID;
        
        public int shapeID;

        public TreeComponentType type;
        public TreeMaterialUsage context;
        public int[] v;
        public Vector2 uvScale;

        public void FlipFace()
        {
            int t = v[0];
            v[0] = v[1];
            v[1] = t;
        }

        public float Area(IList<TreeVertex> vertices)
        {
            var a = vertices[v[0]].position;
            var b = vertices[v[1]].position;
            var c = vertices[v[2]].position;

            return Vector3.Cross(b - a, c - a).magnitude;
        }

        public Vector3 Normal(IList<TreeVertex> vertices)
        {
            var p1 = vertices[v[1]].position - vertices[v[0]].position;
            var p2 = vertices[v[2]].position - vertices[v[0]].position;

            return Vector3.Cross(p1, p2).normalized;
        }

        public void Set(
            ShapeData shape,
            int originalMaterialID,
            int v0,
            int v1,
            int v2,
            TreeMaterialUsage c,
            bool reverse)
        {
            visible = shape.exportGeometry;
            hierarchyID = shape.hierarchyID;
            shapeID = shape.shapeID;
            type = shape.type;
            inputMaterialID = originalMaterialID;
            context = c;

            if (reverse)
            {
                v[2] = v0;
                v[1] = v1;
                v[0] = v2;
            }
            else
            {
                v[0] = v0;
                v[1] = v1;
                v[2] = v2;
            }

        }

        public /*override */void Reset()
        {
            visible = true;
            hierarchyID = 0;
            shapeID = 0;
            type = 0;
            context = TreeMaterialUsage.Bark;
            inputMaterialID = 0;
            v[0] = 0;
            v[1] = 0;
            v[2] = 0;
            uvScale = Vector2.one;
        }

        public TreeTriangle Clone()
        {
            var t = new TreeTriangle();

            t.visible = visible;
            t.hierarchyID = hierarchyID;
            t.inputMaterialID = inputMaterialID;
            t.shapeID = shapeID;
            t.type = type;
            t.context = context;
            t.v = new int[3];
            t.v[0] = v[0];
            t.v[1] = v[1];
            t.v[2] = v[2];
            t.uvScale = uvScale;

            return t;
        }
        
        #region IEquatable

        [DebuggerStepThrough] public bool Equals(TreeTriangle other)
        {
            return (visible == other.visible) && (hierarchyID == other.hierarchyID) && (inputMaterialID == other.inputMaterialID) && (shapeID == other.shapeID) && (type == other.type) && (context == other.context) && Equals(v, other.v) && uvScale.Equals(other.uvScale);
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            return obj is TreeTriangle other && Equals(other);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = visible.GetHashCode();
                hashCode = (hashCode * 397) ^ hierarchyID;
                hashCode = (hashCode * 397) ^ inputMaterialID;
                hashCode = (hashCode * 397) ^ shapeID;
                hashCode = (hashCode * 397) ^ (int) type;
                hashCode = (hashCode * 397) ^ (int) context;
                hashCode = (hashCode * 397) ^ (v != null ? v.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ uvScale.GetHashCode();
                return hashCode;
            }
        }

        [DebuggerStepThrough] public static bool operator ==(TreeTriangle left, TreeTriangle right)
        {
            return left.Equals(right);
        }

        [DebuggerStepThrough] public static bool operator !=(TreeTriangle left, TreeTriangle right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}