using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Uebungsprojekt.Controllers
{
    /// <summary>
    /// Controller for Booking model
    /// </summary>
    public class BookingController : Controller
    {
        private IMemoryCache _cache;

        private List<Booking> bookingList = new List<Booking>();

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
                bookingList.AddRange(createdBookings);
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
        /// Exports the list of bookings currently in cache as json file
        /// </summary>
        /// <returns></returns>
        public IActionResult Export()
        {
            bookingList = (List<Booking>)_cache.Get("booking");
            byte[] vs = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bookingList));
            var export = new FileContentResult(vs, "application/octet-stream");
            return export;
        }

        /// <summary>
        /// Deserializes file if type is json and size is less than 1MB
        /// </summary>
        /// <param name="files"> The file to be imported </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Import(List<IFormFile> files)
        {
            bookingList = (List<Booking>)_cache.Get("booking");
            bookingList.Clear();
            if (ModelState.IsValid)
            {
                foreach (var file in files)
                {
                    if(file.ContentType == "application/json" && file.Length <= 1048576)
                    {
                        StreamReader reader = new StreamReader(file.OpenReadStream());
                        JsonSerializer serializer = new JsonSerializer();
                        string s = reader.ReadToEnd();
                        bookingList = JsonConvert.DeserializeObject<List<Booking>>(s);
                        _cache.Set("booking", bookingList);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
