using System.Collections.Generic;

namespace Uebungsprojekt.Models
{
    public class Vehicle
    {
        /// <summary>Name of the model</summary>
        public string model_name { get; set; }
        
        public int id { get; set; }
        
        /// <summary>Maximum battery capacity in kWh </summary>
        public double capacity { get; set; }
        
        /// <summary>List of available connector type</summary>
        public List<ConnectorType> connector_types { get; set; }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Vehicle()
        {
            
        }
    }
}