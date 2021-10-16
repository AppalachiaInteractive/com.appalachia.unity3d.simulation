using System;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;

namespace Appalachia.Simulation.Trees.Shape
{
    [Serializable]
    public sealed class FruitShapeData : ShapeData
    {
        public override TreeComponentType type => TreeComponentType.Fruit;


        protected override ShapeData GetNew()
        {
            return new FruitShapeData(shapeID, hierarchyID, parentShapeID);
        }

        protected override void Clone(ShapeData shapeData)
        {
        }

        public FruitShapeData(int shapeID, int hierarchyID, int parentShapeID) : base(shapeID, hierarchyID, parentShapeID)
        {
            
        }
    }
}