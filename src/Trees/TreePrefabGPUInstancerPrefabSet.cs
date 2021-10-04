using System;
using GPUInstancer;

namespace Appalachia.Simulation.Trees
{
    [Serializable]
    public struct TreePrefabGPUInstancerPrefabSet
    {
        public GPUInstancerPrefab normal;
        public GPUInstancerPrefab stump;
        public GPUInstancerPrefab stumpRotted;
        public GPUInstancerPrefab felled;
        public GPUInstancerPrefab felledBare;
        public GPUInstancerPrefab felledBareRotted;
        public GPUInstancerPrefab dead;
        public GPUInstancerPrefab deadFelled;
        public GPUInstancerPrefab deadFelledRotted;
    }
}
