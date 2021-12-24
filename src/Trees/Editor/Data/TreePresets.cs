using Appalachia.Simulation.Trees.Core;
using Appalachia.Simulation.Trees.Hierarchy.Options.Properties;
using Appalachia.Simulation.Trees.Hierarchy.Settings;
using Appalachia.Utility.Async;
using Appalachia.Utility.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Data
{
    public class TreePresets : SingletonAppalachiaTreeObject<TreePresets>, ISerializationCallbackReceiver
    {
        #region Fields and Autoproperties

        [FoldoutGroup("Trunk")] public DistributionSettings trunk_distribution;
        [FoldoutGroup("Trunk")] public BranchSettings trunk_branch;
        [FoldoutGroup("Trunk")] public CurvatureSettings trunk_curvature;
        [FoldoutGroup("Trunk")] public LimbSettings trunk_limb;

        [FoldoutGroup("Root")] public DistributionSettings root_distribution;
        [FoldoutGroup("Root")] public BranchSettings root_branch;
        [FoldoutGroup("Root")] public CollarSettings root_collar;
        [FoldoutGroup("Root")] public CurvatureSettings root_curvature;
        [FoldoutGroup("Root")] public LimbSettings root_limb;

        [FoldoutGroup("Branch")] public DistributionSettings branch_distribution;
        [FoldoutGroup("Branch")] public BranchSettings branch_branch;
        [FoldoutGroup("Branch")] public CollarSettings branch_collar;
        [FoldoutGroup("Branch")] public CurvatureSettings branch_curvature;
        [FoldoutGroup("Branch")] public LimbSettings branch_limb;

        [FoldoutGroup("Leaf")] public DistributionSettings leaf_distribution;

        [FoldoutGroup("Knot")] public DistributionSettings knot_distribution;

        [FoldoutGroup("Fungus")] public DistributionSettings fungus_distribution;

        [FoldoutGroup("Fruit")] public DistributionSettings fruit_distribution;

        #endregion

        #region Event Functions

        protected override async AppaTask WhenEnabled()
        {
            await base.WhenEnabled();
            Setup();
        }

        #endregion

        [Button]
        public void Setup()
        {
            trunk_branch.relativeToParent = false;
            branch_branch.relativeToParentAllowed = true;
            root_branch.relativeToParentAllowed = true;

            if (trunk_curvature.crookLikelihood == null)
            {
                trunk_curvature.crookLikelihood = TreeProperty.fCurve(0.0f, 1.0f, 1.0f);
            }

            if (root_curvature.crookLikelihood == null)
            {
                root_curvature.crookLikelihood = TreeProperty.fCurve(0.0f, 1.0f, 1.0f);
            }

            if (branch_curvature.crookLikelihood == null)
            {
                branch_curvature.crookLikelihood = TreeProperty.fCurve(0.0f, 1.0f, 1.0f);
            }

            if (trunk_curvature.crookAbruptness == null)
            {
                trunk_curvature.crookAbruptness = TreeProperty.New(.33f);
            }

            if (trunk_curvature.crookAbruptness == null)
            {
                trunk_curvature.crookAbruptness = TreeProperty.New(.33f);
            }

            if (branch_curvature.crookMemory == null)
            {
                branch_curvature.crookMemory = TreeProperty.New(.33f);
            }

            if (trunk_curvature.crookMemory == null)
            {
                trunk_curvature.crookMemory = TreeProperty.New(.33f);
            }

            if (trunk_curvature.crookMemory == null)
            {
                trunk_curvature.crookMemory = TreeProperty.New(.33f);
            }

            if (branch_curvature.crookMemory == null)
            {
                branch_curvature.crookMemory = TreeProperty.New(.33f);
            }
        }

        #region ISerializationCallbackReceiver Members

        public void OnBeforeSerialize()
        {
            using var scope = APPASERIALIZE.OnBeforeSerialize();
            Setup();
        }

        public void OnAfterDeserialize()
        {
            using var scope = APPASERIALIZE.OnAfterDeserialize();
            Setup();
        }

        #endregion
    }
}
