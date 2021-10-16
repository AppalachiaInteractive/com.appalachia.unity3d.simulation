using Unity.Profiling;

namespace Appalachia.Simulation.Trees.Build.Execution
{
    public static class BUILD_TIME
    {
        public static class LOG_BUILD_MGR
        {
            public static ProfilerMarker FinalizeBuild =
                new ProfilerMarker("LogBuildManager.FinalizeBuild");
            
            public static ProfilerMarker HandleBuildError =
                new ProfilerMarker("LogBuildManager.HandleBuildError");
            
            public static ProfilerMarker AssignMaterialProperties =
                new ProfilerMarker("LogBuildManager.AssignMaterialProperties");
            
            public static ProfilerMarker GenerateDistribution =
                new ProfilerMarker("LogBuildManager.GenerateDistribution");
            
            public static ProfilerMarker GenerateLogGeometry =
                new ProfilerMarker("LogBuildManager.GenerateLogGeometry");
            
            public static ProfilerMarker GenerateEnhancements =
                new ProfilerMarker("LogBuildManager.GenerateEnhancements");
        }
        
        public static class TREE_BUILD_MGR
        {
            public static ProfilerMarker ExecutePendingBuild =
                new ProfilerMarker("TreeBuildManager.ExecutePendingBuild");
            
            
            public static ProfilerMarker FinalizeBuild =
                new ProfilerMarker("TreeBuildManager.FinalizeBuild");
            
            public static ProfilerMarker HandleBuildError =
                new ProfilerMarker("TreeBuildManager.HandleBuildError");
            
            public static ProfilerMarker SaveBuildAssets =
                new ProfilerMarker("TreeBuildManager.SaveBuildAssets");
            
            public static ProfilerMarker GenerateIndividualStageEnhancements =
                new ProfilerMarker("TreeBuildManager.GenerateIndividualStageEnhancements");
            
            public static ProfilerMarker GenerateIndividualStageGeometry =
                new ProfilerMarker("TreeBuildManager.GenerateIndividualStageGeometry");
            
            public static ProfilerMarker UpdateStageSpecificMaterials =
                new ProfilerMarker("TreeBuildManager.UpdateStageSpecificMaterials");
            
            public static ProfilerMarker GenerateSpeciesMaterials =
                new ProfilerMarker("TreeBuildManager.GenerateSpeciesMaterials");
            
            public static ProfilerMarker GenerateBranchMaterial =
                new ProfilerMarker("TreeBuildManager.GenerateBranchMaterial");
            
            public static ProfilerMarker GenerateIndividualStageShapes =
                new ProfilerMarker("TreeBuildManager.GenerateIndividualStageShapes");
        }
        public static class BRNCH_BUILD_MGR
        {
            public static ProfilerMarker ExecutePendingBuild =
                new ProfilerMarker("BranchBuildManager.ExecutePendingBuild");
            
            
            public static ProfilerMarker FinalizeBuild =
                new ProfilerMarker("BranchBuildManager.FinalizeBuild");
            
            public static ProfilerMarker HandleBuildError =
                new ProfilerMarker("BranchBuildManager.HandleBuildError");
            
            public static ProfilerMarker SaveBuildAssets =
                new ProfilerMarker("BranchBuildManager.SaveBuildAssets");
            
            public static ProfilerMarker GenerateIndividualStageEnhancements =
                new ProfilerMarker("BranchBuildManager.GenerateIndividualStageEnhancements");
            
            public static ProfilerMarker GenerateIndividualStageGeometry =
                new ProfilerMarker("BranchBuildManager.GenerateIndividualStageGeometry");
            
            public static ProfilerMarker GenerateSpeciesMaterials =
                new ProfilerMarker("BranchBuildManager.GenerateSpeciesMaterials");
            
            public static ProfilerMarker GenerateIndividualStageShapes =
                new ProfilerMarker("BranchBuildManager.GenerateIndividualStageShapes");


        }

        public static class AMB_OCC_GEN
        {
            public static ProfilerMarker GenerateAOSamplePoints =
                new ProfilerMarker("AmbientOcclusionGenerator.GenerateAOSamplePoints");

            public static ProfilerMarker ApplyDefaultAmbientOcclusion =
                new ProfilerMarker("AmbientOcclusionGenerator.ApplyDefaultAmbientOcclusion");

            public static ProfilerMarker ApplyAmbientOcclusion =
                new ProfilerMarker("AmbientOcclusionGenerator.ApplyAmbientOcclusion");

            public static ProfilerMarker SetAmbientOcclusionForBillboard =
                new ProfilerMarker("AmbientOcclusionGenerator.SetAmbientOcclusionForBillboard");

            public static ProfilerMarker GetAmbientOcclusionAtPoint =
                new ProfilerMarker("AmbientOcclusionGenerator.GetAmbientOcclusionAtPoint");


        }


        public static class ASSET_MGR
        {

            public static ProfilerMarker SaveMesh = new ProfilerMarker("AssetManager.SaveMesh");
            public static ProfilerMarker SaveAllAssets = new ProfilerMarker("AssetManager.SaveAllAssets");
            public static ProfilerMarker SaveAsset = new ProfilerMarker("AssetManager.SaveAsset");
            public static ProfilerMarker SaveIntegrationAsset = new ProfilerMarker("AssetManager.SaveIntegrationAsset");
            public static ProfilerMarker SaveNonPrefabAssets = new ProfilerMarker("AssetManager.SaveNonPrefabAssets");
            public static ProfilerMarker UpdatePrefab = new ProfilerMarker("AssetManager.UpdatePrefab");
            public static ProfilerMarker RequiresUpdate = new ProfilerMarker("AssetManager.RequiresUpdate");
            public static ProfilerMarker UpdatePrefabSave = new ProfilerMarker("AssetManager.UpdatePrefab.Save");
            public static ProfilerMarker UpdatePrefabUpdate = new ProfilerMarker("AssetManager.UpdatePrefab.Update");
            public static ProfilerMarker UpdatePrefabLoaded = new ProfilerMarker("AssetManager.UpdatePrefabLoaded");
        }

        public static class SPC_BUILD_REQ
        {
            public static ProfilerMarker ShouldBuild = new ProfilerMarker("SpeciesBuildRequest.ShouldBuild");
            public static ProfilerMarker GetBuildCosts = new ProfilerMarker("SpeciesBuildRequest.GetBuildCosts");
            public static ProfilerMarker BuildLevel = new ProfilerMarker("SpeciesBuildRequest.BuildLevel");
        }

        public static class DIST_MGR
        {
            public static ProfilerMarker PopulateShapes = new ProfilerMarker("DistributionManager.PopulateShapes");

            public static ProfilerMarker UpdateShapeOffsets =
                new ProfilerMarker("DistributionManager.UpdateShapeOffsets");

            public static ProfilerMarker UpdateShapeVisibility =
                new ProfilerMarker("DistributionManager.UpdateShapeVisibility");

            public static ProfilerMarker UpdateShapeDistributionScales =
                new ProfilerMarker("DistributionManager.UpdateShapeDistributionScales");

            public static ProfilerMarker UpdateShapeAngles =
                new ProfilerMarker("DistributionManager.UpdateShapeAngles");
        }

        public static class SHAPE_GEN_MGR
        {
            public static ProfilerMarker GenerateShapes = new ProfilerMarker("ShapeGenerationManager.GenerateShapes");
        }

        public static class SHAPE_ARR
        {
            public static ProfilerMarker UpdateSplineOrigin = new ProfilerMarker("ShapeArranger.UpdateSplineOrigin");
            public static ProfilerMarker UpdateLeafOrigin = new ProfilerMarker("ShapeArranger.UpdateLeafOrigin");

            public static ProfilerMarker UpdateAddOnShapeOrigin =
                new ProfilerMarker("ShapeArranger.UpdateAddOnShapeOrigin");
        }

        public static class GEO_GEN
        {
            public static ProfilerMarker GenerateGeometry = new ProfilerMarker("GeometryGenerator.GenerateGeometry");
            public static ProfilerMarker GenerateMeshShape = new ProfilerMarker("GeometryGenerator.GenerateMeshShape");

            public static ProfilerMarker GenerateLeafGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateLeafGeometry");

            public static ProfilerMarker MergeMeshGeometryIntoTree =
                new ProfilerMarker("GeometryGenerator.MergeMeshGeometryIntoTree");

            public static ProfilerMarker GenerateLeafBillboardGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateLeafBillboardGeometry");

            public static ProfilerMarker CreateBillboardPlane =
                new ProfilerMarker("GeometryGenerator.CreateBillboardPlane");

            public static ProfilerMarker CreateBillboardVertex =
                new ProfilerMarker("GeometryGenerator.CreateBillboardVertex");

            public static ProfilerMarker GenerateLeafPlaneGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateLeafPlaneGeometry");

            public static ProfilerMarker GenerateLeafDiamondGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateLeafDiamondGeometry");

            public static ProfilerMarker GenerateLeafPyramidGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateLeafPlaneGeometry");

            public static ProfilerMarker GetTextureHullData =
                new ProfilerMarker("GeometryGenerator.GetTextureHullData");

            public static ProfilerMarker GenerateSplineGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateSplineGeometry");

            public static ProfilerMarker GenerateBranchGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateBranchGeometry");

            public static ProfilerMarker HeightSampleSort =
                new ProfilerMarker("GeometryGenerator.HeightSamples.Sort");

            public static ProfilerMarker GenerateRingsFromSamples =
                new ProfilerMarker("GeometryGenerator.GenerateRingsFromSamples");

            public static ProfilerMarker BuildBranchRings =
                new ProfilerMarker("GeometryGenerator.BuildBranchRings");

            public static ProfilerMarker GenerateFrondGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateFrondGeometry");

            public static ProfilerMarker GetSamplePoints = new ProfilerMarker("GeometryGenerator.GetSamplePoints");

            public static ProfilerMarker GenerateFruitGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateFruitGeometry");

            public static ProfilerMarker GenerateKnotGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateKnotGeometry");

            public static ProfilerMarker GenerateFungusGeometry =
                new ProfilerMarker("GeometryGenerator.GenerateFungusGeometry");

            public static ProfilerMarker ResampleSplineChildren =
                new ProfilerMarker("GeometryGenerator.ResampleSplineChildren");
        }
        
        public static class COLL_GEN
         {
             public static ProfilerMarker GenerateCollider =
                 new ProfilerMarker("ColliderGenerator.GenerateCollider");
                 
             public static ProfilerMarker GenerateLogColliders =
                 new ProfilerMarker("ColliderGenerator.GenerateLogColliders");
         }
        
        public static class SHDW_GEN
        {
            public static ProfilerMarker GenerateShadowCaster =
                new ProfilerMarker("ShadowCasterGenerator.GenerateShadowCaster");
        }

        public static class BRANCH_RING
        {
            public static ProfilerMarker Clone =
                new ProfilerMarker("BranchRing.Clone");
            public static ProfilerMarker ConnectLoops =
                new ProfilerMarker("BranchRing.ConnectLoops");
            public static ProfilerMarker CloseRings =
                new ProfilerMarker("BranchRing.CloseRings");
            public static ProfilerMarker FinishBrokenBranch =
                new ProfilerMarker("BranchRing.FinishBrokenBranch");
            public static ProfilerMarker BuildVertices =
                new ProfilerMarker("BranchRing.BuildVertices");
        }

        public static class SHAPE_WELDR
        {
            public static ProfilerMarker WeldShapeOriginToParent =
                new ProfilerMarker("ShapeWelder.WeldShapeOriginToParent");
            
            public static ProfilerMarker WeldGeometryToParent =
                new ProfilerMarker("ShapeWelder.WeldGeometryToParent");
        }

        
        public static class TEX_HULL_GEN
        {
            public static ProfilerMarker GetTextureHullData = new ProfilerMarker("TextureHullGenerator.GetTextureHullData");
            public static ProfilerMarker GetShapePoints = new ProfilerMarker("TextureHullGenerator.GetShapePoints");
            public static ProfilerMarker IteratePixels = new ProfilerMarker("TextureHullGenerator.IteratePixels");
            public static ProfilerMarker AggregatePixelData = new ProfilerMarker("TextureHullGenerator.AggregatePixelData");
            public static ProfilerMarker LoadAndCacheTexture = new ProfilerMarker("TextureHullGenerator.LoadAndCacheTexture");
            public static ProfilerMarker GenerateOutline = new ProfilerMarker("TextureHullGenerator.GenerateOutline");
            public static ProfilerMarker ConvexHull = new ProfilerMarker("TextureHullGenerator.ConvexHull");
            public static ProfilerMarker ReduceVertices = new ProfilerMarker("TextureHullGenerator.ReduceVertices");
            public static ProfilerMarker ClampHull = new ProfilerMarker("TextureHullGenerator.ClampHull");
        }

        public static class TREE_IDV_STG_VAR
        {
            public static ProfilerMarker UpdateShapes = new ProfilerMarker("TreeIndividualStage.UpdateShapes");

            public static ProfilerMarker CreateShapeVariations =
                new ProfilerMarker("TreeIndividualStage.CreateShapeVariations");
        }

        public static class MESH_GEN
        {
            public static ProfilerMarker GenerateMeshes = new ProfilerMarker("MeshGenerator.UpdateShapes");
            public static ProfilerMarker CreateMeshFromData = new ProfilerMarker("MeshGenerator.CreateMeshFromData");
            public static ProfilerMarker RecalculateNormals = new ProfilerMarker("MeshGenerator.RecalculateNormals");
        }

        public static class SPLN_MOD
        {
            public static ProfilerMarker UpdateSpline = new ProfilerMarker("SplineModeler.UpdateSpline");

            public static ProfilerMarker GetApproximateLength =
                new ProfilerMarker("SplineModeler.GetApproximateLength");

            public static ProfilerMarker UpdateTime = new ProfilerMarker("SplineModeler.UpdateTime");
            public static ProfilerMarker UpdateRotations = new ProfilerMarker("SplineModeler.UpdateRotations");
            public static ProfilerMarker GetRotationInternal = new ProfilerMarker("SplineModeler.GetRotationInternal");
            public static ProfilerMarker GetPositionInternal = new ProfilerMarker("SplineModeler.GetPositionInternal");
            public static ProfilerMarker GetRotationAtTime = new ProfilerMarker("SplineModeler.GetRotationAtTime");
            public static ProfilerMarker GetPositionAtTime = new ProfilerMarker("SplineModeler.GetPositionAtTime");

            public static ProfilerMarker GetSurfaceAngleAtTime =
                new ProfilerMarker("SplineModeler.GetSurfaceAngleAtTime");

            public static ProfilerMarker GetRadiusAtTime = new ProfilerMarker("SplineModeler.GetRadiusAtTime");

            public static ProfilerMarker GetFlareCollarAtTime =
                new ProfilerMarker("SplineModeler.GetFlareCollarAtTime");
        }

        public static class WIND_GEN
        {
            public static ProfilerMarker GenerateOctree = new ProfilerMarker("WindGenerator.GenerateOctree");
            
            public static ProfilerMarker GenerateOctreeEncapsulate = new ProfilerMarker("WindGenerator.GenerateOctreeEncapsulate");
            
            public static ProfilerMarker ApplyMeshWindData = new ProfilerMarker("WindGenerator.ApplyMeshWindData");
            
            public static ProfilerMarker GeneratePrimaryWindData = new ProfilerMarker("WindGenerator.GeneratePrimaryWindData");
            
            public static ProfilerMarker GenerateSecondaryWindData = new ProfilerMarker("WindGenerator.GenerateSecondaryWindData");
            
            public static ProfilerMarker GenerateTertiaryWindData = new ProfilerMarker("WindGenerator.GenerateTertiaryWindData");
            
            public static ProfilerMarker GeneratePhaseWindData = new ProfilerMarker("WindGenerator.GeneratePhaseWindData");
        }
        
        public static class LOG_GEN
        {
            
            public static ProfilerMarker ApplyMeshLogData = new ProfilerMarker("LogGenerator.ApplyMeshLogData");
        }
        
        public static class OCTREE
        {
            public static ProfilerMarker QueryNearestWhere = new ProfilerMarker("Octree.QueryNearestWhere");
            
            public static ProfilerMarker Add = new ProfilerMarker("Octree.Add");
        }


        public static class TRN_COLOR_CALC
        {
            public static ProfilerMarker SetAutomaticTransmission =
                new ProfilerMarker("TransmissionColorCalculator.SetAutomaticTransmission");
        }

        public static class OUT_TEX_SET
        {
            public static ProfilerMarker Apply = new ProfilerMarker("OutputTextureSetting.Apply");
            public static ProfilerMarker Create = new ProfilerMarker("OutputTextureSetting.Create");
        }

        /*
        public static class OUT_TEX
        {
            public static ProfilerMarker EnsureCreated = new ProfilerMarker("OutputTexture.EnsureCreated");
            public static ProfilerMarker ReadRenderTexture = new ProfilerMarker("OutputTexture.ReadRenderTexture");
            
            public static ProfilerMarker Save = new ProfilerMarker("OutputTexture.Save");
            public static ProfilerMarker WriteBytes = new ProfilerMarker("OutputTexture.WriteBytes");
            public static ProfilerMarker WriteSettings = new ProfilerMarker("OutputTexture.WriteSettings");
            
            public static ProfilerMarker Refresh = new ProfilerMarker("OutputTexture.Refresh");
            
            public static ProfilerMarker SaveAndReimportTextures = new ProfilerMarker("MaterialBatchManager.SaveAndReimportTextures");
        }
        */

        public static class MAT_BAT_MGR
        {

            public static ProfilerMarker ActivateProcessingTextureImporterSettings =
                new ProfilerMarker("MaterialBatchManager.ActivateProcessingTextureImporterSettings");

            public static ProfilerMarker RestoreOriginalTextureImporterSettings =
                new ProfilerMarker("MaterialBatchManager.RestoreOriginalTextureImporterSettings");

            public static ProfilerMarker SaveTextures =
                new ProfilerMarker("MaterialBatchManager.SaveTextures");

            public static ProfilerMarker ReimportTextures =
                new ProfilerMarker("MaterialBatchManager.ReimportTextures");

            public static ProfilerMarker WriteAllTextures =
                new ProfilerMarker("MaterialBatchManager.WriteAllTextures");

            public static ProfilerMarker WriteTexture =
                new ProfilerMarker("MaterialBatchManager.WriteTexture");
            
            public static ProfilerMarker AssetDatabaseRefresh =
                new ProfilerMarker("MaterialBatchManager.AssetDatabaseManager.Refresh");
            
            public static ProfilerMarker ApplyAllSettings =
                new ProfilerMarker("MaterialBatchManager.ApplyAllSettings");
            
            public static ProfilerMarker ApplySettings =
                new ProfilerMarker("MaterialBatchManager.ApplySettings");
            
            public static ProfilerMarker Import =
                new ProfilerMarker("MaterialBatchManager.Import");
            
            public static ProfilerMarker ImportMaterials =
                new ProfilerMarker("MaterialBatchManager.Import.ImportMaterials");
            
            public static ProfilerMarker ImportMaterialsGetPath =
                new ProfilerMarker("MaterialBatchManager.Import.ImportMaterials.GetPath");
            
            public static ProfilerMarker ImportMaterialsMove =
                new ProfilerMarker("MaterialBatchManager.Import.ImportMaterials.Move");
            
            public static ProfilerMarker ImportMaterialsCreate =
                new ProfilerMarker("MaterialBatchManager.Import.ImportMaterials.Create");
            
            public static ProfilerMarker ImportTextures =
                new ProfilerMarker("MaterialBatchManager.Import.ImportTextures");
            
            public static ProfilerMarker ReloadAll =
                new ProfilerMarker("MaterialBatchManager.ReloadAll");
            
            public static ProfilerMarker Reload =
                new ProfilerMarker("MaterialBatchManager.Reload");
        }



        public static class TEX_READ_HELP
        {
            public static ProfilerMarker ToReadable = new ProfilerMarker("TextureReadabilityHelper.ToReadable");
        }

        public static class TEX_EXTRC
        {
            public static ProfilerMarker GetInputTextureSet = new ProfilerMarker("TextureExtractor.GetInputTextureSet");
        }

        public static class TEX_COMBR
        {
            public static ProfilerMarker DrawAtlasTextures = new ProfilerMarker("TextureCombiner.DrawAtlasTextures");
            public static ProfilerMarker DrawTiledTextures = new ProfilerMarker("TextureCombiner.DrawTiledTextures");
            public static ProfilerMarker DrawTextures = new ProfilerMarker("TextureCombiner.DrawTextures");
            public static ProfilerMarker DrawTexturePass = new ProfilerMarker("TextureCombiner.DrawTexturePass");
            public static ProfilerMarker ReadRenderTexture = new ProfilerMarker("TextureCombiner.ReadRenderTexture");
            public static ProfilerMarker FlipRenderTexture = new ProfilerMarker("TextureCombiner.FlipRenderTexture");
            public static ProfilerMarker SetFlippedPixels = new ProfilerMarker("TextureCombiner.SetFlippedPixels");
            public static ProfilerMarker SaveCombiners = new ProfilerMarker("TextureCombiner.SaveCombiners");

        }

        public static class OUT_MAT
        {
            public static ProfilerMarker Save = new ProfilerMarker("OutputMaterial.Save");
            public static ProfilerMarker Reload = new ProfilerMarker("OutputMaterial.Reload");

            public static ProfilerMarker SaveMaterial =
                new ProfilerMarker("OutputMaterial.SaveAssetToSubfolder");

            public static ProfilerMarker SetTextures = new ProfilerMarker("OutputMaterial.SetTextures");

            public static ProfilerMarker CreateMaterialInternal =
                new ProfilerMarker("OutputMaterial.CreateMaterialInternal");

            public static ProfilerMarker UpdateShader = new ProfilerMarker("OutputMaterial.UpdateShader");
            public static ProfilerMarker UpdateMaterial = new ProfilerMarker("OutputMaterial.UpdateMaterial");
            public static ProfilerMarker Change = new ProfilerMarker("OutputMaterial.Change");

        }

        public static class INPUT_MAT
        {
            public static ProfilerMarker UpdateTextures = new ProfilerMarker("InputMaterial.UpdateTextures");
        }

        public static class INPUT_TEX
        {
            public static ProfilerMarker ActivateProcessingTextureImporterSettings =
                new ProfilerMarker("InputTexture.ActivateProcessingTextureImporterSettings");

            public static ProfilerMarker RestoreOriginalTextureImporterSettings =
                new ProfilerMarker("InputTexture.RestoreOriginalTextureImporterSettings");

            public static ProfilerMarker GetUsageData = new ProfilerMarker("InputTexture.GetUsageData");
        }

        public static class TEX
        {
            public static ProfilerMarker SaveAndReimport = new ProfilerMarker("TextureImporter.SaveAndReimport");
        }


        public static class TREE_MAT_COLL
        {
            public static ProfilerMarker SetDefaultMaterials =
                new ProfilerMarker("TreeMaterialCollection.SetDefaultMaterials");

            public static ProfilerMarker UpdateDefaultMaterials =
                new ProfilerMarker("TreeMaterialCollection.UpdateDefaultMaterials");

            public static ProfilerMarker Update = new ProfilerMarker("TreeMaterialCollection.Update");
            public static ProfilerMarker CalculateHash = new ProfilerMarker("TreeMaterialCollection.CalculateHash");

            public static ProfilerMarker RequiresUpdate = new ProfilerMarker("TreeMaterialCollection.RequiresUpdate");
        }
        
        public static class BRNCH_MAT_COLL
        {

            public static ProfilerMarker CalculateHash = new ProfilerMarker("BranchMaterialCollection.CalculateHash");

            public static ProfilerMarker RequiresUpdate = new ProfilerMarker("BranchMaterialCollection.RequiresUpdate");
        }


        public static class TREE_MAT_CACHE
        {
            public static ProfilerMarker RebuildCache = new ProfilerMarker("TreeMaterialCache.RebuildCache");
            public static ProfilerMarker Update = new ProfilerMarker("TreeMaterialCache.Update");
            public static ProfilerMarker RemoveUnnecessary = new ProfilerMarker("TreeMaterialCache.RemoveUnnecessary");
            
            public static ProfilerMarker SetDefaultMaterials =
                new ProfilerMarker("TreeMaterialCache.SetDefaultMaterials");

            public static ProfilerMarker UpdateDefaultMaterials =
                new ProfilerMarker("TreeMaterialCache.UpdateDefaultMaterials");
        }

        public static class TEX_ATLAS
        {
            public static ProfilerMarker Pack = new ProfilerMarker("TextureAtlas.Pack");
        }

        public static class UV_MGR
        {
            public static ProfilerMarker ApplyLeafRects =
                new ProfilerMarker("UVManager.ApplyLeafRects");
            
            public static ProfilerMarker RemapUVCoordinates =
                new ProfilerMarker("UVManager.RemapUVCoordinates");
        }

        public static class LOG_MAT_PROP_MGR
        {
            public static ProfilerMarker AssignMaterialProperties =
                new ProfilerMarker("LogMaterialPropertyManager.AssignMaterialProperties");
        }

        public static class MAT_PROP_MGR
        {
            public static ProfilerMarker AssignMaterialProperties =
                new ProfilerMarker("MaterialPropertyManager.AssignMaterialProperties");
            
            public static ProfilerMarker SetDynamicPrototypes =
                new ProfilerMarker("MaterialPropertyManager.SetDynamicPrototypes");

            public static ProfilerMarker CopyPrototypeMaterialProperties =
                new ProfilerMarker("MaterialPropertyManager.CopyPrototypeMaterialProperties");
        }

        public static class SEED_MGR
        {
            public static ProfilerMarker UpdateSeeds =
                new ProfilerMarker("SeedManager.UpdateSeeds");
            
            public static ProfilerMarker UpdateShapeSeed =
                new ProfilerMarker("SeedManager.UpdateShapeSeed");
            
            public static ProfilerMarker RebuildSeeds =
                new ProfilerMarker("SeedManager.RebuildSeeds");

            public static ProfilerMarker ResetSeeds =
                new ProfilerMarker("SeedManager.ResetSeeds");
        }
        
        public static class BUILD_PRG_TRCK
        {
            public static ProfilerMarker InitializeBuildBatch =
                new ProfilerMarker("TreeBuildProgressTracker.InitializeBuildBatch");
            
            public static ProfilerMarker InitializeBuild =
                new ProfilerMarker("TreeBuildProgressTracker.InitializeBuild");
            
            public static ProfilerMarker MultiplyBuildCount =
                new ProfilerMarker("TreeBuildProgressTracker.MultiplyBuildCount");

            public static ProfilerMarker AddBuildCost =
                new ProfilerMarker("TreeBuildProgressTracker.AddBuildCost");

            public static ProfilerMarker Update =
                new ProfilerMarker("TreeBuildProgressTracker.Update");

            public static ProfilerMarker CompleteBuild =
                new ProfilerMarker("TreeBuildProgressTracker.CompleteBuild");

            public static ProfilerMarker CompleteBuildBatch =
                new ProfilerMarker("TreeBuildProgressTracker.CompleteBuildBatch");
        }
        
        public static class INDV_STG_GEN_CXT
        {
            public static ProfilerMarker Refresh =
                new ProfilerMarker("IndividualStageGenerationContext.Refresh");
            
            public static ProfilerMarker ClearGeometry =
                new ProfilerMarker("IndividualStageGenerationContext.ClearGeometry");
        }
        
        public static class LOG_GEN_CXT
        {
            public static ProfilerMarker Refresh =
                new ProfilerMarker("TreeLog.Refresh");
            
            public static ProfilerMarker ClearGeometry =
                new ProfilerMarker("TreeLog.ClearGeometry");
        }
        
        public static class SPC_DATA_STRCT
        {
            public static ProfilerMarker RecurseHierarchies =
                new ProfilerMarker("SpeciesDataStructure.RecurseHierarchies");
            
            public static ProfilerMarker RecurseSplines =
                new ProfilerMarker("SpeciesDataStructure.RecurseSplines");
            
        }
        
    }
}
