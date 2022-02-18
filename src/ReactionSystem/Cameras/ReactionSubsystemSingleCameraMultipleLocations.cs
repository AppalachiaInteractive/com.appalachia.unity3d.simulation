using System;
using System.Collections.Generic;
using Appalachia.Simulation.ReactionSystem.Base;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemSingleCameraMultipleLocations<T> : ReactionSubsystemSingleCamera<T>
        where T : ReactionSubsystemSingleCameraMultipleLocations<T>
    {
        #region Fields and Autoproperties

        [OnValueChanged(nameof(InitializeSynchronous))]
        public List<ReactionSubsystemCenter> centers = new();

        #endregion

        protected abstract ReactionSubsystemCenter GetCurrentSubsystemCenter();

        /// <inheritdoc />
        protected override void OnRenderStart()
        {
            using (_PRF_OnRenderStart.Auto())
            {
                var currentCenter = GetCurrentSubsystemCenter();
                cameraComponent.center = currentCenter;

                var tempCameraPosition = cameraComponent.GetCameraRootPosition();
                var pos = tempCameraPosition;
                pos += CameraOffset;

                cameraComponent.renderCamera.transform.position = pos;
                cameraComponent.renderCamera.transform.rotation =
                    Quaternion.LookRotation(CameraDirection, Vector3.forward);
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_OnRenderStart = new(_PRF_PFX + nameof(OnRenderStart));

        #endregion
    }
}
