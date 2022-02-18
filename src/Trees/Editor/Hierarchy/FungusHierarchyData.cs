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
    public sealed class FungusHierarchyData : HierarchyData
    {
        public FungusHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) :
            base(hierarchyID, parentHierarchyID, type)

        {
            geometry = new FungusSettings(type);
        }

        #region Fields and Autoproperties

        [TreeHeader, PropertyOrder(0)]

        //[FoldoutGroup("Shape", false)]
        [TabGroup("Shape", Paddingless = true)]
        public FungusSettings geometry;

        #endregion

        /// <inheritdoc />
        public override TreeComponentType type => TreeComponentType.Fungus;

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
            var cast = model as FungusHierarchyData;
            geometry = cast.geometry.Clone();
        }

        /// <inheritdoc />
        protected override Object[] GetExternalObjects()
        {
            return new Object[] { geometry.prefab.prefab };
        }
    }
}
