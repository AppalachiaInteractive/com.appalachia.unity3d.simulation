using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Unity.Profiling;
using UnityEditor;

namespace Appalachia.Simulation.Trees.UI
{
    [InitializeOnLoad]
    public static class SerializationReconciler
    {
        private const string _PRF_PFX = nameof(SerializationReconciler) + ".";
        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));
        
        static SerializationReconciler()
        {
            EditorApplication.delayCall += Update;
        }

        private static void Update()
        {
            using (_PRF_Update.Auto())
            {
                TreeModel._gizmoRetriever = () => TreeGizmoDelegate.instance;
                TreeIcons.Initialize();
            }
        }
    }
}
