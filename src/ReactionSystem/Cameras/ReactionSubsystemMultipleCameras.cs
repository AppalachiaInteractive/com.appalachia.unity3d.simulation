using System;
using System.Collections.Generic;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemMultipleCameras : ReactionSubsystemCamera
    {
        private const string _PRF_PFX = nameof(ReactionSubsystemMultipleCameras) + ".";

        private static readonly ProfilerMarker _PRF_OnInitialization =
            new(_PRF_PFX + nameof(OnInitialization));

        private static readonly ProfilerMarker _PRF_InitializeUpdateLoop =
            new(_PRF_PFX + nameof(InitializeUpdateLoop));

        private static readonly ProfilerMarker _PRF_DoUpdateLoop =
            new(_PRF_PFX + nameof(DoUpdateLoop));

        private static readonly ProfilerMarker _PRF_TeardownSubsystem =
            new(_PRF_PFX + nameof(TeardownSubsystem));

        [ListDrawerSettings]
        [OnValueChanged(nameof(Initialize), true)]
        [SerializeField]
        public List<SubsystemCameraComponent> cameraComponents = new();

        [FoldoutGroup("Preview")]
        [PropertyOrder(-100)]
        [PropertyRange(0, nameof(_renderTextureMax))]
        public int selectedRenderTexture;

        private int _renderTextureMax => cameraComponents.Count - 1;

        protected override bool showRenderTexture =>
            (cameraComponents != null) &&
            (cameraComponents.Count > 0) &&
            (selectedRenderTexture > 0) &&
            (selectedRenderTexture < cameraComponents.Count) &&
            cameraComponents[selectedRenderTexture].showRenderTexture;

        public override RenderTexture renderTexture =>
            cameraComponents[selectedRenderTexture].renderTexture;

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
                        AppaLog.Error("Must assign system center.");

                        return;
                    }

                    if (string.IsNullOrWhiteSpace(SubsystemName))
                    {
                        AppaLog.Error("Must define system name.");
                    }

                    var objName = $"{SubsystemName}_{i}";

                    gameObject.name = objName;

                    if (!cameraComponent.renderCamera)
                    {
                        cameraComponent.renderCamera =
                            SubsystemCameraComponent.CreateCamera(this, objName);
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

                        cameraComponent.hasReplacementShader = replacementShader != null;
                    }
                }

                return successful;
            }
        }

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
                            cameraComponent.renderCamera.SetReplacementShader(
                                replacementShader,
                                null
                            );
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
                                replacementShader,
                                replacementShaderTag
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

        protected abstract void OnBeforeInitialization();

        protected abstract void OnInitializationStart(SubsystemCameraComponent cam);

        protected abstract void OnInitializationComplete(SubsystemCameraComponent cam);

        protected abstract void OnUpdateLoopStart();

        protected abstract bool ShouldRenderCamera(
            SubsystemCameraComponent cam,
            int cameraIndex,
            int totalCameras);

        protected abstract void OnRenderStart(SubsystemCameraComponent camera);

        protected abstract void OnRenderComplete(SubsystemCameraComponent camera);

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
    }
}
