using UnityEngine;

namespace Appalachia.Core.Trees
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
