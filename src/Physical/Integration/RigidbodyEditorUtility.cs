#if UNITY_EDITOR
using Appalachia.Core.Editing;
using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Editing.Behaviours;
using Appalachia.Core.Editing.Handle;
using Appalachia.Simulation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyEditorUtility : EditorOnlyMonoBehaviour
    {
        private Rigidbody _rigidbody;

        [HorizontalGroup("zzz"), Sirenix.OdinInspector.ReadOnly, SmartLabel]
        public Vector3 maximumVelocity;
        [HorizontalGroup("zzz"), Sirenix.OdinInspector.ReadOnly, SmartLabel]
        public Vector3 maximumAngularVelocity;

        private bool CanExecute
        {
            get
            {
                if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
                if (_rigidbody == null) return false;
                
                if (!PhysicsSimulator.IsSimulationActive) return false;
                
                return true;
            }
        }
        
        private bool EnablePhysicsSimulation_enable =>  !Application.isPlaying && !PhysicsSimulator.IsSimulationActive;
        private bool DisablePhysicsSimulation_enable => !Application.isPlaying &&  PhysicsSimulator.IsSimulationActive;
        
        [ButtonGroup("P"), EnableIf(nameof(EnablePhysicsSimulation_enable))]
        public void EnablePhysicsSimulation()
        {
            PhysicsSimulator.TogglePhysicsSimulation();
        }
        
        [ButtonGroup("P"), EnableIf(nameof(DisablePhysicsSimulation_enable))]
        public void DisablePhysicsSimulation()
        {
            PhysicsSimulator.TogglePhysicsSimulation();
        }

        private bool StopVelocity_enable => CanExecute && _rigidbody.velocity.magnitude > 0f;
        private bool StopAngularVelocity_enable => CanExecute && _rigidbody.angularVelocity.magnitude > 0f;
        private bool StopMovement_enable => CanExecute && (StopVelocity_enable || StopAngularVelocity_enable);
        private bool Reset_enable => CanExecute && !_rigidbody.IsSleeping();
        
        [ButtonGroup("A"), EnableIf(nameof(StopVelocity_enable))]
        public void StopVelocity()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.velocity = Vector3.zero;
        }
        
        [ButtonGroup("A"), EnableIf(nameof(StopAngularVelocity_enable))]
        public void StopSpin()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.angularVelocity = Vector3.zero;
        }
        
        [ButtonGroup("A"), EnableIf(nameof(StopMovement_enable))]
        public void StopMovement()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        public override EditorOnlyExclusionStyle exclusionStyle => EditorOnlyExclusionStyle.Component;

        [ButtonGroup("A"), EnableIf(nameof(Reset_enable))]
        protected override void Internal_Reset()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            StopMovement();
            ResetPosition();
        }
        
        private bool DisableGravity_enable => CanExecute && _rigidbody.useGravity;
        private bool EnableGravity_enable => CanExecute && !_rigidbody.useGravity;
        
        [ButtonGroup("G"), EnableIf(nameof(DisableGravity_enable))]
        public void DisableGravity()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }
        
        [ButtonGroup("G"), EnableIf(nameof(EnableGravity_enable))]
        public void EnableGravity()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.useGravity = true;
        }
        
        private bool Sleep_enable => CanExecute && !_rigidbody.IsSleeping();
        private bool WakeUp_enable => CanExecute && _rigidbody.IsSleeping();
        
        [ButtonGroup("S"), EnableIf(nameof(Sleep_enable))]
        public void Sleep()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.Sleep();
        }
        
        [ButtonGroup("S"), EnableIf(nameof(WakeUp_enable))]
        public void WakeUp()
        {
            if (_rigidbody == null) _rigidbody = _transform.GetComponent<Rigidbody>();
            _rigidbody.WakeUp();
        }

        private bool AddForce_enable => CanExecute;
        private bool AddTorque_enable => CanExecute;
        private bool AddExposiveForce_enable => CanExecute;

        [FoldoutGroup("Force"), SmartLabel, PropertyRange(1.0f, 100f)] public float force;
        [HorizontalGroup("Force/fa"), SmartLabel, ToggleLeft] public bool forceLocal = false;
        [HorizontalGroup("Force/fa"), SmartLabel, ToggleLeft] public bool relativeToMass = true;
        [HorizontalGroup("Force/fa"), SmartLabel, ToggleLeft] public bool showForceGizmo = true;
        [FoldoutGroup("Force"), SmartLabel, PropertyRange(-1f, 1f)] public float forceDirectionX = 0f;
        [FoldoutGroup("Force"), SmartLabel, PropertyRange(-1f, 1f)] public float forceDirectionY = 0f;
        [FoldoutGroup("Force"), SmartLabel, PropertyRange(-1f, 1f)] public float forceDirectionZ = 0f;
        
        [ButtonGroup("Force/fB"), EnableIf(nameof(AddForce_enable))]
        public void AddForce()
        {
            if (!_rigidbody.detectCollisions) _rigidbody.detectCollisions = true;
            if (!_rigidbody.useGravity) _rigidbody.useGravity = true;

            _rigidbody.AddForce(GetForceVector());
        }
        
        [ButtonGroup("Force/fB"), EnableIf(nameof(AddTorque_enable))]
        public void AddTorque()
        {
            if (!_rigidbody.detectCollisions) _rigidbody.detectCollisions = true;
            if (!_rigidbody.useGravity) _rigidbody.useGravity = true;
            
            _rigidbody.AddRelativeTorque(GetForceVector());
        }
        
        [ButtonGroup("Force/fB"), EnableIf(nameof(AddExposiveForce_enable))]
        public void AddExposiveForce()
        {
            if (!_rigidbody.detectCollisions) _rigidbody.detectCollisions = true;
            if (!_rigidbody.useGravity) _rigidbody.useGravity = true;

            _rigidbody.AddExplosionForce(GetForce()*5f, Vector3.zero, 5f, 1.0f);
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
        
        private void OnDrawGizmosSelected()
        {
            if (!showForceGizmo) return;
            
            if (!GizmoCameraChecker.ShouldRenderGizmos())
            {
                return;
            }

            var forceVector = GetForceVector();
            
            var pos = _transform.position;
            
            SmartHandles.DrawLine(pos, pos + forceVector, Color.red);
        }

        private Vector3 originalPosition;
        private Vector3 firstPosition;

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