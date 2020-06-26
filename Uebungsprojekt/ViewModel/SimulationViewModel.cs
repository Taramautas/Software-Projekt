using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    public class SimulationViewModel
    {
        /// <summary>List for displaying a list of all existing simulation configs on configuration View</summary>
        public IEnumerable<SimulationConfig> all_simulation_configs { get; set; }
        
        /// <summary>List for displaying a list of all existing simulation infrastructures on infrastructure View</summary>
        public IEnumerable<SimulationInfrastructure> all_simulation_infrastructures { get; set; }
        
        /// <summary>Simulation configuration chosen for the current simulation</summary>
        public SimulationConfig simulation_config { get; set; }
        
        /// <summary>Simulation infrastructure chosen for the current simulation</summary>
        public SimulationInfrastructure simulation_infrastructure { get; set; }
    }
}