using Appalachia.Core.Types.Enums;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Contracts
{
    public interface IReactionSubsystem
    {
        FilterMode FilterMode { get; }
        GameObject GameObject { get; }
        int Depth { get; }
        ReactionSystem MainSystem { get; }
        RenderQuality RenderTextureQuality { get; }
        RenderTexture RenderTexture { get; }
        RenderTextureFormat RenderTextureFormat { get; }
        Transform Transform { get; }
        void InitializeSubsystem(ReactionSystem system, int groupIndex);
        void UpdateGroupIndex(int i);
    }
}
