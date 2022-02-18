using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Appalachia.Utility.Reflection.Extensions;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.ReactionSystem
{
    [Serializable]
    public class ReactionSubsystemGroup : AppalachiaBase<ReactionSubsystemGroup>
    {
        #region Constants and Static Readonly

        private const string CAMERA_FOLDOUT_NAME = CAMERA_HIDE_NAME + "/Camera";
        private const string CAMERA_HIDE_NAME = nameof(lockSize);
        private const string SUBSYSTEM_GROUP_NAME = "Create New Subsystems";
        private const string SUBSYSTEM_HORIZONTAL_GROUP_NAME = SUBSYSTEM_GROUP_NAME + "/Create";
        private const string SUBSYSTEMS_NAME = "Subsystems";
        private const string TITLE_GROUP_DETAIL_NAME = TITLE_GROUP_NAME + "/Details";

        private const string TITLE_GROUP_NAME = "Group Profile";

        #endregion

        public ReactionSubsystemGroup()
        {
        }

        public ReactionSubsystemGroup(Object owner) : base(owner)
        {
        }

        #region Fields and Autoproperties

        [TitleGroup(TITLE_GROUP_NAME)]
        [OnValueChanged(nameof(InitializeRoot))]
        public string groupName;

        [HorizontalGroup(TITLE_GROUP_DETAIL_NAME)]
        [OnValueChanged(nameof(UpdateActive))]
        public bool enabled;

        [HorizontalGroup(TITLE_GROUP_DETAIL_NAME)]
        [ReadOnly]
        public int groupIndex;

        [TitleGroup(SUBSYSTEMS_NAME)]
        [ListDrawerSettings(HideAddButton = true)]
        public List<IReactionSubsystem> subsystems;

        [HideIfGroup(CAMERA_HIDE_NAME)]
        [FoldoutGroup(CAMERA_FOLDOUT_NAME)]
        [PropertyRange(1, 4096)]
        [OnValueChanged(nameof(InitializeCamera))]
        public int orthographicSize = 50;

        [HideInInspector] public bool lockSize;

        [SerializeField]
        [HideInInspector]
        private ReactionSystem _mainSystem;

        [SerializeField]
        [HideInInspector]
        private Transform _root;

        private ValueDropdownList<Type> _subsystemTypeList;

        [BoxGroup(SUBSYSTEM_GROUP_NAME)]
        [HorizontalGroup(SUBSYSTEM_HORIZONTAL_GROUP_NAME, .8f)]
        [ValueDropdown(nameof(SubsystemTypeList))]
        [LabelWidth(100)]
        [LabelText("Subsystem")]
        [ShowInInspector]
        public Type createSubsystemType;

        #endregion

        private bool CanCreateSubsystem => (_mainSystem != null) && (createSubsystemType != null);

        private ValueDropdownList<Type> SubsystemTypeList
        {
            get
            {
                _subsystemTypeList ??= new ValueDropdownList<Type>();

                if (_subsystemTypeList.Count == 0)
                {
                    var inheritors = typeof(ReactionSubsystemBase<>).GetAllConcreteInheritors();

                    for (var i = 0; i < inheritors.Count; i++)
                    {
                        var inheritor = inheritors[i];

                        _subsystemTypeList.Add(inheritor.GetReadableName(), inheritor);
                    }
                }

                return _subsystemTypeList;
            }
        }

        [BoxGroup(SUBSYSTEM_GROUP_NAME)]
        [HorizontalGroup(SUBSYSTEM_HORIZONTAL_GROUP_NAME, .1f)]
        [Button("Create")]
        [EnableIf(nameof(CanCreateSubsystem))]
        public void CreateNewSubsystem()
        {
            using (_PRF_CreateNewSubsystem.Auto())
            {
                var name = createSubsystemType.GetReadableName();

                var child = new GameObject(name);
                child.transform.SetParent(_root, false);

                var subsystem = child.AddComponent(createSubsystemType);

                subsystems.Add(subsystem as IReactionSubsystem);
            }
        }

        public void OnInitialize(ReactionSystem mainSystem, int groupIndex)
        {
            using (_PRF_OnInitialize.Auto())
            {
                _mainSystem = mainSystem;
                this.groupIndex = groupIndex;

                InitializeAll();
            }
        }

        private void InitializeAll()
        {
            using (_PRF_InitializeAll.Auto())
            {
                UpdateSubsystems(true, true, true, true, enabled);
            }
        }

        private void InitializeCamera()
        {
            using (_PRF_InitializeCamera.Auto())
            {
                UpdateSubsystems(true, false, true, true, enabled);
            }
        }

        private void InitializeRoot()
        {
            using (_PRF_InitializeRoot.Auto())
            {
                UpdateSubsystems(true, false, false, false, enabled);
            }
        }

        private void UpdateActive()
        {
            using (_PRF_UpdateActive.Auto())
            {
                UpdateSubsystems(true, false, false, true, enabled);
            }
        }

        private void UpdateSubsystems(
            bool updateRoot,
            bool initializeSubsystems,
            bool checkCamera,
            bool updateActive,
            bool doEnable)
        {
            using (_PRF_UpdateSubsystems.Auto())
            {
                if (_mainSystem == null)
                {
                    _mainSystem = Object.FindObjectOfType<ReactionSystem>();
                }

                if (_mainSystem != null)
                {
                    _mainSystem.groups ??= new List<ReactionSubsystemGroup>();

                    for (var i = 0; i < _mainSystem.groups.Count; i++)
                    {
                        var group = _mainSystem.groups[i];

                        group.groupIndex = i;

                        group.subsystems ??= new List<IReactionSubsystem>();

                        for (var j = 0; j < group.subsystems.Count; j++)
                        {
                            group.subsystems[j].UpdateGroupIndex(i);
                        }
                    }
                }

                if (updateRoot)
                {
                    if (_root == null)
                    {
                        var rgo = new GameObject(groupName);
                        _root = rgo.transform;
                        _root.transform.SetParent(_mainSystem.transform, false);
                    }

                    _root.name = groupName;
                }

                if (subsystems == null)
                {
                    subsystems = new List<IReactionSubsystem>();
                    return;
                }

                if (subsystems.Count == 0)
                {
                    var subs = _root.GetComponentsInChildren<IReactionSubsystem>();

                    foreach (var sub in subs)
                    {
                        subsystems.Add(sub);
                    }
                }

                for (var index = subsystems.Count - 1; index >= 0; index--)
                {
                    var subsystem = subsystems[index];

                    if (subsystem == null)
                    {
                        subsystems.RemoveAt(index);
                        continue;
                    }

                    if (checkCamera && subsystem is IReactionSubsystemCamera cam)
                    {
                        cam.OrthographicSize = orthographicSize;
                    }

                    if (updateActive)
                    {
                        subsystem.GameObject.SetActive(doEnable);
                    }

                    if (initializeSubsystems)
                    {
                        subsystem.InitializeSubsystem(_mainSystem, groupIndex);
                    }
                }
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_CreateNewSubsystem =
            new(_PRF_PFX + nameof(CreateNewSubsystem));

        private static readonly ProfilerMarker _PRF_InitializeAll = new(_PRF_PFX + nameof(InitializeAll));

        private static readonly ProfilerMarker _PRF_InitializeCamera =
            new(_PRF_PFX + nameof(InitializeCamera));

        private static readonly ProfilerMarker _PRF_InitializeRoot = new(_PRF_PFX + nameof(InitializeRoot));

        private static readonly ProfilerMarker _PRF_OnInitialize =
            new ProfilerMarker(_PRF_PFX + nameof(OnInitialize));

        private static readonly ProfilerMarker _PRF_UpdateActive = new(_PRF_PFX + nameof(UpdateActive));

        private static readonly ProfilerMarker _PRF_UpdateSubsystems =
            new(_PRF_PFX + nameof(UpdateSubsystems));

        #endregion
    }
}
