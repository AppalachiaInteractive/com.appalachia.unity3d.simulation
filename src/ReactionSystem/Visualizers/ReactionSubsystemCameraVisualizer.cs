#if UNITY_EDITOR
using Appalachia.Core.Extensions;
using Appalachia.Simulation.ReactionSystem.Contracts;
using Appalachia.Spatial.Visualizers;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.Visualizers
{
    public sealed class
        ReactionSubsystemCameraVisualizer : InstancedIndirectSpatialMapVisualization<
            ReactionSubsystemCameraVisualizer>
    {
        #region Fields and Autoproperties

        [PropertyOrder(-200)] public IReactionSubsystemCamera subsystem;

        #endregion

        /// <inheritdoc />
        protected override bool CanGenerate => subsystem != null;

        /// <inheritdoc />
        protected override bool CanVisualize => (subsystem != null) && (subsystem.RenderTexture != null);

        /// <inheritdoc />
        protected override bool ShouldRegenerate => (subsystem != null) && subsystem.AutomaticRender;

        /// <inheritdoc />
        protected override void GetVisualizationInfo(
            Vector3 position,
            float4 color,
            out float height,
            out Quaternion rotation,
            out Vector3 scale)
        {
            height = position.y;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        /// <inheritdoc />
        protected override void PrepareInitialGeneration()
        {
            texture = subsystem.RenderTexture.ToTexture2D();
        }

        /// <inheritdoc />
        protected override void PrepareSubsequentGenerations()
        {
            texture = subsystem.RenderTexture.ToTexture2D();
        }
    }
}

#endif
