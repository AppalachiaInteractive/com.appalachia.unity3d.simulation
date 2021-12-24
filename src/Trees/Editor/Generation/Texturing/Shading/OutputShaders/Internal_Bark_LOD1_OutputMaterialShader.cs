/*using System.Collections.Generic;
using Appalachia.Simulation.Trees.Metadata.Texturing.Output;
using Appalachia.Simulation.Trees.Metadata.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Metadata.Texturing.Shading.OutputShaders
{

    public class Internal_Bark_LOD1_OutputMaterialShader : OutputMaterialShader
    {
        public static string Key = _GSR.barkShaders[1].name;
        public override string Name { get; } = Key;
        
        public override LazyShader Shader { get; } = new LazyShader(_GSR.barkShaders[1]);

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas) =>
            new[]
            {
                OutputTextureProfileFactory.Get(TextureMap.Albedo, "_MainTex", atlas),
                OutputTextureProfileFactory.Get(TextureMap.Normal_TS, "_BumpMap", atlas),
                
                OutputTextureProfileFactory.Get(TextureMap.Variant_Albedo, "_MainTex3", atlas),
                OutputTextureProfileFactory.Get(TextureMap.Variant_Normal_TS, "_BumpMap3", atlas),
            };
        
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
    }
}*/