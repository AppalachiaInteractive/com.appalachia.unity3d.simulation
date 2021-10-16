using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public sealed class FruitHierarchyData : HierarchyData
    {
        [TreeHeader, PropertyOrder(0)]
        //[FoldoutGroup("Shape", false)] 
        [TabGroup("Shape", Paddingless = true)] 
        public FruitSettings geometry;

        public override TreeComponentType type => TreeComponentType.Fruit;

        protected override Object[] GetExternalObjects()
        {
            return new Object[] {geometry.prefab.prefab};
        }

        public FruitHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) : base(
            hierarchyID,
            parentHierarchyID,
            type)
        {
            geometry = new FruitSettings(type);
        }
        
        
        protected override void CopyInternalGenerationSettings(HierarchyData model)
        {
            var cast = model as FruitHierarchyData;
            geometry = cast.geometry.Clone();
        }

        /*public override void ToggleCheckboxes(bool enabled)
        {
            distribution.ToggleCheckboxes(enabled);
            geometry.ToggleCheckboxes(enabled);
        }*/
        
        public override string GetSortKey()
        {
            if ((geometry.prefab != null) && (geometry.prefab.prefab != null))
            {
                return geometry.prefab.prefab.name;
            }

            return $"{hierarchyID:0000}";
        }
    }
}

