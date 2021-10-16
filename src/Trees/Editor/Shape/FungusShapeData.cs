using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class FungusShapeData : ShapeData
    {
        public override TreeComponentType type => TreeComponentType.Fungus;


        protected override ShapeData GetNew()
        {
            return new FungusShapeData(shapeID, hierarchyID, parentShapeID);
        }

        protected override void Clone(ShapeData shapeData)
        {
        }

        public FungusShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(shapeID, hierarchyID, parentShapeID)
        {
            
        }
    }
}
