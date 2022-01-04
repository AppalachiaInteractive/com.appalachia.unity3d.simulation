using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Extensions;
using Appalachia.Core.Layers;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Core.Shading;
using Appalachia.Core.Types.Enums;
using Appalachia.Globals.Shading;
using Appalachia.Jobs.TextureJobs.Jobs.GaussianBlur;
using Appalachia.Jobs.TextureJobs.Structures;
using Appalachia.Utility.Async;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Jobs;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Appalachia.Simulation.ReactionSystem.TouchBend.Data
{
    [ExecuteAlways]
    [CallStaticConstructorInEditor]
    public sealed class TouchBendQuad : AppalachiaBehaviour<TouchBendQuad>
    {
        static TouchBendQuad()
        {
            RegisterDependency<GSR>(i => _GSR = i);
        }

        #region Static Fields and Autoproperties

        private static GSR _GSR;

        #endregion

        #region Runtime

        [FoldoutGroup("Runtime")]
        [InlineProperty]
        [HideLabel]
        [InlineEditor(InlineEditorModes.GUIOnly, InlineEditorObjectFieldModes.Hidden)]
        [PropertyOrder(100)]
        [OnValueChanged(nameof(Refresh), true)]
        public TouchBendComponentInfo info;

        [FoldoutGroup("Runtime")]
        [PropertyOrder(110)]
        public LayerSelection renderLayer = 29;

        [FoldoutGroup("Runtime")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(RecreateShot))]
#endif
        [PropertyOrder(-90)]
        public List<Renderer> references = new();

        [FoldoutGroup("Runtime")]
        [PropertyOrder(-80)]
        [ReadOnly]
        public Bounds bounds;

        [ReadOnly] public Vector3 lastPosition;

        [ReadOnly] public float velocity;

        [ReadOnly] public float targetVelocity;

        [PropertyRange(0.01f, .99f)]
        public float velocityChangeSpeed = 0.5f;

        [HideInInspector] public Material renderMaterial;

        [HideInInspector] public MeshFilter quadFilter;
        [HideInInspector] public MeshRenderer quadRenderer;

        private const string _PRF_PFX = nameof(TouchBendQuad) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

#if UNITY_EDITOR
                await initializer.Do(this, nameof(InitializeCamera), InitializeCamera);
#endif
                await initializer.Do(this, nameof(RecalculateTouchBendBounds), RecalculateTouchBendBounds);
                await initializer.Do(this, nameof(InitializeQuadComponents), InitializeQuadComponents);
                await initializer.Do(this, nameof(RepositionQuad), RepositionQuad);
                await initializer.Do(this, nameof(UpdateRenderParameters), () => UpdateRenderParameters());
            }
        }

        protected override async AppaTask WhenDisabled()

        {
            using (_PRF_OnDisable.Auto())
            {
                await base.WhenDisabled();

                if (quadRenderer)
                {
                    quadRenderer.enabled = false;
                }
            }
        }

        private static readonly ProfilerMarker _PRF_InitializeQuadComponents =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeQuadComponents));

        private static readonly ProfilerMarker _PRF_OnDisable =
            new ProfilerMarker(_PRF_PFX + nameof(OnDisable));

        private static readonly ProfilerMarker _PRF_Refresh = new ProfilerMarker(_PRF_PFX + nameof(Refresh));

        private void Refresh()
        {
            using (_PRF_Refresh.Auto())
            {
                RecalculateTouchBendBounds();
                InitializeQuadComponents();
                RepositionQuad();
                UpdateRenderParameters();
            }
        }

        private void InitializeQuadComponents()
        {
            using (_PRF_InitializeQuadComponents.Auto())
            {
                GameObject go = null;

                if (quadFilter != null)
                {
                    go = quadFilter.gameObject;
                }

                if (quadRenderer != null)
                {
                    go = quadRenderer.gameObject;
                }

                if (go == null)
                {
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var child = transform.GetChild(i);

                        if (string.Equals(
                                child.name,
                                "TOUCHBEND",
                                StringComparison.InvariantCultureIgnoreCase
                            ))
                        {
                            go = child.gameObject;

                            break;
                        }
                    }

                    if (go == null)
                    {
                        go = new GameObject("TOUCHBEND", typeof(MeshFilter), typeof(MeshRenderer));

                        go.transform.SetParent(transform, false);
                    }
                }

                if (quadFilter == null)
                {
                    quadFilter = go.GetComponent<MeshFilter>();

                    if (quadFilter == null)
                    {
                        quadFilter = go.AddComponent<MeshFilter>();
                    }
                }

                if (quadRenderer == null)
                {
                    quadRenderer = go.GetComponent<MeshRenderer>();

                    if (quadRenderer == null)
                    {
                        quadRenderer = go.AddComponent<MeshRenderer>();
                    }
                }

                if (renderMaterial == null)
                {
                    renderMaterial = new Material(_GSR.touchbendQuadGenerator);
                }

                quadFilter.sharedMesh = _GSR.touchbendQuadMesh;

                quadRenderer.enabled = true;
                quadRenderer.gameObject.layer = renderLayer;
                quadRenderer.materials = new[] { renderMaterial };
                quadRenderer.receiveShadows = false;
                quadRenderer.shadowCastingMode = ShadowCastingMode.Off;
                quadRenderer.lightProbeUsage = LightProbeUsage.Off;
            }
        }

        private static readonly ProfilerMarker _PRF_UpdateRenderParameters =
            new ProfilerMarker(_PRF_PFX + nameof(UpdateRenderParameters));

        private void UpdateRenderParameters(bool applyTexture = true)
        {
            using (_PRF_UpdateRenderParameters.Auto())
            {
                if (applyTexture)
                {
                    renderMaterial.SetTexture("_MainTex", info.texture);
                }

                renderMaterial.SetFloat(GSC.TOUCHBEND._STRENGTH, info.strength);
                renderMaterial.SetFloat(GSC.TOUCHBEND._MIN_OLD,  info.minOld);
                renderMaterial.SetFloat(GSC.TOUCHBEND._MAX_OLD,  info.maxOld);
                renderMaterial.SetFloat(GSC.TOUCHBEND._VELOCITY, velocity);
            }
        }

        private static readonly ProfilerMarker _PRF_RecalculateTouchBendBounds =
            new ProfilerMarker(_PRF_PFX + nameof(RecalculateTouchBendBounds));

        private void RecalculateTouchBendBounds()
        {
            using (_PRF_RecalculateTouchBendBounds.Auto())
            {
                bounds = new Bounds { center = transform.position };

                foreach (var r in references)
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
        }

        private static readonly ProfilerMarker _PRF_RepositionQuad =
            new ProfilerMarker(_PRF_PFX + nameof(RepositionQuad));

        private void RepositionQuad()
        {
            using (_PRF_RepositionQuad.Auto())
            {
                var t = quadRenderer.transform;

                var center = bounds.center;
                center.y = bounds.min.y;
                t.position = center;

                var localPosition = t.localPosition;
                localPosition += Vector3.forward * info.offset;

                t.localPosition = localPosition;

                t.localScale = Vector3.one * (info.size * info.scale);
            }
        }

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                if (!DependenciesAreReady || !FullyInitialized)
                {
                    return;
                }
                
#if UNITY_EDITOR

                if (finishShot)
                {
                    finishShot = false;
                    finishShot2 = true;
                }
                else if (finishShot2)
                {
                    CameraToRender();

                    SaveTexture();

                    BlurAsset();
                    BlurAsset();

                    dirty = false;
                    SetRenderTexture();
                    finishShot2 = false;
                }

#endif

                var t = transform;
                var p = t.position;

                targetVelocity = (p - lastPosition).magnitude / Time.deltaTime;

                velocity = Mathf.Lerp(velocity, targetVelocity, velocityChangeSpeed);
                renderMaterial.SetFloat(GSC.TOUCHBEND._VELOCITY, velocity);

                lastPosition = p;
            }
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        private static ProfilerMarker _PRF_InitializeCamera = new ProfilerMarker(nameof(InitializeCamera));
        private static ProfilerMarker _PRF_SetRenderTexture = new ProfilerMarker(nameof(SetRenderTexture));
        private static ProfilerMarker _PRF_ClearCamera = new ProfilerMarker(nameof(ClearCamera));
        private static ProfilerMarker _PRF_ClearRender = new ProfilerMarker(nameof(ClearRender));
        private static ProfilerMarker _PRF_DrawModel = new ProfilerMarker(nameof(DrawModel));
        private static ProfilerMarker _PRF_CameraToRender = new ProfilerMarker(nameof(CameraToRender));
        private static ProfilerMarker _PRF_RecreateShot = new ProfilerMarker(nameof(RecreateShot));
        private static ProfilerMarker _PRF_CreateQuadCamera = new ProfilerMarker(nameof(CreateQuadCamera));

        private static ProfilerMarker _PRF_UpdateCameraParameters =
            new ProfilerMarker(nameof(UpdateCameraParameters));

        private static ProfilerMarker _PRF_SaveTexture = new ProfilerMarker(nameof(SaveTexture));
        private static ProfilerMarker _PRF_BlurAsset = new ProfilerMarker(nameof(BlurAsset));
        private static ProfilerMarker _PRF_DilateAsset = new ProfilerMarker(nameof(DilateAsset));
        private static ProfilerMarker _PRF_AutoLevelAsset = new ProfilerMarker(nameof(AutoLevelAsset));
        private static ProfilerMarker _PRF_EnsureReadable = new ProfilerMarker(nameof(EnsureReadable));

        public Camera quadCam;

        public static TextureFormat textureFormat = TextureFormat.RGB24;
        public static RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

        [FoldoutGroup("Editor")]
        [OnValueChanged(nameof(RecreateShot))]
        [PropertyOrder(0)]
        public RenderQuality renderQuality = RenderQuality.Mid_256;

        [FoldoutGroup("Editor")]
        [OnValueChanged(nameof(RecreateShot))]
        [PropertyOrder(-100)]
        public bool addChildRenderers = true;

        [FoldoutGroup("Editor")]
        [PropertyOrder(1)]
        [OnValueChanged(nameof(RecreateShot))]
        [PropertyRange(.1f, 5f)]
        public float cameraScale = 1f;

        [FoldoutGroup("Editor")]
        [PropertyOrder(2)]
        [OnValueChanged(nameof(RecreateShot))]
        [PropertyRange(1f, 5f)]
        public float modelScale = 1f;

        [FoldoutGroup("Editor")]
        [PropertyOrder(2)]
        [OnValueChanged(nameof(RecreateShot))]
        public Vector3 modelCutoff;

        [FoldoutGroup("Editor")]
        [PropertyOrder(-70)]
        public bool hideCamera = true;

        [FoldoutGroup("Editor/Textures")]

        //[TabGroup("Editor/Textures/A", "Asset", order:1)]
        [ShowInInspector]
        [HideLabel]
        [InlineProperty]
        [PreviewField(ObjectFieldAlignment.Center, Height = 256)]
        [PropertyOrder(21)]
        public Texture2D savedTexture => info?.texture;

        //[TabGroup("Editor/Textures/A", "Camera", order:3)]
        //[ShowInInspector]
        //[HideLabel]
        //[InlineProperty]
        //[PreviewField(ObjectFieldAlignment.Center, Height = 256), PropertyOrder(23)]
        //public RenderTexture cameraTexture => quadCam?.targetTexture;

        [HideInInspector] public RenderTexture render;

        [HideInInspector] public Material generationMaterial;

        [HideInInspector] public Material flipperMaterial;

        [FoldoutGroup("Editor")]
        [PropertyOrder(-40)]
        [OnValueChanged(nameof(RecreateShot))]
        public LayerSelection generationLayer = 30;

        [BoxGroup("Editor/Asset")]
        [PropertyOrder(89)]
        [ToggleLeft]
        public bool dirty;

        public void InitializeCamera()
        {
            using (_PRF_InitializeCamera.Auto())
            {
                if (quadCam == null)
                {
                    quadCam = CreateQuadCamera();
                }

                UpdateCameraParameters();

                quadCam.enabled = true;

                if (references == null)
                {
                    references = new List<Renderer>();
                }

                for (var i = references.Count - 1; i >= 0; i--)
                {
                    if (references[i] == null)
                    {
                        references.RemoveAt(i);
                    }
                }

                if (info == null)
                {
                    var runtimeName = ZString.Format("{0}_INTERACTION-INFO", gameObject.name);

                    info = TouchBendComponentInfo.LoadOrCreateNew(runtimeName);
                }
            }
        }

        private bool finishShot;
        private bool finishShot2;

        private void SetRenderTexture()
        {
            using (_PRF_SetRenderTexture.Auto())
            {
                renderMaterial.SetTexture("_MainTex", dirty ? render : info.texture);
                UpdateRenderParameters(false);
            }
        }

        private void ClearCamera()
        {
            using (_PRF_ClearCamera.Auto())
            {
                var srgbWrite = GL.sRGBWrite;
                GL.sRGBWrite = false;

                var active = RenderTexture.active;
                RenderTexture.active = quadCam.targetTexture;
                GL.Clear(true, true, quadCam.backgroundColor);
                GL.sRGBWrite = srgbWrite;

                dirty = true;
                SetRenderTexture();
                RenderTexture.active = active;
            }
        }

        private void ClearRender()
        {
            using (_PRF_ClearRender.Auto())
            {
                var srgbWrite = GL.sRGBWrite;
                GL.sRGBWrite = false;

                var active = RenderTexture.active;
                RenderTexture.active = render;
                GL.Clear(true, true, quadCam.backgroundColor);

                GL.sRGBWrite = srgbWrite;

                dirty = true;
                SetRenderTexture();
                RenderTexture.active = active;
            }
        }

        private void DrawModel()
        {
            using (_PRF_DrawModel.Auto())
            {
                foreach (var reference in references)
                {
                    Mesh mesh;

                    if (reference is MeshRenderer mr)
                    {
                        var mf = reference.GetComponent<MeshFilter>();
                        mesh = mf.sharedMesh;
                    }
                    else
                    {
                        var smr = reference as SkinnedMeshRenderer;
                        mesh = smr.sharedMesh;
                    }

                    var t = reference.transform;

                    var positionMatrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);

                    if (modelCutoff == Vector3.zero)
                    {
                        modelCutoff = bounds.extents;
                    }

                    if (generationMaterial == null)
                    {
                        generationMaterial =
                            new Material(_GSR.touchbendQuadGenerator) { enableInstancing = true };
                    }

                    generationMaterial.SetVector(GSC.TOUCHBEND._GENERATION_MASK, modelCutoff);
                    generationMaterial.SetFloat(GSC.TOUCHBEND._GENERATION_SCALE, modelScale);
                    generationMaterial.SetTexture(
                        GSC.TOUCHBEND._GENERATION_BACKGROUND,
                        _GSR.touchbendQuadBase
                    );

                    Graphics.DrawMesh(mesh, positionMatrix, generationMaterial, generationLayer, quadCam);
                }

                dirty = true;
                SetRenderTexture();
            }
        }

        private void CameraToRender()
        {
            using (_PRF_CameraToRender.Auto())
            {
                if (flipperMaterial == null)
                {
                    flipperMaterial = new Material(_GSR.textureFlipper);
                }

                Graphics.Blit(quadCam.targetTexture, render, flipperMaterial);

                dirty = true;
                SetRenderTexture();
            }
        }

        [BoxGroup("Editor/Asset")]
        [Button]
        [PropertyOrder(89)]
        public void RecreateShot()
        {
            using (_PRF_RecreateShot.Auto())
            {
                if (quadCam)
                {
                    InitializeCamera();

                    if (addChildRenderers)
                    {
                        references = gameObject.GetComponentsInChildren<Renderer>()
                                               .Where(r => r.name != "TOUCHBEND")
                                               .ToList();
                    }
                    else
                    {
                        references = gameObject.GetComponents<Renderer>()
                                               .Where(r => r.name != "TOUCHBEND")
                                               .ToList();
                    }

                    UpdateCameraParameters();
                    InitializeQuadComponents();

                    dirty = true;
                    SetRenderTexture();

                    var textureResolution = renderQuality.GetRenderQualityPixelResolution();

                    var newCamTexture = new RenderTexture(
                        textureResolution,
                        textureResolution,
                        24,
                        renderTextureFormat,
                        RenderTextureReadWrite.Linear
                    )
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Bilinear,
                        autoGenerateMips = true,
                        hideFlags = HideFlags.DontSave
                    };

                    var newRenderTexture = new RenderTexture(
                        textureResolution,
                        textureResolution,
                        24,
                        renderTextureFormat,
                        RenderTextureReadWrite.Linear
                    )
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Bilinear,
                        autoGenerateMips = true,
                        hideFlags = HideFlags.DontSave
                    };

                    var oldCamTexture = quadCam.targetTexture;
                    var oldRenderTexture = render;

                    quadCam.targetTexture = newCamTexture;
                    render = newRenderTexture;

                    if (oldCamTexture)
                    {
                        DestroyImmediate(oldCamTexture);
                    }

                    if (oldRenderTexture)
                    {
                        DestroyImmediate(oldRenderTexture);
                    }

                    ClearCamera();

                    DrawModel();

                    finishShot = true;
                }

                dirty = false;
                SetRenderTexture();
            }
        }

        public Camera CreateQuadCamera()
        {
            using (_PRF_CreateQuadCamera.Auto())
            {
                var camTransform = transform.Find("Quad Generator Cam");

                Camera tbc;

                if (!camTransform)
                {
                    var camObject = new GameObject("Quad Generator Cam");

                    camObject.transform.SetParent(transform, false);
                    camObject.transform.position = Vector3.zero;
                    camObject.transform.rotation = Quaternion.LookRotation(Vector3.up);

                    var tempEditorCamera = camObject.AddComponent<Camera>();
                    tempEditorCamera.orthographicSize = 50f;
                    tbc = tempEditorCamera;
                }
                else
                {
                    tbc = camTransform.gameObject.GetComponent<Camera>();
                }

                return tbc;
            }
        }

        private void UpdateCameraParameters()
        {
            using (_PRF_UpdateCameraParameters.Auto())
            {
                var position = bounds.center;
                position.y = -1000;

                quadCam.transform.position = position;
                quadCam.transform.rotation = Quaternion.LookRotation(Vector3.up, transform.forward);

                quadCam.orthographicSize = bounds.extents.magnitude * (1f / cameraScale);
                info.size = bounds.size.magnitude;

                if (hideCamera)
                {
                    quadCam.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
                else
                {
                    quadCam.gameObject.hideFlags = HideFlags.None;
                }

                quadCam.farClipPlane = 10000;
                quadCam.nearClipPlane = -10000;
                quadCam.depth = -100;
                quadCam.clearFlags = CameraClearFlags.Nothing;
                quadCam.backgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.0f);
                quadCam.renderingPath = RenderingPath.Forward;
                quadCam.useOcclusionCulling = true;
                quadCam.orthographic = true;
                quadCam.cullingMask = generationLayer.Mask;
                quadCam.allowMSAA = false;
                quadCam.allowHDR = false;
                quadCam.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }

        private void SaveTexture()
        {
            using (_PRF_SaveTexture.Auto())
            {
                var rt = RenderTexture.active;

                var newTextureName = ZString.Format("{0}_INTERACTION-MASK", gameObject.name);

                if (info.texture != null)
                {
                    newTextureName = info.texture.name;
                }

                var textureResolution = renderQuality.GetRenderQualityPixelResolution();

                info.texture = new Texture2D(textureResolution, textureResolution, textureFormat, true, true)
                {
                    name = newTextureName, alphaIsTransparency = false, filterMode = FilterMode.Point
                };

                var srgbWrite = GL.sRGBWrite;
                GL.sRGBWrite = false;

                try
                {
                    RenderTexture.active = render;

                    var rect = new Rect(0, 0, textureResolution, textureResolution);

                    info.texture.ReadPixels(rect, 0, 0);
                    info.texture.Apply(true);

                    RenderTexture.active = rt;

                    info.texture = AssetDatabaseManager.SaveTextureAssetToFile(this, info.texture);
                }
                finally
                {
                    RenderTexture.active = rt;

                    GL.sRGBWrite = srgbWrite;
                }

                EnsureReadable(info.texture);

                dirty = false;
                SetRenderTexture();
            }
        }

        private bool canDilate => (info != null) && (info.texture != null);

        [BoxGroup("Editor/Asset")]
        [PropertyOrder(91)]
        [PropertyRange(1, 24)]
        [EnableIf(nameof(canDilate))]
        public int blurRadius = 5;

        [PropertyOrder(92)]
        [Button]
        [HorizontalGroup("Editor/Asset/ASSET")]
        [EnableIf(nameof(canDilate))]
        private void BlurAsset()
        {
            using (_PRF_BlurAsset.Auto())
            {
                if (info.texture.format == TextureFormat.RG16)
                {
                    var rawdata = info.texture.GetRawTextureData<RG16>();

                    new GaussianBlurRG16Job(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGHalf)
                {
                    var rawdata = info.texture.GetRawTextureData<RGHalf>();

                    new GaussianBlurRGHalfJob(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGFloat)
                {
                    var rawdata = info.texture.GetRawTextureData<RGFloat>();

                    new GaussianBlurRGFloatJob(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGB24)
                {
                    var rawdata = info.texture.GetRawTextureData<RGB24>();

                    new GaussianBlurRGB24Job(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGBA32)
                {
                    var rawdata = info.texture.GetRawTextureData<RGBA32>();

                    new GaussianBlurRGBA32Job(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGBAHalf)
                {
                    var rawdata = info.texture.GetRawTextureData<RGBAHalf>();

                    new GaussianBlurRGBAHalfJob(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }
                else if (info.texture.format == TextureFormat.RGBAFloat)
                {
                    var rawdata = info.texture.GetRawTextureData<RGBAFloat>();

                    new GaussianBlurRGBAFloatJob(rawdata, info.texture.width, info.texture.height, blurRadius)
                       .Schedule()
                       .Complete();
                }

                info.texture.Apply();

                info.texture = AssetDatabaseManager.SaveTextureAssetToFile(this, info.texture);

                dirty = false;
                SetRenderTexture();
            }
        }

        [PropertyOrder(93)]
        [Button]
        [HorizontalGroup("Editor/Asset/ASSET")]
        [EnableIf(nameof(canDilate))]
        private void DilateAsset()
        {
            using (_PRF_DilateAsset.Auto())
            {
                var avg = Vector3.zero;
                var avgCount = 0;

                EnsureReadable(info.texture);

                var cols = info.texture.GetPixels();
                var copyCols = info.texture.GetPixels();
                var borderIndices = new HashSet<int>();
                var indexBuffer = new HashSet<int>();
                var w = info.texture.width;
                var h = info.texture.height;
                for (var i = 0; i < cols.Length; i++)
                {
                    if (cols[i].a < 0.5f)
                    {
                        for (var x = -1; x < 2; x++)
                        {
                            for (var y = -1; y < 2; y++)
                            {
                                var index = i + (y * w) + x;
                                if ((index >= 0) &&
                                    (index < cols.Length) &&
                                    (cols[index].a >
                                     0.5f)) // if a non transparent pixel is near the transparent one, add the transparent pixel index to border indices
                                {
                                    borderIndices.Add(i);
                                    goto End;
                                }
                            }
                        }

                        End: ;
                    }
                    else
                    {
                        var col = cols[i];
                        avg += new Vector3(col.r, col.g, col.b);
                        avgCount += 1;
                    }
                }

                for (var iteration = 0; iteration < 8; iteration++)
                {
                    foreach (var i in borderIndices)
                    {
                        var meanCol = Color.black;
                        var opaqueNeighbours = 0;
                        for (var x = -1; x < 2; x++)
                        {
                            for (var y = -1; y < 2; y++)
                            {
                                var index = i + (y * w) + x;
                                if ((index >= 0) && (index < cols.Length) && (index != i))
                                {
                                    if (cols[index].a > 0.5f)
                                    {
                                        cols[index].a = 1;
                                        meanCol += cols[index];
                                        opaqueNeighbours++;
                                    }
                                    else
                                    {
                                        indexBuffer.Add(index);
                                    }
                                }
                            }
                        }

                        cols[i] = meanCol / opaqueNeighbours;
                    }

                    indexBuffer.ExceptWith(borderIndices);

                    borderIndices = indexBuffer;
                    indexBuffer = new HashSet<int>();
                }

                var empty = Color.black;
                empty.a = 0f;
                var bv = avg / avgCount;
                var background = new Color(bv.x, bv.y, bv.z);
                background.a = 0f;
                for (var i = 0; i < cols.Length; i++)
                {
                    cols[i].a = copyCols[i].a;

                    if (cols[i] == empty)
                    {
                        cols[i] = background;
                        cols[i].a = 0f;
                    }
                }

                info.texture.SetPixels(cols);

                info.texture.Apply();

                info.texture = AssetDatabaseManager.SaveTextureAssetToFile(this, info.texture);

                dirty = false;
                SetRenderTexture();
            }
        }

        [PropertyOrder(93)]
        [Button]
        [HorizontalGroup("Editor/Asset/ASSET")]
        [EnableIf(nameof(canDilate))]
        private void AutoLevelAsset()
        {
            using (_PRF_AutoLevelAsset.Auto())
            {
                EnsureReadable(info.texture);

                var colors = info.texture.GetPixels();

                var maxAlpha = 0.0f;
                var minAlpha = 1.0f;

                for (var i = 0; i < colors.Length; i++)
                {
                    var color = colors[i];

                    if (color.r > maxAlpha)
                    {
                        maxAlpha = color.r;
                    }

                    if (color.r < minAlpha)
                    {
                        minAlpha = color.r;
                    }
                }

                info.minOld = minAlpha;
                info.maxOld = maxAlpha;

                UpdateRenderParameters();
            }
        }

        private static void EnsureReadable(Texture2D tex)
        {
            using (_PRF_EnsureReadable.Auto())
            {
                if (tex.isReadable)
                {
                    return;
                }

                var path = AssetDatabaseManager.GetAssetPath(tex);

                if (path == null)
                {
                    return;
                }

                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                importer.isReadable = true;
                importer.SaveAndReimport();
            }
        }

#endif

        #endregion
    }
}
