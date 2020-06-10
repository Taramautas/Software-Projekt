using Microsoft.AspNetCore.Mvc.Rendering;
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

        public enum ConnectorType
        {
            Schuko_Socket = 0,
            Type_1_Plug = 1,
            Type_2_Plug = 2,
            CHAdeMO_Plug = 3,
            Tesla_Supercharger =  4,
            CCS_Combo_2_Plug = 5
            
        }

        public ConnectorType connectorType { get; set; }

        public List<SelectListItem> connectortype { get; set; }
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
        /// constructer of Booking Model with adding the Connector Types in a special list to show them in the HTML file
        /// </summary>
        public Booking()
        {
            connectortype = new List<SelectListItem>();
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.Schuko_Socket.ToString(),


            });
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.Type_1_Plug.ToString(),

            });
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.Type_2_Plug.ToString(),

            });
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.CHAdeMO_Plug.ToString(),

            });
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.Tesla_Supercharger.ToString(),

            });
            connectortype.Add(new SelectListItem
            {
                Text = ConnectorType.CCS_Combo_2_Plug.ToString(),

            });

        }
    }
}
