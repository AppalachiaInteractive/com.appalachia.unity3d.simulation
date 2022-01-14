using System;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Core.Types.Enums;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Base
{
    [ExecuteAlways]
    [Serializable]
    public abstract class ReactionSubsystemBase : AppalachiaBehaviour
    {
        #region Fields and Autoproperties

        public ReactionSystem mainSystem;

        [SerializeField] private int _groupIndex;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public RenderTextureFormat renderTextureFormat;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public RenderQuality renderTextureQuality = RenderQuality.High_1024;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public FilterMode filterMode;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSynchronous))]
        [ValueDropdown(nameof(depths))]
        public int depth;

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

        [InlineProperty]
        [ShowInInspector]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        [FoldoutGroup("Preview")]
        [ShowIf(nameof(showRenderTexture))]
        public abstract RenderTexture renderTexture { get; }

        protected abstract bool showRenderTexture { get; }

        protected abstract string SubsystemName { get; }

        protected ReactionSubsystemGroup Group => mainSystem ? mainSystem.groups[_groupIndex] : null;

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

        public void InitializeSubsystem(ReactionSystem system, int groupIndex)
        {
            using (_PRF_InitializeSubsystem.Auto())
            {
                mainSystem = system;
                _groupIndex = groupIndex;
            }
        }

        public void UpdateGroupIndex(int i)
        {
            _groupIndex = i;
        }

        protected abstract void DoUpdateLoop();

        protected abstract bool InitializeUpdateLoop();

        protected abstract void OnInitialization();

        protected abstract void TeardownSubsystem();

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            updateLoopInitialized = false;

            gameObject.name = SubsystemName;

            OnInitialization();
        }

        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            using (_PRF_WhenDisabled.Auto())
            {
                TeardownSubsystem();
                updateLoopInitialized = false;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(ReactionSubsystemBase) + ".";

        private static readonly ProfilerMarker _PRF_InitializeSubsystem =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeSubsystem));

        private static readonly ProfilerMarker _PRF_WhenDisabled =
            new ProfilerMarker(_PRF_PFX + nameof(WhenDisabled));

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));

        #endregion

        //public abstract void GetRenderingPosition(out Vector3 minimumPosition, out Vector3 size);
    }
}
