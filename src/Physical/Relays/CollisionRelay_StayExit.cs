#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_StayExit : CollisionRelay
    {
        public event OnRelayedCollision OnRelayedCollisionStay;
        public event OnRelayedCollision OnRelayedCollisionExit;

        private void OnCollisionStay(Collision other)
        {
            OnRelayedCollisionStay?.Invoke(this, relayingColliders, other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnRelayedCollisionExit?.Invoke(this, relayingColliders, other);
        }
    }
}
