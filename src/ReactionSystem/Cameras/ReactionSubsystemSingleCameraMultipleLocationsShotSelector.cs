using System;
using Appalachia.Simulation.ReactionSystem.Base;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemSingleCameraMultipleLocationsShotSelector<T> :
            ReactionSubsystemSingleCameraMultipleLocations<T>
        where T : ReactionSubsystemSingleCameraMultipleLocationsShotSelector<T>
    {
        #region Fields and Autoproperties

        [PropertyRange(1, 300)]
        [ShowIf(nameof(SelectionModeXFrames))]
        public int frameShotInterval = 4;

        private SubsystemCameraShotSelector shotSelector;

        #endregion

        public abstract SubsystemCameraShotSelectionMode SelectionMode { get; }

        private bool SelectionModeXFrames => SelectionMode == SubsystemCameraShotSelectionMode.EveryXFrames;

        /// <inheritdoc />
        protected override ReactionSubsystemCenter GetCurrentSubsystemCenter()
        {
            if (shotSelector == default)
            {
                shotSelector = new SubsystemCameraShotSelector();
            }

            shotSelector.InitiateCheck(centers.Count);

            for (var i = 0; i < centers.Count; i++)
            {
                var shouldRender = shotSelector.ShouldRenderAtCenter(
                    SelectionMode,
                    centers[i],
                    i,
                    centers.Count,
                    frameShotInterval
                );

                if (shouldRender)
                {
                    return centers[i];
                }
            }

            return null;
        }
    }
}
