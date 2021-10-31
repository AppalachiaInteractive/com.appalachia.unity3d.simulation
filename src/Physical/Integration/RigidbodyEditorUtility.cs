#if UNITY_EDITOR
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Debugging;
using Appalachia.Editing.Core.Behaviours;
using Appalachia.Editing.Debugging;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyEditorUtility : EditorOnlyBehaviour
    {
        [HorizontalGroup("zzz")]
        [ReadOnly]
        [SmartLabel]
        public Vector3 maximumVelocity;

        [HorizontalGroup("zzz")]
        [ReadOnly]
        [SmartLabel]
        public Vector3 maximumAngularVelocity;

        [FoldoutGroup("Force")]
        [SmartLabel]
        [PropertyRange(1.0f, 100f)]
        public float force;

        [HorizontalGroup("Force/fa")]
        [SmartLabel]
        [ToggleLeft]
        public bool forceLocal;

        [HorizontalGroup("Force/fa")]
        [SmartLabel]
        [ToggleLeft]
        public bool relativeToMass = true;

        [HorizontalGroup("Force/fa")]
        [SmartLabel]
        [ToggleLeft]
        public bool showForceGizmo = true;

        [FoldoutGroup("Force")]
        [SmartLabel]
        [PropertyRange(-1f, 1f)]
        public float forceDirectionX;

        [FoldoutGroup("Force")]
        [SmartLabel]
        [PropertyRange(-1f, 1f)]
        public float forceDirectionY;

        [FoldoutGroup("Force")]
        [SmartLabel]
        [PropertyRange(-1f, 1f)]
        public float forceDirectionZ;

        private Rigidbody _rigidbody;
        private Vector3 firstPosition;

        private Vector3 originalPosition;

        private bool CanExecute
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                if (_rigidbody == null)
                {
                    return false;
                }

                if (!PhysicsSimulator.IsSimulationActive)
                {
                    return false;
                }

                return true;
            }
        }

        private bool EnablePhysicsSimulation_enable =>
            !Application.isPlaying && !PhysicsSimulator.IsSimulationActive;

        private bool DisablePhysicsSimulation_enable =>
            !Application.isPlaying && PhysicsSimulator.IsSimulationActive;

        private bool StopVelocity_enable => CanExecute && (_rigidbody.velocity.magnitude > 0f);

        private bool StopAngularVelocity_enable =>
            CanExecute && (_rigidbody.angularVelocity.magnitude > 0f);

        private bool StopMovement_enable =>
            CanExecute && (StopVelocity_enable || StopAngularVelocity_enable);

        private bool Reset_enable => CanExecute && !_rigidbody.IsSleeping();

        public override EditorOnlyExclusionStyle exclusionStyle =>
            EditorOnlyExclusionStyle.Component;

        private bool DisableGravity_enable => CanExecute && _rigidbody.useGravity;
        private bool EnableGravity_enable => CanExecute && !_rigidbody.useGravity;

        private bool Sleep_enable => CanExecute && !_rigidbody.IsSleeping();
        private bool WakeUp_enable => CanExecute && _rigidbody.IsSleeping();

        private bool AddForce_enable => CanExecute;
        private bool AddTorque_enable => CanExecute;
        private bool AddExposiveForce_enable => CanExecute;

        private void Update()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            if (firstPosition == Vector3.zero)
            {
                firstPosition = _rigidbody.position;
            }

            if (_rigidbody.IsSleeping())
            {
                originalPosition = _rigidbody.position;
            }
            else
            {
                var velocityMagnitude = _rigidbody.velocity.magnitude;
                var maximumVelocityMagnitude = maximumVelocity.magnitude;

                var angularVelocityMagnitude = _rigidbody.angularVelocity.magnitude;
                var maximumAngularVelocityMagnitude = maximumAngularVelocity.magnitude;

                if (velocityMagnitude > maximumVelocityMagnitude)
                {
                    maximumVelocity = _rigidbody.velocity;
                }

                if (angularVelocityMagnitude > maximumAngularVelocityMagnitude)
                {
                    maximumAngularVelocity = _rigidbody.angularVelocity;
                }

                if (_rigidbody.velocity.magnitude > 100)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;

                    ResetPosition();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showForceGizmo)
            {
                return;
            }

            if (!GizmoCameraChecker.ShouldRenderGizmos())
            {
                return;
            }

            var forceVector = GetForceVector();

            var pos = _transform.position;

            SmartHandles.DrawLine(pos, pos + forceVector, Color.red);
        }

        [ButtonGroup("P")]
        [EnableIf(nameof(EnablePhysicsSimulation_enable))]
        public void EnablePhysicsSimulation()
        {
            PhysicsSimulator.TogglePhysicsSimulation();
        }

        [ButtonGroup("P")]
        [EnableIf(nameof(DisablePhysicsSimulation_enable))]
        public void DisablePhysicsSimulation()
        {
            PhysicsSimulator.TogglePhysicsSimulation();
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopVelocity_enable))]
        public void StopVelocity()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.velocity = Vector3.zero;
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopAngularVelocity_enable))]
        public void StopSpin()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.angularVelocity = Vector3.zero;
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopMovement_enable))]
        public void StopMovement()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(Reset_enable))]
        protected override void Internal_Reset()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            StopMovement();
            ResetPosition();
        }

        [ButtonGroup("G")]
        [EnableIf(nameof(DisableGravity_enable))]
        public void DisableGravity()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.useGravity = false;
        }

        [ButtonGroup("G")]
        [EnableIf(nameof(EnableGravity_enable))]
        public void EnableGravity()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.useGravity = true;
        }

        [ButtonGroup("S")]
        [EnableIf(nameof(Sleep_enable))]
        public void Sleep()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.Sleep();
        }

        [ButtonGroup("S")]
        [EnableIf(nameof(WakeUp_enable))]
        public void WakeUp()
        {
            if (_rigidbody == null)
            {
                _rigidbody = _transform.GetComponent<Rigidbody>();
            }

            _rigidbody.WakeUp();
        }

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddForce_enable))]
        public void AddForce()
        {
            if (!_rigidbody.detectCollisions)
            {
                _rigidbody.detectCollisions = true;
            }

            if (!_rigidbody.useGravity)
            {
                _rigidbody.useGravity = true;
            }

            _rigidbody.AddForce(GetForceVector());
        }

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddTorque_enable))]
        public void AddTorque()
        {
            if (!_rigidbody.detectCollisions)
            {
                _rigidbody.detectCollisions = true;
            }

            if (!_rigidbody.useGravity)
            {
                _rigidbody.useGravity = true;
            }

            _rigidbody.AddRelativeTorque(GetForceVector());
        }

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddExposiveForce_enable))]
        public void AddExposiveForce()
        {
            if (!_rigidbody.detectCollisions)
            {
                _rigidbody.detectCollisions = true;
            }

            if (!_rigidbody.useGravity)
            {
                _rigidbody.useGravity = true;
            }

            _rigidbody.AddExplosionForce(GetForce() * 5f, Vector3.zero, 5f, 1.0f);
        }

        private float GetForce()
        {
            if (relativeToMass)
            {
                return _rigidbody.mass * force;
            }

            return force;
        }

        private Vector3 GetForceVector()
        {
            var forceVector = new Vector3(forceDirectionX, forceDirectionY, forceDirectionZ);

            if (forceVector.magnitude == 0)
            {
                forceVector = Vector3.forward;
            }

            if (forceLocal)
            {
                forceVector = _transform.TransformVector(forceVector);
            }

            forceVector = forceVector.normalized;
            forceVector *= GetForce();

            return forceVector;
        }

        private void ResetPosition()
        {
            if (firstPosition == Vector3.zero)
            {
                firstPosition = _rigidbody.position;
            }

            if (originalPosition != Vector3.zero)
            {
                _transform.position = originalPosition;
            }
            else if (firstPosition != Vector3.zero)
            {
                _transform.position = firstPosition;
            }
        }
    }
}

#endif
