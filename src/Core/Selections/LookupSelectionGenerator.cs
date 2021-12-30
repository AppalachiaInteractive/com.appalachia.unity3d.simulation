using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Preferences;
using Appalachia.Core.Preferences.Globals;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Materials;
using UnityEngine;

namespace Appalachia.Simulation.Core.Selections
{
    [CallStaticConstructorInEditor]
    public static class LookupSelectionGenerator
    {
        static LookupSelectionGenerator()
        {
            PhysicsMaterialsCollection.InstanceAvailable += i => _physicsMaterialsCollection = i;
            DensityMetadataCollection.InstanceAvailable += i => _densityMetadataCollection = i;
        }

        #region Static Fields and Autoproperties

        private static DensityMetadataCollection _densityMetadataCollection;

        private static PhysicsMaterialsCollection _physicsMaterialsCollection;

        #endregion

        public static DensityMetadataLookupSelection CreateDensityMetadataSelector(
            Action<DensityMetadata> select)
        {
            return ScriptableObject.CreateInstance<DensityMetadataLookupSelection>()
                                   .Prepare(
                                        _densityMetadataCollection,
                                        select,
                                        ColorPrefs.Instance.DensitySelectorButton,
                                        ColorPrefs.Instance.DensitySelectorColorDrop
                                    );
        }

        public static PhysicMaterialLookupSelection CreatePhysicMaterialSelector(
            Action<PhysicMaterialWrapper> select)
        {
            return ScriptableObject.CreateInstance<PhysicMaterialLookupSelection>()
                                   .Prepare(
                                        _physicsMaterialsCollection,
                                        select,
                                        ColorPrefs.Instance.PhysicMaterialSelectorButton,
                                        ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
                                    );
        }

        public static PhysicMaterialLookupSelection CreatePhysicMaterialSelector(
            Action<PhysicMaterialWrapper> select,
            PREF<Color> buttonColor)
        {
            return ScriptableObject.CreateInstance<PhysicMaterialLookupSelection>()
                                   .Prepare(
                                        _physicsMaterialsCollection,
                                        select,
                                        buttonColor,
                                        ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
                                    );
        }
    }
}
