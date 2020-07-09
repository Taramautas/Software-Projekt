using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
        public int id { get; set; }

        public Boolean busy { get; set; }

        public string manufacturer_name { get; set; }

        public List<ConnectorType> connectors { get; set; }

        public Boolean emergency_reserve { get; set; }

        public int max_concurrent_charging { get; set; }

        public ChargingColumn()
        {

        }
    }
}
