using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Debugging;
using Appalachia.Core.Objects.Root;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.Core;
using Appalachia.Utility.Execution;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.Physical.Integration
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class RigidbodyUtility : AppalachiaBehaviour<RigidbodyUtility>
    {
        #region Fields and Autoproperties

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

        #endregion

        private bool AddExposiveForce_enable => CanExecute;

        private bool AddForce_enable => CanExecute;
        private bool AddTorque_enable => CanExecute;

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

#if UNITY_EDITOR
                if (!PhysicsSimulator.IsSimulationActive)
                {
                    return false;
                }

#endif
                return true;
            }
        }

        private bool DisableGravity_enable => CanExecute && _rigidbody.useGravity;

        private bool DisablePhysicsSimulation_enable
        {
            get
            {
                return !AppalachiaApplication.IsPlayingOrWillPlay
#if UNITY_EDITOR
                     &&
                       PhysicsSimulator.IsSimulationActive
#endif
                    ;
            }
        }

        private bool EnableGravity_enable => CanExecute && !_rigidbody.useGravity;

        private bool EnablePhysicsSimulation_enable
        {
            get
            {
                return !AppalachiaApplication.IsPlayingOrWillPlay
#if UNITY_EDITOR
                     &&
                       !PhysicsSimulator.IsSimulationActive
#endif
                    ;
            }
        }

        private bool Reset_enable => CanExecute && !_rigidbody.IsSleeping();

        private bool Sleep_enable => CanExecute && !_rigidbody.IsSleeping();

        private bool StopAngularVelocity_enable => CanExecute && (_rigidbody.angularVelocity.magnitude > 0f);

        private bool StopMovement_enable => CanExecute && (StopVelocity_enable || StopAngularVelocity_enable);

        private bool StopVelocity_enable => CanExecute && (_rigidbody.velocity.magnitude > 0f);
        private bool WakeUp_enable => CanExecute && _rigidbody.IsSleeping();

        #region Event Functions

        [ButtonGroup("A")]
        [EnableIf(nameof(Reset_enable))]
        protected override void Reset()
        {
            using (_PRF_Reset.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                StopMovement();
                ResetPosition();
            }
        }

        private void Update()
        {
            using (_PRF_Update.Auto())
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
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            using (_PRF_OnDrawGizmosSelected.Auto())
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
#endif
        }

        #endregion

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddExposiveForce_enable))]
        public void AddExposiveForce()
        {
            using (_PRF_AddExposiveForce.Auto())
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
        }

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddForce_enable))]
        public void AddForce()
        {
            using (_PRF_AddForce.Auto())
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
        }

        [ButtonGroup("Force/fB")]
        [EnableIf(nameof(AddTorque_enable))]
        public void AddTorque()
        {
            using (_PRF_AddTorque.Auto())
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
        }

        [ButtonGroup("G")]
        [EnableIf(nameof(DisableGravity_enable))]
        public void DisableGravity()
        {
            using (_PRF_DisableGravity.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.useGravity = false;
            }
        }

        [ButtonGroup("P")]
        [EnableIf(nameof(DisablePhysicsSimulation_enable))]
        public void DisablePhysicsSimulation()
        {
#if UNITY_EDITOR
            PhysicsSimulator.TogglePhysicsSimulation();
#endif
        }

        [ButtonGroup("G")]
        [EnableIf(nameof(EnableGravity_enable))]
        public void EnableGravity()
        {
            using (_PRF_EnableGravity.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.useGravity = true;
            }
        }

        [ButtonGroup("P")]
        [EnableIf(nameof(EnablePhysicsSimulation_enable))]
        public void EnablePhysicsSimulation()
        {
#if UNITY_EDITOR
            PhysicsSimulator.TogglePhysicsSimulation();
#endif
        }

        [ButtonGroup("S")]
        [EnableIf(nameof(Sleep_enable))]
        public void Sleep()
        {
            using (_PRF_Sleep.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.Sleep();
            }
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopMovement_enable))]
        public void StopMovement()
        {
            using (_PRF_StopMovement.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopAngularVelocity_enable))]
        public void StopSpin()
        {
            using (_PRF_StopSpin.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        [ButtonGroup("A")]
        [EnableIf(nameof(StopVelocity_enable))]
        public void StopVelocity()
        {
            using (_PRF_StopVelocity.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.velocity = Vector3.zero;
            }
        }

        [ButtonGroup("S")]
        [EnableIf(nameof(WakeUp_enable))]
        public void WakeUp()
        {
            using (_PRF_WakeUp.Auto())
            {
                if (_rigidbody == null)
                {
                    _rigidbody = _transform.GetComponent<Rigidbody>();
                }

                _rigidbody.WakeUp();
            }
        }

        private float GetForce()
        {
            using (_PRF_GetForce.Auto())
            {
                if (relativeToMass)
                {
                    return _rigidbody.mass * force;
                }

                return force;
            }
        }

        private Vector3 GetForceVector()
        {
            using (_PRF_GetForceVector.Auto())
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
        }

        private void ResetPosition()
        {
            using (_PRF_ResetPosition.Auto())
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

        #region Profiling

        private const string _PRF_PFX = nameof(RigidbodyUtility) + ".";

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));
        private static readonly ProfilerMarker _PRF_Sleep = new ProfilerMarker(_PRF_PFX + nameof(Sleep));

        private static readonly ProfilerMarker _PRF_StopVelocity =
            new ProfilerMarker(_PRF_PFX + nameof(StopVelocity));

        private static readonly ProfilerMarker _PRF_WakeUp = new ProfilerMarker(_PRF_PFX + nameof(WakeUp));

        private static readonly ProfilerMarker
            _PRF_GetForce = new ProfilerMarker(_PRF_PFX + nameof(GetForce));

        private static readonly ProfilerMarker _PRF_GetForceVector =
            new ProfilerMarker(_PRF_PFX + nameof(GetForceVector));

        private static readonly ProfilerMarker _PRF_ResetPosition =
            new ProfilerMarker(_PRF_PFX + nameof(ResetPosition));

        private static readonly ProfilerMarker _PRF_AddExposiveForce =
            new ProfilerMarker(_PRF_PFX + nameof(AddExposiveForce));

        private static readonly ProfilerMarker _PRF_DisableGravity =
            new ProfilerMarker(_PRF_PFX + nameof(DisableGravity));

        private static readonly ProfilerMarker _PRF_EnableGravity =
            new ProfilerMarker(_PRF_PFX + nameof(EnableGravity));

        private static readonly ProfilerMarker _PRF_OnDrawGizmosSelected =
            new ProfilerMarker(_PRF_PFX + nameof(OnDrawGizmosSelected));

        private static readonly ProfilerMarker
            _PRF_AddForce = new ProfilerMarker(_PRF_PFX + nameof(AddForce));

        private static readonly ProfilerMarker _PRF_AddTorque =
            new ProfilerMarker(_PRF_PFX + nameof(AddTorque));

        private static readonly ProfilerMarker
            _PRF_StopSpin = new ProfilerMarker(_PRF_PFX + nameof(StopSpin));

        private static readonly ProfilerMarker _PRF_StopMovement =
            new ProfilerMarker(_PRF_PFX + nameof(StopMovement));

        private static readonly ProfilerMarker _PRF_Reset = new ProfilerMarker(_PRF_PFX + nameof(Reset));

        #endregion
    }
}
