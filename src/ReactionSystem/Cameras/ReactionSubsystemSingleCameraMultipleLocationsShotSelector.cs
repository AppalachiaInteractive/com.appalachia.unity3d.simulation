using System;
using Appalachia.Simulation.ReactionSystem.Base;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemSingleCameraMultipleLocationsShotSelector :
            ReactionSubsystemSingleCameraMultipleLocations
    {
        [PropertyRange(1, 300)]
        [ShowIf(nameof(_selectionModeXFrames))]
        public int frameShotInterval = 4;

        private SubsystemCameraShotSelector shotSelector;
        public abstract SubsystemCameraShotSelectionMode selectionMode { get; }

        private bool _selectionModeXFrames =>
            selectionMode == SubsystemCameraShotSelectionMode.EveryXFrames;

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
                    selectionMode,
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
