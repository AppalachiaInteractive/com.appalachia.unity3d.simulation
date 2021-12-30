using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Simulation.ReactionSystem.Cameras;
using Appalachia.Utility.Reflection.Extensions;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Appalachia.Simulation.ReactionSystem
{
    [Serializable]
    public class ReactionSubsystemGroup : AppalachiaSimpleBase
    {
        #region Constants and Static Readonly

        private const string cameraFoldoutName = cameraHideName + "/Camera";
        private const string cameraHideName = nameof(lockSize);
        private const string subsystemGroupName = "Create New Subsystems";
        private const string subsystemHorizontalGroupName = subsystemGroupName + "/Create";
        private const string subsystemsName = "Subsystems";
        private const string titleGroupDetailName = titleGroupName + "/Details";

        private const string titleGroupName = "Group Profile";

        #endregion

        #region Fields and Autoproperties

        [TitleGroup(titleGroupName)]
        [OnValueChanged(nameof(InitializeRoot))]
        public string groupName;

        [HorizontalGroup(titleGroupDetailName)]
        [OnValueChanged(nameof(UpdateActive))]
        public bool enabled;

        [HorizontalGroup(titleGroupDetailName)]
        [ReadOnly]
        public int groupIndex;

        [TitleGroup(subsystemsName)]
        [ListDrawerSettings(HideAddButton = true)]
        public List<ReactionSubsystemBase> subsystems;

        [HideIfGroup(cameraHideName)]
        [FoldoutGroup(cameraFoldoutName)]
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

        [BoxGroup(subsystemGroupName)]
        [HorizontalGroup(subsystemHorizontalGroupName, .8f)]
        [ValueDropdown(nameof(SubsystemTypeList))]
        [LabelWidth(100)]
        [LabelText("Subsystem")]
        [ShowInInspector]
        public Type createSubsystemType;

        #endregion

        private bool _canCreateSubsystem => (_mainSystem != null) && (createSubsystemType != null);

        private ValueDropdownList<Type> SubsystemTypeList
        {
            get
            {
                if (_subsystemTypeList == null)
                {
                    _subsystemTypeList = new ValueDropdownList<Type>();
                }

                if (_subsystemTypeList.Count == 0)
                {
                    var inheritors = typeof(ReactionSubsystemBase).GetAllConcreteInheritors();

                    for (var i = 0; i < inheritors.Count; i++)
                    {
                        var inheritor = inheritors[i];

                        _subsystemTypeList.Add(inheritor.GetReadableName(), inheritor);
                    }
                }

                return _subsystemTypeList;
            }
        }

        [BoxGroup(subsystemGroupName)]
        [HorizontalGroup(subsystemHorizontalGroupName, .1f)]
        [Button("Create")]
        [EnableIf(nameof(_canCreateSubsystem))]
        public void CreateNewSubsystem()
        {
            using (_PRF_CreateNewSubsystem.Auto())
            {
                var name = createSubsystemType.GetReadableName();

                var child = new GameObject(name);
                child.transform.SetParent(_root, false);

                var subsystem = child.AddComponent(createSubsystemType);

                subsystems.Add(subsystem as ReactionSubsystemBase);
            }
        }

        public void Initialize(ReactionSystem mainSystem, int groupIndex)
        {
            using (_PRF_Initialize.Auto())
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
                    if (_mainSystem.groups == null)
                    {
                        _mainSystem.groups = new List<ReactionSubsystemGroup>();
                    }

                    for (var i = 0; i < _mainSystem.groups.Count; i++)
                    {
                        var group = _mainSystem.groups[i];

                        group.groupIndex = i;

                        if (group.subsystems == null)
                        {
                            group.subsystems = new List<ReactionSubsystemBase>();
                        }

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
                    subsystems = new List<ReactionSubsystemBase>();
                    return;
                }

                if (subsystems.Count == 0)
                {
                    var subs = _root.GetComponentsInChildren<ReactionSubsystemBase>();

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

                    if (checkCamera && subsystem is ReactionSubsystemCamera cam)
                    {
                        cam.orthographicSize = orthographicSize;
                    }

                    if (updateActive)
                    {
                        subsystem.gameObject.SetActive(doEnable);
                    }

                    if (initializeSubsystems)
                    {
                        subsystem.InitializeSubsystem(_mainSystem, groupIndex);
                    }
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(ReactionSubsystemGroup) + ".";

        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_InitializeAll = new(_PRF_PFX + nameof(InitializeAll));

        private static readonly ProfilerMarker _PRF_InitializeRoot = new(_PRF_PFX + nameof(InitializeRoot));

        private static readonly ProfilerMarker _PRF_InitializeCamera =
            new(_PRF_PFX + nameof(InitializeCamera));

        private static readonly ProfilerMarker _PRF_UpdateActive = new(_PRF_PFX + nameof(UpdateActive));

        private static readonly ProfilerMarker _PRF_UpdateSubsystems =
            new(_PRF_PFX + nameof(UpdateSubsystems));

        private static readonly ProfilerMarker _PRF_CreateNewSubsystem =
            new(_PRF_PFX + nameof(CreateNewSubsystem));

        #endregion
    }
}
