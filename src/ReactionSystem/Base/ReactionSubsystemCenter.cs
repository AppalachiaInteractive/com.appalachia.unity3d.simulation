using System.Collections.Generic;
using Appalachia.Core.Debugging;
using Appalachia.Core.Objects.Root;
using Appalachia.Editing.Debugging.Handle;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.ReactionSystem.Base
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class ReactionSubsystemCenter : AppalachiaBehaviour<ReactionSubsystemCenter>
    {
        #region Fields and Autoproperties

        public Vector3 offset;

        public Color gizmoColor = Color.cyan;

        [FormerlySerializedAs("systems")]
        [FormerlySerializedAs("subsystems")]
        private List<IReactionSubsystem> _subsystems = new();

        #endregion

        public IReadOnlyList<IReactionSubsystem> Subsystems => _subsystems;

        #region Event Functions

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            using (_PRF_OnDrawGizmosSelected.Auto())
            {
                if (!GizmoCameraChecker.ShouldRenderGizmos())
                {
                    return;
                }

                SmartHandles.DrawWireSphere(GetPosition(), 2f, gizmoColor);
            }
        }
#endif

        #endregion

        public void EnsureSubsystemIsAdded(IReactionSubsystem subsystem)
        {
            using (_PRF_AddSubsystem.Auto())
            {
                _subsystems ??= new List<IReactionSubsystem>();

                if (!_subsystems.Contains(subsystem))
                {
                    _subsystems.Add(subsystem);
                }
            }
        }

        public Vector3 GetPosition()
        {
            using (_PRF_GetPosition.Auto())
            {
                return Transform.position + offset;
            }
        }

        public void ValidateSubsystems()
        {
            using (_PRF_ValidateSubsystems.Auto())
            {
                if (_subsystems == null)
                {
                    _subsystems = new List<IReactionSubsystem>();
                }

                for (var i = _subsystems.Count - 1; i >= 0; i--)
                {
                    var subsystem = _subsystems[i];

                    if (subsystem == null)
                    {
                        _subsystems.RemoveAt(i);
                    }
                }
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_AddSubsystem =
            new ProfilerMarker(_PRF_PFX + nameof(EnsureSubsystemIsAdded));

        private static readonly ProfilerMarker _PRF_GetPosition = new(_PRF_PFX + nameof(GetPosition));

        private static readonly ProfilerMarker _PRF_ValidateSubsystems =
            new(_PRF_PFX + nameof(ValidateSubsystems));

        #endregion
    }
}
