using System;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Log
{
    public class LogBuildToolbar : BaseBuildToolbar<LogDataContainer>
    {
        private static LogBuildToolbar _instance;

        public static LogBuildToolbar instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogBuildToolbar();
                }

                return _instance;
            }
        }

        protected override QualityMode GetQuality(LogDataContainer data)
        {
            return data.settings.qualityMode;
        }

        protected override void SetQuality(LogDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        protected override void Full()
        {
            LogBuildRequestManager.Full();
        }

        protected override void Default()
        {
            LogBuildRequestManager.Default();
        }

        protected override void TextureOnly()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsColliderOnly => false;

        protected override void CollidersOnly()
        {
            throw new NotImplementedException();
        }

        protected override bool SupportsImpostorOnly => false;

        protected override void ImpostorsOnly()
        {
            throw new NotImplementedException();
        }

        protected override void ForceFull()
        {
            LogBuildRequestManager.ForceFull();
        }
    }
}