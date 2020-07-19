using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.Administration
{
    public class AddChargingColumnViewModel
    {
        public List<ChargingZone> chargingZones { get; set; }

        public List<ChargingColumnType> chargingColumnTypes { get; set; }

        public ChargingColumn chargingColumn { get; set; }

        public AddChargingColumnViewModel(List<ChargingZone> cz, List<ChargingColumnType> cct, ChargingColumn cc)
        {
            chargingZones = cz;
            chargingColumnTypes = cct;
            chargingColumn = cc;
        }
    }
}
