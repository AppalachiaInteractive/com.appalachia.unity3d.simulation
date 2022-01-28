using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
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
    [CallStaticConstructorInEditor]
    public class TreeSpeciesHierarchyEditor : HierarchyBaseEditor<TreeDataContainer>
    {
        static TreeSpeciesHierarchyEditor()
        {
            RegisterInstanceCallbacks.WithoutSorting().When.Object<TreeSpeciesEditorSelection>().IsAvailableThen( i => _treeSpeciesEditorSelection = i);
        }

        #region Static Fields and Autoproperties

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        #endregion

        protected override bool AreHierarchyButtonsEnabled()
        {
            var s = _treeSpeciesEditorSelection.tree;

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

        protected override HierarchyData CreateHierarchy(
            IHierarchyWrite hierarchies,
            TreeComponentType type,
            HierarchyData parent)
        {
            var h = hierarchies.CreateHierarchy(
                type,
                parent,
                _treeSpeciesEditorSelection.tree.selection.selected.materials.inputMaterialCache
            );

            return h;
        }

        protected override HierarchyData CreateTrunkHierarchy(IHierarchyWrite hierarchies)
        {
            var h = hierarchies.CreateTrunkHierarchy(
                _treeSpeciesEditorSelection.tree.selection.selected.materials.inputMaterialCache
            );

            return h;
        }

        protected override IBasicSelection GetSelection()
        {
            return _treeSpeciesEditorSelection.tree;
        }

        protected override void SettingsChanged()
        {
            TreeBuildRequestManager.SettingsChanged(SettingsUpdateTarget.Distribution);
        }
    }
}
