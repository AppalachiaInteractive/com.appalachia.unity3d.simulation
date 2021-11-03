#region

using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Behaviours;
using Appalachia.Core.Collections.Native;
using Appalachia.Core.Extensions;
using Appalachia.Core.Filtering;
using Appalachia.Core.Math.Smoothing;
using Appalachia.Core.Preferences.Globals;
using Appalachia.Core.Shading;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Jobs;
using Appalachia.Jobs.MeshData;
using Appalachia.Simulation.Buoyancy.Data;
using Appalachia.Simulation.Buoyancy.Jobs;
using Appalachia.Simulation.Buoyancy.Voxels;
using Appalachia.Simulation.Core;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Physical;
using Appalachia.Simulation.Physical.Integration;
using Appalachia.Simulation.Physical.Sampling;
using Appalachia.Simulation.Wind;
using Appalachia.Spatial.Voxels;
using Appalachia.Spatial.Voxels.Gizmos;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Extensions;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Simulation.Buoyancy
{
    [ExecuteAlways]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(RigidbodyDensityManager))]
    [ExecutionOrder(-90)]
    public class Buoyant : InstancedAppalachiaBehaviour
    {
        private const string _PRF_PFX = nameof(Buoyant) + ".";

        private static readonly ProfilerMarker _PRF_OnEnable = new(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs));

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_Initialize =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".Initialize");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_PositionDataUpdate =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".PositionDataUpdate");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CompletePhysical =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CompletePhysical");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_Synchronize =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".Synchronize");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_JobHandleComplete =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".JobHandleComplete");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_ResetCenterOfMass =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".ResetCenterOfMass");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_WaterUpdateSamples =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".WaterUpdateSamples");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CalculateForcesJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CalculateForcesJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CalculateSubmersionJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CalculateSubmersionJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_InitializeParameters =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".InitializeParameters");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_MaterialVariables =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".MaterialVariables");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_UpdatePhysical =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".UpdatePhysical");

        private static readonly ProfilerMarker
            _PRF_ScheduleBuoyancyJobs_AggregateForceAndTorqueJob = new(_PRF_PFX +
                nameof(ScheduleBuoyancyJobs) +
                ".AggregateForceAndTorqueJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_ResetForceAndTorqueJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".ResetForceAndTorqueJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CombinedDependencies =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CombinedDependencies");

        private static readonly ProfilerMarker _PRF_Update = new(_PRF_PFX + nameof(Update));

        private static readonly ProfilerMarker _PRF_OnDisable = new(_PRF_PFX + nameof(OnDisable));

        private static readonly ProfilerMarker _PRF_OnDestroy = new(_PRF_PFX + nameof(OnDestroy));

        private static readonly ProfilerMarker _PRF_CleanUp = new(_PRF_PFX + nameof(CleanUp));

        private static readonly ProfilerMarker _PRF_UpdateDrag = new(_PRF_PFX + nameof(UpdateDrag));

        [FoldoutGroup("State")]
        [SmartLabel]
        [ShowInInspector]
        public double3Average averagedTorque;

        [FormerlySerializedAs("data")]
        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        public BuoyancyData buoyancyData;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [ReadOnly]
        public RigidbodyDensityManager densityManager;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        [ShowInInspector]
        [NonSerialized]
        private DensityMetadata _airDensityMetadata;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        [ShowInInspector]
        [NonSerialized]
        public BuoyancyDataDefaults _defaultBuoyancyData;

        [NonSerialized] private bool _initialized;
        [NonSerialized] private PositioningData _positionData;
        [NonSerialized] private Water _water;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        [ShowInInspector]
        [NonSerialized]
        private DensityMetadata _waterDensityMetadata;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        [ShowInInspector]
        [NonSerialized]
        private WaterPhysicsCoefficients _waterPhysicsCoefficients;

        [FoldoutGroup("State")]
        [SmartLabel]
        [NonSerialized]
        [ShowInInspector]
        [PropertyRange(0.001f, 1.0f)]
        public float activeRatio = 1.0f;

        [NonSerialized] public Rigidbody body;

        [NonSerialized] public Vector3 cumulativeForce;
        [NonSerialized] public Vector3 cumulativeTorque;
        [NonSerialized] public TrilinearSamples<WaterVoxel> currentSamples;
        [NonSerialized] public JobHandle jobHandle;

        [FoldoutGroup("State")]
        [SmartLabel]
        [NonSerialized]
        [ShowInInspector]
        public RigidbodyData originalBodyData;

        [FoldoutGroup("State")]
        [SmartLabel]
        [ReadOnly]
        [PropertyRange(0f, 1f)]
        [NonSerialized]
        [ShowInInspector]
        public float percentageSubmerged;

        [FoldoutGroup("State")]
        [SmartLabel(Postfix = true)]
        [ReadOnly]
        [NonSerialized]
        [ShowInInspector]
        public bool resting;

        [NonSerialized]
        [ShowInInspector]
        [FoldoutGroup("State")]
        public float submersionWetness;

        [NonSerialized] public BuoyancyVoxels voxels;

        [NonSerialized]
        [ShowInInspector]
        [FoldoutGroup("State")]
        public float wetness;

        [FoldoutGroup("State")]
        [SmartLabel(Postfix = true)]
        [ReadOnly]
        [ShowInInspector]
        public bool submerged => percentageSubmerged > 0;

        public BuoyancyDataDefaults defaultBuoyancyData
        {
            get
            {
                if (_defaultBuoyancyData == null)
                {
                    _defaultBuoyancyData = BuoyancyDataDefaults.instance;
                }

                return _defaultBuoyancyData;
            }
        }

        public WaterPhysicsCoefficients waterPhysicsCoefficients
        {
            get
            {
                if (_waterPhysicsCoefficients == null)
                {
                    _waterPhysicsCoefficients = WaterPhysicsCoefficients.instance;
                }

                return _waterPhysicsCoefficients;
            }
        }

        public DensityMetadata waterDensityMetadata
        {
            get
            {
                if (_waterDensityMetadata == null)
                {
                    _waterDensityMetadata = DensityMetadataCollection.instance.water;
                }

                return _waterDensityMetadata;
            }
        }

        public DensityMetadata airDensityMetadata
        {
            get
            {
                if (_airDensityMetadata == null)
                {
                    _airDensityMetadata = DensityMetadataCollection.instance.air;
                }

                return _airDensityMetadata;
            }
        }

        private bool _canDirectRegister => PhysicsSimulator.IsSimulationActive && (_water == null);

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                if (buoyancyData == null)
                {
                    return;
                }

                if (_water == null)
                {
                    return;
                }

                if (buoyancyData.buoyancyType == BuoyancyType.NonPhysical)
                {
                    var t = transform;
                    var vec = t.position;
                    vec.y = _water.GetWorldHeightAt(vec) + buoyancyData.waterLevelOffset;
                    t.position = vec;
                    t.up = Vector3.Slerp(t.up, Vector3.up, Time.deltaTime);
                }

                UpdateAllInstancedProperties();
            }
        }

        private void OnEnable()
        {
            using (_PRF_OnEnable.Auto())
            {
                if (_water == null)
                {
                    enabled = false;
                }

                PhysicsSimulator.onSimulationEnd -= Disable;
                PhysicsSimulator.onSimulationEnd += Disable;

                if (body == null)
                {
                    body = GetComponent<Rigidbody>();
                }

                if (body == null)
                {
                    return;
                }

                if (originalBodyData == null)
                {
                    originalBodyData = new RigidbodyData(body);
                }
                else if (Math.Abs(body.mass - originalBodyData.mass) > float.Epsilon)
                {
                    originalBodyData.GetFrom(body);
                }

                _airDensityMetadata = airDensityMetadata;
                _waterDensityMetadata = waterDensityMetadata;
                _defaultBuoyancyData = defaultBuoyancyData;

                body.drag += waterPhysicsCoefficients.data.additionalDrag;
                body.angularDrag += waterPhysicsCoefficients.data.additionalAngularDrag;

                if (waterPhysicsCoefficients.data.disableGravity)
                {
                    body.useGravity = false;
                }
            }
        }

        private void OnDisable()
        {
            using (_PRF_OnDisable.Auto())
            {
#if UNITY_EDITOR

                if (EditorApplication.isCompiling ||
                    EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    CleanUp();
                }
#endif
                if ((originalBodyData == null) || (body == null))
                {
                    return;
                }

                originalBodyData.ApplyTo(body);
            }
        }

        private void OnDestroy()
        {
            using (_PRF_OnDestroy.Auto())
            {
                CleanUp();
            }
        }

        public void EnterWater(Water w)
        {
            _water = w;
            enabled = true;
        }

        public void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (_initialized)
                {
                    return;
                }

#if UNITY_EDITOR
                if (buoyancyData == null)
                {
                    var mesh = MeshObjectManager.GetCheapestMesh(gameObject);

                    AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(mesh, out var key, out long _);

                    buoyancyData =
                        BuoyancyDataCollection.instance.GetOrLoadOrCreateNew(
                            key,
                            $"{mesh.name}_{key}"
                        );

                    buoyancyData.mesh = mesh;

                    EditorUtility.SetDirty(buoyancyData);
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
                else if (buoyancyData.mesh == null)
                {
                    buoyancyData.mesh = MeshObjectManager.GetCheapestMesh(gameObject);

                    EditorUtility.SetDirty(buoyancyData);
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
#endif

                buoyancyData.Prepare();

                body = GetComponent<Rigidbody>();

                if (!TryGetComponent(out body))
                {
                    body = gameObject.AddComponent<Rigidbody>();
                    Debug.LogError(
                        $"Buoyancy:Object \"{name}\" had no Rigidbody. Rigidbody has been added."
                    );
                }

                densityManager = GetComponent<RigidbodyDensityManager>();

                if (densityManager == null)
                {
                    densityManager = RigidbodyDensityManager.CreateNow(gameObject);
                }

                densityManager.Initialize();

                if (voxels != null)
                {
                    voxels.SafeDispose();
                }

                body.ResetCenterOfMass();
                body.centerOfMass += buoyancyData.centerOfMassOffset;

                var lookupInstance = BuoyancyVoxelsDataStoreLookup.instance;
                var persistence = lookupInstance.GetOrLoadOrCreateNew(
                    buoyancyData.name,
                    buoyancyData.name
                );

                var renderers = this.FilterComponentsFromChildren<MeshRenderer>().RunFilter();
                var colliders = this.FilterComponentsFromChildren<Collider>()
                                    .ActiveOnly()
                                    .NoTriggers()
                                    .RunFilter();

                var shouldRestore = persistence.ShouldRestore(
                    buoyancyData.voxelResolution,
                    buoyancyData.voxelPopulationStyle,
                    colliders,
                    renderers
                );

                voxels = new BuoyancyVoxels(buoyancyData.name);
                voxels.dataStore = persistence;

                if (shouldRestore)
                {
                    voxels.RestoreFromDataStore(_transform, colliders, renderers, activeRatio);
                }
                else if (buoyancyData.buoyancyType == BuoyancyType.PhysicalVoxel)
                {
                    voxels = BuoyancyVoxels.Voxelize(
                        voxels,
                        buoyancyData.voxelPopulationStyle,
                        _transform,
                        colliders,
                        renderers,
                        buoyancyData.voxelResolution
                    );

                    densityManager.volume = voxels.worldVolume;
                }
                else
                {
                    var bounds = colliders.GetEncompassingBounds();
                    bounds.Encapsulate(renderers.GetEncompassingBounds());

                    voxels = BuoyancyVoxels.VoxelizeSingle(
                        voxels,
                        _transform,
                        bounds,
                        body.centerOfMass
                    );
                }

                voxels.centerOfMass = body.centerOfMass;

                jobHandle = voxels.SetupPhysical();

                _initialized = true;
            }
        }

        public void ScheduleBuoyancyJobs(Water water, float deltaTime)
        {
            using (_PRF_ScheduleBuoyancyJobs.Auto())
            {
                try
                {
                    if ((buoyancyData.buoyancyType != BuoyancyType.Physical) &&
                        (buoyancyData.buoyancyType != BuoyancyType.PhysicalVoxel))
                    {
                        return;
                    }

                    _water = water;

                    using (_PRF_ScheduleBuoyancyJobs_Initialize.Auto())
                    {
                        Initialize();
                    }

                    int voxelCount;
                    WaterPhysicsCoefficentData metadata;
                    float4x4 localToWorldMatrix;
                    float waterDensity;
                    float airDensity;
                    float3 windDynamicPressure;
                    float worldWaterHeight;
                    float3 archimedesForce;

                    using (_PRF_ScheduleBuoyancyJobs_InitializeParameters.Auto())
                    {
                        voxelCount = voxels.count;
                        metadata = waterPhysicsCoefficients.data;
                        localToWorldMatrix = _transform.localToWorldMatrix;
                        waterDensity = waterDensityMetadata.densityKGPerCubicMeter;
                        airDensity = airDensityMetadata.densityKGPerCubicMeter;
                        var gravity = Physics.gravity;
                        windDynamicPressure = GlobalWindManager.instance.WindDynamicPressure;
                        worldWaterHeight = water.GetWorldHeightAt(body.worldCenterOfMass);
                        var archimedesForceMagnitude = waterDensityMetadata.densityKGPerCubicMeter *
                                                       Mathf.Abs(gravity.y) *
                                                       voxels.worldVolume;
                        archimedesForce = new float3(0, archimedesForceMagnitude, 0) / voxelCount;
                    }

                    using (_PRF_ScheduleBuoyancyJobs_PositionDataUpdate.Auto())
                    {
                        if (_positionData == default)
                        {
                            _positionData = new PositioningData();
                        }

                        _positionData.Update(body, localToWorldMatrix);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_JobHandleComplete.Auto())
                    {
                        jobHandle.Complete();
                    }

                    using (_PRF_ScheduleBuoyancyJobs_CompletePhysical.Auto())
                    {
                        voxels.CompletePhysical();
                    }

                    using (_PRF_ScheduleBuoyancyJobs_Synchronize.Auto())
                    {
                        voxels.Synchronize();
                    }

                    if (activeRatio == 0f)
                    {
                        activeRatio = 1f;
                    }

                    activeRatio = math.clamp(activeRatio, 0f, 1f);
                    voxels.UpdateVoxelActiveRatio(activeRatio);

                    var prevPct = voxels.objectData.previousSubmersionPercentage;
                    var subPct = voxels.objectData.submersionPercentage.Value;

                    voxels.objectData.previousSubmersionPercentage = subPct;

                    using (_PRF_ScheduleBuoyancyJobs_MaterialVariables.Auto())
                    {
                        var wetter = subPct >= prevPct;
                        var wetSpeed = wetter
                            ? buoyancyData.wetnessAccumulationSpeed
                            : buoyancyData.wetnessDispersalSpeed;
                        var wetnessTarget = wetter ? 1.0f : 0.0f;

                        var submergeder = subPct > .75f;
                        var submersionSpeed = submergeder
                            ? buoyancyData.submergednessAccumulationSpeed
                            : buoyancyData.submergednessDispersalSpeed;
                        var submersionTarget = submergeder ? 1.0f : wetnessTarget * .25f;

                        wetness = math.lerp(wetness, wetnessTarget, wetSpeed);
                        submersionWetness = math.lerp(
                            submersionWetness,
                            submersionTarget,
                            submersionSpeed
                        );
                    }

                    using (_PRF_ScheduleBuoyancyJobs_ResetCenterOfMass.Auto())
                    {
                        if (body.centerOfMass != buoyancyData.centerOfMassOffset)
                        {
                            body.ResetCenterOfMass();
                            body.centerOfMass += buoyancyData.centerOfMassOffset;
                        }
                    }

                    using (_PRF_ScheduleBuoyancyJobs_WaterUpdateSamples.Auto())
                    {
                        if (!currentSamples.isCreated)
                        {
                            currentSamples = new TrilinearSamples<WaterVoxel>(voxels.rawBounds, 4);
                        }

                        currentSamples.localToWorld = localToWorldMatrix;

                        _water.UpdateSamples(ref currentSamples);
                    }

                    //var scalarVelocity = math.length(_positionData.current.localVelocity);
                    //var resistanceCoefficient = ResistanceCoefficient(metadata, scalarVelocity, colliderBounds.size.magnitude);
                    JobHandle handle1;
                    JobHandle handle2;
                    JobHandle handle3a;
                    JobHandle handle3b;
                    JobHandle handle3b1;

                    using (_PRF_ScheduleBuoyancyJobs_UpdatePhysical.Auto())
                    {
                        handle1 = voxels.UpdatePhysical(deltaTime);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_CalculateForcesJob.Auto())
                    {
                        handle2 = new BuoyancyForceJobs.CalculateForcesJob
                        {
                            voxels = voxels.voxels,
                            voxelsActive = voxels.voxelsActive,
                            waterLevelOffset = buoyancyData.waterLevelOffset,
                            worldWaterHeight = worldWaterHeight,
                            submersionDisengageSmoothing =
                                buoyancyData.submersionDisengageSmoothing,
                            submersionEngageSmoothing = buoyancyData.submersionEngageSmoothing,
                            metadata = metadata,
                            airDensity = airDensity,
                            waterDensity = waterDensity,
                            totalMass = body.mass,
                            archimedesForce = archimedesForce,
                            windDynamicPressure = windDynamicPressure,
                            currentSamples = currentSamples,
                            buoyancyVoxels = voxels.elementDatas
                        }.Schedule(voxelCount, JOB_SIZE._LARGE, handle1);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_CalculateSubmersionJob.Auto())
                    {
                        handle3a = new BuoyancyForceJobs.CalculateSubmersionJob
                        {
                            buoyancyVoxels = voxels.elementDatas,
                            voxelsActive = voxels.voxelsActive,
                            voxelsActiveCount = voxels.voxelsActiveCount,
                            submersionPercentage = voxels.objectData.submersionPercentage
                        }.Schedule(handle2);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_ResetForceAndTorqueJob.Auto())
                    {
                        handle3b = new BuoyancyForceJobs.ResetForceAndTorqueJob
                        {
                            force = voxels.objectData.force,
                            torque = voxels.objectData.torque,
                            hydrostaticForce = voxels.objectData.hydrostaticForce,
                            viscousWaterResistanceForce =
                                voxels.objectData.viscousWaterResistanceForce,
                            pressureDragForce = voxels.objectData.pressureDragForce,
                            airResistanceForce = voxels.objectData.airResistanceForce,
                            windResistanceForce = voxels.objectData.windResistanceForce,
                            waveDriftingForce = voxels.objectData.waveDriftingForce,
                            slammingForce = voxels.objectData.slammingForce
                        }.Schedule(handle2);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_AggregateForceAndTorqueJob.Auto())
                    {
                        handle3b1 = new BuoyancyForceJobs.AggregateForceAndTorqueJob
                        {
                            voxels = voxels.voxels,
                            voxelsActive = voxels.voxelsActive,
                            activeRatio = voxels.activeRatio,
                            buoyancyVoxels = voxels.elementDatas,
                            worldCenterOfMass = body.worldCenterOfMass,
                            force = voxels.objectData.force.GetParallel(),
                            torque = voxels.objectData.torque.GetParallel(),
                            hydrostaticForce = voxels.objectData.hydrostaticForce.GetParallel(),
                            viscousWaterResistanceForce =
                                voxels.objectData.viscousWaterResistanceForce.GetParallel(),
                            pressureDragForce =
                                voxels.objectData.pressureDragForce.GetParallel(),
                            airResistanceForce =
                                voxels.objectData.airResistanceForce.GetParallel(),
                            windResistanceForce =
                                voxels.objectData.windResistanceForce.GetParallel(),
                            waveDriftingForce =
                                voxels.objectData.waveDriftingForce.GetParallel(),
                            slammingForce = voxels.objectData.slammingForce.GetParallel()
                        }.Schedule(voxelCount, JOB_SIZE._LARGE, handle3b);
                    }

                    using (_PRF_ScheduleBuoyancyJobs_CombinedDependencies.Auto())
                    {
                        jobHandle = JobHandle.CombineDependencies(handle3a, handle3b1);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    jobHandle.Complete();
                    throw;
                }
            }
        }

        private void Disable()
        {
            enabled = false;
        }

        private void CleanUp()
        {
            using (_PRF_CleanUp.Auto())
            {
                PhysicsSimulator.onSimulationEnd -= Disable;

                jobHandle.Complete();

                if (voxels != null)
                {
                    voxels.SafeDispose();
                }

                currentSamples.SafeDispose();

                if ((originalBodyData != null) && (body != null))
                {
                    originalBodyData.ApplyTo(body);
                }

                _initialized = default;
                _water = default;
                _positionData = default;
                voxels = default;
                cumulativeForce = default;
                cumulativeTorque = default;
                jobHandle = default;
                currentSamples = default;
                densityManager = default;
                body = default;
                originalBodyData = default;
                resting = default;
                percentageSubmerged = default;
                buoyancyData = default;
                _waterPhysicsCoefficients = default;
                _waterDensityMetadata = default;
                _airDensityMetadata = default;

#if UNITY_EDITOR
                _gizmo_cumulatv_ForceLines = default;
                _gizmo_waterLines = default;
                _gizmo_hydrosta_ForceLines = default;
                _gizmo_viscosWR_ForceLines = default;
                _gizmo_presrDrg_ForceLines = default;
                _gizmo_airResis_ForceLines = default;
                _gizmo_windResi_ForceLines = default;
                _gizmo_waveDrft_ForceLines = default;
                _gizmo_slamming_ForceLines = default;
#endif
            }
        }

        [ButtonGroup]
        [EnableIf(nameof(_canDirectRegister))]
        public void DirectRegister()
        {
            var waters = FindObjectsOfType<Water>();
            for (var i = 0; i < waters.Length; i++)
            {
                var water = waters[i];

                var voxelBounds =
                    buoyancyData.voxelPopulationStyle == VoxelPopulationStyle.Colliders
                        ? this.FilterComponentsFromChildren<Collider>()
                              .ActiveOnly()
                              .NoTriggers()
                              .RunFilter()
                              .GetEncompassingBounds()
                        : this.FilterComponentsFromChildren<MeshRenderer>()
                              .ActiveOnly()
                              .RunFilter()
                              .GetEncompassingBounds();

                if (water.waterBounds.Intersects(voxelBounds))
                {
                    water.InitiateBuoyancy(this);
                    break;
                }
            }
        }

        [ButtonGroup]
        public void ExecuteReset()
        {
            CleanUp();
            OnEnable();
        }

        public void UpdateDrag(float submergedAmount)
        {
            using (_PRF_UpdateDrag.Auto())
            {
                submergedAmount = math.clamp(submergedAmount, 0f, 1f);
                percentageSubmerged = math.lerp(percentageSubmerged, submergedAmount, 0.25f);
                body.drag = originalBodyData.drag +
                            (originalBodyData.drag *
                             percentageSubmerged *
                             waterPhysicsCoefficients.data.additionalDrag);
                body.angularDrag = originalBodyData.angularDrag +
                                   (percentageSubmerged *
                                    waterPhysicsCoefficients.data.additionalAngularDrag);
            }
        }

        /// <summary>
        ///     The Coefficient of frictional resistance - belongs to Viscous Water Resistance but is same for all so calculate once
        /// </summary>
        [BurstCompile]
        public static float ResistanceCoefficient(
            WaterPhysicsCoefficentData metadata,
            float velocity,
            float length)
        {
            //Reynolds number

            // Rn = (V * L) / nu
            // V - speed of the body
            // L - length of the submerged body
            //Reynolds number
            var reynoldNumber = (velocity * length) / metadata.Viscosity;

            //The resistance coefficient
            var resistanceCoefficient = 0.075f / math.pow(math.log10(reynoldNumber) - 2f, 2f);

            return resistanceCoefficient;
        }

        protected override void UpdateInstancedProperties(MaterialPropertyBlock block, Material m)
        {
            if ((wetness > 0) || (submersionWetness > 0))
            {
                m.EnableKeyword(GSC.WETNESS._WETABBLE_ON);

                block.SetFloat(GSPL.Get(GSC.WETNESS._Wetness),           wetness);
                block.SetFloat(GSPL.Get(GSC.WETNESS._SubmersionWetness), submersionWetness);
            }
            else
            {
                m.DisableKeyword(GSC.WETNESS._WETABBLE_ON);
            }
        }

#if UNITY_EDITOR

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [OnValueChanged(nameof(DoDisableRenderers))]
        public bool disableRenderers;

        private void DoDisableRenderers()
        {
            var renderers_ = (disableRenderers
                ? this.FilterComponentsFromChildren<Renderer>().ActiveOnly()
                : this.FilterComponentsFromChildren<Renderer>().InactiveOnly()).RunFilter();

            for (var i = 0; i < renderers_.Length; i++)
            {
                var renderer_ = renderers_[i];

                renderer_.enabled = !disableRenderers;
            }
        }

#region Gizmos

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [InlineEditor]
        private VoxelDataGizmoSettings _voxelGizmoSettings;

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [InlineEditor]
        private BuoyancyVoxelDataGizmoSettings _buoyancyGizmoSettings;

        private static readonly ProfilerMarker _PRF_OnDrawGizmos =
            new(_PRF_PFX + nameof(OnDrawGizmos));

        private void OnDrawGizmos()
        {
            using (_PRF_OnDrawGizmos.Auto())
            {
                if (!enabled)
                {
                    return;
                }

                if (_buoyancyGizmoSettings == null)
                {
                    _buoyancyGizmoSettings = BuoyancyVoxelDataGizmoSettings.instance;
                }

                if (!_buoyancyGizmoSettings.drawGizmos ||
                    !_buoyancyGizmoSettings.drawSelectedGizmos)
                {
                    return;
                }

                if (!_initialized)
                {
                    return;
                }

                var position = body.worldCenterOfMass;
                if (_buoyancyGizmoSettings.drawCumulativeForceGizmos)
                {
                    SmartHandles.DrawLine(
                        position,
                        position + (cumulativeForce / body.mass),
                        ColorPrefs.Instance.Buoyancy_CumulativeForce.v
                    );

                    SmartHandles.DrawLine(
                        position,
                        position + (cumulativeTorque / body.mass),
                        ColorPrefs.Instance.Buoyancy_CumulativeTorque.v
                    );
                }

                if (_water == null)
                {
                    return;
                }

                if (_buoyancyGizmoSettings.drawCumulativeWaterLevelGizmos)
                {
                    var waterPosition = position;

                    waterPosition.y = _water.GetWorldHeightAt(position);
                    SmartHandles.DrawLine(
                        position,
                        waterPosition,
                        ColorPrefs.Instance.Buoyancy_CumulativeToSurface.v
                    );
                }
            }
        }

        [NonSerialized] private Vector3[] _gizmo_waterLines;
        [NonSerialized] private Vector3[] _gizmo_cumulatv_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_hydrosta_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_viscosWR_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_presrDrg_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_airResis_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_windResi_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_waveDrft_ForceLines;
        [NonSerialized] private Vector3[] _gizmo_slamming_ForceLines;

        private static readonly ProfilerMarker _PRF_OnDrawGizmosSelected =
            new(_PRF_PFX + nameof(OnDrawGizmosSelected));

        private void OnDrawGizmosSelected()
        {
            using (_PRF_OnDrawGizmosSelected.Auto())
            {
                if (!enabled)
                {
                    return;
                }

                if (_buoyancyGizmoSettings == null)
                {
                    _buoyancyGizmoSettings = BuoyancyVoxelDataGizmoSettings.instance;
                }

                if (!_buoyancyGizmoSettings.drawGizmos ||
                    !_buoyancyGizmoSettings.drawSelectedGizmos)
                {
                    return;
                }

                var voxelLength = voxels.count;

                if (voxelLength == 0)
                {
                    return;
                }

                jobHandle.Complete();

                if (_voxelGizmoSettings == null)
                {
                    var lookup = VoxelDataGizmoSettingsLookup.instance;

                    _voxelGizmoSettings = lookup.GetOrLoadOrCreateNew(
                        VoxelDataGizmoStyle.Buoyancy,
                        nameof(VoxelDataGizmoStyle.Buoyancy)
                    );
                }

                voxels.DrawGizmos(_voxelGizmoSettings);

                if (_buoyancyGizmoSettings.drawSelectedCenterOfMass)
                {
                    Gizmos.color = ColorPrefs.Instance.Buoyancy_CenterOfMass.v;
                    var com = body.worldCenterOfMass;

                    var size = _buoyancyGizmoSettings.gizmoCenterOfMassSize;
                    Gizmos.DrawWireSphere(com, size);
                }

                var length = voxels.elementDatas.Length;

                if (length == 0)
                {
                    return;
                }

                var drawSelectedSubmersionPositions =
                    _buoyancyGizmoSettings.drawSelectedSubmersionPositions;
                var gizmoSubmersionBaseSize = _buoyancyGizmoSettings.gizmoSubmersionBaseSize;
                var gizmoSubmersionFlexSize = _buoyancyGizmoSettings.gizmoSubmersionFlexSize;

                var drawSelectedForcePositions = _buoyancyGizmoSettings.drawSelectedForcePositions;
                var gizmoForceBaseSize = _buoyancyGizmoSettings.gizmoForceBaseSize;
                var gizmoForceFlexSize = _buoyancyGizmoSettings.gizmoForceFlexSize;
                var gizmoForceSizeLimit = _buoyancyGizmoSettings.gizmoForceSizeLimit;

                if (drawSelectedForcePositions)
                {
                    Gizmos.color = ColorPrefs.Instance.Buoyancy_ForcePositions.v;

                    for (var i = 0; i < length; i++)
                    {
                        var voxel = voxels.voxels[i];
                        var buoyancyVoxel = voxels.elementDatas[i];

                        var water = voxel.worldPosition.value;
                        water.y -= buoyancyVoxel.distanceToSurface;

                        var forceGizmoSize = gizmoForceBaseSize +
                                             (gizmoForceBaseSize *
                                              gizmoForceFlexSize *
                                              math.length(buoyancyVoxel.force));
                        forceGizmoSize = math.clamp(forceGizmoSize, 0f, gizmoForceSizeLimit);
                        SmartHandles.DrawWireCube(voxel.worldPosition.value, forceGizmoSize);
                    }
                }

                if (drawSelectedSubmersionPositions)
                {
                    Gizmos.color = ColorPrefs.Instance.Buoyancy_SubmersionPositions.v;

                    for (var i = 0; i < length; i++)
                    {
                        var voxel = voxels.voxels[i];
                        var buoyancyVoxel = voxels.elementDatas[i];

                        var water = voxel.worldPosition.value;
                        water.y -= buoyancyVoxel.distanceToSurface;

                        var submersionGizmoSize = gizmoSubmersionBaseSize +
                                                  (gizmoSubmersionFlexSize *
                                                   buoyancyVoxel.submersion.value);

                        SmartHandles.DrawWireCube(voxel.worldPosition.value, submersionGizmoSize);
                    }
                }

                if (_buoyancyGizmoSettings.drawSelectedWaterLines)
                {
                    for (var i = 0; i < length; i++)
                    {
                        var voxel = voxels.voxels[i];
                        var buoyancyVoxel = voxels.elementDatas[i];

                        var water = voxel.worldPosition.value;
                        water.y -= buoyancyVoxel.distanceToSurface;

                        if (_gizmo_waterLines == null)
                        {
                            _gizmo_waterLines = new Vector3[length * 2];
                        }

                        _gizmo_waterLines[i * 2] = voxel.worldPosition.value;
                        _gizmo_waterLines[(i * 2) + 1] = water;
                    }

                    Handles.color = ColorPrefs.Instance.Buoyancy_WaterLines.v;
                    Handles.DrawLines(_gizmo_waterLines);
                }

                var drawSelected_cumulatv_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedForceLines;
                var drawSelected_hydrosta_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedHydrostaticForceLines;
                var drawSelected_viscosWR_ForceLines = _buoyancyGizmoSettings
                   .drawSelectedViscousWaterResistanceForceLines;
                var drawSelected_presrDrg_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedPressureDragForceLines;
                var drawSelected_airResis_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedAirResistanceForceLines;
                var drawSelected_windResi_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedWindResistanceForceLines;
                var drawSelected_waveDrft_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedWaveDriftingForceLines;
                var drawSelected_slamming_ForceLines =
                    _buoyancyGizmoSettings.drawSelectedSlammingForceLines;

                var drawAnyForceLine =
                    ((buoyancyData.buoyancyType == BuoyancyType.Physical) ||
                     (buoyancyData.buoyancyType == BuoyancyType.PhysicalVoxel)) &&
                    (drawSelected_cumulatv_ForceLines ||
                     drawSelected_hydrosta_ForceLines ||
                     drawSelected_viscosWR_ForceLines ||
                     drawSelected_presrDrg_ForceLines ||
                     drawSelected_airResis_ForceLines ||
                     drawSelected_windResi_ForceLines ||
                     drawSelected_waveDrft_ForceLines ||
                     drawSelected_slamming_ForceLines);

                if (drawAnyForceLine)
                {
                    var lineScale =
                        Vector3.one * _buoyancyGizmoSettings.drawSelectedForceLinesScale;
                    var lineOffset = float3c.forward_right *
                                     _buoyancyGizmoSettings.drawSelectedForceLinesOffset;

                    for (var i = 0; i < length; i++)
                    {
                        var voxel = voxels.voxels[i];
                        var buoyancyVoxel = voxels.elementDatas[i];

                        var water = voxel.worldPosition.value;
                        water.y -= buoyancyVoxel.distanceToSurface;

                        if (drawSelected_cumulatv_ForceLines &&
                            (_gizmo_cumulatv_ForceLines == null))
                        {
                            _gizmo_cumulatv_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_hydrosta_ForceLines &&
                            (_gizmo_hydrosta_ForceLines == null))
                        {
                            _gizmo_hydrosta_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_viscosWR_ForceLines &&
                            (_gizmo_viscosWR_ForceLines == null))
                        {
                            _gizmo_viscosWR_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_presrDrg_ForceLines &&
                            (_gizmo_presrDrg_ForceLines == null))
                        {
                            _gizmo_presrDrg_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_airResis_ForceLines &&
                            (_gizmo_airResis_ForceLines == null))
                        {
                            _gizmo_airResis_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_windResi_ForceLines &&
                            (_gizmo_windResi_ForceLines == null))
                        {
                            _gizmo_windResi_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_waveDrft_ForceLines &&
                            (_gizmo_waveDrft_ForceLines == null))
                        {
                            _gizmo_waveDrft_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_slamming_ForceLines &&
                            (_gizmo_slamming_ForceLines == null))
                        {
                            _gizmo_slamming_ForceLines = new Vector3[length * 2];
                        }

                        var indexA = i * 2;

                        var pos = voxel.worldPosition.value;

                        var cumulatv_base = pos + (lineOffset * 0);
                        var hydrosta_base = pos + (lineOffset * 1);
                        var viscosWR_base = pos + (lineOffset * 2);
                        var presrDrg_base = pos + (lineOffset * 3);
                        var airResis_base = pos + (lineOffset * 4);
                        var windResi_base = pos + (lineOffset * 5);
                        var waveDrft_base = pos + (lineOffset * 6);
                        var slamming_base = pos + (lineOffset * 7);

                        if (drawSelected_cumulatv_ForceLines)
                        {
                            _gizmo_cumulatv_ForceLines[indexA] = cumulatv_base;
                        }

                        if (drawSelected_hydrosta_ForceLines)
                        {
                            _gizmo_hydrosta_ForceLines[indexA] = hydrosta_base;
                        }

                        if (drawSelected_viscosWR_ForceLines)
                        {
                            _gizmo_viscosWR_ForceLines[indexA] = viscosWR_base;
                        }

                        if (drawSelected_presrDrg_ForceLines)
                        {
                            _gizmo_presrDrg_ForceLines[indexA] = presrDrg_base;
                        }

                        if (drawSelected_airResis_ForceLines)
                        {
                            _gizmo_airResis_ForceLines[indexA] = airResis_base;
                        }

                        if (drawSelected_windResi_ForceLines)
                        {
                            _gizmo_windResi_ForceLines[indexA] = windResi_base;
                        }

                        if (drawSelected_waveDrft_ForceLines)
                        {
                            _gizmo_waveDrft_ForceLines[indexA] = waveDrft_base;
                        }

                        if (drawSelected_slamming_ForceLines)
                        {
                            _gizmo_slamming_ForceLines[indexA] = slamming_base;
                        }

                        var indexB = indexA + 1;

                        if (drawSelected_cumulatv_ForceLines)
                        {
                            _gizmo_cumulatv_ForceLines[indexB] = cumulatv_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel.force /
                                                                         body.mass));
                        }

                        if (drawSelected_hydrosta_ForceLines)
                        {
                            _gizmo_hydrosta_ForceLines[indexB] = hydrosta_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .hydrostaticForce /
                                                                         body.mass));
                        }

                        if (drawSelected_viscosWR_ForceLines)
                        {
                            _gizmo_viscosWR_ForceLines[indexB] = viscosWR_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .viscousWaterResistanceForce /
                                                                         body.mass));
                        }

                        if (drawSelected_presrDrg_ForceLines)
                        {
                            _gizmo_presrDrg_ForceLines[indexB] = presrDrg_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .pressureDragForce /
                                                                         body.mass));
                        }

                        if (drawSelected_airResis_ForceLines)
                        {
                            _gizmo_airResis_ForceLines[indexB] = airResis_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .airResistanceForce /
                                                                         body.mass));
                        }

                        if (drawSelected_windResi_ForceLines)
                        {
                            _gizmo_windResi_ForceLines[indexB] = windResi_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .windResistanceForce /
                                                                         body.mass));
                        }

                        if (drawSelected_waveDrft_ForceLines)
                        {
                            _gizmo_waveDrft_ForceLines[indexB] = waveDrft_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel
                                                                            .waveDriftingForce /
                                                                         body.mass));
                        }

                        if (drawSelected_slamming_ForceLines)
                        {
                            _gizmo_slamming_ForceLines[indexB] = slamming_base +
                                                                 (lineScale *
                                                                     (buoyancyVoxel.slammingForce /
                                                                         body.mass));
                        }
                    }

                    if (drawSelected_cumulatv_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_cumulatvForceLines.v;
                        Handles.DrawLines(_gizmo_cumulatv_ForceLines);
                    }

                    if (drawSelected_hydrosta_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_hydrostaForceLines.v;
                        Handles.DrawLines(_gizmo_hydrosta_ForceLines);
                    }

                    if (drawSelected_viscosWR_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_viscosWRForceLines.v;
                        Handles.DrawLines(_gizmo_viscosWR_ForceLines);
                    }

                    if (drawSelected_presrDrg_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_presrDrgForceLines.v;
                        Handles.DrawLines(_gizmo_presrDrg_ForceLines);
                    }

                    if (drawSelected_airResis_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_airResisForceLines.v;
                        Handles.DrawLines(_gizmo_airResis_ForceLines);
                    }

                    if (drawSelected_windResi_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_windResiForceLines.v;
                        Handles.DrawLines(_gizmo_windResi_ForceLines);
                    }

                    if (drawSelected_waveDrft_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_waveDrftForceLines.v;
                        Handles.DrawLines(_gizmo_waveDrft_ForceLines);
                    }

                    if (drawSelected_slamming_ForceLines)
                    {
                        Handles.color = ColorPrefs.Instance.Buoyancy_slammingForceLines.v;
                        Handles.DrawLines(_gizmo_slamming_ForceLines);
                    }
                }
            }
        }

#endregion

#endif
    }
}
