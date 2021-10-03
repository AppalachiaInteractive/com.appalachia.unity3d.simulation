using System;
using Appalachia.Core.Trees.Types;

namespace Appalachia.Core.Trees.Metadata
{
    [Serializable]
    public class TreeAgeMetadata
    {
        public TreeStageMetadata normal;
        public TreeStageMetadata stump;
        public TreeStageMetadata stumpRotted;
        public TreeStageMetadata felled;
        public TreeStageMetadata felledBare;
        public TreeStageMetadata felledBareRotted;
        public TreeStageMetadata dead;
        public TreeStageMetadata deadFelled;
        public TreeStageMetadata deadFelledRotted;
        
        

        public TreeStageMetadata this[StageType stage]
        {
            get
            {
                switch (stage)
                {
                    case StageType.Normal:
                        return normal;
                    case StageType.Stump:
                        return stump;
                    case StageType.StumpRotted:
                        return stumpRotted;
                    case StageType.Felled:
                        return felled;
                    case StageType.FelledBare:
                        return felledBare;
                    case StageType.FelledBareRotted:
                        return felledBareRotted;
                    case StageType.Dead:
                        return dead;
                    case StageType.DeadFelled:
                        return deadFelled;
                    case StageType.DeadFelledRotted:
                        return deadFelledRotted;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
                }
            }
        }

        public void Set(StageType stageType, TreeStageMetadata tsm)
        {
            switch (stageType)
            {
                case StageType.Normal:
                    normal = tsm;
                    return;
                case StageType.Stump:
                    stump = tsm;
                    return;
                case StageType.StumpRotted:
                    stumpRotted = tsm;
                    return;
                case StageType.Felled:
                    felled = tsm;
                    return;
                case StageType.FelledBare:
                    felledBare = tsm;
                    return;
                case StageType.FelledBareRotted:
                    felledBareRotted = tsm;
                    return;
                case StageType.Dead:
                    dead = tsm;
                    return;
                case StageType.DeadFelled:
                    deadFelled = tsm;
                    return;
                case StageType.DeadFelledRotted:
                    deadFelledRotted = tsm;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stageType), stageType, null);
            }
        }

        public bool CanRevive(StageType stageType)
        {
            return normal != null;
        }


        public bool CanCut(StageType stageType)
        {
            if (stageType == StageType.Normal)
            {
                return stump != null && felled != null;
            }

            if (stageType == StageType.Dead)
            {
                return stump != null && deadFelled != null;
            }

            return false;
        }
        
        
        public bool CanBare(StageType stageType)
        {
            if (stageType == StageType.Felled)
            {
                return felledBare != null;
            }

            if (stageType == StageType.StumpRotted)
            {
                return stump != null;
            }

            if (stageType == StageType.DeadFelledRotted)
            {
                return deadFelled != null;
            }

            if (stageType == StageType.FelledBareRotted)
            {
                return felledBare != null;
            }

            return false;
        }
        
        
        public bool CanRot(StageType stageType)
        {
            if (stageType == StageType.Stump)
            {
                return stumpRotted != null;
            }

            if (stageType == StageType.FelledBare)
            {
                return felledBareRotted != null;
            }

            if (stageType == StageType.DeadFelled)
            {
                return deadFelledRotted != null;
            }
            
            return false;
        }
    }
}