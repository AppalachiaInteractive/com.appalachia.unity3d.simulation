using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public class Internal_TreeShadows_LOD0_OutputMaterialShader : OutputMaterialShader
    {
        #region Static Fields and Autoproperties

        public static string Key = _GSR.shadowShaders[0].name;

        #endregion

        #region Fields and Autoproperties

        public override LazyShader Shader { get; } = new LazyShader(_GSR.shadowShaders[0]);
        public override string Name { get; } = Key;

        #endregion

        public override void FinalizeSettings(Material material, bool atlas)
        {
        }

        public override IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas)
        {
            return new[] { OutputTextureProfileFactory.Get(TextureMap.Albedo, "_MainTex", atlas), };
        }
    }
}
