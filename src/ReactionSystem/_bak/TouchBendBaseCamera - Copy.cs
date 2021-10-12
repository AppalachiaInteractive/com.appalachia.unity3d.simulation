/*
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Core.Runtime.TouchBend.Cameras
{
    public abstract class TouchBendBaseCamera : InternalMonoBehaviour
    {
        [OnValueChanged(nameof(InitializeSystem))]
        public TouchBendSystemCenter center;

        [DelayedProperty]
        [OnValueChanged(nameof(InitializeSystem))]
        public string systemName;

        protected abstract string SystemName { get; }

        protected abstract bool AutomaticCamera { get; }

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public Camera renderCamera;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public Shader replacementShader;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public string replacementShaderTag;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public LayerMask cullingMask;

            [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public Vector3 cameraOffset = new Vector3(0f, -1000f, 0f);

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public Vector3 cameraDirection = Vector3.up;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public Color backgroundColor;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public CameraClearFlags clearFlags;

        [FoldoutGroup("Camera")]
        [PropertyRange(1, 4096)]
        [OnValueChanged(nameof(InitializeSystem))]
        [ReadOnly]
        public int orthographicSize = 50;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(InitializeSystem))]
        public bool hideCamera = true;

        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSystem))]
        public RenderTextureFormat renderTextureFormat;

        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSystem))]
        public TouchBendQuality renderTextureQuality = TouchBendQuality.High_1024;

        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSystem))]
        public FilterMode filterMode;

        [FoldoutGroup("Texture")]
        [OnValueChanged(nameof(InitializeSystem))]
        [ValueDropdown(nameof(depths))]
        public int depth = 0;

        private ValueDropdownList<int> depths = new ValueDropdownList<int>()
        {
            {0},
            {8},
            {16},
            {24},
            {32}
        };

        [InlineProperty, ShowInInspector]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        [FoldoutGroup("Preview")]
        [ShowIf(nameof(showRenderTexture))]
        public RenderTexture renderTexture => showRenderTexture ? renderCamera.targetTexture : null;

        private bool showRenderTexture => renderCamera != null;

        private void Awake()
        {
            updateLoopInitialized = false;
            InitializeSystem();
        }

        private void OnEnable()
        {
            updateLoopInitialized = false;
            InitializeSystem();
        }

        private void OnDisable()
        {
            if (renderCamera)
            {
                renderCamera.enabled = false;
                
                var rt = renderCamera.targetTexture;
                renderCamera.targetTexture = null;

                if (rt != null)
                {
                    RenderTexture.DestroyImmediate(rt);
                }
            }
        }

        private bool updateLoopInitialized = false;
        private bool renderCameraPresent = false;
        private bool centerPresent = false;
        private bool hasReplacementShader = false;
        private bool shaderReplaced = false;

        private bool InitializeUpdateLoop()
        {
            if (!updateLoopInitialized)
            {
                centerPresent = center != null;
                renderCameraPresent = renderCamera != null;

                if (centerPresent && renderCameraPresent)
                {
                    updateLoopInitialized = true;

                    hasReplacementShader = replacementShader != null;
                }
            }

            return updateLoopInitialized;
        }
        private void Update()
        {
            try
            {
                var initialized = InitializeUpdateLoop();

                if (!initialized)
                {
                    return;
                }

                var tempCameraPosition = GetCameraRootPosition();
                var pos = tempCameraPosition;
                pos += cameraOffset;

                renderCamera.transform.position = pos;
                renderCamera.transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.forward);

                renderCamera.enabled = AutomaticCamera;

                if (renderCamera.enabled)
                {
                    if (hasReplacementShader && !shaderReplaced)
                    {
                        renderCamera.SetReplacementShader(replacementShader, null);
                        shaderReplaced = true;
                    }
                }
                else
                {
                    if (hasReplacementShader)
                    {
                        renderCamera.RenderWithShader(replacementShader, replacementShaderTag);
                    }
                    else
                    {
                        renderCamera.Render();
                    }
                }

                OnRenderCompleted();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected abstract void OnPreInitialization();
        
        protected abstract void OnPostInitialization();
        
        protected abstract void OnRenderCompleted();
        
        [Button]
        private void InitializeSystem()
        {
            systemName = SystemName;
            gameObject.name = SystemName;
            
            EnsureAddedToCenter();
            
            if (center == null)
            {
                Debug.LogError("Must assign system center.");
                
                return;
            }
            
            if (string.IsNullOrWhiteSpace(systemName))
            {
                Debug.LogError("Must define system name.");
            }
            
            if (!renderCamera)
            {
                renderCamera = CreateCamera(systemName);
            }

            if (renderCamera)
            {
                renderCamera.enabled = true;
            }

            OnPreInitialization();
            
            UpdateCamera();

            OnPostInitialization();
        }

        private Camera CreateCamera(string cameraName)
        {
            if (cameraName == null)
            {
                Debug.LogError("Must define camera name.");
                return null;
            }
            
            var cameraTransform = transform.Find(cameraName);

            Camera tbc;

            if (!cameraTransform)
            {
                var cameraGO = new GameObject(cameraName);
                
                cameraGO.transform.SetParent(transform, false);
                cameraGO.transform.position = cameraOffset;
                cameraGO.transform.rotation = Quaternion.LookRotation(cameraDirection);

                var tempEditorCamera = cameraGO.AddComponent<Camera>();
                tempEditorCamera.orthographicSize = 50f;
                tbc = tempEditorCamera;
            }
            else
            {
                tbc = cameraTransform.gameObject.GetComponent<Camera>();
            }

            UpdateCamera();

            return tbc;
        }

        private void UpdateCamera()
        {
            if (!renderCamera)
            {
                return;
            }

            if (hideCamera)
            {
                renderCamera.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                renderCamera.gameObject.hideFlags = HideFlags.None;
            }

            renderCamera.farClipPlane = 10000;
            renderCamera.nearClipPlane = -10000;
            renderCamera.depth = -100;
            renderCamera.clearFlags = clearFlags;
            renderCamera.backgroundColor = backgroundColor;
            renderCamera.renderingPath = RenderingPath.Forward;
            renderCamera.useOcclusionCulling = true;
            renderCamera.orthographic = true;
            renderCamera.orthographicSize = orthographicSize;
            renderCamera.cullingMask = cullingMask;
            renderCamera.allowMSAA = false;
            renderCamera.allowHDR = false;
            renderCamera.stereoTargetEye = StereoTargetEyeMask.None;

            var textureResolution = TouchBendHelpers.GetTouchBendQualityPixelResolution(renderTextureQuality);

            var rt = new RenderTexture(
                textureResolution,
                textureResolution,
                24,
                renderTextureFormat,
                RenderTextureReadWrite.Linear
            )
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = filterMode,
                depth = depth,
                autoGenerateMips = false,
                hideFlags = HideFlags.DontSave
            };

            var oldRenderTexture = renderCamera.targetTexture;
            renderCamera.targetTexture = rt;

            if (oldRenderTexture)
            {
                DestroyImmediate(oldRenderTexture);
            }
        }

        private Vector3 GetCameraRootPosition()
        {
            if (center)
            {
                return center.transform.position;
            }
            
            return Vector3.zero;
        }

        protected void EnsureAddedToCenter()
        {
            if (center != null)
            {
                if (center.systems == null)
                {
                    center.systems = new List<TouchBendBaseCamera>();
                }

                if (!center.systems.Contains(this))
                {
                    center.systems.Add(this);
                }
            }
            else
            {
                var centers = GameObject.FindObjectsOfType<TouchBendSystemCenter>();

                for (var i = 0; i < centers.Length; i++)
                {
                    var c = centers[i];
                    
                    if (c.systems.Contains(this))
                    {
                        center = c;
                    }
                }
            }
        }
    }
}
*/


