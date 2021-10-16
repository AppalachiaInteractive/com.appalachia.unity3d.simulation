/*
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Metadata.Texturing.Output;
using Appalachia.Simulation.Trees.Metadata.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Metadata.Texturing.Shading.OutputShaders
{
    public class Internal_TreeShadows_LOD1_OutputMaterialShader : OutputMaterialShader
    {
        public static string Key = GSR.instance.shadowShaders[1].name;
        public override string Name { get; } = Key;
        
        public override LazyShader Shader { get; } = new LazyShader(GSR.instance.shadowShaders[1]);

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas) =>
            new[]
            {
                OutputTextureProfileFactory.Get(TextureMap.Albedo, "_MainTex", atlas),
            };
        
        public override void FinalizeSettings(Material material, bool atlas)
        {
        }
    }
}
*/
