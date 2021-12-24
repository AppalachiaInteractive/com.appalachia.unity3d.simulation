using Appalachia.Core.Extensions;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Input
{
    public class InputTextureUsageElement : AppalachiaSimpleBase
    {
        public InputTextureUsageElement(
            InputTextureProfileChannel profileChannel, 
            Texture2D value,
            int channel,
            Rect rect
        )
        {
            present = profileChannel != null;
            this.value = value;
            this.channel = channel;
            invert = (profileChannel?.invert ?? false) ? 1f : 0f;
            packing = (float) (profileChannel?.packing ?? 0f);
            this.rect = rect;
            
            var importer = value.GetTextureImporter();

            if (importer.textureType == TextureImporterType.Default)
            {
                textureMode = importer.sRGBTexture ? 1.0f : 0.0f;
            }
            else if (importer.textureType == TextureImporterType.NormalMap)
            {
                textureMode = 2f;
            }
            else //if (importer.textureType == TextureImporterType.SingleChannel)
            {
                textureMode = 3f;
            }

        }
            
        public bool present;
        public Texture2D value;
        public float channel;
        public float invert;
        public float packing;
        public Rect rect;
        public float textureMode;
        public InputMaterial source;
    }
}
