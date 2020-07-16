using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class SimulationInfrastructure
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Please enter the location dao id for this simulation infrastructure.")]
        public int location_dao_id { get; set; }
        
        [Required(ErrorMessage = "Please enter the charging zone dao id for this simulation infrastructure.")]
        public int charging_zone_dao_id { get; set; }
        
        [Required(ErrorMessage = "Please enter the charging column dao id for this simulation infrastructure.")]
        public int charging_column_dao_id { get; set; }
    }
}