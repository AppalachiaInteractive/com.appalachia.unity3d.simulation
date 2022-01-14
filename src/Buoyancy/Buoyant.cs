#region

using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Collections.Native;
using Appalachia.Core.Filtering;
using Appalachia.Core.Math.Smoothing;
using Appalachia.Core.Objects.Behaviours;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Shading;
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
using Appalachia.Utility.Async;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Simulation.Buoyancy
{
    [ExecuteAlways]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(RigidbodyDensityManager))]
    [ExecutionOrder(ExecutionOrders.Buoyant)]
    [CallStaticConstructorInEditor]
    public partial class Buoyant : InstancedAppalachiaBehaviour<Buoyant>
    {
        static Buoyant()
        {
            RegisterDependency<BuoyancyDataDefaults>(i => _buoyancyDataDefaults = i);
            RegisterDependency<WaterPhysicsCoefficients>(i => _waterPhysicsCoefficients = i);
            RegisterDependency<DensityMetadataCollection>(i => _densityMetadataCollection = i);
            RegisterDependency<MainBuoyancyVoxelsDataStoreLookup>(
                i => _mainBuoyancyVoxelsDataStoreLookup = i
            );
            RegisterDependency<MainBuoyancyDataCollection>(i => _mainBuoyancyDataCollection = i);

            RegisterDependency<GlobalWindManager>(i => _globalWindManager = i);
            RegisterDependency<MeshObjectManager>(i => _meshObjectManager = i);

#if UNITY_EDITOR
            RegisterEditorDependencies();
#endif
        }

        #region Static Fields and Autoproperties

        private static BuoyancyDataDefaults _buoyancyDataDefaults;
        private static DensityMetadataCollection _densityMetadataCollection;
        private static GlobalWindManager _globalWindManager;
        private static MainBuoyancyDataCollection _mainBuoyancyDataCollection;
        private static MainBuoyancyVoxelsDataStoreLookup _mainBuoyancyVoxelsDataStoreLookup;

        private static MeshObjectManager _meshObjectManager;

        [FoldoutGroup("Setup")]
        [SmartLabel]
        [InlineEditor]
        [ShowInInspector]
        [NonSerialized]
        private static WaterPhysicsCoefficients _waterPhysicsCoefficients;

        #endregion

        #region Fields and Autoproperties

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

        #endregion

        [FoldoutGroup("State")]
        [SmartLabel(Postfix = true)]
        [ReadOnly]
        [ShowInInspector]
        public bool submerged => percentageSubmerged > 0;

        public BuoyancyDataDefaults buoyancyDataDefaults => _defaultBuoyancyData;

        public DensityMetadata airDensityMetadata
        {
            get
            {
                if (_airDensityMetadata == null)
                {
                    _airDensityMetadata = _densityMetadataCollection.air;
                }

                return _airDensityMetadata;
            }
        }

        public DensityMetadata waterDensityMetadata
        {
            get
            {
                if (_waterDensityMetadata == null)
                {
                    _waterDensityMetadata = _densityMetadataCollection.water;
                }

                return _waterDensityMetadata;
            }
        }

        public WaterPhysicsCoefficients waterPhysicsCoefficients => _waterPhysicsCoefficients;

        #region Event Functions

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                if (ShouldSkipUpdate)
                {
                    return;
                }

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

        #endregion

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

        public void EnterWater(Water w)
        {
            _water = w;
            enabled = true;
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
                        windDynamicPressure = _globalWindManager.WindDynamicPressure;
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

                        wetness = math.lerp(wetness,                     wetnessTarget,    wetSpeed);
                        submersionWetness = math.lerp(submersionWetness, submersionTarget, submersionSpeed);
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
                            submersionDisengageSmoothing = buoyancyData.submersionDisengageSmoothing,
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
                            viscousWaterResistanceForce = voxels.objectData.viscousWaterResistanceForce,
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
                            pressureDragForce = voxels.objectData.pressureDragForce.GetParallel(),
                            airResistanceForce = voxels.objectData.airResistanceForce.GetParallel(),
                            windResistanceForce = voxels.objectData.windResistanceForce.GetParallel(),
                            waveDriftingForce = voxels.objectData.waveDriftingForce.GetParallel(),
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
                    Context.Log.Error(ex.Message, null, ex);
                    jobHandle.Complete();
                    throw;
                }
            }
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

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

#if UNITY_EDITOR
            await InitializeEditor(initializer);
#endif

            buoyancyData.Prepare();

            body = GetComponent<Rigidbody>();

            if (!TryGetComponent(out body))
            {
                body = gameObject.AddComponent<Rigidbody>();
                Context.Log.Error(
                    ZString.Format(
                        "Buoyancy:Object \"{0}\" had no Rigidbody. Rigidbody has been added.",
                        name
                    )
                );
            }

            gameObject.GetOrCreateComponent(ref densityManager);

            if (voxels != null)
            {
                voxels.SafeDispose();
            }

            body.ResetCenterOfMass();
            body.centerOfMass += buoyancyData.centerOfMassOffset;

            var persistence = _mainBuoyancyVoxelsDataStoreLookup.Lookup.GetOrLoadOrCreateNew(
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

                voxels = BuoyancyVoxels.VoxelizeSingle(voxels, _transform, bounds, body.centerOfMass);
            }

            voxels.centerOfMass = body.centerOfMass;

            jobHandle = voxels.SetupPhysical();

            _initialized = true;
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

        protected override async AppaTask WhenDestroyed()
        {
            await base.WhenDestroyed();

            using (_PRF_OnDestroy.Auto())
            {
                CleanUp();
            }
        }

        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            using (_PRF_WhenDisabled.Auto())
            {
#if UNITY_EDITOR
                WhenDisabledEditor();
#endif
                if ((originalBodyData == null) || (body == null))
                {
                    return;
                }

                originalBodyData.ApplyTo(body);
            }
        }

        protected override async AppaTask WhenEnabled()
        {
            using (_PRF_OnEnable.Auto())
            {
                await base.WhenEnabled();

                if (_water == null)
                {
                    enabled = false;
                }

#if UNITY_EDITOR
                WhenEnabledEditor();
#endif

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
                _defaultBuoyancyData = buoyancyDataDefaults;

                body.drag += waterPhysicsCoefficients.data.additionalDrag;
                body.angularDrag += waterPhysicsCoefficients.data.additionalAngularDrag;

                if (waterPhysicsCoefficients.data.disableGravity)
                {
                    body.useGravity = false;
                }
            }
        }

        private void CleanUp()
        {
            using (_PRF_CleanUp.Auto())
            {
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
                CleanUpEditor();
#endif
            }
        }

        private void Disable()
        {
            enabled = false;
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_CleanUp = new(_PRF_PFX + nameof(CleanUp));

        private static readonly ProfilerMarker _PRF_OnDestroy = new(_PRF_PFX + nameof(OnDestroy));
        private static readonly ProfilerMarker _PRF_OnDisable = new(_PRF_PFX + nameof(OnDisable));

        private static readonly ProfilerMarker _PRF_OnEnable = new(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs));

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_AggregateForceAndTorqueJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".AggregateForceAndTorqueJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CalculateForcesJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CalculateForcesJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CalculateSubmersionJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CalculateSubmersionJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CombinedDependencies =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CombinedDependencies");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_CompletePhysical =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".CompletePhysical");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_Initialize =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".Initialize");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_InitializeParameters =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".InitializeParameters");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_JobHandleComplete =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".JobHandleComplete");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_MaterialVariables =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".MaterialVariables");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_PositionDataUpdate =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".PositionDataUpdate");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_ResetCenterOfMass =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".ResetCenterOfMass");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_ResetForceAndTorqueJob =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".ResetForceAndTorqueJob");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_Synchronize =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".Synchronize");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_UpdatePhysical =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".UpdatePhysical");

        private static readonly ProfilerMarker _PRF_ScheduleBuoyancyJobs_WaterUpdateSamples =
            new(_PRF_PFX + nameof(ScheduleBuoyancyJobs) + ".WaterUpdateSamples");

        private static readonly ProfilerMarker _PRF_UpdateDrag = new(_PRF_PFX + nameof(UpdateDrag));

        #endregion
    }
}
