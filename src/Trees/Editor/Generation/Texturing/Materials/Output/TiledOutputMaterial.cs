#region

using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public class TiledOutputMaterial : DynamicShaderOutputMaterial
    {
        [InlineProperty, HideLabel, InfoBox("UV Scale", InfoMessageType.None)]
        [OnValueChanged(nameof(UVSettingsChanged))]
        public UVScale uvScale;

        [SerializeField, HideInInspector]
        private int _inputMaterialID;

        public TiledOutputMaterial(
            int materialID,
            int inputMaterialID,
            UVScale uvScale,
            ResponsiveSettingsType settingsType) : base(materialID, settingsType)
        {
            this.uvScale = uvScale;
            _inputMaterialID = inputMaterialID;
        }

        public int inputMaterialID => _inputMaterialID;

        public override MaterialContext MaterialContext => MaterialContext.TiledOutputMaterial;

        protected override OutputShaderSelectionSet defaultShaders => _defaultShaderResource.tiledShaderSet;
    }
}