using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings.Log
{
    [Serializable]
    [Title("Material Settings", TitleAlignment = TitleAlignments.Centered)]
    public class LogMaterialSettings : ResponsiveSettings
    {
        [PropertyTooltip("Should images be arranged on the atlas based on the mesh " +
            "area they take up, or equally?")]
        [BoxGroup("Atlassing")]
        [ToggleLeft]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public bool imageAtlasIsProportionalToArea;

        [FoldoutGroup("Tiled Material")]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color trunkColor = Color.white;
        
        [FoldoutGroup("Tiled Material")]
        [PropertyRange(0f, 3f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkNormalScale = 1.0f;
        
        [FoldoutGroup("Tiled Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkSmoothness = 0.2f;
        
        [FoldoutGroup("Tiled Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkVariation;
        
        [FoldoutGroup("Tiled Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkTextureOcclusion = 0.6f;
        
        [FoldoutGroup("Tiled Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkVertexOcclusion = 0.5f;

        [FoldoutGroup("Tiled Material (Base Variation)")]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color baseColor = Color.white;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 3f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseNormalScale = 1.0f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseSmoothness = .2f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseOcclusion = 0.6f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0.0f, 20.0f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseBlendHeight = 2.0f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0.0001f, 1.0f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseBlendAmount = 0.1f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseBlendVariation = 0.1f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(-1f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkHeightOffset;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float trunkHeightRange = 1.0f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseBlendHeightContrast = 0.5f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(-1f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseHeightOffset;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseHeightRange = 0.9f;
        
        [FoldoutGroup("Tiled Material (Base Variation)")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float baseBlendNormals;
        
        [FoldoutGroup("Atlas Material")]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color leafColor = Color.white;
        
        [FoldoutGroup("Atlas Material")]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Color nonLeafColor = Color.white;
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0.1f, 2f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafSaturation = 1.0f;
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0.1f, 2f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafBrightness = 1.0f;
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 3f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafNormalScale = 1.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 20f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafTransmission = 2.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafTransmissionCutoff = 0.95f;
 
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float occlusionTransmissionDamping = 0.5f;

        [FoldoutGroup("Atlas Material")]
        [MinMaxSlider(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public Vector2 leafTransparency = new Vector2(.05f, .4f);
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafSmoothness = 0.35f;
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafTextureOcclusion = 0.6f;
        
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float leafVertexOcclusion = 0.5f;

        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 50f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyStrength = 2.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyNormalDistortion = 0.175f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 50f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyScatteringFalloff = 2.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyDirect = 1.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyAmbient = 1.0f;
        
        [HideIf(nameof(hideTreeSettings))]
        [FoldoutGroup("Atlas Material")]
        [PropertyRange(0f, 1f), OnValueChanged(nameof(MaterialPropertiesChanged))]
        public float translucencyShadow = 0.83f;
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Define the materials used when new hierarchies are created.")]
        [FoldoutGroup("Default Materials")]
        [HideLabel]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public DefaultMaterialSettingsPublic defaultMaterials;

        [PropertyTooltip("Should the material prototype be set automatically based on surface area?")]
        [FoldoutGroup("Prototypes")]
        [ToggleLeft]
        [OnValueChanged(nameof(MaterialPropertiesChanged))]
        public bool enableDynamicMaterialPrototypes;

        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Define if the materials are generated the next time the tree is generated.")]
        [BoxGroup("Material Generation")]
        [ToggleLeft]
        public bool forceRegenerateMaterials;
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Define if the materials are always re-generated.")]
        [InfoBox("If enabled, this will regenerate the materials every time the tree is built!", VisibleIf = nameof(forceRegenerateMaterials), InfoMessageType = InfoMessageType.Warning)]
        [BoxGroup("Material Generation")]
        [ToggleLeft]
        public bool doNotUnsetForceRegenerateMaterials;

        
        private bool hideTreeSettings => settingsType != ResponsiveSettingsType.Tree;
        
        public LogMaterialSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            defaultMaterials = new DefaultMaterialSettingsPublic(settingsType);
        }
    }
}
