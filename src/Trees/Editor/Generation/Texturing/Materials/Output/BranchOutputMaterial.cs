using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Base;
using Appalachia.Simulation.Trees.Generation.Texturing.Operations;
using Appalachia.Simulation.Trees.Generation.Texturing.Output;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output
{
    [Serializable]
    public class BranchOutputMaterial : OutputMaterial
    {
        
        public BranchOutputMaterial(int materialID) : base(materialID, ResponsiveSettingsType.Branch)
        {
        }

        public override MaterialContext MaterialContext => MaterialContext.BranchOutputMaterial;


        protected override OutputShaderSelectionSet defaultShaders => DefaultShaderResource.instance.branchShaderSet;

        public void RebuildTextureSets()
        {
            var ots = textureSet;

            if (ots.outputTextures.Count > 0)
            {
                return;
            }
            
            var profiles = GetOutputTextureProfiles(true).ToArray();

            var outputTextures = new List<OutputTexture>();

            var outputMaterial = materials[0];
            
            var recreatedSet = TextureExtractor.GetInputTextureSet(outputMaterial.asset);
            
            for (var i = 0; i < profiles.Length; i++)
            {
                var profile = profiles[i];

                var match = recreatedSet.inputTextures.FirstOrDefault(it => it.profile.map == profile.map);

                if (match != null)
                {
                    var newot = new OutputTexture(profile, match.texture);
                    outputTextures.Add(newot);
                }
            }

            textureSet.Set(outputTextures);
        }
    }
}
