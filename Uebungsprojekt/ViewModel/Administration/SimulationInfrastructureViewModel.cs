using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationInfrastructureViewModel
    {
        /// <summary>List for displaying a list of all existing simulation infrastructures on infrastructure View</summary>
        public IEnumerable<SimulationInfrastructure> all_simulation_infrastructures { get; set; }
        
        /// <summary>Simulation configuration chosen for the current simulation</summary>
        public SimulationConfig simulation_config { get; set; }
    }
}