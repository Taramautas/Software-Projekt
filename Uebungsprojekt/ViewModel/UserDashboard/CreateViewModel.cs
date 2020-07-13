using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.UserDashboard
{
    public class CreateViewModel
    {
        public List<Models.ConnectorType> connectorTypes { get; set; }
        public List<Models.Location> location { get; set; }
        public List<Vehicle> vehicle { get; set; }
        public Booking booking { get; set; }

    }
}
