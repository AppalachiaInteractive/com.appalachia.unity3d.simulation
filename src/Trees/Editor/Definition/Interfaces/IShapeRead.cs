using System;
using System.Collections;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Recursion;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface IShapeRead : IEnumerable<ShapeData>
    {
        void Rebuild();
        
        IList this[int i] { get; }

        int GetHierarchyShapeCount(int hierarchyID, bool visibleOnly);
        
        ShapeData GetParentShapeData(ShapeData shape);
        
        IEnumerable<ShapeData> GetShapes(TreeComponentType type);
        
        ShapeData GetShapeData(ShapeData shape);
        
        ShapeData GetShapeData(int shapeID);
        
        T GetShapeDataAs<T>(ShapeData shape)
            where T : ShapeData;

        T GetShapeDataAs<T>(int shapeID)
            where T : ShapeData;
        
        List<ShapeData> GetShapesByHierarchy(HierarchyData hierarchy);
        
        List<ShapeData> GetShapesByHierarchy(int hierarchyID);
        
        List<ShapeData> GetShapesByParentShape(int parentShapeID);
        
        TreeComponentType GetShapeType(ShapeData shape);
        
        TreeComponentType GetShapeType(int shapeID);
        
        bool IsShapeType(ShapeData shape, TreeComponentType type);
        
        bool IsShapeType(int shapeID, TreeComponentType type);
        
        bool ShapeExists(int shapeID);
       
        void RecurseSplines(IHierarchyRead read, Action<BarkRecursionData> action);
        
        void RecurseLeaves(IHierarchyRead read, Action<LeafRecursionData> action);
        
        void RecurseShapes(IHierarchyRead read, Action<GenericRecursionData> action);
    }
}
