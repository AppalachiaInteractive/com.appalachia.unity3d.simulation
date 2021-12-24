using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Core.Model;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.UI.Gizmos;
using Unity.Profiling;
using UnityEditor;

namespace Appalachia.Simulation.Trees.UI
{
    [CallStaticConstructorInEditor]
    public static class SerializationReconciler
    {
        static SerializationReconciler()
        {
            EditorApplication.delayCall += Update;
            TreeGizmoDelegate.InstanceAvailable += i => _treeGizmoDelegate = i;
        }

        #region Static Fields and Autoproperties

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));
        private static TreeGizmoDelegate _treeGizmoDelegate;

        #endregion

        private static void Update()
        {
            using (_PRF_Update.Auto())
            {
                TreeModel._gizmoRetriever = () => _treeGizmoDelegate;
                TreeIcons.Initialize();
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(SerializationReconciler) + ".";

        #endregion
    }
}
