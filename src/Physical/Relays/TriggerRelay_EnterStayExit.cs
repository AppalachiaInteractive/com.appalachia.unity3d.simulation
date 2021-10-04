#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_EnterStayExit : TriggerRelay
    {
        private void OnTriggerEnter(Collider other)
        {
            OnRelayedTriggerEnter?.Invoke(this, relayingColliders, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnRelayedTriggerExit?.Invoke(this, relayingColliders, other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnRelayedTriggerStay?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedTrigger OnRelayedTriggerEnter;
        public event OnRelayedTrigger OnRelayedTriggerStay;
        public event OnRelayedTrigger OnRelayedTriggerExit;
    }
}
