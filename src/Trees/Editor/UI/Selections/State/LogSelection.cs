using System;
using Appalachia.Simulation.Trees.UI.Selections.Dropdown;

namespace Appalachia.Simulation.Trees.UI.Selections.State
{
    [Serializable]
    public class LogSelection : TSESelection<LogDataContainer, LogDataContainerSelection>
    {
        public override LogDataContainerSelection selection 
        {
            get
            {
                if (_selection == null)
                {
                    _selection = LogDataContainerSelection.instance;
                }

                return _selection;
            }
            set { _selection = value; }
        }
    }
}