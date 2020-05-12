using System;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Booking
    {
        [Required]
        [Range(0, 100)]
        public int charge { get; set; }

        [Required]
        [Range(1, 1000)]
        public int needed_distance { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime start_time { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime end_time { get; set; }

        public Booking()
        { 

        }
    }
}
