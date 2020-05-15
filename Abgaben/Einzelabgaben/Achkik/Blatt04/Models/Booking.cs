using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Booking
    {
        [Required(ErrorMessage = "Bitte geben sie den Ladezustand ihres Autos an.")]
        [Range(0, 100)]
        public int charge { get; set; }
        [Required(ErrorMessage = "Bitte geben sie die benötigte Fahrtstrecke an.")]
        [Range(1, 1000)]
        public int needed_distance { get; set; }
        [Required(ErrorMessage = "Bitte geben sie die Startzeit des Ladens an.")]
        public DateTime start_time { get; set; }
        [Required(ErrorMessage = "Bitte geben sie die Endzeit des Ladens an.")]
        public DateTime end_time { get; set; }

        public Booking()
        { 

        }
    }
}
