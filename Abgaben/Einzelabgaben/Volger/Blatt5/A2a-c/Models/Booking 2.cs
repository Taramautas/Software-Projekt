using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    /// <summary>
    /// Model representing one specific booking 
    /// </summary>
    public class Booking
    {
        /// <summary>Current State of Charge</summary>
        [Required(ErrorMessage = "Please specify the current state of charge.")]
        [Range(0, 100)]
        public int Charge { get; set; }

        /// <summary>Distance needed before next charging</summary>
        [Required(ErrorMessage = "Please specify the distance needed.")]
        [Range(1, 1000)]
        public int Needed_distance { get; set; }

        /// <summary>Preferred start datetime</summary>
        [Required(ErrorMessage = "Please specify the wanted start time.")]
        public DateTime Start_time { get; set; }

        /// <summary>Preferred end datetime</summary>
        [Required(ErrorMessage = "Please specify the wanted end time.")]
        public DateTime End_time { get; set; }

        /// <summary>
        /// Type of connector
        /// </summary>
        [Required]
        public ConnectorType Con_type { get; set; }

        /// <summary>
        /// Different types of connectors
        /// </summary>
        public enum ConnectorType
        {
            Schuko_Socket,
            Type_1_Plug,
            Type_2_Plug,
            CHAdeMO_Plug,
            Tesla_Supercharger,
            CCS_Combo_2_Plug
        }

        /// <summary>
        /// Empty constructor of booking model
        /// </summary>
        public Booking()
        { 

        }
    }
}
