#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class CollisionRelay_EnterExit : CollisionRelay
    {
        private void OnCollisionEnter(Collision other)
        {
            OnRelayedCollisionEnter?.Invoke(this, relayingColliders, other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnRelayedCollisionExit?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedCollision OnRelayedCollisionEnter;
        public event OnRelayedCollision OnRelayedCollisionExit;
    }
}
