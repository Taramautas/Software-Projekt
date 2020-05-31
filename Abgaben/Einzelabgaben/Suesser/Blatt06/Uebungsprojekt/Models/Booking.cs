using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

<<<<<<< HEAD
=======
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


>>>>>>> ef14370eec46d7e75c2d68be9936f6f5d4f87c1f
namespace Uebungsprojekt.Models
{
    /// <summary>
    /// Model representing one specific booking 
    /// </summary>
    public class Booking
    {
        /// <summary>Current State of Charge</summary>
        [Required(ErrorMessage = "Bitte geben sie den Ladezustand ihres Autos an.")]
        [Range(0, 100)]
        public int Charge { get; set; }

        /// <summary>Distance needed before next charging</summary>
        [Required(ErrorMessage = "Bitte geben sie die benötigte Fahrtstrecke an.")]
        [Range(1, 1000)]
        public int Needed_distance { get; set; }

        /// <summary>Preferred start datetime</summary>
        [Required(ErrorMessage = "Bitte geben sie die Startzeit des Ladens an.")]
        public DateTime Start_time { get; set; }

        /// <summary>Preferred end datetime</summary>
        [Required(ErrorMessage = "Bitte geben sie die Endzeit des Ladens an.")]
        public DateTime End_time { get; set; }

        /// <summary>
        /// Empty constructor of booking model
        /// </summary>
        public Booking()
        { 

        }

        /// <summary> Connector Type</summary>
        [Required(ErrorMessage = "Bitte geben sie einen Steckertyp an.")]
        public Steckertyp s_type { get; set; }


        /// <summary> Enumeration of all available Connector Types</summary>
<<<<<<< HEAD
        public enum Steckertyp
        {
            Schuko_Socket,
            Type_1_Plug,
            Type_2_Plug,
            CHAdeMO_Plug,
            Tesla_Supercharger,
            CCS_Combo_2_Plug
        }
    }
=======
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Steckertyp
        {
            [Display(Name = "Schuko Socket")]
            Schuko_Socket,
            [Display(Name = "Type_1_Plug")]
            Type_1_Plug,
            [Display(Name = "Type_2_Plug")]
            Type_2_Plug,
            [Display(Name = "CHAdeMO_Plug")]
            CHAdeMO_Plug,
            [Display(Name = "Tesla_Supercharger")]
            Tesla_Supercharger,
            [Display(Name = "CCS_Combo_2_Plug")]
            CCS_Combo_2_Plug
        }
    }

    
>>>>>>> ef14370eec46d7e75c2d68be9936f6f5d4f87c1f
}
