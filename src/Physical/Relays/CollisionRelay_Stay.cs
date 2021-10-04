#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_Stay : CollisionRelay
    {
        private void OnCollisionStay(Collision other)
        {
            OnRelayedCollisionStay?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedCollision OnRelayedCollisionStay;
    }
}
