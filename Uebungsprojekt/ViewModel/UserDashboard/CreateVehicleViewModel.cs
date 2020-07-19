using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.ViewModel.UserDashboard
{
    public class CreateVehicleViewModel
    {
        public Vehicle vehicle { get; set; }
        public List<User> users { get; set; }

        public CreateVehicleViewModel (Vehicle _vehicle, List<User> _users)
        {
            vehicle = _vehicle;
            users = _users;
        }

    }
}
