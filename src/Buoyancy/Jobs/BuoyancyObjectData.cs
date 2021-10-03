using System;
using Appalachia.Core.Collections.Native;
using Appalachia.Core.Collections.Native.Pointers;
using Appalachia.Voxels.VoxelTypes;
using Unity.Collections;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Jobs
{
    [Serializable]
    public struct BuoyancyObjectData : IDisposable, IVoxelsInit
    {
        [SerializeField] public NativeFloatPtr submersionPercentage;
        [SerializeField] public float previousSubmersionPercentage;
        
        [SerializeField] public NativeFloat3Ptr force;
        [SerializeField] public NativeFloat3Ptr torque;
        [SerializeField] public NativeFloat3Ptr hydrostaticForce;
        [SerializeField] public NativeFloat3Ptr viscousWaterResistanceForce;
        [SerializeField] public NativeFloat3Ptr pressureDragForce;
        [SerializeField] public NativeFloat3Ptr airResistanceForce;
        [SerializeField] public NativeFloat3Ptr windResistanceForce;
        [SerializeField] public NativeFloat3Ptr waveDriftingForce;
        [SerializeField] public NativeFloat3Ptr slammingForce;

        public void Dispose()
        {
            submersionPercentage.SafeDispose();
            force.SafeDispose();
            torque.SafeDispose();
            hydrostaticForce.SafeDispose();
            viscousWaterResistanceForce.SafeDispose();
            pressureDragForce.SafeDispose();
            airResistanceForce.SafeDispose();
            windResistanceForce.SafeDispose();
            waveDriftingForce.SafeDispose();
            slammingForce.SafeDispose();
        }

        public void Initialize()
        {
            submersionPercentage = new NativeFloatPtr(Allocator.Persistent);
            force = new NativeFloat3Ptr(Allocator.Persistent);
            torque = new NativeFloat3Ptr(Allocator.Persistent);
            hydrostaticForce = new NativeFloat3Ptr(Allocator.Persistent);
            viscousWaterResistanceForce = new NativeFloat3Ptr(Allocator.Persistent);
            pressureDragForce = new NativeFloat3Ptr(Allocator.Persistent);
            airResistanceForce = new NativeFloat3Ptr(Allocator.Persistent);
            windResistanceForce = new NativeFloat3Ptr(Allocator.Persistent);
            waveDriftingForce = new NativeFloat3Ptr(Allocator.Persistent);
            slammingForce = new NativeFloat3Ptr(Allocator.Persistent);
        }
    }
}
