using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Geometry;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Core.Shape;
using Appalachia.Simulation.Trees.Generation.Geometry.Splines;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Texturing.Geometry;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Rects;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Prefabs;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Geometry.Leaves
{
    [CallStaticConstructorInEditor]
    public static class LeafGeometryGenerator
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static LeafGeometryGenerator()
        {
            LeafUVRectCollection.InstanceAvailable += i => _leafUVRectCollection = i;
        }

        private static LeafUVRectCollection _leafUVRectCollection;
        private static readonly TreeVertex[] _planeVerts = new TreeVertex[8];
        private static readonly TreeVertex[] _planeVerts2 = new TreeVertex[8];
        private static BillboardPlane _billboardPlane;
        private static readonly TreeTriangle[] _triangles = new TreeTriangle[4];

        
/*
*     DIAMOND WIDTH CUT     DIAMOND LENGTH CUT          PLANE             DIAMOND 1          DIAMOND 2    
*                          
*       0-4 : VERTEX INDEX                                  
*         . : FACE 0      ; : FACE 2                      
*         + : FACE 1      ! : FACE 3                      
*                          
*           -                      -                       -                  -                  -       
*                           
*           1                      0                   0-------3              1              1-------4   
*          /.\                    /|\                  |\++++++|             /|\             |\!!!!!/|   
*         /...\                  /.|+\                 |.\+++++|            /.|!\            |.\!!!/;|   
*        /.....\                /..|++\                |..\++++|           /..|!!\           |..\!/;|   
*     - 0-------2 +          - 1...|+++2 +           - |...\+++| +      - 2---0---4 +      - |...0;;;| + 
*        \+++++/                \..|++/                |....\++|           \++|;;/           |../+\;;|
*         \+++/                  \.|+/                 |.....\+|            \+|;/            |./+++\;|   
*          \+/                    \|/                  |......\|             \|/             |/++++.\|   
*           3                      3                   1-------2              3              2-------3   
*                          
*           +                      +                       +                  +                  +
*/
        public static void GenerateLeafGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings lodSettings,
            MeshSettings meshSettings,
            TreePrefabCollection prefabs,
            LeafShapeData shape,
            BarkShapeData parentShape,
            LeafHierarchyData hierarchy,
            ISeed seed,
            bool weld)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafGeometry.Auto())
            {
                if (weld && (shape.matrix == shape.effectiveMatrix))
                {
                    shape.effectiveMatrix = ShapeWelder.WeldLeafShapeOriginToParent(output, parentShape, shape.effectiveMatrix);
                    
                    shape.welded = true;
                }
                
                var missChance = 1 - Mathf.Clamp01(lodSettings.leafFullness);

                if (shape.variationSeed < missChance)
                {
                    return;
                }

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.Mesh)
                {
                    if (hierarchy.geometry.prefab == null)
                    {
                        return;
                    }

                    if (!prefabs.Contains(hierarchy.geometry.prefab, TreeComponentType.Leaf))
                    {
                        return;
                    }

                    var prefab = prefabs.GetPrefab(hierarchy.geometry.prefab, TreeComponentType.Leaf);
                    MergeMeshGeometryIntoTree(output, inputMaterialCache, shape, prefab);
                }
                else if (hierarchy.geometry.geometryMode == LeafGeometryMode.Billboard)
                {
                    GenerateLeafBillboardGeometry(output, inputMaterialCache, meshSettings, shape, hierarchy, seed);
                }
                else if ((hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondLengthCut) ||
                    (hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondWidthCut))
                {
                    GenerateLeafDiamondGeometry(
                        output,
                        inputMaterialCache,
                        lodSettings,
                        meshSettings,
                        shape,
                        hierarchy
                    );
                }
                else if ((hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondPyramid) ||
                    (hierarchy.geometry.geometryMode == LeafGeometryMode.BoxPyramid))
                {
                    GenerateLeafPyramidGeometry(
                        output,
                        inputMaterialCache,
                        lodSettings,
                        meshSettings,
                        shape,
                        hierarchy
                    );
                }
                else if ((hierarchy.geometry.geometryMode == LeafGeometryMode.Spoke) ||
                    (hierarchy.geometry.geometryMode == LeafGeometryMode.BentSpoke))
                {
                    GenerateLeafSpokeGeometry(
                        output, 
                        inputMaterialCache,
                        lodSettings,
                        meshSettings,
                        shape,
                        hierarchy);
                }
                else
                {
                    GenerateLeafPlaneGeometry(output, inputMaterialCache, lodSettings, meshSettings, shape, hierarchy);
                }
            }
        }

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

        private static void GenerateLeafBillboardGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            MeshSettings meshSettings,
            LeafShapeData shape,
            LeafHierarchyData hierarchy,
            ISeed seed)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafBillboardGeometry.Auto())
            {
                var m = inputMaterialCache.GetInputMaterialData(
                    hierarchy.geometry.leafMaterial,
                    TreeMaterialUsage.LeafPlane
                );

                var materialID = m == null ? -1 : m.materialID;

                var normal = new Vector3(
                    meshSettings.generatedBillboardNormalFactor,
                    1.0f - meshSettings.generatedBillboardNormalFactor,
                    meshSettings.generatedBillboardNormalFactor
                );
                
                // dumb
                var flip = hierarchy.geometry.flipLeafNormals ? 1 : -1;

                normal.y *= flip;

                _planeVerts[0] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[1] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[2] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[3] = new TreeVertex() /*TreeVertex.Get()*/;

                _planeVerts[0].Set(shape, 0f);
                _planeVerts[1].Set(shape, 0f);
                _planeVerts[2].Set(shape, 0f);
                _planeVerts[3].Set(shape, 0f);

                var uvs = new[]
                {
                    new Vector2(0.0f, 1.0f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(1.0f, 1.0f)
                };

                var offset = seed.RandomValue(0, 4);

                _planeVerts[0].raw_uv0 = uvs[(0 + offset) % 4];
                _planeVerts[1].raw_uv0 = uvs[(1 + offset) % 4];
                _planeVerts[2].raw_uv0 = uvs[(2 + offset) % 4];
                _planeVerts[3].raw_uv0 = uvs[(3 + offset) % 4];

                var scale = shape.effectiveScale;

                _planeVerts[0].billboardData = new Vector4(-scale, scale, 0f, -2f);
                _planeVerts[1].billboardData = new Vector4(-scale, -scale, 0f, -2f);
                _planeVerts[2].billboardData = new Vector4(scale, -scale, 0f, -2f);
                _planeVerts[3].billboardData = new Vector4(scale, scale, 0f, -2f);

                for (var i = 0; i < 4; ++i)
                {
                    _planeVerts[i].position = shape.effectiveMatrix.MultiplyPoint(Vector3.zero);
                    _planeVerts[i].normal = Vector3.zero;
                    _planeVerts[i].tangent = Vector3.zero;
                }

                _planeVerts[0].rawWind.tertiaryRoll = 0.60f * shape.effectiveScale;
                _planeVerts[1].rawWind.tertiaryRoll = 0.45f * shape.effectiveScale;
                _planeVerts[2].rawWind.tertiaryRoll = 0.30f * shape.effectiveScale;
                _planeVerts[3].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;

                _planeVerts[0].heightOffset = 0.5f;
                _planeVerts[1].heightOffset = 1.0f;
                _planeVerts[2].heightOffset = 0.5f;
                _planeVerts[3].heightOffset = 0.0f;

                var index0 = output.vertices.Count;

                _billboardPlane.Reset();
                
                for (var i = 0; i < 4; ++i)
                {
                    _planeVerts[i].billboard = true;
                    output.AddVertex(_planeVerts[i]);
                    _billboardPlane[i] = _planeVerts[i];
                }
                
                output.billboards.Add(_billboardPlane);

                var t1 = _triangles[0];
                var t2 = _triangles[1];

                t1.Reset();
                t2.Reset();

                t1.Set(shape, materialID, index0, index0 + 2, index0 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                t2.Set(shape, materialID, index0, index0 + 3, index0 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                output.AddTriangle(t1);
                output.AddTriangle(t2);
            }
        }

        private static void GenerateLeafPlaneGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            MeshSettings meshSettings,
            LeafShapeData shape,
            LeafHierarchyData hierarchy)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafPlaneGeometry.Auto())
            {
                var m = inputMaterialCache.GetInputMaterialData(
                    hierarchy.geometry.leafMaterial,
                    TreeMaterialUsage.LeafPlane
                );
                
                var materialID = m == null ? -1 : m.materialID;

                var planes = 0;
                switch (hierarchy.geometry.geometryMode)
                {
                    case LeafGeometryMode.Plane:
                    case LeafGeometryMode.BentPlane:
                        planes = 1;
                        break;
                    case LeafGeometryMode.Cross:
                    case LeafGeometryMode.BentCross:
                        planes = 2;
                        break;
                    case LeafGeometryMode.TriCross:
                    case LeafGeometryMode.BentTriCross:
                        planes = 3;
                        break;
                }

                TextureHullData hullData;

                using (BUILD_TIME.GEO_GEN.GetTextureHullData.Auto())
                {
                    if (materialID >= 0)
                    {
                        hullData = hierarchy.geometry.leafMaterial.GetTextureHullData();
                    }
                    else
                    {
                        hullData = TextureHullData.Default();
                    }
                }

                var rects = _leafUVRectCollection.Get(hierarchy.geometry.leafMaterial);

                float xRatio = hierarchy.geometry.xRatio;

                if (xRatio == 0.0f)
                {
                    hierarchy.geometry.xRatio.SetValue(1.0f);
                    xRatio = 1.0f;
                }
                
                float xOffset = 0.0f;
                float zOffset = 0.0f;
                
                if (((rects == null) || (rects.Count < 2)) && hierarchy.geometry.correctOffset)
                {
                    var xCorrectionNecessary = .5f - hullData.baseWidthCenter0To1;
                    var zCorrectionNecessary = -1 * hullData.baseHeightOffset0To1;

                    xOffset = xCorrectionNecessary * (shape.effectiveScale * 2);
                    zOffset = zCorrectionNecessary * (shape.effectiveScale * 2);
                }
                
                xOffset += shape.effectiveScale * hierarchy.geometry.xOffset;
                zOffset += shape.effectiveScale * hierarchy.geometry.zOffset;
                
                var offset = new Vector3(xOffset, 0f, zOffset);

                var yOffset = 0f;

                if ((hierarchy.geometry.geometryMode == LeafGeometryMode.BentPlane) ||
                    (hierarchy.geometry.geometryMode == LeafGeometryMode.BentCross) ||
                    (hierarchy.geometry.geometryMode == LeafGeometryMode.BentTriCross))
                {
                    yOffset = hierarchy.geometry.bendFactor * shape.effectiveScale;
                }

                var xPush = shape.effectiveScale*xRatio;
                var zPush = shape.effectiveScale;
                
                var positionsRaw = new[]
                {
                    offset + new Vector3(-xPush, 0f, -zPush),
                    offset + new Vector3(-xPush, 0f, zPush),
                    offset + new Vector3(xPush, 0f, zPush),
                    offset + new Vector3(xPush, yOffset, -zPush)
                };

                var normal = new Vector3(
                    meshSettings.generatedPlaneNormalFactor,
                    1.0f - meshSettings.generatedPlaneNormalFactor,
                    meshSettings.generatedPlaneNormalFactor
                );

                //dumb
                var flip = hierarchy.geometry.flipLeafNormals ? 1 : -1;
                normal.y *= flip;

                var normalsRaw = new[]
                {
                    new Vector3(-normal.x, normal.y, -normal.z).normalized,
                    new Vector3(-normal.x, normal.y, 0).normalized, // note z always 0
                    new Vector3(normal.x, normal.y, 0).normalized, // note z always 0
                    new Vector3(normal.x, normal.y, -normal.z).normalized
                };

                for (var planeIndex = 0; planeIndex < planes; planeIndex++)
                {
                    if ((planeIndex == 1) && (settings.leafGeometryQuality <= .5f))
                    {
                        continue;
                    }

                    if ((planeIndex == 2) & (settings.leafGeometryQuality <= .33f))
                    {
                        continue;
                    }

                    var planeRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    switch (planeIndex)
                    {
                        case 1:
                            planeRotation = Quaternion.Euler(new Vector3(90, 90, 0));
                            break;
                        case 2:
                            planeRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                            break;
                    }

                    _planeVerts[0] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[1] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[2] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[3] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[4] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[5] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[6] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts[7] = new TreeVertex() /*TreeVertex.Get()*/;

                    _planeVerts[0].Set(shape, 0f);
                    _planeVerts[1].Set(shape, 0f);
                    _planeVerts[2].Set(shape, 0f);
                    _planeVerts[3].Set(shape, 0f);
                    _planeVerts[4].Set(shape, 0f);
                    _planeVerts[5].Set(shape, 0f);
                    _planeVerts[6].Set(shape, 0f);
                    _planeVerts[7].Set(shape, 0f);

                    for (var i = 0; i < 4; ++i)
                    {
                        _planeVerts[i].position = shape.effectiveMatrix.MultiplyPoint(planeRotation * positionsRaw[i]);
                        _planeVerts[i].normal = shape.effectiveMatrix.MultiplyVector(planeRotation * normalsRaw[i]);
                        _planeVerts[i].tangent = TangentGenerator.CreateTangent(shape, planeRotation, _planeVerts[i].normal);
                        _planeVerts[i].raw_uv0 = hullData.textureHull[i];
                    }

                    _planeVerts[0].heightOffset = 1f;
                    _planeVerts[1].heightOffset = 1f;
                    _planeVerts[2].heightOffset = 0f;
                    _planeVerts[3].heightOffset = 0f;

                    _planeVerts[0].rawWind.tertiaryRoll = 0.5f;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.4f;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.0f;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.0f;

                    for (var i = 0; i < 4; i++)
                    {
                        _planeVerts[i + 4].LerpVerticesByFactor(_planeVerts, hullData.textureHull[i]);

                        _planeVerts[i + 4].raw_uv0 = _planeVerts[i].raw_uv0;
                        _planeVerts[i + 4].heightOffset = _planeVerts[i].heightOffset;
                        _planeVerts[i + 4].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                        _planeVerts[i + 4].uvScaleUpdated = _planeVerts[i].uvScaleUpdated;
                    }

                    var index0 = output.vertices.Count;

                    for (var i = 0; i < 4;  i++)
                    {
                        output.AddVertex(_planeVerts[i + 4]);
                    }

                    var t1 = _triangles[0];
                    var t2 = _triangles[1];

                    t1.Reset();
                    t2.Reset();

                    t1.Set(shape, materialID, index0, index0 + 1, index0 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                    t2.Set(shape, materialID, index0, index0 + 2, index0 + 3, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                    output.AddTriangle(t1);
                    output.AddTriangle(t2);

                    var faceNormal = shape.effectiveMatrix.MultiplyVector(planeRotation * Vector3.up);

                    if (settings.doubleSidedLeafGeometry)
                    {
                        _planeVerts2[0] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[1] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[2] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[3] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[4] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[5] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[6] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[7] = new TreeVertex() /*TreeVertex.Get()*/;

                        _planeVerts2[0].Set(shape, 0f);
                        _planeVerts2[1].Set(shape, 0f);
                        _planeVerts2[2].Set(shape, 0f);
                        _planeVerts2[3].Set(shape, 0f);
                        _planeVerts2[4].Set(shape, 0f);
                        _planeVerts2[5].Set(shape, 0f);
                        _planeVerts2[6].Set(shape, 0f);
                        _planeVerts2[7].Set(shape, 0f);

                        for (var i = 0; i < 4; ++i)
                        {
                            _planeVerts2[i].position = _planeVerts[i].position;
                            _planeVerts2[i].normal = Vector3.Reflect(_planeVerts[i].normal, faceNormal);
                            _planeVerts2[i].tangent = Vector3.Reflect(_planeVerts[i].tangent, faceNormal);
                            _planeVerts2[i].tangent.w = -1;
                            _planeVerts2[i].raw_uv0 = _planeVerts[i].raw_uv0;
                            _planeVerts2[i].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                            _planeVerts2[i].heightOffset = _planeVerts[i].heightOffset;
                        }

                        for (var i = 0; i < 4; ++i)
                        {
                            _planeVerts2[i + 4].LerpVerticesByFactor(_planeVerts2, hullData.textureHull[i]);
                            _planeVerts2[i + 4].raw_uv0 = _planeVerts2[i].raw_uv0;
                            _planeVerts2[i + 4].uvScaleUpdated = _planeVerts2[i].uvScaleUpdated;
                            _planeVerts2[i + 4].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                            _planeVerts2[i + 4].heightOffset = _planeVerts[i].heightOffset;
                        }

                        var index4 = output.vertices.Count;
                        for (var i = 0; i < 4; ++i)
                        {
                            output.AddVertex(_planeVerts2[i + 4]);
                        }
                        
                        t1.Reset();
                        t2.Reset();

                        t1.Set(shape, materialID, index4, index4 + 2, index4 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals); 
                        t2.Set(shape, materialID, index4, index4 + 3, index4 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                        output.AddTriangle(t1);
                        output.AddTriangle(t2);
                    }
                }
            }
        }
        
        private static void GenerateLeafSpokeGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            MeshSettings meshSettings,
            LeafShapeData shape,
            LeafHierarchyData hierarchy)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafPlaneGeometry.Auto())
            {
                var m = inputMaterialCache.GetInputMaterialData(
                    hierarchy.geometry.leafMaterial,
                    TreeMaterialUsage.LeafPlane
                );
                var materialID = m == null ? -1 : m.materialID;

                if ((hierarchy.geometry.spokeCount == null) || (hierarchy.geometry.spokeCount == 0))
                {
                    hierarchy.geometry.spokeCount = TreeProperty.New(3);
                    hierarchy.geometry.spokeDropPerMeter = TreeProperty.New(.05f);
                }

                var height = shape.effectiveMatrix.MultiplyPoint(Vector3.zero).y;
                var spokeDrop = hierarchy.geometry.spokeDropPerMeter * height;
                var planeCount = Mathf.Max(1, Mathf.RoundToInt(hierarchy.geometry.spokeCount * spokeDrop));
                
                TextureHullData hullData;

                using (BUILD_TIME.GEO_GEN.GetTextureHullData.Auto())
                {
                    if (materialID >= 0)
                    {
                        hullData = hierarchy.geometry.leafMaterial.GetTextureHullData();
                    }
                    else
                    {
                        hullData = TextureHullData.Default();
                    }
                }

                var rects = _leafUVRectCollection.Get(hierarchy.geometry.leafMaterial);

                float xRatio = hierarchy.geometry.xRatio;

                if (xRatio == 0.0f)
                {
                    hierarchy.geometry.xRatio.SetValue(1.0f);
                    xRatio = 1.0f;
                }
                
                float xOffset = 0.0f;
                float zOffset = 0.0f;
                
                if (((rects == null) || (rects.Count < 2)) && hierarchy.geometry.correctOffset)
                {
                    var xCorrectionNecessary = .5f - hullData.baseWidthCenter0To1;
                    var zCorrectionNecessary = -1 * hullData.baseHeightOffset0To1;

                    xOffset = xCorrectionNecessary * (shape.effectiveScale * 2);
                    zOffset = zCorrectionNecessary * (shape.effectiveScale * 2);
                }
                
                xOffset += shape.effectiveScale * hierarchy.geometry.xOffset;
                zOffset += shape.effectiveScale * hierarchy.geometry.zOffset;
                
                var offset = new Vector3(xOffset, 0f, zOffset);

                var yOffset = 0f;

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.BentSpoke)
                {
                    yOffset = hierarchy.geometry.bendFactor * shape.effectiveScale;
                }

                var xPush = shape.effectiveScale*xRatio;
                var zPush = shape.effectiveScale;
                
                var positionsRaw = new[]
                {
                    offset + new Vector3(-xPush, 0f, -zPush),
                    offset + new Vector3(-xPush, 0f, zPush),
                    offset + new Vector3(xPush, 0f, zPush),
                    offset + new Vector3(xPush, yOffset, -zPush)
                };

                var normal = new Vector3(
                    meshSettings.generatedPlaneNormalFactor,
                    1.0f - meshSettings.generatedPlaneNormalFactor,
                    meshSettings.generatedPlaneNormalFactor
                );

                //dumb
                var flip = hierarchy.geometry.flipLeafNormals ? 1 : -1;
                normal.y *= flip;

                var normalsRaw = new[]
                {
                    new Vector3(-normal.x, normal.y, -normal.z).normalized,
                    new Vector3(-normal.x, normal.y, 0).normalized, // note z always 0
                    new Vector3(normal.x, normal.y, 0).normalized, // note z always 0
                    new Vector3(normal.x, normal.y, -normal.z).normalized
                };

                var angles = new float[planeCount];

                for (var i = 0; i < planeCount; i++)
                {
                    var planeTime = i / (planeCount);

                    var rotation = 180.0f * planeTime;
                    angles[i] = rotation;
                }
                
                for (var planeIndex = 0; planeIndex < planeCount; planeIndex++)
                {
                    var planeTime = planeIndex / (float)planeCount;

                    if (planeTime > settings.leafGeometryQuality)
                    {
                        continue;
                    }

                    var planeRotation = Quaternion.Euler(new Vector3(90, angles[planeIndex], 0));
                    
                    _planeVerts[0] = new TreeVertex();
                    _planeVerts[1] = new TreeVertex();
                    _planeVerts[2] = new TreeVertex();
                    _planeVerts[3] = new TreeVertex();
                    _planeVerts[4] = new TreeVertex();
                    _planeVerts[5] = new TreeVertex();
                    _planeVerts[6] = new TreeVertex();
                    _planeVerts[7] = new TreeVertex();

                    _planeVerts[0].Set(shape, 0f);
                    _planeVerts[1].Set(shape, 0f);
                    _planeVerts[2].Set(shape, 0f);
                    _planeVerts[3].Set(shape, 0f);
                    _planeVerts[4].Set(shape, 0f);
                    _planeVerts[5].Set(shape, 0f);
                    _planeVerts[6].Set(shape, 0f);
                    _planeVerts[7].Set(shape, 0f);

                    for (var i = 0; i < 4; ++i)
                    {
                        _planeVerts[i].position = shape.effectiveMatrix.MultiplyPoint(planeRotation * positionsRaw[i]);
                        _planeVerts[i].normal = shape.effectiveMatrix.MultiplyVector(planeRotation * normalsRaw[i]);
                        _planeVerts[i].tangent = TangentGenerator.CreateTangent(shape, planeRotation, _planeVerts[i].normal);
                        _planeVerts[i].raw_uv0 = hullData.textureHull[i];
                    }

                    _planeVerts[0].heightOffset = 1f;
                    _planeVerts[1].heightOffset = 1f;
                    _planeVerts[2].heightOffset = 0f;
                    _planeVerts[3].heightOffset = 0f;

                    _planeVerts[0].rawWind.tertiaryRoll = 0.5f;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.4f;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.0f;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.0f;

                    for (var i = 0; i < 4; i++)
                    {
                        _planeVerts[i + 4].LerpVerticesByFactor(_planeVerts, hullData.textureHull[i]);

                        _planeVerts[i + 4].raw_uv0 = _planeVerts[i].raw_uv0;
                        _planeVerts[i + 4].heightOffset = _planeVerts[i].heightOffset;
                        _planeVerts[i + 4].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                        _planeVerts[i + 4].uvScaleUpdated = _planeVerts[i].uvScaleUpdated;
                    }

                    var index0 = output.vertices.Count;

                    for (var i = 0; i < 4;  i++)
                    {
                        output.AddVertex(_planeVerts[i + 4]);
                    }

                    var t1 = new TreeTriangle();
                    var t2 = new TreeTriangle();

                    t1.Set(shape, materialID, index0, index0 + 1, index0 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                    t2.Set(shape, materialID, index0, index0 + 2, index0 + 3, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                    output.AddTriangle(t1);
                    output.AddTriangle(t2);

                    var faceNormal = shape.effectiveMatrix.MultiplyVector(planeRotation * Vector3.up);

                    if (settings.doubleSidedLeafGeometry)
                    {
                        _planeVerts2[0] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[1] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[2] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[3] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[4] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[5] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[6] = new TreeVertex() /*TreeVertex.Get()*/;
                        _planeVerts2[7] = new TreeVertex() /*TreeVertex.Get()*/;

                        _planeVerts2[0].Set(shape, 0f);
                        _planeVerts2[1].Set(shape, 0f);
                        _planeVerts2[2].Set(shape, 0f);
                        _planeVerts2[3].Set(shape, 0f);
                        _planeVerts2[4].Set(shape, 0f);
                        _planeVerts2[5].Set(shape, 0f);
                        _planeVerts2[6].Set(shape, 0f);
                        _planeVerts2[7].Set(shape, 0f);

                        for (var i = 0; i < 4; ++i)
                        {
                            _planeVerts2[i].position = _planeVerts[i].position;
                            _planeVerts2[i].normal = Vector3.Reflect(_planeVerts[i].normal, faceNormal);
                            _planeVerts2[i].tangent = Vector3.Reflect(_planeVerts[i].tangent, faceNormal);
                            _planeVerts2[i].tangent.w = -1;
                            _planeVerts2[i].raw_uv0 = _planeVerts[i].raw_uv0;
                            _planeVerts2[i].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                            _planeVerts2[i].heightOffset = _planeVerts[i].heightOffset;
                        }

                        for (var i = 0; i < 4; ++i)
                        {
                            _planeVerts2[i + 4].LerpVerticesByFactor(_planeVerts2, hullData.textureHull[i]);
                            _planeVerts2[i + 4].raw_uv0 = _planeVerts2[i].raw_uv0;
                            _planeVerts2[i + 4].uvScaleUpdated = _planeVerts2[i].uvScaleUpdated;
                            _planeVerts2[i + 4].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                            _planeVerts2[i + 4].heightOffset = _planeVerts[i].heightOffset;
                        }

                        var index4 = output.vertices.Count;
                        for (var i = 0; i < 4; ++i)
                        {
                            output.AddVertex(_planeVerts2[i + 4]);
                        }

                        var t1_2 = new TreeTriangle();
                        var t2_2 = new TreeTriangle();

                        t1_2.Set(shape, materialID, index4, index4 + 2, index4 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                        t2_2.Set(shape, materialID, index4, index4 + 3, index4 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                        output.AddTriangle(t1_2);
                        output.AddTriangle(t2_2);
                    }
                }
            }
        }

        private static void GenerateLeafDiamondGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            MeshSettings meshSettings,
            LeafShapeData shape,
            LeafHierarchyData hierarchy)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafDiamondGeometry.Auto())
            {
                var m = inputMaterialCache.GetInputMaterialData(
                    hierarchy.geometry.leafMaterial,
                    TreeMaterialUsage.LeafPlane
                );

                var materialID = m == null ? -1 : m.materialID;

                TextureHullData hullData;

                using (BUILD_TIME.GEO_GEN.GetTextureHullData.Auto())
                {
                    if (materialID >= 0)
                    {
                        hullData = hierarchy.geometry.leafMaterial.GetTextureHullData();
                    }
                    else
                    {
                        hullData = TextureHullData.Default();
                    }
                }

                var yOffset = hierarchy.geometry.bendFactor * shape.effectiveScale;

                var normal = new Vector3(
                    meshSettings.generatedPlaneNormalFactor,
                    1.0f - meshSettings.generatedPlaneNormalFactor,
                    meshSettings.generatedPlaneNormalFactor
                );

                //dumb
                var flip = hierarchy.geometry.flipLeafNormals ? 1 : -1;

                normal.y *= flip;

                var positionsRaw = new Vector3[4];
                var normalsRaw = new Vector3[4];
                var uvsRaw = new Vector2[4];
                var halfY = hullData.baseHeightOffset0To1 + ((1.0f - hullData.baseHeightOffset0To1) / 2);

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondWidthCut)
                {
                    positionsRaw[0] = new Vector3(-shape.effectiveScale, 0.0f, 0.0f);
                    positionsRaw[1] = new Vector3(0.0f, yOffset, -shape.effectiveScale);
                    positionsRaw[2] = new Vector3(shape.effectiveScale, 0.0f, 0.0f);
                    positionsRaw[3] = new Vector3(0.0f, 0.0f, shape.effectiveScale);

                    normalsRaw[0] = new Vector3(-normal.x, normal.y, 0.0f).normalized;
                    normalsRaw[1] = new Vector3(0.0f, normal.y, -normal.z).normalized;
                    normalsRaw[2] = new Vector3(normal.x, normal.y, 0.0f).normalized;
                    normalsRaw[3] = new Vector3(0.0f, normal.y, 0.0f).normalized;

                    uvsRaw[0] = new Vector2(0.0f, halfY);
                    uvsRaw[1] = new Vector2(hullData.baseWidthCenter0To1, 1.0f);
                    uvsRaw[2] = new Vector2(1.0f, halfY);
                    uvsRaw[3] = new Vector2(hullData.baseWidthCenter0To1, hullData.baseHeightOffset0To1);
                }
                else
                {
                    positionsRaw[0] = new Vector3(0.0f, 0.0f, -shape.effectiveScale);
                    positionsRaw[1] = new Vector3(-shape.effectiveScale, yOffset, 0.0f);
                    positionsRaw[2] = new Vector3(0.0f, 0.0f, shape.effectiveScale);
                    positionsRaw[3] = new Vector3(shape.effectiveScale, yOffset, 0.0f);

                    normalsRaw[0] = new Vector3(0.0f, normal.y, -normal.z).normalized;
                    normalsRaw[1] = new Vector3(-normal.x, normal.y, 0.0f).normalized;
                    normalsRaw[2] = new Vector3(0.0f, normal.y, normal.z).normalized;
                    normalsRaw[3] = new Vector3(normal.x, normal.y, 0.0f).normalized;

                    uvsRaw[0] = new Vector2(hullData.baseWidthCenter0To1, 1.0f);
                    uvsRaw[1] = new Vector2(0.0f, halfY);
                    uvsRaw[2] = new Vector2(hullData.baseWidthCenter0To1, hullData.baseHeightOffset0To1);
                    uvsRaw[3] = new Vector2(1.0f, halfY);
                }

                var planeRotation = Quaternion.Euler(new Vector3(90, 0, 0));

                _planeVerts[0] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[1] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[2] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[3] = new TreeVertex() /*TreeVertex.Get()*/;

                _planeVerts[0].Set(shape, 0f);
                _planeVerts[1].Set(shape, 0f);
                _planeVerts[2].Set(shape, 0f);
                _planeVerts[3].Set(shape, 0f);

                for (var i = 0; i < 4; ++i)
                {
                    _planeVerts[i].position = shape.effectiveMatrix.MultiplyPoint(planeRotation * positionsRaw[i]);
                    _planeVerts[i].normal = shape.effectiveMatrix.MultiplyVector(planeRotation * normalsRaw[i]);
                    _planeVerts[i].tangent = TangentGenerator.CreateTangent(shape, planeRotation, _planeVerts[i].normal);
                    _planeVerts[i].raw_uv0 = uvsRaw[i];
                }

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondWidthCut)
                {
                    _planeVerts[0].rawWind.tertiaryRoll = 0.60f * shape.effectiveScale;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.45f * shape.effectiveScale;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.30f * shape.effectiveScale;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;

                    _planeVerts[0].heightOffset = 0.5f;
                    _planeVerts[1].heightOffset = 1.0f;
                    _planeVerts[2].heightOffset = 0.5f;
                    _planeVerts[3].heightOffset = 0.0f;
                }
                else
                {
                    _planeVerts[0].rawWind.tertiaryRoll = 0.60f * shape.effectiveScale;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.41f * shape.effectiveScale;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.28f * shape.effectiveScale;

                    _planeVerts[0].heightOffset = 1.0f;
                    _planeVerts[1].heightOffset = 0.5f;
                    _planeVerts[2].heightOffset = 0.0f;
                    _planeVerts[3].heightOffset = 0.5f;
                }

                var index0 = output.vertices.Count;

                for (var i = 0; i < 4; ++i)
                {
                    output.AddVertex(_planeVerts[i]);
                }

                var t1 = new TreeTriangle();
                var t2 = new TreeTriangle();

                t1.Set(shape, materialID, index0, index0 + 1, index0 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                t2.Set(shape, materialID, index0, index0 + 2, index0 + 3, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                output.AddTriangle(t1);
                output.AddTriangle(t2);

                if (settings.doubleSidedLeafGeometry)
                {
                    var faceNormal = shape.effectiveMatrix.MultiplyVector(planeRotation * Vector3.up);

                    _planeVerts2[0] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[1] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[2] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[3] = new TreeVertex() /*TreeVertex.Get()*/;

                    _planeVerts2[0].Set(shape, 0f);
                    _planeVerts2[1].Set(shape, 0f);
                    _planeVerts2[2].Set(shape, 0f);
                    _planeVerts2[3].Set(shape, 0f);

                    for (var i = 0; i < 4; ++i)
                    {
                        _planeVerts2[i].position = _planeVerts[i].position;
                        _planeVerts2[i].normal = Vector3.Reflect(_planeVerts[i].normal, faceNormal);
                        _planeVerts2[i].tangent = Vector3.Reflect(_planeVerts[i].tangent, faceNormal);
                        _planeVerts2[i].tangent.w = -1;
                        _planeVerts2[i].raw_uv0 = _planeVerts[i].raw_uv0;
                        _planeVerts2[i].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                        _planeVerts2[i].heightOffset = _planeVerts[i].heightOffset;
                    }

                    var index4 = output.vertices.Count;
                    for (var i = 0; i < 4; ++i)
                    {
                        output.AddVertex(_planeVerts2[i]);
                    }

                    var t1_2 = new TreeTriangle();
                    var t2_2 = new TreeTriangle();

                    t1_2.Set(shape, materialID, index4, index4 + 2, index4 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                    t2_2.Set(shape, materialID, index4, index4 + 3, index4 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                    output.AddTriangle(t1_2);
                    output.AddTriangle(t2_2);
                }
            }
        }

        private static void GenerateLeafPyramidGeometry(
            LODGenerationOutput output,
            InputMaterialCache inputMaterialCache,
            LevelOfDetailSettings settings,
            MeshSettings meshSettings,
            LeafShapeData shape,
            LeafHierarchyData hierarchy)
        {
            using (BUILD_TIME.GEO_GEN.GenerateLeafPyramidGeometry.Auto())
            {
                var m = inputMaterialCache.GetInputMaterialData(
                    hierarchy.geometry.leafMaterial,
                    TreeMaterialUsage.LeafPlane
                );

                var materialID = m == null ? -1 : m.materialID;

                TextureHullData hullData;

                using (BUILD_TIME.GEO_GEN.GetTextureHullData.Auto())
                {
                    if (materialID >= 0)
                    {
                        hullData = hierarchy.geometry.leafMaterial.GetTextureHullData();
                    }
                    else
                    {
                        hullData = TextureHullData.Default();
                    }
                }

                var yOffset = hierarchy.geometry.bendFactor * shape.effectiveScale;

                var normal = new Vector3(
                    meshSettings.generatedPlaneNormalFactor,
                    1.0f - meshSettings.generatedPlaneNormalFactor,
                    meshSettings.generatedPlaneNormalFactor
                );

                //dumb
                var flip = hierarchy.geometry.flipLeafNormals ? 1 : -1;

                normal.y *= flip;

                var positionsRaw = new Vector3[5];
                var normalsRaw = new Vector3[5];
                var uvsRaw = new Vector2[5];
                var halfY = hullData.baseHeightOffset0To1 + ((1.0f - hullData.baseHeightOffset0To1) / 2);

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondPyramid)
                {
                    positionsRaw[0] = new Vector3(0.0f, 0.4f * yOffset, 0.0f);
                    positionsRaw[1] = new Vector3(0.0f, 2.0f * yOffset, -shape.effectiveScale);
                    positionsRaw[2] = new Vector3(-shape.effectiveScale, 1.2f * yOffset, 0.0f);
                    positionsRaw[3] = new Vector3(0.0f, 0.0f * yOffset, shape.effectiveScale);
                    positionsRaw[4] = new Vector3(shape.effectiveScale, 0.8f * yOffset, 0.0f);

                    normalsRaw[0] = new Vector3(0.0f, normal.y, 0.0f).normalized;
                    normalsRaw[1] = new Vector3(0.0f, normal.y, -normal.z).normalized;
                    normalsRaw[2] = new Vector3(-normal.x, normal.y, 0.0f).normalized;
                    normalsRaw[3] = new Vector3(0.0f, normal.y, normal.z).normalized;
                    normalsRaw[4] = new Vector3(normal.x, normal.y, 0.0f).normalized;

                    uvsRaw[0] = new Vector2(hullData.baseWidthCenter0To1, halfY);
                    uvsRaw[1] = new Vector2(hullData.baseWidthCenter0To1, 1.0f);
                    uvsRaw[2] = new Vector2(0.0f, halfY);
                    uvsRaw[3] = new Vector2(hullData.baseWidthCenter0To1, hullData.baseHeightOffset0To1);
                    uvsRaw[4] = new Vector2(1.0f, halfY);
                }
                else
                {
                    positionsRaw[0] = new Vector3(0.0f, 0.4f * yOffset, 0.0f);
                    positionsRaw[1] = new Vector3(-shape.effectiveScale, 2.0f * yOffset, -shape.effectiveScale);
                    positionsRaw[2] = new Vector3(-shape.effectiveScale, 0.0f * yOffset, shape.effectiveScale);
                    positionsRaw[3] = new Vector3(shape.effectiveScale, 0.0f * yOffset, shape.effectiveScale);
                    positionsRaw[4] = new Vector3(shape.effectiveScale, 1.6f * yOffset, -shape.effectiveScale);

                    normalsRaw[0] = new Vector3(0.0f, normal.y, 0.0f).normalized;
                    normalsRaw[1] = new Vector3(-normal.x, normal.y, -normal.z).normalized;
                    normalsRaw[2] = new Vector3(-normal.x, normal.y, normal.z).normalized;
                    normalsRaw[3] = new Vector3(normal.x, normal.y, normal.z).normalized;
                    normalsRaw[4] = new Vector3(normal.x, normal.y, -normal.z).normalized;

                    uvsRaw[0] = new Vector2(hullData.baseWidthCenter0To1, halfY);
                    uvsRaw[1] = new Vector2(0.0f, 1.0f);
                    uvsRaw[2] = new Vector2(0.0f, hullData.baseHeightOffset0To1);
                    uvsRaw[3] = new Vector2(1.0f, hullData.baseHeightOffset0To1);
                    uvsRaw[4] = new Vector2(1.0f, 1.0f);
                }

                var planeRotation = Quaternion.Euler(new Vector3(90, 0, 0));

                _planeVerts[0] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[1] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[2] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[3] = new TreeVertex() /*TreeVertex.Get()*/;
                _planeVerts[4] = new TreeVertex() /*TreeVertex.Get()*/;

                _planeVerts[0].Set(shape, 0f);
                _planeVerts[1].Set(shape, 0f);
                _planeVerts[2].Set(shape, 0f);
                _planeVerts[3].Set(shape, 0f);
                _planeVerts[4].Set(shape, 0f);

                for (var i = 0; i < 5; ++i)
                {
                    _planeVerts[i].position = shape.effectiveMatrix.MultiplyPoint(planeRotation * positionsRaw[i]);
                    _planeVerts[i].normal = shape.effectiveMatrix.MultiplyVector(planeRotation * normalsRaw[i]);
                    _planeVerts[i].tangent = TangentGenerator.CreateTangent(shape, planeRotation, _planeVerts[i].normal);
                    _planeVerts[i].raw_uv0 = uvsRaw[i];
                }

                if (hierarchy.geometry.geometryMode == LeafGeometryMode.DiamondPyramid)
                {
                    _planeVerts[0].rawWind.tertiaryRoll = 0.20f * shape.effectiveScale;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.65f * shape.effectiveScale;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.30f * shape.effectiveScale;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;
                    _planeVerts[4].rawWind.tertiaryRoll = 0.25f * shape.effectiveScale;

                    _planeVerts[0].heightOffset = 0.5f;
                    _planeVerts[1].heightOffset = 1.0f;
                    _planeVerts[2].heightOffset = 0.5f;
                    _planeVerts[3].heightOffset = 0.0f;
                    _planeVerts[4].heightOffset = 0.5f;
                }
                else
                {
                    _planeVerts[0].rawWind.tertiaryRoll = 0.30f * shape.effectiveScale;
                    _planeVerts[1].rawWind.tertiaryRoll = 0.65f * shape.effectiveScale;
                    _planeVerts[2].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;
                    _planeVerts[3].rawWind.tertiaryRoll = 0.00f * shape.effectiveScale;
                    _planeVerts[4].rawWind.tertiaryRoll = 0.55f * shape.effectiveScale;

                    _planeVerts[0].heightOffset = 0.5f;
                    _planeVerts[1].heightOffset = 1.0f;
                    _planeVerts[2].heightOffset = 0.0f;
                    _planeVerts[3].heightOffset = 0.0f;
                    _planeVerts[4].heightOffset = 1.0f;
                }

                var index0 = output.vertices.Count;

                for (var i = 0; i < 5; ++i)
                {
                    output.AddVertex(_planeVerts[i]);
                }

                var t1 = new TreeTriangle();
                var t2 = new TreeTriangle();
                var t3 = new TreeTriangle();
                var t4 = new TreeTriangle();

                t1.Set(shape, materialID, index0, index0 + 1, index0 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                t2.Set(shape, materialID, index0, index0 + 2, index0 + 3, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                t3.Set(shape, materialID, index0, index0 + 3, index0 + 4, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                t4.Set(shape, materialID, index0, index0 + 4, index0 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                output.AddTriangle(t1);
                output.AddTriangle(t2);
                output.AddTriangle(t3);
                output.AddTriangle(t4);

                if (settings.doubleSidedLeafGeometry)
                {
                    var faceNormal = shape.effectiveMatrix.MultiplyVector(planeRotation * Vector3.up);

                    _planeVerts2[0] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[1] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[2] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[3] = new TreeVertex() /*TreeVertex.Get()*/;
                    _planeVerts2[4] = new TreeVertex() /*TreeVertex.Get()*/;

                    _planeVerts2[0].Set(shape, 0f);
                    _planeVerts2[1].Set(shape, 0f);
                    _planeVerts2[2].Set(shape, 0f);
                    _planeVerts2[3].Set(shape, 0f);
                    _planeVerts2[4].Set(shape, 0f);

                    for (var i = 0; i < 5; ++i)
                    {
                        _planeVerts2[i].position = _planeVerts[i].position;
                        _planeVerts2[i].normal = Vector3.Reflect(_planeVerts[i].normal, faceNormal);
                        _planeVerts2[i].tangent = Vector3.Reflect(_planeVerts[i].tangent, faceNormal);
                        _planeVerts2[i].tangent.w = -1;
                        _planeVerts2[i].raw_uv0 = _planeVerts[i].raw_uv0;
                        _planeVerts2[i].rawWind.tertiaryRoll = _planeVerts[i].rawWind.tertiaryRoll;
                        _planeVerts2[i].heightOffset = _planeVerts[i].heightOffset;
                    }

                    var index4 = output.vertices.Count;
                    for (var i = 0; i < 5; ++i)
                    {
                        output.AddVertex(_planeVerts2[i]);
                    }

                    var t1_2 = new TreeTriangle();
                    var t2_2 = new TreeTriangle();
                    var t3_2 = new TreeTriangle();
                    var t4_2 = new TreeTriangle();

                    t1_2.Set(shape, materialID, index4, index4 + 2, index4 + 1, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                    t2_2.Set(shape, materialID, index4, index4 + 3, index4 + 2, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                    t3_2.Set(shape, materialID, index4, index4 + 4, index4 + 3, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);
                    t4_2.Set(shape, materialID, index4, index4 + 1, index4 + 4, TreeMaterialUsage.LeafPlane, hierarchy.geometry.flipLeafNormals);

                    output.AddTriangle(t1_2);
                    output.AddTriangle(t2_2);
                    output.AddTriangle(t3_2);
                    output.AddTriangle(t4_2);
                }
            }
        }
    }
}
