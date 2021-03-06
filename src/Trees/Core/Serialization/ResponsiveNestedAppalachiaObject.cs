using Appalachia.Core.Objects.Scriptables;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Utility.Extensions;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveNestedAppalachiaObject<T> : NestedAppalachiaObject<T>, IResponsive
        where T : ResponsiveNestedAppalachiaObject<T>
    {
#if UNITY_EDITOR

        public void RecordUndo(TreeEditMode mode)
        {
            var objects = new Object[] { this };

            MarkAsModified();

            UnityEditor.Undo.RegisterCompleteObjectUndo(objects, mode.ToString().ToTitleCase());
        }

        public abstract void UpdateSettingsType(ResponsiveSettingsType t);

#endif
    }
}
