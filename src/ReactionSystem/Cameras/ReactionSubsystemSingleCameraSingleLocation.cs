using System;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemSingleCameraSingleLocation<T> : ReactionSubsystemSingleCamera<T>
        where T : ReactionSubsystemSingleCameraSingleLocation<T>
    {
        /// <inheritdoc />
        protected override void OnRenderStart()
        {
            using (_PRF_OnRenderStart.Auto())
            {
                var tempCameraPosition = cameraComponent.GetCameraRootPosition();
                var pos = tempCameraPosition;
                pos += CameraOffset;

                var ct = cameraComponent.renderCamera.transform;

                ct.position = pos;
                ct.rotation = Quaternion.LookRotation(CameraDirection, Vector3.forward);
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_OnRenderStart = new(_PRF_PFX + nameof(OnRenderStart));

        #endregion
    }
}
