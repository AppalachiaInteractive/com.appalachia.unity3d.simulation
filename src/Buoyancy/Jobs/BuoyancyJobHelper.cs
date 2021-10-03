using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Jobs
{
    public static class BuoyancyJobHelper
    {
        public enum ForceType
        {
            Buoyancy = 10,
            ViscousWaterResistance = 20,
            PressureDrag = 30,
            Slamming = 40,
            AirResistance = 50,
            WaveDrifting = 55,
            WindResistance = 60
        }

        [BurstCompile]
        public static float3 CheckForceIsValid(float3 force, ForceType forceType)
        {
            if (!float.IsNaN(force.x + force.y + force.z))
            {
                return force;
            }

            LogCheckForce(forceType);

            return float3.zero;
        }

        [BurstDiscard]
        public static void LogCheckForce(ForceType forceType)
        {
            Debug.LogWarning($"{forceType} force is NaN!");
        }


    }
}