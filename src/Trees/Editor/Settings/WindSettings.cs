using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Wind", TitleAlignment = TitleAlignments.Centered)]
    public class WindSettings : ResponsiveSettings
    {
        public WindSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
        }

        #region Fields and Autoproperties

        [PropertyTooltip(
            "Whether or not wind should be baked into the vertex colors. (R - trunk, G - branch, B - leaf, A - phase"
        )]
        [ToggleLeft]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public bool generateWind = true;

        [ShowIfGroup("Wind", Condition = nameof(generateWind))]
        [FoldoutGroup("Wind/Mesh Settings")]
        [PropertyTooltip("Adjusts whether vertex wind data should be normalized to a 0 to 1 range..")]
        [ToggleLeft]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public bool normalizeWind;

        [FormerlySerializedAs("bendDropPerRadiusM")]
        [PropertyTooltip("Adjusts the amount of bending by radius.")]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/B", .33f)]
        [InfoBox("Bend Radius Drop (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchBendDropPerRadiusM = .8f;

        [PropertyTooltip("Increases the amount of wind bend on child branches.")]
        [PropertyRange(0.0f, 0.1f)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/B", .33f)]
        [InfoBox("Bend Length Inc. (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchBendPerLengthM = 0.02f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on branches per meter from origin (vertically / Y axis)."
        )]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/B", .33f)]
        [InfoBox("Branch Bend Range (rads)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchBendRange = .5f;

        [PropertyTooltip("Increases the amount of wind bend on child branches.")]
        [PropertyRange(.9f, 2f)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/A", .33f)]
        [InfoBox("Level Multiplier", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchLevelMultiplier = 1.10f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on branches per meter from origin (horizontally / XZ axis)."
        )]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Wind/Mesh Settings/Branch", CenterLabel = true)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/A", .33f)]
        [InfoBox("Horizontal (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchWindPerMeterHorizontal = .1f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on branches per meter from origin (vertically / Y axis)."
        )]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Branch/A", .33f)]
        [InfoBox("Vertical (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float branchWindPerMeterVertical = .1f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on leaves per meter from origin (horizontally / XZ axis)."
        )]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Wind/Mesh Settings/Leaf", CenterLabel = true)]
        [HorizontalGroup("Wind/Mesh Settings/Leaf/A", .5f)]
        [InfoBox("Horizontal (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float leafWindPerMeterHorizontal = .2f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on leaves per meter from origin (vertically / Y axis)."
        )]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Leaf/A", .5f)]
        [InfoBox("Vertical (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float leafWindPerMeterVertical = .1f;

        [PropertyTooltip("Decreases the amount of bending by trunk radius.")]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/B", .33f)]
        [InfoBox("Trunk Radius Drop (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkBendDropPerRadiusM = .8f;

        [PropertyTooltip("Increases the amount of wind bend on the trunk depending on its length.")]
        [PropertyRange(0.0f, 0.2f)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/B", .33f)]
        [InfoBox("Trunk Length Inc. (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkBendPerLengthM = 0.02f;

        [PropertyTooltip(
            "Adjusts the amount of possible bend on trunks per meter from origin (vertically / Y axis)."
        )]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/B", .33f)]
        [InfoBox("Trunk Bend Range (rads)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkBendRange = .5f;

        [PropertyTooltip("Increases the trunk dead zone, per meter of tree height..")]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Wind/Mesh Settings/Trunk", CenterLabel = true)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/A", .5f)]
        [InfoBox("Trunk Dead Zone/(m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkWindDeadZonePerMeter = .5f;

        [PropertyTooltip("Adjusts the amount of growth per meter of trunk.")]
        [PropertyRange(1.0001f, 1.02f)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/Z", .5f)]
        [InfoBox("Power (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkWindPerMeterPower = 1.015f;

        [PropertyTooltip(
            "Adjusts the amount of possible wind on trunks per meter from origin (vertically / Y axis)."
        )]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Wind/Mesh Settings/Trunk", CenterLabel = true)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/Z", .5f)]
        [InfoBox("Vertical (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public float trunkWindPerMeterVertical = .5f;

        [PropertyTooltip(
            "Adjusts when trunk wind will be generated.  This prevents wind from affecting the lower trunk."
        )]
        [MinMaxSlider(1f, 20f)]
        [HorizontalGroup("Wind/Mesh Settings/Trunk/A", .5f)]
        [InfoBox("Dead Zone Min/Max (m)", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(VertexDataSettingsChanged))]
        public Vector2 trunkWindDeadZoneVertical = new Vector2(5f, 10f);

        #endregion

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is WindSettings cast)
            {
                cast.generateWind = generateWind;
                cast.normalizeWind = normalizeWind;
                cast.branchBendRange = branchBendRange;
                cast.branchLevelMultiplier = branchLevelMultiplier;
                cast.trunkBendRange = trunkBendRange;
                cast.branchBendPerLengthM = branchBendPerLengthM;
                cast.branchWindPerMeterHorizontal = branchWindPerMeterHorizontal;
                cast.branchWindPerMeterVertical = branchWindPerMeterVertical;
                cast.leafWindPerMeterHorizontal = leafWindPerMeterHorizontal;
                cast.leafWindPerMeterVertical = leafWindPerMeterVertical;
                cast.trunkBendPerLengthM = trunkBendPerLengthM;
                cast.trunkWindPerMeterVertical = trunkWindPerMeterVertical;
                cast.trunkWindPerMeterPower = trunkWindPerMeterPower;
                cast.branchBendDropPerRadiusM = branchBendDropPerRadiusM;
                cast.trunkBendDropPerRadiusM = trunkBendDropPerRadiusM;
                cast.trunkWindDeadZonePerMeter = trunkWindDeadZonePerMeter;
                cast.trunkWindDeadZoneVertical = trunkWindDeadZoneVertical;
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

                var settings = tree.settings.wind;

                if (settings == this)
                {
                    continue;
                }

                CopySettingsTo(settings);

                tree.MarkAsModified();
                tree.settings.MarkAsModified();

                tree.Save();
            }
        }
    }
}
