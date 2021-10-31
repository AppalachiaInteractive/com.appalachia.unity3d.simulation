using Appalachia.Core.Extensions;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveSingletonAppalachiaObject<T> : SingletonAppalachiaObject<T>, IResponsive
        where T : SingletonAppalachiaObject<T>
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