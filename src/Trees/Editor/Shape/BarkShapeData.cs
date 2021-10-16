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
        public float breakOffset;
        public bool breakInverted;
        public float capRange;
        public SplineData spline;

        public List<List<BranchRing>> branchRings;

        protected BarkShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(shapeID, hierarchyID, parentShapeID)
        {
            spline = new SplineData();
            branchRings = new List<List<BranchRing>>();
        }
        
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