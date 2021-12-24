using Appalachia.Simulation.Trees.Core;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    public class TreeSpeciesEditorSelection : SingletonAppalachiaTreeObject<TreeSpeciesEditorSelection>
    {
        public TreeSelection tree;
        
        public BranchSelection branch;
        
        public BranchSettingsSelection branchCopySettings;
        
        public TreeSettingsSelection treeCopySettings;
        
        public LogSelection log;
        
        public void Refresh()
        {
            tree.Refresh();
            branch.Refresh();
            branchCopySettings.Refresh();
            treeCopySettings.Refresh();
            log.Refresh();
        }
    }
}
