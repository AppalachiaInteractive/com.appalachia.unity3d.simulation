using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Curves;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public abstract class BarkHierarchyData : HierarchyData
    {
        protected BarkHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) :
            base(hierarchyID, parentHierarchyID, type)
        {
            limb = new LimbSettings(type);
            curvature = new CurvatureSettings(type);
            geometry = new BranchSettings(type);

            /*if (this.type == TreeComponentType.Trunk)
            {
                curvature.showThigmotropism = false;
            }
            else
            {
                curvature.showThigmotropism = true;
            }*/
        }

        protected BarkHierarchyData(
            int hierarchyID,
            int parentHierarchyID,
            TreeEditor.TreeGroupBranch group,
            Material barkMaterial,
            Material breakMaterial,
            Material frondMaterial) : base(hierarchyID, parentHierarchyID, group)
        {
            curvature = new CurvatureSettings(ResponsiveSettingsType.Tree);
            curvature.crookedness.SetValue(new floatCurve(group.crinklyness, group.crinkCurve));

            curvature.noise.SetValue(new floatCurve(group.noise, group.noiseCurve));
            curvature.noiseScaleU.SetValue(group.noiseScaleU);
            curvature.noiseScaleV.SetValue(group.noiseScaleV);
            curvature.phototropism.SetValue(new floatCurve(group.seekBlend, group.seekCurve));

            limb = new LimbSettings(ResponsiveSettingsType.Tree);
            limb.breakingChance.SetValue(group.breakingChance);
            limb.breakingSpot.SetValue(group.breakingSpot);
            limb.capSmoothing.SetValue(group.capSmoothing);

            //limb.breakMaterial = breakMaterial;

            geometry = new BranchSettings(ResponsiveSettingsType.Tree);

            geometry.barkMaterial = barkMaterial;
            geometry.length.SetValue(group.height);
            geometry.radius.SetValue(group.radius);
            geometry.radiusCurve.SetValue(group.radiusCurve);
            geometry.shorterBranchesAreThinner.SetValue(group.radiusMode);
            geometry.lodQualityMultiplier.SetValue(group.lodQualityMultiplier);
            geometry.geometryMode = group.geometryMode.ToInternal();

            geometry.frond = new FrondSettings(ResponsiveSettingsType.Tree);

            geometry.frond.frondMaterial = frondMaterial;
            geometry.frond.frondCount.SetValue(group.frondCount);
            geometry.frond.frondWidth.SetValue(new floatCurve(group.frondWidth, group.frondCurve));
            geometry.frond.frondRange.SetValue(group.frondRange);
            geometry.frond.frondRotation.SetValue(group.frondRotation);
            geometry.frond.frondCrease.SetValue(group.frondCrease);

            /*
            if (this.type == TreeComponentType.Trunk)
            {
                curvature.showThigmotropism = false;
            }
            else
            {
                curvature.showThigmotropism = true;
            }
            */
        }

        #region Fields and Autoproperties

        //[ShowIfGroup("SH", Condition = nameof(showShape), Animate = false)]
        [TreeHeader, PropertyOrder(0)]

        //[FoldoutGroup("SH/Shape", false)]
        [ShowIf(nameof(showShape))]
        [TabGroup("Shape", Paddingless = true)]
        public BranchSettings geometry;

        //[ShowIfGroup("CV", Condition = nameof(showCurvature), Animate = false)]
        [TreeHeader, PropertyOrder(60)]

        //[FoldoutGroup("CV/Curvature", false)]
        [ShowIf(nameof(showCurvature))]
        [TabGroup("Curvature", Paddingless = true)]
        public CurvatureSettings curvature;

        [FormerlySerializedAs("breakage")]

        //[ShowIfGroup("LE", Condition = nameof(showLimbEnds), Animate = false)]
        [TreeHeader, PropertyOrder(200)]

        //[FoldoutGroup("LE/Limb Ends", false)
        [ShowIf(nameof(showLimbEnds))]
        [TabGroup("Limb Ends", Paddingless = true)]
        public LimbSettings limb;

        #endregion

        /// <inheritdoc />
        public override bool IsFrond =>
            (geometry.geometryMode == BranchGeometryMode.Frond) ||
            (geometry.geometryMode == BranchGeometryMode.BranchFrond);

        private bool showCurvature => true;

        private bool showLimbEnds => geometry.geometryMode != BranchGeometryMode.Frond;

        private bool showShape => true;

        /// <inheritdoc />
        public override string GetSortKey()
        {
            if (geometry.barkMaterial != null)
            {
                return geometry.barkMaterial.name;
            }

            return ZString.Format("{0:0000}", hierarchyID);
        }

        /// <inheritdoc />
        protected override Object[] GetExternalObjects()
        {
            var mats = new List<Object>();

            if (geometry.barkMaterial != null)
            {
                mats.Add(geometry.barkMaterial);
            }

            /*if (limb.breakMaterial != null)
            {
                mats.Add(limb.breakMaterial);
            }*/

            if (geometry.frond.frondMaterial != null)
            {
                mats.Add(geometry.frond.frondMaterial);
            }

            return mats.ToArray();
        }
    }
}
