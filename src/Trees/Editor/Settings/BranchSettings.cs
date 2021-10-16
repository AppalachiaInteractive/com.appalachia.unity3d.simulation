using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Settings", "Controls the generation of the branch.", titleAlignment: TitleAlignments.Split)]
    public class BranchSettings : TypeBasedSettings<BranchSettings>
    {
        [HideInInspector] public QualityMode qualityMode = QualityMode.Working;

        [TabGroup("Settings", "Occlusion", Paddingless = true)]
        [InlineProperty, HideLabel]
        public AmbientOcclusionSettings ao;

        [TabGroup("Settings", "Materials", Paddingless = true), InlineProperty, HideLabel]
        public MaterialSettings material;

        [TabGroup("Settings", "Textures", Paddingless = true), InlineProperty, HideLabel]
        public TextureSettings texture;

        [TabGroup("Settings", "LOD", Paddingless = true), InlineProperty, HideLabel]
        public LevelOfDetailSettings lod;

        [TabGroup("Settings", "Mesh", Paddingless = true), InlineProperty, HideLabel]
        public MeshSettings mesh;


        private BranchSettings()
        {

        }

        public static BranchSettings Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("branch-settings");
            var instance = LoadOrCreateNew(folder, assetName);

            instance.ao = new AmbientOcclusionSettings(ResponsiveSettingsType.Branch);
            instance.material = new MaterialSettings(ResponsiveSettingsType.Branch);
            instance.texture = new TextureSettings(ResponsiveSettingsType.Branch);
            instance.lod = new LevelOfDetailSettings(0, ResponsiveSettingsType.Branch);
            instance.mesh = new MeshSettings(ResponsiveSettingsType.Branch);

            return instance;
        }

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
