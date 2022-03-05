#region

using Appalachia.Audio;
using Appalachia.Audio.Analysis;
using Appalachia.Core.Attributes;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Core.Shading;
using Appalachia.Globals.Shading;
using Appalachia.Simulation.Core.Metadata.Wind;
using Appalachia.Utility.Async;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Timing;
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
    [CallStaticConstructorInEditor]
    public class GlobalWindManager : SingletonAppalachiaBehaviour<GlobalWindManager>
    {
        static GlobalWindManager()
        {
            RegisterDependency<GlobalWindParametersGroup>(i => _globalWindParametersGroup = i);
            RegisterDependency<GSR>(i => _GSR = i);
        }

        #region Static Fields and Autoproperties

        private static GlobalWindParametersGroup _globalWindParametersGroup;

        private static GSR _GSR;

        #endregion

        #region Fields and Autoproperties

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

        [FoldoutGroup("Components"), SerializeField]
        private GameObject _windArrow;

        [FoldoutGroup("Components"), SerializeField]
        private MeshFilter _meshFilter;

        [FoldoutGroup("Components"), SerializeField]
        private MeshRenderer _meshRenderer;

        #endregion

        /*[TitleGroup("System Center")]
        public GameObject reference;*/

        [TitleGroup("Wind")]
        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        public float3 WindDirection => Transform.forward;

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
            .5f *
            _globalWindParametersGroup.Current.airDensity.densityKGPerCubicMeter *
            WindVelocity *
            WindVelocity;

        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        [SuffixLabel("m/s")]
        public float3 WindSpeed =>
            gustAudioStrengthActual * _globalWindParametersGroup.Current.maximumWindSpeed;

        [FoldoutGroup("Wind/Physics")]
        [ShowInInspector]
        [SmartLabel]
        [SuffixLabel("m/s")]
        public float3 WindVelocity => WindDirection * WindSpeed;

        #region Event Functions

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                if (ShouldSkipUpdate)
                {
                    return;
                }

#if UNITY_EDITOR
                if (!AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    ExecuteHeadingStepUpdate(false);
                }
#endif

                var param = _globalWindParametersGroup.Current;

#if !UNITY_EDITOR
            if (param.realtimeUpdate)
            {
#endif

                if (_meshRenderer.sharedMaterial != param.arrowMaterial)
                {
                    _meshFilter.sharedMesh = param.arrowMesh;
                    _meshRenderer.sharedMaterial = param.arrowMaterial;
                }

                _meshRenderer.enabled = param.showArrow;

                ToggleHideShowArrowComponents(param.showArrowComponents);

                if (_windArrow == null)
                {
                    return;
                }

                var arrowT = _windArrow.transform;
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
                if (!AppalachiaApplication.IsPlayingOrWillPlay || param.realtimeUpdate)
                {
#endif
                    SetGlobalShaderProperties(CoreClock.Instance.DeltaTime, param);

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
                if (ShouldSkipUpdate)
                {
                    return;
                }
                
                ExecuteHeadingStepUpdate(false);
            }
        }

        #endregion

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            gameObject.name = "Global Wind Manager";

            _GSR.InitializeShaderReferences();

            _windArrow = gameObject.GetChild(AppalachiaRepository.PrefabAddresses.WIND_ARROW);

            if (_windArrow == null)
            {
                _windArrow = await AppalachiaRepository.InstantiatePrefab(
                    AppalachiaRepository.PrefabAddresses.WIND_ARROW,
                    gameObject,
                    true
                );
            }

            _meshFilter = initializer.Get(
                _windArrow,
                _meshFilter,
                nameof(_windArrow) + nameof(_meshFilter),
                _meshFilter == null
            );

            _meshRenderer = initializer.Get(
                _windArrow,
                _meshRenderer,
                nameof(_windArrow) + nameof(_meshRenderer),
                _meshRenderer == null
            );

            gustAudioGroup = _globalWindParametersGroup.gustAudioGroup;
        }

        private static float GetCurrentAudioStrength(AudioSignalSmoothAnalyzer analyzer, bool clamped = true)
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
                else if ((desiredChange < scaledStepRange.x) && (desiredChange > -scaledStepRange.x))
                {
                    current = target;
                    desiredChange = 0;
                }

                current += desiredChange;
            }
        }

        private void ExecuteHeadingStepUpdate(bool forceUpdate)
        {
            var time = CoreClock.Instance.Time;
            if ((time > nextHeadingUpdateTime) || forceUpdate)
            {
                var update = Random.Range(
                    _globalWindParametersGroup.Current.headingUpdateRange.x,
                    _globalWindParametersGroup.Current.headingUpdateRange.y
                );

                var rotation = Quaternion.AngleAxis(update, Vector3.up);

                targetHeading = Transform.rotation * rotation;
                nextHeadingUpdateTime = time +
                                        Random.Range(
                                            _globalWindParametersGroup.Current.headingChangeInterval.x,
                                            _globalWindParametersGroup.Current.headingChangeInterval.y
                                        );
            }
            else
            {
                Transform.rotation = Quaternion.Slerp(
                    Transform.rotation,
                    targetHeading,
                    _globalWindParametersGroup.Current.headingUpdateSpeed
                );
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
                    veryHigh.referenceID = (int)GAC.Lookup(
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
                    high.referenceID = (int)GAC.Lookup(mixer, GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_HIGH);
                }

                if (mid == null)
                {
                    mid = new AudioSignalSmoothAnalyzer();
                }

                if (mid.referenceID <= 0)
                {
                    mid.referenceID = (int)GAC.Lookup(mixer, GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_MID);
                }

                if (low == null)
                {
                    low = new AudioSignalSmoothAnalyzer();
                }

                if (low.referenceID <= 0)
                {
                    low.referenceID = (int)GAC.Lookup(mixer, GAC.WIND._GUST_CHANNEL_MONITOR_INSTANCE_LOW);
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
                    gustAudioStrengthActualVeryHigh = param.realtimeUpdate ? veryHigh.avg : veryHigh.last;
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
                    Shader.SetGlobalVector(GSPL.Get(GSC.WIND._WIND_DIRECTION), Transform.forward);
                    Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_BASE_AMPLITUDE),     param.baseAmplitude);
                    Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_BASE_TO_GUST_RATIO), param.baseToGustRatio);

                    Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_GUST_AMPLITUDE), gustAmplitudeActual);
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
                            Shader.SetGlobalTexture(GSPL.Get(GSC.WIND._WIND_GUST_TEXTURE), param.gustTexture);
                        }

                        Shader.SetGlobalFloat(GSPL.Get(GSC.WIND._WIND_GUST_CONTRAST), param.gustContrast);
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

        private void ToggleHideShowArrowComponents(bool show)
        {
            _windArrow.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
            _meshFilter.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
            _meshRenderer.hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
        }

        [Button]
        [FoldoutGroup("Wind/Runtime/Heading")]
        private void UpdateHeadingNow()
        {
            ExecuteHeadingStepUpdate(true);
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_GetCurrentAudioStrength =
            new(_PRF_PFX + nameof(GetCurrentAudioStrength));

        private static readonly ProfilerMarker _PRF_InitializeAudioPlugins =
            new(_PRF_PFX + nameof(InitializeAudioPlugins));

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties));

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_AddLatest =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".AddLatest");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_ApplyProperties =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".ApplyProperties");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_GetCurrent =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".GetCurrent");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetDefaults =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetDefaults");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetLatest =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetLatest");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetLatest_NonRealtimeUpdate =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetLatest.NonRealtimeUpdate");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetLatest_RealtimeUpdate =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetLatest.RealtimeUpdate");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetShaderProps =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetShaderProps");

        private static readonly ProfilerMarker _PRF_SetGlobalShaderProperties_SetShaderTexture =
            new(_PRF_PFX + nameof(SetGlobalShaderProperties) + ".SetShaderTexture");

        private static readonly ProfilerMarker _PRF_LerpValues = new(_PRF_PFX + nameof(LerpValues));

        #endregion
    }
}
