#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_Exit : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerExit;

        private void OnTriggerExit(Collider other)
        {
            OnRelayedTriggerExit?.Invoke(this, relayingColliders, other);
        }
    }
}
