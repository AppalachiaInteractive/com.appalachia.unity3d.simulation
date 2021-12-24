using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Simulation.Core.Metadata.Tree.Types;
using Appalachia.Simulation.Trees.Build;
using Appalachia.Simulation.Trees.Build.Cost;
using Appalachia.Simulation.Trees.Build.Execution;
using Appalachia.Simulation.Trees.Build.Requests;
using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Core.Seeds;
using Appalachia.Simulation.Trees.Data;
using Appalachia.Simulation.Trees.Definition.Interfaces;
using Appalachia.Simulation.Trees.Generation.Assets;
using Appalachia.Simulation.Trees.Generation.Meshing;
using Appalachia.Simulation.Trees.Generation.Spline;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Input;
using Appalachia.Simulation.Trees.Generation.Texturing.Materials.Management;
using Appalachia.Simulation.Trees.Hierarchy;
using Appalachia.Simulation.Trees.Icons;
using Appalachia.Simulation.Trees.Interfaces;
using Appalachia.Simulation.Trees.Settings;
using Appalachia.Simulation.Trees.Shape;
using Appalachia.Simulation.Trees.Shape.Collections;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Definition
{
    [Serializable]
    public class TreeStage : TypeBasedSettings<TreeStage>, IMenuItemProvider
    {
        #region Fields and Autoproperties

        public AgeType ageType;

        public bool active;

        public int individualID;

        public List<LODGenerationOutput> lods;

        public StageBuildRequests buildRequest;

        public StageType stageType;

        public TreeAsset asset;

        public TreeRuntimeInstanceMetadata runtimeMetadata;

        public TreeShapes shapes;

        #endregion

        public bool CanBeCut => (stageType == StageType.Normal) || (stageType == StageType.Dead);

        public bool RequiresRigidbody
        {
            get
            {
                switch (stageType)
                {
                    case StageType.Normal:
                    case StageType.Stump:
                    case StageType.StumpRotted:
                    case StageType.Dead:
                        return false;
                    case StageType.Felled:
                    case StageType.FelledBare:
                    case StageType.FelledBareRotted:
                    case StageType.DeadFelled:
                    case StageType.DeadFelledRotted:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public LODGenerationOutput LOD0 => lods[0];

        public static TreeStage Create(
            string folder,
            NameBasis nameBasis,
            int individualID,
            AgeType ageType,
            StageType stageType,
            TreeAsset asset)
        {
            var assetName = nameBasis.FileNameStageSO(individualID, ageType, stageType);
            var instance = TreeStage.LoadOrCreateNew(folder, assetName);

            instance.ageType = ageType;
            instance.asset = asset;
            instance.individualID = individualID;
            instance.stageType = stageType;

            instance.shapes = new TreeShapes();
            instance.buildRequest = new StageBuildRequests();

            return instance;
        }

        public static string GetMenuString(StageType stage)
        {
            switch (stage)
            {
                case StageType.Normal:
                    return "Normal";
                case StageType.Stump:
                    return "Stump";
                case StageType.StumpRotted:
                    return "Rotted Stump";
                case StageType.Felled:
                    return "Felled";
                case StageType.FelledBare:
                    return "Barren Felled";
                case StageType.FelledBareRotted:
                    return "Rotted & Barren Felled";
                case StageType.Dead:
                    return "Dead";
                case StageType.DeadFelled:
                    return "Felled Dead";
                case StageType.DeadFelledRotted:
                    return "Rotted & Felled Dead";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ClearGeometry(LevelOfDetailSettingsCollection lodSettings)
        {
            using (BUILD_TIME.INDV_STG_GEN_CXT.ClearGeometry.Auto())
            {
                if (lods.Count > lodSettings.levels)
                {
                    for (var i = lods.Count - 1; i >= 0; i--)
                    {
                        RemoveLOD(i);
                    }
                }

                foreach (var lod in lods)
                {
                    lod.Clear();
                }
            }
        }

        public void CreateShapeVariations(
            IHierarchyRead hierarchyRead,
            TreeVariantSettings settings,
            InputMaterialCache inputMaterials,
            BaseSeed baseSeed)
        {
            using (BUILD_TIME.TREE_IDV_STG_VAR.CreateShapeVariations.Auto())
            {
                var hierarchyMaterials = new Dictionary<int, AtlasInputMaterial>();

                foreach (var h in hierarchyRead.GetHierarchies(TreeComponentType.Leaf)
                                               .Cast<LeafHierarchyData>())
                {
                    if (!hierarchyMaterials.ContainsKey(h.hierarchyID))
                    {
                        var matID = inputMaterials.GetMaterialIDByMaterial(h.geometry.leafMaterial, true);

                        var mat = inputMaterials.GetByMaterialID(matID);

                        hierarchyMaterials.Add(h.hierarchyID, mat as AtlasInputMaterial);
                    }
                }

                foreach (var leafShape in shapes.leafShapes)
                {
                    var mat = hierarchyMaterials[leafShape.hierarchyID];

                    leafShape.forcedInvisible = !mat.eligibleForLiveTrees;
                }

                UpdateFungusVisibility(hierarchyRead, settings, baseSeed);

                // stump
                if ((stageType == StageType.Stump) || (stageType == StageType.StumpRotted) // ||
                    // state == TreeStateType.Dead || 
                    // state == TreeStateType.DeadFelledBare || 
                    // state == TreeStateType.DeadFelledRotted || 
                    // state == TreeStateType.Felled || 
                    // state == TreeStateType.FelledBare ||
                    // state == TreeStateType.FelledBareRotted
                )
                {
                    Stump(hierarchyRead, settings);
                }

                // felled
                if (

                    // state == TreeStateType.Stump || 
                    // state == TreeStateType.StumpRotted ||
                    // state == TreeStateType.Dead || 
                    (stageType == StageType.DeadFelled) ||
                    (stageType == StageType.DeadFelledRotted) ||
                    (stageType == StageType.Felled) ||
                    (stageType == StageType.FelledBare) ||
                    (stageType == StageType.FelledBareRotted))
                {
                    Felled(hierarchyRead, settings);
                }

                // dead
                if (

                    // state == TreeStateType.Stump || 
                    // state == TreeStateType.StumpRotted ||
                    (stageType == StageType.Dead) ||
                    (stageType == StageType.DeadFelled) ||
                    (stageType == StageType.DeadFelledRotted) // || 
                    // state == TreeStateType.Felled || 
                    // state == TreeStateType.FelledBare ||
                    // state == TreeStateType.FelledBareRotted
                )
                {
                    Dead(hierarchyRead, settings, hierarchyMaterials);
                }

                // bare
                if (

                    // state == TreeStateType.Stump || 
                    // state == TreeStateType.StumpRotted ||
                    // state == TreeStateType.Dead || 
                    // state == TreeStateType.DeadFelledBare || 
                    // state == TreeStateType.DeadFelledRotted || 
                    // state == TreeStateType.Felled || 
                    (stageType == StageType.FelledBare) || (stageType == StageType.FelledBareRotted))
                {
                    Bare(hierarchyRead, settings, hierarchyMaterials);
                }

                /*
                if (
                    state == TreeStateType.Stump || 
                    state == TreeStateType.StumpRotted ||
                    state == TreeStateType.Dead || 
                    state == TreeStateType.DeadFelled || 
                    state == TreeStateType.DeadFelledRotted || 
                    state == TreeStateType.Felled || 
                    state == TreeStateType.FelledBare ||
                    state == TreeStateType.FelledBareRotted ||                
                    state == TreeStateType.Spirit
                )
                {
                }
                */
            }
        }

        public void FinalizeStageShapes(
            TreeStage model,
            IHierarchyRead hierarchyRead,
            TreeVariantSettings settings,
            InputMaterialCache inputMaterials,
            BaseSeed baseSeed)
        {
            using (BUILD_TIME.TREE_IDV_STG_VAR.UpdateShapes.Auto())
            {
                if (shapes == null)
                {
                    shapes = new TreeShapes();
                }

                if (model != null)
                {
                    shapes.Clear();

                    model.shapes.CopyPropertiesToClone(shapes);
                    shapes.Rebuild();
                }

                CreateShapeVariations(hierarchyRead, settings, inputMaterials, baseSeed);

                foreach (var shape in shapes)
                {
                    shape.stageType = stageType;
                }
            }
        }

        public IEnumerable<BuildCost> GetBuildCosts(BuildRequestLevel level)
        {
            return buildRequest.GetBuildCosts(level);
        }

        public string GetMenuString()
        {
            return GetMenuString(stageType);
        }

        public BuildRequestLevel GetRequestLevel(BuildState buildState)
        {
            var rl = buildRequest.requestLevel;

            if (rl == BuildRequestLevel.InitialPass)
            {
                return rl;
            }

            if (buildRequest == null)
            {
                buildRequest = new StageBuildRequests();
            }

            if (active || (buildState > BuildState.Full))
            {
                rl = rl.Max(buildRequest.requestLevel);

                if (rl == BuildRequestLevel.InitialPass)
                {
                    return rl;
                }
            }

            return rl;
        }

        public void Rebuild()
        {
            shapes.Rebuild();
        }

        public void RecreateMesh(int level)
        {
            asset.levels[level].CreateMesh(asset.GetMeshName(level));
        }

        public void Refresh(TreeSettings settings, IHierarchyRead hierarchies)
        {
            using (BUILD_TIME.INDV_STG_GEN_CXT.Refresh.Auto())
            {
                shapes.RemoveOrphanedShapes(hierarchies);
                shapes.Rebuild();
                asset.Refresh(settings);

                if (lods == null)
                {
                    lods = new List<LODGenerationOutput>();
                }

                var current = lods;
                lods = new List<LODGenerationOutput>();

                for (var i = 0; i < settings.lod.levels; i++)
                {
                    var match = current.FirstOrDefault(s => s.lodLevel == i);

                    if (match == null)
                    {
                        match = new LODGenerationOutput(i);
                    }

                    lods.Add(match);
                }
            }
        }

        public void SetMaterials(int level, Material[] materials)
        {
            if (materials.Length == 0)
            {
                asset.levels[level].materials = new[] { _defaultMaterialResource.material };
            }
            else
            {
                asset.levels[level].materials = materials;
            }
        }

        public bool ShouldRebuildGeometry(BuildRequestLevel level)
        {
            return buildRequest.ShouldBuild(BuildCategory.HighQualityGeometry, level) ||
                   buildRequest.ShouldBuild(BuildCategory.LowQualityGeometry,  level);
        }

        protected void UpdateFungusVisibility(
            IHierarchyRead hierarchyRead,
            TreeVariantSettings settings,
            BaseSeed baseSeed)
        {
            var likelihood = settings.liveLikelihood;

            if ((stageType == StageType.Dead) || (stageType == StageType.DeadFelled))
            {
                likelihood = settings.deadLikelihood;
            }
            else if ((stageType == StageType.StumpRotted) ||
                     (stageType == StageType.DeadFelledRotted) ||
                     (stageType == StageType.FelledBareRotted))
            {
                likelihood = settings.rottedLikelihood;
            }

            var heightRange = settings.liveHeightRange.Value;

            if ((stageType == StageType.Dead) || (stageType == StageType.DeadFelled))
            {
                heightRange = settings.deadHeightRange;
            }
            else if ((stageType == StageType.StumpRotted) ||
                     (stageType == StageType.DeadFelledRotted) ||
                     (stageType == StageType.FelledBareRotted))
            {
                heightRange = settings.rottedHeightRange;
            }

            foreach (var fungus in shapes.fungusShapes)
            {
                var h = hierarchyRead.GetHierarchyData(fungus.hierarchyID) as FungusHierarchyData;

                var seed = new VirtualSeed(baseSeed, h.seed);

                if (seed.RandomValue() > likelihood)
                {
                    fungus.forcedInvisible = true;
                }
                else
                {
                    fungus.forcedInvisible = false;
                }

                var fHeight = fungus.matrix.MultiplyPoint(Vector3.zero).y;

                if ((fHeight < heightRange.x) || (fHeight > heightRange.y))
                {
                    fungus.forcedInvisible = true;
                }
            }
        }

        private void Bare(
            IHierarchyRead readStructure,
            TreeVariantSettings settings,
            Dictionary<int, AtlasInputMaterial> inputMaterials)
        {
            foreach (var leafShape in shapes.leafShapes)
            {
                var mat = inputMaterials[leafShape.hierarchyID];

                if (!mat.eligibleForDeadTrees)
                {
                    leafShape.forcedInvisible = true;
                }
            }

            foreach (var fruit in shapes.fruitShapes)
            {
                fruit.forcedInvisible = true;
            }
        }

        private void Dead(
            IHierarchyRead readStructure,
            TreeVariantSettings settings,
            Dictionary<int, AtlasInputMaterial> inputMaterials)
        {
            var levels = new Dictionary<int, int>();

            foreach (var branchShape in shapes.branchShapes)
            {
                var hierarchy = readStructure.GetHierarchyData(branchShape.hierarchyID);
                var parentHierarchy = readStructure.GetHierarchyData(hierarchy.parentHierarchyID);

                if (parentHierarchy is BranchHierarchyData)
                {
                    var doubleParentHierarachy =
                        readStructure.GetHierarchyData(parentHierarchy.parentHierarchyID);

                    if (doubleParentHierarachy is BranchHierarchyData)
                    {
                        var tripleParentHierarchy =
                            readStructure.GetHierarchyData(parentHierarchy.parentHierarchyID);

                        if (tripleParentHierarchy is BranchHierarchyData)
                        {
                            levels.Add(branchShape.shapeID, 4);
                        }
                        else
                        {
                            levels.Add(branchShape.shapeID, 3);
                        }
                    }
                    else
                    {
                        levels.Add(branchShape.shapeID, 2);
                    }
                }
                else
                {
                    levels.Add(branchShape.shapeID, 1);
                }
            }

            var max = levels.Values.Max();

            foreach (var branchShape in shapes.branchShapes)
            {
                var level = levels[branchShape.shapeID];

                if (max == 4)
                {
                    branchShape.breakOffset = Mathf.Min(
                        branchShape.breakOffset,
                        level switch
                        {
                            4 => .2f,
                            3 => .4f,
                            2 => .6f,
                            _ => .8f
                        }
                    );
                }
                else if (max == 3)
                {
                    branchShape.breakOffset = Mathf.Min(
                        branchShape.breakOffset,
                        level switch
                        {
                            3 => .25f,
                            2 => .50f,
                            _ => .75f
                        }
                    );
                }
                else if (max == 2)
                {
                    branchShape.breakOffset = Mathf.Min(branchShape.breakOffset, level == 2 ? .33f : .66f);
                }
                else
                {
                    branchShape.breakOffset = Mathf.Min(branchShape.breakOffset, .5f);
                }
            }

            shapes.RecurseSplines(
                readStructure,
                data =>
                {
                    if ((data.barkParentShape != null) &&
                        (data.barkParentShape.breakOffset >= data.shape.offset))
                    {
                        data.shape.forcedInvisible = true;
                    }
                }
            );

            shapes.RecurseShapes(
                readStructure,
                data =>
                {
                    if (data.shape.type == TreeComponentType.Fruit)
                    {
                        data.shape.forcedInvisible = true;
                    }
                    else if (data.shape.type == TreeComponentType.Leaf)
                    {
                        var mat = inputMaterials[data.shape.hierarchyID];

                        if (!mat.eligibleForDeadTrees)
                        {
                            data.shape.forcedInvisible = true;
                        }

                        var parentShape = data.parentShape as BarkShapeData;
                        if (data.shape.offset >= parentShape.breakOffset)
                        {
                            data.shape.forcedInvisible = true;
                        }
                    }
                }
            );
        }

        private void Felled(IHierarchyRead readStructure, TreeVariantSettings settings)
        {
            Stump(readStructure, settings);

            shapes.RecurseShapes(
                readStructure,
                data =>
                {
                    if (data.type == TreeComponentType.Trunk)
                    {
                        var b = data.shape as BarkShapeData;
                        b.breakInverted = true;
                    }
                    else
                    {
                        data.shape.forcedInvisible = !data.shape.forcedInvisible;
                    }

                    if ((data.parentShape != null) && (data.parentShape.type == TreeComponentType.Trunk))
                    {
                        var parentTrunk = data.parentShape as BarkShapeData;

                        if (data.shape.offset < (parentTrunk.breakOffset + settings.trunkCutDeadZone))
                        {
                            data.shape.forcedInvisible = true;
                        }
                    }
                }
            );
        }

        private string GetFileStateString()
        {
            var pretty = GetPrettyStateString();
            return pretty.Replace(" ", "").Replace(",", "-");
        }

        private string GetPrettyStateString()
        {
            switch (stageType)
            {
                case StageType.Normal:
                    return "Normal";
                case StageType.Stump:
                    return "Stump";
                case StageType.StumpRotted:
                    return "Stump, Rotted";
                case StageType.Dead:
                    return "Dead";
                case StageType.DeadFelled:
                    return "Dead, Felled, Bare";
                case StageType.DeadFelledRotted:
                    return "Dead, Felled, Rotted";
                case StageType.Felled:
                    return "Felled";
                case StageType.FelledBare:
                    return "Felled, Bare";
                case StageType.FelledBareRotted:
                    return "Felled, Bare, Rotted";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /*public OdinMenuItem GetMenuItem(OdinMenuTree tree)
        {
            var item = new OdinMenuItem(tree, GetMenuString(), this) {Icon = GetIcon(true).icon};

            return item;
        }*/

        private void RemoveLOD(int i)
        {
            lods.RemoveAt(i);
        }

        private void Stump(IHierarchyRead readStructure, TreeVariantSettings settings)
        {
            var groundOffset = readStructure.GetVerticalOffset();

            var visibleHierarchies = new HashSet<int>();

            shapes.RecurseShapes(
                readStructure,
                data =>
                {
                    if (data.shape.type == TreeComponentType.Root)
                    {
                    }
                    else if (data.shape.type == TreeComponentType.Trunk)
                    {
                        var barkShape = data.shape as BarkShapeData;

                        var trunkLength = SplineModeler.GetApproximateLength(barkShape.spline);

                        var cutTime = (groundOffset + settings.trunkCutHeight) / trunkLength;

                        barkShape.breakOffset = cutTime;

                        visibleHierarchies.Add(data.hierarchy.hierarchyID);
                    }
                    else if (data.parentShape.type == TreeComponentType.Trunk)
                    {
                        var parentTrunk = data.parentShape as BarkShapeData;

                        if (data.shape.offset < (parentTrunk.breakOffset - settings.trunkCutDeadZone))
                        {
                            visibleHierarchies.Add(data.hierarchy.hierarchyID);

                            return;
                        }

                        data.shape.forcedInvisible = true;
                    }
                    else if (visibleHierarchies.Contains(data.hierarchy.parentHierarchyID))
                    {
                    }
                    else if (data.shape.type != TreeComponentType.Fungus)
                    {
                        data.shape.forcedInvisible = true;
                    }
                }
            );
        }

        #region IMenuItemProvider Members

        public TreeIcon GetIcon(bool enabled)
        {
            switch (stageType)
            {
                case StageType.Normal:
                    return enabled ? TreeIcons.tree : TreeIcons.disabledTree;
                case StageType.Stump:
                    return enabled ? TreeIcons.stump : TreeIcons.disabledStump;
                case StageType.StumpRotted:
                    return enabled ? TreeIcons.stumpRotted : TreeIcons.disabledStumpRotted;
                case StageType.Dead:
                    return enabled ? TreeIcons.dead : TreeIcons.disabledDead;
                case StageType.DeadFelled:
                    return enabled ? TreeIcons.deadFelled : TreeIcons.disabledDeadFelled;
                case StageType.DeadFelledRotted:
                    return enabled ? TreeIcons.deadFelledRotted : TreeIcons.disabledDeadFelledRotted;
                case StageType.Felled:
                    return enabled ? TreeIcons.felled : TreeIcons.disabledFelled;
                case StageType.FelledBare:
                    return enabled ? TreeIcons.felledBare : TreeIcons.disabledFelledBare;
                case StageType.FelledBareRotted:
                    return enabled ? TreeIcons.felledBareRotted : TreeIcons.disabledFelledBareRotted;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
