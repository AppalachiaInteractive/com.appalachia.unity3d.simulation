using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Settings", "Controls the generation of the branch.", TitleAlignments.Split)]
    public class BranchSettings : TypeBasedSettings<BranchSettings>
    {
        private BranchSettings()
        {
        }

        #region Fields and Autoproperties

        [TabGroup("Settings", "Occlusion", Paddingless = true)]
        [InlineProperty, HideLabel]
        public AmbientOcclusionSettings ao;

        [TabGroup("Settings", "LOD", Paddingless = true), InlineProperty, HideLabel]
        public LevelOfDetailSettings lod;

        [TabGroup("Settings", "Materials", Paddingless = true), InlineProperty, HideLabel]
        public MaterialSettings material;

        [TabGroup("Settings", "Mesh", Paddingless = true), InlineProperty, HideLabel]
        public MeshSettings mesh;

        [HideInInspector] public QualityMode qualityMode = QualityMode.Working;

        [TabGroup("Settings", "Textures", Paddingless = true), InlineProperty, HideLabel]
        public TextureSettings texture;

        #endregion

        public static BranchSettings Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("branch-settings");
            var instance = LoadOrCreateNew<BranchSettings>(folder, assetName);

            instance.ao = new AmbientOcclusionSettings(ResponsiveSettingsType.Branch);
            instance.material = new MaterialSettings(ResponsiveSettingsType.Branch);
            instance.texture = new TextureSettings(ResponsiveSettingsType.Branch);
            instance.lod = new LevelOfDetailSettings(0, ResponsiveSettingsType.Branch);
            instance.mesh = new MeshSettings(ResponsiveSettingsType.Branch);

            return instance;
        }

        /// <inheritdoc />
        public override void CopySettingsTo(BranchSettings t)
        {
            ao.CopySettingsTo(t.ao);
            lod.CopySettingsTo(t.lod);
            material.CopySettingsTo(t.material);
            mesh.CopySettingsTo(t.mesh);
            texture.CopySettingsTo(t.texture);
            t.qualityMode = qualityMode;
        }
    }
}
