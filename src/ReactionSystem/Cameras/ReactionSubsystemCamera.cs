using System;
using System.Linq;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public abstract class ReactionSubsystemCamera<T> : ReactionSubsystemBase<T>, IReactionSubsystemCamera
        where T : ReactionSubsystemCamera<T>
    {
        #region Fields and Autoproperties

        [FormerlySerializedAs("replacementShader")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private Shader _replacementShader;

        [FormerlySerializedAs("replacementShaderTag")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private string _replacementShaderTag;

        [FormerlySerializedAs("cullingMask")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private LayerMask _cullingMask;

        [FormerlySerializedAs("cameraOffset")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private Vector3 _cameraOffset = new(0f, -1000f, 0f);

        [FormerlySerializedAs("cameraDirection")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private Vector3 _cameraDirection = Vector3.up;

        [FormerlySerializedAs("backgroundColor")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private Color _backgroundColor;

        [FormerlySerializedAs("clearFlags")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private CameraClearFlags _clearFlags;

        [FormerlySerializedAs("orthographicSize")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [PropertyRange(1, 4096)]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [ReadOnly]
        private int _orthographicSize = 50;

        [FormerlySerializedAs("hideCamera")]
        [SerializeField]
        [FoldoutGroup("Camera")]
        [SmartLabel]
        [OnValueChanged(nameof(InitializeSynchronous))]
        private bool _hideCamera = true;

        #endregion

        protected static void EnsureSubsystemCenterIsPrepared(
            SubsystemCameraComponent cam,
            IReactionSubsystemCamera subsystem)
        {
            using (_PRF_EnsureSubsystemCenterIsPrepared.Auto())
            {
                if (cam.center != null)
                {
                    cam.center.ValidateSubsystems();

                    cam.center.EnsureSubsystemIsAdded(subsystem);
                }
                else
                {
                    var centers = FindObjectsOfType<ReactionSubsystemCenter>();

                    for (var i = 0; i < centers.Length; i++)
                    {
                        var c = centers[i];

                        c.ValidateSubsystems();

                        if (c.Subsystems.Contains(subsystem))
                        {
                            cam.center = c;
                        }
                    }
                }
            }
        }

        #region IReactionSubsystemCamera Members

        public Shader ReplacementShader => _replacementShader;
        public string ReplacementShaderTag => _replacementShaderTag;

        public LayerMask CullingMask
        {
            get => _cullingMask;
            set => _cullingMask = value;
        }

        public Vector3 CameraOffset => _cameraOffset;
        public Vector3 CameraDirection => _cameraDirection;
        public Color BackgroundColor => _backgroundColor;
        public CameraClearFlags ClearFlags => _clearFlags;

        public int OrthographicSize
        {
            get => _orthographicSize;
            set => _orthographicSize = value;
        }

        public bool HideCamera => _hideCamera;

        public abstract bool AutomaticRender { get; }

        // ReSharper disable once UnusedParameter.Global
        public abstract bool IsManualRenderingRequired(SubsystemCameraComponent cam);

        #endregion

        #region Profiling

        private static readonly ProfilerMarker _PRF_EnsureSubsystemCenterIsPrepared =
            new(_PRF_PFX + nameof(EnsureSubsystemCenterIsPrepared));

        #endregion
    }
}
