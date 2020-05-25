using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Uebungsprojekt.Models
{
    /// <summary>
    /// Model representing one specific booking 
    /// </summary>
    public class Booking
    {
        /// <summary>All possible connector types</summary>
        public enum ConnectorType
        {
            [Display(Name = "Schuko Socket")]
            Schuko_Socket,
            [Display(Name = "Type 1 Plug")]
            Type_1_Plug,
            [Display(Name = "Type 2 Plug")]
            Type_2_Plug,
            [Display(Name = "CHAdeMO Plug")]
            CHAdeMO_Plug,
            [Display(Name = "Tesla Supercharger")]
            Tesla_Supercharger,
            [Display(Name = "CCS Combo 2 Plug")]
            CCS_Combo_2_Plug
        };

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

        /// <summary>Connector type for charging</summary>
        [Required(ErrorMessage = "Please select at least one of the plug types.")]
        public ConnectorType Connector_Type { get; set; }

        /// <summary>
        /// Empty constructor of booking model
        /// </summary>
        public Booking()
        { 

        }
    }
}
