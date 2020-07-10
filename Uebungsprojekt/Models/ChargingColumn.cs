using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
        public int id { get; set; }
        
        public int charging_column_type_id { get; set; }

        public Boolean busy { get; set; }

        public Boolean emergency_reserve { get; set; }
        
        public ChargingZone charging_zone { get; set; }

        public ChargingColumn()
        {

        }
    }
}
