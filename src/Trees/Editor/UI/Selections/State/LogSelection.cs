using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class LogSelection : TSESelection<LogDataContainer, LogDataContainerSelection>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static LogSelection()
        {
            LogDataContainerSelection.InstanceAvailable += i => _logDataContainerSelection = i;
        }

        #region Static Fields and Autoproperties

        private static LogDataContainerSelection _logDataContainerSelection;

        #endregion

        public override LogDataContainerSelection selection
        {
            get
            {
                if (_selection == null)
                {
                    _selection = _logDataContainerSelection;
                }

                return _selection;
            }
        }
    }
}
