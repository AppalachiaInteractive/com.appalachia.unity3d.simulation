using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public class StandardSpecular_OutputMaterialShader : OutputMaterialShader
    {
        #region Fields and Autoproperties

        public override LazyShader Shader { get; } = new LazyShader("Standard (Specular setup)");
        public override string Name { get; } = "Standard (Specular setup)";

        #endregion

        public override void FinalizeSettings(Material material, bool atlas)
        {
            material.SetFloat("_SmoothnessTextureChannel", 0f);

            if (atlas)
            {
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_Mode", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
            else
            {
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_Mode", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.renderQueue = -1;
            }
        }

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas)
        {
            return new[]
            {
                OutputTextureProfileFactory.Get(TextureMap.Albedo,                "_MainTex",         atlas),
                OutputTextureProfileFactory.Get(TextureMap.SpecularityGlossiness, "_SpecGlossMap",    atlas),
                OutputTextureProfileFactory.Get(TextureMap.Normal_TS,             "_BumpMap",         atlas),
                OutputTextureProfileFactory.Get(TextureMap.Displacement,          "_ParallaxMap",     atlas),
                OutputTextureProfileFactory.Get(TextureMap.AmbientOcclusion,      "_OcclusionMap",    atlas),
                OutputTextureProfileFactory.Get(TextureMap.Emission,              "_EmissionMap",     atlas),
                OutputTextureProfileFactory.Get(TextureMap.DetailMask,            "_DetailMask",      atlas),
                OutputTextureProfileFactory.Get(TextureMap.AlbedoDetail,          "_DetailAlbedoMap", atlas),
                OutputTextureProfileFactory.Get(TextureMap.NormalBump_TS,         "_DetailNormalMap", atlas)
            };
        }
    }
}
