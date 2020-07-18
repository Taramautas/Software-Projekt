using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class ChargingColumnType
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Please enter the name of the charging column.")]
        public string model_name { get; set; }
        
        [Required(ErrorMessage = "Please enter the manufacturer name of the charging column.")]
        public string manufacturer_name { get; set; }
        
        [Required(ErrorMessage = "Please specify the maximum of parallel chargable vehicles.")]
        [Range(1, 4)]
        public int max_parallel_charging { get; set; }

        // Connectortypes and their charging capacity
        //[Required(ErrorMessage = "Please select the connector types available at this column (multiple of same type allowed).")]
        //public List<Tuple<ConnectorType, int>> connectors { get; set; }
        [Required(ErrorMessage = "Please select the connector types available at this column (multiple of same type allowed).")]
        public List<Tuple<ConnectorType, int>> connectors { get; set; }

    }
}