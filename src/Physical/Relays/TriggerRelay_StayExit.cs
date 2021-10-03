#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_StayExit : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerStay;
        public event OnRelayedTrigger OnRelayedTriggerExit;

        private void OnTriggerStay(Collider other)
        {
            OnRelayedTriggerStay?.Invoke(this, relayingColliders, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnRelayedTriggerExit?.Invoke(this, relayingColliders, other);
        }
    }
}
