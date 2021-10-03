using UnityEngine;

namespace Appalachia.Core.Trees
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
