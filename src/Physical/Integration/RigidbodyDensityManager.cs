using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Filtering;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Materials;
using Appalachia.Utility.Async;
using Appalachia.Utility.Extensions;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [CallStaticConstructorInEditor]
    public sealed class RigidbodyDensityManager : AppalachiaBehaviour<RigidbodyDensityManager>
    {
        static RigidbodyDensityManager()
        {
            RegisterDependency<PhysicsMaterialsCollection>(i => _physicsMaterialsCollection = i);
        }

        #region Static Fields and Autoproperties

        private static PhysicsMaterialsCollection _physicsMaterialsCollection;

        #endregion

        #region Fields and Autoproperties

        [BoxGroup("Density")]
        [OnValueChanged(nameof(InitializeSynchronous), true)]
        [HorizontalGroup("Density/A", .7f)]
        [HideLabel]
        [LabelWidth(0)]
        [InlineEditor]
        public DensityMetadata density;

        [HorizontalGroup("Density/B", .3f)]
        [SmartLabel(Postfix = true)]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public bool overrideDensity;

        [HorizontalGroup("Density/B", .7f)]
        [SmartLabel]
        [EnableIf(nameof(overrideDensity))]
        [PropertyRange(100f, 1500f)]
        [OnValueChanged(nameof(InitializeSynchronous))]
        public float densityKGPerCubicMeter = 1000f;

        [BoxGroup("Runtime")]
        [ReadOnly]
        [NonSerialized]
        [ShowInInspector]
        private float _volume;

        [BoxGroup("Runtime")]
        [ReadOnly]
        [NonSerialized]
        [ShowInInspector]
        public Vector3 volumeTakenAtScale;

        [BoxGroup("Runtime")]
        [ReadOnly]
        public Rigidbody rb;

        #endregion

        [BoxGroup("Runtime")]
        [ReadOnly]
        [ShowInInspector]
        public float mass => rb.mass;

        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                volumeTakenAtScale = rb.transform.lossyScale;
            }
        }

        private bool _canCreateDensity => density == null;

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
                        StaticContext.Log.Error("Cannot have density without colliders!");

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

                        material = _physicsMaterialsCollection.Lookup(mat);

                        if (material != null)
                        {
                            break;
                        }
                    }

                    if (material == null)
                    {
                        StaticContext.Log.Error(
                            "Need to set collider materials!  Using default material & density for now...",
                            densityManager
                        );
                        material = _physicsMaterialsCollection.defaultValue;
                    }

                    densityManager.density = material.defaultDensity;
                }

                return densityManager;
            }
        }

        [HorizontalGroup("Density/A", .3f)]
        [Button]
        [EnableIf(nameof(_canCreateDensity))]
        public void CreateNewDensity()
        {
#if UNITY_EDITOR
            using (_PRF_CreateNewDensity.Auto())
            {
                density = DensityMetadata.LoadOrCreateNew(gameObject.name);
            }
#endif
        }

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

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

            if ((_volume == 0f) || scaleChanged)
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

        #region Profiling

#if UNITY_EDITOR
        private static readonly ProfilerMarker _PRF_CreateNewDensity =
            new(_PRF_PFX + nameof(CreateNewDensity));
#endif
        private static readonly ProfilerMarker _PRF_CreateNow = new(_PRF_PFX + nameof(CreateNow));

        #endregion
    }
}
