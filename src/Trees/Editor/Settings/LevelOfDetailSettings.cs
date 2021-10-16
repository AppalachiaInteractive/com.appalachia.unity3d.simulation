using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    public class LevelOfDetailSettings : ResponsiveSettings
    {
        //[HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("The screen relative height to use for the transition.")]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("$title/0")]
        [InfoBox("Transition Height", InfoMessageType.None), HideLabel]
        public float screenRelativeTransitionHeight = .9f;
        
        //[HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Width of the cross-fade transition zone (proportion to the current LODs whole length).")] 
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("$title/0")]
        [InfoBox("Transition Width", InfoMessageType.None), HideLabel]
        public float fadeTransitionWidth = .25f;

        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip(
            "Below and above this range, LOD quality will become 0 to prevent complex geometry where it can not be seen."
        )]
        [MinMaxSlider(-3f, 50f)]
        [InfoBox("Height Quality Dropoff", InfoMessageType.None), HideLabel]
        [HorizontalGroup("$title/1")]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public Vector2 heightQualityDropoff = new Vector2(-.1f, 20f);
        
        [PropertyTooltip("Adjusts the quality of trunks relative to the tree's LOD Quality, so that it is of either higher or lower quality than the rest of the tree.")] 
        [PropertyRange(0f, 2f)]
        [HorizontalGroup("$title/A")]
        [InfoBox("Trunks", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float trunkGeometryQuality = 1f;

        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Adjusts the quality of roots relative to the tree's LOD Quality, so that it is of either higher or lower quality than the rest of the tree.")] 
        [PropertyRange(0f, 2f)]
        [FoldoutGroup("$title", Expanded = false)]
        [HorizontalGroup("$title/A")]
        [InfoBox("Roots", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float rootsGeometryQuality = 1f;

        [PropertyTooltip("Adjusts the quality of branches relative to the tree's LOD Quality, so that it is of either higher or lower quality than the rest of the tree.")] 
        [PropertyRange(0f, 2f)]
        [HorizontalGroup("$title/B")]
        [InfoBox("Branches", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float branchesGeometryQuality = 1f;

        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Adjusts the quality of leaves relative to the tree's LOD Quality, so that it is of either higher or lower quality than the rest of the tree.")] 
        [PropertyRange(0f, 2f)]
        [HorizontalGroup("$title/B")]
        [InfoBox("Leaves", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float leafGeometryQuality = 1f;

        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Controls the fullness of leaves.  At 0.5, half of the leaf planes will appear.")] 
        [PropertyRange(0f, 1f)]
        [FoldoutGroup("$title")]
        [InfoBox("Leaf Fullness", InfoMessageType.None), HideLabel]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float leafFullness = 1f;
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Generates double sided leaf planes.")] 
        [HorizontalGroup("$title/C")]
        [LabelWidth(80), LabelText("Double Sided")]
        [PropertySpace, ToggleLeft]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool doubleSidedLeafGeometry;
        
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to render fruit at this level of detail.")]  
        [HorizontalGroup("$title/C")]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool showFruit = true;
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to render knots at this level of detail.")] 
        [HorizontalGroup("$title/D" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool showKnots = true;    
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to render fungus at this level of detail.")] 
        [HorizontalGroup("$title/D" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool showFungus = true;


        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to add additional spline samples at child offsets (to improve vertex data quality.")] 
        [HorizontalGroup("$title/E" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool resampleSplineAtChildren = true;


        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to add additional spline samples on the trunk only")] 
        [HorizontalGroup("$title/E" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [EnableIf(nameof(resampleSplineAtChildren))]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool resampleOnTrunkOnly = true;


        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to add additional spline samples when teh child is a leaf.")] 
        [HorizontalGroup("$title/F" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [EnableIf(nameof(resampleSplineAtChildren))]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool resampleLeaves;
        

        [HideIf(nameof(hideTreeSettings))]
        [PropertyTooltip("Whether to add additional spline samples when teh child is fungi, knots, or fruits.")] 
        [HorizontalGroup("$title/F" )]
        [LabelWidth(65)]
        [PropertySpace, ToggleLeft]
        [EnableIf(nameof(resampleSplineAtChildren))]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public bool resampleAddOns = true;
        
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyRange(0f, 1f)]
        [HorizontalGroup("$title/G" )]
        [InfoBox("Resample Height", InfoMessageType.None), HideLabel]
        [EnableIf(nameof(resampleSplineAtChildren))]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float childResampleHeightLimit = .5f;
        
        [HideIf(nameof(hideTreeSettings))]
        [PropertyRange(0.0001f, .05f)]
        [HorizontalGroup("$title/G" )]
        [InfoBox("Resample Threshold", InfoMessageType.None), HideLabel]
        [EnableIf(nameof(resampleSplineAtChildren))]
        [OnValueChanged(nameof(LevelOfDetailSettingsChanged))]
        public float childResampleThreshold = .001f;

        private string title => $"LOD{level} Geometry Quality";


        public int level { get; internal set; }

        public float GetGeometryQuality(TreeComponentType type)
        {
            switch (type)
            {
                case TreeComponentType.Root:
                    return rootsGeometryQuality;
                
                case TreeComponentType.Trunk:
                    return trunkGeometryQuality;
                
                case TreeComponentType.Branch:
                    return branchesGeometryQuality;
                
                case TreeComponentType.Leaf:
                    return leafGeometryQuality;
                
                case TreeComponentType.Fruit:
                    return 1f;
                
                case TreeComponentType.Knot:
                    return 1f;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        private bool hideTreeSettings => settingsType != ResponsiveSettingsType.Tree;
        
        public LevelOfDetailSettings(int level, ResponsiveSettingsType settingsType) : base(settingsType)
        {
            this.level = level;
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is LevelOfDetailSettings cast)
            {
                cast.level = level;
                cast.leafFullness = leafFullness;
                cast.resampleLeaves = resampleLeaves;
                cast.showFruit = showFruit;
                cast.showFungus = showFungus;
                cast.showKnots = showKnots;
                cast.resampleAddOns = resampleAddOns;
                cast.doubleSidedLeafGeometry = doubleSidedLeafGeometry;
                cast.resampleOnTrunkOnly = resampleOnTrunkOnly;
                cast.resampleSplineAtChildren = resampleSplineAtChildren;
                cast.branchesGeometryQuality = branchesGeometryQuality;
                cast.childResampleThreshold = childResampleThreshold;
                cast.fadeTransitionWidth = fadeTransitionWidth;
                cast.heightQualityDropoff = heightQualityDropoff;
                cast.leafGeometryQuality = leafGeometryQuality;
                cast.rootsGeometryQuality = rootsGeometryQuality;
                cast.trunkGeometryQuality = trunkGeometryQuality;
                cast.childResampleHeightLimit = childResampleHeightLimit;
                cast.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
            }
        }
    }
}