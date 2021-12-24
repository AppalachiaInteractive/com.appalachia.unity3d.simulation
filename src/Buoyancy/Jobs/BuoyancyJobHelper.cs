using System;
using Appalachia.CI.Constants;
using Appalachia.Utility.Strings;
using Unity.Burst;
using Unity.Mathematics;

namespace Appalachia.Simulation.Buoyancy.Jobs
{
    public static class BuoyancyJobHelper
    {
        [NonSerialized] private static AppaContext _context;

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(BuoyancyJobHelper));
                }

                return _context;
            }
        }
        
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
            Context.Log.Warn(ZString.Format("{0} force is NaN!", forceType));
        }
    }
}
