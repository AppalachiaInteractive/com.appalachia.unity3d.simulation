using System;
using Appalachia.Core.Behaviours;
using Appalachia.Core.Types.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Base
{
    [ExecuteAlways]
    [Serializable]
    public abstract class ReactionSubsystemBase : AppalachiaBehaviour
    {
        private const string _PRF_PFX = nameof(ReactionSubsystemBase) + ".";

        public ReactionSystem mainSystem;

        [SerializeField] private int _groupIndex;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(Initialize))]
        public RenderTextureFormat renderTextureFormat;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(Initialize))]
        public RenderQuality renderTextureQuality = RenderQuality.High_1024;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(Initialize))]
        public FilterMode filterMode;

        [SerializeField]
        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(Initialize))]
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

        protected abstract string SubsystemName { get; }

        protected ReactionSubsystemGroup Group =>
            mainSystem ? mainSystem.groups[_groupIndex] : null;

        [InlineProperty]
        [ShowInInspector]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        [FoldoutGroup("Preview")]
        [ShowIf(nameof(showRenderTexture))]
        public abstract RenderTexture renderTexture { get; }

        protected abstract bool showRenderTexture { get; }

        //public abstract void GetRenderingPosition(out Vector3 minimumPosition, out Vector3 size);

        private void Awake()
        {
            updateLoopInitialized = false;
            Initialize();
        }

        private void Update()
        {
            try
            {
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
                Debug.LogError(ex);
            }
        }

        private void OnEnable()
        {
            updateLoopInitialized = false;
            Initialize();
        }

        private void OnDisable()
        {
            TeardownSubsystem();
            updateLoopInitialized = false;
        }

        protected abstract void TeardownSubsystem();

        protected abstract void OnInitialization();

        public void InitializeSubsystem(ReactionSystem system, int groupIndex)
        {
            mainSystem = system;
            _groupIndex = groupIndex;

            Initialize();
        }

        [Button]
        public void Initialize()
        {
            gameObject.name = SubsystemName;

            OnInitialization();
        }

        public void UpdateGroupIndex(int i)
        {
            _groupIndex = i;
        }

        protected abstract bool InitializeUpdateLoop();

        protected abstract void DoUpdateLoop();
    }
}
