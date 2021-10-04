using System;
using Appalachia.Base.Scriptables;
using Appalachia.Simulation.Core.Metadata.Tree.Types;

namespace Appalachia.Simulation.Core.Metadata.Tree
{
    [Serializable]
    public class TreeIndividualMetadata : SelfSavingAndIdentifyingScriptableObject<TreeIndividualMetadata>
    {
        public int individualID;
        
        public TreeAgeMetadata sapling;
        public TreeAgeMetadata young;
        public TreeAgeMetadata adult;
        public TreeAgeMetadata mature;
        public TreeAgeMetadata spirit;

        public TreeAgeMetadata this[AgeType age]
        {
            get
            {
                switch (age)
                {
                    case AgeType.None:
                        return null;
                    case AgeType.Sapling:
                        return sapling;
                    case AgeType.Young:
                        return young;
                    case AgeType.Adult:
                        return adult;
                    case AgeType.Mature:
                        return mature;
                    case AgeType.Spirit:
                        return spirit;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(age), age, null);
                }
            }
        }

        public void Set(AgeType ageType, TreeAgeMetadata tam)
        {
            switch (ageType)
            {
                case AgeType.Sapling:
                    sapling = tam;
                    return;
                case AgeType.Young:
                    young = tam;
                    return;
                case AgeType.Adult:
                    adult = tam;
                    return;
                case AgeType.Mature:
                    mature = tam;
                    return;
                case AgeType.Spirit:
                    spirit = tam;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ageType), ageType, null);
            }
        }
        
        public bool TryGet(AgeType ageType, out TreeAgeMetadata ageMetadata)
        {
            switch (ageType)
            {
                case AgeType.Sapling:
                    ageMetadata = sapling;
                    break;
                case AgeType.Young:
                    ageMetadata = young;
                    break;
                case AgeType.Adult:
                    ageMetadata = adult;
                    break;
                case AgeType.Mature:
                    ageMetadata = mature;
                    break;
                case AgeType.Spirit:
                    ageMetadata = spirit;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ageType), ageType, null);
            }
            
            return ageMetadata != null;
        }
    }
}