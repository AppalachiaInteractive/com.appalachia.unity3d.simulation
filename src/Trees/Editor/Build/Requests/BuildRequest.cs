using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public abstract class BuildRequest
    {
        public abstract BuildRequestLevel requestLevel { get; }

        public abstract IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level);

        public abstract bool ShouldBuild(BuildCategory category, BuildRequestLevel level);
        
        public void DecreaseBuildLevel(BuildRequestLevel level)
        {
            if (level == BuildRequestLevel.InitialPass)
            {
                SetAllFromTo(BuildRequestLevel.InitialPass, BuildRequestLevel.FinalPass);                
            }
            else if (level == BuildRequestLevel.FinalPass)
            {
                SetAllFromTo(BuildRequestLevel.FinalPass, BuildRequestLevel.None);        
            }
        }
        
        public void CollapseDelayedBuildLevel()
        {
            SetAllFromTo(BuildRequestLevel.InitialPass, BuildRequestLevel.FinalPass);
        }
        
        protected abstract void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to);

    }
}
