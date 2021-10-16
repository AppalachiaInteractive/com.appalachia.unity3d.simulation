#region

using System.Collections.Generic;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Definition;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Trees.Seeds
{
    public static class SeedManager
    {
        public static void UpdateSeeds(TSEDataContainer dataContainer)
        {
            if (dataContainer is TreeDataContainer t)
            {
                UpdateSeeds(t.species);
            }
            else if (dataContainer is LogDataContainer l)
            {
                UpdateSeeds(l.log);
            }
            else if (dataContainer is BranchDataContainer b)
            {
                UpdateSeeds(b.branch);
            }            
        }

        public static void UpdateSeeds(TreeSpecies species, IReadOnlyCollection<TreeIndividual> individuals)
        {
            using (BUILD_TIME.SEED_MGR.UpdateSeeds.Auto())
            {
                UpdateSeeds(species);

                foreach (var individual in individuals)
                {
                    if (individual.seed == null)
                    {
                        individual.seed = new InternalSeed(
                            species.Seed.secondary.RandomValue(0, BaseSeed.HIGH_ELEMENT)
                        );
                    }
                    else
                    {
                        individual.seed.SetInternalSeed(species.Seed.secondary.RandomValue(0, BaseSeed.HIGH_ELEMENT));
                    }

                    individual.seed.Reset();
                }
            }
        }
        
        public static void UpdateSeeds(TreeSpecies species)
        {
            using (BUILD_TIME.SEED_MGR.UpdateSeeds.Auto())
            {
                if (species.Seed == null)
                {
                    species.Seed = new ExternalDualSeed(
                        Random.Range(0, BaseSeed.HIGH_ELEMENT),
                        Random.Range(0, BaseSeed.HIGH_ELEMENT),
                        Random.Range(0, BaseSeed.HIGH_ELEMENT)
                    );
                }
                else
                {
                    species.Seed.Reset();
                }

                foreach (var hierarchy in species)
                {
                    if (hierarchy.seed == null)
                    {
                        hierarchy.seed = new ExternalSeed(
                            species.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT),
                            species.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT)
                        );
                    }
                    else
                    {
                        hierarchy.seed.SetInternalSeed(species.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT));

                        species.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT);
                    }

                    hierarchy.seed.Reset();
                }
            }
        }
        
        public static void UpdateSeeds(TreeLog log, IReadOnlyCollection<LogInstance> instances)
        {
            using (BUILD_TIME.SEED_MGR.UpdateSeeds.Auto())
            {
                UpdateSeeds(log);

                foreach (var instance in instances)
                {
                    if (instance.seed == null)
                    {
                        instance.seed = new InternalSeed(
                            log.Seed.secondary.RandomValue(0, BaseSeed.HIGH_ELEMENT)
                        );
                    }
                    else
                    {
                        instance.seed.SetInternalSeed(log.Seed.secondary.RandomValue(0, BaseSeed.HIGH_ELEMENT));
                    }

                    instance.seed.Reset();
                }
            }
        }
        
        public static void UpdateSeeds(TreeLog log)
        {
            using (BUILD_TIME.SEED_MGR.UpdateSeeds.Auto())
            {
                if (log.Seed == null)
                {
                    log.Seed = new ExternalDualSeed(
                        Random.Range(0, BaseSeed.HIGH_ELEMENT),
                        Random.Range(0, BaseSeed.HIGH_ELEMENT),
                        Random.Range(0, BaseSeed.HIGH_ELEMENT)
                    );
                }
                else
                {
                    log.Seed.Reset();
                }

                foreach (var hierarchy in log)
                {
                    if (hierarchy.seed == null)
                    {
                        hierarchy.seed = new ExternalSeed(
                            log.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT),
                            log.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT)
                        );
                    }
                    else
                    {
                        hierarchy.seed.SetInternalSeed(log.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT));

                        log.Seed.primary.RandomValue(0, BaseSeed.HIGH_ELEMENT);
                    }

                    hierarchy.seed.Reset();
                }
            }
        }

        public static void UpdateSeeds(TreeBranch branch)
        {
            using (BUILD_TIME.SEED_MGR.UpdateSeeds.Auto())
            {                
                if (branch.Seed == null)
                {
                    branch.Seed = new ExternalSeed(
                        Random.Range(0, BaseSeed.HIGH_ELEMENT),
                        Random.Range(0, BaseSeed.HIGH_ELEMENT)
                    );
                }
                else
                {
                    branch.Seed.Reset();
                }

                foreach (var hierarchy in branch)
                {
                    if (hierarchy.seed == null)
                    {
                        hierarchy.seed = new ExternalSeed(
                            branch.Seed.RandomValue(0, BaseSeed.HIGH_ELEMENT),
                            branch.Seed.RandomValue(0, BaseSeed.HIGH_ELEMENT)
                        );
                    }
                    else
                    {
                        hierarchy.seed.SetInternalSeed(branch.Seed.RandomValue(0, BaseSeed.HIGH_ELEMENT));

                        branch.Seed.RandomValue(0, BaseSeed.HIGH_ELEMENT);
                    }

                    hierarchy.seed.Reset();
                }
            }
        }

    }
}
