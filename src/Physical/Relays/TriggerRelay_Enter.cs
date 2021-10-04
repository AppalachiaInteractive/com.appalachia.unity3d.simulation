#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public class TriggerRelay_Enter : TriggerRelay
    {
        private void OnTriggerEnter(Collider other)
        {
            OnRelayedTriggerEnter?.Invoke(this, relayingColliders, other);
        }

        public event OnRelayedTrigger OnRelayedTriggerEnter;
    }
}
