#region

using Appalachia.Utility.Constants;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core
{
    public static class PhysicsSimulator
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(PhysicsSimulator) + ".";

        public static OnSimulationEnd onSimulationEnd;
        public static OnSimulationStart onSimulationStart;
        public static OnSimulationUpdate onSimulationUpdate;
        private static bool s_Enabled;

        private static readonly ProfilerMarker _PRF_Update = new(_PRF_PFX + nameof(PhysicsSimulator_Update));

        #endregion

        private const string MENU_TOGGLE = "Toggle Physics Simulation" + SHC.CTRL_ALT_SHFT_S;

        static PhysicsSimulator()
        {
            s_Enabled = false;
        }

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

                EditorApplication.update += PhysicsSimulator_Update;
                s_Enabled = true;
            }
            else if (!enabled && s_Enabled)
            {
                Physics.autoSimulation = true;

                onSimulationEnd?.Invoke();

                EditorApplication.update -= PhysicsSimulator_Update;
                s_Enabled = false;
            }
        }

        [MenuItem(PKG.Menu.Appalachia.Tools.Base + MENU_TOGGLE, false, priority = PKG.Priority)]
        public static void TogglePhysicsSimulation()
        {
            SetEnabled(!s_Enabled);
        }

        [MenuItem(PKG.Menu.Appalachia.Tools.Base + MENU_TOGGLE, true, priority = PKG.Priority)]
        public static bool TogglePhysicsSimulationValidate()
        {
            Menu.SetChecked(MENU_TOGGLE, s_Enabled);
            return true;
        }

        private static void PhysicsSimulator_Update()
        {
            using (_PRF_Update.Auto())
            {
                if (Application.isPlaying)
                {
                    SetEnabled(false);

                    return;
                }

                _elapsed += Time.deltaTime;
                _frames += 1;

                if (_elapsed >= Time.fixedDeltaTime)
                {
                    Physics.autoSimulation = false;
                    onSimulationUpdate?.Invoke(Time.fixedDeltaTime);
                    Physics.Simulate(Time.fixedDeltaTime);

                    _elapsed = 0;
                    _hits += 1;
                }
            }
        }
    }
}
