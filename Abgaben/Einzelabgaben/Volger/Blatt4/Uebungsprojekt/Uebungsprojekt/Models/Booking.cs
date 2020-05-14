using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Booking
    {
        [Required]
        [Range (0, 100)]
        //Versuch einer Clientseitigen Validierung, macht aber anscheinend nichts
        //[Remote(action:"VerifyCharge", controller:"Booking")]
        public int charge { get; set; }

        [Required]
        [Range (1, 1000)]
        public int needed_distance { get; set; }

        [Required]
        public DateTime start_time { get; set; }

        [Required]
        public DateTime end_time { get; set; }

        public Booking()
        { 

        }

    }
}
