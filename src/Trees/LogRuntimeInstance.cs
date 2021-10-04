#region

using Appalachia.Base.Behaviours;
using Appalachia.Core.Extensions;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Core.Metadata.Wood;
using Appalachia.Simulation.Physical.Integration;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR

#endif

#endregion

namespace Appalachia.Simulation.Trees
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RigidbodyDensityManager))]
    public class LogRuntimeInstance : InternalInstancedMonoBehaviour
    {
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

        [BoxGroup("Properties")]
        [ReadOnly]
        [ShowInInspector]
        public float mass => rigidbodyDensityManager.rb.mass;

        private void OnEnable()
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
                Debug.LogWarning(errorMessage, this);
                return;
#else
                throw new NotSupportedException(errorMessage. this);
#endif
            }

            var densityMetadata = wood.densityMetadata;

            if (densityMetadata == null)
            {
                var errorMessage = $"Need to assign density to this wood [{wood.name}]!";

#if UNITY_EDITOR
                Debug.LogWarning(errorMessage, this);
                return;
#else
                throw new NotSupportedException(errorMessage. this);
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

        protected override void UpdateInstancedProperties(MaterialPropertyBlock block, Material m)
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

        [Button]
        [EnableIf(nameof(_missingModel))]
        public void InitializeWithoutModel()
        {
            logName = name;

            var _renderer = GetComponentInChildren<Renderer>();

            var mesh = _renderer.GetSharedMesh();

            mesh.GetVolumeAndCenterOfMass(out volume, out centerOfMass);
        }

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private ILogModel _model;

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
            Selection.objects = new Object[] {_model.GameObject};
        }

        private void Update()
        {
            if (_model == null)
            {
                _model = GetComponentInParent<ILogModel>();
            }
        }
#endif
    }
}
