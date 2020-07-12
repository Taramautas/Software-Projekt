using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel.Administration;

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

        [HttpPost]
        public IActionResult Create(Booking book)
        {
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            UserDaoImpl user_dao = new UserDaoImpl(cache);
/*            booking_dao.Create(
                book.start_state_of_charge,
                book.target_state_of_charge,
                book.start_time,
                book.end_time,
                false,
                _vehicle,
                user_dao.GetById(user_id),
                book.charging_column,
                0
                );
*/  
            //TODO: finish this shit
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Infrastructure()
        {
            // Create Daos for Infrastructure
            LocationDao location_dao = new LocationDaoImpl(cache);
            ChargingZoneDao chargingzone_dao = new ChargingZoneDaoImpl(cache);
            ChargingColumnDao chargingcolumn_dao = new ChargingColumnDaoImpl(cache);
            // Create ViewModel and set required daos
            InfrastructureViewModel view_model = new InfrastructureViewModel();
            List<Location> location = location_dao.GetAll(0);
            List<ChargingZone> chargingzone = chargingzone_dao.GetAll(0);
            List<ChargingColumn> chargingcolumn = chargingcolumn_dao.GetAll(0);
            view_model.locations = location;
            view_model.charging_zones = chargingzone;
            view_model.charging_columns = chargingcolumn;
            return View(view_model);
        }
    }
}
