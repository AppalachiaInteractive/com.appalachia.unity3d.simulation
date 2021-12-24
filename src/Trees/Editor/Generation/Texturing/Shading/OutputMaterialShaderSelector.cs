#region

using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using Appalachia.Simulation.Trees.UI;
using Sirenix.OdinInspector.Editor;

#endregion

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading
{
    public class OutputMaterialShaderSelector : OdinSelector<IOutputMaterialShader>
    {
        private IReadOnlyCollection<IOutputMaterialShader> source;

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            if (source == null)
            {
                source = TreeShaderFactory.GetOutputMaterialShaders();
            }

            tree.DefaultMenuStyle = TreeGUI.MenuStyles.MaterialSelectionMenuStyle;
            tree.Config = TreeGUI.MenuStyles.MaterialSelectionMenuConfig;
            tree.Config.DefaultMenuStyle = TreeGUI.MenuStyles.MaterialSelectionMenuStyle;
            tree.Selection.SupportsMultiSelect = false;

            foreach (var s in source)
            {
                tree.Add(s.Name, s);
            }

            if (_current != null)
            {
                tree.MenuItems.FirstOrDefault(s => s.Value == _current)?.Select(true);
            }
        }

        public void Show(IOutputMaterialShader current)
        {
            if (current != null)
            {
                _current = current;
            }
            
            ShowInPopup(200);
        }

        private IOutputMaterialShader _current;
        
        
    }
}
