using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationInfrastructureViewModel
    {
        /// <summary>List for displaying a list of all existing simulation infrastructures on infrastructure View</summary>
        public IEnumerable<SimulationInfrastructure> all_simulation_infrastructures { get; set; }
        
        public List<ChargingColumnType> charging_column_types { get; set; }
        
        public List<ChargingColumn> charging_columns { get; set; }
        public List<ChargingZone> charging_zones { get; set; }
        public List<Location> locations { get; set; }

        public int simulation_infrastructure_id { get; set; }
    }
}