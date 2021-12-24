using Appalachia.Core.Objects.Root.Contracts;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Interfaces
{
    public interface ISpeciesDataProvider : ICrossAssemblySerializable
    {
        #if UNITY_EDITOR
        
        int GetIndividualCount();

        string GetSpeciesName();

        bool HasIndividual(int individualIndex, AgeType age, StageType stage);

        GameObject GetIndividual(int individualIndex, AgeType age, StageType stage);

        int GetMaxShapeIndex(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType);

        Matrix4x4 GetShapeMatrix(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex);

        ShapeGeometryData GetShapeGeometry(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex);

        ShapeData GetShapeData(
            int individualIndex,
            AgeType age,
            StageType stage,
            TreeComponentType shapeType,
            ref int shapeIndex);

        ShapeData GetShapeDataByID(
            int individualIndex,
            AgeType age,
            StageType stage,
            int shapeID);
        
        bool drawGizmos { get; }
        
        #endif
        
    }
}
