using Appalachia.Core.Scriptables;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    public class DefaultMaterialResource : SelfSavingSingletonScriptableObject<DefaultMaterialResource>
    {
        public Material material;
    }
}
