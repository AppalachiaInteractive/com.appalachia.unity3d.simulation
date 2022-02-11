using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Appalachia.Simulation.ReactionSystem.Base;
using Appalachia.Utility.Timing;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Cameras
{
    [Serializable]
    public struct SubsystemCameraShotSelector : IEquatable<SubsystemCameraShotSelector>
    {
        private const string _PRF_PFX = nameof(SubsystemCameraShotSelector) + ".";

        private static readonly ProfilerMarker _PRF_InitiateCheck =
            new(_PRF_PFX + nameof(InitiateCheck));

        private static readonly ProfilerMarker _PRF_ShouldRenderAtCenter =
            new(_PRF_PFX + nameof(ShouldRenderAtCenter));

        private static readonly ProfilerMarker _PRF_ShouldRenderCamera =
            new(_PRF_PFX + nameof(ShouldRenderCamera));

        private static readonly ProfilerMarker _PRF_CheckIndex = new(_PRF_PFX + nameof(CheckIndex));

        [SerializeField] private bool _initialized;
        public int currentIndex;

        public void InitiateCheck(int totalCameras)
        {
            using (_PRF_InitiateCheck.Auto())
            {
                CheckIndex(totalCameras);
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public bool ShouldRenderAtCenter(
            SubsystemCameraShotSelectionMode selectionMode,
            ReactionSubsystemCenter center,
            int centerIndex,
            int totalCameras,
            int frameInterval)
        {
            using (_PRF_ShouldRenderAtCenter.Auto())
            {
                switch (selectionMode)
                {
                    case SubsystemCameraShotSelectionMode.RoundRobin:

                        return centerIndex == currentIndex;

                    case SubsystemCameraShotSelectionMode.EveryXFrames:

                        if (frameInterval == 0)
                        {
                            throw new NotSupportedException("Set frame interval!");
                        }

                        return ((CoreClock.Instance.FrameCount + centerIndex) % frameInterval) == 0;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(selectionMode),
                            selectionMode,
                            null
                        );
                }
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public bool ShouldRenderCamera(
            SubsystemCameraShotSelectionMode selectionMode,
            SubsystemCameraComponent camera,
            int cameraIndex,
            int totalCameras,
            int frameInterval)
        {
            using (_PRF_ShouldRenderCamera.Auto())
            {
                switch (selectionMode)
                {
                    case SubsystemCameraShotSelectionMode.RoundRobin:

                        return cameraIndex == currentIndex;

                    case SubsystemCameraShotSelectionMode.EveryXFrames:

                        if (frameInterval == 0)
                        {
                            throw new NotSupportedException("Set frame interval!");
                        }

                        return ((CoreClock.Instance.FrameCount + cameraIndex) % frameInterval) == 0;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(selectionMode),
                            selectionMode,
                            null
                        );
                }
            }
        }

        private void CheckIndex(int count)
        {
            using (_PRF_CheckIndex.Auto())
            {
                if (_initialized)
                {
                    currentIndex += 1;
                }
                else
                {
                    _initialized = true;
                }

                if ((currentIndex >= count) || (currentIndex < 0))
                {
                    currentIndex = 0;
                }
            }
        }

#region IEquatable

        [DebuggerStepThrough] public bool Equals(SubsystemCameraShotSelector other)
        {
            return (_initialized == other._initialized) && (currentIndex == other.currentIndex);
        }

        [DebuggerStepThrough] public override bool Equals(object obj)
        {
            return obj is SubsystemCameraShotSelector other && Equals(other);
        }

        [DebuggerStepThrough] public override int GetHashCode()
        {
            unchecked
            {
                return (_initialized.GetHashCode() * 397) ^ currentIndex;
            }
        }

        [DebuggerStepThrough] public static bool operator ==(
            SubsystemCameraShotSelector left,
            SubsystemCameraShotSelector right)
        {
            return left.Equals(right);
        }

        [DebuggerStepThrough] public static bool operator !=(
            SubsystemCameraShotSelector left,
            SubsystemCameraShotSelector right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}
