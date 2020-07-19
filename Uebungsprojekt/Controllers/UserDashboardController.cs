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
            if(user_dao.GetById(user_id).role == Role.Assistant)
            {
                var cvwa = new CreateViewModel(location_dao.GetAll(0), vehicle_dao.GetAll(), new Booking(), user_dao.GetAll());
                return View(cvwa);
            }
            var cvw = new CreateViewModel(location_dao.GetAll(0), vehicle_dao.GetVehiclesByUserId(user_id), new Booking(), user_dao.GetAll());
            return View(cvw);
        }

        /// <summary>
        /// Add booking to DAO if valid and return to Bookings
        /// </summary>
        /// <param name="booking">Booking</param>
        [HttpPost]
        public IActionResult Create(Booking booking, int vehicle_id, int location_id)
        {
            int usr_id;
            UserDao user_dao = new UserDaoImpl(cache);
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            LocationDao location_dao = new LocationDaoImpl(cache);
            BookingDao booking_dao = new BookingDaoImpl(cache);
            //TODO: Make it beautiful :) 
            if (location_id == 0 || vehicle_id == 0) return RedirectToAction("Create");
            if (user_dao.GetById(user_id).role == Role.Assistant)
            {
                usr_id = vehicle_dao.GetById(vehicle_id).user.id;
            }
            else
            {
                usr_id = user_id;
            }
            booking_dao.Create(
                booking.start_state_of_charge,
                booking.target_state_of_charge,
                booking.start_time,
                booking.end_time,
                vehicle_dao.GetById(vehicle_id),
                user_dao.GetById(usr_id),
                location_dao.GetById(location_id, 0),
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


        [HttpGet, ActionName("CheckIn")]
        public ActionResult CheckIn(int id)
        {
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            Booking b = booking_dao.GetById(id, 0);
            b.checkedIn = 1;
            return RedirectToAction("Bookings");
        }

        [HttpPost, ActionName("CheckIn")]
        public ActionResult CheckInConfirmed(int id)
        {
            return RedirectToAction("Bookings");
        }

        [HttpGet, ActionName("CheckOut")]
        public ActionResult CheckOut(int id)
        {
            BookingDaoImpl booking_dao = new BookingDaoImpl(cache);
            Booking b = booking_dao.GetById(id, 0);
            b.checkedIn = 2;
            return RedirectToAction("Bookings");
        }

        [HttpPost, ActionName("CheckOut")]
        public ActionResult CheckOutConfirmed(int id)
        {
            return RedirectToAction("Bookings");
        }

        /// <summary>
        /// Display a table of all vehicles from user
        /// </summary>
        [HttpGet]
        public IActionResult Vehicles()
        {
            UserDao userDao = new UserDaoImpl(cache);
            VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
            if (userDao.GetById(user_id).role == Role.Assistant)
            {
                return View(vehicle_dao.GetAll());
            }
            return View(vehicle_dao.GetVehiclesByUserId(user_id));
        }

        /// <summary>
        /// Show Create form for vehicle
        /// </summary>
        [HttpGet]
        public IActionResult CreateVehicle()
        {
            UserDao user_dao = new UserDaoImpl(cache);
            var cvvm = new CreateVehicleViewModel(new Vehicle(), user_dao.GetAll());
            return View(cvvm);
            // eventuell muss hier eine Liste nur mit den vehicles vom eingeloggten user zurückgegeben werden
        }

        /// <summary>
        /// Add vehicle to DAO if valid and return to Vehicles
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        [HttpPost]
        public IActionResult CreateVehicle(Vehicle vehicle, int eindeutige_benutzernummer)
        {
            //When vehcicle has no connectortypes selected -> redirect to CreateVehicle
            if (ModelState.IsValid && !ReferenceEquals(vehicle.connector_types, null))
            {
                int usr_id;
                if (eindeutige_benutzernummer == 0)
                {
                    usr_id = user_id;
                }
                else
                {
                    usr_id = eindeutige_benutzernummer;
                }
                VehicleDao vehicle_dao = new VehicleDaoImpl(cache);
                UserDao user_dao = new UserDaoImpl(cache);
                vehicle_dao.Create(vehicle.model_name, vehicle.capacity, vehicle.connector_types, user_dao.GetById(usr_id));
                return RedirectToAction("Vehicles");
            }
            return RedirectToAction("CreateVehicle");
        }
        
        [HttpGet, ActionName("DeleteVehicle")]
        public ActionResult DeleteVehicle(int id)
        {
            VehicleDaoImpl vehicleDao = new VehicleDaoImpl(cache);
            vehicleDao.Delete(id);
            return RedirectToAction("Vehicles");
        }

        [HttpPost, ActionName("DeleteVehicle")]
        public ActionResult DeleteVehicleConfirmed(int id)
        {
            return RedirectToAction("Vehicles");
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
