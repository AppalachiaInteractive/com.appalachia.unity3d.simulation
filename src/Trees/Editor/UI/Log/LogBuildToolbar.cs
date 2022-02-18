using System;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Base;

namespace Appalachia.Simulation.Trees.UI.Log
{
    public class LogBuildToolbar : BaseBuildToolbar<LogDataContainer>
    {
        #region Static Fields and Autoproperties

        private static LogBuildToolbar _instance;

        #endregion

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

        /// <inheritdoc />
        protected override bool SupportsColliderOnly => false;

        /// <inheritdoc />
        protected override bool SupportsImpostorOnly => false;

        /// <inheritdoc />
        protected override void CollidersOnly()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void Default()
        {
            LogBuildRequestManager.Default();
        }

        /// <inheritdoc />
        protected override void ForceFull()
        {
            LogBuildRequestManager.ForceFull();
        }

        /// <inheritdoc />
        protected override void Full()
        {
            LogBuildRequestManager.Full();
        }

        /// <inheritdoc />
        protected override QualityMode GetQuality(LogDataContainer data)
        {
            return data.settings.qualityMode;
        }

        /// <inheritdoc />
        protected override void ImpostorsOnly()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void SetQuality(LogDataContainer data, QualityMode mode)
        {
            data.settings.qualityMode = mode;
        }

        /// <inheritdoc />
        protected override void TextureOnly()
        {
            throw new NotImplementedException();
        }
    }
}
