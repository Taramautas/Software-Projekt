using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class ChargingZone
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please define the maximum charging power of this charging zone.")]
        public int overall_performance { get; set; }

        [Required(ErrorMessage = "Please select the location this zone is located in.")]
        public Location location { get; set; }

        public string name { get; set; }
    }
}
