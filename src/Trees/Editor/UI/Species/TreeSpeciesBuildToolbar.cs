using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Species
{
    public class TreeSpeciesBuildToolbar : BaseBuildToolbar<TreeDataContainer>
    {
        #region Static Fields and Autoproperties

        private static TreeSpeciesBuildToolbar _instance;

        #endregion

        public static TreeSpeciesBuildToolbar instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TreeSpeciesBuildToolbar();
                }

                return _instance;
            }
        }

        /// <inheritdoc />
        protected override bool SupportsColliderOnly => true;

        /// <inheritdoc />
        protected override bool SupportsImpostorOnly => true;

        /// <inheritdoc />
        protected override void CollidersOnly()
        {
            TreeBuildRequestManager.CollidersOnly();
        }

        /// <inheritdoc />
        protected override void Default()
        {
            TreeBuildRequestManager.Default();
        }

        /// <inheritdoc />
        protected override void ForceFull()
        {
            TreeBuildRequestManager.ForceFull();
        }

        /// <inheritdoc />
        protected override void Full()
        {
            TreeBuildRequestManager.Full();
        }

        /// <inheritdoc />
        protected override QualityMode GetQuality(TreeDataContainer data)
        {
            return data.settings.qualityMode;
        }

        /// <inheritdoc />
        protected override void ImpostorsOnly()
        {
            TreeBuildRequestManager.ImpostorsOnly();
        }

        /// <inheritdoc />
        protected override void SetQuality(TreeDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        /// <inheritdoc />
        protected override void TextureOnly()
        {
            TreeBuildRequestManager.TextureOnly();
        }
    }
}
