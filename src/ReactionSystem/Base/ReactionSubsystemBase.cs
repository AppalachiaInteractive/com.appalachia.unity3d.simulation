using System;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Core.Types.Enums;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.ReactionSystem.Base
{
    [ExecuteAlways]
    [Serializable]
    public abstract class ReactionSubsystemBase<T> : AppalachiaBehaviour<T>, IReactionSubsystem
        where T : ReactionSubsystemBase<T>
    {
        #region Fields and Autoproperties

        [FormerlySerializedAs("mainSystem")]
        [SerializeField]
        private ReactionSystem _mainSystem;

        [SerializeField] private int _groupIndex;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [FormerlySerializedAs("renderTextureFormat")]
        private RenderTextureFormat _renderTextureFormat;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [FormerlySerializedAs("renderTextureQuality")]
        private RenderQuality _renderTextureQuality = RenderQuality.High_1024;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [FormerlySerializedAs("filterMode")]
        private FilterMode _filterMode;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [ValueDropdown(nameof(depths))]
        [FormerlySerializedAs("depth")]
        private int _depth;

        private ValueDropdownList<int> depths = new()
        {
            0,
            8,
            16,
            24,
            32
        };

        private bool updateLoopInitialized;

        #endregion

        protected abstract bool HideRenderTexture { get; }

        protected abstract string SubsystemName { get; }

        protected ReactionSubsystemGroup Group => _mainSystem ? _mainSystem.groups[_groupIndex] : null;

        #region Event Functions

        protected void Update()
        {
            using (_PRF_Update.Auto())
            {
                try
                {
                    if (ShouldSkipUpdate)
                    {
                        return;
                    }

                    if (!updateLoopInitialized)
                    {
                        updateLoopInitialized = InitializeUpdateLoop();
                    }

                    if (!updateLoopInitialized)
                    {
                        return;
                    }

                    DoUpdateLoop();
                }
                catch (Exception ex)
                {
                    Context.Log.Error(ex);
                }
            }
        }

        #endregion

        protected abstract void DoUpdateLoop();

        protected abstract bool InitializeUpdateLoop();

        protected abstract void OnInitialization();

        protected abstract void TeardownSubsystem();

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            updateLoopInitialized = false;

            gameObject.name = SubsystemName;

            OnInitialization();
        }

        /// <inheritdoc />
        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            using (_PRF_WhenDisabled.Auto())
            {
                TeardownSubsystem();
                updateLoopInitialized = false;
            }
        }

        #region IReactionSubsystem Members

        public ReactionSystem MainSystem => _mainSystem;
        public RenderTextureFormat RenderTextureFormat => _renderTextureFormat;
        public RenderQuality RenderTextureQuality => _renderTextureQuality;
        public FilterMode FilterMode => _filterMode;
        public int Depth => _depth;

        [InlineProperty]
        [ShowInInspector]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        [FoldoutGroup("Preview")]
        [HideIf(nameof(HideRenderTexture))]
        public abstract RenderTexture RenderTexture { get; }

        public void InitializeSubsystem(ReactionSystem system, int groupIndex)
        {
            using (_PRF_InitializeSubsystem.Auto())
            {
                _mainSystem = system;
                _groupIndex = groupIndex;
            }
        }

        public void UpdateGroupIndex(int i)
        {
            _groupIndex = i;
        }

        #endregion

        #region Profiling

        private static readonly ProfilerMarker _PRF_InitializeSubsystem =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeSubsystem));

        #endregion
    }
}
