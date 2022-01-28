#if UNITY_EDITOR

#region

using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Filtering;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Preferences.Globals;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Core;
using Appalachia.Spatial.Voxels;
using Appalachia.Spatial.Voxels.Gizmos;
using Appalachia.Utility.Async;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEditor.SceneManagement;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Buoyancy
{
    public partial class Buoyant
    {
        #region Static Fields and Autoproperties

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [InlineEditor]
        private static BuoyancyVoxelDataGizmoSettings _buoyancyVoxelDataGizmoSettings;

        private static MainVoxelDataGizmoSettingsCollection _mainVoxelDataGizmoSettingsCollection;

        #endregion

        #region Fields and Autoproperties

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [OnValueChanged(nameof(DoDisableRenderers))]
        public bool disableRenderers;

        #endregion

        [ButtonGroup]
        [EnableIf(nameof(_canDirectRegister))]
        public void DirectRegister()
        {
            var waters = FindObjectsOfType<Water>();
            for (var i = 0; i < waters.Length; i++)
            {
                var water = waters[i];

                var voxelBounds = buoyancyData.voxelPopulationStyle == VoxelPopulationStyle.Colliders
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

        private static void RegisterEditorDependencies()
        {
            RegisterDependency<MainVoxelDataGizmoSettingsCollection>(
                i => _mainVoxelDataGizmoSettingsCollection = i
            );
            RegisterDependency<BuoyancyVoxelDataGizmoSettings>(i => _buoyancyVoxelDataGizmoSettings = i);
        }

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

        private async AppaTask InitializeEditor(Initializer initializer)
        {
            using (_PRF_InitializeEditor.Auto())
            {
                if (buoyancyData == null)
                {
                    var mesh = _meshObjectManager.GetCheapestMesh(gameObject);

                    AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(mesh, out var key, out var _);

                    buoyancyData = _mainBuoyancyDataCollection.Lookup.GetOrLoadOrCreateNew(
                        key,
                        ZString.Format("{0}_{1}", mesh.name, key)
                    );

                    buoyancyData.mesh = mesh;

                    buoyancyData.MarkAsModified();
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
                else if (buoyancyData.mesh == null)
                {
                    buoyancyData.mesh = _meshObjectManager.GetCheapestMesh(gameObject);

                    buoyancyData.MarkAsModified();
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }

                await AppaTask.CompletedTask;
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_InitializeEditor =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeEditor));

        #endregion

        #region Gizmos

        [FoldoutGroup("Gizmos")]
        [NonSerialized]
        [ShowInInspector]
        [SmartLabel]
        [InlineEditor]
        private VoxelDataGizmoSettings _voxelGizmoSettings;

        private void OnDrawGizmos()
        {
            using (_PRF_OnDrawGizmos.Auto())
            {
                if (!enabled)
                {
                    return;
                }

                if (!_buoyancyVoxelDataGizmoSettings.drawGizmos ||
                    !_buoyancyVoxelDataGizmoSettings.drawSelectedGizmos)
                {
                    return;
                }

                if (!_initialized)
                {
                    return;
                }

                var position = body.worldCenterOfMass;
                if (_buoyancyVoxelDataGizmoSettings.drawCumulativeForceGizmos)
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

                if (_buoyancyVoxelDataGizmoSettings.drawCumulativeWaterLevelGizmos)
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

        private static readonly ProfilerMarker _PRF_CleanUpEditor =
            new ProfilerMarker(_PRF_PFX + nameof(CleanUpEditor));

        private static readonly ProfilerMarker _PRF_WhenEnabledEditor =
            new ProfilerMarker(_PRF_PFX + nameof(WhenEnabledEditor));

        private static readonly ProfilerMarker _PRF_WhenDisabledEditor =
            new ProfilerMarker(_PRF_PFX + nameof(WhenDisabledEditor));

        private void WhenDisabledEditor()
        {
            using (_PRF_WhenDisabledEditor.Auto())
            {
                if (AppalachiaApplication.IsCompiling || AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    CleanUp();
                }
            }
        }

        private bool _canDirectRegister => PhysicsSimulator.IsSimulationActive && (_water == null);

        private void WhenEnabledEditor()
        {
            using (_PRF_WhenEnabledEditor.Auto())
            {
                PhysicsSimulator.onSimulationEnd -= Disable;
                PhysicsSimulator.onSimulationEnd += Disable;
            }
        }

        private void CleanUpEditor()
        {
            using (_PRF_CleanUpEditor.Auto())
            {
                PhysicsSimulator.onSimulationEnd -= Disable;

                _gizmo_cumulatv_ForceLines = default;
                _gizmo_waterLines = default;
                _gizmo_hydrosta_ForceLines = default;
                _gizmo_viscosWR_ForceLines = default;
                _gizmo_presrDrg_ForceLines = default;
                _gizmo_airResis_ForceLines = default;
                _gizmo_windResi_ForceLines = default;
                _gizmo_waveDrft_ForceLines = default;
                _gizmo_slamming_ForceLines = default;
            }
        }

        private void OnDrawGizmosSelected()
        {
            using (_PRF_OnDrawGizmosSelected.Auto())
            {
                if (!enabled)
                {
                    return;
                }

                if (!_buoyancyVoxelDataGizmoSettings.drawGizmos ||
                    !_buoyancyVoxelDataGizmoSettings.drawSelectedGizmos)
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
                    var lookup = _mainVoxelDataGizmoSettingsCollection.Lookup;

                    _voxelGizmoSettings = lookup.GetOrLoadOrCreateNew(
                        VoxelDataGizmoStyle.Buoyancy,
                        nameof(VoxelDataGizmoStyle.Buoyancy)
                    );
                }

                voxels.DrawGizmos(_voxelGizmoSettings);

                if (_buoyancyVoxelDataGizmoSettings.drawSelectedCenterOfMass)
                {
                    Gizmos.color = ColorPrefs.Instance.Buoyancy_CenterOfMass.v;
                    var com = body.worldCenterOfMass;

                    var size = _buoyancyVoxelDataGizmoSettings.gizmoCenterOfMassSize;
                    Gizmos.DrawWireSphere(com, size);
                }

                var length = voxels.elementDatas.Length;

                if (length == 0)
                {
                    return;
                }

                var drawSelectedSubmersionPositions =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedSubmersionPositions;
                var gizmoSubmersionBaseSize = _buoyancyVoxelDataGizmoSettings.gizmoSubmersionBaseSize;
                var gizmoSubmersionFlexSize = _buoyancyVoxelDataGizmoSettings.gizmoSubmersionFlexSize;

                var drawSelectedForcePositions = _buoyancyVoxelDataGizmoSettings.drawSelectedForcePositions;
                var gizmoForceBaseSize = _buoyancyVoxelDataGizmoSettings.gizmoForceBaseSize;
                var gizmoForceFlexSize = _buoyancyVoxelDataGizmoSettings.gizmoForceFlexSize;
                var gizmoForceSizeLimit = _buoyancyVoxelDataGizmoSettings.gizmoForceSizeLimit;

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
                                                  (gizmoSubmersionFlexSize * buoyancyVoxel.submersion.value);

                        SmartHandles.DrawWireCube(voxel.worldPosition.value, submersionGizmoSize);
                    }
                }

                if (_buoyancyVoxelDataGizmoSettings.drawSelectedWaterLines)
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

                    UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_WaterLines.v;
                    UnityEditor.Handles.DrawLines(_gizmo_waterLines);
                }

                var drawSelected_cumulatv_ForceLines = _buoyancyVoxelDataGizmoSettings.drawSelectedForceLines;
                var drawSelected_hydrosta_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedHydrostaticForceLines;
                var drawSelected_viscosWR_ForceLines = _buoyancyVoxelDataGizmoSettings
                   .drawSelectedViscousWaterResistanceForceLines;
                var drawSelected_presrDrg_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedPressureDragForceLines;
                var drawSelected_airResis_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedAirResistanceForceLines;
                var drawSelected_windResi_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedWindResistanceForceLines;
                var drawSelected_waveDrft_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedWaveDriftingForceLines;
                var drawSelected_slamming_ForceLines =
                    _buoyancyVoxelDataGizmoSettings.drawSelectedSlammingForceLines;

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
                    var lineScale = Vector3.one * _buoyancyVoxelDataGizmoSettings.drawSelectedForceLinesScale;
                    var lineOffset = float3c.forward_right *
                                     _buoyancyVoxelDataGizmoSettings.drawSelectedForceLinesOffset;

                    for (var i = 0; i < length; i++)
                    {
                        var voxel = voxels.voxels[i];
                        var buoyancyVoxel = voxels.elementDatas[i];

                        var water = voxel.worldPosition.value;
                        water.y -= buoyancyVoxel.distanceToSurface;

                        if (drawSelected_cumulatv_ForceLines && (_gizmo_cumulatv_ForceLines == null))
                        {
                            _gizmo_cumulatv_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_hydrosta_ForceLines && (_gizmo_hydrosta_ForceLines == null))
                        {
                            _gizmo_hydrosta_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_viscosWR_ForceLines && (_gizmo_viscosWR_ForceLines == null))
                        {
                            _gizmo_viscosWR_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_presrDrg_ForceLines && (_gizmo_presrDrg_ForceLines == null))
                        {
                            _gizmo_presrDrg_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_airResis_ForceLines && (_gizmo_airResis_ForceLines == null))
                        {
                            _gizmo_airResis_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_windResi_ForceLines && (_gizmo_windResi_ForceLines == null))
                        {
                            _gizmo_windResi_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_waveDrft_ForceLines && (_gizmo_waveDrft_ForceLines == null))
                        {
                            _gizmo_waveDrft_ForceLines = new Vector3[length * 2];
                        }

                        if (drawSelected_slamming_ForceLines && (_gizmo_slamming_ForceLines == null))
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
                                                                  (buoyancyVoxel.force / body.mass));
                        }

                        if (drawSelected_hydrosta_ForceLines)
                        {
                            _gizmo_hydrosta_ForceLines[indexB] = hydrosta_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.hydrostaticForce /
                                                                   body.mass));
                        }

                        if (drawSelected_viscosWR_ForceLines)
                        {
                            _gizmo_viscosWR_ForceLines[indexB] = viscosWR_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.viscousWaterResistanceForce /
                                                                   body.mass));
                        }

                        if (drawSelected_presrDrg_ForceLines)
                        {
                            _gizmo_presrDrg_ForceLines[indexB] = presrDrg_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.pressureDragForce /
                                                                   body.mass));
                        }

                        if (drawSelected_airResis_ForceLines)
                        {
                            _gizmo_airResis_ForceLines[indexB] = airResis_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.airResistanceForce /
                                                                   body.mass));
                        }

                        if (drawSelected_windResi_ForceLines)
                        {
                            _gizmo_windResi_ForceLines[indexB] = windResi_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.windResistanceForce /
                                                                   body.mass));
                        }

                        if (drawSelected_waveDrft_ForceLines)
                        {
                            _gizmo_waveDrft_ForceLines[indexB] = waveDrft_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.waveDriftingForce /
                                                                   body.mass));
                        }

                        if (drawSelected_slamming_ForceLines)
                        {
                            _gizmo_slamming_ForceLines[indexB] = slamming_base +
                                                                 (lineScale *
                                                                  (buoyancyVoxel.slammingForce / body.mass));
                        }
                    }

                    if (drawSelected_cumulatv_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_cumulatvForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_cumulatv_ForceLines);
                    }

                    if (drawSelected_hydrosta_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_hydrostaForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_hydrosta_ForceLines);
                    }

                    if (drawSelected_viscosWR_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_viscosWRForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_viscosWR_ForceLines);
                    }

                    if (drawSelected_presrDrg_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_presrDrgForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_presrDrg_ForceLines);
                    }

                    if (drawSelected_airResis_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_airResisForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_airResis_ForceLines);
                    }

                    if (drawSelected_windResi_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_windResiForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_windResi_ForceLines);
                    }

                    if (drawSelected_waveDrft_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_waveDrftForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_waveDrft_ForceLines);
                    }

                    if (drawSelected_slamming_ForceLines)
                    {
                        UnityEditor.Handles.color = ColorPrefs.Instance.Buoyancy_slammingForceLines.v;
                        UnityEditor.Handles.DrawLines(_gizmo_slamming_ForceLines);
                    }
                }
            }
        }

        #endregion
    }
}

#endif
