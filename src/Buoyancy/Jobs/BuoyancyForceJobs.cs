using Appalachia.Core.Collections.Native.Pointers;
using Appalachia.Simulation.Buoyancy.Data;
using Appalachia.Simulation.Physical.Sampling;
using Appalachia.Voxels;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Appalachia.Simulation.Buoyancy.Jobs
{
    public static class BuoyancyForceJobs
    {
        [BurstCompile]
        public struct CalculateSubmersionJob : IJob
        {
            [ReadOnly] public NativeArray<BuoyancyVoxel> buoyancyVoxels;
            [ReadOnly] public NativeBitArray voxelsActive;
            [ReadOnly] public NativeIntPtr voxelsActiveCount;

            [WriteOnly] public NativeFloatPtr submersionPercentage;

            public void Execute()
            {
                float sum = 0f;

                var length = buoyancyVoxels.Length;
                
                for (var index = 0; index < length; index++)
                {
                    if (!voxelsActive.IsSet(index))
                    {
                        continue;
                    }
                    
                    var buoyancyVoxel = buoyancyVoxels[index];

                    var submersion = buoyancyVoxel.submersion.value;
                    
                    sum += submersion;
                }

                submersionPercentage.Value = sum / voxelsActiveCount.Value;
            }
        }

        /*
        
        /// <summary>

        /// </summary>

        /// <summary>
        ///     Force 1 - Viscous Water Resistance (Frictional Drag)
        ///     Resistance forces from http://www.gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php
        /// 
        /// Viscous resistance occurs when water sticks to the boat's surface and the boat has to drag that water with it
        /// F = 0.5 * rho * v^2 * S * Cf
        /// rho - density of the medium you have
        /// v - speed
        /// S - surface area
        /// Cf - Coefficient of frictional resistance
        ///We need the tangential velocity 
        ///Projection of the velocity on the plane with the normal normal vector
        ///http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/
        /// </summary>
        [BurstCompile]
        public struct CalculateViscousWaterResistanceForceJob : IJobParallelFor
        {
            [ReadOnly] public float4x4 localToWorldMatrix;
            [ReadOnly] public float waterDensity;
            [ReadOnly] public float resistanceCoefficient;
            [ReadOnly] public NativeArray<float3> velocitiesWorld;
            [ReadOnly] public NativeArray<float> submersions;
            [ReadOnly] public NativeArray<float> triangleSurfaceAreas;
            [ReadOnly] public NativeArray<float3> localTriangleNormals;

            [WriteOnly] public NativeArray<float3> viscousWaterResistanceForces;

            public void Execute(int index)
            {
                var submersion = submersions[index];

                if (submersion <= 0.0f)
                {
                    viscousWaterResistanceForces[index] = float3.zero;
                    return;
                }

                var submergedArea = submersion * triangleSurfaceAreas[index];

                var velocityWorld = velocitiesWorld[index];
                var localTriangleNormal = localTriangleNormals[index];
                var worldTriangleNormal = math.normalizesafe(localToWorldMatrix.MultiplyVector(localTriangleNormal));

                var normalCross = math.cross(worldTriangleVelocity, worldTriangleNormal);
                var normalLength = math.length(worldTriangleNormal);
                
                var velocityTangent = math.cross(worldTriangleNormal, normalCross / normalLength) / normalLength;

                //The direction of the tangential velocity (-1 to get the flow which is in the opposite direction)
                var tangentialDirection = math.normalize(velocityTangent) * -1f;

                //The speed of the triangle as if it was in the tangent's direction
                //So we end up with the same speed as in the center of the triangle but in the direction of the flow
                var v_f_vec = math.length(worldTriangleVelocity) * tangentialDirection;

                //The final resistance force
                // F = 0.5 * rho * v^2 * S * Cf
                var viscousWaterResistanceForce = 0.5f * waterDensity * (v_f_vec * v_f_vec) * submergedArea * resistanceCoefficient;

                viscousWaterResistanceForces[index] = viscousWaterResistanceForce;
            }
        }


        [BurstCompile]
        public struct CalculatePressureDragForceJob : IJobParallelFor
        {
            [ReadOnly] public float4x4 localToWorldMatrix;  
            [ReadOnly] public WaterPhysicsCoefficentData metadata;
            [ReadOnly] public NativeArray<float> submersions;
            [ReadOnly] public NativeArray<float3> worldTriangleVelocities;
            [ReadOnly] public NativeArray<float> triangleSurfaceAreas;
            [ReadOnly] public NativeArray<float3> localTriangleNormals;
            [ReadOnly] public NativeArray<float> surfaceCosineThetas;

            [WriteOnly] public NativeArray<float3> pressureDragForces;

            public void Execute(int index)
            {
                var submersion = submersions[index];

                if (submersion <= 0.0f)
                {
                    pressureDragForces[index] = float3.zero;
                    return;
                }

                var submergedArea = submersion * triangleSurfaceAreas[index];

                var worldTriangleVelocity = worldTriangleVelocities[index];
                var localTriangleNormal = localTriangleNormals[index];
                var worldTriangleNormal = math.normalizesafe(localToWorldMatrix.MultiplyVector(localTriangleNormal));
                var surfaceCosineTheta = surfaceCosineThetas[index];

                var velocityMagnitude = math.length(worldTriangleVelocity);

                float3 pressureDragForce;

                //Modify for different turning behavior and planing forces
                //f_p and f_S - falloff power, should be smaller than 1
                //C - coefficients to modify 
                
                if (surfaceCosineTheta > 0f)
                {
                    var linearPressureDragCoefficient = metadata.linearPressureDragCoefficient;
                    var quadraticPressureDragCoefficient = metadata.quadraticPressureDragCoefficient;
                    var facingDotProductFalloffPower = metadata.facingDotProductFalloffPower;

                    pressureDragForce =
                        -((linearPressureDragCoefficient * velocityMagnitude) +
                          (quadraticPressureDragCoefficient * (velocityMagnitude * velocityMagnitude))) *
                        submergedArea *
                        math.pow(surfaceCosineTheta, facingDotProductFalloffPower) *
                        worldTriangleNormal;
                }
                else
                {
                    var linearSuctionDragCoefficient = metadata.linearSuctionDragCoefficient;
                    var quadraticSuctionDragCoefficient = metadata.quadraticSuctionDragCoefficient;
                    var suctionFalloffPower = metadata.suctionFalloffPower;

                    pressureDragForce =
                        ((linearSuctionDragCoefficient * velocityMagnitude) +
                         (quadraticSuctionDragCoefficient * (velocityMagnitude * velocityMagnitude))) *
                        submergedArea *
                        math.pow(math.abs(surfaceCosineTheta), suctionFalloffPower) *
                        worldTriangleNormal;
                }
                
                pressureDragForces[index] = pressureDragForce;
            }
        }

*/
        /// <summary>
        /// Buoyancy is a hydrostatic force - it's there even if the water isn't flowing or if the boat stays still
        /// F_buoyancy = rho * g * V
        /// rho - density of the medium you are in
        /// g - localGravity
        /// V - volume of fluid directly above the curved surface 
        ///
        /// V = z * S * n 
        /// z - distance to surface
        /// S - surface area
        /// n - normal to the surface
        /// ---------------------------------------------------
        /// Let W(x,y) give the height of the water surface at (x,y). In this case, W is a sum of sine waves.
        /// Let P(y) give the pressure at distance y relative to the water surface (negative is underwater).
        /// We will define P(y) as:
        /// 
        /// P(y) = 
        ///     P_0 if y GE 0 
        ///     P_0 + p * g * y if y LT 0
        /// 
        ///  where P_0 is standard atmospheric pressure, p is the density of water, and g is gravitational acceleration.
        /// 
        /// Our algorithm is then:
        ///
        /// for each face (pos1, pos2, pos3):
        ///   let center = (pos1 + pos2 + pos3) / 3
        ///   let n = (pos2 - pos1) * (pos3 - pos1)
        ///   let normal = n / ||n||
        ///   Let a = ||n|| / 2
        ///   let f = -normal * a * P(center.y - W(center.x, center.z))
        ///   apply force f at point c
        ///
        /// Air resistance on the part of the object above the water, facing the velocity direction
        ///
        /// Wind resistance on the part of the object above the water, facing the wind.
        /// 
        ///     Calculate the wave drifting force so the boat can float with the waves
        ///  Drifting from waves according to:
        /// http://ocw.mit.edu/courses/mechanical-engineering/2-019-design-of-ocean-systems-spring-2011/lecture-notes/MIT2_019S11_DVL1.pdf
        /// F = rho * g * A^2 * n 
        /// rho - density of the medium
        /// g - localGravity
        /// A - area
        /// n - normal
        /// </summary>
        [BurstCompile]
        public struct CalculateForcesJob : IJobParallelFor, ITrilinearSampler<WaterVoxel>
        {
            [ReadOnly] public NativeArray<Voxel> voxels;
            [ReadOnly] public NativeBitArray voxelsActive;
            [ReadOnly] public WaterPhysicsCoefficentData metadata;
            [ReadOnly] public float airDensity;
            [ReadOnly] public float waterDensity;
            [ReadOnly] public float totalMass;
            [ReadOnly] public float worldWaterHeight;
            [ReadOnly] public float waterLevelOffset;
            [ReadOnly] public float submersionEngageSmoothing;
            [ReadOnly] public float submersionDisengageSmoothing;
            [ReadOnly] public float3 archimedesForce;
            [ReadOnly] public float3 windDynamicPressure;
            [ReadOnly] public TrilinearSamples<WaterVoxel> currentSamples;

            public NativeArray<BuoyancyVoxel> buoyancyVoxels;

            public void Execute(int index)
            {
                if (!voxelsActive.IsSet(index))
                {
                    return;
                }
                
                var voxel = voxels[index];
                //var samplePoint = samplePoints[voxel.indices];
                var buoyancyVoxel = buoyancyVoxels[index];

                buoyancyVoxel = UpdateSubmersionState(voxel, buoyancyVoxel);

                var submersion = buoyancyVoxel.submersion;
                var currentSubmersion = submersion.value;

                buoyancyVoxel = ResetForces(buoyancyVoxel);

                buoyancyVoxel = HydrostaticForce(buoyancyVoxel);

                if (voxel.faceData.isFace)
                {
                    var voxelArea = voxel.faceData.worldArea;
                    var areaInWater = voxelArea * currentSubmersion;
                    var areaInAir = voxelArea - areaInWater;

                    // Wind calculations if not fully submerged
                    if (currentSubmersion < 1.0f)
                    {
                        buoyancyVoxel = AirResistanceForce(voxel, buoyancyVoxel, areaInAir);
                        buoyancyVoxel = WindResistanceForce(voxel, buoyancyVoxel, areaInAir);
                    }

                    // Wave calculations if not fully breached
                    if (currentSubmersion > 0.0f)
                    {
                        buoyancyVoxel = WaveDriftingForce(voxel, buoyancyVoxel, areaInWater);
                    }

                    if (submersion.hasDifference1)
                    {
                        buoyancyVoxel = SlammingForce(voxel, buoyancyVoxel);
                    }
                }

                buoyancyVoxel = CheckForcesAreValid(buoyancyVoxel);
                buoyancyVoxel = ScaleAndAggregateForce(buoyancyVoxel, totalMass);

                buoyancyVoxels[index] = buoyancyVoxel;
            }

            private static BuoyancyVoxel ResetForces(BuoyancyVoxel buoyancyVoxel)
            {
                buoyancyVoxel.force = float3.zero;
                buoyancyVoxel.hydrostaticForce = float3.zero;
                buoyancyVoxel.viscousWaterResistanceForce = float3.zero;
                buoyancyVoxel.pressureDragForce = float3.zero;
                buoyancyVoxel.airResistanceForce = float3.zero;
                buoyancyVoxel.windResistanceForce = float3.zero;
                buoyancyVoxel.waveDriftingForce = float3.zero;
                buoyancyVoxel.slammingForce = float3.zero;

                return buoyancyVoxel;
            }

            public WaterVoxel Sample(TrilinearSamples<WaterVoxel> samples)
            {
                var sx = samples.sampleStrengths.x;
                var nx = 1.0f - sx;

                var c00 = (nx * samples.c000) + (sx * samples.c100);
                var c01 = (nx * samples.c001) + (sx * samples.c101);
                var c10 = (nx * samples.c010) + (sx * samples.c110);
                var c11 = (nx * samples.c011) + (sx * samples.c111);
                
                var sy = samples.sampleStrengths.y;
                var ny = 1.0f - sy;

                var c0 = (ny * c00) + (sy * c10);
                var c1 = (ny * c01) + (sy * c11);                
                
                var sz = samples.sampleStrengths.z;
                var nz = 1.0f - sz;

                var c = (nz * c0) + (sz * c1);
                
                return c;
            }

            private BuoyancyVoxel UpdateSubmersionState(
                Voxel voxel,
                BuoyancyVoxel buoyancyVoxel
                )
            {
                var voxelWorldPostiion = voxel.worldPosition.value;
                var waterWorldPosition = voxelWorldPostiion;
                
                waterWorldPosition.y = worldWaterHeight;

                var offsetWaterWorldPosition = waterWorldPosition;
                offsetWaterWorldPosition.y += waterLevelOffset;

                buoyancyVoxel.water = waterWorldPosition;
                buoyancyVoxel.offsetWater = offsetWaterWorldPosition;
                
                
                buoyancyVoxel.distanceToSurface = voxelWorldPostiion.y -  waterWorldPosition.y;
                buoyancyVoxel.offsetDistanceToSurface = voxelWorldPostiion.y -  offsetWaterWorldPosition.y;

                var targetSubmersion = 1.0f;
                var targetOffsetSubmersion = 1.0f;
                
                var smoothing = submersionEngageSmoothing;
                var offsetSmoothing = submersionEngageSmoothing;
                
                if (buoyancyVoxel.distanceToSurface >= 0)
                {
                    targetSubmersion = 0f;
                    smoothing = submersionDisengageSmoothing;
                }
                
                if (buoyancyVoxel.offsetDistanceToSurface >= 0)
                {
                    targetOffsetSubmersion = 0f;
                    offsetSmoothing = submersionDisengageSmoothing;
                }

                var submersion = buoyancyVoxel.submersion;
                var offsetSubmersion = buoyancyVoxel.offsetSubmersion;
                
                var currentSubmersion = submersion.value;
                var currentOffsetSubmersion = offsetSubmersion.value;
                
                var newSubmersion = math.lerp(currentSubmersion, targetSubmersion, smoothing);
                var newOffsetSubmersion = math.lerp(currentOffsetSubmersion, targetOffsetSubmersion, offsetSmoothing);
                
                submersion.Update(newSubmersion);
                offsetSubmersion.Update(newOffsetSubmersion);
                
                buoyancyVoxel.submersion = submersion;
                buoyancyVoxel.offsetSubmersion = offsetSubmersion;

                return buoyancyVoxel;
            }

            private BuoyancyVoxel HydrostaticForce(
                BuoyancyVoxel buoyancyVoxel)
            {
                var offsetSubmersion = buoyancyVoxel.offsetSubmersion.value;
                
                buoyancyVoxel.hydrostaticForce = offsetSubmersion * archimedesForce;
                buoyancyVoxel.hydrostaticForce.x = 0;
                buoyancyVoxel.hydrostaticForce.z = 0;

                return buoyancyVoxel;
            }

            private BuoyancyVoxel AirResistanceForce(
                Voxel voxel,
                BuoyancyVoxel buoyancyVoxel,
                float areaInAir)
            {
                // R_air = 0.5 * rho * v^2 * A_p * C_air
                // rho - air density
                // v - speed of ship
                // A_p - projected transverse profile area of ship
                // C_r - coefficient of air resistance (drag coefficient)

                var airSurfaceFactor = math.clamp(voxel.normalVelocityCodirectionality, 0, 1);
                var airResistanceArea = areaInAir * airSurfaceFactor;
                var voxelVelocitySq = voxel.worldVelocity.value * voxel.worldVelocity.value;
                var airResistanceForce = -0.5f * airDensity * voxelVelocitySq * airResistanceArea * metadata.CoefficientAirResistance;

                buoyancyVoxel.airResistanceForce = airResistanceForce;

                return buoyancyVoxel;
            }

            private BuoyancyVoxel WindResistanceForce(
                Voxel voxel,
                BuoyancyVoxel buoyancyVoxel,
                float areaInAir
                )
            {
                var windSurfaceFactor = math.clamp(math.dot(voxel.faceData.worldNormal, windDynamicPressure), -1, 0);
                var windResistanceArea = math.abs(areaInAir * windSurfaceFactor);
                var windResistanceForce = windDynamicPressure * windResistanceArea * metadata.CoefficientWindResistance;

                buoyancyVoxel.windResistanceForce = -windResistanceForce;

                return buoyancyVoxel;
            }
            

            private BuoyancyVoxel WaveDriftingForce(
                Voxel voxel,
                BuoyancyVoxel buoyancyVoxel,
                float areaInWater
            )
            {
                var currentSample = Sample(currentSamples);
                var waveSurfaceFactor = math.clamp(math.dot(voxel.faceData.worldNormal, currentSample.currentVector), -1f, 0f);
                var waveResistanceArea = math.abs(waveSurfaceFactor) * areaInWater;

                var waveDriftingForce = .5f * waterDensity * waveResistanceArea * currentSample.currentVector;

                buoyancyVoxel.waveDriftingForce = waveDriftingForce;
                buoyancyVoxel.waveDriftingForce.y = 0;

                return buoyancyVoxel;
            }

            /// <summary>
            /// Capture the response of the fluid to sudden accelerations or penetrations
            /// https://www.orcina.com/webhelp/OrcaFlex/Content/html/Slammingtheory.htm
            /// </summary>
            private BuoyancyVoxel SlammingForce(
                Voxel voxel,
                BuoyancyVoxel buoyancyVoxel)
            {
                if (!buoyancyVoxel.submersion.hasDifference1)
                {
                    buoyancyVoxel.slammingForce = float3.zero;
                    return buoyancyVoxel;
                }

                if (voxel.normalVelocityCodirectionality <= 0f)
                {
                    buoyancyVoxel.slammingForce = float3.zero;
                    return buoyancyVoxel;
                }

                /* The slam (fs) or water exit (fe) force on a cylinder is given by 
                   fs = +.5ρ Cs Aw vn^2 n
                   fe = −.5ρ Ce Aw vn^2 n
                   
                   where ρ = water density
               */

                // Cs and Ce are slam coefficients for entry (subscript s for slam) and exit (subscript e) respectively. 
                var slamCoefficient = buoyancyVoxel.submersion.delta > 0 ? metadata.slamEntryCoefficient : -metadata.slamExitCoefficient;

                var submersionDelta = math.abs(buoyancyVoxel.submersion.delta);
                
                // n is a unit vector in the water surface outward normal direction. This (together with the minus sign in the
                // exit slam force formula) ensures that the slam force opposes the penetration of the water surface in both directions.
                var slamSurfaceFactor = math.clamp(voxel.normalVelocityCodirectionality, 0, 1);
                
                // Aw = The instantaneous slam water-plane area, the area of the intersection of the water surface and voxel.
                var slamWaterplaneArea = voxel.worldSurfaceArea * submersionDelta;
                
                // vn is the component in the surface normal direction of the cylinder velocity relative to the fluid velocity.        
                var voxelVelocity = voxel.worldVelocity.value;
                var voxelVelocitySq = voxelVelocity * voxelVelocity;

                var slammingForce = .5f * slamCoefficient * slamWaterplaneArea * voxelVelocitySq * slamSurfaceFactor;

                var slamForceHorizontality = buoyancyVoxel.submersion.delta > 0 ? metadata.slamEntryHorizontality : metadata.slamExitHorizontality;
                    
                slammingForce.x *= slamForceHorizontality;
                slammingForce.z *= slamForceHorizontality;
                buoyancyVoxel.slammingForce = slammingForce;
                return buoyancyVoxel;
            }
            
            private BuoyancyVoxel CheckForcesAreValid(BuoyancyVoxel buoyancyVoxel)
            {
                buoyancyVoxel.hydrostaticForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.hydrostaticForce,
                    BuoyancyJobHelper.ForceType.Buoyancy
                );
                buoyancyVoxel.viscousWaterResistanceForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.viscousWaterResistanceForce,
                    BuoyancyJobHelper.ForceType.ViscousWaterResistance
                );
                buoyancyVoxel.pressureDragForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.pressureDragForce,
                    BuoyancyJobHelper.ForceType.PressureDrag
                );
                buoyancyVoxel.airResistanceForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.airResistanceForce,
                    BuoyancyJobHelper.ForceType.AirResistance
                );
                buoyancyVoxel.windResistanceForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.windResistanceForce,
                    BuoyancyJobHelper.ForceType.WindResistance
                );
                buoyancyVoxel.waveDriftingForce = BuoyancyJobHelper.CheckForceIsValid(
                    buoyancyVoxel.waveDriftingForce,
                    BuoyancyJobHelper.ForceType.WaveDrifting
                );
                buoyancyVoxel.slammingForce = BuoyancyJobHelper.CheckForceIsValid(buoyancyVoxel.slammingForce, BuoyancyJobHelper.ForceType.Slamming);

                return buoyancyVoxel;
            }

            private BuoyancyVoxel ScaleAndAggregateForce(BuoyancyVoxel buoyancyVoxel, float mass)
            {
                buoyancyVoxel.hydrostaticForce *= metadata.hydrostaticScale;
                buoyancyVoxel.viscousWaterResistanceForce *= metadata.viscousWaterResistanceScale;
                buoyancyVoxel.pressureDragForce *= metadata.pressureDragScale;
                buoyancyVoxel.airResistanceForce *= metadata.airResistanceScale;
                buoyancyVoxel.windResistanceForce *= metadata.windResistanceScale;
                buoyancyVoxel.waveDriftingForce *= metadata.waveDriftingScale;
                buoyancyVoxel.slammingForce *= metadata.slammingScale;
                
                buoyancyVoxel.hydrostaticForce = math.clamp(buoyancyVoxel.hydrostaticForce, -metadata.hydrostaticLimit*mass, metadata.hydrostaticLimit*mass); 
                buoyancyVoxel.viscousWaterResistanceForce = math.clamp(buoyancyVoxel.viscousWaterResistanceForce, -metadata.viscousWaterResistanceLimit*mass, metadata.viscousWaterResistanceLimit*mass); 
                buoyancyVoxel.pressureDragForce = math.clamp(buoyancyVoxel.pressureDragForce, -metadata.pressureDragLimit*mass, metadata.pressureDragLimit*mass); 
                buoyancyVoxel.airResistanceForce = math.clamp(buoyancyVoxel.airResistanceForce, -metadata.airResistanceLimit*mass, metadata.airResistanceLimit*mass); 
                buoyancyVoxel.windResistanceForce = math.clamp(buoyancyVoxel.windResistanceForce, -metadata.windResistanceLimit*mass, metadata.windResistanceLimit*mass);
                buoyancyVoxel.waveDriftingForce = math.clamp(buoyancyVoxel.waveDriftingForce, -metadata.waveDriftingLimit*mass, metadata.waveDriftingLimit*mass); 
                buoyancyVoxel.slammingForce = math.clamp(buoyancyVoxel.slammingForce, -metadata.slammingLimit*mass, metadata.slammingLimit*mass); 

                var force = buoyancyVoxel.hydrostaticForce +
                            buoyancyVoxel.viscousWaterResistanceForce +
                            buoyancyVoxel.pressureDragForce +
                            buoyancyVoxel.airResistanceForce +
                            buoyancyVoxel.windResistanceForce +
                            buoyancyVoxel.waveDriftingForce +
                            buoyancyVoxel.slammingForce;

                force *= metadata.scale;

                force = math.clamp(force, -metadata.limit * mass, metadata.limit * mass);

                buoyancyVoxel.force = force;
                
                return buoyancyVoxel;
            }
        }

        [BurstCompile]
        public struct ResetForceAndTorqueJob : IJob
        {
            public NativeFloat3Ptr force;
            public NativeFloat3Ptr torque;
            public NativeFloat3Ptr hydrostaticForce;
            public NativeFloat3Ptr viscousWaterResistanceForce;
            public NativeFloat3Ptr pressureDragForce;
            public NativeFloat3Ptr airResistanceForce;
            public NativeFloat3Ptr windResistanceForce;
            public NativeFloat3Ptr waveDriftingForce;
            public NativeFloat3Ptr slammingForce;
            
            public void Execute()
            {
                force.Value = 0;
                torque.Value = 0;
                hydrostaticForce.Value = 0;
                viscousWaterResistanceForce.Value = 0;
                pressureDragForce.Value = 0;
                airResistanceForce.Value = 0;
                windResistanceForce.Value = 0;
                waveDriftingForce.Value = 0;
                slammingForce.Value = 0;
            }
        }

        [BurstCompile]
        public struct AggregateForceAndTorqueJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Voxel> voxels;
            [ReadOnly] public NativeBitArray voxelsActive;
            [ReadOnly] public NativeArray<BuoyancyVoxel> buoyancyVoxels;
            [ReadOnly] public float activeRatio;
            [ReadOnly] public float3 worldCenterOfMass;

            public NativeFloat3Ptr.Parallel force;
            public NativeFloat3Ptr.Parallel torque;
            public NativeFloat3Ptr.Parallel hydrostaticForce;
            public NativeFloat3Ptr.Parallel viscousWaterResistanceForce;
            public NativeFloat3Ptr.Parallel pressureDragForce;
            public NativeFloat3Ptr.Parallel airResistanceForce;
            public NativeFloat3Ptr.Parallel windResistanceForce;
            public NativeFloat3Ptr.Parallel waveDriftingForce;
            public NativeFloat3Ptr.Parallel slammingForce;

            public void Execute(int index)
            {
                if (!voxelsActive.IsSet(index))
                {
                    return;
                }

                var voxel = voxels[index];
                var buoyancyVoxel = buoyancyVoxels[index];

                hydrostaticForce.Add(buoyancyVoxel.hydrostaticForce / activeRatio);
                viscousWaterResistanceForce.Add(buoyancyVoxel.viscousWaterResistanceForce / activeRatio);
                pressureDragForce.Add(buoyancyVoxel.pressureDragForce / activeRatio);
                airResistanceForce.Add(buoyancyVoxel.airResistanceForce / activeRatio);
                windResistanceForce.Add(buoyancyVoxel.windResistanceForce / activeRatio);
                waveDriftingForce.Add(buoyancyVoxel.waveDriftingForce / activeRatio);
                slammingForce.Add(buoyancyVoxel.slammingForce / activeRatio);
                
                var voxelForce = buoyancyVoxel.force;

                force.Add(voxelForce / activeRatio);

                var voxelTorque = GetTorqueFromForce(worldCenterOfMass, voxelForce, voxel.worldPosition.value);
                torque.Add(voxelTorque);
            }
            
            private static float3 GetTorqueFromForce(float3 centerOfMass, float3 force, float3 position)
            {
                var x = position - centerOfMass;
                var torque = math.cross(x, force);

                return torque;
            }
        }
    }
}