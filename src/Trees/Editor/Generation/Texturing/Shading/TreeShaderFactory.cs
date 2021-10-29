using System.Collections.Generic;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.InputShaders;
using Appalachia.Simulation.Trees.Generation.Texturing.Shading.OutputShaders;

namespace Appalachia.Simulation.Trees.Generation.Texturing.Shading
{
    
    public static class TreeShaderFactory
    {

        [ExecuteOnEnable]
        private static void Initialize()
        {
            _inputMaterialShaders = new List<IInputMaterialShader>();

            _inputMaterialShaders.Add(new Internal_Tree_InputMaterialShader());
            _inputMaterialShaders.Add(new Internal_Plant_InputMaterialShader());
            _inputMaterialShaders.Add(new Standard_InputMaterialShader());
            _inputMaterialShaders.Add(new StandardSpecular_InputMaterialShader());
            _inputMaterialShaders.Add(new BOXOPHOBIC_AdvancedDynamicShaders_InputMaterialShader());
            _inputMaterialShaders.Add(new CTI_301_InputMaterialShader());
            _inputMaterialShaders.Add(new BestEffort_InputMaterialShader());

            
            _outputMaterialShaders = new Dictionary<string, IOutputMaterialShader>();
            
            var standard = new Standard_OutputMaterialShader();
            var spec = new StandardSpecular_OutputMaterialShader();
            var internalBark_LOD0 = new Internal_Bark_LOD0_OutputMaterialShader();
            //var internalBark_LOD1 = new Internal_Bark_LOD1_OutputMaterialShader();
            var internalLeaf_LOD0 = new Internal_Leaf_LOD0_OutputMaterialShader();
            //var internalLeaf_LOD1 = new Internal_Leaf_LOD1_OutputMaterialShader();
            var internalShadow_LOD0 = new Internal_TreeShadows_LOD0_OutputMaterialShader();
            //var internalShadow_LOD1 = new Internal_TreeShadows_LOD1_OutputMaterialShader();

            _outputMaterialShaders.Add(standard.Name, standard);
            _outputMaterialShaders.Add(spec.Name, spec);
            _outputMaterialShaders.Add(internalBark_LOD0.Name, internalBark_LOD0);
            //_outputMaterialShaders.Add(internalBark_LOD1.Name, internalBark_LOD1);
            _outputMaterialShaders.Add(internalLeaf_LOD0.Name, internalLeaf_LOD0);
            //_outputMaterialShaders.Add(internalLeaf_LOD1.Name, internalLeaf_LOD1);
            _outputMaterialShaders.Add(internalShadow_LOD0.Name, internalShadow_LOD0);
            //_outputMaterialShaders.Add(internalShadow_LOD1.Name, internalShadow_LOD1);
        }

        private static List<IInputMaterialShader> _inputMaterialShaders;
        private static Dictionary<string, IOutputMaterialShader> _outputMaterialShaders;

        public static void RegisterInputMaterialShader(IInputMaterialShader s)
        {
            if (_inputMaterialShaders == null)
            {
                _inputMaterialShaders = new List<IInputMaterialShader>();
            }

            _inputMaterialShaders.Add(s);
            _inputMaterialShaders.Sort(
                (shader, other) => shader.Priority.CompareTo(other.Priority)
            );
        }
        
        public static void RegisterOutputMaterialShader(IOutputMaterialShader s)
        {
            if (_inputMaterialShaders == null)
            {
                _outputMaterialShaders = new  Dictionary<string, IOutputMaterialShader>();
            }

            _outputMaterialShaders.Add(s.Name, s);
        }
        
        public static IReadOnlyList<IInputMaterialShader> GetInputMaterialShaders()
        {
            return _inputMaterialShaders;
        }
        public static IReadOnlyCollection<IOutputMaterialShader> GetOutputMaterialShaders()
        {
            return _outputMaterialShaders.Values;
        }

        public static bool ContainsName(string name)
        {
            return _outputMaterialShaders.ContainsKey(name);
        }
        
        public static IOutputMaterialShader GetByName(string name)
        {
            if (!_outputMaterialShaders.ContainsKey(name))
            {
                throw new KeyNotFoundException($"The given key [{name}] was not found in the dictionary.");
            }
            return _outputMaterialShaders[name];
        }
    }
}
