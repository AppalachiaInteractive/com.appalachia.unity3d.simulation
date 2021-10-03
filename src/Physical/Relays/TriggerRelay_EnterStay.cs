#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_EnterStay : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerEnter;
        public event OnRelayedTrigger OnRelayedTriggerStay;

        private void OnTriggerEnter(Collider other)
        {
            OnRelayedTriggerEnter?.Invoke(this, relayingColliders, other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnRelayedTriggerStay?.Invoke(this, relayingColliders, other);
        }
    }
}
