#if UNITY_EDITOR

#region

using Appalachia.Core.Attributes;
using Appalachia.Utility.Constants;
using Appalachia.Utility.Execution;
using Appalachia.Utility.Timing;
using Unity.Profiling;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core
{
    [CallStaticConstructorInEditor]
    public static class PhysicsSimulator
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(PhysicsSimulator) + ".";

        private static readonly ProfilerMarker _PRF_Update = new(_PRF_PFX + nameof(PhysicsSimulator_Update));

        #endregion

        #region Constants and Static Readonly

        private const string MENU_TOGGLE = "Toggle Physics Simulation" + SHC.CTRL_ALT_SHFT_S;

        #endregion

        static PhysicsSimulator()
        {
            s_Enabled = false;
        }

        public static OnSimulationEnd onSimulationEnd;
        public static OnSimulationStart onSimulationStart;
        public static OnSimulationUpdate onSimulationUpdate;
        private static bool s_Enabled;

        private static double _elapsed;
        private static int _frames;

        private static int _hits;

        public static bool IsSimulationActive => !Physics.autoSimulation;

        public static void SetEnabled(bool enabled)
        {
            if (enabled && !s_Enabled)
            {
                Physics.autoSimulation = false;

                onSimulationStart?.Invoke();

                UnityEditor.EditorApplication.update += PhysicsSimulator_Update;
                s_Enabled = true;
            }
            else if (!enabled && s_Enabled)
            {
                Physics.autoSimulation = true;

                onSimulationEnd?.Invoke();

                UnityEditor.EditorApplication.update -= PhysicsSimulator_Update;
                s_Enabled = false;
            }
        }

        private static void PhysicsSimulator_Update()
        {
            using (_PRF_Update.Auto())
            {
                if (AppalachiaApplication.IsPlayingOrWillPlay)
                {
                    SetEnabled(false);

                    return;
                }

                _elapsed += CoreClock.Instance.DeltaTime;
                _frames += 1;

                if (_elapsed >= CoreClock.Instance.FixedDeltaTime)
                {
                    Physics.autoSimulation = false;
                    onSimulationUpdate?.Invoke(CoreClock.Instance.FixedDeltaTime);
                    Physics.Simulate(CoreClock.Instance.FixedDeltaTime);

                    _elapsed = 0;
                    _hits += 1;
                }
            }
        }

        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + MENU_TOGGLE, priority = PKG.Priority)]
        public static void TogglePhysicsSimulation()
        {
            SetEnabled(!s_Enabled);
        }

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Tools.Base + MENU_TOGGLE, true, priority = PKG.Priority)]
        public static bool TogglePhysicsSimulationValidate()
        {
            UnityEditor.Menu.SetChecked(MENU_TOGGLE, s_Enabled);
            return true;
        }

        #endregion
    }
}
#endif