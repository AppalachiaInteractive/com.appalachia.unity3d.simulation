using System.Collections.Generic;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
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

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
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

                group.OnInitialize(this, i);
            }
        }
    }
}
