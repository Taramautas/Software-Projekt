using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    /// <summary>
    /// New Viewmodel for a connector with a percentage value of using this in bookings
    /// </summary>
    public class ConnectorTypeEvaluationViewModel
    {
        public Booking.ConnectorType connectorType { get; set; }

        public double Percentage { get; set; }

        public ConnectorTypeEvaluationViewModel() { }

    }
}
