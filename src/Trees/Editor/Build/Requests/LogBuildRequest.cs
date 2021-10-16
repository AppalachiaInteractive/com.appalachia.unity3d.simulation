using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Execution;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class LogBuildRequest : BuildRequest
    {
        private BuildRequestLevel _distribution = BuildRequestLevel.None;
        private BuildRequestLevel _materialProperties = BuildRequestLevel.None;
  
        public BuildRequestLevel distribution
        {
            get => _distribution;
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _distribution = BuildRequestLevel.None;
                }
                if (value < _distribution) return;
                
                _distribution = value;
            }
        }

        public BuildRequestLevel materialProperties
        {
            get => _materialProperties.Max(distribution);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _materialProperties = BuildRequestLevel.None;
                }
                if (value < _materialProperties) return;
                
                _materialProperties = value;
            }
        }

        public override BuildRequestLevel requestLevel
        {
            get
            {
                using (BUILD_TIME.SPC_BUILD_REQ.BuildLevel.Auto())
                {
                    BuildRequestLevel rqst = BuildRequestLevel.None;

                    rqst = rqst.Max(distribution);
                    if (rqst == BuildRequestLevel.InitialPass) return rqst;

                    rqst = rqst.Max(materialProperties);
                    if (rqst == BuildRequestLevel.InitialPass) return rqst;

                    return rqst;
                }
            }
        }

        public override IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            using (BUILD_TIME.SPC_BUILD_REQ.GetBuildCosts.Auto())
            {
                if (distribution >= level) yield return new BuildCost(BuildCategory.Distribution);
                if (materialProperties >= level) yield return new BuildCost(BuildCategory.MaterialProperties);
            }
        }
        
        public override bool ShouldBuild(BuildCategory category, BuildRequestLevel level)
        {
            using (BUILD_TIME.SPC_BUILD_REQ.ShouldBuild.Auto())
            {
                switch (category)
                {
                    case BuildCategory.Distribution:
                        return distribution >= level;
                    case BuildCategory.MaterialProperties:
                        return materialProperties >= level;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(category), category, null);
                }
            }
        }
        
        protected override void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to)
        {
            if (_distribution == from) _distribution = to;
            if (_materialProperties == from) _materialProperties = to;
        }
    }
}