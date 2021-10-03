#region

using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    public delegate void OnRelayedCollision(CollisionRelay relay, Collider[] these, Collision other);
}
