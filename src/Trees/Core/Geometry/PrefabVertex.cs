#region

using System;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    [Serializable]
    public class PrefabVertex : /*TreePoolObject<PrefabVertex>,*/ IEquatable<PrefabVertex>
    {
        public GameObject prefab;
        public int prefabID;
        public Quaternion rotation = Quaternion.identity;
        public float scale = 1f;

        public int hierarchyID;
        public int shapeID;
        public float parentOffset;
        public float heightOffset;
        public Matrix4x4 matrix;
        public Vector3 normal = Vector3.forward;
        public Vector3 position;

        public TreeComponentType type;

        [DebuggerStepThrough] public bool Equals(PrefabVertex other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(prefab, other.prefab) &&
                (prefabID == other.prefabID) &&
                rotation.Equals(other.rotation) &&
                scale.Equals(other.scale) &&
                (hierarchyID == other.hierarchyID) &&
                (shapeID == other.shapeID) &&
                parentOffset.Equals(other.parentOffset) &&
                heightOffset.Equals(other.heightOffset) &&
                matrix.Equals(other.matrix) &&
                normal.Equals(other.normal) &&
                position.Equals(other.position) &&
                (type == other.type);
        }

        public void Set(
            ShapeData shape,
            Vector3 pos,
            Vector3 nor,
            GameObject pf,
            int pfID,
            Quaternion rot,
            float scl,
            float offset,
            Matrix4x4 mtx)
        {
            prefab = pf;
            prefabID = pfID;
            rotation = rot;
            scale = scl;

            hierarchyID = shape.hierarchyID;
            shapeID = shape.shapeID;
            parentOffset = shape.offset;
            heightOffset = offset;
            matrix = mtx;
            normal = nor;
            position = pos;
            type = shape.type;
        }

        /*
        public override void Reset()
        {
            prefab = null;
            prefabID = 0;
            rotation = Quaternion.identity;
            scale = Vector3.one;

            hierarchyID = 0;
            shapeID = 0;
            parentOffset = 0;
            heightOffset = 0;
            matrix = Matrix4x4.identity;
            normal = Vector3.zero;
            position = Vector3.zero;
            type = 0;
        }

        public override void Initialize()
        {
        }
        */

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PrefabVertex) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = prefab != null ? prefab.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ prefabID;
                hashCode = (hashCode * 397) ^ rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ scale.GetHashCode();
                hashCode = (hashCode * 397) ^ hierarchyID;
                hashCode = (hashCode * 397) ^ shapeID;
                hashCode = (hashCode * 397) ^ parentOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ heightOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ matrix.GetHashCode();
                hashCode = (hashCode * 397) ^ normal.GetHashCode();
                hashCode = (hashCode * 397) ^ position.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) type;
                return hashCode;
            }
        }

        [DebuggerStepThrough] public static bool operator ==(PrefabVertex left, PrefabVertex right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator !=(PrefabVertex left, PrefabVertex right)
        {
            return !Equals(left, right);
        }
    }
}
