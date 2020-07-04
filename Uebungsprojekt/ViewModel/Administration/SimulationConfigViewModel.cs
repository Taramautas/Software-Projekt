using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationConfigViewModel
    {
        /// <summary>List for displaying a list of all existing simulation configs on configuration View</summary>
        public IEnumerable<SimulationConfig> all_simulation_configs { get; set; }
    }
}