using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    public struct BranchRollVector : IEquatable<BranchRollVector>
    {
        public bool Equals(BranchRollVector other)
        {
            return forward.Equals(other.forward) && verticality.Equals(other.verticality);
        }

        public override bool Equals(object obj)
        {
            return obj is BranchRollVector other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = forward.GetHashCode();
                hashCode = (hashCode * 397) ^ verticality.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BranchRollVector left, BranchRollVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BranchRollVector left, BranchRollVector right)
        {
            return !left.Equals(right);
        }

        public static BranchRollVector operator *(BranchRollVector left, Vector4 right)
        {
            var t = new BranchRollVector
            {
                forward = {x = left.forward.x * right.x, y = left.forward.y * right.y, z = left.forward.z * right.z},
                verticality = left.verticality * right.w
            };

            return t;
        }

        public static BranchRollVector operator *(Vector4 left, BranchRollVector right)
        {
            return right * left;
        }

        public Vector3 forward;
        public float verticality;
    }
}
