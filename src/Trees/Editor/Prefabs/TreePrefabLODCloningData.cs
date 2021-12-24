using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Core.Objects.Root;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedParameter.Local

namespace Appalachia.Simulation.Trees.Prefabs
{
    [Serializable]
    public class TreePrefabLODCloningData : AppalachiaSimpleBase
    {
        public List<TreePrefabLODRendererCloningData> lodCloningData;
        public TreeComponentType type;

        public IEnumerable<Object> GetExternalObjects()
        {
            return lodCloningData.SelectMany(cd => cd.GetExternalObjects());
        }

        public IEnumerable<Material> GetMaterials()
        {
            return lodCloningData.SelectMany(cd => cd.GetMaterials());
        }

        public bool CanMergeIntoTree()
        {
            return lodCloningData.All(cd => cd.CanMergeIntoTree());
        }

        public void MergeIntoTree(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            ShapeData shape,
            int hierarchyID,
            int shapeID,
            float heightOffset)
        {
            foreach (var cloneSet in lodCloningData)
            {
                var vertexOffset = output.vertices.Count;

                cloneSet.MergeVertices(
                    output,
                    shape,
                    hierarchyID,
                    shapeID,
                    heightOffset
                );
                cloneSet.MergeTriangles(output, shape, inputMaterialCache, vertexOffset);
            }
        }

        public TreePrefabLODCloningData(
            Renderer[] renderers,
            TreeComponentType type,
            float fadeTransitionWidth = .05f,
            float screenRelativeTransitionHeight = .01f)
        {
            lodCloningData = new List<TreePrefabLODRendererCloningData>();
            this.type = type;

            for (var i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];

                lodCloningData.Add(new TreePrefabLODRendererCloningData(renderer, type));
            }
        }
    }
}