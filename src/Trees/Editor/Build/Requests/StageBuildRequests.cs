using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Cost;

namespace Appalachia.Simulation.Trees.Build.Requests
{
    [Serializable]
    public class StageBuildRequests : BuildRequest
    {
        #region Fields and Autoproperties

        private BuildRequestLevel _ambientOcclusion = BuildRequestLevel.None;
        private BuildRequestLevel _collision = BuildRequestLevel.None;
        private BuildRequestLevel _highQualityGeometry = BuildRequestLevel.None;
        private BuildRequestLevel _impostor = BuildRequestLevel.None;
        private BuildRequestLevel _levelsOfDetail = BuildRequestLevel.None;
        private BuildRequestLevel _lowQualityGeometry = BuildRequestLevel.None;
        private BuildRequestLevel _uv = BuildRequestLevel.None;
        private BuildRequestLevel _vertexData = BuildRequestLevel.None;

        #endregion

        /// <inheritdoc />
        public override BuildRequestLevel requestLevel
        {
            get
            {
                var rqst = BuildRequestLevel.None;

                rqst = rqst.Max(uv);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(lowQualityGeometry);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(ambientOcclusion);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(impostor);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(collision);
                if (rqst == BuildRequestLevel.InitialPass)
                {
                    return rqst;
                }

                rqst = rqst.Max(vertexData);

                return rqst;
            }
        }

        public BuildRequestLevel ambientOcclusion
        {
            get => _ambientOcclusion.Max(highQualityGeometry);
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

        public BuildRequestLevel impostor
        {
            get => _impostor.Max(highQualityGeometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _impostor = BuildRequestLevel.None;
                }

                if (value < _impostor)
                {
                    return;
                }

                _impostor = value;
            }
        }

        public BuildRequestLevel lowQualityGeometry
        {
            get => _lowQualityGeometry.Max(_highQualityGeometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _lowQualityGeometry = BuildRequestLevel.None;
                }

                if (value < _lowQualityGeometry)
                {
                    return;
                }

                _lowQualityGeometry = value;
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

        /*
        public BuildRequestLevel levelsOfDetail
        {
            get => _levelsOfDetail.Max(highQualityGeometry);
            set
            {
                if (value == BuildRequestLevel.None)
                {
                    _levelsOfDetail = BuildRequestLevel.None;
                }

                if (value < _levelsOfDetail) return;

                _levelsOfDetail = value;
            }
        }
        */

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
            if (ambientOcclusion >= level)
            {
                yield return new BuildCost(BuildCategory.AmbientOcclusion);
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
            else if (lowQualityGeometry >= level)
            {
                yield return new BuildCost(BuildCategory.LowQualityGeometry);
            }

            if (ShouldBuildMesh(level))
            {
                yield return new BuildCost(BuildCategory.Mesh);
            }

            if (impostor >= level)
            {
                yield return new BuildCost(BuildCategory.Impostor);
            }
        }

        /// <inheritdoc />
        public override bool ShouldBuild(BuildCategory category, BuildRequestLevel level)
        {
            switch (category)
            {
                case BuildCategory.UV:
                    return uv >= level;
                case BuildCategory.Collision:
                    return collision >= level;
                case BuildCategory.HighQualityGeometry:
                    return highQualityGeometry >= level;
                case BuildCategory.LowQualityGeometry:
                    return lowQualityGeometry >= level;
                case BuildCategory.AmbientOcclusion:
                    return ambientOcclusion >= level;
                /*case BuildCategory.LevelsOfDetail:
                    return levelsOfDetail >= level;*/
                case BuildCategory.VertexData:
                    return vertexData >= level;
                case BuildCategory.Impostor:
                    return impostor >= level;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        public bool ShouldBuildMesh(BuildRequestLevel level)
        {
            return (_lowQualityGeometry == level) ||
                   (_highQualityGeometry == level) ||
                   (_uv == level) ||
                   (_vertexData == level) ||
                   (_levelsOfDetail == level) ||
                   (_ambientOcclusion == level);
        }

        /// <inheritdoc />
        protected override void SetAllFromTo(BuildRequestLevel from, BuildRequestLevel to)
        {
            if (_highQualityGeometry == from)
            {
                _highQualityGeometry = to;
            }

            if (_uv == from)
            {
                _uv = to;
            }

            if (_collision == from)
            {
                _collision = to;
            }

            if (_lowQualityGeometry == from)
            {
                _lowQualityGeometry = to;
            }

            if (_ambientOcclusion == from)
            {
                _ambientOcclusion = to;
            }

            if (_levelsOfDetail == from)
            {
                _levelsOfDetail = to;
            }

            if (_vertexData == from)
            {
                _vertexData = to;
            }

            if (_impostor == from)
            {
                _impostor = to;
            }
        }
    }
}
