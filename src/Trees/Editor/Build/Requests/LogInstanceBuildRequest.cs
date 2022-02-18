using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class LogInstanceBuildRequest : BuildRequest
    {
        #region Fields and Autoproperties

        private BuildRequestLevel _collision = BuildRequestLevel.None;
        private BuildRequestLevel _distribution = BuildRequestLevel.None;
        private BuildRequestLevel _highQualityGeometry = BuildRequestLevel.None;
        private BuildRequestLevel _levelsOfDetail = BuildRequestLevel.None;
        private BuildRequestLevel _uv = BuildRequestLevel.None;
        private BuildRequestLevel _vertexData = BuildRequestLevel.None;

        #endregion

        /// <inheritdoc />
        public override BuildRequestLevel requestLevel
        {
            get
            {
                var rqst = BuildRequestLevel.None;

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

                rqst = rqst.Max(levelsOfDetail);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(vertexData);

                return rqst;
            }
        }

        public BuildRequestLevel collision
        {
            get => _collision.Max(_highQualityGeometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _collision = BuildRequestLevel.None;
                }

                if (value < _collision)
                {
                    return;
                }

                _collision = value;
            }
        }

        public BuildRequestLevel distribution
        {
            get => _distribution.Max(_highQualityGeometry);
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

        public BuildRequestLevel highQualityGeometry
        {
            get => _highQualityGeometry;
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _highQualityGeometry = BuildRequestLevel.None;
                }

                if (value < _highQualityGeometry)
                {
                    return;
                }

                _highQualityGeometry = value;
            }
        }

        public BuildRequestLevel levelsOfDetail
        {
            get => _levelsOfDetail.Max(highQualityGeometry).Max(collision);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _levelsOfDetail = BuildRequestLevel.None;
                }

                if (value < _levelsOfDetail)
                {
                    return;
                }

                _levelsOfDetail = value;
            }
        }

        public BuildRequestLevel uv
        {
            get => _uv.Max(_highQualityGeometry);
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

        public BuildRequestLevel vertexData
        {
            get => _vertexData.Max(highQualityGeometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _vertexData = BuildRequestLevel.None;
                }

                if (value < _vertexData)
                {
                    return;
                }

                _vertexData = value;
            }
        }

        /// <inheritdoc />
        public override IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            if (distribution >= level)
            {
                yield return new BuildCost(BuildCategory.Distribution);
            }

            if (uv >= level)
            {
                yield return new BuildCost(BuildCategory.UV);
            }

            if (collision >= level)
            {
                yield return new BuildCost(BuildCategory.Collision);
            }

            if (vertexData >= level)
            {
                yield return new BuildCost(BuildCategory.VertexData);
            }

            if (highQualityGeometry >= level)
            {
                yield return new BuildCost(BuildCategory.HighQualityGeometry);
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
                case BuildCategory.Distribution:
                    return distribution >= level;
                case BuildCategory.UV:
                    return uv >= level;
                case BuildCategory.Collision:
                    return collision >= level;
                case BuildCategory.HighQualityGeometry:
                    return highQualityGeometry >= level;
                case BuildCategory.LevelsOfDetail:
                    return levelsOfDetail >= level;
                case BuildCategory.VertexData:
                    return vertexData >= level;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        public bool ShouldBuildMesh(BuildRequestLevel level)
        {
            return requestLevel == level;
        }

        /// <inheritdoc />
        protected override void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to)
        {
            if (_highQualityGeometry == from)
            {
                _highQualityGeometry = to;
            }

            if (_distribution == from)
            {
                _distribution = to;
            }

            if (_uv == from)
            {
                _uv = to;
            }

            if (_collision == from)
            {
                _collision = to;
            }

            if (_levelsOfDetail == from)
            {
                _levelsOfDetail = to;
            }

            if (_vertexData == from)
            {
                _vertexData = to;
            }
        }
    }
}
