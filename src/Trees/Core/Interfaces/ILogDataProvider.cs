using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Interfaces
{
    public interface ILogDataProvider : ICrossAssemblySerializable
    {
        #if UNITY_EDITOR
        
        int GetLogCount();

        string GetName();

        bool HasLog(int logIndex);

        GameObject GetLog(int logIndex);

        int GetMaxShapeIndex(int logIndex, TreeComponentType shapeType);

        Matrix4x4 GetShapeMatrix(int logIndex, TreeComponentType shapeType, ref int shapeIndex);

        ShapeGeometryData GetShapeGeometry(int logIndex, TreeComponentType shapeType, ref int shapeIndex);


        ShapeData GetShapeData(int logIndex, TreeComponentType shapeType, ref int shapeIndex);

        ShapeData GetShapeDataByID(int logIndex, int shapeID);

        bool drawGizmos { get; }
        
        #endif
    }
}
