using Appalachia.Core.Shading;
using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.TouchBend
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TouchBendCurrentStateSpatialCamera : ReactionSubsystemSingleCameraSingleLocation<
        TouchBendCurrentStateSpatialCamera>
    {
        #region Constants and Static Readonly

        private const string SYSTEM_NAME = "TOUCH_BEND_CURRENT_STATE_SPATIAL";

        #endregion

        #region Fields and Autoproperties

        public Vector4 mapMinXZ;

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
        }

        /// <inheritdoc />
        protected override void OnRenderComplete()
        {
            Shader.SetGlobalTexture(GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_SPATIAL, RenderTexture);

            var targetPosition = cameraComponent.center.transform.position;

            mapMinXZ = new Vector4(targetPosition.x, targetPosition.y, targetPosition.z);

            mapMinXZ.z = -mapMinXZ.z;
            mapMinXZ.w = OrthographicSize * 2;
            mapMinXZ.x -= OrthographicSize;
            mapMinXZ.z -= OrthographicSize;

            Shader.SetGlobalVector(GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_MIN_XZ, mapMinXZ);
        }

        /// <inheritdoc />
        protected override void OnRenderStart()
        {
        }
    }
}
