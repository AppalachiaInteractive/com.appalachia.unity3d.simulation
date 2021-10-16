using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Collision", TitleAlignment = TitleAlignments.Centered)]
    public class CollisionSettings : ResponsiveSettings
    {

        [PropertyTooltip("Toggles collider generation on or off.")]
        [ToggleLeft]
        [OnValueChanged(nameof(CollisionSettingsChanged))]
        public bool generateColliders = true;

        [PropertyTooltip("Minimum spline radius for colliders to be placed on.")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        [MinMaxSlider(.01f, 1f)]
        public Vector2 minimumSplineRadiusByHeight = new Vector2(.01f, 1f);
        
        [PropertyTooltip("Adjust collision mesh quality.")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        public float maximumHeight = 20f;
        
        [PropertyTooltip("Adjust collision mesh quality.")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        public float minimumHeight = -.25f;
        
        [FormerlySerializedAs("collisionMeshSettings")]
        [PropertyTooltip("Adjust collision mesh quality.")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        public LevelOfDetailSettings quality;

        [FormerlySerializedAs("collisionMeshSettings2")]
        [PropertyTooltip("Adjust collision mesh properties.")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        public MeshSettings properties;
        
        public CollisionSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            quality = new LevelOfDetailSettings(100, settingsType)
            {
                leafFullness = 0,
                showFruit = true,
                showFungus = true,
                showKnots = true,
                branchesGeometryQuality = 0,
                rootsGeometryQuality = 0,
                leafGeometryQuality = 0,
                trunkGeometryQuality = 0,
                resampleLeaves = false,
                resampleOnTrunkOnly = false,
                resampleSplineAtChildren = false,
                resampleAddOns = false
            };

            properties = new MeshSettings(settingsType) {recalculateNormals = false, recalculateTangents = false};
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is CollisionSettings cast)
            {
                properties.CopySettingsTo(cast.properties);
                quality.CopySettingsTo(cast.quality);
                cast.generateColliders = generateColliders;
                cast.maximumHeight = maximumHeight;
                cast.minimumHeight = minimumHeight;
                cast.minimumSplineRadiusByHeight = minimumSplineRadiusByHeight;
            }
        }
        
        [Button]
        public void PushToAll()
        {
            var trees = AssetDatabaseManager.FindAssets("t:TreeDataContainer");

            for (var i = 0; i < trees.Length; i++)
            {
                var treeGuid = trees[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(treePath);

                var settings = tree.settings.collision;

                if (settings == this)
                {
                    continue;
                }

                CopySettingsTo(settings);

                EditorUtility.SetDirty(tree);
                EditorUtility.SetDirty(tree.settings);

                tree.Save();
            }
        }
    }
}
