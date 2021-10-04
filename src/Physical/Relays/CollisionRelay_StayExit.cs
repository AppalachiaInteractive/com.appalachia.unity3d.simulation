#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_StayExit : CollisionRelay
    {
        private void OnCollisionExit(Collision other)
        {
            OnRelayedCollisionExit?.Invoke(this, relayingColliders, other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnRelayedCollisionStay?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedCollision OnRelayedCollisionStay;
        public event OnRelayedCollision OnRelayedCollisionExit;
    }
}
