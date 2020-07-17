using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class AddRushHoursViewModel
    {
        public SimulationConfig config { get; set; }

        public AddRushHoursViewModel(SimulationConfig con)
        {
            config = con;
        }
    }
}
