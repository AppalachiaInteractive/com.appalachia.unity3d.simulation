using System.Collections.Generic;
using Appalachia.Core.Behaviours;
using Appalachia.Core.Debugging;
using Appalachia.Editing.Debugging;
using Appalachia.Editing.Debugging.Handle;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Appalachia.Simulation.ReactionSystem.Base
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ReactionSubsystemCenter : AppalachiaMonoBehaviour
    {
        private const string _PRF_PFX = nameof(ReactionSubsystemCenter) + ".";

        private static readonly ProfilerMarker _PRF_OnDrawGizmosSelected =
            new(_PRF_PFX + nameof(OnDrawGizmosSelected));

        private static readonly ProfilerMarker _PRF_GetPosition =
            new(_PRF_PFX + nameof(GetPosition));

        private static readonly ProfilerMarker _PRF_ValidateSubsystems =
            new(_PRF_PFX + nameof(ValidateSubsystems));

        public Vector3 offset;

        public Color gizmoColor = Color.cyan;

        [FormerlySerializedAs("systems")]
        public List<ReactionSubsystemBase> subsystems = new();

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
    }
}
