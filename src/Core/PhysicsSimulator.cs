#region

using Appalachia.CI.Constants;
using Appalachia.Utility.src.Constants;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Core
{
    public static class PhysicsSimulator
    {
        private const string _PRF_PFX = nameof(PhysicsSimulator) + ".";

        private const string MENU_BASE = "Tools/Physics/";

        private const string MENU_TOGGLE =
            MENU_BASE + "Toggle Physics Simulation" + SHC.CTRL_ALT_SHFT_S;

        private static bool s_Enabled;

        public static OnSimulationStart onSimulationStart;

        public static OnSimulationUpdate onSimulationUpdate;

        public static OnSimulationEnd onSimulationEnd;

        private static readonly ProfilerMarker _PRF_Update =
            new(_PRF_PFX + nameof(PhysicsSimulator_Update));

        private static double _elapsed;

        private static int _hits;
        private static int _frames;

        static PhysicsSimulator()
        {
            s_Enabled = false;
        }

        public static bool IsSimulationActive => !Physics.autoSimulation;

        [MenuItem(MENU_TOGGLE, true, priority = APPA_MENU.TOOLS.PHYSICS.PRIORITY)]
        public static bool TogglePhysicsSimulationValidate()
        {
            Menu.SetChecked(MENU_TOGGLE, s_Enabled);
            return true;
        }

        [MenuItem(MENU_TOGGLE, false, priority = APPA_MENU.TOOLS.PHYSICS.PRIORITY)]
        public static void TogglePhysicsSimulation()
        {
            SetEnabled(!s_Enabled);
        }

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
