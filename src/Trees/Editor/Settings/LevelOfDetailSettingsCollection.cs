using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Simulation.Trees.Core.Settings;
using Appalachia.Simulation.Trees.ResponsiveUI;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Appalachia.Simulation.Trees.Settings
{
    [Serializable]
    [Title("Levels Of Detail & Quality", TitleAlignment = TitleAlignments.Centered)]
    public class LevelOfDetailSettingsCollection : ResponsiveSettings
    {
        [PropertyTooltip("The levels of detail to use when rendering this tree.")]
        [ListDrawerSettings(AddCopiesLastElement = false,
            AlwaysAddDefaultValue = false,
            CustomAddFunction = nameof(AddLevelOfDetailSetting),
            Expanded = false, 
            DraggableItems = false)]
        public List<LevelOfDetailSettings> levelsOfDetail;

        [BoxGroup("Impostor Settings"), InlineProperty, HideLabel, PropertyOrder(-100)]
        public ImpostorSettings impostor;

        [PropertyTooltip("Should a shadow caster mesh be created?."), PropertyOrder(-99)]
        public bool shadowCaster;

        public LevelOfDetailSettings this[int index] => levelsOfDetail[index];

        public int levels => levelsOfDetail.Count;

        public void SetIndices()
        {
            if (impostor == null)
            {
                impostor = new ImpostorSettings(settingsType);
            }
            
            for (var i = 0; i < levelsOfDetail.Count; i++)
            {
                levelsOfDetail[i].level = i;
            }
        }

        private LevelOfDetailSettings AddLevelOfDetailSetting()
        {
            var newLOD = new LevelOfDetailSettings(levelsOfDetail.Count, settingsType);
            
            if (levelsOfDetail.Count == 0)
            {
                
            }
            else
            {
                var last = levelsOfDetail[levelsOfDetail.Count - 1];

                newLOD.leafFullness = last.leafFullness * .75f;
                newLOD.showFruit = last.showFruit;
                newLOD.showKnots = last.showKnots;
                newLOD.branchesGeometryQuality = last.branchesGeometryQuality * .5f;
                newLOD.leafGeometryQuality = last.leafGeometryQuality * .5f;
                newLOD.rootsGeometryQuality = last.rootsGeometryQuality * .5f;
                newLOD.trunkGeometryQuality = last.trunkGeometryQuality * .5f;
                newLOD.doubleSidedLeafGeometry = last.doubleSidedLeafGeometry;
            }

            return newLOD;
        }

        public LevelOfDetailSettingsCollection(ResponsiveSettingsType settingsType) : base(settingsType)
        {
            impostor = new ImpostorSettings(settingsType);
            levelsOfDetail = new List<LevelOfDetailSettings>();

            if (settingsType == ResponsiveSettingsType.Tree)
            {
                
                var lod0 = new LevelOfDetailSettings(0, settingsType);
                
                var lod1 = new LevelOfDetailSettings(1, settingsType)
                {
                    branchesGeometryQuality = .5f,
                    leafGeometryQuality = .5f,
                    leafFullness = 1f,
                    rootsGeometryQuality = .5f,
                    trunkGeometryQuality = .75f,
                    resampleAddOns = false,
                    screenRelativeTransitionHeight = .70f,
                };

                var lod2 = new LevelOfDetailSettings(2, settingsType)
                {
                    branchesGeometryQuality = 0f,
                    leafGeometryQuality = .25f,
                    leafFullness = 1f,
                    rootsGeometryQuality = 0f,
                    showFruit = false,
                    trunkGeometryQuality = 0f,
                    resampleSplineAtChildren = false,
                    screenRelativeTransitionHeight = .03f,
                };

                levelsOfDetail.Add(lod0);
                levelsOfDetail.Add(lod1);
                levelsOfDetail.Add(lod2);
            }
            else if (settingsType == ResponsiveSettingsType.Branch)
            {
                var lod0 = new LevelOfDetailSettings(0, settingsType);                

                levelsOfDetail.Add(lod0);
            }
            else
            {
                var lod0 = new LevelOfDetailSettings(0, settingsType);

                var lod1 = new LevelOfDetailSettings(1, settingsType)
                {
                    branchesGeometryQuality = 0f,
                    leafGeometryQuality = 0f,
                    leafFullness = 0f,
                    rootsGeometryQuality = 0f,
                    showFruit = false,
                    trunkGeometryQuality = 0f,
                    resampleSplineAtChildren = false,
                    screenRelativeTransitionHeight = .3f,
                };

                levelsOfDetail.Add(lod0);
                levelsOfDetail.Add(lod1);
            }
        }

        public override void CopySettingsTo(ResponsiveSettings t)
        {
            if (t is LevelOfDetailSettingsCollection cast)
            {
                impostor.CopySettingsTo(cast.impostor);
                
                cast.levelsOfDetail.Clear();
                
                for (var i = 0; i < levelsOfDetail.Count; i++)
                {
                    var newLOD = new LevelOfDetailSettings(i, settingsType);

                    levelsOfDetail[i].CopySettingsTo(newLOD);

                    cast.levelsOfDetail.Add(newLOD);
                }
            }
        }
        
        [Button]
        public void PushToAll()
        { 
            var trees = AssetDatabaseManager.FindAssets("t:TreeDataContainer");

            for (var i = 0; i < trees.Length; i++)
            {
                var treeGuid = trees[i];

                var treePath = AssetDatabaseManager.GUIDToAssetPath(treeGuid);

                var tree = AssetDatabaseManager.LoadAssetAtPath<TreeDataContainer>(treePath);
                
                var settings = tree.settings.lod;

                if (settings == this)
                {
                    continue;
                }

                CopySettingsTo(settings);

                EditorUtility.SetDirty(tree);
                EditorUtility.SetDirty(tree.settings);
                
                tree.Save();
            }
        }
    }
}
