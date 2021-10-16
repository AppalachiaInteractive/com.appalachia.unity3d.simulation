using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Core.Interfaces;
using Appalachia.Simulation.Trees.Core.Model;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.Gizmos
{
    public class TreeGizmoDelegate : SelfSavingSingletonScriptableObject<TreeGizmoDelegate>, ISpeciesGizmoDelegate
    {
        private TreeGizmoDelegate()
        {
        }
        
        public void DrawSpeciesGizmos(
            ISpeciesDataProvider container,
            ModelSelection selection, 
            Transform transform,
            Mesh mesh)
        {
            var tree = container as TreeDataContainer;

            try
            {
                var stage = tree.individuals[selection.individualSelection][selection.ageSelection][selection
                        .stageSelection];

                if (stage != null)
                {
                    TreeGizmoDrawer.Draw(
                        tree.species.hierarchies,
                        stage.shapes,
                        transform,
                        mesh
                    );
                }
            }
            catch
            {
                if (tree != null)
                {
                    tree.RebuildStructures();
                }
            }
        }
        
        public void DrawSpeciesGizmos(
            ILogDataProvider container,
            LogModelSelection selection, 
            Transform transform,
            Mesh mesh)
        {
            var log = container as LogDataContainer;

            try
            {
                if (log is not null)
                {
                    var i = log.logInstances[selection.instanceSelection];

                    if (i != null)
                    {
                        TreeGizmoDrawer.Draw(
                            log.log.hierarchies,
                            i.shapes,
                            transform,
                            mesh
                        );
                    }
                }
            }
            catch
            {
                if (log != null)
                {
                    log.RebuildStructures();
                }
            }
        }

        public void DrawNormals(Transform transform, Mesh mesh, int vertexStart, int vertexEnd)
        {
            TreeGizmoDrawer.DrawNormals_Range(transform, mesh, vertexStart, vertexEnd);
        }

        public void DrawWeldGizmo(
            Vector3 rayOrigin,
            Vector3 v0,
            Vector3 v1,
            Vector3 v2)
        {
            TreeGizmoDrawer.DrawWeldGizmo(rayOrigin, v0, v1, v2);
        }

        public void DrawVertex(Transform transform, Mesh mesh, ref int selectedVertex)
        {
            TreeGizmoDrawer.DrawVertex(transform, mesh, ref selectedVertex);
        }
    }
}
