#region

using Appalachia.Base.Scriptables;
using Appalachia.Editing.Attributes;
using Appalachia.Simulation.Core.Metadata.Density;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core.Metadata.Wind
{
    public class GlobalWindParameters : SelfSavingScriptableObject<GlobalWindParameters>
    {
        [FoldoutGroup("System")]
        [SmartLabel]
        public bool realtimeUpdate = true;

        [BoxGroup("System/Physics")]
        [SmartLabel]
        [SuffixLabel("m/s")]
        [PropertyRange(0.1f, 50f)]
        public float maximumWindSpeed = 25f;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public bool showArrow;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public bool showArrowComponents;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public Mesh arrowMesh;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public Material arrowMaterial;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public Vector3 arrowOffset = Vector3.up * 10.0f;

        [BoxGroup("System/Setup")]
        [SmartLabel]
        public Vector3 arrowScale = Vector3.one * 2.0f;

        [TitleGroup("Heading")]
        [FoldoutGroup("Heading/Updates")]
        [MinMaxSlider(15f, 1200f, true)]
        [SmartLabel]
        public Vector2 headingChangeInterval = new(60f, 600f);

        [FoldoutGroup("Heading/Updates")]
        [MinMaxSlider(-180f, 180f, true)]
        [SmartLabel]
        public Vector2 headingUpdateRange = new(-60f, 60f);

        [FoldoutGroup("Heading/Updates")]
        [PropertyRange(0f, 1f)]
        [SmartLabel]
        public float headingUpdateSpeed = 0.05f;

        [TitleGroup("Motion")]
        [FoldoutGroup("Motion/Base")]
        [PropertyRange(0f, 1f)]
        [SmartLabel]
        public float baseAmplitude = 0.05f;

        [FoldoutGroup("Motion/Base")]
        [PropertyRange(0f, 1.0f)]
        [SmartLabel]
        public float baseToGustRatio = 0.15f;

        [FoldoutGroup("Motion/Gust")]
        [SmartLabel]
        public Texture2D gustTexture;

        [FoldoutGroup("Motion/Gust")]
        [PropertyRange(0.001f, 2f)]
        [SmartLabel]
        public float gustContrast = 1.0f;

        [FoldoutGroup("Motion/Gust")]
        [PropertyRange(.01f, 1.0f)]
        [SmartLabel]
        public float gustAmplitudeSharpness = 0.25f;

        [FoldoutGroup("Motion/Gust")]
        [MinMaxSlider(.0001f, 0.05f, true)]
        [SuffixLabel("/s")]
        [SmartLabel]
        public Vector2 gustStep = new(.001f, .01f);

        [FoldoutGroup("Motion/Audio")]
        [PropertyRange(.01f, 1.0f)]
        [SmartLabel]
        public float audioSharpness = 0.25f;

        [FoldoutGroup("Motion/Audio")]
        [MinMaxSlider(.0001f, 0.05f, true)]
        [SuffixLabel("/s")]
        [SmartLabel]
        public Vector2 audioStep = new(.001f, .01f);

        [InlineProperty]
        [HideLabel]
        public WindParameterCategory trunks;

        [InlineProperty]
        [HideLabel]
        public WindParameterCategory branches;

        [InlineProperty]
        [HideLabel]
        public WindParameterCategory leaves;

        [InlineProperty]
        [HideLabel]
        public WindParameterCategory plants;

        [InlineProperty]
        [HideLabel]
        public WindParameterCategory grass;

        [BoxGroup("System/Physics")]
        [ShowInInspector]
        [SmartLabel]
        public DensityMetadata airDensity => DensityMetadataCollection.instance.air;

        private void OnEnable()
        {
            if (maximumWindSpeed == 0f)
            {
                maximumWindSpeed = 25f;
            }
        }

        /*
        [Button]
        public void InitializeNewGroups()
        {
            trunks = new WindParameterCategory(WindParameterCategory.WindParameterCategoryType.Trunks);
            
            branches = new WindParameterCategory(WindParameterCategory.WindParameterCategoryType.Branches);
            
            leaves = new WindParameterCategory(WindParameterCategory.WindParameterCategoryType.Leaves);
            
            plants = new WindParameterCategory(WindParameterCategory.WindParameterCategoryType.Plants);
            
            grass = new WindParameterCategory(WindParameterCategory.WindParameterCategoryType.Grass);

            trunks.groups.Add(new WindParameterGroup(trunks.category, WindParameterGroup.WindParameterGroupType.Base, false));
            trunks.groups.Add(new WindParameterGroup(trunks.category, WindParameterGroup.WindParameterGroupType.Gust, false));

            branches.groups.Add(new WindParameterGroup(branches.category, WindParameterGroup.WindParameterGroupType.Base, true));
            branches.groups.Add(new WindParameterGroup(branches.category, WindParameterGroup.WindParameterGroupType.Gust, true));

            leaves.groups.Add(new WindParameterGroup(leaves.category, WindParameterGroup.WindParameterGroupType.Base, false));
            leaves.groups.Add(new WindParameterGroup(leaves.category, WindParameterGroup.WindParameterGroupType.Gust, false));
            leaves.groups.Add(new WindParameterGroup(leaves.category, WindParameterGroup.WindParameterGroupType.Mid, false));
            leaves.groups.Add(new WindParameterGroup(leaves.category, WindParameterGroup.WindParameterGroupType.Micro, false));

            plants.groups.Add(new WindParameterGroup(plants.category, WindParameterGroup.WindParameterGroupType.Base, false));
            plants.groups.Add(new WindParameterGroup(plants.category, WindParameterGroup.WindParameterGroupType.Gust, false));
            plants.groups.Add(new WindParameterGroup(plants.category, WindParameterGroup.WindParameterGroupType.Mid, false));
            plants.groups.Add(new WindParameterGroup(plants.category, WindParameterGroup.WindParameterGroupType.Micro, false));

            grass.groups.Add(new WindParameterGroup(grass.category, WindParameterGroup.WindParameterGroupType.Base, false));
            grass.groups.Add(new WindParameterGroup(grass.category, WindParameterGroup.WindParameterGroupType.Gust, false));
            grass.groups.Add(new WindParameterGroup(grass.category, WindParameterGroup.WindParameterGroupType.Mid, false));
            grass.groups.Add(new WindParameterGroup(grass.category, WindParameterGroup.WindParameterGroupType.Micro, false));
            
              trunks.groups[0].strengthMax = strengthMax;
              trunks.groups[1].strengthMax = strengthMax;
            branches.groups[0].strengthMax = strengthMax;
            branches.groups[1].strengthMax = strengthMax;
              leaves.groups[0].strengthMax = strengthMax;
              leaves.groups[1].strengthMax = strengthMax;
              leaves.groups[2].strengthMax = strengthMax;
              leaves.groups[3].strengthMax = strengthMax;
              plants.groups[0].strengthMax = strengthMax;
              plants.groups[1].strengthMax = strengthMax;
              plants.groups[2].strengthMax = strengthMax;
              plants.groups[3].strengthMax = strengthMax;
               grass.groups[0].strengthMax = strengthMax;
               grass.groups[1].strengthMax = strengthMax;
               grass.groups[2].strengthMax = strengthMax;
               grass.groups[3].strengthMax = strengthMax;
               
              trunks.groups[0].strengthMin = strengthMin;
              trunks.groups[1].strengthMin = strengthMin;
            branches.groups[0].strengthMin = strengthMin;
            branches.groups[1].strengthMin = strengthMin;
              leaves.groups[0].strengthMin = strengthMin;
              leaves.groups[1].strengthMin = strengthMin;
              leaves.groups[2].strengthMin = strengthMin;
              leaves.groups[3].strengthMin = strengthMin;
              plants.groups[0].strengthMin = strengthMin;
              plants.groups[1].strengthMin = strengthMin;
              plants.groups[2].strengthMin = strengthMin;
              plants.groups[3].strengthMin = strengthMin;
               grass.groups[0].strengthMin = strengthMin;
               grass.groups[1].strengthMin = strengthMin;
               grass.groups[2].strengthMin = strengthMin;
               grass.groups[3].strengthMin = strengthMin;
               
              /*
              trunks.groups[0].cycleTime = baseTrunkCycleTime;
              trunks.groups[1].cycleTime = gustTrunkCycleTime;
            branches.groups[0].cycleTime = baseBranchCycleTime;
            branches.groups[1].cycleTime = gustBranchCycleTime;
              leaves.groups[0].cycleTime = baseLeafCycleTime;
              leaves.groups[1].cycleTime = gustLeafCycleTime;
              leaves.groups[2].cycleTime = gustLeafCycleTime;
              leaves.groups[3].cycleTime = gustLeafCycleTime;
              plants.groups[0].cycleTime = basePlantCycleTime;
              plants.groups[1].cycleTime = gustPlantCycleTime;
              plants.groups[2].cycleTime = gustPlantCycleTime;
              plants.groups[3].cycleTime = gustPlantCycleTime;
               grass.groups[0].cycleTime = baseGrassCycleTime;
               grass.groups[1].cycleTime = gustGrassCycleTime;
               grass.groups[2].cycleTime = gustGrassCycleTime;
               grass.groups[3].cycleTime = gustGrassMicroCycleTime;
               #1#
              
              trunks.groups[0].cycleTimeMin = trunkCycleTimeMin;
              trunks.groups[1].cycleTimeMin = trunkCycleTimeMin;
            branches.groups[0].cycleTimeMin = branchCycleTimeMin;
            branches.groups[1].cycleTimeMin = branchCycleTimeMin;
              leaves.groups[0].cycleTimeMin = leafCycleTimeMin;
              leaves.groups[1].cycleTimeMin = leafCycleTimeMin;
              leaves.groups[2].cycleTimeMin = leafCycleTimeMin;
              leaves.groups[3].cycleTimeMin = leafCycleTimeMin;
              plants.groups[0].cycleTimeMin = plantCycleTimeMin;
              plants.groups[1].cycleTimeMin = plantCycleTimeMin;
              plants.groups[2].cycleTimeMin = plantCycleTimeMin;
              plants.groups[3].cycleTimeMin = plantCycleTimeMin;
               grass.groups[0].cycleTimeMin = grassCycleTimeMin;
               grass.groups[1].cycleTimeMin = grassCycleTimeMin;
               grass.groups[2].cycleTimeMin = grassCycleTimeMin;
               grass.groups[3].cycleTimeMin = grassMicroCycleTimeMin;
              
              trunks.groups[0].cycleTimeMax = trunkCycleTimeMax;
              trunks.groups[1].cycleTimeMax = trunkCycleTimeMax;
            branches.groups[0].cycleTimeMax = branchCycleTimeMax;
            branches.groups[1].cycleTimeMax = branchCycleTimeMax;
              leaves.groups[0].cycleTimeMax = leafCycleTimeMax;
              leaves.groups[1].cycleTimeMax = leafCycleTimeMax;
              leaves.groups[2].cycleTimeMax = leafCycleTimeMax;
              leaves.groups[3].cycleTimeMax = leafCycleTimeMax;
              plants.groups[0].cycleTimeMax = plantCycleTimeMax;
              plants.groups[1].cycleTimeMax = plantCycleTimeMax;
              plants.groups[2].cycleTimeMax = plantCycleTimeMax;
              plants.groups[3].cycleTimeMax = plantCycleTimeMax;
               grass.groups[0].cycleTimeMax = grassCycleTimeMax;
               grass.groups[1].cycleTimeMax = grassCycleTimeMax;
               grass.groups[2].cycleTimeMax = grassCycleTimeMax;
               grass.groups[3].cycleTimeMax = grassMicroCycleTimeMax;
               
              /*trunks.groups[0].fieldSize = baseTrunkFieldSize;
              trunks.groups[1].fieldSize = gustTrunkFieldSize;
            branches.groups[0].fieldSize = baseBranchFieldSize;
            branches.groups[1].fieldSize = gustBranchFieldSize;
              leaves.groups[0].fieldSize = baseLeafFieldSize;
              leaves.groups[1].fieldSize = gustLeafFieldSize;
              leaves.groups[2].fieldSize = gustLeafFieldSize;
              leaves.groups[3].fieldSize = gustLeafFieldSize;
              plants.groups[0].fieldSize = basePlantFieldSize;
              plants.groups[1].fieldSize = gustPlantFieldSize;
              plants.groups[2].fieldSize = gustPlantFieldSize;
              plants.groups[3].fieldSize = gustPlantFieldSize;
               grass.groups[0].fieldSize = baseGrassFieldSize;
               grass.groups[1].fieldSize = gustGrassFieldSize;
               grass.groups[2].fieldSize = gustGrassFieldSize;
               grass.groups[3].fieldSize = gustGrassMicroFieldSize;#1#
              
              trunks.groups[0].fieldSizeMin = trunkFieldSizeMin;
              trunks.groups[1].fieldSizeMin = trunkFieldSizeMin;
            branches.groups[0].fieldSizeMin = branchFieldSizeMin;
            branches.groups[1].fieldSizeMin = branchFieldSizeMin;
              leaves.groups[0].fieldSizeMin = leafFieldSizeMin;
              leaves.groups[1].fieldSizeMin = leafFieldSizeMin;
              leaves.groups[2].fieldSizeMin = leafFieldSizeMin;
              leaves.groups[3].fieldSizeMin = leafFieldSizeMin;
              plants.groups[0].fieldSizeMin = plantFieldSizeMin;
              plants.groups[1].fieldSizeMin = plantFieldSizeMin;
              plants.groups[2].fieldSizeMin = plantFieldSizeMin;
              plants.groups[3].fieldSizeMin = plantFieldSizeMin;
               grass.groups[0].fieldSizeMin = grassFieldSizeMin;
               grass.groups[1].fieldSizeMin = grassFieldSizeMin;
               grass.groups[2].fieldSizeMin = grassFieldSizeMin;
               grass.groups[3].fieldSizeMin = grassMicroFieldSizeMin;
              
              trunks.groups[0].fieldSizeMax = trunkFieldSizeMax;
              trunks.groups[1].fieldSizeMax = trunkFieldSizeMax;
            branches.groups[0].fieldSizeMax = branchFieldSizeMax;
            branches.groups[1].fieldSizeMax = branchFieldSizeMax;
              leaves.groups[0].fieldSizeMax = leafFieldSizeMax;
              leaves.groups[1].fieldSizeMax = leafFieldSizeMax;
              leaves.groups[2].fieldSizeMax = leafFieldSizeMax;
              leaves.groups[3].fieldSizeMax = leafFieldSizeMax;
              plants.groups[0].fieldSizeMax = plantFieldSizeMax;
              plants.groups[1].fieldSizeMax = plantFieldSizeMax;
              plants.groups[2].fieldSizeMax = plantFieldSizeMax;
              plants.groups[3].fieldSizeMax = plantFieldSizeMax;
               grass.groups[0].fieldSizeMax = grassFieldSizeMax;
               grass.groups[1].fieldSizeMax = grassFieldSizeMax;
               grass.groups[2].fieldSizeMax = grassFieldSizeMax;
               grass.groups[3].fieldSizeMax = grassMicroFieldSizeMax;
               
              /*
              trunks.groups[0].variationStrength = baseTrunkVariationStrength;
              trunks.groups[1].variationStrength = gustTrunkVariationStrength;#1#
            branches.groups[0].variationStrength = baseBranchVariationStrength;
            branches.groups[1].variationStrength = gustBranchVariationStrength;
              /*leaves.groups[0].variationStrength = baseLeafVariationStrength;
              leaves.groups[1].variationStrength = gustLeafVariationStrength;
              leaves.groups[2].variationStrength = gustLeafVariationStrength;
              leaves.groups[3].variationStrength = gustLeafVariationStrength;
              plants.groups[0].variationStrength = basePlantVariationStrength;
              plants.groups[1].variationStrength = gustPlantVariationStrength;
              plants.groups[2].variationStrength = gustPlantVariationStrength;
              plants.groups[3].variationStrength = gustPlantVariationStrength;
               grass.groups[0].variationStrength = baseGrassVariationStrength;
               grass.groups[1].variationStrength = gustGrassVariationStrength;
               grass.groups[2].variationStrength = gustGrassVariationStrength;
               grass.groups[3].variationStrength = gustGrassMicroVariationStrength;
              
              trunks.groups[0].variationStrengthMin = trunkVariationStrengthMin;
              trunks.groups[1].variationStrengthMin = trunkVariationStrengthMin;#1#
            branches.groups[0].variationStrengthMin = branchVariationStrengthMin;
            branches.groups[1].variationStrengthMin = branchVariationStrengthMin;
              /*leaves.groups[0].variationStrengthMin = leafVariationStrengthMin;
              leaves.groups[1].variationStrengthMin = leafVariationStrengthMin;
              leaves.groups[2].variationStrengthMin = leafVariationStrengthMin;
              leaves.groups[3].variationStrengthMin = leafVariationStrengthMin;
              plants.groups[0].variationStrengthMin = plantVariationStrengthMin;
              plants.groups[1].variationStrengthMin = plantVariationStrengthMin;
              plants.groups[2].variationStrengthMin = plantVariationStrengthMin;
              plants.groups[3].variationStrengthMin = plantVariationStrengthMin;
               grass.groups[0].variationStrengthMin = grassVariationStrengthMin;
               grass.groups[1].variationStrengthMin = grassVariationStrengthMin;
               grass.groups[2].variationStrengthMin = grassVariationStrengthMin;
               grass.groups[3].variationStrengthMin = grassMicroVariationStrengthMin;
              
              trunks.groups[0].variationStrengthMax = trunkVariationStrengthMax;
              trunks.groups[1].variationStrengthMax = trunkVariationStrengthMax;#1#
            branches.groups[0].variationStrengthMax = branchVariationStrengthMax;
            branches.groups[1].variationStrengthMax = branchVariationStrengthMax;
              /*leaves.groups[0].variationStrengthMax = leafVariationStrengthMax;
              leaves.groups[1].variationStrengthMax = leafVariationStrengthMax;
              leaves.groups[2].variationStrengthMax = leafVariationStrengthMax;
              leaves.groups[3].variationStrengthMax = leafVariationStrengthMax;
              plants.groups[0].variationStrengthMax = plantVariationStrengthMax;
              plants.groups[1].variationStrengthMax = plantVariationStrengthMax;
              plants.groups[2].variationStrengthMax = plantVariationStrengthMax;
              plants.groups[3].variationStrengthMax = plantVariationStrengthMax;
               grass.groups[0].variationStrengthMax = grassVariationStrengthMax;
               grass.groups[1].variationStrengthMax = grassVariationStrengthMax;
               grass.groups[2].variationStrengthMax = grassVariationStrengthMax;
               grass.groups[3].variationStrengthMax = grassMicroVariationStrengthMax;#1#
            
        }*/

        /*
        [FoldoutGroup("System/Ranges")] public float strengthMin = 0.0f;
        [FoldoutGroup("System/Ranges")] public float strengthMax = 1.0f;
    
        [BoxGroup("System/Ranges/Trunk")] public float trunkCycleTimeMin = 1.0f;
        [BoxGroup("System/Ranges/Trunk")] public float trunkCycleTimeMax = 60.0f;
        [BoxGroup("System/Ranges/Trunk")] public float trunkFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Trunk")] public float trunkFieldSizeMax = 8192.0f;
        
        [BoxGroup("System/Ranges/Branch")] public float branchCycleTimeMin = 0.1f;
        [BoxGroup("System/Ranges/Branch")] public float branchCycleTimeMax = 30.0f;
        [BoxGroup("System/Ranges/Branch")] public float branchFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Branch")] public float branchFieldSizeMax = 1024.0f;
        [BoxGroup("System/Ranges/Branch")] public float branchVariationStrengthMin = 0.99f;
        [BoxGroup("System/Ranges/Branch")] public float branchVariationStrengthMax = 99.99f;
    
        [BoxGroup("System/Ranges/Leaf")] public float leafCycleTimeMin = .001f;
        [BoxGroup("System/Ranges/Leaf")] public float leafCycleTimeMax = 10.0f;
        [BoxGroup("System/Ranges/Leaf")] public float leafFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Leaf")] public float leafFieldSizeMax = 64.0f;
    
        [BoxGroup("System/Ranges/Plant")] public float plantCycleTimeMin = .001f;
        [BoxGroup("System/Ranges/Plant")] public float plantCycleTimeMax = 30.0f;
        [BoxGroup("System/Ranges/Plant")] public float plantFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Plant")] public float plantFieldSizeMax = 256.0f;
    
        [BoxGroup("System/Ranges/Grass")] public float grassCycleTimeMin = .001f;
        [BoxGroup("System/Ranges/Grass")] public float grassCycleTimeMax = 10.0f;
        [BoxGroup("System/Ranges/Grass")] public float grassFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Grass")] public float grassFieldSizeMax = 64.0f;
    
        [BoxGroup("System/Ranges/Grass")] public float grassMicroCycleTimeMin = .001f;
        [BoxGroup("System/Ranges/Grass")] public float grassMicroCycleTimeMax = 10.0f;
        [BoxGroup("System/Ranges/Grass")] public float grassMicroFieldSizeMin = 1.0f;
        [BoxGroup("System/Ranges/Grass")] public float grassMicroFieldSizeMax = 64.0f;
        */

        /*
    [TitleGroup("Audio Factor")]
    [PropertyRange(.01f, 1.0f)]
    public float gustAmplitudeAudioSharpness = 0.25f;

    [TitleGroup("Audio Factor")]
    [MinMaxSlider(.0001f, 0.05f, true), SuffixLabel("/s")]
    public Vector2 gustAudioStep = new Vector2(.001f, .01f);
    */

        /*
        [HideInInspector]
        public float trunkStrength = 1.0f;
    
        [HideInInspector]
        public float baseTrunkCycleTime = 6.0f;
    
        [BoxGroup("Properties/Trunk/Base Wind")]
        [PropertyRange(nameof(trunkFieldSizeMin), nameof(trunkFieldSizeMax)), SuffixLabel("meters")]
        public float baseTrunkFieldSize = 512.0f;
    
        [BoxGroup("Properties/Trunk/Gust Wind")]
        [PropertyRange(nameof(trunkCycleTimeMin), nameof(trunkCycleTimeMax)), SuffixLabel("seconds")]
        public float gustTrunkCycleTime = 1.0f;
    
        [BoxGroup("Properties/Trunk/Gust Wind")]
        [PropertyRange(nameof(trunkFieldSizeMin), nameof(trunkFieldSizeMax)), SuffixLabel("meters")]
        public float gustTrunkFieldSize = 2048.0f;

        [FoldoutGroup("Properties/Branch")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        public float branchStrength = 1.0f;
    
        [BoxGroup("Properties/Branch/Base Wind")]
        [PropertyRange(nameof(branchCycleTimeMin), nameof(branchCycleTimeMax)), SuffixLabel("seconds")]
        public float baseBranchCycleTime = 1.2f;
    
        [BoxGroup("Properties/Branch/Base Wind")]
        [PropertyRange(nameof(branchFieldSizeMin), nameof(branchFieldSizeMax)), SuffixLabel("meters")]
        public float baseBranchFieldSize = 256.0f;
    
        [BoxGroup("Properties/Branch/Base Wind")]
        [PropertyRange(nameof(branchVariationStrengthMin), nameof(branchVariationStrengthMax))]
        public float baseBranchVariationStrength = 10.88f;
    
        [BoxGroup("Properties/Branch/Gust Wind")]
        [PropertyRange(nameof(branchCycleTimeMin), nameof(branchCycleTimeMax)), SuffixLabel("seconds")]
        public float gustBranchCycleTime = 0.35f;
    
        [BoxGroup("Properties/Branch/Gust Wind")]
        [PropertyRange(nameof(branchFieldSizeMin), nameof(branchFieldSizeMax)), SuffixLabel("meters")]
        public float gustBranchFieldSize = 128.0f; 
    
        [BoxGroup("Properties/Branch/Gust Wind")]
        [PropertyRange(nameof(branchVariationStrengthMin), nameof(branchVariationStrengthMax))]
        public float gustBranchVariationStrength = 20.99f;

        [FoldoutGroup("Properties/Leaf")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        public float leafStrength = 1.0f;
    
        [BoxGroup("Properties/Leaf/Base Wind")]
        [PropertyRange(nameof(leafCycleTimeMin), nameof(leafCycleTimeMax)), SuffixLabel("seconds")]
        public float baseLeafCycleTime = 2.0f;
    
        [BoxGroup("Properties/Leaf/Base Wind")]
        [PropertyRange(nameof(leafFieldSizeMin), nameof(leafFieldSizeMax)), SuffixLabel("meters")]
        public float baseLeafFieldSize = 8.0f;
    
        [BoxGroup("Properties/Leaf/Gust Wind")]
        [PropertyRange(nameof(leafCycleTimeMin), nameof(leafCycleTimeMax)), SuffixLabel("seconds")]
        public float gustLeafCycleTime = 0.25f;
    
        [BoxGroup("Properties/Leaf/Gust Wind")]
        [PropertyRange(nameof(leafFieldSizeMin), nameof(leafFieldSizeMax)), SuffixLabel("meters")]
        public float gustLeafFieldSize = 4.0f;

        [FoldoutGroup("Properties/Plant")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        public float plantStrength = 1.0f;
    
        [BoxGroup("Properties/Plant/Base Wind")]
        [PropertyRange(nameof(plantCycleTimeMin), nameof(plantCycleTimeMax)), SuffixLabel("seconds")]
        public float basePlantCycleTime = 2.0f;
    
        [BoxGroup("Properties/Plant/Base Wind")]
        [PropertyRange(nameof(plantFieldSizeMin), nameof(plantFieldSizeMax)), SuffixLabel("meters")]
        public float basePlantFieldSize = 8.0f;
    
        [BoxGroup("Properties/Plant/Gust Wind")]
        [PropertyRange(nameof(plantCycleTimeMin), nameof(plantCycleTimeMax)), SuffixLabel("seconds")]
        public float gustPlantCycleTime = 0.25f;
    
        [BoxGroup("Properties/Plant/Gust Wind")]
        [PropertyRange(nameof(plantFieldSizeMin), nameof(plantFieldSizeMax)), SuffixLabel("meters")]
        public float gustPlantFieldSize = 4.0f;

        [FoldoutGroup("Properties/Grass")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        public float grassStrength = 1.0f;
    
        [BoxGroup("Properties/Grass/Base Wind")]
        [PropertyRange(nameof(grassCycleTimeMin), nameof(grassCycleTimeMax)), SuffixLabel("seconds")]
        public float baseGrassCycleTime = 2.0f;
    
        [BoxGroup("Properties/Grass/Base Wind")]
        [PropertyRange(nameof(grassFieldSizeMin), nameof(grassFieldSizeMax)), SuffixLabel("meters")]
        public float baseGrassFieldSize = 8.0f;
    
        [BoxGroup("Properties/Grass/Gust Wind")]
        [PropertyRange(nameof(grassCycleTimeMin), nameof(grassCycleTimeMax)), SuffixLabel("seconds")]
        public float gustGrassCycleTime = 0.25f;
    
        [BoxGroup("Properties/Grass/Gust Wind")]
        [PropertyRange(nameof(grassFieldSizeMin), nameof(grassFieldSizeMax)), SuffixLabel("meters")]
        public float gustGrassFieldSize = 4.0f;

        [FoldoutGroup("Properties/Grass/Micro")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        public float grassMicroStrength = 1.0f;

        [BoxGroup("Properties/Grass/Micro/Gust Wind")]
        [PropertyRange(nameof(grassMicroCycleTimeMin), nameof(grassMicroCycleTimeMax)), SuffixLabel("seconds")]
        public float gustGrassMicroCycleTime = 0.25f;
    
        [BoxGroup("Properties/Grass/Micro/Gust Wind")]
        [PropertyRange(nameof(grassMicroFieldSizeMin), nameof(grassMicroFieldSizeMax)), SuffixLabel("meters")]
        public float gustGrassMicroFieldSize = 4.0f;
        */
    }
}
