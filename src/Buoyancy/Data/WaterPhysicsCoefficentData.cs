using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Simulation.Buoyancy.Data
{
    [Serializable]
    [BurstCompile]
    public struct WaterPhysicsCoefficentData : IEquatable<WaterPhysicsCoefficentData>
    {
        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        public float maximumMaximum;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumDrag;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumAngularDrag;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumScale;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumLimit;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float minimumCoefficient;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumCoefficient;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float minimumSlamCoefficient;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumSlamCoefficient;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float minimumDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Limits")]
        [SmartLabel]
        [DelayedProperty]
        [OnValueChanged(nameof(ConfirmRanges))]
        [PropertyRange(0.01f, nameof(maximumMaximum))]
        public float maximumDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Rigidbody")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumDrag))]
        public float additionalDrag;

        [SerializeField]
        [FoldoutGroup("Rigidbody")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumAngularDrag))]
        public float additionalAngularDrag;

        [SerializeField]
        [FoldoutGroup("Rigidbody")]
        [SmartLabel]
        public bool disableGravity;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float scale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float hydrostaticScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float viscousWaterResistanceScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float pressureDragScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float airResistanceScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float windResistanceScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float waveDriftingScale;

        [SerializeField]
        [FoldoutGroup("Scaling")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumScale))]
        public float slammingScale;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float limit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float hydrostaticLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float viscousWaterResistanceLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float pressureDragLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float airResistanceLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float windResistanceLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float waveDriftingLimit;

        [SerializeField]
        [FoldoutGroup("Limiting")]
        [SmartLabel]
        [PropertyRange(0.0f, nameof(maximumLimit))]
        public float slammingLimit;

        /// <summary>
        ///     viscosity of the fluid [m^2 / s].  Viscosity depends on the temperature, but at 20 degrees celsius: 0.000001f
        ///     At 30 degrees celsius: nu = 0.0000008f; so no big difference
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Coefficients")]
        [SmartLabel]
        [ReadOnly]
        [PropertyRange(0.0000005f, 0.0000015f)]
        public float Viscosity;

        /// <summary>
        ///     C_w - coefficient of wind resistance (drag coefficient)
        ///     Between 0.6 and 1.1 for all boats, so have to estimate
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Coefficients")]
        [SmartLabel]
        [PropertyRange(nameof(minimumCoefficient), nameof(maximumCoefficient))]
        public float CoefficientWindResistance;

        /// <summary>
        ///     C_r - coefficient of air resistance (drag coefficient)
        ///     Between 0.6 and 1.1 for all boats, so have to estimate
        /// </summary>
        [SerializeField]
        [FoldoutGroup("Coefficients")]
        [SmartLabel]
        [PropertyRange(nameof(minimumCoefficient), nameof(maximumCoefficient))]
        public float CoefficientAirResistance;

        [SerializeField]
        [FoldoutGroup("Coefficients")]
        [SmartLabel]
        [PropertyRange(nameof(minimumCoefficient), nameof(maximumCoefficient))]
        public float CoefficientDragFlatPlatePerpendicularToFlow;

        [SerializeField]
        [FoldoutGroup("Slamming")]
        [SmartLabel]
        [PropertyRange(nameof(minimumSlamCoefficient), nameof(maximumSlamCoefficient))]
        public float slamEntryCoefficient;

        [SerializeField]
        [FoldoutGroup("Slamming")]
        [SmartLabel]
        [PropertyRange(nameof(minimumSlamCoefficient), nameof(maximumSlamCoefficient))]
        public float slamExitCoefficient;

        [SerializeField]
        [FoldoutGroup("Slamming")]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float slamEntryHorizontality;

        [SerializeField]
        [FoldoutGroup("Slamming")]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float slamExitHorizontality;

        [SerializeField]
        [FoldoutGroup("Pressure Drag")]
        [SmartLabel]
        [PropertyRange(nameof(minimumDragCoefficient), nameof(maximumDragCoefficient))]
        public float linearPressureDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Pressure Drag")]
        [SmartLabel]
        [PropertyRange(nameof(minimumDragCoefficient), nameof(maximumDragCoefficient))]
        public float quadraticPressureDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Pressure Drag")]
        [SmartLabel]
        [PropertyRange(0.0f, 1.0f)]
        public float facingDotProductFalloffPower;

        [SerializeField]
        [FoldoutGroup("Suction Drag")]
        [SmartLabel]
        [PropertyRange(nameof(minimumDragCoefficient), nameof(maximumDragCoefficient))]
        public float linearSuctionDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Suction Drag")]
        [SmartLabel]
        [PropertyRange(nameof(minimumDragCoefficient), nameof(maximumDragCoefficient))]
        public float quadraticSuctionDragCoefficient;

        [SerializeField]
        [FoldoutGroup("Suction Drag")]
        [SmartLabel]
        [PropertyRange(0.0f, 1.0f)]
        public float suctionFalloffPower;

        public WaterPhysicsCoefficentData(float scale = 1.0f)
        {
            maximumScale = 5.0f;
            additionalDrag = 10.0f;
            additionalAngularDrag = 0.5f;
            disableGravity = true;
            this.scale = scale;
            hydrostaticScale = 1.0f;
            viscousWaterResistanceScale = 1.0f;
            pressureDragScale = 1.0f;
            airResistanceScale = 1.0f;
            windResistanceScale = 1.0f;
            waveDriftingScale = 1.0f;
            slammingScale = 1.0f;
            limit = 10f;
            hydrostaticLimit = 10.0f;
            viscousWaterResistanceLimit = 10.0f;
            pressureDragLimit = 10.0f;
            airResistanceLimit = 10.0f;
            windResistanceLimit = 10.0f;
            waveDriftingLimit = 10.0f;
            slammingLimit = 10.0f;
            CoefficientDragFlatPlatePerpendicularToFlow = 1.28f;
            CoefficientWindResistance = 1.0f;
            CoefficientAirResistance = 1.0f;
            Viscosity = 0.000001f;
            linearPressureDragCoefficient = 10f;
            quadraticPressureDragCoefficient = 10f;
            facingDotProductFalloffPower = 0.5f;
            linearSuctionDragCoefficient = 10f;
            quadraticSuctionDragCoefficient = 10f;
            suctionFalloffPower = 0.5f;

            slamEntryCoefficient = 1.0f;
            slamExitCoefficient = 1.0f;

            slamEntryHorizontality = .1f;
            slamExitHorizontality = .1f;

            maximumDrag = 0f;
            maximumAngularDrag = 0f;
            maximumScale = 0f;
            maximumLimit = 0f;
            minimumCoefficient = 0f;
            maximumCoefficient = 0f;
            minimumSlamCoefficient = 0f;
            maximumSlamCoefficient = 0f;
            minimumDragCoefficient = 0f;
            maximumDragCoefficient = 0f;

            maximumMaximum = 100f;

            ConfirmRanges();
        }

        public void ConfirmRanges()
        {
            if (maximumDrag == 0f)
            {
                maximumDrag = 20f;
            }

            if (maximumAngularDrag == 0f)
            {
                maximumAngularDrag = 1f;
            }

            if (maximumScale == 0f)
            {
                maximumScale = 50f;
            }

            if (maximumLimit == 0f)
            {
                maximumLimit = 10f;
            }

            if (minimumCoefficient == 0f)
            {
                minimumCoefficient = .5f;
            }

            if (maximumCoefficient == 0f)
            {
                maximumCoefficient = 1.5f;
            }

            if (minimumSlamCoefficient == 0f)
            {
                minimumSlamCoefficient = .1f;
            }

            if (maximumSlamCoefficient == 0f)
            {
                maximumSlamCoefficient = 100f;
            }

            if (minimumDragCoefficient == 0f)
            {
                minimumDragCoefficient = .1f;
            }

            if (maximumDragCoefficient == 0f)
            {
                maximumDragCoefficient = 20f;
            }

            maximumDrag = math.clamp(maximumDrag,                       0f, maximumMaximum);
            maximumAngularDrag = math.clamp(maximumAngularDrag,         0f, maximumMaximum);
            maximumScale = math.clamp(maximumScale,                     0f, maximumMaximum);
            maximumLimit = math.clamp(maximumLimit,                     0f, maximumMaximum);
            minimumCoefficient = math.clamp(minimumCoefficient,         0f, maximumMaximum);
            maximumCoefficient = math.clamp(maximumCoefficient,         0f, maximumMaximum);
            minimumSlamCoefficient = math.clamp(minimumSlamCoefficient, 0f, maximumMaximum);
            maximumSlamCoefficient = math.clamp(maximumSlamCoefficient, 0f, maximumMaximum);
            minimumDragCoefficient = math.clamp(minimumDragCoefficient, 0f, maximumMaximum);
            maximumDragCoefficient = math.clamp(maximumDragCoefficient, 0f, maximumMaximum);

            additionalDrag = math.clamp(additionalDrag,               0f, maximumDrag);
            additionalAngularDrag = math.clamp(additionalAngularDrag, 0f, maximumAngularDrag);

            scale = math.clamp(scale,                                             0f, maximumScale);
            hydrostaticScale = math.clamp(hydrostaticScale,                       0f, maximumScale);
            viscousWaterResistanceScale = math.clamp(viscousWaterResistanceScale, 0f, maximumScale);
            pressureDragScale = math.clamp(pressureDragScale,                     0f, maximumScale);
            airResistanceScale = math.clamp(airResistanceScale,                   0f, maximumScale);
            windResistanceScale = math.clamp(windResistanceScale,                 0f, maximumScale);
            waveDriftingScale = math.clamp(waveDriftingScale,                     0f, maximumScale);
            slammingScale = math.clamp(slammingScale,                             0f, maximumScale);

            limit = math.clamp(limit,                                             0f, maximumLimit);
            hydrostaticLimit = math.clamp(hydrostaticLimit,                       0f, maximumLimit);
            viscousWaterResistanceLimit = math.clamp(viscousWaterResistanceLimit, 0f, maximumLimit);
            pressureDragLimit = math.clamp(pressureDragLimit,                     0f, maximumLimit);
            airResistanceLimit = math.clamp(airResistanceLimit,                   0f, maximumLimit);
            windResistanceLimit = math.clamp(windResistanceLimit,                 0f, maximumLimit);
            waveDriftingLimit = math.clamp(waveDriftingLimit,                     0f, maximumLimit);
            slammingLimit = math.clamp(slammingLimit,                             0f, maximumLimit);

            CoefficientWindResistance = math.clamp(
                CoefficientWindResistance,
                minimumCoefficient,
                maximumCoefficient
            );
            CoefficientAirResistance = math.clamp(
                CoefficientAirResistance,
                minimumCoefficient,
                maximumCoefficient
            );
            CoefficientDragFlatPlatePerpendicularToFlow = math.clamp(
                CoefficientDragFlatPlatePerpendicularToFlow,
                minimumCoefficient,
                maximumCoefficient
            );

            slamEntryCoefficient = math.clamp(
                slamEntryCoefficient,
                minimumSlamCoefficient,
                maximumSlamCoefficient
            );
            slamExitCoefficient = math.clamp(
                slamExitCoefficient,
                minimumSlamCoefficient,
                maximumSlamCoefficient
            );

            linearPressureDragCoefficient = math.clamp(
                linearPressureDragCoefficient,
                minimumDragCoefficient,
                maximumDragCoefficient
            );
            quadraticPressureDragCoefficient = math.clamp(
                quadraticPressureDragCoefficient,
                minimumDragCoefficient,
                maximumDragCoefficient
            );

            linearSuctionDragCoefficient = math.clamp(
                linearSuctionDragCoefficient,
                minimumDragCoefficient,
                maximumDragCoefficient
            );
            quadraticSuctionDragCoefficient = math.clamp(
                quadraticSuctionDragCoefficient,
                minimumDragCoefficient,
                maximumDragCoefficient
            );
        }

#region IEquatable

        public bool Equals(WaterPhysicsCoefficentData other)
        {
            return maximumMaximum.Equals(other.maximumMaximum) &&
                   maximumDrag.Equals(other.maximumDrag) &&
                   maximumAngularDrag.Equals(other.maximumAngularDrag) &&
                   maximumScale.Equals(other.maximumScale) &&
                   maximumLimit.Equals(other.maximumLimit) &&
                   minimumCoefficient.Equals(other.minimumCoefficient) &&
                   maximumCoefficient.Equals(other.maximumCoefficient) &&
                   minimumSlamCoefficient.Equals(other.minimumSlamCoefficient) &&
                   maximumSlamCoefficient.Equals(other.maximumSlamCoefficient) &&
                   minimumDragCoefficient.Equals(other.minimumDragCoefficient) &&
                   maximumDragCoefficient.Equals(other.maximumDragCoefficient) &&
                   additionalDrag.Equals(other.additionalDrag) &&
                   additionalAngularDrag.Equals(other.additionalAngularDrag) &&
                   (disableGravity == other.disableGravity) &&
                   scale.Equals(other.scale) &&
                   hydrostaticScale.Equals(other.hydrostaticScale) &&
                   viscousWaterResistanceScale.Equals(other.viscousWaterResistanceScale) &&
                   pressureDragScale.Equals(other.pressureDragScale) &&
                   airResistanceScale.Equals(other.airResistanceScale) &&
                   windResistanceScale.Equals(other.windResistanceScale) &&
                   waveDriftingScale.Equals(other.waveDriftingScale) &&
                   slammingScale.Equals(other.slammingScale) &&
                   limit.Equals(other.limit) &&
                   hydrostaticLimit.Equals(other.hydrostaticLimit) &&
                   viscousWaterResistanceLimit.Equals(other.viscousWaterResistanceLimit) &&
                   pressureDragLimit.Equals(other.pressureDragLimit) &&
                   airResistanceLimit.Equals(other.airResistanceLimit) &&
                   windResistanceLimit.Equals(other.windResistanceLimit) &&
                   waveDriftingLimit.Equals(other.waveDriftingLimit) &&
                   slammingLimit.Equals(other.slammingLimit) &&
                   Viscosity.Equals(other.Viscosity) &&
                   CoefficientWindResistance.Equals(other.CoefficientWindResistance) &&
                   CoefficientAirResistance.Equals(other.CoefficientAirResistance) &&
                   CoefficientDragFlatPlatePerpendicularToFlow.Equals(
                       other.CoefficientDragFlatPlatePerpendicularToFlow
                   ) &&
                   slamEntryCoefficient.Equals(other.slamEntryCoefficient) &&
                   slamExitCoefficient.Equals(other.slamExitCoefficient) &&
                   slamEntryHorizontality.Equals(other.slamEntryHorizontality) &&
                   slamExitHorizontality.Equals(other.slamExitHorizontality) &&
                   linearPressureDragCoefficient.Equals(other.linearPressureDragCoefficient) &&
                   quadraticPressureDragCoefficient.Equals(
                       other.quadraticPressureDragCoefficient
                   ) &&
                   facingDotProductFalloffPower.Equals(other.facingDotProductFalloffPower) &&
                   linearSuctionDragCoefficient.Equals(other.linearSuctionDragCoefficient) &&
                   quadraticSuctionDragCoefficient.Equals(other.quadraticSuctionDragCoefficient) &&
                   suctionFalloffPower.Equals(other.suctionFalloffPower);
        }

        public override bool Equals(object obj)
        {
            return obj is WaterPhysicsCoefficentData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = maximumMaximum.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumDrag.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumAngularDrag.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumScale.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ minimumCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ minimumSlamCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumSlamCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ minimumDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ maximumDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ additionalDrag.GetHashCode();
                hashCode = (hashCode * 397) ^ additionalAngularDrag.GetHashCode();
                hashCode = (hashCode * 397) ^ disableGravity.GetHashCode();
                hashCode = (hashCode * 397) ^ scale.GetHashCode();
                hashCode = (hashCode * 397) ^ hydrostaticScale.GetHashCode();
                hashCode = (hashCode * 397) ^ viscousWaterResistanceScale.GetHashCode();
                hashCode = (hashCode * 397) ^ pressureDragScale.GetHashCode();
                hashCode = (hashCode * 397) ^ airResistanceScale.GetHashCode();
                hashCode = (hashCode * 397) ^ windResistanceScale.GetHashCode();
                hashCode = (hashCode * 397) ^ waveDriftingScale.GetHashCode();
                hashCode = (hashCode * 397) ^ slammingScale.GetHashCode();
                hashCode = (hashCode * 397) ^ limit.GetHashCode();
                hashCode = (hashCode * 397) ^ hydrostaticLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ viscousWaterResistanceLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ pressureDragLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ airResistanceLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ windResistanceLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ waveDriftingLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ slammingLimit.GetHashCode();
                hashCode = (hashCode * 397) ^ Viscosity.GetHashCode();
                hashCode = (hashCode * 397) ^ CoefficientWindResistance.GetHashCode();
                hashCode = (hashCode * 397) ^ CoefficientAirResistance.GetHashCode();
                hashCode = (hashCode * 397) ^
                           CoefficientDragFlatPlatePerpendicularToFlow.GetHashCode();
                hashCode = (hashCode * 397) ^ slamEntryCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ slamExitCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ slamEntryHorizontality.GetHashCode();
                hashCode = (hashCode * 397) ^ slamExitHorizontality.GetHashCode();
                hashCode = (hashCode * 397) ^ linearPressureDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ quadraticPressureDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ facingDotProductFalloffPower.GetHashCode();
                hashCode = (hashCode * 397) ^ linearSuctionDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ quadraticSuctionDragCoefficient.GetHashCode();
                hashCode = (hashCode * 397) ^ suctionFalloffPower.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(
            WaterPhysicsCoefficentData left,
            WaterPhysicsCoefficentData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            WaterPhysicsCoefficentData left,
            WaterPhysicsCoefficentData right)
        {
            return !left.Equals(right);
        }

#endregion
    }
}
