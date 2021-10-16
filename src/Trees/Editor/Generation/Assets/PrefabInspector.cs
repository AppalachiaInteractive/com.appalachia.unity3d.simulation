using System.Text;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    public static class PrefabInspector
    {
        private static StringBuilder _builder = new StringBuilder();
        
        public static void Inspect(GameObject go)
        {
            if (_builder == null)
            {
                _builder = new StringBuilder();
            }

            _builder.Clear();
            _builder.AppendLine($"------------------------------------");
            _builder.AppendLine(go.name);
            _builder.AppendLine($"------------------------------------");
            _builder.AppendLine($"[Asset Path]:                       {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go)}");     
            _builder.AppendLine($"------------------------------------");
            _builder.AppendLine($"[GetPrefabAssetType]:               {PrefabUtility.GetPrefabAssetType(go)}");            
            _builder.AppendLine($"[GetPrefabInstanceStatus]:          {PrefabUtility.GetPrefabInstanceStatus(go)}");
            _builder.AppendLine($"[IsPrefabAssetMissing]:             {PrefabUtility.IsPrefabAssetMissing(go)}");
            _builder.AppendLine($"[IsAddedGameObjectOverride]:        {PrefabUtility.IsAddedGameObjectOverride(go)}");
            _builder.AppendLine($"[IsDisconnectedFromPrefabAsset]:    {PrefabUtility.IsDisconnectedFromPrefabAsset(go)}");
            _builder.AppendLine($"[IsOutermostPrefabInstanceRoot]:    {PrefabUtility.IsOutermostPrefabInstanceRoot(go)}");
            _builder.AppendLine($"[IsAnyPrefabInstanceRoot]:          {PrefabUtility.IsAnyPrefabInstanceRoot(go)}");
            _builder.AppendLine($"[IsPartOfAnyPrefab]:                {PrefabUtility.IsPartOfAnyPrefab(go)}");
            _builder.AppendLine($"[IsPartOfImmutablePrefab]:          {PrefabUtility.IsPartOfImmutablePrefab(go)}");
            _builder.AppendLine($"[IsPartOfModelPrefab]:              {PrefabUtility.IsPartOfModelPrefab(go)}");
            _builder.AppendLine($"[IsPartOfPrefabAsset]:              {PrefabUtility.IsPartOfPrefabAsset(go)}");
            _builder.AppendLine($"[IsPartOfPrefabInstance]:           {PrefabUtility.IsPartOfPrefabInstance(go)}");
            _builder.AppendLine($"[IsPartOfRegularPrefab]:            {PrefabUtility.IsPartOfRegularPrefab(go)}");
            _builder.AppendLine($"[IsPartOfVariantPrefab]:            {PrefabUtility.IsPartOfVariantPrefab(go)}");
            _builder.AppendLine($"[IsPartOfNonAssetPrefabInstance]:   {PrefabUtility.IsPartOfNonAssetPrefabInstance(go)}");
            _builder.AppendLine($"[IsPartOfPrefabThatCanBeAppliedTo]: {PrefabUtility.IsPartOfPrefabThatCanBeAppliedTo(go)}");

            Debug.LogWarning(_builder.ToString());
      
        }

    }
}
