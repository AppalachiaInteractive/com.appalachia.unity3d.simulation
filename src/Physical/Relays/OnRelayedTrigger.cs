#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public delegate void OnRelayedTrigger(TriggerRelay relay, Collider[] these, Collider other);
}
