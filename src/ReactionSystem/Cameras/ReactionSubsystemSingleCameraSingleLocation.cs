using System;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class
        ReactionSubsystemSingleCameraSingleLocation : ReactionSubsystemSingleCamera
    {
        private const string _PRF_PFX = nameof(ReactionSubsystemSingleCameraSingleLocation) + ".";

        private static readonly ProfilerMarker _PRF_OnRenderStart =
            new(_PRF_PFX + nameof(OnRenderStart));

        protected override void OnRenderStart()
        {
            using (_PRF_OnRenderStart.Auto())
            {
                var tempCameraPosition = cameraComponent.GetCameraRootPosition();
                var pos = tempCameraPosition;
                pos += cameraOffset;

                var ct = cameraComponent.renderCamera.transform;

                ct.position = pos;
                ct.rotation = Quaternion.LookRotation(cameraDirection, Vector3.forward);
            }
        }

        /*public override void GetRenderingPosition(out Vector3 minimumPosition, out Vector3 size)
        {
            var c = cameraComponent.renderCamera;
            var ct = c.transform;
            
            c.f
        }*/
    }
}
