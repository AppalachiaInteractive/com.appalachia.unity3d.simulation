using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.UI.Branches;
using Appalachia.Utility.Extensions;
using UnityEditor;

namespace Appalachia.Simulation.Trees.UI.UV
{
    [InitializeOnLoad]
    public class BranchUVEditor : UVEditor<BranchDataContainer>
    {
        #region Fields and Autoproperties

        private BranchDataContainer _container;

        #endregion

        /// <inheritdoc />
        public override BranchDataContainer container
        {
            get
            {
                if (BranchEditor.branchData == null)
                {
                    _container = null;
                }
                else if (_container == null)
                {
                    ResetState();
                    _container = BranchEditor.branchData;
                }
                else if (_container != BranchEditor.branchData)
                {
                    ResetState();
                    _container = BranchEditor.branchData;
                }

                return _container;
            }
            set => _container = value;
        }

        public static void OpenInstance()
        {
            if (instance == null)
            {
                instance = GetWindow<BranchUVEditor>();
            }

            instance.position = instance.position.AlignCenter(700, 700);
            instance.container = BranchEditor.branchData;
        }

        /// <inheritdoc />
        protected override BranchDataContainer GetContainer()
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
