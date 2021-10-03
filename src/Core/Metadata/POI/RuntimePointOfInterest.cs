using System;
using Appalachia.Core.Editing;
using UnityEngine;

namespace Appalachia.Core.Trees
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
