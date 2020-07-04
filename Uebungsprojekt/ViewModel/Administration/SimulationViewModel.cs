using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationViewModel
    {
        /// <summary>Simulation infrastructure chosen for the current simulation</summary>
        public SimulationInfrastructure simulation_infrastructure { get; set; }
        
        /// <summary>Simulation configuration chosen for the current simulation</summary>
        public SimulationConfig simulation_config { get; set; }
    }
}