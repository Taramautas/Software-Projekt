using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Uebungsprojekt.Models.Booking;

namespace Uebungsprojekt.ViewModel
{
    public class ConnectorTypeEvaluationViewModel
    {
        public ConnectorType connectorType { get; set; }

        public int percentage { get; set; }

        public ConnectorTypeEvaluationViewModel() 
        {

        }
    }
}
