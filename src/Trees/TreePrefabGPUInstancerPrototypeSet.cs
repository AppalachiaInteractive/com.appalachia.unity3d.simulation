using System;
using GPUInstancer;

namespace Appalachia.Core.Trees
{
    [Serializable]
    public struct TreePrefabGPUInstancerPrototypeSet
    {
        public GPUInstancerPrototype normal;
        public GPUInstancerPrototype stump;
        public GPUInstancerPrototype stumpRotted;
        public GPUInstancerPrototype felled;
        public GPUInstancerPrototype felledBare;
        public GPUInstancerPrototype felledBareRotted;
        public GPUInstancerPrototype dead;
        public GPUInstancerPrototype deadFelled;
        public GPUInstancerPrototype deadFelledRotted;
    }
}
