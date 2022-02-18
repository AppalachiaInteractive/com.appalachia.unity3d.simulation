using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public sealed class FruitHierarchyData : HierarchyData
    {
        public FruitHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) : base(
            hierarchyID,
            parentHierarchyID,
            type
        )
        {
            geometry = new FruitSettings(type);
        }

        #region Fields and Autoproperties

        [TreeHeader, PropertyOrder(0)]

        //[FoldoutGroup("Shape", false)] 
        [TabGroup("Shape", Paddingless = true)]
        public FruitSettings geometry;

        #endregion

        /// <inheritdoc />
        public override TreeComponentType type => TreeComponentType.Fruit;

        /*public override void ToggleCheckboxes(bool enabled)
        {
            distribution.ToggleCheckboxes(enabled);
            geometry.ToggleCheckboxes(enabled);
        }*/

        /// <inheritdoc />
        public override string GetSortKey()
        {
            if ((geometry.prefab != null) && (geometry.prefab.prefab != null))
            {
                return geometry.prefab.prefab.name;
            }

            return ZString.Format("{0:0000}", hierarchyID);
        }

        /// <inheritdoc />
        protected override void CopyInternalGenerationSettings(HierarchyData model)
        {
            var cast = model as FruitHierarchyData;
            geometry = cast.geometry.Clone();
        }

        /// <inheritdoc />
        protected override Object[] GetExternalObjects()
        {
            return new Object[] { geometry.prefab.prefab };
        }
    }
}
