#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_Enter : TriggerRelay
    {
        public event OnRelayedTrigger OnRelayedTriggerEnter;

        private void OnTriggerEnter(Collider other)
        {
            OnRelayedTriggerEnter?.Invoke(this, relayingColliders, other);
        }
    }
}
