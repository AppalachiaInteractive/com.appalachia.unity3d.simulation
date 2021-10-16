using System;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Geometry
{
    public struct WindVector : IEquatable<WindVector>
    {
        public bool Equals(WindVector other)
        {
            return primaryRoll.Equals(other.primaryRoll) &&
                primaryPivot.Equals(other.primaryPivot) && 
                secondaryRoll.Equals(other.secondaryRoll) &&
                secondaryPivot.Equals(other.secondaryPivot) && 
                secondaryBend.Equals(other.secondaryBend) &&
                secondaryForward.Equals(other.secondaryForward) && 
                secondaryVerticality.Equals(other.secondaryVerticality) &&
                tertiaryRoll.Equals(other.tertiaryRoll) && 
                phase.Equals(other.phase) &&
                variation.Equals(other.variation);
        }

        public override bool Equals(object obj)
        {
            return obj is WindVector other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = primaryRoll.GetHashCode();
                hashCode = (hashCode * 397) ^ primaryPivot.GetHashCode();
                hashCode = (hashCode * 397) ^ primaryBend.GetHashCode();
                hashCode = (hashCode * 397) ^ secondaryRoll.GetHashCode();
                hashCode = (hashCode * 397) ^ secondaryPivot.GetHashCode();
                hashCode = (hashCode * 397) ^ secondaryBend.GetHashCode();
                hashCode = (hashCode * 397) ^ secondaryForward.GetHashCode();
                hashCode = (hashCode * 397) ^ secondaryVerticality.GetHashCode();
                hashCode = (hashCode * 397) ^ tertiaryRoll.GetHashCode();
                hashCode = (hashCode * 397) ^ phase.GetHashCode();
                hashCode = (hashCode * 397) ^ variation.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(WindVector left, WindVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WindVector left, WindVector right)
        {
            return !left.Equals(right);
        }

        /*
        public static WindVector operator *(WindVector left, Vector4 right)
        {
            return new WindVector()
            {
                primary = left.primary * right.x,
                secondary = left.secondary * right.y,
                tertiary = left.tertiary * right.z,
                phase = left.phase * right.w
            };
        }

        public static WindVector operator *(Vector4 left, WindVector right)
        {
            return right * left;
        }*/


        public float primaryRoll;
        public Vector3 primaryPivot;
        public float primaryBend;
        
        public float secondaryRoll;
        public Vector3 secondaryPivot;
        public float secondaryBend;
        public Vector3 secondaryForward;
        public float secondaryVerticality;

        public float tertiaryRoll;
        
        public float phase;
        public float variation;
        
        /*
         PrimaryRoll = 10,         // VERTEX R
        PrimaryPivot = 10,          // UV2 XYZ
        PrimaryBend = 22,           // UV2 W
        
        SecondaryRoll = 20,         // VERTEX G
        SecondaryPivot = 21,        // UV4 XYZ
        SecondaryBend = 22,         // UV4 W
        SecondaryDirection = 23,    // UV3 XYZ
        SecondaryVerticality = 24,  // UV3 W
        
        Tertiary = 30,              // VERTEX B
        

        GustStrength = 90,
        GustDirectionality = 91,
        

        AmbientOcclusion = 97,       // UV0 Z
        Variation = 98,              // UV0 W
        Phase = 99,                  // VERTEX A*/
    }
}
