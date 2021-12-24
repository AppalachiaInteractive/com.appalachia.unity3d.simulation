#region

using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Root;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Simulation.Buoyancy
{
    public class BuoyancyVoxelDataGizmoSettings : SingletonAppalachiaObject<BuoyancyVoxelDataGizmoSettings>
    {
        #region Constants and Static Readonly

        private const string _GC = "Cumulative";
        private const string _GS = "Selected";
        private const string _GSC = "Selected/Center Of Mass";
        private const string _GSF = "Selected/Force";
        private const string _GSFL = "Selected/Force/Lines";
        private const string _GSS = "Selected/Submersion";

        #endregion

        #region Fields and Autoproperties

        [SmartLabel(Postfix = true)]
        public bool drawGizmos;

        [FoldoutGroup(_GC)]
        [SmartLabel(Postfix = true)]
        public bool drawCumulativeGizmos;

        [FoldoutGroup(_GC)]
        [SmartLabel(Postfix = true)]
        public bool drawCumulativeForceGizmos;

        [FoldoutGroup(_GC)]
        [SmartLabel(Postfix = true)]
        public bool drawCumulativeWaterLevelGizmos;

        [FoldoutGroup(_GS)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedGizmos;

        [BoxGroup(_GSC)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedCenterOfMass;

        [BoxGroup(_GSC)]
        [SmartLabel]
        public float gizmoCenterOfMassSize;

        [BoxGroup(_GSS)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedSubmersionPositions;

        [BoxGroup(_GSS)]
        [SmartLabel]
        public float gizmoSubmersionBaseSize;

        [BoxGroup(_GSS)]
        [SmartLabel]
        public float gizmoSubmersionFlexSize;

        [BoxGroup(_GSS)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedWaterLines;

        [BoxGroup(_GSF)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedForcePositions;

        [BoxGroup(_GSF)]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float gizmoForceBaseSize;

        [BoxGroup(_GSF)]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float gizmoForceFlexSize;

        [BoxGroup(_GSF)]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float gizmoForceSizeLimit;

        [SmartLabel(Postfix = true)]
        public bool drawSelectedForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel]
        [PropertyRange(0f, 1f)]
        public float drawSelectedForceLinesScale;

        [BoxGroup(_GSFL)]
        [SmartLabel]
        [PropertyRange(0.001f, .05f)]
        public float drawSelectedForceLinesOffset;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedHydrostaticForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedViscousWaterResistanceForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedPressureDragForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedAirResistanceForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedWindResistanceForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedWaveDriftingForceLines;

        [BoxGroup(_GSFL)]
        [SmartLabel(Postfix = true)]
        public bool drawSelectedSlammingForceLines;

        #endregion

        
    }
}
