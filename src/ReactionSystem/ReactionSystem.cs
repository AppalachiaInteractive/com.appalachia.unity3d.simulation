using System.Collections.Generic;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class ReactionSystem : AppalachiaBehaviour<ReactionSystem>
    {
        #region Fields and Autoproperties

        public List<ReactionSubsystemGroup> groups;

        #endregion

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

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

        #region Profiling

        private const string _PRF_PFX = nameof(ReactionSystem) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        #endregion
    }
}
