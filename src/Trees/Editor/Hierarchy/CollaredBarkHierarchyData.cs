using System;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public abstract class CollaredBarkHierarchyData : BarkHierarchyData
    {
        //[ShowIfGroup("CL", Condition = nameof(showCollar), Animate = false)]
        [TreeHeader, /*ShowIf(nameof(showCollar)),*/ PropertyOrder(100)]
        //[FoldoutGroup("CL/Branch Collar", false)]
        [ShowIf(nameof(showCollar))]
        [TabGroup("Branch Collar", Paddingless = true)]
        public CollarSettings collar;

        /*protected virtual bool showCollar { get; } = true;*/

        protected bool showCollar => !geometry.forked && !limb.log;

        protected CollaredBarkHierarchyData(int hierarchyID, int parentHierarchyID, ResponsiveSettingsType type) : base(
            hierarchyID, parentHierarchyID, type)
        {
            collar = new CollarSettings(type);
        }

        protected CollaredBarkHierarchyData(
            int hierarchyID, int parentHierarchyID,
            TreeEditor.TreeGroupBranch group,
            Material barkMaterial,
            Material breakMaterial,
            Material frondMaterial) : base(hierarchyID, parentHierarchyID, group, barkMaterial, breakMaterial, frondMaterial)
        {
            collar = new CollarSettings(ResponsiveSettingsType.Tree);

            collar.collarHeight.SetValue(group.weldHeight);
            collar.collarSpreadTop.SetValue(group.weldSpreadTop);
            collar.collarSpreadBottom.SetValue(group.weldSpreadBottom);

        }
    }
}