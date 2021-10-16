using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    public struct LogVector : IEquatable<LogVector>
    {        
        public bool Equals(LogVector other)
        {
            return bark.Equals(other.bark) && woodDarkening.Equals(other.woodDarkening) && noise1.Equals(other.noise1) && noise2.Equals(other.noise2);
        }

        public override bool Equals(object obj)
        {
            return obj is LogVector other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = bark.GetHashCode();
                hashCode = (hashCode * 397) ^ woodDarkening.GetHashCode();
                hashCode = (hashCode * 397) ^ noise1.GetHashCode();
                hashCode = (hashCode * 397) ^ noise2.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(LogVector left, LogVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LogVector left, LogVector right)
        {
            return !left.Equals(right);
        }

        public static LogVector operator *(LogVector left, Vector4 right)
        {
            return new LogVector
            {
                bark = left.bark * right.x,
                woodDarkening = left.woodDarkening * right.y,
                noise1 = left.noise1 * right.z,
                noise2 = left.noise2 * right.w
            };
        }

        public static LogVector operator *(Vector4 left, LogVector right)
        {
            return right * left;
        }

        public float bark;
        public float woodDarkening;
        public float noise1;
        public float noise2;
    }
}
