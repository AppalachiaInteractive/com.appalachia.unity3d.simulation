using System;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemMultipleCamerasShotSelector<T> : ReactionSubsystemMultipleCameras<T>
        where T : ReactionSubsystemMultipleCamerasShotSelector<T>
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
        protected override void OnUpdateLoopStart()
        {
            if (shotSelector == default)
            {
                shotSelector = new SubsystemCameraShotSelector();
            }

            shotSelector.InitiateCheck(cameraComponents.Count);
        }

        /// <inheritdoc />
        protected override bool ShouldRenderCamera(
            SubsystemCameraComponent cam,
            int cameraIndex,
            int totalCameras)
        {
            return shotSelector.ShouldRenderCamera(
                SelectionMode,
                cam,
                cameraIndex,
                totalCameras,
                frameShotInterval
            );
        }
    }
}
