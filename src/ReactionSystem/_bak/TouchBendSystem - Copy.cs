/*
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Core.Runtime.TouchBend
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TouchBendSystem : MonoBehaviour
    {
        private static TouchBendSystem _instance;

        public static float previewSize;

        [OnValueChanged(nameof(InitializeSystem))]
        public TouchBendSystemCenter center;

        [OnValueChanged(nameof(InitializeSystem))]
        public LayerSelection invisibleLayer = 29;

        [OnValueChanged(nameof(InitializeSystem))]
        public bool autoselectCamera = true;

        [OnValueChanged(nameof(InitializeSystem))]
        [DisableIf(nameof(autoselectCamera))]
        public Camera selectedCamera;

        [HideInInspector] public Camera touchBendCamera;

        [OnValueChanged(nameof(InitializeSystem))]
        public float cameraYPosition = -1000f;

        [OnValueChanged(nameof(InitializeSystem))]
        public TouchBendQuality touchBendQuality = TouchBendQuality.High_1024;

        [PropertyRange(10, 500)]
        [OnValueChanged(nameof(InitializeSystem))]
        public int orthographicSize = 50;

        [OnValueChanged(nameof(InitializeSystem))]
        public bool hideTouchBendCamera = true;

        [TitleGroup("Runtime Data")]
        [ReadOnly]
        public Vector4 position;

        public static TouchBendSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TouchBendSystem>();
                }

                return _instance;
            }
        }

        [TitleGroup("Runtime Data")]
        [ShowInInspector]
        [HideLabel]
        [InlineProperty]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        public RenderTexture texture => touchBendCamera.targetTexture;

        private void Awake()
        {
            InitializeSystem();
        }

        private void OnEnable()
        {
            InitializeSystem();
        }

        private void OnDisable()
        {
            if (touchBendCamera)
            {
                touchBendCamera.enabled = false;
            }

            if (touchBendCamera)
            {
                var rt = touchBendCamera.targetTexture;
                touchBendCamera.targetTexture = null;
                DestroyImmediate(rt);
            }
        }

        public void UpdateCamera()
        {
            if (touchBendCamera)
            {
                UpdateCameraParameters(touchBendCamera, invisibleLayer);

                touchBendCamera.cullingMask = 1 << invisibleLayer;
                touchBendCamera.orthographicSize = orthographicSize;

                var textureResolution = TouchBendHelpers.GetTouchBendQualityPixelResolution(touchBendQuality);
                var rt = new RenderTexture(
                    textureResolution,
                    textureResolution,
                    24,
                    RenderTextureFormat.ARGBHalf,
                    RenderTextureReadWrite.Linear
                )
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    autoGenerateMips = true,
                    hideFlags = HideFlags.DontSave
                };

                var oldRenderTexture = touchBendCamera.targetTexture;
                touchBendCamera.targetTexture = rt;

                if (oldRenderTexture)
                {
                    DestroyImmediate(oldRenderTexture);
                }
            }
        }

        private void Update()
        {
            if (touchBendCamera && (center || selectedCamera))
            {
                var tempCameraPosition = GetCameraPosition();
                var pos = tempCameraPosition;

                pos.y = cameraYPosition;

                touchBendCamera.transform.position = pos;
            }

            touchBendCamera.Render();

            UpdateShaders();
        }

        private void UpdateShaders()
        {
            Shader.SetGlobalTexture(GSC.TOUCHBEND._TOUCHBEND_MAP, touchBendCamera.targetTexture);

            Transform target;

            if (center == null)
            {
                target = touchBendCamera.transform;
            }
            else
            {
                target = center.transform;
            }

            position = target.position;
            position.z = -position.z;
            position.w = touchBendCamera.orthographicSize * 2;
            position.x -= touchBendCamera.orthographicSize;
            position.z -= touchBendCamera.orthographicSize;

            Shader.SetGlobalVector(GSC.TOUCHBEND._TOUCHBEND_CENTER_POSITION, position);
        }

        public void InitializeSystem()
        {
            if (center == null)
            {
                center = FindObjectOfType<TouchBendSystemCenter>();
            }

            if (touchBendCamera)
            {
                touchBendCamera.enabled = true;
            }

            if (!touchBendCamera)
            {
                touchBendCamera = CreateTouchBendCamera();
            }

            UpdateTouchBendCamera();

            if (autoselectCamera)
            {
                selectedCamera = Camera.main;
            }

            UpdateCamera();
        }

        public void UpdateTouchBendCamera()
        {
            if (touchBendCamera && hideTouchBendCamera)
            {
                touchBendCamera.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                touchBendCamera.gameObject.hideFlags = HideFlags.None;
            }
        }

        public Camera CreateTouchBendCamera(string cameraName = "TouchBendCamera")
        {
            var touchBendCameraTransform = transform.Find(cameraName);

            Camera tbc;

            if (!touchBendCameraTransform)
            {
                var touchBendCameraObject = new GameObject(cameraName);
                touchBendCameraObject.transform.SetParent(transform, false);
                touchBendCameraObject.transform.position = Vector3.zero;
                touchBendCameraObject.transform.rotation = Quaternion.LookRotation(Vector3.up);

                var tempEditorCamera = touchBendCameraObject.AddComponent<Camera>();
                tempEditorCamera.orthographicSize = 50f;
                tbc = tempEditorCamera;
            }
            else
            {
                tbc = touchBendCameraTransform.gameObject.GetComponent<Camera>();
            }

            UpdateCameraParameters(tbc, invisibleLayer);

            return tbc;
        }

        private static void UpdateCameraParameters(Camera camera, int layer)
        {
            camera.farClipPlane = 10000;
            camera.nearClipPlane = -10000;
            camera.depth = -100;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.clear;
            camera.renderingPath = RenderingPath.Forward;
            camera.useOcclusionCulling = true;
            camera.orthographic = true;
            camera.cullingMask = 1 << layer;
            camera.allowMSAA = false;
            camera.allowHDR = false;
            camera.stereoTargetEye = StereoTargetEyeMask.None;
        }

        public Vector3 GetCameraPosition()
        {
            if (center)
            {
                return center.transform.position;
            }

            if (selectedCamera)
            {
                return selectedCamera.transform.position;
            }

            return Vector3.zero;
        }
    }
}
*/


