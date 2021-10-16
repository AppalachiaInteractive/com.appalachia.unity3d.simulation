using AmplifyImpostors;
using Appalachia.Core.Scriptables;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    public class DefaultShaderResource : SelfSavingSingletonScriptableObject<DefaultShaderResource>
    {
        public OutputShaderSelectionSet branchShaderSet;
        public OutputShaderSelectionSet tiledShaderSet;
        public OutputShaderSelectionSet atlasShaderSet;
        public OutputShaderSelectionSet shadowShaderSet;
        public Shader logShader;
        public AmplifyImpostorBakePreset impostorPreset;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            for (var i = branchShaderSet.Count - 1; i >= 0; i--)
            {
                if (branchShaderSet[i] == null)
                {
                    branchShaderSet.RemoveAt(i);
                }
            }
        
            for (var i = tiledShaderSet.Count - 1; i >= 0; i--)
            {
                if (tiledShaderSet[i] == null)
                {
                    tiledShaderSet.RemoveAt(i);
                }
            }
        
            for (var i = atlasShaderSet.Count - 1; i >= 0; i--)
            {
                if (atlasShaderSet[i] == null)
                {
                    atlasShaderSet.RemoveAt(i);
                }
            }
        
            for (var i = shadowShaderSet.Count - 1; i >= 0; i--)
            {
                if (shadowShaderSet[i] == null)
                {
                    shadowShaderSet.RemoveAt(i);
                }
            }
        }
    }
}
