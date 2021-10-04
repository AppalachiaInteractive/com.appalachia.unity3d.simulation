#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_Exit : TriggerRelay
    {
        private void OnTriggerExit(Collider other)
        {
            OnRelayedTriggerExit?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedTrigger OnRelayedTriggerExit;
    }
}
