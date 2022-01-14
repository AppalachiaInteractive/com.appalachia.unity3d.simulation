using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class CTI_301_InputMaterialShader : IInputMaterialShader
    {
        #region Fields and Autoproperties

        private InputTextureProfile[] _leaves =
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo,                 "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.NormalTSSpecularityMap, "_BumpSpecMap"),
            InputTextureProfileFactory.Get(TextureMap.MAOTS,                  "_TranslucencyMap"),
        };

        private InputTextureProfile[] _trunk =
        {
            InputTextureProfileFactory.Get(TextureMap.AlbedoGlossiness,                "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.NormalTSSpecularityOcclusionMap, "_BumpSpecAOMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_AlbedoGlossiness,        "_DetailAlbedoMap"),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_NormalTSSpecularityOcclusionMap,
                "_DetailNormalMapX"
            ),
        };

        private InputTextureProfile[] _trunkShadowMap =
        {
            InputTextureProfileFactory.Get(TextureMap.AlbedoGlossiness,                "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.NormalTSSpecularityOcclusionMap, "_BumpSpecAOMap"),
            InputTextureProfileFactory.Get(TextureMap.Variant_AlbedoGlossiness,        "_DetailAlbedoMap"),
            InputTextureProfileFactory.Get(
                TextureMap.Variant_NormalTSSpecularityOcclusionMap,
                "_DetailNormalMapX"
            ),
            InputTextureProfileFactory.Get(TextureMap.ShadowMap, "_ShadowMap"),
        };

        #endregion

        #region IInputMaterialShader Members

        public int Priority { get; } = 0;

        public bool AllowInsignificantPropertyNameDifferences { get; } = false;

        public bool CanProvideProfiles(Shader shader)
        {
            return shader.name.Contains("CTI") && shader.name.Contains("301");
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

        public IEnumerable<InputTextureProfile> GetInputProfiles(Material m)
        {
            var shadowMap = m.GetFloat("_EnableShadowMap") > 0;
            var trunk = m.HasProperty("_BumpSpecAOMap");

            return trunk
                ? shadowMap
                    ? _trunkShadowMap
                    : _trunk
                : _leaves;
        }

        #endregion
    }
}
