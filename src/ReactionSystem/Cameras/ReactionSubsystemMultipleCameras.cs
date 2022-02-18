using System;
using System.Collections.Generic;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemMultipleCameras<T> : ReactionSubsystemCamera<T>
        where T : ReactionSubsystemMultipleCameras<T>
    {
        #region Fields and Autoproperties

        [ListDrawerSettings]
        [OnValueChanged(nameof(Initialize), true)]
        [SerializeField]
        public List<SubsystemCameraComponent> cameraComponents = new();

        [FoldoutGroup("Preview")]
        [PropertyOrder(-100)]
        [PropertyRange(0, nameof(RenderTextureMax))]
        public int selectedRenderTexture;

        #endregion

        /// <inheritdoc />
        public override RenderTexture RenderTexture => cameraComponents[selectedRenderTexture].RenderTexture;

        /// <inheritdoc />
        protected override bool ShowRenderTexture =>
            cameraComponents is { Count: > 0 } &&
            (selectedRenderTexture > 0) &&
            (selectedRenderTexture < cameraComponents.Count) &&
            cameraComponents[selectedRenderTexture].ShowRenderTexture;

        private int RenderTextureMax => cameraComponents.Count - 1;

        protected abstract void OnBeforeInitialization();

        protected abstract void OnInitializationComplete(SubsystemCameraComponent cam);

        protected abstract void OnInitializationStart(SubsystemCameraComponent cam);

        protected abstract void OnRenderComplete(SubsystemCameraComponent camera);

        protected abstract void OnRenderStart(SubsystemCameraComponent camera);

        protected abstract void OnUpdateLoopStart();

        protected abstract bool ShouldRenderCamera(
            SubsystemCameraComponent cam,
            int cameraIndex,
            int totalCameras);

        /// <inheritdoc />
        protected override void DoUpdateLoop()
        {
            using (_PRF_DoUpdateLoop.Auto())
            {
                OnUpdateLoopStart();

                for (var i = 0; i < cameraComponents.Count; i++)
                {
                    var cameraComponent = cameraComponents[i];

                    if (!ShouldRenderCamera(cameraComponent, i, cameraComponents.Count))
                    {
                        continue;
                    }

                    if (AutomaticRender)
                    {
                        OnRenderStart(cameraComponent);

                        cameraComponent.renderCamera.enabled = true;

                        if (cameraComponent.hasReplacementShader && !cameraComponent.shaderReplaced)
                        {
                            cameraComponent.renderCamera.SetReplacementShader(ReplacementShader, null);
                            cameraComponent.shaderReplaced = true;
                        }

                        OnRenderComplete(cameraComponent);
                    }
                    else
                    {
                        cameraComponent.renderCamera.enabled = false;

                        if (!IsManualRenderingRequired(cameraComponent))
                        {
                            return;
                        }

                        OnRenderStart(cameraComponent);

                        if (cameraComponent.hasReplacementShader)
                        {
                            cameraComponent.renderCamera.RenderWithShader(
                                ReplacementShader,
                                ReplacementShaderTag
                            );
                        }
                        else
                        {
                            cameraComponent.renderCamera.Render();
                        }

                        OnRenderComplete(cameraComponent);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override bool InitializeUpdateLoop()
        {
            using (_PRF_InitializeUpdateLoop.Auto())
            {
                var successful = false;

                for (var i = 0; i < cameraComponents.Count; i++)
                {
                    var cameraComponent = cameraComponents[i];

                    cameraComponent.centerPresent = cameraComponent.center != null;
                    cameraComponent.renderCameraPresent = cameraComponent.renderCamera != null;

                    if (cameraComponent.centerPresent && cameraComponent.renderCameraPresent)
                    {
                        successful = true;

                        cameraComponent.hasReplacementShader = ReplacementShader != null;
                    }
                }

                return successful;
            }
        }

        /// <inheritdoc />
        protected override void OnInitialization()
        {
            using (_PRF_OnInitialization.Auto())
            {
                OnBeforeInitialization();

                for (var i = 0; i < cameraComponents.Count; i++)
                {
                    var cameraComponent = cameraComponents[i];

                    EnsureSubsystemCenterIsPrepared(cameraComponent, this);

                    if (cameraComponent.center == null)
                    {
                        Context.Log.Error("Must assign system center.");

                        return;
                    }

                    if (string.IsNullOrWhiteSpace(SubsystemName))
                    {
                        Context.Log.Error("Must define system name.");
                    }

                    var objName = ZString.Format("{0}_{1}", SubsystemName, i);

                    gameObject.name = objName;

                    if (!cameraComponent.renderCamera)
                    {
                        cameraComponent.renderCamera = SubsystemCameraComponent.CreateCamera(this, objName);
                    }

                    if (cameraComponent.renderCamera)
                    {
                        cameraComponent.renderCamera.enabled = true;
                    }

                    OnInitializationStart(cameraComponent);

                    SubsystemCameraComponent.UpdateCamera(this, cameraComponent.renderCamera);

                    OnInitializationComplete(cameraComponent);
                }
            }
        }

        /// <inheritdoc />
        protected override void TeardownSubsystem()
        {
            using (_PRF_TeardownSubsystem.Auto())
            {
                for (var i = 0; i < cameraComponents.Count; i++)
                {
                    var cameraComponent = cameraComponents[i];

                    if (cameraComponent.renderCamera)
                    {
                        cameraComponent.renderCamera.enabled = false;

                        var rt = cameraComponent.renderCamera.targetTexture;
                        cameraComponent.renderCamera.targetTexture = null;

                        if (rt != null)
                        {
                            DestroyImmediate(rt);
                        }
                    }
                }
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_DoUpdateLoop = new(_PRF_PFX + nameof(DoUpdateLoop));

        private static readonly ProfilerMarker _PRF_InitializeUpdateLoop =
            new(_PRF_PFX + nameof(InitializeUpdateLoop));

        private static readonly ProfilerMarker _PRF_OnInitialization =
            new(_PRF_PFX + nameof(OnInitialization));

        private static readonly ProfilerMarker _PRF_TeardownSubsystem =
            new(_PRF_PFX + nameof(TeardownSubsystem));

        #endregion
    }
}
