#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_Enter : CollisionRelay
    {
        private void OnCollisionEnter(Collision other)
        {
            OnRelayedCollisionEnter?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedCollision OnRelayedCollisionEnter;
    }
}
