#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_EnterExit : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerEnter;
        public event OnRelayedTrigger OnRelayedTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            OnRelayedTriggerEnter?.Invoke(this, relayingColliders, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnRelayedTriggerExit?.Invoke(this, relayingColliders, other);
        }
    }
}
