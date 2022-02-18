using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Settings", titleAlignment: TitleAlignments.Centered)]
    public class TreeSettings : TypeBasedSettings<TreeSettings>
    {
        private TreeSettings()
        {
        }

        #region Fields and Autoproperties

        [TabGroup("Settings",              "Appearance", Paddingless = true)]
        [TabGroup("Settings/Appearance/A", "Occlusion",  Paddingless = true), InlineProperty, HideLabel]
        public AmbientOcclusionSettings ao;

        [TabGroup("Settings/Assets/A", "Collision", Paddingless = true), InlineProperty, HideLabel]
        public CollisionSettings collision;

        [TabGroup("Settings",          "Assets", Paddingless = true)]
        [TabGroup("Settings/Assets/A", "LOD",    Paddingless = true), InlineProperty, HideLabel]
        public LevelOfDetailSettingsCollection lod;

        [TabGroup("Settings/Appearance/A", "Materials", Paddingless = true), InlineProperty, HideLabel]
        public MaterialSettings material;

        [TabGroup("Settings/Assets/A", "Meshes", Paddingless = true), InlineProperty, HideLabel]
        public MeshSettings mesh;

        [HideInInspector] public QualityMode qualityMode = QualityMode.Working;

        [TabGroup("Settings/Assets/A", "Shadow", Paddingless = true), InlineProperty, HideLabel]
        [ShowIf(nameof(showShadow))]
        public ShadowCasterSettings shadow;

        [TabGroup("Settings/Assets/A", "Textures", Paddingless = true), InlineProperty, HideLabel]
        public TextureSettings texture;

        [TabGroup("Settings/Appearance/A", "Transmission", Paddingless = true), InlineProperty, HideLabel]
        public TransmissionSettings transmission;

        [TabGroup("Settings", "Variants", Paddingless = true), InlineProperty, HideLabel]
        public TreeVariantSettings variants;

        [TabGroup("Settings/Appearance/A", "Wind", Paddingless = true), InlineProperty, HideLabel]
        public WindSettings wind;

        #endregion

        private bool showShadow => lod?.shadowCaster ?? false;

        public static TreeSettings Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("settings");
            var instance = LoadOrCreateNew<TreeSettings>(folder, assetName);

            instance.ao = new AmbientOcclusionSettings(ResponsiveSettingsType.Tree);
            instance.transmission = new TransmissionSettings(ResponsiveSettingsType.Tree);
            instance.wind = new WindSettings(ResponsiveSettingsType.Tree);
            instance.lod = new LevelOfDetailSettingsCollection(ResponsiveSettingsType.Tree);
            instance.material = new MaterialSettings(ResponsiveSettingsType.Tree);
            instance.texture = new TextureSettings(ResponsiveSettingsType.Tree);
            instance.mesh = new MeshSettings(ResponsiveSettingsType.Tree);
            instance.collision = new CollisionSettings(ResponsiveSettingsType.Tree);

            return instance;
        }

        /// <inheritdoc />
        public override void CopySettingsTo(TreeSettings t)
        {
            ao.CopySettingsTo(t.ao);
            transmission.CopySettingsTo(t.transmission);
            wind.CopySettingsTo(t.wind);
            lod.CopySettingsTo(t.lod);
            material.CopySettingsTo(t.material);
            texture.CopySettingsTo(t.texture);
            mesh.CopySettingsTo(t.mesh);
            collision.CopySettingsTo(t.collision);
            variants.CopySettingsTo(t.variants);
        }

        public void Check()
        {
            if (ao == null)
            {
                ao = new AmbientOcclusionSettings(ResponsiveSettingsType.Tree);
            }

            if (transmission == null)
            {
                transmission = new TransmissionSettings(ResponsiveSettingsType.Tree);
            }

            if (wind == null)
            {
                wind = new WindSettings(ResponsiveSettingsType.Tree);
            }

            if (lod == null)
            {
                lod = new LevelOfDetailSettingsCollection(ResponsiveSettingsType.Tree);
            }

            if (material == null)
            {
                material = new MaterialSettings(ResponsiveSettingsType.Tree);
            }

            if (texture == null)
            {
                texture = new TextureSettings(ResponsiveSettingsType.Tree);
            }

            if (mesh == null)
            {
                mesh = new MeshSettings(ResponsiveSettingsType.Tree);
            }

            if (collision == null)
            {
                collision = new CollisionSettings(ResponsiveSettingsType.Tree);
            }

            if (variants == null)
            {
                variants = new TreeVariantSettings();
            }
        }
    }
}
