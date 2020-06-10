﻿using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    /// <summary>
    /// ViewModel for evaluating the proportion of bookings with each specific connector type
    /// </summary>
    public class ConnectorTypeEvaluationViewModel
    {
        /// <summary> Connector type </summary>
        public Booking.ConnectorType ConnectorType { get; set; }

        /// <summary> Proportion of bookings using this connector type </summary>
        public double Proportion { get; set; }

        /// <summary>
        /// Empty constructor of ConnectorTypeEvaluation ViewModel
        /// </summary>
        /// <param name="connectorType">One of the possible connector types specified in the Booking.ConnectorType enum</param>
        /// <param name="proportion">The proportion of the total amount of bookings and used connector types</param>
        public ConnectorTypeEvaluationViewModel(Booking.ConnectorType connectorType, double proportion)
        {
            ConnectorType = connectorType;
            Proportion = proportion;
        }
    }
}
