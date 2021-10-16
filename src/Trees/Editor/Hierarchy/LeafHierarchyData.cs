using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Geometry.Leaves;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Sirenix.OdinInspector;
using TreeEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public sealed class LeafHierarchyData : HierarchyData
    {
        [TreeHeader]
        [PropertyOrder(0)]
        //[FoldoutGroup("Shape", false)]
        [TabGroup("Shape", Paddingless = true)]
        public LeafSettings geometry;

        public LeafHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) : base(
            hierarchyID,
            parentHierarchyID,
            type
        )
        {
            geometry = new LeafSettings(type);
        }

        public LeafHierarchyData(
            int hierarchyID,
            int parentHierarchyID,
            TreeGroupLeaf group,
            Material leafMaterial,
            GameObject prefab) : base(hierarchyID, parentHierarchyID, group)
        {
            geometry = new LeafSettings(ResponsiveSettingsType.Tree)
            {
                prefab = new PrefabSetup(ResponsiveSettingsType.Tree) {prefab = prefab}
            };

            geometry.leafMaterial = leafMaterial;
            geometry.size.SetValue(group.size);
            geometry.bendFactor.SetValue(0.5f);

            geometry.geometryMode = ((TreeGroupLeaf.GeometryMode) group.geometryMode).ToInternal();
        }

        public override TreeComponentType type => TreeComponentType.Leaf;

        protected override Object[] GetExternalObjects()
        {
            if (geometry.geometryMode == LeafGeometryMode.Mesh)
            {
                return new Object[] {geometry.prefab.prefab};
            }

            if (geometry.leafMaterial != null)
            {
                return new Object[] {geometry.leafMaterial};
            }

            return null;
        }

        protected override void CopyInternalGenerationSettings(HierarchyData model)
        {
            var cast = model as LeafHierarchyData;
            geometry = cast.geometry.Clone();
        }

        public string GetMenuString()
        {
            if (geometry.leafMaterial)
            {
                return $"{hierarchyID}: {geometry.leafMaterial.name}";
            }

            if (geometry.prefab.prefab)
            {
                return $"{hierarchyID}: {geometry.prefab.prefab.name}";
            }

            return $"{hierarchyID}";
        }
        
        /*public override void ToggleCheckboxes(bool enabled)
        {
            distribution.ToggleCheckboxes(enabled);
            geometry.ToggleCheckboxes(enabled);
        }*/
        
        public override string GetSortKey()
        {
            if (geometry.leafMaterial != null)
            {
                return geometry.leafMaterial.name;
            }

            if ((geometry.prefab != null) && (geometry.prefab.prefab != null))
            {
                return geometry.prefab.prefab.name;
            }

            return $"{hierarchyID:0000}";
        }
    }
}
