using Appalachia.Core.Extensions;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveNestedScriptableObject<T, TP> : NestedScriptableObject<T, TP>, IResponsive
        where T : ResponsiveNestedScriptableObject<T, TP>
        where TP : ResponsiveAppalachiaScriptableObject<TP>
    {
        #if UNITY_EDITOR
        
        public void RecordUndo(TreeEditMode mode)
        {
            var objects = new Object[] {this};

            EditorUtility.SetDirty(this);

            Undo.RegisterCompleteObjectUndo(objects, mode.ToString().ToTitleCase());
        }
        
        public abstract void UpdateSettingsType(ResponsiveSettingsType t);
        
        #endif
    }
}