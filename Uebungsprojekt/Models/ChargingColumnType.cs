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

        public bool Equals(ChargingColumnType cct)
        {
            if (!this.manufacturer_name.Equals(cct.manufacturer_name))
                return false;

            if (!this.model_name.Equals(cct.model_name))
                return false;

            if (!this.max_parallel_charging.Equals(cct.max_parallel_charging))
                return false;
            if (this.connectors.Count != cct.connectors.Count)
                return false;
            for (int i = 0; i < this.connectors.Count; i++)
            {
                if (!this.connectors[i].Item1.Equals(cct.connectors[i].Item1))
                    return false;
                if (!this.connectors[i].Item2.Equals(cct.connectors[i].Item2))
                    return false;
            }
            return true;
        }
    }
}