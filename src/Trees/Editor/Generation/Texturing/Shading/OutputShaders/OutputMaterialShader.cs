using System.Collections.Generic;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    [CallStaticConstructorInEditor]
    public abstract class OutputMaterialShader : IOutputMaterialShader
    {
        static OutputMaterialShader()
        {
            RegisterInstanceCallbacks.For<OutputMaterialShader>()
                                     .When.Object<GSR>()
                                     .IsAvailableThen(i => _GSR = i);
        }

        #region Static Fields and Autoproperties

        public static GSR _GSR;

        #endregion

        #region IOutputMaterialShader Members

        public abstract string Name { get; }

        public abstract LazyShader Shader { get; }

        public abstract IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas);

        public abstract void FinalizeSettings(Material material, bool atlas);

        #endregion
    }
}
