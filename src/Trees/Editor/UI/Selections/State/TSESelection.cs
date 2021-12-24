using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;
using Sirenix.OdinInspector.Editor;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public abstract class TSESelection<T, TS> : IBasicSelection 
        where TS : TreeScriptableObjectContainerSelection<T, TS>
        where T : TSEDataContainer
    {
        protected TS _selection;
        protected PropertyTree<TS> _property;
        
        public int hierarchyID;
        public int tab;
        public int subtab;
        public int id;

        public abstract TS selection { get; }
        
        public void DrawAndSelect()
        {
            if (selection.NeedsRefresh)
            {
                selection.Refresh();
            }

            if (_property == null)
            {
                _property = new PropertyTree<TS>(new[] {selection});
            }

            _property.Draw(); 
            _property.ApplyChanges();
        } 
        
        public void Refresh()
        {            
            if (selection.NeedsRefresh)
            {
                selection.Refresh();
            }
        }

        public int HierarchyID
        {
            get => hierarchyID;
            set => hierarchyID = value;
        }
        public int Tab
        {
            get => tab;
            set => tab = value;
        }
        public int ID
        {
            get => id;
            set => id = value;
        }
    }
}