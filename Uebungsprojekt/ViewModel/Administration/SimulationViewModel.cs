using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationViewModel
    {
        /// <summary>Simulation infrastructure chosen for the current simulation</summary>
        public int simulation_infrastructure_id { get; set; }
        
        /// <summary>Simulation configuration chosen for the current simulation</summary>
        public int simulation_config_id { get; set; }
    }
}