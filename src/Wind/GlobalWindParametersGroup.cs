using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Core.Metadata.Wind;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace Appalachia.Simulation.Wind
{
    public sealed class GlobalWindParametersGroup : SingletonAppalachiaObject<GlobalWindParametersGroup>
    {
        #region Fields and Autoproperties

        public bool debug;

        public AudioMixerGroup gustAudioGroup;

        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [HideIf(nameof(debug))]
        public GlobalWindParameters parameters;

        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowIf(nameof(debug))]
        public GlobalWindParameters debugParameters;

        #endregion

        public GlobalWindParameters Current
        {
            get
            {
                if (debug)
                {
                    return debugParameters;
                }

                return parameters;
            }
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

#if UNITY_EDITOR
            initializer.Do(
                this,
                nameof(debugParameters),
                debugParameters == null,
                () =>
                {
                    using (_PRF_Initialize.Auto())
                    {
                        debugParameters = GlobalWindParameters.LoadOrCreateNew("GLOBAL-WIND-PARAMS_DEBUG");
                    }
                }
            );

            initializer.Do(
                this,
                nameof(parameters),
                parameters == null,
                () =>
                {
                    using (_PRF_Initialize.Auto())
                    {
                        parameters = GlobalWindParameters.LoadOrCreateNew("GLOBAL-WIND-PARAMS");
                    }
                }
            );
#endif
        }

        #region Profiling

        #endregion
    }
}
