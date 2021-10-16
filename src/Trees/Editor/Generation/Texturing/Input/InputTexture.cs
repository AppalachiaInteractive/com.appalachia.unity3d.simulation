#region

using System;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    [Serializable]
    public class InputTexture
    {
        [PreviewField(ObjectFieldAlignment.Left, Height = 82f), HideLabel, InlineProperty]
        [HorizontalGroup("TEX", MaxWidth = 96f)]
        [PropertySpace]
        public Texture2D texture;

        [VerticalGroup("TEX/PANE")]
        [HideLabel, InlineProperty]
        public InputTextureProfile profile;

        [SerializeField, HideInInspector]
        private bool _isReadable;

        [SerializeField, HideInInspector]
        private TextureImporterAlphaSource _alphaSource;

        [SerializeField, HideInInspector]
        private TextureImporterCompression _textureCompression;

        [SerializeField, HideInInspector]
        private int _maxTextureSize;

        [SerializeField, HideInInspector]
        private bool _importSettingsCached;
        
        public string ActivateProcessingTextureImporterSettings(TextureSize maxTextureSize)
        {
            using (BUILD_TIME.INPUT_TEX.ActivateProcessingTextureImporterSettings.Auto())
            {
                var importer = texture.GetTextureImporter();

                if (importer.isReadable && (importer.textureCompression == TextureImporterCompression.Uncompressed))
                {
                    return null;
                }

                if (!_importSettingsCached)
                {
                    _isReadable = importer.isReadable;
                    _textureCompression = importer.textureCompression;
                    _alphaSource = importer.alphaSource;
                    _maxTextureSize = importer.maxTextureSize;
                    _importSettingsCached = true;
                }
                
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.maxTextureSize = (int)maxTextureSize;

                importer.WriteSettings();

                return importer.assetPath;
            }
        }

        public string RestoreOriginalTextureImporterSettings()
        {
            using (BUILD_TIME.INPUT_TEX.RestoreOriginalTextureImporterSettings.Auto())
            {
                var importer = texture.GetTextureImporter();

                if (importer == null)
                {
                    return null;
                }

                if (!_importSettingsCached)
                {
                    return null;
                }

                importer.isReadable = _isReadable;
                importer.textureCompression = _textureCompression;
                importer.alphaSource = _alphaSource;
                importer.maxTextureSize = _maxTextureSize;

                importer.WriteSettings();

                return importer.assetPath;
            }
        }
        

        public InputTextureUsageElement GetUsageData(
         TextureChannel channel,
         Rect rect)
        {
            using (BUILD_TIME.INPUT_TEX.GetUsageData.Auto())
            {
                switch (channel)
                {
                    case TextureChannel.R:
                        return new InputTextureUsageElement(profile.red, texture, 0, rect);
                    case TextureChannel.G:
                        return new InputTextureUsageElement(profile.green, texture, 1, rect);
                    case TextureChannel.B:
                        return new InputTextureUsageElement(profile.blue, texture, 2, rect);
                    case TextureChannel.A:
                        return new InputTextureUsageElement(profile.alpha, texture, 3, rect);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
                }
            }
        }
    }
}