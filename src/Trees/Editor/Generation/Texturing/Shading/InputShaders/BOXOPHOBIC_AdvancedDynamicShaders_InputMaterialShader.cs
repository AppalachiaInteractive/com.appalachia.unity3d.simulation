using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class BOXOPHOBIC_AdvancedDynamicShaders_InputMaterialShader : IInputMaterialShader
    {
        public int Priority { get; } = 0;
        
        public bool AllowInsignificantPropertyNameDifferences { get; } = false;

        public bool CanProvideProfiles(Shader shader)
        {
            return shader.name.Contains("Advanced Dynamic Shaders");
        }

        public bool FailOnMissingTexture(TextureMap texture)
        {
            switch (texture)
            {
                case TextureMap.Albedo:
                case TextureMap.Normal_TS:
                    return true;
            }
            
            return false;
        }

        private InputTextureProfile[] _standard = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOHS, "_SurfaceTex", "_MetallicGlossMap"),
        };

        private InputTextureProfile[] _standardTrunk = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOHS, "_SurfaceTex", "_MetallicGlossMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Albedo, "_AlbedoTex3", "_MainTex3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_NormalTex3", "_BumpMap3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_MAOHS, "_SurfaceTex3", "_MetallicGlossMap3"),
        };

        private InputTextureProfile[] _simple = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
        };

        private InputTextureProfile[] _simpleTrunk = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Albedo, "_AlbedoTex3", "_MainTex3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_NormalTex3", "_BumpMap3"),
        };

        public IEnumerable<InputTextureProfile> GetInputProfiles(Material m)
        {
            var standard = m.HasProperty("_MetallicGlossMap");
            var trunk = m.HasProperty("_MainTex3");

            return trunk
                ? standard
                    ? _standardTrunk
                    : _simpleTrunk
                : standard
                    ? _standard
                    : _simple;
        }
    }
}
