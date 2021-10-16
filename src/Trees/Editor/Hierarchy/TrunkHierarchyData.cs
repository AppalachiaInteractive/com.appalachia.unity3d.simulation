using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Hierarchy.Options.Attributes;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Sirenix.OdinInspector;
using TreeEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Hierarchy
{
    [Serializable]
    public sealed class TrunkHierarchyData : BarkHierarchyData
    {

        private bool showTrunk => settingsType != ResponsiveSettingsType.Log;
        
        [TreeHeader]
        [PropertyOrder(100)]
        //[ShowIfGroup("TR", MemberName = nameof(showTrunk), Animate = false)]
        //[FoldoutGroup("TR/Trunk", false)]
        [ShowIf(nameof(showTrunk))]
        [TabGroup("Trunk", Paddingless = true)]
        public TrunkSettings trunk;

        public TrunkHierarchyData(int hierarchyId, ResponsiveSettingsType type) : base(hierarchyId, -1, type)
        {
            trunk = new TrunkSettings(type);
        }

        public TrunkHierarchyData(
            int hierarchyID,
            TreeGroupBranch trunkGroup,
            TreeGroupRoot root,
            Material barkMaterial,
            Material breakMaterial,
            Material frondMaterial) : base(hierarchyID, -1, trunkGroup, barkMaterial, breakMaterial, frondMaterial)
        {
            trunk = new TrunkSettings(ResponsiveSettingsType.Tree);

            trunk.trunkSpread.SetValue(root.rootSpread);
            trunk.flareHeight.SetValue(trunkGroup.flareHeight);
            trunk.flareNoise.SetValue(trunkGroup.flareNoise);
            trunk.flareRadius.SetValue(trunkGroup.flareSize);
            parentHierarchyID = -1;
        }

        public override TreeComponentType type => TreeComponentType.Trunk;

        protected override void CopyInternalGenerationSettings(HierarchyData model)
        {
            var cast = model as TrunkHierarchyData;
            trunk = cast.trunk.Clone();
            geometry = cast.geometry.Clone();
            limb = cast.limb.Clone();
            curvature = cast.curvature.Clone();
        }

        /*public override void ToggleCheckboxes(bool enabled)
        {
            trunk.ToggleCheckboxes(enabled);
            curvature.ToggleCheckboxes(enabled);
            distribution.ToggleCheckboxes(enabled);
            geometry.ToggleCheckboxes(enabled);
            limb.ToggleCheckboxes(enabled);
        }*/
    }
}
