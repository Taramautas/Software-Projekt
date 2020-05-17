using System;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    public class Booking
    {
        /*Referenz zu den Validate-Attributen:
        https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-3.1
        */
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

        public Steckertyp s_type { get; set; }

        public enum Steckertyp 
        {
            Schuko_Socket,
            Type_1_Plug,
            Type_2_Plug,
            CHAdeMO_Plug,
            Tesla_Supercharger,
            CCS_Combo_2_Plug
        }


        public Booking()
        { 

        }
    }
}
