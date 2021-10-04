using System;
using Appalachia.Spatial.Octree;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy
{
    [Serializable]
    public struct WaterVoxel : IOctreeNodeGizmoDrawer
    {
        [SerializeField] public float3 currentVector;

        [SerializeField] public float distanceToSurface;

        public void DrawGizmo(float3 position, float3 scale)
        {
            Gizmos.DrawLine(position, position + (currentVector * scale));
        }

#region Operators

        public static WaterVoxel operator *(WaterVoxel a, float b)
        {
            a.currentVector *= b;
            a.distanceToSurface *= b;

            return a;
        }

        public static WaterVoxel operator *(float a, WaterVoxel b)
        {
            return b * a;
        }

        public static WaterVoxel operator +(WaterVoxel a, WaterVoxel b)
        {
            a.currentVector += b.currentVector;
            a.distanceToSurface += b.distanceToSurface;

            return a;
        }

        public static WaterVoxel operator -(WaterVoxel a, WaterVoxel b)
        {
            a.currentVector -= b.currentVector;
            a.distanceToSurface -= b.distanceToSurface;

            return a;
        }

        public static WaterVoxel operator /(WaterVoxel a, WaterVoxel b)
        {
            a.currentVector /= b.currentVector;
            a.distanceToSurface /= b.distanceToSurface;

            return a;
        }

#endregion
    }
}
