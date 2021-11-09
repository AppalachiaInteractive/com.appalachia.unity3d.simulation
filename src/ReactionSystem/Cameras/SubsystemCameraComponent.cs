using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public struct SubsystemCameraComponent
    {
        private const string _PRF_PFX = nameof(SubsystemCameraComponent) + ".";

        private static readonly ProfilerMarker _PRF_CreateCamera =
            new(_PRF_PFX + nameof(CreateCamera));

        private static readonly ProfilerMarker _PRF_UpdateCamera =
            new(_PRF_PFX + nameof(UpdateCamera));

        private static readonly ProfilerMarker _PRF_GetCameraRootPosition =
            new(_PRF_PFX + nameof(GetCameraRootPosition));

        [SmartLabel]
        [HorizontalGroup("Data", .5f)]
        public ReactionSubsystemCenter center;

        [SmartLabel]
        [HorizontalGroup("Data", .5f)]
        public Camera renderCamera;

        [HideInInspector] public bool renderCameraPresent;
        [HideInInspector] public bool centerPresent;
        [HideInInspector] public bool hasReplacementShader;
        [HideInInspector] public bool shaderReplaced;

        public RenderTexture renderTexture => showRenderTexture ? renderCamera.targetTexture : null;
        public bool showRenderTexture => renderCamera != null;

        public static Camera CreateCamera(ReactionSubsystemCamera baseCamera, string cameraName)
        {
            using (_PRF_CreateCamera.Auto())
            {
                if (cameraName == null)
                {
                    AppaLog.Error("Must define camera name.");
                    return null;
                }

                var baseTransform = baseCamera.transform;

                var cameraTransform = baseTransform.Find(cameraName);

                Camera subsystemCamera;

                if (!cameraTransform)
                {
                    var cameraGO = new GameObject(cameraName);

                    cameraGO.transform.SetParent(baseTransform, false);
                    cameraGO.transform.position = baseCamera.cameraOffset;
                    cameraGO.transform.rotation =
                        Quaternion.LookRotation(baseCamera.cameraDirection);

                    var tempEditorCamera = cameraGO.AddComponent<Camera>();
                    tempEditorCamera.orthographicSize = 50f;
                    subsystemCamera = tempEditorCamera;
                }
                else
                {
                    subsystemCamera = cameraTransform.gameObject.GetComponent<Camera>();
                }

                UpdateCamera(baseCamera, subsystemCamera);

                return subsystemCamera;
            }
        }

        public static void UpdateCamera(ReactionSubsystemCamera baseCamera, Camera subsystemCamera)
        {
            using (_PRF_UpdateCamera.Auto())
            {
                if (!subsystemCamera)
                {
                    return;
                }

                if (baseCamera.hideCamera)
                {
                    subsystemCamera.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
                else
                {
                    subsystemCamera.gameObject.hideFlags = HideFlags.None;
                }

                subsystemCamera.farClipPlane = 10000;
                subsystemCamera.nearClipPlane = -10000;
                subsystemCamera.depth = -100;
                subsystemCamera.clearFlags = baseCamera.clearFlags;
                subsystemCamera.backgroundColor = baseCamera.backgroundColor;
                subsystemCamera.renderingPath = RenderingPath.Forward;
                subsystemCamera.useOcclusionCulling = true;
                subsystemCamera.orthographic = true;
                subsystemCamera.orthographicSize = baseCamera.orthographicSize;
                subsystemCamera.cullingMask = baseCamera.cullingMask;
                subsystemCamera.allowMSAA = false;
                subsystemCamera.allowHDR = false;
                subsystemCamera.stereoTargetEye = StereoTargetEyeMask.None;

                subsystemCamera.targetTexture = subsystemCamera.targetTexture.Recreate(
                    baseCamera.renderTextureQuality,
                    baseCamera.renderTextureFormat,
                    baseCamera.filterMode,
                    baseCamera.cullingMask
                );
            }
        }

        public Vector3 GetCameraRootPosition()
        {
            using (_PRF_GetCameraRootPosition.Auto())
            {
                if (center)
                {
                    return center.GetPosition();
                }

                return Vector3.zero;
            }
        }
    }
}
