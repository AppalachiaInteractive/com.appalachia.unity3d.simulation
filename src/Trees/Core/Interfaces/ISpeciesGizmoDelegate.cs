using Appalachia.Core.Objects.Root.Contracts;
using Appalachia.Simulation.Trees.Core.Model;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Core.Interfaces
{
    public interface ISpeciesGizmoDelegate : ICrossAssemblySerializable
    {
        #if UNITY_EDITOR
        
        void DrawSpeciesGizmos(
            ISpeciesDataProvider container,
            ModelSelection selection, Transform transform, Mesh mesh);
        
        void DrawSpeciesGizmos(
            ILogDataProvider container,
            LogModelSelection selection, Transform transform, Mesh mesh);

        void DrawNormals(Transform transform, Mesh mesh, int vertexStart, int vertexEnd);

        void DrawWeldGizmo(
            Vector3 rayOrigin,
            Vector3 v0,
            Vector3 v1,
            Vector3 v2);

        void DrawVertex(Transform transform, Mesh mesh, ref int selectedVertex);
        
        #endif
    }
}
