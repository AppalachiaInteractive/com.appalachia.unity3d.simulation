#region

using System;
using Appalachia.Audio;
using Appalachia.Audio.Components;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Behaviours;
using Appalachia.Core.Scriptables;
using Appalachia.Core.Shading;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Core.Metadata.Wind;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

#endregion



namespace Appalachia.Simulation.Wind
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class GlobalWindManager : SingletonAppalachiaBehaviour<GlobalWindManager>
    {
        private const string _PRF_PFX = nameof(GlobalWindManager) + ".";
        private static readonly ProfilerMarker _PRF_Awake = new(_PRF_PFX + "Awake");
        private static readonly ProfilerMarker _PRF_Start = new(_PRF_PFX + "Start");
        private static readonly ProfilerMarker _PRF_OnEnable = new(_PRF_PFX + "OnEnable");
        private static readonly ProfilerMarker _PRF_FixedUpdate = new(_PRF_PFX + "FixedUpdate");
        private static readonly ProfilerMarker _PRF_Update = new(_PRF_PFX + "Update");
        private static readonly ProfilerMarker _PRF_LateUpdate = new(_PRF_PFX + "LateUpdate");
        private static readonly ProfilerMarker _PRF_OnDisable = new(_PRF_PFX + "OnDisable");
        private static readonly ProfilerMarker _PRF_OnDestroy = new(_PRF_PFX + "OnDestroy");
        private static readonly ProfilerMarker _PRF_Reset = new(_PRF_PFX + "Reset");
        private static readonly ProfilerMarker _PRF_OnDrawGizmos = new(_PRF_PFX + "OnDrawGizmos");

        private static readonly ProfilerMarker _PRF_OnDrawGizmosSelected =
            new(_PRF_PFX + "OnDrawGizmosSelected");

        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_RecreateArrowComponents =
            new(_PRF_PFX + nameof(RecreateArrowComponents));

        private static readonly ProfilerMarker _PRF_GetCurrentAudioStrength =
            new(_PRF_PFX + nameof(GetCurrentAudioStrength));

        private static readonly ProfilerMarker _PRF_InitializeAudioPlugins =
            new(_PRF_PFX + nameof(InitializeAudioPlugins));

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties));

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetDefaults =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetDefaults");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_GetCurrent =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".GetCurrent");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_AddLatest =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".AddLatest");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetLatest =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetLatest");

        private static readonly ProfilerMarker
            _PRF_SetGlobalShaderProperties_SetLatest_RealtimeUpdate = new(_PRF_PFX +
                nameof(SetGlobalShaderProperties) +
                ".SetLatest.RealtimeUpdate");

        private static readonly ProfilerMarker
            _PRF_SetGlobalShaderProperties_SetLatest_NonRealtimeUpdate = new(_PRF_PFX +
                nameof(SetGlobalShaderProperties) +
                ".SetLatest.NonRealtimeUpdate");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetShaderProps =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetShaderProps");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetShaderTexture =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetShaderTexture");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_ApplyProperties =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".ApplyProperties");

        private static readonly ProfilerMarker _PRF_LerpValues = new(_PRF_PFX + nameof(LerpValues));

        [FoldoutGroup("Wind/Runtime")]
        [PropertyRange(0f, 1.0f)]
        [SerializeField]
        public float audioInfluence = .5f;

        [FoldoutGroup("Wind/Runtime")]
        [PropertyRange(0f, .01f)]
        [SerializeField]
        public float volumeLightVelocityInfluence = .001f;

        [FoldoutGroup("Wind/Runtime")]
        [PropertyRange(0f, .01f)]
        [SerializeField]
        public float fogVelocityInfluence = .001f;

        [FoldoutGroup("Wind/Runtime")]
        [PropertyRange(0f, 1.0f)]
        [SerializeField]
        public float gustAmplitude;

        [FoldoutGroup("Wind/Runtime")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [SerializeField]
        public float gustAmplitudeActual;

        [FoldoutGroup("Wind/Runtime/Heading")]
        [SerializeField]
        public float nextHeadingUpdateTime;

        [FoldoutGroup("Wind/Runtime/Heading")]
        [SerializeField]
        public Quaternion targetHeading;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [SerializeField]
        public bool useRealGustAudio = true;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [DisableIf(nameof(useRealGustAudio))]
        [SerializeField]
        public float gustAudioEffect = 0.9f;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [EnableIf(nameof(useRealGustAudio))]
        [SerializeField]
        public AudioMixerGroup gustAudioGroup;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [LabelText("Raw")]
        [SmartLabel]
        public float gustAudioStrengthActual;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [LabelText("Very High")]
        [SmartLabel]
        public float gustAudioStrengthActualVeryHigh;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [LabelText("High")]
        [SmartLabel]
        public float gustAudioStrengthActualHigh;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [LabelText("Med")]
        [SmartLabel]
        public float gustAudioStrengthActualMid;

        [FoldoutGroup("Wind/Runtime/Audio")]
        [PropertyRange(0f, 1.0f)]
        [ReadOnly]
        [LabelText("Low")]
        [SmartLabel]
        public float gustAudioStrengthActualLow;

        [FoldoutGroup("Wind/Runtime/Audio/Tracking")]
        [FoldoutGroup("Wind/Runtime/Audio/Tracking/Very High")]
        [InlineProperty]
        [HideLabel]
        public AudioSignalSmoothAnalyzer veryHigh;

        [FoldoutGroup("Wind/Runtime/Audio/Tracking/High")]
        [InlineProperty]
        [HideLabel]
        public AudioSignalSmoothAnalyzer high;

        [FoldoutGroup("Wind/Runtime/Audio/Tracking/Mid")]
        [InlineProperty]
        [HideLabel]
        public AudioSignalSmoothAnalyzer mid;

        [FoldoutGroup("Wind/Runtime/Audio/Tracking/Low")]
        [InlineProperty]
        [HideLabel]
        public AudioSignalSmoothAnalyzer low;

        [FoldoutGroup("Wind/Runtime")]
        public bool debug;

        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideIf(nameof(debug))]
        public GlobalWindParameters parameters;

        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf(nameof(debug))]
        public GlobalWindParameters debugParameters;

        [HideInInspector] public GameObject _arrow;
        [HideInInspector] public MeshFilter _meshFilter;
        [HideInInspector] public MeshRenderer _meshRenderer;
        [NonSerialized] private bool _initialized;

        /*[TitleGroup("System Center")]
        public GameObject reference;*/

        [TitleGroup("Wind")]
        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        public float3 WindDirection => _transform.forward;

        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        [SuffixLabel("m/s")]
        public float3 WindSpeed => gustAudioStrengthActual * parameters.maximumWindSpeed;

        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        [SmartLabel]
        [SuffixLabel("m/s")]
        public float3 WindVelocity => WindDirection * WindSpeed;

        /// <summary>
        ///     Fw = pd A
        ///     Fw = 1/2 ρ v2 A
        ///     where
        ///     Fw = wind force (N)
        ///     A = surface area (m2)
        ///     pd = dynamic pressure  (Pa)
        ///     ρ = density of air (kg/m3)
        ///     v = wind speed (m/s)
        /// </summary>
        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        public float3 WindDynamicPressure =>
            .5f * parameters.airDensity.densityKGPerCubicMeter * WindVelocity * WindVelocity;

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                CheckSetup();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    ExecuteHeadingStepUpdate(false);
                }
#endif

                var param = debug ? debugParameters : parameters;

#if !UNITY_EDITOR
            if (param.realtimeUpdate)
            {
#endif
                if (!_initialized)
                {
                    Initialize();
                }

                if (_meshRenderer.sharedMaterial != param.arrowMaterial)
                {
                    _meshFilter.sharedMesh = param.arrowMesh;
                    _meshRenderer.sharedMaterial = param.arrowMaterial;
                }

                _meshRenderer.enabled = param.showArrow;

                ToggleHideShowArrowComponents(param.showArrowComponents);

                var arrowT = _arrow.transform;
                arrowT.localPosition = param.arrowOffset;
                arrowT.localRotation = Quaternion.identity;
                arrowT.localScale = param.arrowScale;

                /*if (reference != null)
                {
                    var position = reference.transform.position;
                    position.y = 100f;
                    
                    transform.position = position;
                }*/

#if UNITY_EDITOR
                if (!Application.isPlaying || param.realtimeUpdate)
                {
#endif
                    SetGlobalShaderProperties(Time.deltaTime, param);

                    var enviro = EnviroSky.instance;

                    if (enviro != null)
                    {
                        Shader.SetGlobalFloat(
                            GSPL.Get(GSC.SKY._GLOBAL_SOLAR_TIME),
                            Mathf.Clamp(EnviroSky.instance.GameTime.solarTime, 0.0f, 1f)
                        );
                        EnviroSky.instance.windIntensity = gustAudioStrengthActual;

                        //EnviroSky.instance.windVolumeLightIntensity = volumeLightVelocityInfluence;
                        //EnviroSky.instance.windFogIntensity = fogVelocityInfluence;
                        var zone = EnviroSky.instance.Components.windZone;
                        zone.windMain = gustAudioStrengthActual;
                        var t = transform;
                        zone.transform.SetPositionAndRotation(t.position, t.rotation);
                    }
#if UNITY_EDITOR
                }
#endif
#if !UNITY_EDITOR
            }
#endif
            }
        }

        private void FixedUpdate()
        {
            using (_PRF_FixedUpdate.Auto())
            {
                CheckSetup();

                ExecuteHeadingStepUpdate(false);
            }
        }

        public override void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                base.Initialize();
                
                _initialized = true;

                gameObject.name = "Global Wind Manager";

                GSR.instance.InitializeShaderReferences();
            }
        }

        [Button]
        private void RecreateArrowComponents()
        {
            using (_PRF_RecreateArrowComponents.Auto())
            {
                if (_arrow == null)
                {
                    _arrow = new GameObject("Arrow");
                    _arrow.transform.SetParent(transform);
                }

                if (_meshFilter == null)
                {
                    _meshFilter = _arrow.GetComponent<MeshFilter>();
                }

                if (_meshFilter == null)
                {
                    _meshFilter = _arrow.AddComponent<MeshFilter>();
                }

                if (_meshRenderer == null)
                {
                    _meshRenderer = _arrow.GetComponent<MeshRenderer>();
                }

                if (_meshRenderer == null)
                {
                    _meshRenderer = _arrow.AddComponent<MeshRenderer>();
                }
            }
        }

        private void ToggleHideShowArrowComponents(bool show)
        {
            _arrow.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
            _meshFilter.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
            _meshRenderer.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        }

        private void CheckSetup()
        {
#if UNITY_EDITOR

            if (debugParameters == null)
            {
                debugParameters = AppalachiaObject.LoadOrCreateNew<GlobalWindParameters>(
                    "GLOBAL-WIND-PARAMS_DEBUG",
                    false,
                    false
                );
            }

            if (parameters == null)
            {
                parameters = AppalachiaObject.LoadOrCreateNew<GlobalWindParameters>(
                    "GLOBAL-WIND-PARAMS",
                    false,
                    false
                );
            }
#endif
        }

        [Button]
        [FoldoutGroup("Wind/Runtime/Heading")]
        private void UpdateHeadingNow()
        {
            ExecuteHeadingStepUpdate(true);
        }

        private void ExecuteHeadingStepUpdate(bool forceUpdate)
        {
            var time = Time.time;
            if ((time > nextHeadingUpdateTime) || forceUpdate)
            {
                var update = Random.Range(
                    parameters.headingUpdateRange.x,
                    parameters.headingUpdateRange.y
                );

                var rotation = Quaternion.AngleAxis(update, Vector3.up);

                targetHeading = _transform.rotation * rotation;
                nextHeadingUpdateTime = time +
                                        Random.Range(
                                            parameters.headingChangeInterval.x,
                                            parameters.headingChangeInterval.y
                                        );
            }
            else
            {
                _transform.rotation = Quaternion.Slerp(
                    _transform.rotation,
                    targetHeading,
                    parameters.headingUpdateSpeed
                );
            }
        }

        private static float GetCurrentAudioStrength(
            AudioSignalSmoothAnalyzer analyzer,
            bool clamped = true)
        {
            using (_PRF_GetCurrentAudioStrength.Auto())
            {
                var i = GAC.ChannelMonitor_GetLoudnessData_dB(analyzer.referenceID);
                var mag = GAC.dBToNormalized(
                    i,
                    GAC.WIND.WIND_THRESHOLD_LOW_DB,
                    GAC.WIND.WIND_THRESHOLD_HIGH_DB,
                    clamp: clamped
                );

                return mag;
            }
        }

        private void InitializeAudioPlugins(AudioMixer mixer)
        {
            using (_PRF_InitializeAudioPlugins.Auto())
            {
                if (veryHigh == null)
                {
                    veryHigh = new AudioSignalSmoothAnalyzer();
                }

                if (veryHigh.referenceID <= 0)
                {
                    veryHigh.referenceID = (int) GAC.Lookup(
                        mixer,
                        GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_VERYHIGH
                    );
                }

                if (high == null)
                {
                    high = new AudioSignalSmoothAnalyzer();
                }

                if (high.referenceID <= 0)
                {
                    high.referenceID = (int) GAC.Lookup(
                        mixer,
                        GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_HIGH
                    );
                }

                if (mid == null)
                {
                    mid = new AudioSignalSmoothAnalyzer();
                }

                if (mid.referenceID <= 0)
                {
                    mid.referenceID = (int) GAC.Lookup(
                        mixer,
                        GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_MID
                    );
                }

                if (low == null)
                {
                    low = new AudioSignalSmoothAnalyzer();
                }

                if (low.referenceID <= 0)
                {
                    low.referenceID = (int) GAC.Lookup(
                        mixer,
                        GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_LOW
                    );
                }
            }
        }

        private void SetGlobalShaderProperties(float timeStep, GlobalWindParameters param)
        {
            using (_PRF_SetGlobalShaderProperties.Auto())
            {
                InitializeAudioPlugins(gustAudioGroup.audioMixer);

                float currentAudioStrength;
                float currentAudioStrengthVeryHigh;
                float currentAudioStrengthHigh;
                float currentAudioStrengthMid;
                float currentAudioStrengthLow;

                using (_PRF_SetGlobalShaderProperties_SetDefaults.Auto())
                {
                    // ReSharper disable once RedundantAssignment
                    currentAudioStrength = gustAudioEffect;
                    currentAudioStrengthVeryHigh = gustAudioEffect;
                    currentAudioStrengthHigh = gustAudioEffect;
                    currentAudioStrengthMid = gustAudioEffect;
                    currentAudioStrengthLow = gustAudioEffect;
                }

                using (_PRF_SetGlobalShaderProperties_GetCurrent.Auto())
                {
                    if ((gustAudioGroup != null) && useRealGustAudio)
                    {
                        currentAudioStrengthVeryHigh = GetCurrentAudioStrength(veryHigh);
                        currentAudioStrengthHigh = GetCurrentAudioStrength(high);
                        currentAudioStrengthMid = GetCurrentAudioStrength(mid);
                        currentAudioStrengthLow = GetCurrentAudioStrength(low);
                    }
                }

                using (_PRF_SetGlobalShaderProperties_AddLatest.Auto())
                {
                    veryHigh.Add(currentAudioStrengthVeryHigh);
                    high.Add(currentAudioStrengthHigh);
                    mid.Add(currentAudioStrengthMid);
                    low.Add(currentAudioStrengthLow);
                }

                using (_PRF_SetGlobalShaderProperties_SetLatest.Auto())
                {
                    gustAudioStrengthActualVeryHigh =
                        param.realtimeUpdate ? veryHigh.avg : veryHigh.last;
                    gustAudioStrengthActualHigh = param.realtimeUpdate ? high.avg : high.last;
                    gustAudioStrengthActualMid = param.realtimeUpdate ? mid.avg : mid.last;
                    gustAudioStrengthActualLow = param.realtimeUpdate ? low.avg : low.last;

                    currentAudioStrength = math.max(
                        math.max(
                            math.max(gustAudioStrengthActualVeryHigh, gustAudioStrengthActualHigh),
                            gustAudioStrengthActualMid
                        ),
                        gustAudioStrengthActualLow
                    );

                    if (param.realtimeUpdate)
                    {
                        using (_PRF_SetGlobalShaderProperties_SetLatest_RealtimeUpdate.Auto())
                        {
                            LerpValues(
                                timeStep,
                                param.gustStep,
                                gustAmplitude,
                                param.gustAmplitudeSharpness,
                                ref gustAmplitudeActual
                            );

                            LerpValues(
                                timeStep,
                                param.audioStep,
                                currentAudioStrength,
                                param.audioSharpness,
                                ref gustAudioStrengthActual
                            );
                        }
                    }
                    else
                    {
                        using (_PRF_SetGlobalShaderProperties_SetLatest_NonRealtimeUpdate.Auto())
                        {
                            gustAmplitudeActual = gustAmplitude;
                            gustAudioStrengthActual = currentAudioStrength;
                        }
                    }
                }

                using (_PRF_SetGlobalShaderProperties_SetShaderProps.Auto())
                {
                    Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_AUDIO_INFLUENCE), audioInfluence);
                    Shader.SetGlobalVector(GSPL.Get(GSC.WIND._WIND_DIRECTION), _transform.forward);
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_BASE_AMPLITUDE),
                        param.baseAmplitude
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_BASE_TO_GUST_RATIO),
                        param.baseToGustRatio
                    );

                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AMPLITUDE),
                        gustAmplitudeActual
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AUDIO_STRENGTH),
                        gustAudioStrengthActual
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AUDIO_STRENGTH_VERYHIGH),
                        gustAudioStrengthActualVeryHigh
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AUDIO_STRENGTH_HIGH),
                        gustAudioStrengthActualHigh
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AUDIO_STRENGTH_MID),
                        gustAudioStrengthActualMid
                    );
                    Shader.SetGlobalFloat(
                        GSPL.Get(GSC.WIND._WIND_GUST_AUDIO_STRENGTH_LOW),
                        gustAudioStrengthActualLow
                    );
                }

                using (_PRF_SetGlobalShaderProperties_SetShaderTexture.Auto())
                {
                    if ((param.gustTexture == null) || (param.gustContrast <= 0))
                    {
                        Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_GUST_TEXTURE_ON), 0.0f);
                    }
                    else
                    {
                        Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_GUST_TEXTURE_ON), 1.0f);

                        if (param.gustTexture == null)
                        {
                            Shader.SetGlobalTexture(
                                GSPL.Get(GSC.WIND._WIND_GUST_TEXTURE),
                                Texture2D.whiteTexture
                            );
                        }
                        else
                        {
                            Shader.SetGlobalTexture(
                                GSPL.Get(GSC.WIND._WIND_GUST_TEXTURE),
                                param.gustTexture
                            );
                        }

                        Shader.SetGlobalFloat(
                            GSPL.Get(GSC.WIND._WIND_GUST_CONTRAST),
                            param.gustContrast
                        );
                    }
                }

                using (_PRF_SetGlobalShaderProperties_ApplyProperties.Auto())
                {
                    param.trunks?.ApplyProperties();
                    param.branches?.ApplyProperties();
                    param.leaves?.ApplyProperties();
                    param.plants?.ApplyProperties();
                    param.grass?.ApplyProperties();
                }
            }
        }

        private static void LerpValues(
            float timeStep,
            Vector2 stepRange,
            float target,
            float sharpness,
            ref float current)
        {
            using (_PRF_LerpValues.Auto())
            {
                var scaledStepRange = timeStep * stepRange;

                var newTarget = Mathf.Lerp(current, target, sharpness);

                var desiredChange = newTarget - current;

                if (desiredChange > scaledStepRange.y)
                {
                    desiredChange = scaledStepRange.y;
                }
                else if (desiredChange < -scaledStepRange.y)
                {
                    desiredChange = -scaledStepRange.y;
                }
                else if ((desiredChange < scaledStepRange.x) &&
                         (desiredChange > -scaledStepRange.x))
                {
                    current = target;
                    desiredChange = 0;
                }

                current += desiredChange;
            }
        }
    }
}
