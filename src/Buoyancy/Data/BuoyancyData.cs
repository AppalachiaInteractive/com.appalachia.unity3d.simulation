using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Scriptables;
using Appalachia.Spatial.Voxels;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    public class BuoyancyData : SelfSavingScriptableObject<BuoyancyData>
    {
        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWetnessAccumulationSpeed;

        [EnableIf(nameof(overrideDefaultWetnessAccumulationSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float wetnessAccumulationSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWetnessDispersalSpeed;

        [EnableIf(nameof(overrideDefaultWetnessDispersalSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float wetnessDispersalSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmergednessAccumulationSpeed;

        [EnableIf(nameof(overrideDefaultSubmergednessAccumulationSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float submergednessAccumulationSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmergednessDispersalSpeed;

        [EnableIf(nameof(overrideDefaultSubmergednessDispersalSpeed))]
        [SmartLabel]
        [PropertyRange(0.00001f, 0.01f)]
        [SerializeField]
        public float submergednessDispersalSpeed;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultBuoyancyType;

        /// <summary>
        ///     type of buoyancy to calculate
        /// </summary>
        [EnableIf(nameof(overrideDefaultBuoyancyType))]
        [SmartLabel]
        [SerializeField]
        public BuoyancyType buoyancyType;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideVoxelPopulationStyle;

        /// <summary>
        ///     type of voxel population to use
        /// </summary>
        [EnableIf(nameof(overrideVoxelPopulationStyle))]
        [SmartLabel]
        [SerializeField]
        public VoxelPopulationStyle voxelPopulationStyle;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultVoxelResolution;

        /// <summary>
        ///     voxel resolution, represents the half size of a voxel when creating the voxel representation
        /// </summary>
        [EnableIf(nameof(overrideDefaultVoxelResolution))]
        [SmartLabel]
        [PropertyRange(0.01f, 3f)]
        [SerializeField]
        public float voxelResolution;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmersionEngageSmoothing;

        [EnableIf(nameof(overrideDefaultSubmersionEngageSmoothing))]
        [SmartLabel]
        [PropertyRange(0.01f, 1.0f)]
        [SerializeField]
        public float submersionEngageSmoothing;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultSubmersionDisengageSmoothing;

        [EnableIf(nameof(overrideDefaultSubmersionDisengageSmoothing))]
        [SmartLabel]
        [PropertyRange(0.01f, 1.0f)]
        [SerializeField]
        public float submersionDisengageSmoothing;

        [SmartLabel(Postfix = true)]
        [SerializeField]
        public bool overrideDefaultWaterLevelOffset;

        [EnableIf(nameof(_enableWaterLevelOffset))]
        [PropertyRange(-10f, 10f)]
        [SerializeField]
        public float waterLevelOffset;

        [SerializeField] public Vector3 centerOfMassOffset;

        [SerializeField] private Mesh _mesh;
        [SerializeField] private string _meshGUID;

        private bool _enableWaterLevelOffset => overrideDefaultWaterLevelOffset;

        public Mesh mesh
        {
            get => _mesh;
#if UNITY_EDITOR
            set
            {
                _mesh = value;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_mesh, out _meshGUID, out long _);
            }
#endif
        }

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

                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_mesh, out _meshGUID, out long _);
#endif
                return _meshGUID;
            }
        }

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
                submergednessAccumulationSpeed =
                    BuoyancyDataDefaults.instance.submergednessAccumulationSpeed;
            }

            if (!overrideDefaultSubmergednessDispersalSpeed)
            {
                submergednessDispersalSpeed =
                    BuoyancyDataDefaults.instance.submergednessDispersalSpeed;
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
                submersionDisengageSmoothing =
                    BuoyancyDataDefaults.instance.submersionDisengageSmoothing;
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
