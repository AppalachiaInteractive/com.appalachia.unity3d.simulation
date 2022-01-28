using System;
using System.Diagnostics;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core.Serialization;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.UI.Utilities;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.Trees
{
    [CallStaticConstructorInEditor]
    public abstract class TSEDataContainer : ResponsiveAppalachiaObject
    {
        public enum DataState
        {
            Normal = 0,
            Dirty = 10,
            PendingSave = 20
        }

        static TSEDataContainer()
        {
            RegisterInstanceCallbacks.For<TSEDataContainer>()
                                     .When.Object<DefaultShaderResource>()
                                     .IsAvailableThen(i => _defaultShaderResource = i);
        }

        #region Static Fields and Autoproperties

        protected static DefaultShaderResource _defaultShaderResource;

        #endregion

        #region Fields and Autoproperties

        [HideInInspector] public bool graphDirty;

        [HideInInspector] public bool initialized;

        [HideInInspector] public BuildState buildState = BuildState.Default;

        [HideInInspector] public DataState dataState;

        [FormerlySerializedAs("individualIDs")]
        [HideInInspector]
        public IDIncrementer primaryIDs = new IDIncrementer(false);

        [PropertyOrder(-100)]
        [HideIf(nameof(initialized))]
        [InfoBox("Initialize it!")]
        [HideLabel]
        [Title("Initialization", "Initialize the tree or branch before proceeding.")]
        public InitializationSettings initializationSettings;

        [HideInInspector] public TreeAssetSubfolders subfolders;

        [HideInInspector] public TreePrefabCollection hierarchyPrefabs;

        private BuildProgressTracker _progressTracker;

        #endregion

        public abstract ResponsiveSettingsType settingsType { get; }

        public bool canInitialize =>
            !initialized &&
            (initializationSettings != null) &&
            !string.IsNullOrWhiteSpace(initializationSettings.name) &&
            (initializationSettings.name.Length > 2) &&
            (!initializationSettings.convertTreeData || (initializationSettings.original != null));

        public BuildProgressTracker progressTracker
        {
            get
            {
                if (_progressTracker == null)
                {
                    _progressTracker = new BuildProgressTracker();
                }

                return _progressTracker;
            }
        }

        public abstract void BuildDefault();
        public abstract void BuildForceFull();

        public abstract void BuildFull();

        public abstract void CopyHierarchiesFrom(TSEDataContainer tsd);

        public abstract void CopySettingsFrom(TSEDataContainer tsd);

        public abstract NameBasis GetNameBasis();

        public abstract void RebuildStructures();

        public abstract void SetDirtyStates();

        public new abstract void SettingsChanged(SettingsUpdateTarget target);

        [DebuggerStepThrough]
        public override string ToString()
        {
            var basis = GetNameBasis();

            if (basis == null)
            {
                return name;
            }

            return GetNameBasis().friendlyName;
        }

        public override void UpdateSettingsType(ResponsiveSettingsType t)
        {
            using (_PRF_UpdateSettingsType.Auto())
            {
                this.HandleResponsiveUpdate(t);
            }
        }

        public void Save(bool generateImpostors = false)
        {
            using (_PRF_Save.Auto())
            {
                using (var progress = new ProgressDisplay(3))
                {
                    try
                    {
                        progress.Do(SetDirtyStates,                          "Setting states...");
                        progress.Do(() => AssetDatabaseManager.SaveAssets(), "Saving assets...");
                        progress.Do(() => SaveAllAssets(generateImpostors),  "Persisting changes...");
                        dataState = DataState.Normal;
                    }
                    catch (Exception ex)
                    {
                        Context.Log.Error(ex);
                        dataState = DataState.PendingSave;
                    }
                }
            }
        }

        protected abstract void SaveAllAssets(bool saveImpostors);

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            UpdateSettingsType(settingsType);
        }

        #region Profiling

        private const string _PRF_PFX = nameof(TSEDataContainer) + ".";

        private static readonly ProfilerMarker _PRF_Save = new ProfilerMarker(_PRF_PFX + nameof(Save));

        private static readonly ProfilerMarker
            _PRF_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_UpdateSettingsType =
            new ProfilerMarker(_PRF_PFX + nameof(UpdateSettingsType));

        #endregion
    }
}
