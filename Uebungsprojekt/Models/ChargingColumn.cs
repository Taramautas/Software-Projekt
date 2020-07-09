using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uebungsprojekt.Models
{
    public class ChargingColumn
    {
<<<<<<< HEAD
        public int Id { get; set; }

        public Boolean Busy { get; set; }

        public string Manufacturer_name { get; set; }

        public Dictionary<ConnectorType, ConnectorType> Connectors { get; set; }

        public Boolean Emergency_reserve { get; set; }

        public int Max_concurrent_charging { get; set; }
=======
        public int id { get; set; }

        public Boolean busy { get; set; }

        public string manufacturer_name { get; set; }

        public List<ConnectorType> connectors { get; set; }

        public Boolean emergency_reserve { get; set; }

        public int max_concurrent_charging { get; set; }
>>>>>>> e213ca796f337ad62f1a805509f4fcc72d197655

        public ChargingColumn()
        {

        }
    }
}
