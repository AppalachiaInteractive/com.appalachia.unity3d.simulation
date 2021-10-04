#region

using System;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core
{
    [Serializable]
    public class RigidbodyData
    {
        private const string _PRF_PFX = nameof(RigidbodyData) + ".";

        private static readonly ProfilerMarker _PRF_RigidbodyData =
            new(_PRF_PFX + nameof(RigidbodyData));

        private static readonly ProfilerMarker _PRF_ApplyTo = new(_PRF_PFX + nameof(ApplyTo));

        private static readonly ProfilerMarker _PRF_GetFrom = new(_PRF_PFX + nameof(GetFrom));

        [ReadOnly]
        [SerializeField]
        [SmartLabel(Postfix = true)]
        public bool set;

        [SerializeField]
        [SmartLabel(Postfix = true)]
        public bool useGravity = true;

        [ShowIf(nameof(showMass))]
        [SerializeField]
        [SmartLabel]
        public float mass = 1.0f;

        [SerializeField]
        [SmartLabel]
        public RigidbodyConstraints constraints = RigidbodyConstraints.None;

        [SerializeField]
        [SmartLabel(Postfix = true)]
        public bool detectCollisions = true;

        [SerializeField]
        [SmartLabel]
        public float drag;

        [SerializeField]
        [SmartLabel]
        public float angularDrag;

        [SerializeField]
        [SmartLabel]
        public bool isKinematic;

        [SerializeField]
        [SmartLabel]
        public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;

        [HideInInspector]
        [SerializeField]
        [SmartLabel]
        public bool showMass = true;

        public RigidbodyData(bool showMass)
        {
            this.showMass = showMass;
        }

        public RigidbodyData(Rigidbody rigidbody)
        {
            using (_PRF_RigidbodyData.Auto())
            {
                useGravity = rigidbody.useGravity;
                angularDrag = rigidbody.angularDrag;
                mass = rigidbody.mass;
                constraints = rigidbody.constraints;
                detectCollisions = rigidbody.detectCollisions;
                drag = rigidbody.drag;
                isKinematic = rigidbody.isKinematic;
                interpolation = rigidbody.interpolation;
                set = true;
            }
        }

        public void ApplyTo(Rigidbody rigidbody)
        {
            using (_PRF_ApplyTo.Auto())
            {
                if (!set)
                {
                    throw new NotSupportedException();
                }

                rigidbody.useGravity = useGravity;
                rigidbody.angularDrag = angularDrag;
                rigidbody.mass = mass;
                rigidbody.constraints = constraints;
                rigidbody.detectCollisions = detectCollisions;
                rigidbody.drag = drag;
                rigidbody.isKinematic = isKinematic;
                rigidbody.interpolation = interpolation;
            }
        }

        public void GetFrom(Rigidbody rigidbody)
        {
            using (_PRF_GetFrom.Auto())
            {
                useGravity = rigidbody.useGravity;
                angularDrag = rigidbody.angularDrag;
                mass = rigidbody.mass;
                constraints = rigidbody.constraints;
                detectCollisions = rigidbody.detectCollisions;
                drag = rigidbody.drag;
                isKinematic = rigidbody.isKinematic;
                interpolation = rigidbody.interpolation;
                set = true;
            }
        }
    }
}
