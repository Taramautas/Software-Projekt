using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class SimulationViewModel
    {
        public List<Location> locations { get; set; }
        public List<ChargingZone> charging_zones { get; set; }
        
        public SimulationResult result { get; set; }
    }
}