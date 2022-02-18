#region

using Appalachia.Core.Objects.Filtering;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [DisallowMultipleComponent]
    public abstract class CollisionRelay : ColliderRelayBase
    {
        /// <inheritdoc />
        protected override Collider[] GetColliders()
        {
            return this.FilterComponents<Collider>(false).NoTriggers().RunFilter();
        }
    }
}
