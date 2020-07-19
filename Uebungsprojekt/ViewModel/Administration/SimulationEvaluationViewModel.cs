using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationEvaluationViewModel
    {
        public List<Location> locations { get; set; }
        public List<ChargingZone> charging_zones { get; set; }
        
        public List<ChargingColumn> charging_columns { get; set; }
        
        public SimulationResult result { get; set; }
    }
}