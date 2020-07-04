using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Uebungsprojekt.Impl;

namespace Uebungsprojekt.Controllers
{
    /// <summary>
    /// Controller for Booking model
    /// </summary>
    public class BookingController : Controller
    {
        private IMemoryCache _cache;

        /// <summary>
        /// Constructor of controller.
        /// </summary>
        /// <param name="memoryCache">IMemoryCache object for initializing the memory cache</param>
        public BookingController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }       
        /// <summary>
        /// Displays the booking View and passes the booking list initialized in the constructor as well as the booking in the cache, if one exists
        /// </summary>
        /// <returns>
        /// The booking View displaying the list of bookings
        /// </returns>
        public IActionResult Index()
        {
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {
                return View(createdBookings);
            }
            return View(new List<Booking>());
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
        /// Receives a booking request from the Create Form, checks if the data is valid and inserts it into the cache
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
            if (!ModelState.IsValid) return View();
            
            if (_cache.TryGetValue("CreateBooking", out List<Booking> createdBookings))
            {
                createdBookings.Add(booking);
            } 
            else
            {
                createdBookings = new List<Booking> {booking};
                _cache.Set("CreateBooking", createdBookings);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Serializes and exports a list of all bookings currently in the cache, formated as readable json
        /// </summary>
        /// <returns>List of Booking as .json-file</returns>
        public IActionResult Export()
        {

            
            // Try to read the cache
            if (_cache.TryGetValue("CreateBooking", out List<Booking> bookings))
            {
                var output = Impl.Export.BookingExport(_cache, bookings);
                return output;
            }
            // Return to Index if there are no bookings to export
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Imports and deserializes a json formated list of bookings and adds them to the list in cached booking list
        /// </summary>
        /// <param name="json_files">File in json format. Containing a serialized list of booking objects. Max. size 1MB</param>
        /// <returns>Updated Index View</returns>
        [HttpPost]
        public IActionResult Import(List<IFormFile> json_files)
        {
            // Check if exactly one file was uploaded
            if (json_files.Count == 1)
            {
                Impl.Import.BookingImport(_cache, json_files);
            }
            // Return Index View (will update accordingly)
            return RedirectToAction("Index");
        }
    }
}
