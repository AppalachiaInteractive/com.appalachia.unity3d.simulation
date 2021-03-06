using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Editing.Labels;
using Appalachia.Rendering.Prefabs.Core;
using Appalachia.Rendering.Prefabs.Rendering;
using Appalachia.Rendering.Prefabs.Rendering.Replacement;
using Appalachia.Simulation.Core.Metadata.Tree;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
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
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.Seeds;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Utility.Async;
using Appalachia.Utility.Extensions;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Appalachia.Simulation.Trees
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public sealed class TreeDataContainer : TSEDataContainer, ISpeciesDataProvider
    {
        static TreeDataContainer()
        {
            RegisterInstanceCallbacks.For<TreeDataContainer>()
                                     .When.Object<LabelSets>()
                                     .IsAvailableThen(i => _labelSets = i);
            RegisterInstanceCallbacks.For<TreeDataContainer>()
                                     .When.Object<PrefabRenderingSetCollection>()
                                     .IsAvailableThen(i => _prefabRenderingSetCollection = i);
            RegisterInstanceCallbacks.For<TreeDataContainer>()
                                     .When.Object<PrefabReplacementCollection>()
                                     .IsAvailableThen(i => _prefabReplacementCollection = i);
            RegisterInstanceCallbacks.For<TreeDataContainer>()
                                     .When.Behaviour<PrefabRenderingManager>()
                                     .IsAvailableThen(i => _prefabRenderingManager = i);
        }

        #region Static Fields and Autoproperties

        private static LabelSets _labelSets;
        private static PrefabRenderingManager _prefabRenderingManager;
        private static PrefabRenderingSetCollection _prefabRenderingSetCollection;
        private static PrefabReplacementCollection _prefabReplacementCollection;

        #endregion

        #region Fields and Autoproperties

        [HideInInspector] public bool _drawGizmos;

        [HideInInspector] public List<TreeIndividual> individuals;

        [HideInInspector] public TreeBuildRequest buildRequest;

        [HideInInspector] public TreeMaterialCollection materials;

        [ShowIf(nameof(initialized))]
        [PropertyOrder(100)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        public TreeSettings settings;

        [PropertyOrder(0)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideLabel]
        [Title(
            "Species Hierarchies",
            "Define the hierarchies for the species (trunk, roots, branches, knots, leaves, and fruit)."
        )]
        [HideInInspector]
        public TreeSpecies species;

        [HideInInspector] public TreeSpeciesMetadata runtimeSpeciesMetadata;

        #endregion

        private static bool DependenciesAreReady =>
            (_labelSets != null) &&
            (_prefabRenderingSetCollection != null) &&
            (_prefabReplacementCollection != null) &&
            (_prefabRenderingManager != null);

        /// <inheritdoc />
        public override ResponsiveSettingsType settingsType => ResponsiveSettingsType.Tree;

        public BuildRequestLevel requestLevel
        {
            get
            {
                if (buildRequest == null)
                {
                    buildRequest = new TreeBuildRequest();
                }

                var rl = buildRequest.requestLevel;

                foreach (var individual in individuals)
                {
                    if (individual.active || (buildState > BuildState.Full))
                    {
                        rl = rl.Max(individual.GetRequestLevel(buildState));

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
            TreeBuildRequestManager.Default();
        }

        /// <inheritdoc />
        public override void BuildForceFull()
        {
            TreeBuildRequestManager.ForceFull();
        }

        /// <inheritdoc />
        public override void BuildFull()
        {
            TreeBuildRequestManager.Full();
        }

        /// <inheritdoc />
        public override void CopyHierarchiesFrom(TSEDataContainer tse)
        {
            if (tse is TreeDataContainer dc)
            {
                dc.species.hierarchies.CopyHierarchiesTo(species.hierarchies);
            }
        }

        /// <inheritdoc />
        public override void CopySettingsFrom(TSEDataContainer tse)
        {
            if (tse is TreeDataContainer dc)
            {
                dc.settings.CopySettingsTo(settings);
            }
        }

        /// <inheritdoc />
        public override NameBasis GetNameBasis()
        {
            if (species != null)
            {
                return species.nameBasis;
            }

            return null;
        }

        /// <inheritdoc />
        public override void RebuildStructures()
        {
            species.hierarchies.Rebuild();

            settings.lod.SetIndices();

            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    foreach (var stage in age.stages)
                    {
                        stage.Refresh(settings, species.hierarchies);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void SetDirtyStates()
        {
            MarkAsModified();
            settings.MarkAsModified();
            species.MarkAsModified();
            materials.MarkAsModified();
            subfolders.MarkAsModified();
            runtimeSpeciesMetadata.MarkAsModified();

            foreach (var material in materials.outputMaterialCache.tiledOutputMaterials)
            {
                foreach (var subMaterial in material)
                {
                    if (subMaterial.asset != null)
                    {
                        subMaterial.asset.MarkAsModified();
                    }
                }
            }

            if (settings.lod.shadowCaster)
            {
                foreach (var subMaterial in materials.outputMaterialCache.shadowCasterOutputMaterial)
                {
                    if (subMaterial.asset != null)
                    {
                        subMaterial.asset.MarkAsModified();
                    }
                }
            }

            if (materials.outputMaterialCache.atlasOutputMaterial != null)
            {
                foreach (var subMaterial in materials.outputMaterialCache.atlasOutputMaterial)
                {
                    if (subMaterial.asset != null)
                    {
                        subMaterial.asset.MarkAsModified();
                    }
                }
            }

            hierarchyPrefabs.MarkAsModified();

            foreach (var individual in individuals)
            {
                individual.MarkAsModified();

                foreach (var age in individual.ages)
                {
                    age.MarkAsModified();
                    age.integrationAsset.MarkAsModified();

                    foreach (var stage in age.stages)
                    {
                        stage.MarkAsModified();
                        stage.asset.MarkAsModified();

                        if (stage.asset.prefab != null)
                        {
                            stage.asset.prefab.MarkAsModified();
                        }

                        foreach (var level in stage.asset.levels)
                        {
                            if (level.mesh != null)
                            {
                                level.mesh.MarkAsModified();
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void SettingsChanged(SettingsUpdateTarget target)
        {
            TreeBuildRequestManager.SettingsChanged(target);
        }

        public TreeIndividual AddNewIndividual()
        {
            var individualID = primaryIDs.GetNextIdAndIncrement();
            var newAsset = TreeAsset.Create(
                subfolders.data,
                species.nameBasis,
                individualID,
                AgeType.Mature,
                StageType.Normal
            );

            var set = TreeIndividual.Create(subfolders.data, species.nameBasis, individualID, newAsset);

            set.seed.SetInternalSeed(Random.Range(1, BaseSeed.HIGH_ELEMENT));

            individuals.Add(set);

            return set;
        }

        public void CreateRuntimeMetadata()
        {
            runtimeSpeciesMetadata.name = species.nameBasis.nameBasis;
            runtimeSpeciesMetadata = TreeSpeciesMetadata.LoadOrCreateNew(GetNameBasis().safeName);
            runtimeSpeciesMetadata.individuals = new List<TreeIndividualMetadata>();

            foreach (var individual in individuals)
            {
                var assetName = species.nameBasis.FileNameIndividualMetadataSO(individual.individualID);

                var instance = TreeIndividualMetadata.LoadOrCreateNew(subfolders.data, assetName);

                foreach (var age in individual.ages)
                {
                    var tam = new TreeAgeMetadata();

                    foreach (var stage in age.stages)
                    {
                        var tsm = new TreeStageMetadata();

                        tsm.prefab = stage.asset.prefab;

                        tam.Set(stage.stageType, tsm);
                    }

                    instance.Set(age.ageType, tam);
                }

                runtimeSpeciesMetadata.individuals.Add(instance);
            }
        }

        public IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            var costs = buildRequest.GetBuildCosts(level);

            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    if (age.active || (buildState == BuildState.Full) || (buildState == BuildState.ForceFull))
                    {
                        costs = costs.Concat(age.GetBuildCosts(level));
                    }

                    foreach (var stage in age.stages)
                    {
                        if (stage.active ||
                            (buildState == BuildState.Full) ||
                            (buildState == BuildState.ForceFull))
                        {
                            costs = costs.Concat(stage.GetBuildCosts(level));
                        }
                    }
                }
            }

            return costs;
        }

        public TreeIndividual GetIndividualByID(int individualID)
        {
            foreach (var individual in individuals)
            {
                if (individual.individualID == individualID)
                {
                    return individual;
                }
            }

            return null;
        }

        public ShapeGeometryData GetShapeGeometryByID(
            int individualIndex,
            AgeType age,
            StageType stage,
            int shapeID)
        {
            var shapes = individuals[individualIndex][age][stage].shapes;

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

        public TreeStage GetStage(int individualID, AgeType ageType, StageType stageType)
        {
            foreach (var individual in individuals)
            {
                if (individual.individualID != individualID)
                {
                    continue;
                }

                foreach (var age in individual.ages)
                {
                    if (age.ageType != ageType)
                    {
                        continue;
                    }

                    foreach (var stage in age.stages)
                    {
                        if (stage.stageType != stageType)
                        {
                            continue;
                        }

                        return stage;
                    }
                }
            }

            return null;
        }

        public void PushBuildRequestLevel(
            BuildRequestLevel distribution = BuildRequestLevel.None,
            BuildRequestLevel materialProperties = BuildRequestLevel.None,
            /*BuildRequestLevel prefabCreation = BuildRequestLevel.None,*/
            BuildRequestLevel materialGeneration = BuildRequestLevel.None,
            BuildRequestLevel uv = BuildRequestLevel.None,
            BuildRequestLevel collision = BuildRequestLevel.None,
            BuildRequestLevel highQualityGeometry = BuildRequestLevel.None,
            BuildRequestLevel lowQualityGeometry = BuildRequestLevel.None,
            BuildRequestLevel ambientOcclusion = BuildRequestLevel.None,

            //BuildRequestLevel levelsOfDetail = BuildRequestLevel.None,
            BuildRequestLevel wind = BuildRequestLevel.None,
            BuildRequestLevel impostors = BuildRequestLevel.None)
        {
            if (materialGeneration != BuildRequestLevel.None)
            {
                buildRequest.materialGeneration = materialGeneration;
            }

            if (materialProperties != BuildRequestLevel.None)
            {
                buildRequest.materialProperties = materialProperties;
            }

            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    if (distribution != BuildRequestLevel.None)
                    {
                        age.buildRequest.distribution = distribution;
                    }

                    foreach (var stage in age.stages)
                    {
                        if (uv != BuildRequestLevel.None)
                        {
                            stage.buildRequest.uv = uv;
                        }

                        if (collision != BuildRequestLevel.None)
                        {
                            stage.buildRequest.collision = collision;
                        }

                        if (highQualityGeometry != BuildRequestLevel.None)
                        {
                            stage.buildRequest.highQualityGeometry = highQualityGeometry;
                        }

                        if (lowQualityGeometry != BuildRequestLevel.None)
                        {
                            stage.buildRequest.lowQualityGeometry = lowQualityGeometry;
                        }

                        if (ambientOcclusion != BuildRequestLevel.None)
                        {
                            stage.buildRequest.ambientOcclusion = ambientOcclusion;
                        }

                        if (wind != BuildRequestLevel.None)
                        {
                            stage.buildRequest.vertexData = wind;
                        }

                        if (impostors != BuildRequestLevel.None)
                        {
                            stage.buildRequest.impostor = impostors;
                        }
                    }
                }
            }
        }

        public void PushBuildRequestLevelAll(BuildRequestLevel level)
        {
            if (buildRequest == null)
            {
                buildRequest = new TreeBuildRequest();
            }

            buildRequest.materialGeneration = level;
            buildRequest.materialProperties = level;
            /*buildRequest.prefabCreation = level;*/

            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    age.buildRequest.distribution = level;

                    foreach (var stage in age.stages)
                    {
                        stage.buildRequest.uv = level;
                        stage.buildRequest.highQualityGeometry = level;
                        stage.buildRequest.lowQualityGeometry = level;
                        stage.buildRequest.ambientOcclusion = level;

                        //stage.buildRequest.levelsOfDetail = level;
                        stage.buildRequest.vertexData = level;
                        stage.buildRequest.collision = level;
                        stage.buildRequest.impostor = level;
                    }
                }
            }
        }

        public void RemoveIndividual(int individualID)
        {
            for (var i = individuals.Count - 1; i >= 0; i--)
            {
                if (individuals[i].individualID == individualID)
                {
                    individuals.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetActive(int individualID, AgeType ageType, StageType stageType)
        {
            foreach (var individual in individuals)
            {
                individual.active = individual.individualID == individualID;

                foreach (var age in individual.ages)
                {
                    age.active = (age.individualID == individualID) && (age.ageType == ageType);

                    foreach (var stage in age.stages)
                    {
                        stage.active = (stage.individualID == individualID) &&
                                       (stage.ageType == ageType) &&
                                       (stage.stageType == stageType);
                    }
                }
            }
        }

        public void UpdateIntegrationData()
        {
            AddToPrefabRenderingCollection();
            AddToPrefabReplacementCollection();
        }

        public void UpdateRuntimeMetadata()
        {
            runtimeSpeciesMetadata.woodData = species.woodData;
            runtimeSpeciesMetadata.individuals = new List<TreeIndividualMetadata>();

            foreach (var individual in individuals)
            {
                var tim = TreeIndividualMetadata.CreateNew();

                tim.individualID = individual.individualID;

                foreach (var age in individual.ages)
                {
                    var tam = new TreeAgeMetadata();

                    foreach (var stage in age.stages)
                    {
                        var tsm = new TreeStageMetadata();

                        tsm.prefab = stage.asset.prefab;

                        tam.Set(stage.stageType, tsm);
                    }

                    tim.Set(age.ageType, tam);
                }

                runtimeSpeciesMetadata.individuals.Add(tim);
            }

            UpdateIntegrationData();
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

                if (initializationSettings.convertTreeData)
                {
                    species = TreeSpecies.Create(initializationSettings.original, subfolders.main, basis);
                    species.hierarchies.verticalOffset = initializationSettings.original.root.groundOffset;
                }
                else
                {
                    species = TreeSpecies.Create(subfolders.data, basis);
                    species.seed.value = Random.Range(1, BaseSeed.HIGH_ELEMENT);
                    species.seed.SetInternalSeed(
                        Random.Range(1, BaseSeed.HIGH_ELEMENT),
                        Random.Range(1, BaseSeed.HIGH_ELEMENT)
                    );
                }

                if (settings == null)
                {
                    settings = TreeSettings.Create(subfolders.data, basis);
                }

                if (settings.lod.impostor.impostorAfterLastLevel)
                {
                    subfolders.CreateImpostorFolder();
                }

                individuals = new List<TreeIndividual>();

                hierarchyPrefabs = TreePrefabCollection.Create(subfolders.data, basis);
                hierarchyPrefabs.ResetPrefabs();
                hierarchyPrefabs.UpdatePrefabs(species);

                materials = TreeMaterialCollection.Create(subfolders.data, basis);
                materials.UpdateMaterials(
                    species,
                    hierarchyPrefabs,
                    settings.lod.levels,
                    settings.lod.shadowCaster
                );
                materials.inputMaterialCache.SetDefaultMaterials(settings.material.defaultMaterials);
                materials.inputMaterialCache.UpdateDefaultMaterials();

                if (!initializationSettings.convertTreeData)
                {
                    species.hierarchies.CreateTrunkHierarchy(materials.inputMaterialCache);
                }

                SeedManager.UpdateSeeds(species);

                AddNewIndividual();

                buildRequest = new TreeBuildRequest();

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
            UpdateRuntimeMetadata();

            AssetManager.SaveAllAssets(this, saveImpostors);
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
                                species.nameBasis = basis;
                                continue;
                            }

                            AssetDatabaseManager.RemoveObjectFromAsset(basis);
                        }
                    }
                }
            }
        }

        private void AddToPrefabRenderingCollection()
        {
            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    foreach (var stage in age.stages)
                    {
                        var bounds = stage.asset.levels[0].mesh.bounds;

                        var magnitude = bounds.size.magnitude;

                        var t = PrefabModelType.TreeLarge;

                        if (magnitude < _labelSets.trees.terms[0].allowedMagnitude)
                        {
                            t = PrefabModelType.TreeSmall;
                        }
                        else if (magnitude < _labelSets.trees.terms[1].allowedMagnitude)
                        {
                            t = PrefabModelType.TreeMedium;
                        }

                        var prefabSetCollection = _prefabRenderingSetCollection;
                        var prefab = stage.asset.prefab;

                        PrefabRenderingSet set;

                        if (!prefabSetCollection.Sets.TryGet(prefab, out set))
                        {
                            set = _prefabRenderingManager.ManageNewPrefabRegistration(prefab);
                        }

                        set.modelType = t;

                        LabelManager.ApplyLabelsToPrefab(prefab, t);
                    }
                }
            }
        }

        private void AddToPrefabReplacementCollection()
        {
            foreach (var individual in individuals)
            {
                foreach (var age in individual.ages)
                {
                    foreach (var stage in age.stages)
                    {
                        _prefabReplacementCollection.State.AddOrUpdate(
                            stage.asset.prefab,
                            age.integrationAsset.prefab
                        );
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
                case TreeComponentType.Root:
                {
                    shapeTypeIndex = 1;
                }
                    break;
                case TreeComponentType.Branch:
                {
                    shapeTypeIndex = 2;
                }
                    break;
                case TreeComponentType.Knot:
                {
                    shapeTypeIndex = 3;
                }
                    break;
                case TreeComponentType.Leaf:
                {
                    shapeTypeIndex = 4;
                }
                    break;
                case TreeComponentType.Fruit:
                {
                    shapeTypeIndex = 5;
                }
                    break;
                case TreeComponentType.Fungus:
                {
                    shapeTypeIndex = 6;
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
            species = null;
            settings = null;
            individuals = null;
            materials = null;
            buildRequest = null;
        }

        #region ISpeciesDataProvider Members

        public int GetIndividualCount()
        {
            return individuals?.Count ?? 0;
        }

        public string GetSpeciesName()
        {
            return species.nameBasis.nameBasis;
        }

        public bool HasIndividual(int individualIndex, AgeType age, StageType stage)
        {
            return (individuals.Count > individualIndex) &&
                   (individualIndex >= 0) &&
                   individuals[individualIndex].HasType(age) &&
                   individuals[individualIndex][age].HasType(stage);
        }

        public GameObject GetIndividual(int individualIndex, AgeType ageType, StageType stageType)
        {
            var individual = individuals[individualIndex];
            var age = individual[ageType];
            var stage = age.normalStage;

            if (stageType != StageType.Normal)
            {
                stage = age[stageType];
            }

            if (stage != null)
            {
                return stage.asset.ToInstance();
            }

            return null;
        }

        public int GetMaxShapeIndex(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType)
        {
            switch (shapeType)
            {
                case TreeComponentType.Trunk:
                    return individuals[individualIndex][age][stage].shapes[0].Count - 1;
                case TreeComponentType.Root:
                    return individuals[individualIndex][age][stage].shapes[1].Count - 1;
                case TreeComponentType.Branch:
                    return individuals[individualIndex][age][stage].shapes[2].Count - 1;
                case TreeComponentType.Knot:
                    return individuals[individualIndex][age][stage].shapes[3].Count - 1;
                case TreeComponentType.Leaf:
                    return individuals[individualIndex][age][stage].shapes[4].Count - 1;
                case TreeComponentType.Fruit:
                    return individuals[individualIndex][age][stage].shapes[5].Count - 1;
                case TreeComponentType.Fungus:
                    return individuals[individualIndex][age][stage].shapes[6].Count - 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Matrix4x4 GetShapeMatrix(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(individualIndex, age, stage, shapeType));

            var shapes = individuals[individualIndex][age][stage].shapes[shapeTypeIndex];

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
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(individualIndex, age, stage, shapeType));

            var shapes = individuals[individualIndex][age][stage].shapes[shapeTypeIndex];

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

        public ShapeData GetShapeData(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex)
        {
            var shapeTypeIndex = GetShapeTypeIndex(shapeType);
            shapeIndex = Mathf.Clamp(shapeIndex, 0, GetMaxShapeIndex(individualIndex, age, stage, shapeType));

            var shapes = individuals[individualIndex][age][stage].shapes[shapeTypeIndex];

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

        public ShapeData GetShapeDataByID(int individualIndex, AgeType age, StageType stage, int shapeID)
        {
            var shapes = individuals[individualIndex][age][stage].shapes;

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

#if UNITY_EDITOR

        #region Menu Items

        [UnityEditor.MenuItem(
            PKG.Menu.Assets.Base + nameof(TreeDataContainer),
            priority = PKG.Menu.Assets.Priority
        )]
        public static void CreateAsset()
        {
            CreateNew(typeof(TreeDataContainer));
        }

        #endregion

#endif

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(TreeDataContainer) + ".";

        private static readonly ProfilerMarker _PRF_WhenEnabled =
            new ProfilerMarker(_PRF_PFX + nameof(WhenEnabled));

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        #endregion
    }
}
