using System.Collections.Generic;
using Appalachia.Core.Behaviours;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ReactionSystem : AppalachiaMonoBehaviour
    {
        private const string _PRF_PFX = nameof(ReactionSystem) + ".";

        private static readonly ProfilerMarker _PRF_Awake = new(_PRF_PFX + nameof(Awake));

        private static readonly ProfilerMarker _PRF_Start = new(_PRF_PFX + nameof(Start));

        private static readonly ProfilerMarker _PRF_OnEnable = new(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));
        public List<ReactionSubsystemGroup> groups;

        private void Awake()
        {
            using (_PRF_Awake.Auto())
            {
                Initialize();
            }
        }

        private void Start()
        {
            using (_PRF_Start.Auto())
            {
                Initialize();
            }
        }

        private void OnEnable()
        {
            using (_PRF_OnEnable.Auto())
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (groups == null)
                {
                    groups = new List<ReactionSubsystemGroup>();
                }

                for (var i = groups.Count - 1; i >= 0; i--)
                {
                    var group = groups[i];

                    if (group == null)
                    {
                        groups.RemoveAt(i);
                        continue;
                    }

                    group.Initialize(this, i);
                }
            }
        }
    }
}
