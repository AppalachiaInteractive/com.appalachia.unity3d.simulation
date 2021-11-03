#region

using Appalachia.Core.Extensions;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.UI.Species;
using Appalachia.Utility.Extensions;

#endregion

namespace Appalachia.Simulation.Trees.UI.UV
{
    public class TreeUVEditor : UVEditor<TreeDataContainer>
    {
        private TreeDataContainer _container;

        public override TreeDataContainer container
        {
            get
            {
                if (TreeSpeciesEditor._tree == null)
                {
                    _container = null;
                }
                else if (_container == null)
                {
                    ResetState();
                    _container = TreeSpeciesEditor._tree;
                }
                else if (_container != TreeSpeciesEditor._tree)
                {
                    ResetState();
                    _container = TreeSpeciesEditor._tree;
                }
                
                return _container;
            }
            set => _container = value;
        }

        protected override TreeDataContainer GetContainer()
        {
            return container;
        }

        protected override InputMaterialCache GetData()
        {
            return container.materials.inputMaterialCache;
        }

        public static void OpenInstance()
        {
            if (instance == null)
            {
                instance = GetWindow<TreeUVEditor>();
            }
            
            instance.position = instance.position.AlignCenter(700, 700);
            instance.container = TreeSpeciesEditor._tree;
        }
    }
}
