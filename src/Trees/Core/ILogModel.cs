using UnityEngine;

namespace Appalachia.Simulation.Trees.Core
{
    public interface ILogModel
    {
#if UNITY_EDITOR
        void OpenLog();

        GameObject GameObject { get; }

        bool MissingContainer { get; }
#endif
    }
}
