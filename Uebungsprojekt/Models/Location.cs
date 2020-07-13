using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Location
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please specify the city this location is located in.")]
        public string city { get; set; }

        [Required(ErrorMessage = "Please enter the post code.")]
        public string post_code { get; set; }

        [Required(ErrorMessage = "Please enter the address.")]
        public string address { get; set; }
    }
}
