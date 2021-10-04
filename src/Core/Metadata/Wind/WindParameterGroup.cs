#region

using System;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core.Metadata.Wind
{
    [Serializable]
    public class WindParameterGroup
    {
        public enum WindParameterGroupType
        {
            Unassigned,
            Base,
            Gust,
            Mid,
            Micro
        }

        [Title("$" + nameof(group))]
        [FoldoutGroup("$" + nameof(group))]
        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public bool showAdvanced;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showAdvanced))]
        [SmartLabel]
        public bool showVariation;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showAdvanced))]
        [SmartLabel]
        public bool showBranchProperties;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showAdvanced))]
        [SmartLabel]
        public WindParameterCategory.WindParameterCategoryType category;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showAdvanced))]
        [SmartLabel]
        public WindParameterGroupType group;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float strengthMin;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float strengthMax = 1.0f;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float cycleTimeMin = .001f;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float cycleTimeMax = 10.0f;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float fieldSizeMin = 1.0f;

        [FoldoutGroup("$" + nameof(group) + "/Setup")]
        [SmartLabel]
        public float fieldSizeMax = 64.0f;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showVariation))]
        [SmartLabel]
        public float variationStrengthMin = 0.99f;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showVariation))]
        [SmartLabel]
        public float variationStrengthMax = 99.99f;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showBranchProperties))]
        [SmartLabel]
        public float branchStrengthMin;

        [FoldoutGroup("$" + nameof(group) + "/Setup"), ShowIf(nameof(showBranchProperties))]
        [SmartLabel]
        public float branchStrengthMax = 1.0f;

        [BoxGroup("$" + nameof(group) + "/Properties")]
        [OnValueChanged(nameof(ApplyProperties))]
        [SmartLabel]
        public bool enabled = true;

        [BoxGroup("$" + nameof(group) + "/Properties")]
        [PropertyRange(nameof(strengthMin), nameof(strengthMax))]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float strength = 1.0f;

        [BoxGroup("$" + nameof(group) + "/Properties")]
        [PropertyRange(nameof(cycleTimeMin), nameof(cycleTimeMax)), SuffixLabel("seconds")]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float cycleTime = 0.25f;

        [BoxGroup("$" + nameof(group) + "/Properties")]
        [PropertyRange(nameof(fieldSizeMin), nameof(fieldSizeMax)), SuffixLabel("meters")]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float fieldSize = 4.0f;

        [BoxGroup("$" + nameof(group) + "/Properties"), ShowIf(nameof(showVariation))]
        [PropertyRange(nameof(variationStrengthMin), nameof(variationStrengthMax))]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float variationStrength = 20.99f;

        [BoxGroup("$" + nameof(group) + "/Properties"), ShowIf(nameof(showBranchProperties))]
        [PropertyRange(nameof(branchStrengthMin), nameof(branchStrengthMax))]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float oppositeStrength = 1.0f;

        [BoxGroup("$" + nameof(group) + "/Properties"), ShowIf(nameof(showBranchProperties))]
        [PropertyRange(nameof(branchStrengthMin), nameof(branchStrengthMax))]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float perpendicularStrength = 1.0f;

        [BoxGroup("$" + nameof(group) + "/Properties"), ShowIf(nameof(showBranchProperties))]
        [PropertyRange(nameof(branchStrengthMin), nameof(branchStrengthMax))]
        [OnValueChanged(nameof(ApplyProperties)), EnableIf(nameof(enabled))]
        [SmartLabel]
        public float parallelStrength = 1.0f;

        private int[] propertyIDs = new int[7];

        public WindParameterGroup(WindParameterCategory.WindParameterCategoryType category, WindParameterGroupType groupType, bool showVariation)
        {
            this.category = category;
            group = groupType;
            this.showVariation = showVariation;
        }

        public void ApplyProperties()
        {
            propertyIDs = WindParameterHelper.EnsurePropertyIDLookupIsCreated(propertyIDs, category, group, showVariation, showBranchProperties);

            Shader.SetGlobalFloat(propertyIDs[0], enabled ? strength : 0.0f);
            Shader.SetGlobalFloat(propertyIDs[1], cycleTime);
            Shader.SetGlobalFloat(propertyIDs[2], fieldSize);

            if (showVariation)
            {
                Shader.SetGlobalFloat(propertyIDs[3], variationStrength);
            }

            if (showBranchProperties)
            {
                Shader.SetGlobalFloat(propertyIDs[4], oppositeStrength);
                Shader.SetGlobalFloat(propertyIDs[5], perpendicularStrength);
                Shader.SetGlobalFloat(propertyIDs[6], parallelStrength);
            }
        }
    }
}
