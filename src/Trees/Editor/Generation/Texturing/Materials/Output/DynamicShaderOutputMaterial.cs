using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public abstract class DynamicShaderOutputMaterial : OutputMaterial
    {
        /*[VerticalGroup("RIGHT", Order = 10), LabelWidth(110)]
        public Material prototypeMaterial;*/

        protected DynamicShaderOutputMaterial(int materialID, ResponsiveSettingsType settingsType) : base(
            materialID,
            settingsType
        )
        {
            _textureSet = new OutputTextureSet();
        }
    }
}
