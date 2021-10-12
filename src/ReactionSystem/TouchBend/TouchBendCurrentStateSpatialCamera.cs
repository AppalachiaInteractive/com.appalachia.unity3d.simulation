using Appalachia.Core.Shading;
using Appalachia.Simulation.ReactionSystem.Cameras;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.TouchBend
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TouchBendCurrentStateSpatialCamera : ReactionSubsystemSingleCameraSingleLocation
    {
        private const string _systemName = "TOUCH_BEND_CURRENT_STATE_SPATIAL";

        public Vector4 mapMinXZ;

        protected override string SubsystemName => _systemName;

        public override bool AutomaticRender => true;

        public override bool IsManualRenderingRequired(SubsystemCameraComponent cam)
        {
            return false;
        }

        protected override void OnBeforeInitialization()
        {
        }

        protected override void OnInitializationStart()
        {
            if (cullingMask == 0)
            {
                cullingMask = LayerMask.GetMask(LayerMask.LayerToName(29));
            }
        }

        protected override void OnInitializationComplete()
        {
        }

        protected override void OnRenderStart()
        {
        }

        protected override void OnRenderComplete()
        {
            Shader.SetGlobalTexture(
                GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_SPATIAL,
                renderTexture
            );

            var targetPosition = cameraComponent.center.transform.position;

            mapMinXZ = new Vector4(targetPosition.x, targetPosition.y, targetPosition.z);

            mapMinXZ.z = -mapMinXZ.z;
            mapMinXZ.w = orthographicSize * 2;
            mapMinXZ.x -= orthographicSize;
            mapMinXZ.z -= orthographicSize;

            Shader.SetGlobalVector(GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_MIN_XZ, mapMinXZ);
        }
    }
}
