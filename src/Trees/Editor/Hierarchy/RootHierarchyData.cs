using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using TreeEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public sealed class RootHierarchyData : CollaredBarkHierarchyData
    {
        public RootHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) : base(
            hierarchyID,
            parentHierarchyID,
            type
        )
        {
        }

        public RootHierarchyData(
            int hierarchyID,
            int parentHierarchyID,
            TreeGroupBranch group,
            Material barkMaterial,
            Material breakMaterial,
            Material frondMaterial) : base(
            hierarchyID,
            parentHierarchyID,
            group,
            barkMaterial,
            breakMaterial,
            frondMaterial
        )
        {
        }

        /// <inheritdoc />
        public override TreeComponentType type => TreeComponentType.Root;

        /// <inheritdoc />
        protected override void CopyInternalGenerationSettings(HierarchyData model)
        {
            var cast = model as RootHierarchyData;
            collar = cast.collar.Clone();
            geometry = cast.geometry.Clone();
            limb = cast.limb.Clone();
            curvature = cast.curvature.Clone();
        }

        /*public override void ToggleCheckboxes(bool enabled)
        {
            distribution.ToggleCheckboxes(enabled);
            collar.ToggleCheckboxes(enabled);
            geometry.ToggleCheckboxes(enabled);
            limb.ToggleCheckboxes(enabled);
            curvature.ToggleCheckboxes(enabled);
        }*/
    }
}
