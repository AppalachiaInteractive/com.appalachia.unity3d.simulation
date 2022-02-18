using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class BranchSettings : AgeOverrideResponsiveSettings, ICloneable<BranchSettings>
    {
        #region Constants and Static Readonly

        private static readonly string[] SearchStrings = { "bark", "wood", "branch", "trunk" };

        #endregion

        public BranchSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            frond = new FrondSettings(settingsType);
        }

        #region Fields and Autoproperties

        [PropertyTooltip(
            "Adjusts the radius of the spline. " +
            "Use the curve to fine-tune the radius along the length of the spline."
        )]
        [HorizontalGroup("CurveA", .3f)]
        [TreeCurve]
        [HideLabel, TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public AnimationCurveTree radiusCurve = TreeProperty.Curve(
            new Keyframe(0.0f, 1.0f,  -1.0f, -1.0f),
            new Keyframe(1.0f, 0.05f, -1.0f, -1.0f)
        );

        [PropertyTooltip("Determines whether the branch moves in the wind.")]
        [TreeProperty, LabelText("Disable Wind")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public bool disableWind;

        [ShowIf(nameof(showBranch))]
        [PropertyTooltip(
            "Determines whether the branch comes off of the parent, or from the middle as a fork."
        )]
        [TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public bool forked;

        [FormerlySerializedAs("lengthRelativeToParent")]
        [ShowIf(nameof(relativeToParentAllowed))]
        [PropertyTooltip("Determines whether the length & radius of a branch are relative to the parent.")]
        [TreeProperty, LabelText("Parent Relative")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public bool relativeToParent;

        [FormerlySerializedAs("lengthRelativeToParentAllowed")]
        [HideInInspector]
        public bool relativeToParentAllowed = true;

        [PropertyTooltip("Determines whether the radius of a branch is affected by its length.")]
        [TreeProperty, LabelText("Shorter Thinner")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public boolTree shorterBranchesAreThinner = TreeProperty.New(true);

        [PropertyTooltip("Type of geometry for this group.")]
        [TreePropertySimple]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public BranchGeometryMode geometryMode = BranchGeometryMode.Branch;

        [PropertyTooltip("Adjusts the quality of this hierarchy relative to tree.")]
        [PropertyRange(0f, 2f), TreeProperty, LabelText("LOD Quality")]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree lodQualityMultiplier = TreeProperty.New(1f);

        [ShowIf(nameof(notRelativeToparent))]
        [PropertyTooltip("Adjusts the radius of the spline.")]
        [HorizontalGroup("CurveA")]
        [PropertyRange(0.05f, 8f), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree radius = TreeProperty.New(.5f);

        [ShowIf(nameof(relativeToParent))]
        [PropertyTooltip("Adjusts the radius of the spline, relative to the parent.")]
        [HorizontalGroup("CurveA")]
        [PropertyRange(0.1f, 1f), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree relativeRadius = TreeProperty.New(.8f);

        [ShowIf(nameof(showSplineStepSize))]
        [PropertyTooltip("Adjusts the size between steps along the spline's length.")]
        [PropertyRange(0.20f, 5f), TreeProperty, LabelText("Spline Step")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree splineStepSize = TreeProperty.New(1f);

        [HideIf(nameof(disableWind))]
        [PropertyTooltip("Modifies the wind strength for this branch.")]
        [PropertyRange(0.01f, 2f), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatTree windStrength = TreeProperty.New(1.0f);

        [TreeHeader, ShowIfGroup(nameof(showFrond)), PropertyOrder(51)]
        public FrondSettings frond;

        [PropertyTooltip("Adjusts the number of colliders generated for the branch.")]
        [PropertyRange(1, 5), TreeProperty, LabelText("Colliders")]
        [OnValueChanged(nameof(CollisionSettingsChanged), true)]
        public intTree colliderMultiplier = TreeProperty.New(1);

        [PropertyTooltip("The primary bark material.")]
        [TreePropertySimple]
        [ShowIf(nameof(showBranch))]
        [HideIf(nameof(hideLog))]
        [OnValueChanged(nameof(MaterialGenerationChanged), true)]
        [HorizontalGroup("Z")]
        [DelayedProperty]
        [ValueDropdown(nameof(GetBarkMaterials), AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        public Material barkMaterial;

        [PropertyTooltip("Type of geometry for this group.")]
        [TreePropertySimple]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public SplineStepMode splineStepMode = SplineStepMode.Adaptive;

        [PropertyTooltip("The primary bark material scale.")]
        [TreePropertySimple]
        [ShowIf(nameof(showBranch))]
        [HideIf(nameof(hideLog))]
        [OnValueChanged(nameof(UVSettingsChanged), true)]
        [HorizontalGroup("Z", .4f), InlineProperty, HideLabel]
        public UVScaleTree barkScale = TreeProperty.uv(1, 1);

        [ShowIf(nameof(notRelativeToparent))]
        [PropertyTooltip("Adjusts the length of the spline.")]
        [MinMaxSlider(1f, 100f, true), TreeProperty, LabelText(@"$lengthLabel")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree length = TreeProperty.v2(10.0f, 15.0f);

        [ShowIf(nameof(relativeToParent))]
        [PropertyTooltip("Adjusts the length of the spline, relative to the parent.")]
        [MinMaxSlider(0.01f, 6f, true), TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree relativeLength = TreeProperty.v2(0.4f, 0.55f);

        [ShowIf(nameof(showSplineStepSizeV2))]
        [PropertyTooltip("Adjusts the size between steps along the spline's length.")]
        [MinMaxSlider(0.20f, 5f), TreeProperty, LabelText("Spline Step")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree splineStepSizeV2 = TreeProperty.v2(1f, 3f);

        #endregion

        private bool hideLog => settingsType == ResponsiveSettingsType.Log;

        private bool notRelativeToparent => !relativeToParent;

        private bool showBranch =>
            (geometryMode == BranchGeometryMode.Branch) || (geometryMode == BranchGeometryMode.BranchFrond);

        private bool showFrond =>
            ((geometryMode == BranchGeometryMode.Frond) ||
             (geometryMode == BranchGeometryMode.BranchFrond)) &&
            !hideLog;

        private bool showSplineStepSize => splineStepMode == SplineStepMode.Fixed;
        private bool showSplineStepSizeV2 => splineStepMode == SplineStepMode.Gradient;

        private string lengthLabel => settingsType == ResponsiveSettingsType.Log ? "Segments" : "Length";

        /// <inheritdoc />
        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is BranchSettings cast)
            {
                cast.geometryMode = geometryMode;
                cast.barkMaterial = barkMaterial;
                cast.barkScale = barkScale.Clone();
                cast.frond = frond.Clone();
                cast.relativeToParentAllowed = relativeToParentAllowed;
                cast.relativeToParent = relativeToParent;
                cast.length = length.Clone();
                cast.relativeLength = relativeLength.Clone();
                cast.radius = radius.Clone();
                cast.relativeRadius = relativeRadius.Clone();
                cast.radiusCurve = radiusCurve.Clone();
                cast.shorterBranchesAreThinner = shorterBranchesAreThinner.Clone();
                cast.lodQualityMultiplier = lodQualityMultiplier.Clone();
                cast.forked = forked;
                cast.windStrength = windStrength.Clone();
                cast.colliderMultiplier = colliderMultiplier.Clone();
                cast.splineStepMode = splineStepMode;
                cast.splineStepSize = splineStepSize.Clone();
            }
        }

        private List<Material> GetBarkMaterials()
        {
            var assets = AssetDatabaseManager.FindAssets("t: Material");
            var results = new List<Material>();

            foreach (var assetGuid in assets)
            {
                var material =
                    AssetDatabaseManager.LoadAssetAtPath<Material>(
                        AssetDatabaseManager.GUIDToAssetPath(assetGuid)
                    );

                foreach (var searchString in SearchStrings)
                {
                    var s = searchString.ToLowerInvariant();

                    if (material.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }

                    if (material.shader.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }
                }
            }

            return results;
        }

        #region ICloneable<BranchSettings> Members

        public BranchSettings Clone()
        {
            var clone = new BranchSettings(settingsType);
            clone.geometryMode = geometryMode;
            clone.barkMaterial = barkMaterial;
            clone.barkScale = barkScale.Clone();
            clone.frond = frond.Clone();
            clone.relativeToParentAllowed = relativeToParentAllowed;
            clone.relativeToParent = relativeToParent;
            clone.length = length.Clone();
            clone.relativeLength = relativeLength.Clone();
            clone.radius = radius.Clone();
            clone.relativeRadius = relativeRadius.Clone();
            clone.radiusCurve = radiusCurve.Clone();
            clone.shorterBranchesAreThinner = shorterBranchesAreThinner.Clone();
            clone.lodQualityMultiplier = lodQualityMultiplier.Clone();
            clone.forked = forked;
            clone.windStrength = windStrength.Clone();
            clone.colliderMultiplier = colliderMultiplier.Clone();
            clone.splineStepMode = splineStepMode;
            clone.splineStepSize = splineStepSize.Clone();

            return clone;
        }

        #endregion
    }
}
