using System;
using System.Collections.Generic;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Recursion;

namespace Appalachia.Simulation.Trees.Definition.Interfaces
{
    public interface IHierarchyRead
    {
        float GetVerticalOffset();

        bool DoesHierarchyExist(int hierarchyID);
        
        HierarchyData GetHierarchyData(int hierarchyID);

        HierarchyData GetHierarchyData(ShapeData shape);
        
        IEnumerable<HierarchyData> GetHierarchies();
        
        IEnumerable<HierarchyData> GetHierarchies(TreeComponentType type);
        
        int GetHierarchyCount(TreeComponentType type);

        IEnumerable<HierarchyData> GetHierarchiesByParent(int parentHierarchyID);

        void RecurseHierarchies(Action<HierarchyData> action);

        void RecurseHierarchiesWithData(Action<HierarchyRecursionData> action);

        bool IsAncestor(HierarchyData ancestor, HierarchyData descendent);
    }
}