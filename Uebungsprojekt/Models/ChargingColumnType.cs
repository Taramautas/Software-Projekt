using System;
using System.Collections.Generic;

namespace Uebungsprojekt.Models
{
    public class ChargingColumnType
    {
        public int id { get; set; }
        public int max_concurrent_charging { get; set; }
        
        public string manufacturer_name { get; set; }

        public List<ConnectorType> connectors { get; set; }

    }
}