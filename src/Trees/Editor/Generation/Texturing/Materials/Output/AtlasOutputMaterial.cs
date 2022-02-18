using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public class AtlasOutputMaterial : DynamicShaderOutputMaterial
    {
        public AtlasOutputMaterial(int materialID, ResponsiveSettingsType settingsType) : base(
            materialID,
            settingsType
        )
        {
        }

        /// <inheritdoc />
        public override MaterialContext MaterialContext => MaterialContext.AtlasOutputMaterial;

        /// <inheritdoc />
        protected override OutputShaderSelectionSet defaultShaders => _defaultShaderResource.atlasShaderSet;
    }
}
