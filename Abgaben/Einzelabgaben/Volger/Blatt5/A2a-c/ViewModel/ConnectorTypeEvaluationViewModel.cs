using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    /// <summary>
    /// New Viewmodel for a connector with a percentage
    /// </summary>
    public class ConnectorTypeEvaluationViewModel
    {
        public Booking.ConnectorType Con_type { get; set; }

        public double Percentage { get; set; }

        public ConnectorTypeEvaluationViewModel() { }

    }
}
