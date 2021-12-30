using Appalachia.Core.Attributes;
using Appalachia.Core.Extensions;
using Appalachia.Core.Shading;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.ReactionSystem.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.ReactionSystem.TouchBend
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [CallStaticConstructorInEditor]
    public class TouchBendCurrentStateMotion : ReactionSubsystemBase
    {
        #region Constants and Static Readonly

        private const string _systemName = "TOUCH_BEND_CURRENT_STATE_MOTION";

        #endregion

        static TouchBendCurrentStateMotion()
        {
            GSR.InstanceAvailable += i => _GSR = i;
        }

        #region Static Fields and Autoproperties

        private static GSR _GSR;

        #endregion

        #region Fields and Autoproperties

        public TouchBendCurrentStateMaskCamera maskCamera;
        public TouchBendCurrentStateSpatialCamera spatialCamera;

        public Material mat;

        [PropertyRange(.5f, .999f)]
        public float motionDecay = .99f;

        [PropertyRange(0.0f, 0.1f)]
        public float motionCutoff = 0.01f;

        [ReadOnly] public Vector2 uvOffset;

        private Vector4 _previousMapMinXZ;

        private RenderTexture _previousRenderTexture;

/*#if UNITY_EDITOR
        public Texture2D testTexture;
#endif*/
        private RenderTexture _renderTexture;
        private int motionCutoffID;

        private int motionDecayID;
        private int motionUVOffsetID;

        #endregion

        public override RenderTexture renderTexture => _renderTexture;

        protected override bool showRenderTexture => _renderTexture != null;

        protected override string SubsystemName => _systemName;

        protected override void DoUpdateLoop()
        {
            var currentMapMinXZ = spatialCamera.mapMinXZ;

            var difference = currentMapMinXZ - _previousMapMinXZ;
            uvOffset = new Vector2(difference.x, difference.z) / currentMapMinXZ.w;

            mat.SetFloat(motionDecayID,  motionDecay);
            mat.SetFloat(motionCutoffID, motionCutoff);
            mat.SetVector(motionUVOffsetID, uvOffset);
            Graphics.Blit(_previousRenderTexture, _renderTexture, mat);

            Shader.SetGlobalTexture(GSC.TOUCHBEND._TOUCHBEND_CURRENT_STATE_MAP_MOTION, _renderTexture);

            Graphics.Blit(_renderTexture, _previousRenderTexture);
            _previousMapMinXZ = spatialCamera.mapMinXZ;
        }

        protected override bool InitializeUpdateLoop()
        {
            if (maskCamera == null)
            {
                maskCamera = FindObjectOfType<TouchBendCurrentStateMaskCamera>();
            }

            if (spatialCamera == null)
            {
                spatialCamera = FindObjectOfType<TouchBendCurrentStateSpatialCamera>();
            }

            if (maskCamera == null)
            {
                return false;
            }

            if (_GSR.touchbendMovement == null)
            {
                return false;
            }

            if (mat == null)
            {
                mat = new Material(_GSR.touchbendMovement);
            }

            _renderTexture = _renderTexture.Recreate(
                renderTextureQuality,
                renderTextureFormat,
                filterMode,
                depth
            );
            _previousRenderTexture = new RenderTexture(_renderTexture);
            motionDecayID = GSPL.Get(GSC.TOUCHBEND._MOTION_DECAY);
            motionCutoffID = GSPL.Get(GSC.TOUCHBEND._MOTION_CUTOFF);
            motionUVOffsetID = GSPL.Get(GSC.TOUCHBEND._MOTION_UV_OFFSET);

            return true;
        }

        protected override void OnInitialization()
        {
        }

        protected override void TeardownSubsystem()
        {
        }
    }
}
