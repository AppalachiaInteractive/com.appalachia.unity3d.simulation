using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Utility.Extensions;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveAppalachiaObject : AppalachiaObject, IResponsive
    {
#if UNITY_EDITOR

        public void RecordUndo(TreeEditMode mode)
        {
            this.MarkAsModified();
            this.CreateUndoStep(mode.ToString().ToTitleCase());
        }

        public abstract void UpdateSettingsType(ResponsiveSettingsType t);

#endif
    }
}