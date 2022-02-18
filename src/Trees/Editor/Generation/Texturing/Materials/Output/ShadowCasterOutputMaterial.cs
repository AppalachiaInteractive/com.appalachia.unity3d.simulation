using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public class ShadowCasterOutputMaterial : OutputMaterial
    {
        public ShadowCasterOutputMaterial(int materialID) : base(materialID, ResponsiveSettingsType.Tree)
        {
        }

        /// <inheritdoc />
        public override MaterialContext MaterialContext => MaterialContext.ShadowCaster;

        /// <inheritdoc />
        protected override OutputShaderSelectionSet defaultShaders => _defaultShaderResource.shadowShaderSet;
    }
}
