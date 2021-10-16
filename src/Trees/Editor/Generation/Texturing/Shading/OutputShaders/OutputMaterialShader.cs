using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public abstract class OutputMaterialShader : IOutputMaterialShader
    {
        public abstract string Name { get; }

        public abstract LazyShader Shader { get; }

        public abstract IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas);

        public abstract void FinalizeSettings(Material material, bool atlas);
        
    }
}
