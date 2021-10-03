#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_Exit : CollisionRelay
    {
        public event OnRelayedCollision OnRelayedCollisionExit;

        private void OnCollisionExit(Collision other)
        {
            OnRelayedCollisionExit?.Invoke(this, relayingColliders, other);
        }
    }
}
