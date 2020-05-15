using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {
        private IMemoryCache _cache;

        List<Booking> bookingList = new List<Booking>();


        public BookingController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            bookingList.Add(
                new Booking()
                {
                    charge = 20,
                    needed_distance = 46,
                    start_time = new DateTime(2020, 12 , 15, 4, 30, 45),
                    end_time = new DateTime(2020, 12, 15, 5, 30, 45),
                });

            bookingList.Add(
                new Booking()
                {
                    charge = 40,
                    needed_distance = 78,
                    start_time = new DateTime(2020, 12, 15, 2, 0, 0),
                    end_time = new DateTime(2020, 12, 15, 3, 0, 0),
                });

            bookingList.Add(
                new Booking()
                {
                    charge = 100,
                    needed_distance = 94,
                    start_time = new DateTime(2020, 12, 14, 12, 0, 0),
                    end_time = new DateTime(2020, 12, 15, 0, 0, 0),
                });

            bookingList.Add(
                new Booking()
                {
                    charge = 45,
                    needed_distance = 100,
                    start_time = new DateTime(2020, 12, 15, 8, 30, 0),
                    end_time = new DateTime(2020, 12, 15, 16, 30, 0),
                });

        }       
        public IActionResult Index()
        {
            Booking created_booking;
            if (_cache.TryGetValue("CreateBooking", out created_booking))
            {
                bookingList.Add(created_booking);
            }
            return View(bookingList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Booking());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            // Server side validation
            if (ModelState.IsValid)
            {
                _cache.Set("CreateBooking", booking);
                return RedirectToAction("Index");

            }
            return View();
        }
    }
}
