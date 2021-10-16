using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.Base;
using Appalachia.Simulation.Trees.UI.Selections.State;

namespace Appalachia.Simulation.Trees.UI.Species
{
    public class TreeSpeciesHierarchyEditor : HierarchyBaseEditor<TreeDataContainer>
    {
        protected override HierarchyData CreateTrunkHierarchy(IHierarchyWrite hierarchies)
        {
            var h = hierarchies.CreateTrunkHierarchy(
                TreeSpeciesEditorSelection.instance.tree.selection.selected.materials.inputMaterialCache
            );
            
            return h;
        }

        protected override HierarchyData CreateHierarchy(IHierarchyWrite hierarchies, TreeComponentType type, HierarchyData parent)
        {
            var h = hierarchies.CreateHierarchy(
                type, parent,
                TreeSpeciesEditorSelection.instance.tree.selection.selected.materials.inputMaterialCache
            );
            
            return h;
        }

        protected override void SettingsChanged()
        {
            TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }

        protected override bool AreHierarchyButtonsEnabled()
        {
            var s = TreeSpeciesEditorSelection.instance.tree;

            if ((s.selection == null) || (s.selection.selected == null))
            {
                return false;
            }
            
            if ((s.stage == StageType.Normal) && s.selection.selected.initialized)
            {
                return true;
            }

            return false;
        }

        protected override IBasicSelection GetSelection()
        {
            return TreeSpeciesEditorSelection.instance.tree;
        }
    }
}
