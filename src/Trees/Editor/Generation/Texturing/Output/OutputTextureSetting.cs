using System;
using System.Diagnostics;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Appalachia.Simulation.Trees.Settings;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Output
{
    [Serializable]
    public class OutputTextureSetting : IEquatable<OutputTextureSetting>
    {
        public OutputTextureSetting(
            TextureImporterType type,
            bool sRGB,
            bool inputTextureAlpha,
            bool alphaIsTransparency,
            bool mipmapEnabled,
            bool mipmapsPreserveCoverage,
            float alphaTestReferenceValue,
            FilterMode filterMode,
            int anisoLevel)
        {
            this.type = type;
            this.sRGB = sRGB;
            this.inputTextureAlpha = inputTextureAlpha;
            this.alphaIsTransparency = alphaIsTransparency;
            this.mipmapEnabled = mipmapEnabled;
            this.mipmapsPreserveCoverage = mipmapsPreserveCoverage;
            this.alphaTestReferenceValue = alphaTestReferenceValue;
            this.filterMode = filterMode;
            this.anisoLevel = anisoLevel;
        }

        #region Fields and Autoproperties

        [LabelWidth(100)] public bool alphaIsTransparency;

        [LabelWidth(100)] public bool inputTextureAlpha;

        [LabelWidth(100)] public bool mipmapEnabled;

        [ShowIf(nameof(mipmapEnabled))]
        [LabelWidth(100)]
        public bool mipmapsPreserveCoverage;

        [LabelWidth(100)] public bool sRGB;

        [LabelWidth(100)] public FilterMode filterMode;

        [ShowIf(nameof(_showAlphaTest))]
        [PropertyRange(0f, 1f)]
        [LabelWidth(100)]
        public float alphaTestReferenceValue;

        [PropertyRange(0, 16)]
        [LabelWidth(100)]
        public int anisoLevel;

        [LabelWidth(100)] public TextureImporterType type;

        [HorizontalGroup("Wrap")]
        [LabelWidth(50)]
        public TextureWrapMode wrapModeU;

        [HorizontalGroup("Wrap")]
        [LabelWidth(50)]
        public TextureWrapMode wrapModeV;

        #endregion

        private bool _showAlphaTest => mipmapsPreserveCoverage && mipmapEnabled;

        [DebuggerStepThrough]
        public static bool operator ==(OutputTextureSetting left, OutputTextureSetting right)
        {
            return Equals(left, right);
        }

        [DebuggerStepThrough]
        public static bool operator !=(OutputTextureSetting left, OutputTextureSetting right)
        {
            return !Equals(left, right);
        }

        public static OutputTextureSetting RGB_Linear()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                false,
                false,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                1
            );
        }

        public static OutputTextureSetting RGB_Normal()
        {
            return new OutputTextureSetting(
                TextureImporterType.NormalMap,
                false,
                false,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                8
            );
        }

        public static OutputTextureSetting RGB_sRGB()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                true,
                false,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                1
            );
        }

        public static OutputTextureSetting RGBA_Linear()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                false,
                true,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                1
            );
        }

        public static OutputTextureSetting RGBA_Linear_Transparency()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                false,
                true,
                true,
                true,
                true,
                0.5f,
                FilterMode.Trilinear,
                1
            );
        }

        public static OutputTextureSetting RGBA_Normal()
        {
            return new OutputTextureSetting(
                TextureImporterType.NormalMap,
                false,
                true,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                8
            );
        }

        public static OutputTextureSetting RGBA_Normal_Transparency()
        {
            return new OutputTextureSetting(
                TextureImporterType.NormalMap,
                false,
                true,
                true,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                8
            );
        }

        public static OutputTextureSetting RGBA_sRGB()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                true,
                true,
                false,
                true,
                false,
                0.0f,
                FilterMode.Trilinear,
                4
            );
        }

        public static OutputTextureSetting RGBA_sRGB_Transparency()
        {
            return new OutputTextureSetting(
                TextureImporterType.Default,
                true,
                true,
                true,
                true,
                true,
                0.5f,
                FilterMode.Trilinear,
                8
            );
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

            return Equals((OutputTextureSetting)obj);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)type;
                hashCode = (hashCode * 397) ^ sRGB.GetHashCode();
                hashCode = (hashCode * 397) ^ inputTextureAlpha.GetHashCode();
                hashCode = (hashCode * 397) ^ alphaIsTransparency.GetHashCode();
                hashCode = (hashCode * 397) ^ mipmapEnabled.GetHashCode();
                hashCode = (hashCode * 397) ^ mipmapsPreserveCoverage.GetHashCode();
                hashCode = (hashCode * 397) ^ alphaTestReferenceValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)filterMode;
                hashCode = (hashCode * 397) ^ anisoLevel;
                hashCode = (hashCode * 397) ^ (int)wrapModeU;
                hashCode = (hashCode * 397) ^ (int)wrapModeV;
                return hashCode;
            }
        }

        public void Apply(string path, TextureSize textureSize, QualityMode mode)
        {
            using (BUILD_TIME.OUT_TEX_SET.Apply.Auto())
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                if (importer == null)
                {
                    throw new NotSupportedException("Need to save texture first!");
                }

                importer.isReadable = true;
                importer.textureType = type;
                importer.sRGBTexture = sRGB;
                importer.alphaSource = mode == QualityMode.Working
                    ? TextureImporterAlphaSource.FromInput
                    : inputTextureAlpha
                        ? TextureImporterAlphaSource.FromInput
                        : TextureImporterAlphaSource.None;
                importer.alphaIsTransparency = alphaIsTransparency;
                importer.mipmapEnabled = mipmapEnabled;
                importer.mipMapsPreserveCoverage = mipmapsPreserveCoverage;
                importer.alphaTestReferenceValue =
                    importer.mipMapsPreserveCoverage ? alphaTestReferenceValue : 0f;
                importer.filterMode = filterMode;
                importer.anisoLevel = anisoLevel;
                importer.wrapModeU = wrapModeU;
                importer.wrapModeV = wrapModeV;
                importer.maxTextureSize = (int)textureSize;

                importer.textureCompression = mode == QualityMode.Working
                    ? TextureImporterCompression.Uncompressed
                    : TextureImporterCompression.CompressedHQ;

                var platformSettings = importer.GetDefaultPlatformTextureSettings();
                platformSettings.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(platformSettings);

                importer.WriteSettings();
            }
        }

        public OutputTextureSetting Clamp()
        {
            wrapModeU = TextureWrapMode.Clamp;
            wrapModeV = TextureWrapMode.Clamp;
            return this;
        }

        public Texture2D Create(Vector2 textureSize, string name)
        {
            using (BUILD_TIME.OUT_TEX_SET.Create.Auto())
            {
                var tex = new Texture2D(
                    (int)textureSize.x,
                    (int)textureSize.y,
                    TextureFormat.RGBA32,
                    true,
                    !sRGB
                ) { name = name };

                return tex;
            }
        }

        public OutputTextureSetting Repeat()
        {
            wrapModeU = TextureWrapMode.Repeat;
            wrapModeV = TextureWrapMode.Repeat;
            return this;
        }

        public void UpdateAlphaTestReferenceValue(string path, float value)
        {
            alphaTestReferenceValue = value;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                if (importer.mipMapsPreserveCoverage &&
                    (Math.Abs(importer.alphaTestReferenceValue - value) < float.Epsilon))
                {
                    return;
                }

                importer.mipMapsPreserveCoverage = true;
                importer.alphaTestReferenceValue = value;

                importer.WriteSettings();
            }
        }

        #region IEquatable<OutputTextureSetting> Members

        [DebuggerStepThrough]
        public bool Equals(OutputTextureSetting other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return (type == other.type) &&
                   (sRGB == other.sRGB) &&
                   (inputTextureAlpha == other.inputTextureAlpha) &&
                   (alphaIsTransparency == other.alphaIsTransparency) &&
                   (mipmapEnabled == other.mipmapEnabled) &&
                   (mipmapsPreserveCoverage == other.mipmapsPreserveCoverage) &&
                   alphaTestReferenceValue.Equals(other.alphaTestReferenceValue) &&
                   (filterMode == other.filterMode) &&
                   (anisoLevel == other.anisoLevel) &&
                   (wrapModeU == other.wrapModeU) &&
                   (wrapModeV == other.wrapModeV);
        }

        #endregion
    }
}
