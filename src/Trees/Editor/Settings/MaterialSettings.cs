using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Material Generation", TitleAlignment = TitleAlignments.Centered)]
    public class MaterialSettings : ResponsiveSettings
    {
        [PropertyTooltip("Should images be arranged on the atlas based on the mesh " +
            "area they take up, or equally?")]
        [BoxGroup("Atlassing")]
        [ToggleLeft]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public bool imageAtlasIsProportionalToArea;

   
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Define the materials used when new hierarchies are created.")]
        [FoldoutGroup("Default Materials")]
        [HideLabel]
        [OnValueChanged(nameof(MaterialGenerationChanged))]
        public DefaultMaterialSettingsPublic defaultMaterials;

        private bool hideTreeSettings => settingsType != ResponsiveSettingsType.Tree;
        
        public MaterialSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            defaultMaterials = new DefaultMaterialSettingsPublic(settingsType);
        }
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is MaterialSettings cast)
            {
                cast.imageAtlasIsProportionalToArea = imageAtlasIsProportionalToArea;
            }
        }
    }
}
