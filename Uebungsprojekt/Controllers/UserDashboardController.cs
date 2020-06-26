using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;

namespace Uebungsprojekt.Controllers
{
    [Authorize(Roles = "Employee")]

    public class UserDashboardController : Controller
    {
        public UserDashboardController()
        {
            //TODO
        }

        //TODO Evaluate if this Class is the right place for the notifyUserOnChargingWindow() method declared in issue #22 
    }
}
