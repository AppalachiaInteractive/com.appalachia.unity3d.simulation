using System;
using System.Diagnostics;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.TouchBend.Data
{
    [Serializable]
    public class TouchBendComponentInfo : AppalachiaObject<TouchBendComponentInfo>,
                                          IEquatable<TouchBendComponentInfo>
    {
        public Texture2D texture;

        [ReadOnly] public float size;

        [PropertyRange(-1f, 1f)] public float offset;

        [PropertyRange(0f, 4f)] public float strength = 1.0f;

        [PropertyRange(0f, 2f)] public float scale = 1.0f;

        [PropertyRange(0f, 1.0)] public float minOld;

        [PropertyRange(0f, 1.0f)]
        public float maxOld = 1.0f;

        [DebuggerStepThrough] public bool Equals(TouchBendComponentInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return base.Equals(other) &&
                   Equals(texture, other.texture) &&
                   size.Equals(other.size) &&
                   offset.Equals(other.offset) &&
                   strength.Equals(other.strength) &&
                   minOld.Equals(other.minOld) &&
                   maxOld.Equals(other.maxOld);
        }

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

            return Equals((TouchBendComponentInfo) obj);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (texture != null ? texture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ size.GetHashCode();
                hashCode = (hashCode * 397) ^ offset.GetHashCode();
                hashCode = (hashCode * 397) ^ strength.GetHashCode();
                hashCode = (hashCode * 397) ^ minOld.GetHashCode();
                hashCode = (hashCode * 397) ^ maxOld.GetHashCode();
                return hashCode;
            }
        }

        [DebuggerStepThrough] public static bool operator ==(TouchBendComponentInfo left, TouchBendComponentInfo right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough] public static bool operator !=(TouchBendComponentInfo left, TouchBendComponentInfo right)
        {
            return !Equals(left, right);
        }
    }
}
