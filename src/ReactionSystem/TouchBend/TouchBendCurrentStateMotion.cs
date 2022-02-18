using Appalachia.Core.Attributes;
using Appalachia.Core.Extensions;
using Appalachia.Core.Objects.Availability;
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
    public class TouchBendCurrentStateMotion : ReactionSubsystemBase<TouchBendCurrentStateMotion>
    {
        #region Constants and Static Readonly

        private const string SYSTEM_NAME = "TOUCH_BEND_CURRENT_STATE_MOTION";

        #endregion

        static TouchBendCurrentStateMotion()
        {
            RegisterInstanceCallbacks.For<TouchBendCurrentStateMotion>()
                                     .When.Object<GSR>()
                                     .IsAvailableThen(i => _GSR = i);
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

        /// <inheritdoc />
        public override RenderTexture RenderTexture => _renderTexture;

        /// <inheritdoc />
        protected override bool ShowRenderTexture => _renderTexture != null;

        /// <inheritdoc />
        protected override string SubsystemName => SYSTEM_NAME;

        /// <inheritdoc />
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

        /// <inheritdoc />
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
                RenderTextureQuality,
                RenderTextureFormat,
                FilterMode,
                Depth
            );
            _previousRenderTexture = new RenderTexture(_renderTexture);
            motionDecayID = GSPL.Get(GSC.TOUCHBEND._MOTION_DECAY);
            motionCutoffID = GSPL.Get(GSC.TOUCHBEND._MOTION_CUTOFF);
            motionUVOffsetID = GSPL.Get(GSC.TOUCHBEND._MOTION_UV_OFFSET);

            return true;
        }

        /// <inheritdoc />
        protected override void OnInitialization()
        {
        }

        /// <inheritdoc />
        protected override void TeardownSubsystem()
        {
        }
    }
}
