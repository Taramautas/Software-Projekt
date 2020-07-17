using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.DAO;
using Uebungsprojekt.Models;
using Uebungsprojekt.ViewModel.Administration;
using Uebungsprojekt.ViewModel.UserDashboard;
using System.Runtime.InteropServices;

namespace Uebungsprojekt.Controllers
{
    // Controller is only accessible for authorized (logged in) users
    [Authorize]
    public class UserDashboardController : Controller
    {
        private int user_id;
        private IMemoryCache cache;
        
        /// <summary>
        /// Contructor for UserDashboardController
        /// </summary>
        /// <param name="user_manager">UserManager passed via Dependency Injection</param>
        /// <param name="http_context_accessor">IHttpContextAccessor passed via Dependency Injection</param>
        /// <param name="cache">IMemoryCache passed via Dependency Injection</param>
        public UserDashboardController(UserManager user_manager, IHttpContextAccessor http_context_accessor, IMemoryCache cache)
        {
            this.cache = cache;
            user_id = user_manager.GetUserIdByHttpContext(http_context_accessor.HttpContext);
        }
        
        /// <summary>
        /// Static description of all possible actions within the userdashboard controller
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Display a table of all bookings associated to the current user
        /// </summary>
        public IActionResult Bookings()
        {
            BookingDao booking_dao = new BookingDaoImpl(cache);
            List<Booking> bookings = booking_dao.GetAcceptedBookingsByUserId(user_id);
            UserDaoImpl user_dao = new UserDaoImpl(cache);
            if (user_dao.GetById(user_id).role == Role.Assistant)
                return View(booking_dao.GetAll(0));
            bookings.AddRange(booking_dao.GetOpenBookingsByUserId(user_id));
            return View(bookings);
        }
        
        /// <summary>
        /// Show Create form for booking
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            LocationDao location_dao = new LocationDaoImpl(cache);
            UserDao user_dao = new UserDaoImpl(cache);
            var cvw = new CreateViewModel(location_dao.GetAll(0), vehicle_dao.GetAll(), new Booking(), user_dao.GetAll());
            return View(cvw);
        }

        /// <summary>
        /// Add booking to DAO if valid and return to Bookings
        /// </summary>
        /// <param name="booking">Booking</param>
        [HttpPost]
        public IActionResult Create(Booking booking, int vehicle_id, int location_id, int eindeutige_benutzernummer)
        {
            int usr_id;
            //TODO: Make it beautiful :) 
            if (location_id == 0 || vehicle_id == 0) return RedirectToAction("Create");
            if (eindeutige_benutzernummer == 0)
            {
                usr_id = user_id;
            }
            else
            {
                usr_id = eindeutige_benutzernummer;
            }
            LocationDaoImpl location_dao = new LocationDaoImpl(cache);
            VehicleDaoImpl vehicle_dao = new VehicleDaoImpl(cache);
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            UserDaoImpl user_dao = new UserDaoImpl(cache);
            booking_dao.Create(
                booking.start_state_of_charge,
                booking.target_state_of_charge,
                booking.start_time,
                booking.end_time,
                vehicle_dao.GetById(vehicle_id),
                user_dao.GetById(usr_id),
                location_dao.GetById(location_id,0),
                0
                );
            return RedirectToAction("Bookings");
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            booking_dao.Delete(id, 0);
            return RedirectToAction("Bookings");
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            return RedirectToAction("Bookings");
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
        
        /// <summary>
        /// Display a table of all users in system
        /// </summary>
        [Authorize(Roles = "Assistant")]
        [HttpGet]
        public IActionResult Users()
        {
            UserDao user_dao = new UserDaoImpl(cache);
            return View(user_dao.GetAll());
        }
        
        /// <summary>
        /// Show Create form for User
        /// </summary>
        [Authorize(Roles = "Assistant")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new User());
        }
        
        /// <summary>
        /// Add user to DAO if valid and return to Users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "Assistant")]
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                UserDao user_dao = new UserDaoImpl(cache);
                user_dao.Create(user.name, user.email, user.password, user.role);
                return RedirectToAction("Users");
            }
            return RedirectToAction("Users");
        }

        [HttpGet, ActionName("DeleteUser")]
        public ActionResult DeleteUser(int id)
        {
            UserDaoImpl userDao = new UserDaoImpl(cache);
            userDao.Delete(id);
            return RedirectToAction("Users");
        }

        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteUserConfirmed(int id)
        {
            return RedirectToAction("Users");
        }
    }
}
