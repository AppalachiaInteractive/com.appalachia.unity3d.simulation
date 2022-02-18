using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Simulation.Trees.Build;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.RequestManagers;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Interfaces;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.Settings.Log;
using Appalachia.Utility.Async;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Appalachia.Simulation.Trees
{
    [Serializable]
    public sealed class LogDataContainer : TSEDataContainer, ILogDataProvider
    {
        #region Fields and Autoproperties

        [HideInInspector] public bool _drawGizmos;

        [HideInInspector] public List<LogInstance> logInstances;

        [HideInInspector] public TreeLog log;

        [HideInInspector] public Material material;

        [ShowIf(nameof(initialized))]
        [PropertyOrder(100)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        public LogSettings settings;

        [HideInInspector] public LogBuildRequest buildRequest;

        #endregion

        /// <inheritdoc />
        public override ResponsiveSettingsType settingsType => ResponsiveSettingsType.Log;

        public BuildRequestLevel requestLevel
        {
            get
            {
                if (buildRequest == null)
                {
                    buildRequest = new LogBuildRequest();
                }

                var rl = buildRequest.requestLevel;

                foreach (var logInstance in logInstances)
                {
                    if (logInstance.active || (buildState > BuildState.Full))
                    {
                        rl = rl.Max(logInstance.GetRequestLevel(buildState));

                        if (rl == BuildRequestLevel.InitialPass)
                        {
                            return rl;
                        }
                    }
                }

                return rl;
            }
        }

        /// <inheritdoc />
        public override void BuildDefault()
        {
            LogBuildRequestManager.Default();
        }

        /// <inheritdoc />
        public override void BuildForceFull()
        {
            LogBuildRequestManager.ForceFull();
        }

        /// <inheritdoc />
        public override void BuildFull()
        {
            LogBuildRequestManager.Full();
        }

        /// <inheritdoc />
        public override void CopyHierarchiesFrom(TSEDataContainer tsd)
        {
            if (tsd is LogDataContainer dc)
            {
                dc.log.hierarchies.CopyHierarchiesTo(log.hierarchies);
            }
        }

        /// <inheritdoc />
        public override void CopySettingsFrom(TSEDataContainer tsd)
        {
            if (tsd is LogDataContainer dc)
            {
                dc.settings.CopySettingsTo(settings);
            }
        }

        /// <inheritdoc />
        public override NameBasis GetNameBasis()
        {
            if (log != null)
            {
                return log.nameBasis;
            }

            return null;
        }

        /// <inheritdoc />
        public override void RebuildStructures()
        {
            if (material == null)
            {
                material = new Material(_defaultShaderResource.logShader)
                {
                    name = ZString.Format("{0}", log.nameBasis.safeName)
                };
            }

            for (var i = logInstances.Count - 1; i >= 0; i--)
            {
                if ((logInstances[i].asset == null) && (logInstances[i].logID == 0))
                {
                    logInstances.RemoveAt(i);
                }
            }

            log.hierarchies.Rebuild();

            settings.lod.SetIndices();

            foreach (var instance in logInstances)
            {
                instance.Refresh(settings.lod);
            }
        }

        /// <inheritdoc />
        public override void SetDirtyStates()
        {
            MarkAsModified();
            settings.MarkAsModified();
            log.MarkAsModified();
            subfolders.MarkAsModified();
            if (material != null)
            {
                material.MarkAsModified();
            }

            foreach (var instance in logInstances)
            {
                instance.MarkAsModified();
                instance.asset.MarkAsModified();

                if (instance.asset.prefab != null)
                {
                    instance.asset.prefab.MarkAsModified();
                }

                foreach (var level in instance.asset.levels)
                {
                    if (level.mesh != null)
                    {
                        level.mesh.MarkAsModified();
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void SettingsChanged(SettingsUpdateTarget target)
        {
            LogBuildRequestManager.SettingsChanged(target);
        }

        public LogInstance AddNewLog()
        {
            var logID = primaryIDs.GetNextIdAndIncrement();
            var newAsset = LogAsset.Create(subfolders.data, log.nameBasis, logID);

            var instance = LogInstance.Create(subfolders.data, log.nameBasis, logID, newAsset);

            instance.seed.SetInternalSeed(Random.Range(1, BaseSeed.HIGH_ELEMENT));

            logInstances.Add(instance);

            return instance;
        }

        public IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            var costs = buildRequest.GetBuildCosts(level);

            foreach (var instance in logInstances)
            {
                if (instance.active ||
                    (buildState == BuildState.Full) ||
                    (buildState == BuildState.ForceFull))
                {
                    costs = costs.Concat(instance.GetBuildCosts(level));
                }
            }

            return costs;
        }

        public LogInstance GetinstanceByID(int logID)
        {
            foreach (var logInstance in logInstances)
            {
                if (logInstance.logID == logID)
                {
                    return logInstance;
                }
            }

            return null;
        }

        public LogInstance GetLogInstance(int logID)
        {
            foreach (var logInstance in logInstances)
            {
                if (logInstance.logID != logID)
                {
                    continue;
                }

                return logInstance;
            }

            return null;
        }

        public ShapeGeometryData GetShapeGeometryByID(int logIndex, int shapeID)
        {
            var shapes = logInstances[logIndex].shapes;

            foreach (var shape in shapes)
            {
                if ((shape != null) && (shape.shapeID == shapeID))
                {
                    var geo = shape.geometry;

                    if ((geo != null) && (geo.Count > 0))
                    {
                        return geo[0];
                    }
                }
            }

            return null;
        }

        public void PushBuildRequestLevel(
            BuildRequestLevel distribution = BuildRequestLevel.None,
            BuildRequestLevel materialProperties = BuildRequestLevel.None,
            BuildRequestLevel materialGeneration = BuildRequestLevel.None,
            BuildRequestLevel uv = BuildRequestLevel.None,
            BuildRequestLevel collision = BuildRequestLevel.None,
            BuildRequestLevel highQualityGeometry = BuildRequestLevel.None,
            BuildRequestLevel levelsOfDetail = BuildRequestLevel.None,
            BuildRequestLevel vertex = BuildRequestLevel.None)
        {
            if (materialGeneration != BuildRequestLevel.None)
            {
                buildRequest.distribution = materialGeneration;
            }

            if (materialProperties != BuildRequestLevel.None)
            {
                buildRequest.materialProperties = materialProperties;
            }

            foreach (var logInstance in logInstances)
            {
                if (distribution != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.distribution = distribution;
                }

                if (uv != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.uv = uv;
                }

                if (collision != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.collision = uv;
                }

                if (highQualityGeometry != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.highQualityGeometry = highQualityGeometry;
                }

                if (levelsOfDetail != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.levelsOfDetail = levelsOfDetail;
                }

                if (vertex != BuildRequestLevel.None)
                {
                    logInstance.buildRequest.vertexData = vertex;
                }
            }
        }

        public void PushBuildRequestLevelAll(BuildRequestLevel level)
        {
            if (buildRequest == null)
            {
                buildRequest = new LogBuildRequest();
            }

            buildRequest.distribution = level;
            buildRequest.materialProperties = level;

            foreach (var instance in logInstances)
            {
                instance.buildRequest.distribution = level;
                instance.buildRequest.uv = level;
                instance.buildRequest.highQualityGeometry = level;
                instance.buildRequest.levelsOfDetail = level;
                instance.buildRequest.vertexData = level;
            }
        }

        public void RemoveLog(int logID)
        {
            for (var i = logInstances.Count - 1; i >= 0; i--)
            {
                if (logInstances[i].logID == logID)
                {
                    logInstances.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetActive(int logID)
        {
            foreach (var instance in logInstances)
            {
                instance.active = instance.logID == logID;
            }
        }

        [Button]
        [HideIf(nameof(initialized))]
        [EnableIf(nameof(canInitialize))]

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            try
            {
                if (initialized)
                {
                    return;
                }

                await base.Initialize(initializer);

                ResetInitialization();

                var basis = NameBasis.CreateNested(this);
                basis.nameBasis = initializationSettings.name.ToSafe().TrimEnd(',', '.', '_', '-');

                UpdateNameAndMove(basis.nameBasis);

                subfolders = TreeAssetSubfolders.CreateNested(this, false);

                subfolders.nameBasis = basis;
                subfolders.InitializeFromParent(this);

                subfolders.CreateFolders();

                material = new Material(_defaultShaderResource.logShader)
                {
                    name = ZString.Format("{0}", basis.safeName)
                };

                log = TreeLog.Create(subfolders.main, basis);

                if (settings == null)
                {
                    settings = LogSettings.Create(subfolders.data, basis);
                }

                logInstances = new List<LogInstance>();

                log.hierarchies.CreateTrunkHierarchy(null);

                SeedManager.UpdateSeeds(log);

                AddNewLog();

                buildRequest = new LogBuildRequest();

                initialized = true;
                basis.Lock();
                dataState = DataState.Dirty;

                SetDirtyStates();
            }
            catch (Exception ex)
            {
                Context.Log.Error(ex.Message, this);
                initialized = false;
                throw;
            }
        }

        /// <inheritdoc />
        protected override void SaveAllAssets(bool saveImpostors)
        {
            AssetManager.SaveAllAssets(this);
        }

        /// <inheritdoc />
        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();
            using (_PRF_WhenEnabled.Auto())
            {
                if (HasAssetPath(out _))
                {
                    if (HasSubAssets(out var subAssets))
                    {
                        var nameBasisFound = false;

                        for (var i = subAssets.Length - 1; i >= 0; i--)
                        {
                            var basis = subAssets[i] as NameBasis;

                            if (basis == null)
                            {
                                continue;
                            }

                            if (!nameBasisFound)
                            {
                                nameBasisFound = true;
                                log.nameBasis = basis;
                                continue;
                            }

                            AssetDatabaseManager.RemoveObjectFromAsset(basis);
                        }
                    }
                }
            }
        }

        private int GetShapeTypeIndex(TreeComponentType shapeType)
        {
            var shapeTypeIndex = 0;

            switch (shapeType)
            {
                case TreeComponentType.Trunk:
                {
                    shapeTypeIndex = 0;
                }
                    break;
                case TreeComponentType.Branch:
                {
                    shapeTypeIndex = 1;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return shapeTypeIndex;
        }

        private void ResetInitialization()
        {
            //initializationSettings = new InitializationSettings(ResponsiveSettingsType.Tree) {name = name};
            dataState = DataState.Normal;
            initialized = false;
            primaryIDs.SetNextID(0);
            hierarchyPrefabs = null;
            subfolders = null;
            log = null;
            settings = null;
            logInstances = null;
            material = null;
            buildRequest = null;
        }

        #region ILogDataProvider Members

        public int GetLogCount()
        {
            return logInstances.Count;
        }

        public string GetName()
        {
            return log.nameBasis.nameBasis;
        }

        public bool HasLog(int logIndex)
        {
            return (logInstances.Count > logIndex) && (logIndex >= 0);
        }

        public GameObject GetLog(int logIndex)
        {
            var instance = logInstances[logIndex];

            return instance.asset.ToInstance();
        }

        public int GetMaxShapeIndex(int logIndex, TreeComponentType shapeType)
        {
            return logInstances[logIndex].shapes[GetShapeTypeIndex(shapeType)].Count - 1;
        }

        public Matrix4x4 GetShapeMatrix(int logIndex, TreeComponentType shapeType, ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(logIndex, shapeType));

            var shapes = logInstances[logIndex].shapes[shapeTypeIndex];

            if (shapes.Count == 0)
            {
                shapeIndex = 0;
                return Matrix4x4.identity;
            }

            var shape = shapes[shapeIndex] as ShapeData;
            if (shape != null)
            {
                return shape.effectiveMatrix;
            }

            return Matrix4x4.identity;
        }

        public ShapeGeometryData GetShapeGeometry(
            int logIndex,
            TreeComponentType shapeType,
            ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(logIndex, shapeType));

            var shapes = logInstances[logIndex].shapes[shapeTypeIndex];

            if (shapes.Count == 0)
            {
                shapeIndex = 0;
                return null;
            }

            var shape = shapes[shapeIndex] as ShapeData;

            if (shape != null)
            {
                var geo = shape.geometry;

                if ((geo != null) && (geo.Count > 0))
                {
                    return geo[0];
                }
            }

            return null;
        }

        public ShapeData GetShapeData(int logIndex, TreeComponentType shapeType, ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(logIndex, shapeType));

            var shapes = logInstances[logIndex].shapes[shapeTypeIndex];

            if (shapes.Count == 0)
            {
                return null;
            }

            var shape = shapes[shapeIndex] as ShapeData;
            if (shape != null)
            {
                return shape;
            }

            return null;
        }

        public ShapeData GetShapeDataByID(int logIndex, int shapeID)
        {
            var shapes = logInstances[logIndex].shapes;

            foreach (var shape in shapes)
            {
                if ((shape != null) && (shape.shapeID == shapeID))
                {
                    return shape;
                }
            }

            return null;
        }

        public bool drawGizmos
        {
            get => _drawGizmos;
            set => _drawGizmos = value;
        }

        public ScriptableObject GetSerializable()
        {
            return this;
        }

        #endregion

        #region Menu Items

        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(LogDataContainer),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew<LogDataContainer>();
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(LogDataContainer) + ".";

        private static readonly ProfilerMarker _PRF_WhenEnabled =
            new ProfilerMarker(_PRF_PFX + nameof(WhenEnabled));

        #endregion
    }
}
