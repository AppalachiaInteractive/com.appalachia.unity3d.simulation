using System;
using Appalachia.Core.Layers;
using UnityEngine;

namespace Appalachia.Simulation.Core.Metadata.POI
{
    [Serializable]
    public class RuntimePointOfInterest
    {
        public string name;
        
        public RuntimePointOfInterestType zoneType;

        public float radius;

        public Vector3 position;

        public LayerSelection layer;

        public SphereCollider sphereCollider;
        
        
    }
}
