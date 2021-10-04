using UnityEngine;

namespace Appalachia.Simulation.Trees
{
    public interface ITreeModel
    {
#if UNITY_EDITOR
        void OpenSpecies();

        GameObject GameObject { get; }

        bool MissingContainer { get; }
#endif
    }
}
