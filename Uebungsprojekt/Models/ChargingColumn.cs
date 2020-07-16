using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Please select one of the available charging column types.")]
        public ChargingColumnType charging_column_type_id { get; set; }

        public Boolean busy { get; set; }

        public Boolean emergency_reserve { get; set; }
        
        [Required(ErrorMessage = "Please specify in which charging zone this column is located.")]
        public ChargingZone charging_zone { get; set; }

        public List<Tuple<DateTime,DateTime>> list { get; set; }

        public ChargingColumn()
        {
            list = new List<Tuple<DateTime, DateTime>>();
        }
    }
}
