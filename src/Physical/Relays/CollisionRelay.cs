#region

using Appalachia.Filtering;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [DisallowMultipleComponent]
    public abstract class CollisionRelay : ColliderRelayBase
    {
        protected override Collider[] GetColliders()
        {
            return this.FilterComponents<Collider>(false).NoTriggers().RunFilter();
        }
    }
}
