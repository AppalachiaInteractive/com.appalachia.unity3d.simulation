#region

using System;
using System.Collections.Generic;
using Appalachia.Globals.Shading;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core.Metadata.Wind
{
    [Serializable]
    public class WindParameterCategory
    {
        public enum WindParameterCategoryType
        {
            Unassigned,
            Trunks,
            Branches,
            Leaves,
            Plants,
            Grass
        }

        [HideInInspector] public WindParameterCategoryType category;

        [Title("$" + nameof(category))]
        [PropertyRange(0.0f, 1.0f)]
        [OnValueChanged(nameof(ApplyProperties))]
        public float strength = 1.0f;

        private int strengthPropertyID;

        [FoldoutGroup("Groups")]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = false,
            ShowPaging = false,
            ShowItemCount = false,
            HideAddButton = true,
            HideRemoveButton = true,
            NumberOfItemsPerPage = 5
        )]
        public List<WindParameterGroup> groups = new List<WindParameterGroup>();

        public WindParameterCategory(WindParameterCategoryType category)
        {
            this.category = category;
        }

        public void ApplyProperties()
        {
            if (strengthPropertyID == 0)
            {
                var propertyName = string.Empty;

                switch (category)
                {
                    case WindParameterCategoryType.Trunks:
                        propertyName = GSC.WIND._WIND_TRUNK_STRENGTH;
                        break;
                    case WindParameterCategoryType.Branches:
                        propertyName = GSC.WIND._WIND_BRANCH_STRENGTH;
                        break;
                    case WindParameterCategoryType.Leaves:
                        propertyName = GSC.WIND._WIND_LEAF_STRENGTH;
                        break;
                    case WindParameterCategoryType.Plants:
                        propertyName = GSC.WIND._WIND_PLANT_STRENGTH;
                        break;
                    case WindParameterCategoryType.Grass:
                        propertyName = GSC.WIND._WIND_GRASS_STRENGTH;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                strengthPropertyID = GSPL.Get(propertyName);
            }

            Shader.SetGlobalFloat(strengthPropertyID, strength);

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];

                group.ApplyProperties();
            }
        }
    }
}
