using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Definition;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Output;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Settings.Log;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Spatial.ConvexDecomposition;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Collisions
{
    public static class ColliderGenerator
    {
        public static void GenerateCollider(
            StageType stage,
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            InputMaterialCache inputMaterialCache,
            TreePrefabCollection prefabs,
            TreeSettings settings,
            Dictionary<int, ShapeData> shapeLookup,
            CollisionMeshCollection meshes)
        {

            using (BUILD_TIME.COLL_GEN.GenerateCollider.Auto())
            {
                var collisionSettings = settings.collision;

                var output = new LODGenerationOutput(settings.lod.levelsOfDetail.Count);
                
                AssetDatabaseManager.SaveAssets();

                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.shape.geometry.Count == settings.lod.levelsOfDetail.Count)
                        {
                            data.shape.geometry.Add(new ShapeGeometryData());
                        }
                        if (data.shape is BarkShapeData bsd)
                        {
                            if (bsd.branchRings == null)
                            {
                                bsd.branchRings = new List<List<BranchRing>>();
                            }
                            
                            while (bsd.branchRings.Count <= output.lodLevel)
                            {
                                bsd.branchRings.Add(new List<BranchRing>());
                            }
                        }

                        data.shape.geometry[settings.lod.levelsOfDetail.Count].Reset();

                        data.hierarchy.seed.Reset();

                        switch (data.type)
                        {
                            case TreeComponentType.Root:
                            case TreeComponentType.Trunk:
                            case TreeComponentType.Branch:
                            {
                                var barkHierarchyData = data.hierarchy as BarkHierarchyData;
                                var barkShapeData = data.shape as BarkShapeData;
                                var parentShapeData = data.parentShape as BarkShapeData;

                                var rings = barkShapeData.branchRings[output.lodLevel];
                                rings.Clear();

                                SplineGeometryGenerator.GenerateSplineGeometry(
                                    hierarchies,
                                    shapes,
                                    output,
                                    inputMaterialCache,
                                    collisionSettings.quality,
                                    settings?.variants,
                                    barkShapeData,
                                    parentShapeData,
                                    barkHierarchyData,
                                    false,
                                    1.0f
                                );

                                break;
                            }
                        }
                    }
                );

                var visibilityLookup = new Dictionary<int, bool>();

                shapes.RecurseShapes(
                    hierarchies,
                    data => { visibilityLookup.Add(data.shape.shapeID, data.shape.forcedInvisible); }
                );


                var meshShapes = 0;

                var maxTrunk = 0f;

                foreach (var vertex in output.vertices)
                {
                    if (vertex.type == TreeComponentType.Trunk)
                    {
                        maxTrunk = Mathf.Max(vertex.position.y, maxTrunk);
                    }
                }
                    
                maxTrunk -= hierarchies.GetVerticalOffset();
                    
                var cutoffHeight = maxTrunk * settings.wind.trunkWindDeadZonePerMeter;

                cutoffHeight = Mathf.Clamp(
                    cutoffHeight,
                    settings.wind.trunkWindDeadZoneVertical.x,
                    settings.wind.trunkWindDeadZoneVertical.y
                );
                
                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.hierarchy.HasSpline)
                        {
                            if ((data.shape.effectiveLength < 0.5f) || (data.shape.effectiveSize < .02f))
                            {                                
                                data.shape.forcedInvisible = true;
                            }

                            var root = data.shape.effectiveMatrix.MultiplyPoint(Vector3.zero);

                            if (stage.IsFelled())
                            {
                                
                            }
                            else if (stage.IsStump() && (data.type == TreeComponentType.Branch))
                            {
                                data.shape.forcedInvisible = true;
                            }
                            else
                            {
                                if ((stage == StageType.Normal) && (data.type == TreeComponentType.Branch))
                                {
                                    data.shape.forcedInvisible = true;
                                }
                                
                                if (root.y > cutoffHeight)
                                {
                                    data.shape.forcedInvisible = true;
                                }
                            }

                            if (!data.shape.forcedInvisible)
                            {
                                var bh = data.hierarchy as BarkHierarchyData;

                                if (bh.geometry.colliderMultiplier == 0)
                                {
                                    bh.geometry.colliderMultiplier.defaultValue = 1;
                                }
                                
                                meshShapes += bh.geometry.colliderMultiplier;
                            
                                if (data.shape.effectiveLength > 10)
                                {
                                    meshShapes += bh.geometry.colliderMultiplier;
                                }
                            }
                        }
                        else
                        {
                            data.shape.forcedInvisible = true;
                        }
                    }
                );

                /*foreach (var shape in shapes.GetShapes(TreeComponentType.Trunk))
                {
                    if (shape is TrunkShapeData tsd)
                    {
                        foreach (var lod in tsd.branchRings)
                        {
                            var baseRing = lod[0];
                            
                            
                        }
                    }
                }*/

                var bounds = new Bounds {size = Vector3.one * 1024};
                var min = bounds.min;
                var max = bounds.max;
                min.y = collisionSettings.minimumHeight;
                max.y = cutoffHeight;

                bounds.min = min;
                bounds.max = max;

                if (stage.IsFelled())
                { 
                    bounds = default;                    
                }
                
                MeshGenerator.GenerateMeshes(
                    output,
                    shapeLookup,
                    null,
                    new List<OutputMaterial>(),
                    settings.collision.properties,
                    meshes.collisionMesh,
                    vd => Color.black,
                    true,
                    true,
                    bounds);

                meshes.decompositionMeshes =
                    ConvexMeshColliderGenerator.GenerateCollisionMesh(meshes.collisionMesh, meshShapes);
                
                shapes.RecurseShapes(
                    hierarchies,
                    data => { data.shape.forcedInvisible = visibilityLookup[data.shape.shapeID]; }
                );
            }
        }
        
        
        public static void GenerateLogColliders(
            IHierarchyRead hierarchies,
            IShapeRead shapes,
            LogInstance instance,
            LogSettings settings,
            Dictionary<int, ShapeData> shapeLookup,
            CollisionMeshCollection meshes)
        {

            using (BUILD_TIME.COLL_GEN.GenerateLogColliders.Auto())
            {
                var collisionSettings = settings.collision;

                var output = new LODGenerationOutput(settings.lod.levelsOfDetail.Count);

                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.shape.geometry.Count == settings.lod.levelsOfDetail.Count)
                        {
                            data.shape.geometry.Add(new ShapeGeometryData());
                        }
                        
                        if (data.shape is BarkShapeData bsd)
                        {
                            if (bsd.branchRings == null)
                            {
                                bsd.branchRings = new List<List<BranchRing>>();
                            }
                            
                            while (bsd.branchRings.Count <= output.lodLevel)
                            {
                                bsd.branchRings.Add(new List<BranchRing>());
                            }
                        }
                        
                        data.shape.geometry[settings.lod.levelsOfDetail.Count].Reset();

                        data.hierarchy.seed.Reset();

                        switch (data.type)
                        {
                            case TreeComponentType.Trunk:
                            case TreeComponentType.Branch:
                            {
                                var barkHierarchyData = data.hierarchy as BarkHierarchyData;
                                var barkShapeData = data.shape as BarkShapeData;
                                var parentShapeData = data.parentShape as BarkShapeData;

                                var rings = barkShapeData.branchRings[output.lodLevel];
                                rings.Clear();

                                var weldRadius = 1f;

                                if (data.type != TreeComponentType.Trunk)
                                {
                                    var parentHierarchyData = data.parentHierarchy as BarkHierarchyData;
                                    weldRadius = SplineModeler.GetRadiusAtTime(
                                        parentShapeData,
                                        parentHierarchyData,
                                        data.shape.offset
                                    );

                                    weldRadius = Mathf.Max(1, weldRadius);
                                }

                                SplineGeometryGenerator.GenerateSplineGeometry(
                                    hierarchies,
                                    shapes,
                                    output,
                                    collisionSettings.quality,
                                    barkShapeData,
                                    parentShapeData,
                                    barkHierarchyData,
                                    true,
                                    weldRadius
                                );

                                break;
                            }
                        }
                    }
                );

                var visibilityLookup = new Dictionary<int, bool>();

                shapes.RecurseShapes(
                    hierarchies,
                    data => { visibilityLookup.Add(data.shape.shapeID, data.shape.forcedInvisible); }
                );
                
                var meshShapes = 0;
                
                shapes.RecurseShapes(
                    hierarchies,
                    data =>
                    {
                        if (data.hierarchy.HasSpline)
                        {
                            if (!data.shape.forcedInvisible)
                            {
                                var bh = data.hierarchy as BarkHierarchyData;

                                if (bh.geometry.colliderMultiplier == 0)
                                {
                                    bh.geometry.colliderMultiplier.defaultValue = 1;
                                }
                                
                                meshShapes += bh.geometry.colliderMultiplier;
                            }
                        }
                        else
                        {
                            data.shape.forcedInvisible = true;
                        }
                    }
                );


                var bounds = output.GetBounds();
                
                if (instance.constrainLength)
                {
                    for (var index = 0; index < output.vertices.Count; index++)
                    {
                        var vertex = output.vertices[index];
                        vertex.position -= bounds.center;
                        vertex.position *= instance.effectiveScale;
                    }
                }

                bounds = output.GetBounds();
                    
                for (var index = 0; index < output.vertices.Count; index++)
                {
                    var vertex = output.vertices[index];
                    vertex.position.x -= bounds.center.x;
                    vertex.position.y -= bounds.min.y;
                    vertex.position.z -= bounds.center.z;
                }

                MeshGenerator.GenerateMeshes(
                    output,
                    shapeLookup,
                    null,
                    null,
                    settings.collision.properties,
                    meshes.collisionMesh,
                    vd => Color.black,
                    true,
                    false,
                    bounds
                );

                meshes.collisionMesh.uv = null;
                meshes.collisionMesh.uv2 = null;
                meshes.collisionMesh.uv3 = null;
                meshes.collisionMesh.uv4 = null;
                meshes.collisionMesh.uv5 = null;

                meshes.decompositionMeshes =
                    ConvexMeshColliderGenerator.GenerateCollisionMesh(meshes.collisionMesh, meshShapes);

                
                for (var i = 0; i < meshes.decompositionMeshes.Count; i++)
                {
                    var mesh = meshes.decompositionMeshes[i];

                    var vertices = mesh.vertices;

                    for (var j = 0; j < vertices.Length; j++)
                    {
                        vertices[j] = vertices[j] + (instance.colliderInflation * mesh.normals[j]);
                    }

                    mesh.vertices = vertices;
                }
                
                shapes.RecurseShapes(
                    hierarchies,
                    data => { data.shape.forcedInvisible = visibilityLookup[data.shape.shapeID]; }
                );
            }
        }
    }
}
