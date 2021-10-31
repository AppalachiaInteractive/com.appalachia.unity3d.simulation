using System.Collections.Generic;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Core.Metadata.Wood;

namespace Appalachia.Simulation.Core.Metadata.Tree
{
    public class TreeSpeciesMetadata : IdentifiableAppalachiaObject<TreeSpeciesMetadata>
    {
        public string speciesName;

        public WoodSimulationData woodData;

        public List<TreeIndividualMetadata> individuals = new();

        private Dictionary<int, TreeIndividualMetadata> _lookup;

        public TreeIndividualMetadata GetIndividual(int individualID)
        {
            if (_lookup == null)
            {
                _lookup = new Dictionary<int, TreeIndividualMetadata>();
            }

            if (_lookup.Count == 0)
            {
                for (var i = 0; i < individuals.Count; i++)
                {
                    var individual = individuals[i];

                    _lookup.Add(individual.individualID, individual);
                }
            }

            return _lookup[individualID];
        }
    }
}
