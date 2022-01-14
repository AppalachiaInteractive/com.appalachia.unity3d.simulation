using System.Collections.Generic;
using Appalachia.Simulation.Trees.Generation.Texturing.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Specifications;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders
{
    public interface IInputMaterialShader
    {
        /// <summary>
        ///     Whether or not differences between the specified properties names returned from
        ///     <see cref="GetInputProfiles" /> and the properties actually on the material should
        ///     be ignored.  Differences in hyphenation, capitalization, etc.
        /// </summary>
        bool AllowInsignificantPropertyNameDifferences { get; }

        /// <summary>
        ///     The priority of this shader, in relative terms to the other existing input material shaders.
        ///     If multiple implementations can provide input texture profiles for a given material, the
        ///     priority of each will be used to determine which will be utilized.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     Does this class provide input texture profiles for the provided shader?
        /// </summary>
        /// <param name="shader">The shader in question.</param>
        /// <returns>A value indicating whether or not profiles can be provided for the shader.</returns>
        bool CanProvideProfiles(Shader shader);

        /// <summary>
        ///     Will be invoked when the specified texture was not found in the material.
        ///     Return
        ///     <value>true</value>
        ///     if the execution should fail as a result.
        /// </summary>
        /// <param name="texture">The texture in question.</param>
        /// <returns>A value indicating whether or not material generation should continue.</returns>
        bool FailOnMissingTexture(TextureMap texture);

        /// <summary>
        ///     Returns the input texture profiles that enable texture extraction from the provided material.
        ///     The material is provided to enable the checking of relevant properties.
        ///     For example, on the Unity Standard shader, the glossiness may be in one of two different
        ///     channels, depending on the
        ///     <value>_SmoothnessTextureChannel</value>
        ///     value.
        /// </summary>
        IEnumerable<InputTextureProfile> GetInputProfiles(Material m);
    }
}
