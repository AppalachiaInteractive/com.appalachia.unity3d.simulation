using System;
using System.Text;
using Appalachia.CI.Constants;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.Generation.Assets
{
    public static class PrefabInspector
    {
        [NonSerialized] private static AppaContext _context;

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(PrefabInspector));
                }

                return _context;
            }
        }
        
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
            _builder.AppendLine(
                ZString.Format(
                    "[Asset Path]:                       {0}",
                    PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go)
                )
            );     
            _builder.AppendLine($"------------------------------------");
            _builder.AppendLine(
                ZString.Format(
                    "[GetPrefabAssetType]:               {0}",
                    PrefabUtility.GetPrefabAssetType(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[GetPrefabInstanceStatus]:          {0}",
                    PrefabUtility.GetPrefabInstanceStatus(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPrefabAssetMissing]:             {0}",
                    PrefabUtility.IsPrefabAssetMissing(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsAddedGameObjectOverride]:        {0}",
                    PrefabUtility.IsAddedGameObjectOverride(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsDisconnectedFromPrefabAsset]:    {0}",
                    PrefabUtility.IsDisconnectedFromPrefabAsset(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsOutermostPrefabInstanceRoot]:    {0}",
                    PrefabUtility.IsOutermostPrefabInstanceRoot(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsAnyPrefabInstanceRoot]:          {0}",
                    PrefabUtility.IsAnyPrefabInstanceRoot(go)
                )
            );
            _builder.AppendLine(
                ZString.Format("[IsPartOfAnyPrefab]:                {0}", PrefabUtility.IsPartOfAnyPrefab(go))
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfImmutablePrefab]:          {0}",
                    PrefabUtility.IsPartOfImmutablePrefab(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfModelPrefab]:              {0}",
                    PrefabUtility.IsPartOfModelPrefab(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfPrefabAsset]:              {0}",
                    PrefabUtility.IsPartOfPrefabAsset(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfPrefabInstance]:           {0}",
                    PrefabUtility.IsPartOfPrefabInstance(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfRegularPrefab]:            {0}",
                    PrefabUtility.IsPartOfRegularPrefab(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfVariantPrefab]:            {0}",
                    PrefabUtility.IsPartOfVariantPrefab(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfNonAssetPrefabInstance]:   {0}",
                    PrefabUtility.IsPartOfNonAssetPrefabInstance(go)
                )
            );
            _builder.AppendLine(
                ZString.Format(
                    "[IsPartOfPrefabThatCanBeAppliedTo]: {0}",
                    PrefabUtility.IsPartOfPrefabThatCanBeAppliedTo(go)
                )
            );

            Context.Log.Warn(_builder.ToString());
      
        }

    }
}
