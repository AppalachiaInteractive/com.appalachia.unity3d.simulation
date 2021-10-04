using System;
using Appalachia.Jobs.Types.Temporal;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Jobs
{
    [Serializable]
    public struct BuoyancyVoxel
    {
        [SerializeField] public float_temporal submersion;
        [SerializeField] public float_temporal offsetSubmersion;
        [SerializeField] public float3 water;
        [SerializeField] public float3 offsetWater;
        [SerializeField] public float distanceToSurface;
        [SerializeField] public float offsetDistanceToSurface;
        [SerializeField] public float3 force;
        [SerializeField] public float3 hydrostaticForce;
        [SerializeField] public float3 viscousWaterResistanceForce;
        [SerializeField] public float3 pressureDragForce;
        [SerializeField] public float3 airResistanceForce;
        [SerializeField] public float3 windResistanceForce;
        [SerializeField] public float3 waveDriftingForce;
        [SerializeField] public float3 slammingForce;
    }
}
