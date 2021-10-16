using System.Collections.Generic;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class Internal_Tree_InputMaterialShader : IInputMaterialShader
    {
        public int Priority { get; } = 0;
        
        public bool AllowInsignificantPropertyNameDifferences { get; } = false;

        public bool CanProvideProfiles(Shader shader)
        {
            return GSR.instance.barkShaders.Contains(shader) || GSR.instance.leafShaders.Contains(shader);
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

        private InputTextureProfile[] _bark = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOHS, "_SurfaceTex", "_MetallicGlossMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Albedo, "_AlbedoTex3", "_MainTex3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_NormalTex3", "_BumpMap3"),
            InputTextureProfileFactory.Get(TextureMap.Variant_MAOHS, "_SurfaceTex3", "_MetallicGlossMap3"),
        };
        
        private InputTextureProfile[] _leaf = new[]
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo, "_AlbedoTex", "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS, "_NormalTex", "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOTS, "_SurfaceTex", "_MetallicGlossMap"),
        };



        public IEnumerable<InputTextureProfile> GetInputProfiles(Material m)
        {
            if (GSR.instance.barkShaders.Contains(m.shader))
            {
                return _bark;
            }
            
            return _leaf;
        }
    }
}
