using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class AgeBuildRequests : BuildRequest
    {
        #region Fields and Autoproperties

        private BuildRequestLevel _distribution = BuildRequestLevel.None;

        #endregion

        /// <inheritdoc />
        public override BuildRequestLevel requestLevel => distribution;

        public BuildRequestLevel distribution
        {
            get => _distribution;
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _distribution = BuildRequestLevel.None;
                }

                if (value < _distribution)
                {
                    return;
                }

                _distribution = value;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            if (distribution >= level)
            {
                yield return new BuildCost(BuildCategory.Distribution);
            }
        }

        /// <inheritdoc />
        public override bool ShouldBuild(BuildCategory category, BuildRequestLevel level)
        {
            switch (category)
            {
                case BuildCategory.Distribution:
                    return distribution >= level;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        /// <inheritdoc />
        protected override void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to)
        {
            if (_distribution == from)
            {
                _distribution = to;
            }
        }
    }
}
