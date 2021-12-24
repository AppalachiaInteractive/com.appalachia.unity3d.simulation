using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public class Internal_Leaf_LOD0_OutputMaterialShader : OutputMaterialShader
    {
        public static string Key = _GSR.leafShaders[0].name;
        public override string Name { get; } = Key;

        public override LazyShader Shader { get; } = new LazyShader(_GSR.leafShaders[0]);

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
