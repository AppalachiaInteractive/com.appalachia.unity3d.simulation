using System;
using Appalachia.Core.Attributes;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Extensions;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Appalachia.Simulation.Trees.UI.Selections.State;

namespace Appalachia.Simulation.Trees.Build.RequestManagers
{
    [CallStaticConstructorInEditor]
    public static class BranchBuildRequestManager
    {
        static BranchBuildRequestManager()
        {
            TreeSpeciesEditorSelection.InstanceAvailable += i => _treeSpeciesEditorSelection = i;
        }

        private static TreeSpeciesEditorSelection _treeSpeciesEditorSelection;

        private static BranchDataContainer CTX => _treeSpeciesEditorSelection.branch.selection.selected;
        
        /// <summary>
        /// Builds all individuals for the species, resetting assets beforehand.
        /// </summary>
        public static void ForceFull()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.ForceFull;            
            CTX.PushBuildRequestLevelAll(true);
        }
        
        /// <summary>
        /// Builds all individuals for the species, only what needs to be built.
        /// </summary>
        public static void Full()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Full;
            CTX.PushBuildRequestLevelAll(true);
        }

        /// <summary>
        /// Builds only the current stage, only what needs to be built.
        /// </summary>
        public static void Default()
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            CTX.buildState = BuildState.Default;
            CTX.PushBuildRequestLevelAll(true);
        }
        
        private static void MaterialGenerationChanged()
        {
            CTX.BuildMaterialGeneration()
                .BuildMaterialProperties();
        }
        
        private static void MaterialPropertiesChanged()
        {
            CTX.BuildMaterialProperties();
        }

        private static void DistributionSettingsChanged()
        {
            CTX.BuildDistribution()
                /*.BuildMaterialProperties(BuildRequestLevel.FinalPass)*/
                .BuildStage();
        }
        
        private static void GeometrySettingsChanged()
        {
            CTX.BuildDistribution()
                .BuildStage()
                /*.BuildLowQualityGeometry(BuildRequestLevel.InitialPass)
                .BuildUv(BuildRequestLevel.InitialPass)*/;
        }
        
        private static void MeshSettingsChanged()
        {        
            CTX.BuildDistribution().BuildStage()
                /*.BuildLowQualityGeometry(BuildRequestLevel.InitialPass)
                .BuildUv(BuildRequestLevel.InitialPass)*/;
        }

        
        private static void AmbientOcclusionSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage();
        }

        private static void LevelOfDetailSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage();
        }

        private static void WindSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildMaterialProperties()
                .BuildStage();
        }
        
        
        private static void UVSettingsChanged()
        {
            CTX//.BuildDistribution(BuildRequestLevel.FinalPass)
                .BuildStage();
        }


        public static void SettingsChanged(SettingsUpdateTarget type)
        {
            CTX.dataState = TSEDataContainer.DataState.Dirty;
            
            if (CTX.buildState != BuildState.Disabled)
            {
                CTX.buildState = BuildState.Default;
            }
            
            switch (type)
            {
                case SettingsUpdateTarget.MaterialGeneration:
                    MaterialGenerationChanged();
                    break;
                case SettingsUpdateTarget.MaterialProperty:
                    MaterialPropertiesChanged();
                    break;
                case SettingsUpdateTarget.Geometry:
                    GeometrySettingsChanged();
                    break;
                case SettingsUpdateTarget.Mesh:
                    MeshSettingsChanged();
                    break;
                case SettingsUpdateTarget.Distribution:
                    DistributionSettingsChanged();
                    break;
                case SettingsUpdateTarget.UV:
                    UVSettingsChanged();
                    break;
                case SettingsUpdateTarget.AmbientOcclusion:
                    AmbientOcclusionSettingsChanged();
                    break;
                case SettingsUpdateTarget.LevelOfDetail:
                    LevelOfDetailSettingsChanged();
                    break;
                case SettingsUpdateTarget.VertexData:
                    WindSettingsChanged();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
