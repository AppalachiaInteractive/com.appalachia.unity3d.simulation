#region

using Appalachia.Core.Objects.Filtering;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [DisallowMultipleComponent]
    public abstract class TriggerRelay : ColliderRelayBase
    {
        protected override Collider[] GetColliders()
        {
            return this.FilterComponents<Collider>(false).OnlyTriggers().RunFilter();
        }
    }
}
