using System;

namespace Appalachia.Simulation.Core.Metadata.Tree.Types
{
    public static class StageTypeExtensions
    {
        public static bool IsDead(this StageType stageType)
        {
            switch (stageType)
            {
                case StageType.Normal:
                case StageType.Stump:
                case StageType.Felled:
                    return false;
                case StageType.StumpRotted:
                case StageType.FelledBare:
                case StageType.FelledBareRotted:
                case StageType.Dead:
                case StageType.DeadFelled:
                case StageType.DeadFelledRotted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public static bool IsBare(this StageType stageType)
        {
            switch (stageType)
            {
                case StageType.Normal:
                case StageType.Stump:
                case StageType.Felled:
                    return false;
                case StageType.StumpRotted:
                case StageType.FelledBare:
                case StageType.FelledBareRotted:
                case StageType.Dead:
                case StageType.DeadFelled:
                case StageType.DeadFelledRotted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public static bool IsRotted(this StageType stageType)
        {
            switch (stageType)
            {
                case StageType.Normal:
                case StageType.Stump:
                case StageType.Felled:
                case StageType.FelledBare:
                case StageType.Dead:
                case StageType.DeadFelled:
                    return false;
                case StageType.StumpRotted:
                case StageType.FelledBareRotted:
                case StageType.DeadFelledRotted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public static bool IsFelled(this StageType stageType)
        {
            switch (stageType)
            {
                case StageType.Normal:
                case StageType.Stump:
                case StageType.Dead:
                case StageType.StumpRotted:
                    return false;
                case StageType.Felled:
                case StageType.FelledBare:
                case StageType.DeadFelled:
                case StageType.FelledBareRotted:
                case StageType.DeadFelledRotted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public static bool IsStump(this StageType stageType)
        {
            switch (stageType)
            {
                case StageType.Normal:
                case StageType.Dead:
                case StageType.Felled:
                case StageType.FelledBare:
                case StageType.DeadFelled:
                case StageType.FelledBareRotted:
                case StageType.DeadFelledRotted:
                    return false;
                case StageType.Stump:
                case StageType.StumpRotted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public static bool IsStumpOrFelled(this StageType stageType)
        {
            return IsFelled(stageType) || IsStump(stageType);
        }
    }
}
