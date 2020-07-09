using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        private readonly UserManager user_manager;
        private int user_id;
        private IMemoryCache cache;
        public UserDashboardController(UserManager user_manager, IHttpContextAccessor http_context_accessor, IMemoryCache cache)
        {
            this.cache = cache;
            this.user_manager = user_manager;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            List<Booking> bookings = booking_dao.GetAcceptedBookingsByUserId(user_id);
            bookings.AddRange(booking_dao.GetOpenBookingsByUserId(user_id));
            return View(bookings);
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

        [HttpGet]
        public IActionResult Infrastructure()
        {
            LocationDao location_dao = new LocationDaoImpl(cache);
            return View(location_dao.GetAll(0));
        }
    }
}
