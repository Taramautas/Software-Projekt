using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class CreateChargingZoneViewModel
    {
        public List<Location> location { get; set; }

        public ChargingZone chargingzone { get; set; }

        public CreateChargingZoneViewModel(List<Location> loc, ChargingZone cha)
        {
            location = loc;
            chargingzone = cha;
        }
    }
}
