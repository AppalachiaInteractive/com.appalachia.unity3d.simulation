using System;
using Appalachia.Core.Attributes.Editing;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Physical
{
    [Serializable]
    public struct PositioningData : IEquatable<PositioningData>
    {
        [SmartLabel] public PositiongDataInstant current;
        [SmartLabel] public PositiongDataInstant previous;

        public void Update(Rigidbody body, float4x4 ltw)
        {
            previous = current;

            current = new PositiongDataInstant(body, ltw);
            ;
        }

#region IEquatable

        public bool Equals(PositioningData other)
        {
            return current.Equals(other.current) && previous.Equals(other.previous);
        }

        public override bool Equals(object obj)
        {
            return obj is PositioningData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (current.GetHashCode() * 397) ^ previous.GetHashCode();
            }
        }

        public static bool operator ==(PositioningData left, PositioningData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PositioningData left, PositioningData right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}
