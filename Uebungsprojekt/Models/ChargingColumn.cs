using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
        public int Id { get; set; }

        public Boolean Busy { get; set; }

        public string Manufacturer_name { get; set; }

        public Dictionary<ConnectorType, ConnectorType> Connectors { get; set; }

        public Boolean Emergency_reserve { get; set; }

        public int Max_concurrent_charging { get; set; }

        public ChargingColumn()
        {

        }
    }
}
