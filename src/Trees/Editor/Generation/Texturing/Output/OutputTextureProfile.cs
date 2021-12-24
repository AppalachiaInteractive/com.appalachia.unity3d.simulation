using System;
using System.Collections.Generic;
using System.Diagnostics;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Output
{
    [Serializable]
    public class OutputTextureProfile : AppalachiaSimpleBase
    {
        private sealed class OutputTextureProfileEqualityComparer : IEqualityComparer<OutputTextureProfile>
        {
            [DebuggerStepThrough] public bool Equals(OutputTextureProfile x, OutputTextureProfile y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return (x._map == y._map) && x._color.Equals(y._color) && (x.propertyName == y.propertyName) && (x.fileNameSuffix == y.fileNameSuffix) && (x.red == y.red) && (x.green == y.green) && (x.blue == y.blue) && (x.alpha == y.alpha) && Equals(x.settings, y.settings);
            }

            [DebuggerStepThrough] public int GetHashCode(OutputTextureProfile obj)
            {
                unchecked
                {
                    var hashCode = (int) obj._map;
                    hashCode = (hashCode * 397) ^ obj._color.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.propertyName != null ? obj.propertyName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.fileNameSuffix != null ? obj.fileNameSuffix.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int) obj.red;
                    hashCode = (hashCode * 397) ^ (int) obj.green;
                    hashCode = (hashCode * 397) ^ (int) obj.blue;
                    hashCode = (hashCode * 397) ^ (int) obj.alpha;
                    hashCode = (hashCode * 397) ^ (obj.settings != null ? obj.settings.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<OutputTextureProfile> OutputTextureProfileComparer { get; } = new OutputTextureProfileEqualityComparer();

        public OutputTextureProfile(TextureMap map, Color defaultColor)
        {
            _map = map;
            _color = defaultColor;
        }

        [SerializeField]
        [TabGroup("Group", "General", Paddingless = true)]
        [LabelWidth(100)]
        private TextureMap _map;
        public TextureMap map => _map;

        [SerializeField]
        [TabGroup("Group", "General", Paddingless = true)]
        [LabelWidth(100)]
        private Color _color;
        public Color color => _color;

        [TabGroup("Group", "General", Paddingless = true)]
        [LabelWidth(100)]
        public string propertyName = "_MainTex";
        
        [TabGroup("Group", "General", Paddingless = true)]
        [LabelWidth(100)]
        public string fileNameSuffix = "_Albedo";
        
        /*
        [TabGroup("Group", "General", Paddingless = true)]
        [LabelWidth(100)]
        public TextureSize textureSize;
        */
        
        
        [TabGroup("Group", "Channels", Paddingless = true)]
        [LabelWidth(100)]
        public TextureMapChannel red;
        [TabGroup("Group", "Channels", Paddingless = true)]
        [LabelWidth(100)]
        public TextureMapChannel green;
        [TabGroup("Group", "Channels", Paddingless = true)]
        [LabelWidth(100)]
        public TextureMapChannel blue;
        [TabGroup("Group", "Channels", Paddingless = true)]
        [LabelWidth(100)]
        public TextureMapChannel alpha;

        [TabGroup("Group", "Settings", Paddingless = true)]
        [InlineProperty, HideLabel]
        public OutputTextureSetting settings;

        public TextureMapChannel this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return red;
                    case 1:
                        return green;
                    case 2:
                        return blue;
                    case 3:
                        return alpha;
                    default:
                        throw new IndexOutOfRangeException("0 to 3 allowed.");
                }
            }
        }
    }
}