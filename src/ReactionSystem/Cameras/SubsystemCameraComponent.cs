using System;
using Appalachia.CI.Constants;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Extensions;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public struct SubsystemCameraComponent
    {
        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        #region Fields and Autoproperties

        [HideInInspector] public bool centerPresent;
        [HideInInspector] public bool hasReplacementShader;

        [HideInInspector] public bool renderCameraPresent;
        [HideInInspector] public bool shaderReplaced;

        [SmartLabel]
        [HorizontalGroup("Data", .5f)]
        public Camera renderCamera;

        [SmartLabel]
        [HorizontalGroup("Data", .5f)]
        public ReactionSubsystemCenter center;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(SubsystemCameraComponent));
                }

                return _context;
            }
        }

        public bool HideRenderTexture => renderCamera == null;

        public RenderTexture RenderTexture => HideRenderTexture ? renderCamera.targetTexture : null;

        public static Camera CreateCamera(IReactionSubsystemCamera baseCamera, string cameraName)
        {
            using (_PRF_CreateCamera.Auto())
            {
                if (cameraName == null)
                {
                    Context.Log.Error("Must define camera name.");
                    return null;
                }

                var baseTransform = baseCamera.Transform;

                var cameraTransform = baseTransform.Find(cameraName);

                Camera subsystemCamera;

                if (!cameraTransform)
                {
                    var cameraGo = new GameObject(cameraName);

                    cameraGo.transform.SetParent(baseTransform, false);
                    cameraGo.transform.position = baseCamera.CameraOffset;
                    cameraGo.transform.rotation = Quaternion.LookRotation(baseCamera.CameraDirection);

                    var tempEditorCamera = cameraGo.AddComponent<Camera>();
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

        public static void UpdateCamera(IReactionSubsystemCamera baseCamera, Camera subsystemCamera)
        {
            using (_PRF_UpdateCamera.Auto())
            {
                if (!subsystemCamera)
                {
                    return;
                }

                if (baseCamera.HideCamera)
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
                subsystemCamera.clearFlags = baseCamera.ClearFlags;
                subsystemCamera.backgroundColor = baseCamera.BackgroundColor;
                subsystemCamera.renderingPath = RenderingPath.Forward;
                subsystemCamera.useOcclusionCulling = true;
                subsystemCamera.orthographic = true;
                subsystemCamera.orthographicSize = baseCamera.OrthographicSize;
                subsystemCamera.cullingMask = baseCamera.CullingMask;
                subsystemCamera.allowMSAA = false;
                subsystemCamera.allowHDR = false;
                subsystemCamera.stereoTargetEye = StereoTargetEyeMask.None;

                subsystemCamera.targetTexture = subsystemCamera.targetTexture.Recreate(
                    baseCamera.RenderTextureQuality,
                    baseCamera.RenderTextureFormat,
                    baseCamera.FilterMode,
                    baseCamera.CullingMask
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

        #region Profiling

        private const string _PRF_PFX = nameof(SubsystemCameraComponent) + ".";

        private static readonly ProfilerMarker _PRF_CreateCamera = new(_PRF_PFX + nameof(CreateCamera));

        private static readonly ProfilerMarker _PRF_UpdateCamera = new(_PRF_PFX + nameof(UpdateCamera));

        private static readonly ProfilerMarker _PRF_GetCameraRootPosition =
            new(_PRF_PFX + nameof(GetCameraRootPosition));

        #endregion
    }
}
