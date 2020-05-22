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

        List<Booking> bookingList;

        /// <summary>
        /// Constructor of controller. Staticly initialize some booking instances and memory cache
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for intializing the memory cache</param>
        public BookingController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            if (!_cache.TryGetValue("booking", out bookingList))
            {
                bookingList = new List<Booking>();
                _cache.Set("booking", bookingList);
            }

        }       
        /// <summary>
        /// Displays the booking View and passes the booking list initialized in the constructor as well as the booking in the cache, if one exists
        /// </summary>
        /// <returns>
        /// The booking View displaying the list of bookings
        /// </returns>
        public IActionResult Index()
        {
            return View(_cache.Get("booking"));
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
            // return View(Enum.GetValues(typeof(Booking.Steckertyp)));
            return View();
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
            if (ModelState.IsValid)
            {
                //bookingList aus Cache laden
                bookingList = (List<Booking>)_cache.Get("booking");
                //erstellte Booking Objekt hinzufügen
                bookingList.Add(booking);
                //Zurück in den Cache speichern
            //    _cache.Set("booking", bookingList);
                //Öffnet die Buchungswebsite
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
