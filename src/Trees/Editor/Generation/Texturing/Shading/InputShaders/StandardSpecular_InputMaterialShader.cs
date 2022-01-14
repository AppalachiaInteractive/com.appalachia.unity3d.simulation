using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public class StandardSpecular_InputMaterialShader : IInputMaterialShader
    {
        #region Fields and Autoproperties

        private InputTextureProfile[] _profiles_AlbedoAlpha =
        {
            InputTextureProfileFactory.Get(TextureMap.AlbedoGlossiness, "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.Specularity,      "_SpecGlossMap"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS,        "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.Displacement,     "_ParallaxMap"),
            InputTextureProfileFactory.Get(TextureMap.AmbientOcclusion, "_OcclusionMap"),
            InputTextureProfileFactory.Get(TextureMap.Emission,         "_EmissionMap"),
            InputTextureProfileFactory.Get(TextureMap.DetailMask,       "_DetailMask"),
            InputTextureProfileFactory.Get(TextureMap.AlbedoDetail,     "_DetailAlbedoMap"),
            InputTextureProfileFactory.Get(TextureMap.NormalBump_TS,    "_DetailNormalMap")
        };

        private InputTextureProfile[] _profiles_SpecularAlpha =
        {
            InputTextureProfileFactory.Get(TextureMap.Albedo,                "_MainTex"),
            InputTextureProfileFactory.Get(TextureMap.SpecularityGlossiness, "_SpecGlossMap"),
            InputTextureProfileFactory.Get(TextureMap.Normal_TS,             "_BumpMap"),
            InputTextureProfileFactory.Get(TextureMap.Displacement,          "_ParallaxMap"),
            InputTextureProfileFactory.Get(TextureMap.AmbientOcclusion,      "_OcclusionMap"),
            InputTextureProfileFactory.Get(TextureMap.Emission,              "_EmissionMap"),
            InputTextureProfileFactory.Get(TextureMap.DetailMask,            "_DetailMask"),
            InputTextureProfileFactory.Get(TextureMap.AlbedoDetail,          "_DetailAlbedoMap"),
            InputTextureProfileFactory.Get(TextureMap.NormalBump_TS,         "_DetailNormalMap")
        };

        #endregion

        #region IInputMaterialShader Members

        public int Priority { get; } = 100;

        public bool AllowInsignificantPropertyNameDifferences { get; } = true;

        public bool CanProvideProfiles(Shader shader)
        {
            return shader.name == "Standard (Specular setup)";
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
            if (m.GetFloat("_SmoothnessTextureChannel") <= 0.001f)
            {
                return _profiles_SpecularAlpha;
            }

            return _profiles_AlbedoAlpha;
        }

        #endregion
    }
}
