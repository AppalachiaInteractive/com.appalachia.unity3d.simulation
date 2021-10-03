#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_Enter : CollisionRelay
    {
        public event OnRelayedCollision OnRelayedCollisionEnter;

        private void OnCollisionEnter(Collision other)
        {
            OnRelayedCollisionEnter?.Invoke(this, relayingColliders, other);
        }
    }
}
