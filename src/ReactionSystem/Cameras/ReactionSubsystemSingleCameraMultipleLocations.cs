using System;
using System.Collections.Generic;
using Appalachia.Simulation.ReactionSystem.Base;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemSingleCameraMultipleLocations : ReactionSubsystemSingleCamera
    {
        private const string _PRF_PFX =
            nameof(ReactionSubsystemSingleCameraMultipleLocations) + ".";

        private static readonly ProfilerMarker _PRF_OnRenderStart =
            new(_PRF_PFX + nameof(OnRenderStart));

        [OnValueChanged(nameof(InitializeSynchronous))]
        public List<ReactionSubsystemCenter> centers = new();

        protected override void OnRenderStart()
        {
            using (_PRF_OnRenderStart.Auto())
            {
                var currentCenter = GetCurrentSubsystemCenter();
                cameraComponent.center = currentCenter;

                var tempCameraPosition = cameraComponent.GetCameraRootPosition();
                var pos = tempCameraPosition;
                pos += cameraOffset;

                cameraComponent.renderCamera.transform.position = pos;
                cameraComponent.renderCamera.transform.rotation =
                    Quaternion.LookRotation(cameraDirection, Vector3.forward);
            }
        }

        protected abstract ReactionSubsystemCenter GetCurrentSubsystemCenter();
    }
}
