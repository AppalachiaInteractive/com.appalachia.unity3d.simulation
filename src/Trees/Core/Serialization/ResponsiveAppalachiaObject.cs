using Appalachia.Simulation.Trees.Core.Editing;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Utility.Extensions;
using Unity.Profiling;

namespace Appalachia.Simulation.Trees.Core.Serialization
{
    public abstract class ResponsiveAppalachiaObject : AppalachiaTreeObject, IResponsive
    {
#if UNITY_EDITOR

        private static readonly ProfilerMarker _PRF_RecordUndo =
            new ProfilerMarker(_PRF_PFX + nameof(RecordUndo));

        private const string _PRF_PFX = nameof(ResponsiveAppalachiaObject) + ".";

        public void RecordUndo(TreeEditMode mode)
        {
            using (_PRF_RecordUndo.Auto())
            {
                MarkAsModified();
                this.CreateUndoStep(mode.ToString().ToTitleCase());
            }
        }

        public abstract void UpdateSettingsType(ResponsiveSettingsType t);

#endif
    }
}
