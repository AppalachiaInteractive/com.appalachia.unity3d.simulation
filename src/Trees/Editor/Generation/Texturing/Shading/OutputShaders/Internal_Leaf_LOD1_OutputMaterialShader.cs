/*
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Metadata.Texturing.Output;
using Appalachia.Simulation.Trees.Metadata.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Metadata.Texturing.Shading.OutputShaders
{
    public class Internal_Leaf_LOD1_OutputMaterialShader : OutputMaterialShader
    {
        public static string Key = GSR.instance.leafShaders[1].name;
        public override string Name { get; } = Key;
        
        public override LazyShader Shader { get; } = new LazyShader(GSR.instance.leafShaders[1]);

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas) =>
            new[]
            {
                OutputTextureProfileFactory.Get(TextureMap.Albedo, "_MainTex", atlas),
                OutputTextureProfileFactory.Get(TextureMap.Normal_TS, "_BumpMap", atlas),
                OutputTextureProfileFactory.Get(TextureMap.MAOTS, "_MetallicGlossMap", atlas)
            };
        
        public override void FinalizeSettings(Material material, bool atlas)
        {
        }
    }
}
*/
