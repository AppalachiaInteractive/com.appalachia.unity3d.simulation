using System.Collections.Generic;
using Appalachia.Core.Debugging;
using Appalachia.Core.Objects.Root;
using Appalachia.Editing.Debugging.Handle;
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
        public List<ReactionSubsystemBase> subsystems = new();

        #endregion

        #region Event Functions

#if UNITY_EDITOR

        private static readonly ProfilerMarker _PRF_OnDrawGizmosSelected =
            new(_PRF_PFX + nameof(OnDrawGizmosSelected));

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

        public Vector3 GetPosition()
        {
            using (_PRF_GetPosition.Auto())
            {
                return _transform.position + offset;
            }
        }

        public void ValidateSubsystems()
        {
            using (_PRF_ValidateSubsystems.Auto())
            {
                if (subsystems == null)
                {
                    subsystems = new List<ReactionSubsystemBase>();
                }

                for (var i = subsystems.Count - 1; i >= 0; i--)
                {
                    var subsystem = subsystems[i];

                    if (subsystem == null)
                    {
                        subsystems.RemoveAt(i);
                    }
                }
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(ReactionSubsystemCenter) + ".";

        private static readonly ProfilerMarker _PRF_GetPosition = new(_PRF_PFX + nameof(GetPosition));

        private static readonly ProfilerMarker _PRF_ValidateSubsystems =
            new(_PRF_PFX + nameof(ValidateSubsystems));

        #endregion
    }
}
