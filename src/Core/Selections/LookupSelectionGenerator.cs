using System;
using Appalachia.Core.Preferences;
using Appalachia.Core.Preferences.Globals;
using Appalachia.Simulation.Core.Metadata.Density;
using Appalachia.Simulation.Core.Metadata.Materials;
using UnityEngine;

namespace Appalachia.Simulation.Core.Selections
{
    public static class LookupSelectionGenerator
    {
        public static PhysicMaterialLookupSelection CreatePhysicMaterialSelector(
            Action<PhysicMaterialWrapper> select)
        {
            return ScriptableObject.CreateInstance<PhysicMaterialLookupSelection>()
                                   .Initialize(
                                        PhysicsMaterialsCollection.instance,
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
                                   .Initialize(
                                        PhysicsMaterialsCollection.instance,
                                        select,
                                        buttonColor,
                                        ColorPrefs.Instance.PhysicMaterialSelectorColorDrop
                                    );
        }

        public static DensityMetadataLookupSelection CreateDensityMetadataSelector(
            Action<DensityMetadata> select)
        {
            return ScriptableObject.CreateInstance<DensityMetadataLookupSelection>()
                                   .Initialize(
                                        DensityMetadataCollection.instance,
                                        select,
                                        ColorPrefs.Instance.DensitySelectorButton,
                                        ColorPrefs.Instance.DensitySelectorColorDrop
                                    );
        }

    }
}