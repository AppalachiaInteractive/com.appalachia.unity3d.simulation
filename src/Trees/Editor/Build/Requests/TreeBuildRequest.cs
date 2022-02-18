using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Execution;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class TreeBuildRequest : BuildRequest
    {
        #region Fields and Autoproperties

        private BuildRequestLevel _materialGeneration = BuildRequestLevel.None;
        private BuildRequestLevel _materialProperties = BuildRequestLevel.None;

        #endregion

        /*
        public BuildRequestLevel prefabCreation
        {
            get => _prefabCreation.Max(materialGeneration);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _prefabCreation = BuildRequestLevel.None;
                }

                if (value < _prefabCreation) return;

                _prefabCreation = value;
            }
        }*/

        /// <inheritdoc />
        public override BuildRequestLevel requestLevel
        {
            get
            {
                using (BUILD_TIME.SPC_BUILD_REQ.BuildLevel.Auto())
                {
                    var rqst = BuildRequestLevel.None;

                    rqst = rqst.Max(materialGeneration);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    rqst = rqst.Max(materialProperties);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    /*
                    requestLevel = requestLevel.Max(prefabCreation);
                    if (requestLevel == BuildRequestLevel.InitialPass) return requestLevel;*/

                    return rqst;
                }
            }
        }
        /*private BuildRequestLevel _prefabCreation = BuildRequestLevel.None;*/

        public BuildRequestLevel materialGeneration
        {
            get => _materialGeneration;
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _materialGeneration = BuildRequestLevel.None;
                }

                if (value < _materialGeneration)
                {
                    return;
                }

                _materialGeneration = value;
            }
        }

        public BuildRequestLevel materialProperties
        {
            get => _materialProperties.Max(materialGeneration);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _materialProperties = BuildRequestLevel.None;
                }

                if (value < _materialProperties)
                {
                    return;
                }

                _materialProperties = value;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            using (BUILD_TIME.SPC_BUILD_REQ.GetBuildCosts.Auto())
            {
                if (materialGeneration >= level)
                {
                    yield return new BuildCost(BuildCategory.MaterialGeneration);
                }

                if (materialProperties >= level)
                {
                    yield return new BuildCost(BuildCategory.MaterialProperties);
                }
                /*if (prefabCreation >= level) yield return new BuildCost(BuildCategory.PrefabCreation);*/
            }
        }

        /// <inheritdoc />
        public override bool ShouldBuild(BuildCategory category, BuildRequestLevel level)
        {
            using (BUILD_TIME.SPC_BUILD_REQ.ShouldBuild.Auto())
            {
                switch (category)
                {
                    case BuildCategory.MaterialGeneration:
                        return materialGeneration >= level;
                    case BuildCategory.MaterialProperties:
                        return materialProperties >= level;
                    /*case BuildCategory.PrefabCreation:
                        return prefabCreation >= level;*/
                    default:
                        throw new ArgumentOutOfRangeException(nameof(category), category, null);
                }
            }
        }

        /// <inheritdoc />
        protected override void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to)
        {
            if (_materialGeneration == from)
            {
                _materialGeneration = to;
            }

            if (_materialProperties == from)
            {
                _materialProperties = to;
            }
            /*if (_prefabCreation == from) _prefabCreation = to;*/
        }
    }
}
