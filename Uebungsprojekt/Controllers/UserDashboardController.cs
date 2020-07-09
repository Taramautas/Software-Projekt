using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Controllers
{
    [Authorize(Roles = "Employee")]

    public class UserDashboardController : Controller
    {
        private readonly UserManager user_manager;
        private string user_id;
        public UserDashboardController(UserManager user_manager, IHttpContextAccessor http_context_accessor)
        {
            this.user_manager = user_manager;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// Displays the Create Booking View only on GET request
        /// </summary>
        /// <returns>
        /// Booking View
        /// </returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Booking());
        }

        //TODO Evaluate if this Class is the right place for the notifyUserOnChargingWindow() method declared in issue #22 
    }
}
