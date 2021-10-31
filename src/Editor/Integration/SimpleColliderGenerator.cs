using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Extensions;
using Appalachia.Core.Filtering;
using Appalachia.Core.Types.Enums;
using Appalachia.Editing.Core.Behaviours;
using Appalachia.Simulation.Core.Selections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Integration
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SimpleColliderGenerator : EditorOnlyFrustumCulledBehaviour
    {
        public override EditorOnlyExclusionStyle exclusionStyle => EditorOnlyExclusionStyle.Component;

        protected override void Internal_OnEnable()
        {
            materialSelector = LookupSelectionGenerator.CreatePhysicMaterialSelector(
                (mat) =>
                {
                    _collider.sharedMaterial = mat.material;
                }
            );
        }

        
        [FoldoutGroup("Setup"), SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public string childName = "COLLIDERS";

        [FoldoutGroup("Setup"), SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public Transform colliderTransform;
        
        [BoxGroup("Collider"), SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public PrimitiveColliderType colliderType;
        
        [BoxGroup("Collider"), SmartLabel, NonSerialized, ShowInInspector]
        [OnValueChanged(nameof(UpdateCollider))]
        public PhysicMaterialLookupSelection materialSelector;

        [BoxGroup("Collider")]
        [SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public bool isTrigger;

        [BoxGroup("Collider"), SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public Vector3 scale = Vector3.one;

        [BoxGroup("Collider"), SmartLabel]
        [OnValueChanged(nameof(UpdateCollider))]
        public Vector3 offset = Vector3.zero;

        [BoxGroup("Collider Detail;")]
        [InlineEditor, SmartLabel, SmartLabelChildren]
        public Collider _collider;

        [Button]
        public void UpdateCollider()
        {
            if (scale == Vector3.zero)
            {
                scale = Vector3.one;
            }
            
            if (colliderTransform == null)
            {
                colliderTransform = _transform.Find(childName);
            }

            if (colliderTransform == null)
            {
                var child = new GameObject(childName).transform;
                child.parent = _transform;

                colliderTransform = child;
            }

            if (colliderTransform == null)
            {
                return;
            }
            
            if (_collider == null)
            {
                _collider = colliderTransform.GetComponent<Collider>();
            }

            if (_collider != null)
            {
                switch (colliderType)
                {
                    case PrimitiveColliderType.Sphere:
                        if (_collider is SphereCollider)
                        {
                            break;
                        }
                        
                        _collider.DestroySafely();
                        break;
                    case PrimitiveColliderType.Cube:
                        if (_collider is BoxCollider)
                        {
                            break;
                        }
                        
                        _collider.DestroySafely();
                        break;
                    case PrimitiveColliderType.Capsule:
                        if (_collider is CapsuleCollider)
                        {
                            break;
                        }
                        
                        _collider.DestroySafely();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_collider == null)
            {
                switch (colliderType)
                {
                    case PrimitiveColliderType.Sphere:
                        _collider = colliderTransform.gameObject.AddComponent<SphereCollider>();
                        break;
                    case PrimitiveColliderType.Cube:
                        _collider = colliderTransform.gameObject.AddComponent<BoxCollider>();
                        break;
                    case PrimitiveColliderType.Capsule:
                        _collider = colliderTransform.gameObject.AddComponent<CapsuleCollider>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _collider.isTrigger = isTrigger;
            
            FitToBounds();
        }

        [Button]
        public void FitToBounds()
        {
            var r = _transform.FilterComponents<Renderer>(true).CheapestRenderer();
            var mf = r.GetComponent<MeshFilter>();
            var rTransform = r.transform;
            colliderTransform.position = rTransform.position;
            colliderTransform.rotation = rTransform.rotation;
            colliderTransform.localScale = rTransform.localScale;

            var bounds = mf.sharedMesh.bounds;

            bounds.size = Vector3.Scale(bounds.size, scale);
            
            var localCenter = bounds.center + offset;
            var size = bounds.size;
            var diameter = size.magnitude;
            
            switch (colliderType)
            {
                case PrimitiveColliderType.Sphere:
                    if (_collider is SphereCollider sc)
                    {
                        sc.center = localCenter;
                        sc.radius = .5f*diameter;
                    }
                    
                    return;

                case PrimitiveColliderType.Cube:
                    if (_collider is BoxCollider bc)
                    {
                        bc.center = localCenter;
                        bc.size = size;
                    }
                    
                    return;
                case PrimitiveColliderType.Capsule:
                    if (_collider is CapsuleCollider cc)
                    {
                        cc.center = localCenter;
                        
                        if ((size.x > size.y) && (size.x > size.z))
                        {
                            cc.direction = 0;
                            cc.height = size.x;
                            cc.radius = size.y > size.z ? size.y : size.z;
                        }
                        else if ((size.y > size.x) && (size.y > size.z))
                        {
                            cc.direction = 1;
                            cc.height = size.y;
                            cc.radius = size.x > size.z ? size.x : size.z;
                        }
                        else
                        {
                            cc.direction = 2;
                            cc.height = size.z;
                            cc.radius = size.y > size.x ? size.y : size.x;
                        }

                        cc.radius *= .5f;
                    }
                    
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
