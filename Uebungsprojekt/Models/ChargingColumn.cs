using System;
using System.Collections.Generic;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
        public int id { get; set; }
        
        public ChargingColumnType charging_column_type_id { get; set; }

        public Boolean busy { get; set; }

        public Boolean emergency_reserve { get; set; }
        
        public ChargingZone charging_zone { get; set; }

        public List<Tuple<DateTime,DateTime>> list { get; set; }

        public ChargingColumn()
        {
            list = new List<Tuple<DateTime, DateTime>>();
        }
    }
}
