using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Root;
using Appalachia.Spatial.Voxels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class BuoyancyData : AppalachiaObject<BuoyancyData>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static BuoyancyData()
        {
            RegisterDependency<BuoyancyDataDefaults>(i => _buoyancyDataDefaults = i);
        }

        #region Static Fields and Autoproperties

        private static BuoyancyDataDefaults _buoyancyDataDefaults;

        #endregion

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
                wetnessAccumulationSpeed = _buoyancyDataDefaults.wetnessAccumulationSpeed;
            }

            if (!overrideDefaultWetnessDispersalSpeed)
            {
                wetnessDispersalSpeed = _buoyancyDataDefaults.wetnessDispersalSpeed;
            }

            if (!overrideDefaultSubmergednessAccumulationSpeed)
            {
                submergednessAccumulationSpeed = _buoyancyDataDefaults.submergednessAccumulationSpeed;
            }

            if (!overrideDefaultSubmergednessDispersalSpeed)
            {
                submergednessDispersalSpeed = _buoyancyDataDefaults.submergednessDispersalSpeed;
            }

            if (!overrideDefaultBuoyancyType)
            {
                buoyancyType = _buoyancyDataDefaults.buoyancyType;
            }

            if (!overrideVoxelPopulationStyle)
            {
                voxelPopulationStyle = _buoyancyDataDefaults.voxelPopulationStyle;
            }

            if (!overrideDefaultVoxelResolution)
            {
                voxelResolution = _buoyancyDataDefaults.voxelResolution;
            }

            if (!overrideDefaultSubmersionEngageSmoothing)
            {
                submersionEngageSmoothing = _buoyancyDataDefaults.submersionEngageSmoothing;
            }

            if (!overrideDefaultSubmersionDisengageSmoothing)
            {
                submersionDisengageSmoothing = _buoyancyDataDefaults.submersionDisengageSmoothing;
            }

            if (!overrideDefaultWaterLevelOffset)
            {
                waterLevelOffset = _buoyancyDataDefaults.waterLevelOffset;
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
