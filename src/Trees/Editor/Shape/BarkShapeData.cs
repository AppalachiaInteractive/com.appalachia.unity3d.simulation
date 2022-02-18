using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Spline;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public abstract class BarkShapeData : ShapeData
    {
        protected BarkShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(
            shapeID,
            hierarchyID,
            parentShapeID
        )
        {
            spline = new SplineData();
            branchRings = new List<List<BranchRing>>();
        }

        #region Fields and Autoproperties

        public bool breakInverted;
        public float breakOffset;
        public float capRange;

        public List<List<BranchRing>> branchRings;
        public SplineData spline;

        #endregion

        /// <inheritdoc />
        protected override void Clone(ShapeData shapeData)
        {
            var s = shapeData as BarkShapeData;
            s.spline = spline;
            s.capRange = capRange;
            s.breakOffset = breakOffset;

            s.branchRings = new List<List<BranchRing>>();

            foreach (var br in branchRings)
            {
                var nlod = new List<BranchRing>();
                s.branchRings.Add(nlod);

                foreach (var lod in br)
                {
                    nlod.Add(lod.Clone());
                }
            }
        }

        /// <inheritdoc />
        protected override void SetUpInternal()
        {
            breakOffset = 1f;
            breakInverted = false;
            capRange = 0f;
            spline?.points.Clear();
            branchRings?.Clear();
        }
    }
}
