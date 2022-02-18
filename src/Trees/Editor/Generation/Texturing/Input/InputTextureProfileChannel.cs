using System;
using System.Diagnostics;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    [Serializable]
    public class InputTextureProfileChannel : IEquatable<InputTextureProfileChannel>
    {
        #region Fields and Autoproperties

        [HorizontalGroup]

        //[InfoBox("Invert", InfoMessageType.None), HideLabel, LabelWidth(40)]
        [HideLabel]
        public bool invert;

        [HorizontalGroup]

        //[InfoBox("Packing", InfoMessageType.None), HideLabel]
        [HideLabel]
        public TextureChannelPacking packing;

        [HorizontalGroup]

        //[InfoBox("Channel", InfoMessageType.None), HideLabel]
        [HideLabel]
        public TextureMapChannel mapChannel;

        #endregion

        [DebuggerStepThrough]
        public static bool operator ==(TextureMapChannel left, InputTextureProfileChannel right)
        {
            if (right == null)
            {
                if (left == TextureMapChannel.None)
                {
                    return true;
                }

                return false;
            }

            return left == right.mapChannel;
        }

        [DebuggerStepThrough]
        public static bool operator ==(InputTextureProfileChannel left, TextureMapChannel right)
        {
            if (left == null)
            {
                if (right == TextureMapChannel.None)
                {
                    return true;
                }

                return false;
            }

            return left.mapChannel == right;
        }

        [DebuggerStepThrough]
        public static bool operator !=(TextureMapChannel left, InputTextureProfileChannel right)
        {
            return !(left == right);
        }

        [DebuggerStepThrough]
        public static bool operator !=(InputTextureProfileChannel left, TextureMapChannel right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public override bool Equals(object obj)
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

            return Equals((InputTextureProfileChannel)obj);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)mapChannel;
                hashCode = (hashCode * 397) ^ invert.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)packing;
                return hashCode;
            }
        }

        #region IEquatable<InputTextureProfileChannel> Members

        [DebuggerStepThrough]
        public bool Equals(InputTextureProfileChannel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (mapChannel == other.mapChannel) && (invert == other.invert) && (packing == other.packing);
        }

        #endregion
    }
}
