using System;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    [CallStaticConstructorInEditor]
    public class LogSelection : TSESelection<LogDataContainer, LogDataContainerSelection>
    {
        static LogSelection()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<LogDataContainerSelection>()
                                     .IsAvailableThen(i => _logDataContainerSelection = i);
        }

        #region Static Fields and Autoproperties

        private static LogDataContainerSelection _logDataContainerSelection;

        #endregion

        /// <inheritdoc />
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
