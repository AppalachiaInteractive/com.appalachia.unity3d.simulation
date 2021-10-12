/*
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Core.ReactionSystem.Base
{
    public abstract class ReactionSubsystemBaseCamera : ReactionSubsystemBase
    {
        [OnValueChanged(nameof(Initialize))]
        public ReactionSubsystemCenter center;

        protected abstract bool AutomaticCamera { get; }
        
        protected abstract bool NonAutomaticRenderReady { get; }

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public Camera renderCamera;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public Shader replacementShader;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public string replacementShaderTag;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public LayerMask cullingMask;

            [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public Vector3 cameraOffset = new Vector3(0f, -1000f, 0f);

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public Vector3 cameraDirection = Vector3.up;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public Color backgroundColor;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public CameraClearFlags clearFlags;

        [FoldoutGroup("Camera")]
        [PropertyRange(1, 4096)]
        [OnValueChanged(nameof(Initialize))]
        [ReadOnly]
        public int orthographicSize = 50;

        [FoldoutGroup("Camera")]
        [OnValueChanged(nameof(Initialize))]
        public bool hideCamera = true;

        public override RenderTexture renderTexture => showRenderTexture ? renderCamera.targetTexture : null;

        protected override bool showRenderTexture => renderCamera != null;

        protected override void TeardownSubsystem()
        {
            if (renderCamera)
            {
                renderCamera.enabled = false;
                
                var rt = renderCamera.targetTexture;
                renderCamera.targetTexture = null;

                if (rt != null)
                {
                    DestroyImmediate(rt);
                }
            }
        }

        private bool renderCameraPresent;
        private bool centerPresent;
        private bool hasReplacementShader;
        private bool shaderReplaced;

        protected override bool InitializeUpdateLoop()
        {
            var successful = false;
           
            centerPresent = center != null;
            renderCameraPresent = renderCamera != null;

            if (centerPresent && renderCameraPresent)
            {
                successful = true;

                hasReplacementShader = replacementShader != null;
            }
            
            return successful;
        }
        
        protected override void DoUpdateLoop()
        {
            OnRenderStarted();
            
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
            else if (NonAutomaticRenderReady)
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

        protected abstract void OnPreInitializationEarly();

        protected abstract void OnPreInitialization();
        
        protected abstract void OnPostInitialization();
        
        protected abstract void OnRenderStarted();
        
        protected abstract void OnRenderCompleted();
        
        protected override void OnInitialization()
        {
            OnPreInitializationEarly();
            
            EnsureAddedToCenter();
            
            if (center == null)
            {
                Debug.LogError("Must assign system center.");
                
                return;
            }
            
            if (string.IsNullOrWhiteSpace(SubsystemName))
            {
                Debug.LogError("Must define system name.");
            }
            
            gameObject.name = SubsystemName;
            
            if (!renderCamera)
            {
                renderCamera = CreateCamera(SubsystemName);
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

            var oldRenderTexture = renderCamera.targetTexture;

            renderCamera.targetTexture = null;
            
            renderCamera.targetTexture = RecreateRenderTexture(oldRenderTexture);
        }

        private Vector3 GetCameraRootPosition()
        {
            if (center)
            {
                return center.GetPosition();
            }
            
            return Vector3.zero;
        }

        protected void EnsureAddedToCenter()
        {
            if (center != null)
            {
                center.ValidateSubsystems();
                
                if (!center.subsystems.Contains(this))
                {
                    center.subsystems.Add(this);
                }
            }
            else
            {
                var centers = FindObjectsOfType<ReactionSubsystemCenter>();

                for (var i = 0; i < centers.Length; i++)
                {
                    var c = centers[i];
                    
                    c.ValidateSubsystems();
                    
                    if (c.subsystems.Contains(this))
                    {
                        center = c;
                    }
                }
            }
        }
    }
}
*/


