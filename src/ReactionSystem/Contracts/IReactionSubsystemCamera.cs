using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Contracts
{
    public interface IReactionSubsystemCamera : IReactionSubsystem
    {
        bool AutomaticRender { get; }
        bool HideCamera { get; }
        CameraClearFlags ClearFlags { get; }
        Color BackgroundColor { get; }
        LayerMask CullingMask { get; }
        Shader ReplacementShader { get; }
        string ReplacementShaderTag { get; }
        Vector3 CameraDirection { get; }
        Vector3 CameraOffset { get; }
        int OrthographicSize { get; set; }
        bool IsManualRenderingRequired(SubsystemCameraComponent cam);
    }
}
