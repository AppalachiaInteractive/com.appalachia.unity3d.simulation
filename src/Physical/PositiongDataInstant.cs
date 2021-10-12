using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Physical
{
    [Serializable]
    public struct PositiongDataInstant : IEquatable<PositiongDataInstant>
    {
        [SmartLabel] public float time;
        [SmartLabel] public float4x4 localToWorld;
        [SmartLabel] public float3 position;
        [SmartLabel] public quaternion rotation;
        [SmartLabel] public float3 localVelocity;
        [SmartLabel] public float3 localHeading;
        [SmartLabel] public float3 localAngularVelocity;
        [SmartLabel] public float3 localCenterOfMass;
        [SmartLabel] public float mass;

        public PositiongDataInstant(Rigidbody body, float4x4 ltw)
        {
            time = Time.time;
            localToWorld = ltw;
            position = body.position;
            rotation = body.rotation;
            localVelocity = localToWorld.MultiplyVector(body.velocity);
            localHeading = math.normalize(localVelocity);
            localAngularVelocity = localToWorld.MultiplyVector(body.angularVelocity);
            localCenterOfMass = body.centerOfMass;
            mass = body.mass;
        }

#region IEquatable

        public bool Equals(PositiongDataInstant other)
        {
            return time.Equals(other.time) &&
                   localToWorld.Equals(other.localToWorld) &&
                   position.Equals(other.position) &&
                   rotation.Equals(other.rotation) &&
                   localVelocity.Equals(other.localVelocity) &&
                   localHeading.Equals(other.localHeading) &&
                   localAngularVelocity.Equals(other.localAngularVelocity) &&
                   localCenterOfMass.Equals(other.localCenterOfMass) &&
                   mass.Equals(other.mass);
        }

        public override bool Equals(object obj)
        {
            return obj is PositiongDataInstant other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = time.GetHashCode();
                hashCode = (hashCode * 397) ^ localToWorld.GetHashCode();
                hashCode = (hashCode * 397) ^ position.GetHashCode();
                hashCode = (hashCode * 397) ^ rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ localVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ localHeading.GetHashCode();
                hashCode = (hashCode * 397) ^ localAngularVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ localCenterOfMass.GetHashCode();
                hashCode = (hashCode * 397) ^ mass.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(PositiongDataInstant left, PositiongDataInstant right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PositiongDataInstant left, PositiongDataInstant right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}
