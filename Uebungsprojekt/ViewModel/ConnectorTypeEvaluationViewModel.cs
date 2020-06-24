using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    /// <summary>
    /// ViewModel for evaluating the proportion of bookings with each specific connector type
    /// </summary>
    public class ConnectorTypeEvaluationViewModel
    {
        /// <summary> Connector type </summary>
        [Required(ErrorMessage = "Please select at least one of the plug types.")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectorType ConnectorType { get; set; }

        /// <summary> Proportion of bookings using this connector type </summary>
        [Required(ErrorMessage = "Please specify the proportion of bookings using this connector type.")]
        public double Proportion { get; set; }

        /// <summary>
        /// Empty constructor of ConnectorTypeEvaluation ViewModel
        /// </summary>
        /// <param name="connectorTypeEnume of the possible connector types specified in the Booking.ConnectorType enum</param>
        /// <param name="proportion">The proportion of the total amount of bookings and used connector types</param>
        public ConnectorTypeEvaluationViewModel(ConnectorType connectorType, double proportion)
        {
            ConnectorType = connectorType;
            Proportion = proportion;
        }
    }
}
