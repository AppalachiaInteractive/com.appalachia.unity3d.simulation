using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    public struct BranchBendVector : IEquatable<BranchBendVector>
    {
        public bool Equals(BranchBendVector other)
        {
            return pivot.Equals(other.pivot) && bend.Equals(other.bend);
        }

        public override bool Equals(object obj)
        {
            return obj is BranchBendVector other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                var hashCode = pivot.GetHashCode();
                // ReSharper disable once NonReadonlyMemberInGetHashCode
                hashCode = (hashCode * 397) ^ bend.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BranchBendVector left, BranchBendVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BranchBendVector left, BranchBendVector right)
        {
            return !left.Equals(right);
        }

        public static BranchBendVector operator *(BranchBendVector left, Vector4 right)
        {
            var t = new BranchBendVector
            {
                pivot = {x = left.pivot.x * right.x, y = left.pivot.y * right.y, z = left.pivot.z * right.z},
                bend = left.bend * right.w
            };

            return t;
        }

        public static BranchBendVector operator *(Vector4 left, BranchBendVector right)
        {
            return right * left;
        }

        public Vector3 pivot;
        public float bend;
    }
}
