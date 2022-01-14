#region

using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [ExecuteAlways]
    public abstract class ColliderRelayBase : AppalachiaBehaviour<ColliderRelayBase>
    {
        #region Fields and Autoproperties

        public Collider[] relayingColliders;

        #endregion

        protected abstract Collider[] GetColliders();

        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            relayingColliders = null;
        }

        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();

            UpdateRelayingColliders();
        }

        [Button]
        private void UpdateRelayingColliders()
        {
            relayingColliders = GetColliders();
        }
    }
}
