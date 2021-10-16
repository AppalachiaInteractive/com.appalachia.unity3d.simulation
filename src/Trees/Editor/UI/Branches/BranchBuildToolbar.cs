using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Branches
{
    public class BranchBuildToolbar : BaseBuildToolbar<BranchDataContainer>
    {
        private static BranchBuildToolbar _instance;

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

        
        protected override QualityMode GetQuality(BranchDataContainer data)
        {
            return data.settings.qualityMode;
        }

        protected override void SetQuality(BranchDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        protected override void Full()
        {
            BranchBuildRequestManager.Full();
        }

        protected override void Default()
        {
            BranchBuildRequestManager.Default();
        }

        protected override void TextureOnly()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsColliderOnly => false;

        protected override void CollidersOnly()
        {
            throw new System.NotImplementedException();
        }

        protected override bool SupportsImpostorOnly => false;

        protected override void ImpostorsOnly()
        {
            throw new System.NotImplementedException();
        }

        protected override void ForceFull()
        {
            BranchBuildRequestManager.ForceFull();
        }
    }
}
