#region

using Appalachia.Core.Behaviours;
using Appalachia.Core.Shading;
using Appalachia.Simulation.Core.Metadata.Wood;
using Appalachia.Simulation.Physical.Integration;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Simulation.Trees.Core
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RigidbodyDensityManager))]
    public class LogRuntimeInstance : InstancedAppalachiaBehaviour
    {
        #region Fields and Autoproperties

        [FormerlySerializedAs("densityRigidbodyManager")]
        [FoldoutGroup("References")]
        public RigidbodyDensityManager rigidbodyDensityManager;

        [BoxGroup("Log")]
        [ReadOnly]
        public string logName;

        [BoxGroup("Log")]
        [ReadOnly]
        [HideInInspector]
        public int logID;

        [BoxGroup("Log")] public WoodSimulationData wood;

        /*[BoxGroup("Properties"), ReadOnly]
        public float effectiveScale;
        
        [BoxGroup("Properties"), ReadOnly]
        public float actualLength;
        
        [BoxGroup("Properties"), ReadOnly]
        public float actualDiameter;
        
        [BoxGroup("Properties"), ReadOnly]
        public Vector3 center = Vector3.zero;*/

        [BoxGroup("Properties")]
        [ReadOnly]
        public Vector3 centerOfMass = Vector3.zero;

        [BoxGroup("Properties")]
        [ReadOnly]
        public float volume;

        [BoxGroup("Instancing")]
        [PropertyRange(0f, 1f)]
        [OnValueChanged(nameof(UpdateAllInstancedProperties))]
        public float _Burned;

        [BoxGroup("Instancing")]
        [PropertyRange(0f, 1f)]
        [OnValueChanged(nameof(UpdateAllInstancedProperties))]
        public float _Heat;

        [BoxGroup("Instancing")]
        [PropertyRange(0f, 1f)]
        [OnValueChanged(nameof(UpdateAllInstancedProperties))]
        public float _Seasoned;

        [BoxGroup("Instancing")]
        [PropertyRange(0f, 1f)]
        [OnValueChanged(nameof(UpdateAllInstancedProperties))]
        public float _WindProtection;

        #endregion

        [BoxGroup("Properties")]
        [ReadOnly]
        [ShowInInspector]
        public float mass => rigidbodyDensityManager.rb.mass;

        #region Event Functions

        protected override void OnEnable()
        {
            using (_PRF_OnEnable.Auto())
            {
                if (rigidbodyDensityManager == null)
                {
                    rigidbodyDensityManager = GetComponent<RigidbodyDensityManager>();

                    if (rigidbodyDensityManager == null)
                    {
                        rigidbodyDensityManager = gameObject.AddComponent<RigidbodyDensityManager>();
                    }
                }

                if (wood == null)
                {
                    var errorMessage = $"Need to assign wood to this log [{name}]!";

#if UNITY_EDITOR
                    AppaLog.Warn(errorMessage, this);
                    return;
#else
                throw new NotSupportedException(errorMessage);
#endif
                }

                var densityMetadata = wood.densityMetadata;

                if (densityMetadata == null)
                {
                    var errorMessage = $"Need to assign density to this wood [{wood.name}]!";

#if UNITY_EDITOR
                    AppaLog.Warn(errorMessage, this);
                    return;
#else
                throw new NotSupportedException(errorMessage);
#endif
                }

#if UNITY_EDITOR

                if (densityMetadata.name != wood.name)
                {
                    densityMetadata.name = wood.name;
                    densityMetadata.UpdateName();
                }
#endif

                rigidbodyDensityManager.density = densityMetadata;
            }
        }

        #endregion

        [Button]
#if UNITY_EDITOR
        [EnableIf(nameof(_missingModel))]
#endif
        public void InitializeWithoutModel()
        {
            using (_PRF_InitializeWithoutModel.Auto())
            {
                logName = name;

                var _renderer = GetComponentInChildren<Renderer>();

                var mesh = _renderer.GetSharedMesh();

                mesh.GetVolumeAndCenterOfMass(out volume, out centerOfMass);
            }
        }

        protected override void UpdateInstancedProperties(MaterialPropertyBlock block, Material m)
        {
            using (_PRF_UpdateInstancedProperties.Auto())
            {
                if ((_Burned > 0) || (_Heat > 0))
                {
                    m.EnableKeyword(GSC.FIRE._BURNABLE_ON);
                }

                block.SetFloat(GSPL.Get(GSC.FIRE._Burned),         _Burned);
                block.SetFloat(GSPL.Get(GSC.FIRE._Heat),           _Heat);
                block.SetFloat(GSPL.Get(GSC.FIRE._Seasoned),       _Seasoned);
                block.SetFloat(GSPL.Get(GSC.FIRE._WindProtection), _WindProtection);
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(LogRuntimeInstance) + ".";

        private static readonly ProfilerMarker _PRF_InitializeWithoutModel =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeWithoutModel));

        private static readonly ProfilerMarker _PRF_UpdateInstancedProperties =
            new ProfilerMarker(_PRF_PFX + nameof(UpdateInstancedProperties));

        private static readonly ProfilerMarker
            _PRF_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(OnEnable));

        #endregion

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private LogModel _model;

        private bool _missingModel => (_model == null) || _model.MissingContainer;

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingModel))]
        public void OpenLog()
        {
            _model.OpenLog();
        }

        [Button(ButtonSizes.Medium)]
        [GUIColor(0.79f, 0.68f, 0.19f)]
        [PropertyOrder(-100)]
        [DisableIf(nameof(_missingModel))]
        public void SelectModel()
        {
            UnityEditor.Selection.objects = new Object[] {_model.GameObject};
        }

        private void Update()
        {
            if (_model == null)
            {
                _model = GetComponentInParent<LogModel>();
            }
        }
#endif
    }
}
