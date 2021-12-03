#region

using Appalachia.Core.Behaviours;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Simulation.Physical.Relays
{
    [ExecuteAlways]
    public abstract class ColliderRelayBase: AppalachiaBehaviour
    {
        public Collider[] relayingColliders;

        protected override void Awake()
        {
            base.Awake();
            
            UpdateRelayingColliders();
        }

        protected override void Start()
        {
            base.Start();
            
            UpdateRelayingColliders();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            UpdateRelayingColliders();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            relayingColliders = null;
        }

        [Button]
        private void UpdateRelayingColliders()
        {
            relayingColliders = GetColliders();
        }

        protected abstract Collider[] GetColliders();
    }
}
