using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemSingleCamera<T> : ReactionSubsystemCamera<T>
        where T : ReactionSubsystemSingleCamera<T>
    {
        #region Fields and Autoproperties

        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public SubsystemCameraComponent cameraComponent;

        #endregion

        /// <inheritdoc />
        public override RenderTexture RenderTexture =>
            ShowRenderTexture ? cameraComponent.renderCamera.targetTexture : null;

        /// <inheritdoc />
        protected override bool ShowRenderTexture => cameraComponent.ShowRenderTexture;

        protected abstract void OnBeforeInitialization();

        protected abstract void OnInitializationComplete();

        protected abstract void OnInitializationStart();

        protected abstract void OnRenderComplete();

        protected abstract void OnRenderStart();

        /// <inheritdoc />
        protected override void DoUpdateLoop()
        {
            using (_PRF_DoUpdateLoop.Auto())
            {
                if (AutomaticRender)
                {
                    OnRenderStart();

                    cameraComponent.renderCamera.enabled = true;

                    if (cameraComponent.hasReplacementShader && !cameraComponent.shaderReplaced)
                    {
                        cameraComponent.renderCamera.SetReplacementShader(ReplacementShader, null);
                        cameraComponent.shaderReplaced = true;
                    }

                    OnRenderComplete();
                }
                else
                {
                    cameraComponent.renderCamera.enabled = false;

                    if (!IsManualRenderingRequired(cameraComponent))
                    {
                        return;
                    }

                    OnRenderStart();

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

                    OnRenderComplete();
                }
            }
        }

        /// <inheritdoc />
        protected override bool InitializeUpdateLoop()
        {
            using (_PRF_InitializeUpdateLoop.Auto())
            {
                var successful = false;

                cameraComponent.centerPresent = cameraComponent.center != null;
                cameraComponent.renderCameraPresent = cameraComponent.renderCamera != null;

                if (cameraComponent.centerPresent && cameraComponent.renderCameraPresent)
                {
                    successful = true;

                    cameraComponent.hasReplacementShader = ReplacementShader != null;
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

                EnsureSubsystemCenterIsPrepared(cameraComponent, this as T);

                if (cameraComponent.center == null)
                {
                    Context.Log.Error("Must assign system center.");

                    return;
                }

                if (string.IsNullOrWhiteSpace(SubsystemName))
                {
                    Context.Log.Error("Must define system name.");
                }

                gameObject.name = SubsystemName;

                if (!cameraComponent.renderCamera)
                {
                    cameraComponent.renderCamera = SubsystemCameraComponent.CreateCamera(this, SubsystemName);
                }

                if (cameraComponent.renderCamera)
                {
                    cameraComponent.renderCamera.enabled = true;
                }

                OnInitializationStart();

                SubsystemCameraComponent.UpdateCamera(this, cameraComponent.renderCamera);

                OnInitializationComplete();
            }
        }

        /// <inheritdoc />
        protected override void TeardownSubsystem()
        {
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

        #region Profiling

        private static readonly ProfilerMarker _PRF_DoUpdateLoop = new(_PRF_PFX + nameof(DoUpdateLoop));

        private static readonly ProfilerMarker _PRF_InitializeUpdateLoop =
            new(_PRF_PFX + nameof(InitializeUpdateLoop));

        private static readonly ProfilerMarker _PRF_OnInitialization =
            new(_PRF_PFX + nameof(OnInitialization));

        #endregion
    }
}
