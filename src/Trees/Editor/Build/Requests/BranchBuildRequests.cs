using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Execution;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class BranchBuildRequests : BuildRequest
    {
        #region Fields and Autoproperties

        [SerializeField] private BuildRequestLevel _ambientOcclusion = BuildRequestLevel.None;
        [SerializeField] private BuildRequestLevel _distribution = BuildRequestLevel.None;
        [SerializeField] private BuildRequestLevel _geometry = BuildRequestLevel.None;
        [SerializeField] private BuildRequestLevel _materialGeneration = BuildRequestLevel.None;
        [SerializeField] private BuildRequestLevel _materialProperties = BuildRequestLevel.None;
        [SerializeField] private BuildRequestLevel _uv = BuildRequestLevel.None;

        #endregion

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

                    rqst = rqst.Max(distribution);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    rqst = rqst.Max(uv);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    rqst = rqst.Max(geometry);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    rqst = rqst.Max(ambientOcclusion);
                    if (rqst == BuildRequestLevel.InitialPass)
                    {
                        return rqst;
                    }

                    return rqst;
                }
            }
        }

        public BuildRequestLevel ambientOcclusion
        {
            get => _ambientOcclusion.Max(geometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _ambientOcclusion = BuildRequestLevel.None;
                }

                if (value < _ambientOcclusion)
                {
                    return;
                }

                _ambientOcclusion = value;
            }
        }

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

        public BuildRequestLevel geometry
        {
            get => _geometry;
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _geometry = BuildRequestLevel.None;
                }

                if (value < _geometry)
                {
                    return;
                }

                _geometry = value;
            }
        }

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

        public BuildRequestLevel uv
        {
            get => _uv.Max(geometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _uv = BuildRequestLevel.None;
                }

                if (value < _uv)
                {
                    return;
                }

                _uv = value;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            if (materialGeneration >= level)
            {
                yield return new BuildCost(BuildCategory.MaterialGeneration);
            }

            if (materialProperties >= level)
            {
                yield return new BuildCost(BuildCategory.MaterialProperties);
            }

            if (distribution >= level)
            {
                yield return new BuildCost(BuildCategory.Distribution);
            }

            if (uv >= level)
            {
                yield return new BuildCost(BuildCategory.UV);
            }

            if (geometry >= level)
            {
                yield return new BuildCost(BuildCategory.HighQualityGeometry);
            }
            else if (ShouldBuildLowQualityGeometry(level))
            {
                yield return new BuildCost(BuildCategory.LowQualityGeometry);
            }

            if (ambientOcclusion >= level)
            {
                yield return new BuildCost(BuildCategory.AmbientOcclusion);
            }

            if (ShouldBuildMesh(level))
            {
                yield return new BuildCost(BuildCategory.Mesh);
            }
        }

        /// <inheritdoc />
        public override bool ShouldBuild(BuildCategory category, BuildRequestLevel level)
        {
            switch (category)
            {
                case BuildCategory.MaterialGeneration:
                    return materialGeneration >= level;
                case BuildCategory.MaterialProperties:
                    return materialProperties >= level;
                case BuildCategory.Distribution:
                    return distribution >= level;
                case BuildCategory.UV:
                    return uv >= level;
                case BuildCategory.HighQualityGeometry:
                    return geometry >= level;
                case BuildCategory.AmbientOcclusion:
                    return ambientOcclusion >= level;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        public bool ShouldBuildGeometry(BuildRequestLevel level)
        {
            if (geometry == level)
            {
                return true;
            }

            return false;
        }

        public bool ShouldBuildLowQualityGeometry(BuildRequestLevel level)
        {
            return (distribution == level) && (level > geometry);
        }

        public bool ShouldBuildMesh(BuildRequestLevel level)
        {
            return requestLevel == level;
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

            if (_distribution == from)
            {
                _distribution = to;
            }

            if (_uv == from)
            {
                _uv = to;
            }

            if (_geometry == from)
            {
                _geometry = to;
            }

            if (_ambientOcclusion == from)
            {
                _ambientOcclusion = to;
            }
        }
    }
}
