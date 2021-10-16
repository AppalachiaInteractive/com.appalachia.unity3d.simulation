using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Addons
{
    public static class AddonGeometryGenerator
    {
        private static void MergeMeshGeometryIntoTree(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            ShapeData shape,
            TreePrefab prefab)
        {
            using (BUILD_TIME.GEO_GEN.MergeMeshGeometryIntoTree.Auto())
            {
                if ((prefab.LODCount <= 0) || !prefab.canMergeIntoTree)
                {
                    return;
                }

                var lod = prefab.GetLOD(output.lodLevel);

                lod.MergeIntoTree(output, inputMaterialCache, shape, shape.hierarchyID, shape.shapeID, 0f);
            }
        }
        
        public static void GenerateFruitGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            TreePrefabCollection prefabs,
            FruitShapeData shape,
            ShapeData parentShape,
            FruitHierarchyData hierarchy,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateFruitGeometry.Auto())
            {
                if (settings.showFruit)
                {
                    if (hierarchy.geometry.prefab == null)
                    {
                        return;
                    }

                    if (!prefabs.Contains(hierarchy.geometry.prefab, TreeComponentType.Fruit))
                    {
                        return;
                    }

                    if (weld)
                    {
                        shape.effectiveMatrix = ShapeWelder.WeldShapeOriginToParent(
                            output,
                            shape,
                            parentShape,
                            shape.effectiveMatrix,
                            weldRadius * 2f
                        );
                    }

                    var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Fruit);

                    MergeMeshGeometryIntoTree(output, inputMaterialCache, shape, prefab);

                    var position = GetAverageShapePosition(
                        output,
                        shape.geometry[output.lodLevel].modelVertexStart,
                        shape.geometry[output.lodLevel].modelVertexEnd
                    );

                    var vertex = prefab.ToVertex(shape, position, 0f);

                    output.fruits.Add(vertex);
                }
            }
        }

        public static void GenerateKnotGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            TreePrefabCollection prefabs,
            KnotShapeData shape,
            ShapeData parentShape,
            KnotHierarchyData hierarchy,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateKnotGeometry.Auto())
            {
                if (settings.showKnots)
                {
                    if (hierarchy.geometry.prefab == null)
                    {
                        return;
                    }

                    if (!prefabs.Contains(hierarchy.geometry.prefab, TreeComponentType.Knot))
                    {
                        return;
                    }

                    var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Knot);

                    if (weld) 
                    {
                        shape.effectiveMatrix = ShapeWelder.WeldShapeOriginToParent(
                            output,
                            shape,
                            parentShape,
                            shape.effectiveMatrix,
                            weldRadius * 2f
                        );
                    }

                    MergeMeshGeometryIntoTree(output, inputMaterialCache, shape, prefab);

                    var position = GetAverageShapePosition(
                        output,
                        shape.geometry[output.lodLevel].modelVertexStart,
                        shape.geometry[output.lodLevel].modelVertexEnd
                    );

                    var vertex = prefab.ToVertex(shape, position, 0f);

                    output.knots.Add(vertex);
                }
            }
        }

        public static void GenerateFungusGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            TreePrefabCollection prefabs,
            FungusShapeData shape,
            ShapeData parentShape,
            FungusHierarchyData hierarchy,
            bool weld,
            float weldRadius)
        {
            using (BUILD_TIME.GEO_GEN.GenerateFungusGeometry.Auto())
            {
                if (settings.showFungus)
                {
                    if (hierarchy.geometry.prefab == null)
                    {
                        return;
                    }

                    if (!prefabs.Contains(hierarchy.geometry.prefab, TreeComponentType.Fungus))
                    {
                        return;
                    }

                    var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Fungus);

                    if (weld)
                    {
                        shape.effectiveMatrix = ShapeWelder.WeldShapeOriginToParent(
                            output,
                            shape,
                            parentShape,
                            shape.effectiveMatrix,
                            weldRadius*2f
                        );
                    }

                    MergeMeshGeometryIntoTree(output, inputMaterialCache, shape, prefab);

                    var position = GetAverageShapePosition(
                        output,
                        shape.geometry[output.lodLevel].modelVertexStart,
                        shape.geometry[output.lodLevel].modelVertexEnd
                    );

                    var vertex = prefab.ToVertex(shape, position, 0f);

                    output.fungi.Add(vertex);
                }
            }
        }

        private static Vector3 GetAverageShapePosition(LODGenerationOutput output, int vertexStart, int vertexEnd)
        {
            var position = Vector3.zero;

            for (var i = vertexStart; i < vertexEnd; i++)
            {
                position += output.vertices[i].position;
            }

            position /= vertexEnd - vertexStart;

            return position;
        }
    }
}
