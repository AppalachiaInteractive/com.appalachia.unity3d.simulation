#region

using System;
using Appalachia.Core.Globals.Shading;

#endregion

namespace Appalachia.Simulation.Wind
{
    public static class WindParameterHelper
    {
        public static int[] EnsurePropertyIDLookupIsCreated(
            int[] propertyIDs,
            WindParameterCategory.WindParameterCategoryType category,
            WindParameterGroup.WindParameterGroupType group,
            bool includeVariation,
            bool includeBranchProperties)
        {
            if ((propertyIDs == null) || (propertyIDs.Length != 7))
            {
                propertyIDs = new int[7];
            }

            var index = 0;
            string propertyName;

            if (propertyIDs[index] == 0)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Trunks:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_TRUNK_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_TRUNK_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_BRANCH_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Leaves:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_LEAF_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MID_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MICRO_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Plants:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_PLANT_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MID_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MICRO_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Grass:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_GRASS_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MID_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MICRO_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if (propertyIDs[index] == 0)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Trunks:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_TRUNK_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_TRUNK_CYCLE_TIME;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_BRANCH_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_CYCLE_TIME;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Leaves:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_LEAF_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MID_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MICRO_CYCLE_TIME;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Plants:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_PLANT_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MID_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MICRO_CYCLE_TIME;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Grass:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_GRASS_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MID_CYCLE_TIME;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MICRO_CYCLE_TIME;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if (propertyIDs[index] == 0)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Trunks:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_TRUNK_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_TRUNK_FIELD_SIZE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_BRANCH_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_FIELD_SIZE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Leaves:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_LEAF_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MID_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_LEAF_MICRO_FIELD_SIZE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Plants:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_PLANT_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MID_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_PLANT_MICRO_FIELD_SIZE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case WindParameterCategory.WindParameterCategoryType.Grass:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_GRASS_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Mid:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MID_FIELD_SIZE;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Micro:
                                propertyName = GSC.WIND._WIND_GUST_GRASS_MICRO_FIELD_SIZE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if ((propertyIDs[index] == 0) && includeVariation)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Base:
                                propertyName = GSC.WIND._WIND_BASE_BRANCH_VARIATION_STRENGTH;
                                break;
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_VARIATION_STRENGTH;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if ((propertyIDs[index] == 0) && includeBranchProperties)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_STRENGTH_OPPOSITE;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if ((propertyIDs[index] == 0) && includeBranchProperties)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_STRENGTH_PERPENDICULAR;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            index += 1;
            if ((propertyIDs[index] == 0) && includeBranchProperties)
            {
                switch (category)
                {
                    case WindParameterCategory.WindParameterCategoryType.Branches:
                        switch (group)
                        {
                            case WindParameterGroup.WindParameterGroupType.Gust:
                                propertyName = GSC.WIND._WIND_GUST_BRANCH_STRENGTH_PARALLEL;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                propertyIDs[index] = GSPL.Get(propertyName);
            }

            return propertyIDs;
        }
    }
}
