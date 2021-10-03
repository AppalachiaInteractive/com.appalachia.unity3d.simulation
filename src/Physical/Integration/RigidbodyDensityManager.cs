using System;
using Appalachia.Core.Editing;
using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Editing.Behaviours;
using Appalachia.Core.Extensions;
using Appalachia.Filtering;
using Appalachia.Simulation.Physical.Density;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class RigidbodyDensityManager : EditorOnlyMonoBehaviour
    {
        private const string _PRF_PFX = nameof(RigidbodyDensityManager) + ".";
        
        [BoxGroup("Density"), OnValueChanged(nameof(Initialize), true)]
#if UNITY_EDITOR
        [HorizontalGroup("Density/A", .7f), HideLabel, LabelWidth(0), InlineEditor()]
#else
        [InlineEditor]
#endif
        public DensityMetadata density;
        
#if UNITY_EDITOR

        public override EditorOnlyExclusionStyle exclusionStyle => EditorOnlyExclusionStyle.Component;
        
        private bool _canCreateDensity => density == null;

        private static readonly ProfilerMarker _PRF_CreateNewDensity = new ProfilerMarker(_PRF_PFX + nameof(CreateNewDensity));
        [HorizontalGroup("Density/A", .3f)]
        [Button, EnableIf(nameof(_canCreateDensity))]
        public void CreateNewDensity()
        {
            using (_PRF_CreateNewDensity.Auto())
            {
                density = DensityMetadata.LoadOrCreateNew(gameObject.name);
            }
        }
#endif

        [HorizontalGroup("Density/B", .3f), SmartLabel(Postfix = true), OnValueChanged(nameof(Initialize))]
        public bool overrideDensity;

        [HorizontalGroup("Density/B", .7f), SmartLabel, EnableIf(nameof(overrideDensity)), PropertyRange(100f, 1500f), OnValueChanged(nameof(Initialize))]
        public float densityKGPerCubicMeter = 1000f;


        [BoxGroup("Runtime"), ReadOnly, NonSerialized, ShowInInspector]
        private float _volume;

        public float volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                volumeTakenAtScale = rb.transform.lossyScale;
            }
        }

        [BoxGroup("Runtime"), ReadOnly, NonSerialized, ShowInInspector]
        public Vector3 volumeTakenAtScale;

        [BoxGroup("Runtime"), ReadOnly]
        public Rigidbody rb;

        [BoxGroup("Runtime"), ReadOnly, ShowInInspector]
        public float mass => rb.mass;

        private static readonly ProfilerMarker _PRF_Internal_Awake = new ProfilerMarker(_PRF_PFX + nameof(Internal_Awake));
        protected override void Internal_Awake()
        {
            using (_PRF_Internal_Awake.Auto())
            {
                Initialize();
            }
        }

        private static readonly ProfilerMarker _PRF_Internal_OnEnable = new ProfilerMarker(_PRF_PFX + nameof(Internal_OnEnable));
        protected override void Internal_OnEnable()
        {
            using (_PRF_Internal_OnEnable.Auto())
            {
                Initialize();
            }
        }

        private static readonly ProfilerMarker _PRF_Initialize = new ProfilerMarker(_PRF_PFX + nameof(Initialize));
        [Button]
        public void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (rb == null)
                {
                    rb = GetComponent<Rigidbody>();
                }

                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }

                if (overrideDensity)
                {
                    rb.SetDensity(density.densityKGPerCubicMeter);
                    return;
                }

                var lossyScale = rb.transform.lossyScale;
                var scaleChanged = (lossyScale - volumeTakenAtScale).magnitude < .01f;
            
                if (_volume == 0f || scaleChanged)
                {
                    var originalMass = rb.mass;

                    rb.SetDensity(1.0f);
                    _volume = rb.mass;
                    volumeTakenAtScale = lossyScale;
                    rb.mass = originalMass;
                
                }

                if (density != null)
                {
                    var newMass = density.densityKGPerCubicMeter * _volume;
                
                    rb.mass = newMass;

                    //var scale = ;
                    //rb.mass = density.densityKGPerCubicMeter * volume * scale;
                }
            }
        }

        private static readonly ProfilerMarker _PRF_CreateNow = new ProfilerMarker(_PRF_PFX + nameof(CreateNow));
        public static RigidbodyDensityManager CreateNow(GameObject go)
        {
            using (_PRF_CreateNow.Auto())
            {
                var densityManager = go.GetComponent<RigidbodyDensityManager>();

                if (densityManager == null)
                {
                    densityManager = go.AddComponent<RigidbodyDensityManager>();
                }

                if (densityManager.rb == null)
                {
                    densityManager.rb = go.GetComponent<Rigidbody>();
            
                    if (densityManager.rb == null)
                    {
                        densityManager.rb = go.AddComponent<Rigidbody>();
                    }
                }

                if (densityManager.density == null)
                {
                    var colliders = go.FilterComponentsFromChildren<Collider>().NoTriggers().RunFilter();

                    if (colliders.Length == 0)
                    {
                        Debug.LogError("Cannot have density without colliders!");

                        densityManager.enabled = false;
                        densityManager.DestroySafely();
                        return null;
                    }

                    PhysicMaterialWrapper material = null;
                
                    for (var i = 0; i < colliders.Length; i++)
                    {
                        var c = colliders[i];

                        var mat = c.sharedMaterial;

                        if (mat == null)
                        {
                            continue;
                        }

                        material = PhysicsMaterials.instance.Lookup(mat);

                        if (material != null)
                        {
                            break;
                        }
                    }

                    if (material == null)
                    {
                        Debug.LogError("Need to set collider materials!  Using default material & density for now...", densityManager);
                        material = PhysicsMaterials.instance.defaultValue;
                    }

                    densityManager.density = material.defaultDensity;
                }
            
                densityManager.Initialize();

                return densityManager;
            }
        }
    }
}
