using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Vehicle
    {
        /// <summary>Name of the model</summary>
        [Required(ErrorMessage = "Please specify the name of the Vehicle")]
        public string model_name { get; set; }
        
        public int id { get; set; }
        
        /// <summary>Maximum battery capacity in kWh </summary>
        [Required(ErrorMessage = "Please specify the capacity of the Vehicle")]
        [Range(1,10000)]
        public int capacity { get; set; }
        
        /// <summary>List of available connector type</summary>
        [Required(ErrorMessage = "Please specify at least one ConnectorType")]
        public List<ConnectorType> connector_types { get; set; }

        public User user { get; set; }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Vehicle()
        {
            
        }
    }
}