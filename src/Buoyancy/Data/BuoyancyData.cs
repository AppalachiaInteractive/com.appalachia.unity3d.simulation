using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Scriptables;
using Appalachia.Spatial.Voxels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class BuoyancyData : AppalachiaObject
    {
        #region Fields and Autoproperties

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultBuoyancyType;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmergednessAccumulationSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmergednessDispersalSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmersionDisengageSmoothing;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmersionEngageSmoothing;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultVoxelResolution;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWaterLevelOffset;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWetnessAccumulationSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWetnessDispersalSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideVoxelPopulationStyle;

        /// <summary>
        ///     type of buoyancy to calculate
        /// </summary>
        [EnableIf(nameof(overrideDefaultBuoyancyType))]
        [SmartLabel]
        [SerializeField]
        public BuoyancyType buoyancyType;

        [EnableIf(nameof(overrideDefaultSubmergednessAccumulationSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float submergednessAccumulationSpeed;

        [EnableIf(nameof(overrideDefaultSubmergednessDispersalSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float submergednessDispersalSpeed;

        [EnableIf(nameof(overrideDefaultSubmersionDisengageSmoothing))]
        [SmartLabel]
        [PropertyRange(0.01f, 1.0f)]
        [SerializeField]
        public float submersionDisengageSmoothing;

        [EnableIf(nameof(overrideDefaultSubmersionEngageSmoothing))]
        [SmartLabel]
        [PropertyRange(0.01f, 1.0f)]
        [SerializeField]
        public float submersionEngageSmoothing;

        /// <summary>
        ///     voxel resolution, represents the half size of a voxel when creating the voxel representation
        /// </summary>
        [EnableIf(nameof(overrideDefaultVoxelResolution))]
        [SmartLabel]
        [PropertyRange(0.01f, 3f)]
        [SerializeField]
        public float voxelResolution;

        [EnableIf(nameof(_enableWaterLevelOffset))]
        [PropertyRange(-10f, 10f)]
        [SerializeField]
        public float waterLevelOffset;

        [EnableIf(nameof(overrideDefaultWetnessAccumulationSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float wetnessAccumulationSpeed;

        [EnableIf(nameof(overrideDefaultWetnessDispersalSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float wetnessDispersalSpeed;

        [SerializeField] public Vector3 centerOfMassOffset;

        /// <summary>
        ///     type of voxel population to use
        /// </summary>
        [EnableIf(nameof(overrideVoxelPopulationStyle))]
        [SmartLabel]
        [SerializeField]
        public VoxelPopulationStyle voxelPopulationStyle;

        [SerializeField] private Mesh _mesh;
        [SerializeField] private string _meshGUID;

        #endregion

        public string meshGUID
        {
            get
            {
#if UNITY_EDITOR
                if (_mesh == null)
                {
                    _meshGUID = null;
                    return null;
                }

                AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(_mesh, out _meshGUID, out var _);
#endif
                return _meshGUID;
            }
        }

        public Mesh mesh
        {
            get => _mesh;
#if UNITY_EDITOR
            set
            {
                _mesh = value;
                AssetDatabaseManager.TryGetGUIDAndLocalFileIdentifier(_mesh, out _meshGUID, out var _);
            }
#endif
        }

        private bool _enableWaterLevelOffset => overrideDefaultWaterLevelOffset;

        [Button]
        public void Prepare()
        {
            if (!overrideDefaultWetnessAccumulationSpeed)
            {
                wetnessAccumulationSpeed = BuoyancyDataDefaults.instance.wetnessAccumulationSpeed;
            }

            if (!overrideDefaultWetnessDispersalSpeed)
            {
                wetnessDispersalSpeed = BuoyancyDataDefaults.instance.wetnessDispersalSpeed;
            }

            if (!overrideDefaultSubmergednessAccumulationSpeed)
            {
                submergednessAccumulationSpeed = BuoyancyDataDefaults.instance.submergednessAccumulationSpeed;
            }

            if (!overrideDefaultSubmergednessDispersalSpeed)
            {
                submergednessDispersalSpeed = BuoyancyDataDefaults.instance.submergednessDispersalSpeed;
            }

            if (!overrideDefaultBuoyancyType)
            {
                buoyancyType = BuoyancyDataDefaults.instance.buoyancyType;
            }

            if (!overrideVoxelPopulationStyle)
            {
                voxelPopulationStyle = BuoyancyDataDefaults.instance.voxelPopulationStyle;
            }

            if (!overrideDefaultVoxelResolution)
            {
                voxelResolution = BuoyancyDataDefaults.instance.voxelResolution;
            }

            if (!overrideDefaultSubmersionEngageSmoothing)
            {
                submersionEngageSmoothing = BuoyancyDataDefaults.instance.submersionEngageSmoothing;
            }

            if (!overrideDefaultSubmersionDisengageSmoothing)
            {
                submersionDisengageSmoothing = BuoyancyDataDefaults.instance.submersionDisengageSmoothing;
            }

            if (!overrideDefaultWaterLevelOffset)
            {
                waterLevelOffset = BuoyancyDataDefaults.instance.waterLevelOffset;
            }

            /*if (mesh == null)
            {
                return;
            }

            if (!mesh.data.isCreated)
            {
                mesh.CreateAndGetData(false);
            }*/
        }
    }
}
