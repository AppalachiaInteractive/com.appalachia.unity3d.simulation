using System.Collections.Generic;
using Appalachia.Core.Attributes;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    [CallStaticConstructorInEditor]
    public abstract class OutputMaterialShader : IOutputMaterialShader
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static OutputMaterialShader()
        {
            GSR.InstanceAvailable += i => _GSR = i;
        }

        protected static GSR _GSR;
        
        public abstract string Name { get; }

        public abstract LazyShader Shader { get; }

        public abstract IEnumerable<OutputTextureProfile> GetOutputProfiles(bool atlas);

        public abstract void FinalizeSettings(Material material, bool atlas);
        
    }
}
