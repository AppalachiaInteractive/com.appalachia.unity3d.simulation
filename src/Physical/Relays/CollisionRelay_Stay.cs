#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_Stay : CollisionRelay
    {
        public event OnRelayedCollision OnRelayedCollisionStay;

        private void OnCollisionStay(Collision other)
        {
            OnRelayedCollisionStay?.Invoke(this, relayingColliders, other);
        }
    }
}
