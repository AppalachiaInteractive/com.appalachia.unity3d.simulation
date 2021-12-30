using System;
using Appalachia.Core.Layers;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.POI
{
    [Serializable]
    public class RuntimePointOfInterest
    {
        #region Fields and Autoproperties

        public float radius;

        public LayerSelection layer;

        public RuntimePointOfInterestType zoneType;

        public SphereCollider sphereCollider;
        public string name;

        public Vector3 position;

        #endregion
    }
}
