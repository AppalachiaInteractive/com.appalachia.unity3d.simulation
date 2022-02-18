using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public class TreeVariantSettings : ResponsiveSettings
    {
        public TreeVariantSettings() : base(ResponsiveSettingsType.Tree)
        {
        }

        #region Fields and Autoproperties

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the likelihood the the fungi will spawn when the tree is dead.")]
        [PropertyRange(0f, 1f), TreeProperty, PropertyOrder(50)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree deadLikelihood = TreeProperty.New(.5f);

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the likelihood the the fungi will spawn when the tree is alive.")]
        [PropertyRange(0f, 1f), TreeProperty, PropertyOrder(40)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree liveLikelihood = TreeProperty.New(.15f);

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the likelihood the the fungi will spawn when the tree is rotted.")]
        [PropertyRange(0f, 1f), TreeProperty, PropertyOrder(60)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree rottedLikelihood = TreeProperty.New(1f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("How much material is moved from around the cut area.")]
        [PropertyRange(0.005f, .1f)]
        [OnValueChanged(nameof(GeometrySettingsChanged))]
        [TreeProperty, LabelText("Dead Zone")]
        public floatTree trunkCutDeadZone = TreeProperty.New(0.025f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyRange(0.1f, 5f)]
        [TreeProperty, LabelText("Beaver Factor")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkCutDepth = TreeProperty.New(2.5f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("The height that the tree will be cut at.")]
        [PropertyRange(.5f, 10f)]
        [OnValueChanged(nameof(GeometrySettingsChanged))]
        [TreeProperty, LabelText("Cut Height")]
        public floatTree trunkCutHeight = TreeProperty.New(2.0f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("Adds random noise of the broken limb.")]
        [PropertyRange(0f, 4f)]
        [TreeProperty, LabelText("Cut Noise")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkCutRandomness = TreeProperty.New(2.0f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("Scales the random noise for the broken limb.")]
        [PropertyRange(0.1f, 10f)]
        [TreeProperty, LabelText("Cut Noise Scale")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkCutRandomnessScale = TreeProperty.New(0.75f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("Controls the shape of the broken limb.")]
        [PropertyRange(0f, 2f)]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        [TreeProperty, LabelText("Cut Roundness")]
        public floatTree trunkCutSphericalCap = TreeProperty.New(0.3f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("The thickness of the cut through the trunk.")]
        [PropertyRange(0.05f, 1f)]
        [OnValueChanged(nameof(GeometrySettingsChanged))]
        [TreeProperty, LabelText("Cut Thickness")]
        public floatTree trunkCutThickness = TreeProperty.New(0.1f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("Scales the random noise for the broken limb.")]
        [PropertyRange(1.0f, 3f)]
        [TreeProperty, LabelText("Ring Res.")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkRingResolution = TreeProperty.New(1.0f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("Scales the random noise for the broken limb.")]
        [PropertyRange(1.0f, 3f)]
        [TreeProperty, LabelText("Segment Res.")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree trunkSegmentResolution = TreeProperty.New(1.0f);

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the real world height that the fungus can appear at.")]
        [MinMaxSlider(0.1f, 15.0f, true), TreeProperty, PropertyOrder(12)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree deadHeightRange = TreeProperty.v2(0.25f, 6.0f);

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the real world height that the fungus can appear at.")]
        [MinMaxSlider(0.1f, 15.0f, true), TreeProperty, PropertyOrder(12)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree liveHeightRange = TreeProperty.v2(0.25f, 2f);

        [TabGroup("Fungus", Paddingless = true)]
        [PropertyTooltip("Adjusts the real world height that the fungus can appear at.")]
        [MinMaxSlider(0.1f, 15.0f, true), TreeProperty, PropertyOrder(12)]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree rottedHeightRange = TreeProperty.v2(0.25f, 10.0f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("The UV offset of the trunk cut texture.")]
        [TreeProperty, LabelText("Tex Offset")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public Vector2Tree trunkTextureUVOffset = TreeProperty.v2(0.0f, 0.0f);

        [TabGroup("Felled & Stump", Paddingless = true)]
        [PropertyTooltip("The scale of the limb break texture.")]
        [TreeProperty, LabelText("Tex Scale")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public Vector2Tree trunkTextureUVScale = TreeProperty.v2(1.0f, 1.0f);

        #endregion

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is TreeVariantSettings cast)
            {
                cast.trunkCutHeight = trunkCutHeight.Clone();
                cast.trunkCutThickness = trunkCutThickness.Clone();
                cast.trunkCutDeadZone = trunkCutDeadZone.Clone();
                cast.trunkCutSphericalCap = trunkCutSphericalCap.Clone();
                cast.trunkCutDepth = trunkCutDepth.Clone();
                cast.trunkCutRandomness = trunkCutRandomness.Clone();
                cast.trunkCutRandomnessScale = trunkCutRandomnessScale.Clone();
                cast.trunkRingResolution = trunkRingResolution.Clone();
                cast.trunkSegmentResolution = trunkSegmentResolution.Clone();
                cast.trunkTextureUVOffset = trunkTextureUVOffset.Clone();
                cast.trunkTextureUVScale = trunkTextureUVScale.Clone();
                cast.liveLikelihood = liveLikelihood.Clone();
                cast.deadLikelihood = deadLikelihood.Clone();
                cast.rottedLikelihood = rottedLikelihood.Clone();
                cast.liveHeightRange = liveHeightRange.Clone();
                cast.deadHeightRange = deadHeightRange.Clone();
                cast.rottedHeightRange = rottedHeightRange.Clone();
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

                var settings = tree.settings.variants;

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
