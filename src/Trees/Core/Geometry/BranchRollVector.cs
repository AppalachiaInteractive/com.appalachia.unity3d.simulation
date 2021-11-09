using System;
using System.Diagnostics;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    public struct BranchRollVector : IEquatable<BranchRollVector>
    {
        [DebuggerStepThrough] public bool Equals(BranchRollVector other)
        {
            return forward.Equals(other.forward) && verticality.Equals(other.verticality);
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            return obj is BranchRollVector other && Equals(other);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = forward.GetHashCode();
                hashCode = (hashCode * 397) ^ verticality.GetHashCode();
                return hashCode;
            }
        }

        [DebuggerStepThrough] public static bool operator ==(BranchRollVector left, BranchRollVector right)
        {
            return left.Equals(right);
        }

        [DebuggerStepThrough] public static bool operator !=(BranchRollVector left, BranchRollVector right)
        {
            return !left.Equals(right);
        }

        [DebuggerStepThrough] public static BranchRollVector operator *(BranchRollVector left, Vector4 right)
        {
            var t = new BranchRollVector
            {
                forward = {x = left.forward.x * right.x, y = left.forward.y * right.y, z = left.forward.z * right.z},
                verticality = left.verticality * right.w
            };

            return t;
        }

        [DebuggerStepThrough] public static BranchRollVector operator *(Vector4 left, BranchRollVector right)
        {
            return right * left;
        }

        public Vector3 forward;
        public float verticality;
    }
}
