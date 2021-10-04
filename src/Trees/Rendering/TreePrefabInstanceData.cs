using System;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Rendering
{
    [Serializable]
    public class TreePrefabInstanceData
    {
        public Matrix4x4 originalTransform;
        public Matrix4x4 currentTransform;
        public TreePrefabData prefabData;
        
        public StageType currentStage;
        public double currentStageChangeTime;
        public TreePrefabInstanceState prefabState;
        public int treePrefabInstanceID;

        public bool IsNormal => currentStage == StageType.Normal;

        public bool IsStump => currentStage == StageType.Stump || currentStage == StageType.StumpRotted;

        public bool IsRotted =>
            currentStage == StageType.StumpRotted ||
            currentStage == StageType.DeadFelledRotted ||
            currentStage == StageType.FelledBareRotted;

        public bool IsFelled =>
            currentStage == StageType.Felled ||
            currentStage == StageType.FelledBare ||
            currentStage == StageType.FelledBareRotted ||
            currentStage == StageType.DeadFelled ||
            currentStage == StageType.DeadFelled;

        public bool IsDead =>
            currentStage == StageType.Dead ||
            currentStage == StageType.DeadFelled ||
            currentStage == StageType.DeadFelledRotted;

        public bool IsBare => currentStage == StageType.FelledBare || currentStage == StageType.FelledBareRotted;

        public bool CanCut =>
            !IsFelled &&
            ((IsNormal && prefabData.prototypes.felled != null && prefabData.prototypes.stump != null) ||
                (IsDead && prefabData.prototypes.deadFelled != null && prefabData.prototypes.stump != null));

        public bool CanRot =>
            !IsRotted &&
            ((IsStump && prefabData.prototypes.stumpRotted != null) ||
                (IsDead && IsFelled && prefabData.prototypes.deadFelledRotted != null) ||
                (IsFelled && IsBare && prefabData.prototypes.felledBareRotted != null));

        public bool CanBare => !IsBare && !IsDead && IsFelled && prefabData.prototypes.felledBare != null;

        public bool CanRevertRot =>
            IsRotted && (IsFelled && IsBare && prefabData.prototypes.felledBare != null) ||
            (IsFelled && IsDead && prefabData.prototypes.deadFelled != null) ||
            (IsStump && prefabData.prototypes.stump != null);
    }
}