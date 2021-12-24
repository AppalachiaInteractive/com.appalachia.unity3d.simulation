using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Types;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Recursion;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Shape.Collections
{
    [Serializable]
    public abstract class ShapeCollection<T> : ISerializationCallbackReceiver, IShapeReadWrite
        where T : ShapeCollection<T>
    {
        protected ShapeCollection()
        {
            InitializeSelf();
        }

        #region Fields and Autoproperties

        [NonSerialized] public Dictionary<int, Dictionary<int, List<ShapeData>>> byHierarchyIDAndParentID;
        [NonSerialized] public Dictionary<int, List<ShapeData>> byHierarchyID;
        [NonSerialized] public Dictionary<int, List<ShapeData>> byParentID;
        [NonSerialized] public Dictionary<int, ShapeData> byID;

        [HideInInspector] public IDIncrementer idGenerator;

        #endregion

        public abstract void CopyPropertiesToClone(T clone);

        public void Clear()
        {
            ClearSelf();
            ClearInternal();
        }

        public void RecurseShapes(IHierarchyRead read, List<ShapeData> s, Action<GenericRecursionData> action)
        {
            if (s == null)
            {
                foreach (var trunk in read.GetHierarchies(TreeComponentType.Trunk))
                {
                    if (byHierarchyID.ContainsKey(trunk.hierarchyID))
                    {
                        RecurseShapes(read, byHierarchyID[trunk.hierarchyID], action);
                    }
                }
            }
            else
            {
                for (var i = s.Count - 1; i >= 0; i--)
                {
                    var shape = s[i];
                    var hierarchy = read.GetHierarchyData(shape);
                    var parentHierarchy = read.GetHierarchyData(hierarchy.parentHierarchyID);
                    var parentShape = GetShapeData(shape.parentShapeID);

                    var recursion = new GenericRecursionData(
                        shape,
                        hierarchy,
                        i,
                        s.Count,
                        parentHierarchy,
                        parentShape
                    );

                    action(recursion);

                    if (!byParentID.ContainsKey(shape.shapeID))
                    {
                        continue;
                    }

                    var childShapes = byParentID[shape.shapeID];

                    RecurseShapes(read, childShapes, action);
                }
            }
        }

        public void RemoveOrphanedShapes(IHierarchyRead hierarchies)
        {
            InitializeSelf();

            var removals = new HashSet<ShapeData>();

            foreach (var shape in this)
            {
                if (!hierarchies.DoesHierarchyExist(shape.hierarchyID))
                {
                    removals.Add(shape);
                }
            }

            foreach (var shape in removals)
            {
                DeleteShapeChain(shape.shapeID, false);
            }
        }

        public int TotalShapeCount()
        {
            if (byID == null)
            {
                return 0;
            }

            return byID.Count;
        }

        protected abstract void ClearInternal();

        protected abstract ShapeData CreateNewShape(
            TreeComponentType type,
            int hierarchyID,
            int shapeID,
            int parentShapeID);

        protected abstract IEnumerable<ShapeData> GetShapesInternal(TreeComponentType type);

        protected abstract void RemoveShape(ShapeData shape);

        private void ClearSelf()
        {
            //idGenerator.SetNextID(0);
            byID.Clear();
            byHierarchyID.Clear();
            byHierarchyIDAndParentID.Clear();
            byParentID.Clear();
        }

        private void InitializeSelf()
        {
            if (idGenerator == null)
            {
                idGenerator = new IDIncrementer(true);
            }

            if (byID == null)
            {
                byID = new Dictionary<int, ShapeData>();
            }

            if (byHierarchyID == null)
            {
                byHierarchyID = new Dictionary<int, List<ShapeData>>();
            }

            if (byHierarchyIDAndParentID == null)
            {
                byHierarchyIDAndParentID = new Dictionary<int, Dictionary<int, List<ShapeData>>>();
            }

            if (byParentID == null)
            {
                byParentID = new Dictionary<int, List<ShapeData>>();
            }
        }

        private void RecurseSplines(
            IHierarchyRead read,
            BarkHierarchyData hierarchy,
            Action<BarkRecursionData> action)
        {
            if (hierarchy == null)
            {
                foreach (var trunk in read.GetHierarchies(TreeComponentType.Trunk))
                {
                    RecurseSplines(read, trunk as BarkHierarchyData, action);
                }
            }
            else
            {
                var s = byHierarchyID[hierarchy.hierarchyID];

                for (var i = 0; i < s.Count; i++)
                {
                    var shape = s[i] as BarkShapeData;

                    if (shape == null)
                    {
                        continue;
                    }

                    var parentHierarchy = read.GetHierarchyData(hierarchy.parentHierarchyID);
                    var parentShape = GetShapeData(shape.parentShapeID) as BarkShapeData;

                    var recursion = new BarkRecursionData(
                        shape,
                        hierarchy,
                        i,
                        s.Count,
                        parentHierarchy,
                        parentShape
                    );

                    action(recursion);
                }

                var childHierarchies = read.GetHierarchiesByParent(hierarchy.parentHierarchyID);

                if (childHierarchies == null)
                {
                    return;
                }

                foreach (var childHierarchy in childHierarchies)
                {
                    if (childHierarchy is BarkHierarchyData barkHierarchy)
                    {
                        RecurseSplines(read, barkHierarchy, action);
                    }
                }
            }
        }

        #region ISerializationCallbackReceiver Members

        public void OnBeforeSerialize()
        {
            using var scope = APPASERIALIZE.OnBeforeSerialize();
        }

        public void OnAfterDeserialize()
        {
            using var scope = APPASERIALIZE.OnAfterDeserialize();

            Rebuild();
        }

        #endregion

        #region IShapeReadWrite Members

        public abstract IList this[int i] { get; }

        public void Rebuild()
        {
            InitializeSelf();
            ClearSelf();

            var shp = byHierarchyIDAndParentID;

            var tempShapeIDs = new HashSet<int>();
            tempShapeIDs.Add(-1);

            foreach (var shape in this)
            {
                tempShapeIDs.Add(shape.shapeID);
            }

            var removals = new HashSet<int>();

            foreach (var shape in this)
            {
                if ((shape.parentShapeID == -1) && (shape.type != TreeComponentType.Trunk))
                {
                    removals.Add(shape.shapeID);
                }

                if (!tempShapeIDs.Contains(shape.parentShapeID))
                {
                    removals.Add(shape.shapeID);
                }

                if (removals.Contains(shape.parentShapeID))
                {
                    removals.Add(shape.shapeID);
                }
            }

            foreach (var shape in this)
            {
                if (!byID.ContainsKey(shape.shapeID))
                {
                    byID.Add(shape.shapeID, shape);
                }

                if (!byHierarchyID.ContainsKey(shape.hierarchyID))
                {
                    byHierarchyID.Add(shape.hierarchyID, new List<ShapeData>());
                }

                byHierarchyID[shape.hierarchyID].Add(shape);

                if (shape.shapeID != shape.parentShapeID)
                {
                    if (!byParentID.ContainsKey(shape.parentShapeID))
                    {
                        byParentID.Add(shape.parentShapeID, new List<ShapeData>());
                    }

                    byParentID[shape.parentShapeID].Add(shape);
                }

                if (!shp.ContainsKey(shape.hierarchyID))
                {
                    shp.Add(shape.hierarchyID, new Dictionary<int, List<ShapeData>>());
                }

                if (!shp[shape.hierarchyID].ContainsKey(shape.parentShapeID))
                {
                    shp[shape.hierarchyID].Add(shape.parentShapeID, new List<ShapeData>());
                }

                shp[shape.hierarchyID][shape.parentShapeID].Add(shape);
            }

            foreach (var shape in removals)
            {
                DeleteShapeChain(shape, false);
            }
        }

        public abstract IEnumerator<ShapeData> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RecurseSplines(IHierarchyRead read, Action<BarkRecursionData> action)
        {
            RecurseSplines(read, null, action);
        }

        public void RecurseLeaves(IHierarchyRead read, Action<LeafRecursionData> action)
        {
            foreach (var hierarchy in read.GetHierarchies(TreeComponentType.Leaf))
            {
                if (!byHierarchyID.ContainsKey(hierarchy.hierarchyID))
                {
                    continue;
                }

                var childShapes = byHierarchyID[hierarchy.hierarchyID];

                var childLeaves = new List<LeafShapeData>();

                foreach (var childShape in childShapes)
                {
                    if (childShape is LeafShapeData leafShape)
                    {
                        childLeaves.Add(leafShape);
                    }
                }

                for (var j = 0; j < childLeaves.Count; j++)
                {
                    var shape = childLeaves[j];

                    if (shape == null)
                    {
                        continue;
                    }

                    var parentHierarchy =
                        read.GetHierarchyData(hierarchy.parentHierarchyID) as BarkHierarchyData;
                    var parentShape = GetShapeData(shape.parentShapeID) as BarkShapeData;

                    var recursion = new LeafRecursionData(
                        shape,
                        hierarchy as LeafHierarchyData,
                        j,
                        childLeaves.Count,
                        parentHierarchy,
                        parentShape
                    );

                    action(recursion);
                }
            }
        }

        public void RecurseShapes(IHierarchyRead read, Action<GenericRecursionData> action)
        {
            RecurseShapes(read, null, action);
        }

        public int GetHierarchyShapeCount(int hierarchyID, bool visibleOnly)
        {
            if (!byHierarchyID.ContainsKey(hierarchyID))
            {
                byHierarchyID.Add(hierarchyID, new List<ShapeData>());

                Rebuild();
            }

            return GetShapesByHierarchy(hierarchyID).Count(s => !visibleOnly || s.exportGeometry);
        }

        public void UpdateIndividualHierarchyShapeCounts(
            HierarchyData hierarchy,
            HierarchyData parentHierarchy,
            int shapeCount,
            bool rebuildState = true)
        {
            if (!byHierarchyIDAndParentID.ContainsKey(hierarchy.hierarchyID))
            {
                byHierarchyIDAndParentID.Add(hierarchy.hierarchyID, new Dictionary<int, List<ShapeData>>());
            }

            if (!byHierarchyIDAndParentID.ContainsKey(hierarchy.hierarchyID))
            {
                byHierarchyIDAndParentID.Add(hierarchy.hierarchyID, new Dictionary<int, List<ShapeData>>());
            }

            var individualShapesByHierarchyAndParent = byHierarchyIDAndParentID[hierarchy.hierarchyID];

            if ((parentHierarchy != null) && byHierarchyID.ContainsKey(parentHierarchy.hierarchyID))
            {
                var individualParentHierarchyShapes = byHierarchyID[parentHierarchy.hierarchyID];

                for (var i = 0; i < individualParentHierarchyShapes.Count; i++)
                {
                    var parentShape = individualParentHierarchyShapes[i];

                    if (!individualShapesByHierarchyAndParent.ContainsKey(parentShape.shapeID))
                    {
                        individualShapesByHierarchyAndParent.Add(parentShape.shapeID, new List<ShapeData>());
                    }

                    var individualCurrentShapes = individualShapesByHierarchyAndParent[parentShape.shapeID];

                    if (individualCurrentShapes.Count == shapeCount)
                    {
                    }
                    else if (individualCurrentShapes.Count < shapeCount)
                    {
                        var difference = shapeCount - individualCurrentShapes.Count;

                        for (var j = 0; j < difference; j++)
                        {
                            CreateNewShape(hierarchy.type, hierarchy.hierarchyID, parentShape.shapeID);
                        }
                    }
                    else
                    {
                        individualCurrentShapes[0].parentShapeID = parentShape.shapeID;

                        var excessShapeIDs = new List<int>();

                        for (var j = individualCurrentShapes.Count - 1; j >= 0; j--)
                        {
                            excessShapeIDs.Add(individualCurrentShapes[j].shapeID);

                            if ((individualCurrentShapes.Count - excessShapeIDs.Count) == shapeCount)
                            {
                                break;
                            }
                        }

                        DeleteManyShapeChains(excessShapeIDs);
                    }

                    for (var j = 0; j < shapeCount; j++)
                    {
                        individualCurrentShapes[j].parentShapeID = parentShape.shapeID;
                    }
                }
            }
            else
            {
                if (!byHierarchyID.ContainsKey(hierarchy.hierarchyID))
                {
                    byHierarchyID.Add(hierarchy.hierarchyID, new List<ShapeData>());
                }

                var currentHierarchyShapes = byHierarchyID[hierarchy.hierarchyID];

                if ((shapeCount == 0) && (currentHierarchyShapes.Count == 0))
                {
                }
                else if (currentHierarchyShapes.Count == shapeCount)
                {
                }
                else if (currentHierarchyShapes.Count < shapeCount)
                {
                    var difference = shapeCount - currentHierarchyShapes.Count;

                    for (var j = 0; j < difference; j++)
                    {
                        CreateNewShape(hierarchy.type, hierarchy.hierarchyID, -1);
                    }
                }
                else
                {
                    var excessShapeIDs = new List<int>();
                    for (var j = currentHierarchyShapes.Count - 1; j >= shapeCount; j--)
                    {
                        excessShapeIDs.Add(currentHierarchyShapes[j].shapeID);
                    }

                    DeleteManyShapeChains(excessShapeIDs, false);
                }
            }

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public List<ShapeData> GetShapesByHierarchy(HierarchyData hierarchy)
        {
            return GetShapesByHierarchy(hierarchy.hierarchyID);
        }

        public List<ShapeData> GetShapesByHierarchy(int hierarchyID)
        {
            if (hierarchyID == -1)
            {
                return null;
            }

            if (!byHierarchyID.ContainsKey(hierarchyID))
            {
                return new List<ShapeData>();
            }

            var hit = byHierarchyID[hierarchyID];

            for (var i = hit.Count - 1; i >= 0; i--)
            {
                if (hit[i] == null)
                {
                    hit.RemoveAt(i);
                }
            }

            return hit;
        }

        public List<ShapeData> GetShapesByParentShape(int parentShapeID)
        {
            if (parentShapeID == -1)
            {
                return null;
            }

            if (byParentID.ContainsKey(parentShapeID))
            {
                return byParentID[parentShapeID];
            }

            return null;
        }

        public ShapeData CreateNewShape(TreeComponentType type, int hierarchyID, int parentShapeID)
        {
            if (!byHierarchyID.ContainsKey(hierarchyID))
            {
                byHierarchyID.Add(hierarchyID, new List<ShapeData>());
            }

            if (!byHierarchyIDAndParentID.ContainsKey(hierarchyID))
            {
                byHierarchyIDAndParentID.Add(hierarchyID, new Dictionary<int, List<ShapeData>>());
            }

            if (!byHierarchyIDAndParentID[hierarchyID].ContainsKey(parentShapeID))
            {
                byHierarchyIDAndParentID[hierarchyID].Add(parentShapeID, new List<ShapeData>());
            }

            var shapeID = idGenerator.GetNextIdAndIncrement();

            return CreateNewShape(type, hierarchyID, shapeID, parentShapeID);
        }

        public bool ShapeExists(int shapeID)
        {
            if (shapeID == -1)
            {
                return false;
            }

            return byID.ContainsKey(shapeID);
        }

        public bool IsShapeType(ShapeData shape, TreeComponentType type)
        {
            return IsShapeType(shape.shapeID, type);
        }

        public bool IsShapeType(int shapeID, TreeComponentType type)
        {
            if (shapeID == -1)
            {
                return false;
            }

            return byID[shapeID].type == type;
        }

        public TreeComponentType GetShapeType(ShapeData shape)
        {
            return GetShapeType(shape.shapeID);
        }

        public TreeComponentType GetShapeType(int shapeID)
        {
            if (shapeID == -1)
            {
                return default;
            }

            return byID[shapeID].type;
        }

        public IEnumerable<ShapeData> GetShapes(TreeComponentType type)
        {
            return GetShapesInternal(type);
        }

        public ShapeData GetShapeData(ShapeData shape)
        {
            return GetShapeData(shape.shapeID);
        }

        public ShapeData GetShapeData(int shapeID)
        {
            if (shapeID == -1)
            {
                return null;
            }

            if (!byID.ContainsKey(shapeID))
            {
                throw new KeyNotFoundException(
                    ZString.Format("Could not find shape {0} in the collection.", shapeID)
                );
            }

            return byID[shapeID];
        }

        public TS GetShapeDataAs<TS>(ShapeData shape)
            where TS : ShapeData
        {
            return GetShapeDataAs<TS>(shape.shapeID);
        }

        public TS GetShapeDataAs<TS>(int shapeID)
            where TS : ShapeData
        {
            if (shapeID == -1)
            {
                return null;
            }

            return byID[shapeID] as TS;
        }

        public ShapeData GetParentShapeData(ShapeData shape)
        {
            if (shape.parentShapeID == -1)
            {
                return null;
            }

            return GetShapeData(shape.parentShapeID);
        }

        public void DeleteShapeChain(int shapeID, bool rebuildState = true, int depth = 0)
        {
            if (shapeID == -1)
            {
                return;
            }

            if (!byID.ContainsKey(shapeID))
            {
                return;
            }

            var shape = byID[shapeID];

            if (depth > 100)
            {
                throw new NotSupportedException(
                    ZString.Format(
                        "Preventing stack overflow...shape {0} in hierarchy {1}, parent shape {2}.",
                        shapeID,
                        shape.hierarchyID,
                        shape.parentShapeID
                    )
                );
            }

            if (byParentID.ContainsKey(shape.shapeID))
            {
                var children = byParentID[shape.shapeID];

                foreach (var child in children)
                {
                    if (child.shapeID != shape.shapeID)
                    {
                        DeleteShapeChain(child.shapeID, rebuildState, depth + 1);
                    }
                }
            }

            byID.Remove(shapeID);

            RemoveShape(shape);

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public void DeleteManyShapeChains(IEnumerable<int> shapeIDs, bool rebuildState = true)
        {
            foreach (var shapeID in shapeIDs)
            {
                DeleteShapeChain(shapeID, false);
            }

            if (rebuildState)
            {
                Rebuild();
            }
        }

        public void DeleteAllShapeChainsInHierarchy(int hierarchyID, bool rebuildState = true)
        {
            if (hierarchyID == -1)
            {
                return;
            }

            if (!byHierarchyID.ContainsKey(hierarchyID))
            {
                return;
            }

            var s = byHierarchyID[hierarchyID];

            for (var i = 0; i < s.Count; i++)
            {
                DeleteShapeChain(s[i].shapeID, false);
            }

            if (rebuildState)
            {
                Rebuild();
            }
        }

        #endregion
    }
}
