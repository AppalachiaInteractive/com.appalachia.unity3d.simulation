using System;
using System.Collections.Generic;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders
{
    [Serializable]
    public sealed class OutputShaderSelectionSet
    {
        public List<OutputShaderSelection> shaders;

        public int Count => shaders?.Count ?? 0;

        public OutputShaderSelectionSet(params IOutputMaterialShader[] args)
        {
            shaders = new List<OutputShaderSelection>();

            foreach (var arg in args)
            {
                var selection = new OutputShaderSelection(arg);
                shaders.Add(selection);
            }
        }

        public OutputShaderSelectionSet Clone()
        {
            var clone = new OutputShaderSelectionSet();

            foreach (var shader in shaders)
            {
                var cloneShader = new OutputShaderSelection(shader.materialShader);
                clone.shaders.Add(cloneShader);
            }
            
            return clone;
        }

        public OutputShaderSelection this[int i]
        {
            get { return shaders[i]; }
        }

        public void RemoveAt(int i)
        {
            shaders.RemoveAt(i);
        }
    }
}
