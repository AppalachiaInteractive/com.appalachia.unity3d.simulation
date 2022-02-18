using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Branches
{
    public class BranchBuildToolbar : BaseBuildToolbar<BranchDataContainer>
    {
        #region Static Fields and Autoproperties

        private static BranchBuildToolbar _instance;

        #endregion

        public static BranchBuildToolbar instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BranchBuildToolbar();
                }

                return _instance;
            }
        }

        /// <inheritdoc />
        protected override bool SupportsColliderOnly => false;

        /// <inheritdoc />
        protected override bool SupportsImpostorOnly => false;

        /// <inheritdoc />
        protected override void CollidersOnly()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        protected override void Default()
        {
            BranchBuildRequestManager.Default();
        }

        /// <inheritdoc />
        protected override void ForceFull()
        {
            BranchBuildRequestManager.ForceFull();
        }

        /// <inheritdoc />
        protected override void Full()
        {
            BranchBuildRequestManager.Full();
        }

        /// <inheritdoc />
        protected override QualityMode GetQuality(BranchDataContainer data)
        {
            return data.settings.qualityMode;
        }

        /// <inheritdoc />
        protected override void ImpostorsOnly()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        protected override void SetQuality(BranchDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        /// <inheritdoc />
        protected override void TextureOnly()
        {
            throw new System.NotImplementedException();
        }
    }
}
