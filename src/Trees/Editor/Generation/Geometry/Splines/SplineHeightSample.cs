using System;
using System.Diagnostics;
using Appalachia.Utility.Strings;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Splines
{
    public struct SplineHeightSample : IComparable<SplineHeightSample>, IComparable
    {
        public float height;
        
        public bool trunkCutStump;
        public bool trunkCutTimber;
        public bool ignored;
        public bool critical;

        public static SplineHeightSample At(float height)
        {
            return new SplineHeightSample()
            {
                height = height
            };
        }

        public static SplineHeightSample TrunkCutStump(float height)
        {
            return new SplineHeightSample()
            {
                height = height,
                trunkCutStump = true,
            };
        }
        
        public static SplineHeightSample TrunkCutTimber(float height)
        {
            return new SplineHeightSample()
            {
                height = height,
                trunkCutTimber = true,
            };
        }
        
        public static SplineHeightSample Critical(float height)
        {
            return new SplineHeightSample()
            {
                height = height,
                critical = true
            };
        }

        [DebuggerStepThrough] public int CompareTo(SplineHeightSample other)
        {
            var heightComparison = height.CompareTo(other.height);
            if (heightComparison != 0)
            {
                return heightComparison;
            }

            var trunkCutStumpComparison = trunkCutStump.CompareTo(other.trunkCutStump);
            if (trunkCutStumpComparison != 0)
            {
                return trunkCutStumpComparison;
            }

            var trunkCutTimberComparison = trunkCutTimber.CompareTo(other.trunkCutTimber);
            if (trunkCutTimberComparison != 0)
            {
                return trunkCutTimberComparison;
            }

            return ignored.CompareTo(other.ignored);
        }

        [DebuggerStepThrough] public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            return obj is SplineHeightSample other
                ? CompareTo(other)
                : throw new ArgumentException(
                    ZString.Format("Object must be of type {0}", nameof(SplineHeightSample))
                );
        }

        [DebuggerStepThrough] public static bool operator <(SplineHeightSample left, SplineHeightSample right)
        {
            return left.CompareTo(right) < 0;
        }

        [DebuggerStepThrough] public static bool operator >(SplineHeightSample left, SplineHeightSample right)
        {
            return left.CompareTo(right) > 0;
        }

        [DebuggerStepThrough] public static bool operator <=(SplineHeightSample left, SplineHeightSample right)
        {
            return left.CompareTo(right) <= 0;
        }

        [DebuggerStepThrough] public static bool operator >=(SplineHeightSample left, SplineHeightSample right)
        {
            return left.CompareTo(right) >= 0;
        }

        [DebuggerStepThrough] public override string ToString()
        {
            return ZString.Format("{0:0.000}", height);
        }
    }
}
