using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    public interface IOutputMaterialShader
    {
        /// <summary>
        /// A name to identify this shader option.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The shader to use to render the optimized material.
        /// </summary>
        LazyShader Shader { get; }

        /// <summary>
        /// Returns the output texture profiles that enable texture assignment to the generated material.
        /// </summary>
        IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas);

        /// <summary>
        /// Opportunity to update any shader specific settings that are not dependent on the texture generation.
        /// </summary>
        /// <param name="material"></param>
        void FinalizeSettings(Material material, bool atlas);
    }
    
    
}