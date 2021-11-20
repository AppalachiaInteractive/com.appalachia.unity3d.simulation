using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings.Log
{
    [Serializable]
    [Title("Settings", "Control the generation of all logs.", TitleAlignments.Split)]
    public class LogSettings : TypeBasedSettings<LogSettings>
    {
        private LogSettings()
        {
        }

        #region Fields and Autoproperties

        [TabGroup("Settings", "Collision", Paddingless = true), InlineProperty, HideLabel]
        public CollisionSettings collision;

        [TabGroup("Settings", "LOD", Paddingless = true), InlineProperty, HideLabel]
        public LevelOfDetailSettingsCollection lod;

        [TabGroup("Settings", "Materials", Paddingless = true), InlineProperty, HideLabel]
        public LogMaterialSettings material;

        [TabGroup("Settings", "Meshes", Paddingless = true), InlineProperty, HideLabel]
        public MeshSettings mesh;

        [HideInInspector] public QualityMode qualityMode = QualityMode.Working;

        [TabGroup("Settings", "Textures", Paddingless = true), InlineProperty, HideLabel]
        public TextureSettings texture;

        [TabGroup("Settings", "Log", Paddingless = true), InlineProperty, HideLabel]
        public VertexSettings vertex;

        #endregion

        public static LogSettings Create(string folder, NameBasis nameBasis)
        {
            var assetName = nameBasis.FileNameSO("settings");
            var instance = LoadOrCreateNew<LogSettings>(folder, assetName);

            instance.vertex = new VertexSettings(ResponsiveSettingsType.Log);
            instance.lod = new LevelOfDetailSettingsCollection(ResponsiveSettingsType.Log);
            instance.material = new LogMaterialSettings(ResponsiveSettingsType.Log);
            instance.texture = new TextureSettings(ResponsiveSettingsType.Log);
            instance.mesh = new MeshSettings(ResponsiveSettingsType.Log);
            instance.collision = new CollisionSettings(ResponsiveSettingsType.Log);

            return instance;
        }

        public void Check()
        {
            if (vertex == null)
            {
                vertex = new VertexSettings(ResponsiveSettingsType.Log);
            }

            if (lod == null)
            {
                lod = new LevelOfDetailSettingsCollection(ResponsiveSettingsType.Log);
            }

            if (material == null)
            {
                material = new LogMaterialSettings(ResponsiveSettingsType.Log);
            }

            if (texture == null)
            {
                texture = new TextureSettings(ResponsiveSettingsType.Log);
            }

            if (mesh == null)
            {
                mesh = new MeshSettings(ResponsiveSettingsType.Log);
            }

            if (collision == null)
            {
                collision = new CollisionSettings(ResponsiveSettingsType.Log);
            }
        }
    }
}
