using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel
{
    public class ConnectorTypeEvaluationViewModel
    {
        public Booking.ConnectorType ConType { get; set; }
        public double Percentage { get; set; }

        public ConnectorTypeEvaluationViewModel()
        {
        }
    }

}
