using System;
using Sirenix.OdinInspector;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemMultipleCamerasShotSelector : ReactionSubsystemMultipleCameras
    {
        [PropertyRange(1, 300)]
        [ShowIf(nameof(_selectionModeXFrames))]
        public int frameShotInterval = 4;

        private SubsystemCameraShotSelector shotSelector;
        public abstract SubsystemCameraShotSelectionMode selectionMode { get; }

        private bool _selectionModeXFrames =>
            selectionMode == SubsystemCameraShotSelectionMode.EveryXFrames;

        protected override void OnUpdateLoopStart()
        {
            if (shotSelector == default)
            {
                shotSelector = new SubsystemCameraShotSelector();
            }

            shotSelector.InitiateCheck(cameraComponents.Count);
        }

        protected override bool ShouldRenderCamera(
            SubsystemCameraComponent cam,
            int cameraIndex,
            int totalCameras)
        {
            return shotSelector.ShouldRenderCamera(
                selectionMode,
                cam,
                cameraIndex,
                totalCameras,
                frameShotInterval
            );
        }
    }
}
