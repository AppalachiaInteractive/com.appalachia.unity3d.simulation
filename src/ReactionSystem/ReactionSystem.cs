using System.Collections.Generic;
using Appalachia.Core.Behaviours;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ReactionSystem : AppalachiaBehaviour
    {
        private const string _PRF_PFX = nameof(ReactionSystem) + ".";

        private static readonly ProfilerMarker _PRF_Awake = new(_PRF_PFX + nameof(Awake));

        private static readonly ProfilerMarker _PRF_Start = new(_PRF_PFX + nameof(Start));

        private static readonly ProfilerMarker _PRF_OnEnable = new(_PRF_PFX + nameof(OnEnable));

        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));
        public List<ReactionSubsystemGroup> groups;

        protected override bool InitializeAlways => true;

        protected override void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                base.Initialize();
                
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
