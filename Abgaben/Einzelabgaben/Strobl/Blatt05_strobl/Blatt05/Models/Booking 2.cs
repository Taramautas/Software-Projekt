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
        /// <summary>
        /// Enumeration for all Connector Types of chargers for electric cars
        /// </summary>
        public enum ConnectorType
        {
            Schuko_Socket,
            Type_1_Plug,
            Type_2_Plug,
            Chademo_Plug,
            Tesla_Supercharger,
            Ccs_Combo_2_Plug
        }

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

        /// <summary>Connector type </summary>
        [Required(ErrorMessage = "Please specify the wanted connector type.")]
        public ConnectorType Plug { get; set; }

        /// <summary>
        /// Empty constructor of booking model
        /// </summary>
        public Booking()
        { 

        }
    }
}
