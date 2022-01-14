using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public class Internal_Bark_LOD0_OutputMaterialShader : OutputMaterialShader
    {
        #region Static Fields and Autoproperties

        public static string Key = _GSR.barkShaders[0].name;

        #endregion

        #region Fields and Autoproperties

        public override LazyShader Shader { get; } = new LazyShader(_GSR.barkShaders[0]);
        public override string Name { get; } = Key;

        #endregion

        public override void FinalizeSettings(Material material, bool atlas)
        {
            if (material.GetTexture("_MainTex3"))
            {
                material.SetFloat("_EnableBase", 1);
            }
            else
            {
                material.SetFloat("_EnableBase", 0);
            }
        }

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas)
        {
            return new[]
            {
                OutputTextureProfileFactory.Get(TextureMap.Albedo,            "_MainTex",           atlas),
                OutputTextureProfileFactory.Get(TextureMap.Normal_TS,         "_BumpMap",           atlas),
                OutputTextureProfileFactory.Get(TextureMap.MAOHS,             "_MetallicGlossMap",  atlas),
                OutputTextureProfileFactory.Get(TextureMap.Variant_Albedo,    "_MainTex3",          atlas),
                OutputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_BumpMap3",          atlas),
                OutputTextureProfileFactory.Get(TextureMap.Variant_MAOHS,     "_MetallicGlossMap3", atlas)
            };
        }
    }
}
