using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Species
{
    public class TreeSpeciesBuildToolbar : BaseBuildToolbar<TreeDataContainer>
    {
        private static TreeSpeciesBuildToolbar _instance;

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

        protected override QualityMode GetQuality(TreeDataContainer data)
        {
            return data.settings.qualityMode;
        }

        protected override void SetQuality(TreeDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        protected override void Full()
        {
            TreeBuildRequestManager.Full();
        }

        protected override void Default()
        {
            TreeBuildRequestManager.Default();
        }

        protected override void TextureOnly()
        {
            TreeBuildRequestManager.TextureOnly();
        }

        protected override bool SupportsColliderOnly => true;

        protected override void CollidersOnly()
        {
            TreeBuildRequestManager.CollidersOnly();
        }

        protected override bool SupportsImpostorOnly => true;

        protected override void ImpostorsOnly()
        {
            TreeBuildRequestManager.ImpostorsOnly();
        }

        protected override void ForceFull()
        {
            TreeBuildRequestManager.ForceFull();
        }
    }
}