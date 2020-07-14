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
        
        [Required(ErrorMessage = "Please specify the maximum concurrent charging power.")]
        [Range(1, 100)]
        public double max_concurrent_charging { get; set; }

        [Required(ErrorMessage = "Please select the connector types available at this column (multiple of same type allowed).")]
        public List<ConnectorType> connectors { get; set; }

    }
}