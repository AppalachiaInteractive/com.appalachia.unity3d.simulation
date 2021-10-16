using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Mesh & Prefab Generation", TitleAlignment = TitleAlignments.Centered)]
    public class MeshSettings : ResponsiveSettings
    {
        [PropertyTooltip("The normal value to use when generating leaf billboards.")]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("A", .5f)]
        [InfoBox("Billboard Normal Factor", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(GeometrySettingsChanged))]
        public float generatedBillboardNormalFactor = 0.8f;

        [PropertyTooltip("The normal value to use when generating leaf planes.")]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("A", .5f)]
        [InfoBox("Plane Normal Factor", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(GeometrySettingsChanged))]
        public float generatedPlaneNormalFactor = 0.8f;

        [PropertyTooltip("Should normals be recalculated when producing the mesh?")]
        [HorizontalGroup("B")]
        [InfoBox("Recalculate Normals", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MeshSettingsChanged))]
        //[ToggleLeft]
        public bool recalculateNormals;

        [PropertyTooltip("Should tangents be recalculated when producing the mesh?")]
        [HorizontalGroup("B")]
        [InfoBox("Recalculate Tangents", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MeshSettingsChanged))]
        //[ToggleLeft]
        public bool recalculateTangents;
        
        [PropertyTooltip("Should normals be recalculated when producing the mesh?")]
        [PropertyRange(0f, 179f)]
        [HorizontalGroup("C")]
        [InfoBox("Hard Edge Angle", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MeshSettingsChanged))]
        [EnableIf(nameof(recalculateNormals))]
        public float hardEdgeAngle = 60f;

        [PropertyTooltip("Should normals be recalculated when producing the mesh?")]
        [PropertyRange(100, 1000000)]
        [HorizontalGroup("C")]
        [InfoBox("Grouping Scale", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(MeshSettingsChanged))]
        [EnableIf(nameof(recalculateNormals))]
        public int groupingScale = 100000;
        
        public MeshSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is MeshSettings cast)
            {
                cast.groupingScale = groupingScale;
                cast.recalculateNormals = recalculateNormals;
                cast.recalculateTangents = recalculateTangents;
                cast.hardEdgeAngle = hardEdgeAngle;
                cast.generatedBillboardNormalFactor = generatedBillboardNormalFactor;
                cast.generatedPlaneNormalFactor = generatedPlaneNormalFactor;
            }
        }[Button]
        public void PushToAll()
        {
            var trees = AssetDatabaseManager.FindAssets("t:TreeDataContainer");

            for (var i = 0; i < trees.Length; i++)
            {
                var treeGuid = trees[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(treePath);

                var settings = tree.settings.mesh;

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