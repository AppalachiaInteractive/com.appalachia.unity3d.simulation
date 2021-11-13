using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Utility.Extensions;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveAppalachiaObject<T> : AppalachiaObject<T>, IResponsive
        where T : AppalachiaObject<T>
    {
#if UNITY_EDITOR

        public void RecordUndo(TreeEditMode mode)
        {
            var objects = new Object[] {this};

            UnityEditor.EditorUtility.SetDirty(this);

            UnityEditor.Undo.RegisterCompleteObjectUndo(objects, mode.ToString().ToTitleCase());
        }

        public abstract void UpdateSettingsType(ResponsiveSettingsType t);

#endif
    }
}