using Appalachia.Simulation.Core;
using Obi;
using UnityEngine;

namespace Appalachia.Simulation.Obi
{
    /// <summary>
    ///     Updater class that will perform simulation during FixedUpdate(). This is the most physically correct updater,
    ///     and the one to be used in most cases. Also allows to perform substepping, greatly improving convergence.
    [AddComponentMenu("Physics/Obi/Obi Simulation Fixed Updater", 801)]
    [ExecuteAlways]
    public class ObiSimulationFixedUpdater : ObiUpdater
    {
        private float accumulatedTime;

        private void Awake()
        {
            accumulatedTime = 0;
#if UNITY_EDITOR
            PhysicsSimulator.onSimulationUpdate -= FixedUpdate;
            PhysicsSimulator.onSimulationUpdate -= FixedUpdate;
#endif
        }

        private void Update()
        {
            ObiProfiler.EnableProfiler();
            Interpolate(Time.fixedDeltaTime, accumulatedTime);
            ObiProfiler.DisableProfiler();

            accumulatedTime += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            FixedUpdate(Time.fixedDeltaTime);
        }

        private void FixedUpdate(float deltaTime)
        {
            ObiProfiler.EnableProfiler();

            BeginStep(deltaTime);
            Substep(deltaTime, deltaTime, 0);
            EndStep(deltaTime);

            ObiProfiler.DisableProfiler();

            accumulatedTime -= deltaTime;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            PhysicsSimulator.onSimulationUpdate -= FixedUpdate;
            PhysicsSimulator.onSimulationUpdate += FixedUpdate;
        }

        private void OnDisable()
        {
            PhysicsSimulator.onSimulationUpdate -= FixedUpdate;
        }
#endif
    }
}
