using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    /// <summary>Helps with returning View with 3 Lists</summary>
    public class InfrastructureViewModel
    {
        public List<ChargingColumn> charging_columns { get; set; }
        public List<ChargingZone> charging_zones { get; set; }
        public List<Location> locations { get; set; }

    }
}
