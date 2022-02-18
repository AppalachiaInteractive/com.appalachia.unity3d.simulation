#region

using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.UI.Species;
using Appalachia.Utility.Extensions;

#endregion

namespace Appalachia.Simulation.Trees.UI.UV
{
    public class TreeUVEditor : UVEditor<TreeDataContainer>
    {
        #region Fields and Autoproperties

        private TreeDataContainer _container;

        #endregion

        /// <inheritdoc />
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

        public static void OpenInstance()
        {
            if (instance == null)
            {
                instance = GetWindow<TreeUVEditor>();
            }

            instance.position = instance.position.AlignCenter(700, 700);
            instance.container = TreeSpeciesEditor._tree;
        }

        /// <inheritdoc />
        protected override TreeDataContainer GetContainer()
        {
            return container;
        }

        /// <inheritdoc />
        protected override InputMaterialCache GetData()
        {
            return container.materials.inputMaterialCache;
        }
    }
}
