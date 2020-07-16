using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class AddSimulationVehicleViewModel
    {
        public List<Vehicle> vehicles { get; set; }

        public AddSimulationVehicleViewModel(List<Vehicle> veh)
        {
            vehicles = veh;
        }
    }
}
