using Appalachia.Core.Shading;
using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.TouchBend
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class
        TouchBendCurrentStateMaskCamera : ReactionSubsystemSingleCameraSingleLocation<
            TouchBendCurrentStateMaskCamera>
    {
        #region Constants and Static Readonly

        private const string SYSTEM_NAME = "TOUCH_BEND_CURRENT_STATE_MASK";

        #endregion

        /// <inheritdoc />
        public override bool AutomaticRender => true;

        /// <inheritdoc />
        protected override string SubsystemName => SYSTEM_NAME;

        /// <inheritdoc />
        public override bool IsManualRenderingRequired(SubsystemCameraComponent cam)
        {
            return false;
        }

        /// <inheritdoc />
        protected override void OnBeforeInitialization()
        {
        }

        /// <inheritdoc />
        protected override void OnInitializationComplete()
        {
        }

        /// <inheritdoc />
        protected override void OnInitializationStart()
        {
            if (CullingMask == 0)
            {
                CullingMask = LayerMask.GetMask(LayerMask.LayerToName(29));
            }

            cameraComponent.renderCamera.depth = -90;
        }

        /// <inheritdoc />
        protected override void OnRenderComplete()
        {
            Shader.SetGlobalTexture(GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_MASK, RenderTexture);
        }

        /// <inheritdoc />
        protected override void OnRenderStart()
        {
        }
    }
}
