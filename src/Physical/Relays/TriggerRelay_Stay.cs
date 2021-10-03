#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_Stay : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerStay;

        private void OnTriggerStay(Collider other)
        {
            OnRelayedTriggerStay?.Invoke(this, relayingColliders, other);
        }
    }
}
