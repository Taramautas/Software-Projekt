using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    /// <summary>
    /// Controller for Booking model
    /// </summary>
    public class BookingController : Controller
    {
        private IMemoryCache _cache;

        List<Booking> bookingList = new List<Booking>();

        /// <summary>
        /// Constructor of controller. Staticly initialize some booking instances and memory cache
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for intializing the memory cache</param>
        public BookingController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

           /* bookingList.Add(
                new Booking()
                {
                    Charge = 20,
                    Needed_distance = 46,
                    Start_time = new DateTime(2020, 12 , 15, 4, 30, 45),
                    End_time = new DateTime(2020, 12, 15, 5, 30, 45),
                });

            bookingList.Add(
                new Booking()
                {
                    Charge = 40,
                    Needed_distance = 78,
                    Start_time = new DateTime(2020, 12, 15, 2, 0, 0),
                    End_time = new DateTime(2020, 12, 15, 3, 0, 0),
                });

            bookingList.Add(
                new Booking()
                {
                    Charge = 100,
                    Needed_distance = 94,
                    Start_time = new DateTime(2020, 12, 14, 12, 0, 0),
                    End_time = new DateTime(2020, 12, 15, 0, 0, 0),
                });

            bookingList.Add(
                new Booking()
                {
                    Charge = 45,
                    Needed_distance = 100,
                    Start_time = new DateTime(2020, 12, 15, 8, 30, 0),
                    End_time = new DateTime(2020, 12, 15, 16, 30, 0),
                });*/

        }       
        /// <summary>
        /// Displays the booking View and passes the booking list initialized in the constructor as well as the booking in the cache, if one exists
        /// </summary>
        /// <returns>
        /// The booking View displaying the list of bookings
        /// </returns>
        public IActionResult Index()
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> created_bookings))
            {
                bookingList.AddRange(created_bookings);
            }
            return View(bookingList);
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

        /// <summary>
        /// Recieves a booking request from the Create Form, checks if the data is valid and inserts it into the cache
        /// </summary>
        /// <param name="booking">The booking to be inserted in the cache and the bookings table</param>
        /// <returns>
        /// Returns the booking View on success (valid booking object) and remains on create View on failure
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            // Server side validation
            if (ModelState.IsValid)
            {
                if (_cache.TryGetValue("CreateBooking", out List<Booking> created_bookings))
                {
                    created_bookings.Add(booking);
                } 
                else
                {
                    created_bookings = new List<Booking>();
                    created_bookings.Add(booking);
                    _cache.Set("CreateBooking", created_bookings);
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
