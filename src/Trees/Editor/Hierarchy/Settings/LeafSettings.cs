using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Hierarchy.Options;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.UV;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy.Settings
{
    [Serializable]
    public class LeafSettings : AgeOverrideResponsiveSettings, ICloneable<LeafSettings>
    {
        private bool _hideLeafMaterial => (geometryMode == LeafGeometryMode.Mesh);
        private bool _hideLeafPrefab => (geometryMode != LeafGeometryMode.Mesh);
        
        
        [PropertyTooltip("Material used for the leaves")]
        [TreePropertySimple]
        [HideIf(nameof(_hideLeafMaterial))]
        [OnValueChanged(nameof(MaterialGenerationChanged), true)]
        [DelayedProperty]
        [ValueDropdown(nameof(GetLeafMaterials), AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        public Material leafMaterial;

        private static readonly string[] SearchStrings = new[] {"leaf", "twig", "branch", "leave"};
        private static readonly string[] ExcludedSearchStrings_Tree = new[] {"_atlas", "_tiled"};
        private static readonly string[] ExcludedSearchStrings_Branch = new[] {"_atlas", "_tiled", "_snapshot"};
        
        private List<Material> GetLeafMaterials()
        {
            var assets = AssetDatabaseManager.FindAssets("t: Material");
            var results = new List<Material>();

            foreach (var assetGuid in assets)
            {
                var material = AssetDatabaseManager.LoadAssetAtPath<Material>(AssetDatabaseManager.GUIDToAssetPath(assetGuid));

                var exclusions = settingsType == ResponsiveSettingsType.Tree
                    ? ExcludedSearchStrings_Tree
                    : ExcludedSearchStrings_Branch;

                var skip = false;
                foreach (var exclusion in exclusions)
                {
                    if (material.name.ToLowerInvariant().Contains(exclusion.ToLowerInvariant()))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }
                
                foreach (var searchString in SearchStrings)
                {
                    
                    var s = searchString.ToLowerInvariant();
                    
                    if (material.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }
                    else if (material.shader.name.ToLowerInvariant().Contains(s))
                    {
                        results.Add(material);
                        break;
                    }
                } 
            }

            return results;
        }
        
        /*
        [HideInInspector]
        public List<LeafUVRect> uvRects = new List<LeafUVRect>();*/
        
        [Button]
        public void EditUVRects()
        {
            if (settingsType == ResponsiveSettingsType.Branch)
            {
                BranchUVEditor.OpenInstance();
            }
            else
            {
                TreeUVEditor.OpenInstance();                
            }
        }

        [InlineProperty, HideLabel, ShowIf(nameof(_showPrefab))]
        public PrefabSetup prefab;

        private bool _showPrefab => (geometryMode == LeafGeometryMode.Mesh);
        
        [PropertyTooltip("The type of geometry created.")]
        [TreePropertySimple]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public LeafGeometryMode geometryMode = LeafGeometryMode.DiamondPyramid;
        
        [PropertyTooltip("The number of crosses in the spoke.")]
        [PropertyRange(1, 6), TreeProperty]
        [ShowIf(nameof(_showSpokeCount))] 
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public intTree spokeCount = TreeProperty.New(3);
        
        [PropertyTooltip("The number of crosses will decrease every X meters.")]
        [PropertyRange(0f, 1.0f), TreeProperty]
        [ShowIf(nameof(_showSpokeCount))] 
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public float spokeDropPerMeter = TreeProperty.New(0.05f);

        [PropertyTooltip(
            "Adjusts the size of the leaves, use the range to adjust the minimum and the maximum size.")]
        [MinMaxSlider(.1f, 10.0f, true),TreeProperty]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public Vector2Tree size = TreeProperty.v2(2f, 3f);

        private bool _showRatio =>
            (geometryMode == LeafGeometryMode.Plane) ||
            (geometryMode == LeafGeometryMode.Cross) ||
            (geometryMode == LeafGeometryMode.TriCross) ||
            (geometryMode == LeafGeometryMode.BentPlane) ||
            (geometryMode == LeafGeometryMode.BentCross) ||
            (geometryMode == LeafGeometryMode.BentTriCross) ||
            (geometryMode == LeafGeometryMode.BoxPyramid) ||
            (geometryMode == LeafGeometryMode.DiamondPyramid) ||
            (geometryMode == LeafGeometryMode.DiamondWidthCut) ||
            (geometryMode == LeafGeometryMode.DiamondWidthCut) ||
            (geometryMode == LeafGeometryMode.Spoke) ||
            (geometryMode == LeafGeometryMode.BentSpoke);
        
        private bool _showSpokeCount =>
            (geometryMode == LeafGeometryMode.Spoke) ||
            (geometryMode == LeafGeometryMode.BentSpoke);
        private bool _showOffset =>
            (geometryMode == LeafGeometryMode.Plane) ||
            (geometryMode == LeafGeometryMode.Cross) ||
            (geometryMode == LeafGeometryMode.TriCross) ||
            (geometryMode == LeafGeometryMode.BentPlane) ||
            (geometryMode == LeafGeometryMode.BentCross) ||
            (geometryMode == LeafGeometryMode.BentTriCross) ||
            (geometryMode == LeafGeometryMode.Spoke) ||
            (geometryMode == LeafGeometryMode.BentSpoke);
        
        private bool _showBendFactor =>
            (geometryMode == LeafGeometryMode.BentPlane) ||
            (geometryMode == LeafGeometryMode.BentCross) ||
            (geometryMode == LeafGeometryMode.BentTriCross) ||
            (geometryMode == LeafGeometryMode.BoxPyramid) ||
            (geometryMode == LeafGeometryMode.DiamondPyramid) ||
            (geometryMode == LeafGeometryMode.DiamondLengthCut) ||
            (geometryMode == LeafGeometryMode.DiamondWidthCut) ||
            (geometryMode == LeafGeometryMode.BentSpoke);
        
        [PropertyTooltip("what is the horizontal component of the horizontal-to-vertical ratio?")]
        [PropertyRange(0f, 2f),TreeProperty]
        [ShowIf(nameof(_showRatio))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree xRatio = TreeProperty.New(01.0f);
        
        [PropertyTooltip("How much is the plane offset in the x (left/right) direction?")]
        [PropertyRange(-1f, 1f),TreeProperty]
        [ShowIf(nameof(_showOffset))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree xOffset = TreeProperty.New(0.0f);
        
        [PropertyTooltip("How much is the plane offset in the z (forward/backward) direction?")]
        [PropertyRange(-1f, 1f),TreeProperty]
        [ShowIf(nameof(_showOffset))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree zOffset = TreeProperty.New(0.0f);
        
        [PropertyTooltip("Should planes be adjusted automatically?")]
        [TreeProperty]
        [ShowIf(nameof(_showOffset))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public boolTree correctOffset = TreeProperty.New(true);

        [PropertyTooltip("Defines whether or not leaves are welded to their parents.")]
        [TreeProperty]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public boolTree disableWelding = TreeProperty.New(false);
        
        [PropertyTooltip("How bent is the plane?")]
        [PropertyRange(-1f, 1f),TreeProperty]
        [ShowIf(nameof(_showBendFactor))]
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public floatTree  bendFactor = TreeProperty.New(.5f);
        
        [PropertyTooltip("Flips the direction that the leaves are facing.")]
        [TreeProperty]
        //[ShowIf(nameof(_showBendFactor))] // intentional
        [OnValueChanged(nameof(GeometrySettingsChanged), true)]
        public boolTree  flipLeafNormals = TreeProperty.New(false);

        [PropertyTooltip("Adjusts whether the leaves are aligned horizontally.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve(MinValue = -1.0f), TreeProperty, LabelText("Horizontal")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree horizontalAlign = TreeProperty.fCurve(0.0f, 1.0f, 0.0f);

        /*[PropertyTooltip("Adjusts whether the leaves are aligned vertically.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve(MinValue = -1.0f), TreeProperty, LabelText("Vertical")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree verticalAlign = TreeProperty.fCurve(0.0f, 0.0f, 1.0f);*/

        [PropertyTooltip("Adjusts how the leaf planes are rotated.")]
        [PropertyRange(0.0f, 1.0f)]
        [TreeCurve, TreeProperty, LabelText("Rotation")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree rotationalAlign = TreeProperty.fCurve(0.0f, 0.0f, 1.0f);
          
        /*
        [PropertyTooltip("Adjusts whether the leaves are aligned perpendicular to the parent branch.")]
        [PropertyRange(0f, 1f)]
        [TreeCurve, TreeProperty, LabelText("Perpendicular")]
        [OnValueChanged(nameof(DistributionSettingsChanged), true)]
        public floatCurveTree perpendicularAlign = TreeProperty.fCurve(1.0f, 0.0f, 1.0f);
        */
        
        public LeafSettings Clone()
        {
            var clone = new LeafSettings(settingsType);
            clone.leafMaterial = leafMaterial;
            clone.prefab = prefab.Clone();
            clone.geometryMode = geometryMode;
            clone.size = size.Clone();
            clone.xOffset = xOffset.Clone();
            clone.zOffset = zOffset.Clone();
            clone.correctOffset = correctOffset.Clone();
            clone.disableWelding = disableWelding.Clone();
            clone.bendFactor = bendFactor.Clone();
            clone.flipLeafNormals = flipLeafNormals.Clone();
            clone.horizontalAlign = horizontalAlign.Clone();
            //clone.verticalAlign = verticalAlign.Clone();
            clone.rotationalAlign = rotationalAlign.Clone();

            return clone;
        }

        public LeafSettings(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            prefab = new PrefabSetup(settingsType);
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is LeafSettings cast)
            {
                cast.leafMaterial = leafMaterial;
                cast.prefab = prefab.Clone();
                cast.geometryMode = geometryMode;
                cast.size = size.Clone();
                cast.xOffset = xOffset.Clone();
                cast.zOffset = zOffset.Clone();
                cast.correctOffset = correctOffset.Clone();
                cast.disableWelding = disableWelding.Clone();
                cast.bendFactor = bendFactor.Clone();
                cast.flipLeafNormals = flipLeafNormals.Clone();
                cast.horizontalAlign = horizontalAlign.Clone();
                //cast.verticalAlign = verticalAlign.Clone();
                cast.rotationalAlign = rotationalAlign.Clone();
            }
        }
    }
}