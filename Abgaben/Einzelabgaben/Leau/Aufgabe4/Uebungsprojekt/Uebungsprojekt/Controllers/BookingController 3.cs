using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {
        private IMemoryCache _cache;                                    // new cache

        List<Booking> bookingList;                                      // booking list

        public BookingController(IMemoryCache memoryCache)              // constructor
        {
            _cache = memoryCache;
            if(!_cache.TryGetValue("booking", out bookingList))         // if cache is empty, make first item of the list
            {
                bookingList = new List<Booking>();
                _cache.Set("booking", bookingList);                     // set data in cache
            }
        }    
        public IActionResult Index()
        {
            return View(bookingList);
        }

        [HttpGet]                                                       // HttpGet is used to request data from a specified resource.
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]                                                      // HttpPost is used to send data to a server to create/update a resource.
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)                                     // check if valid data
            {
                bookingList = (List<Booking>)_cache.Get("booking");     // get data from booking
                bookingList.Add(booking);                               // add data to the list
                _cache.Set("booking", bookingList);                     // set data in cache
                return RedirectToAction(nameof(Index));                 // update website
            }
            return View();
        }
    }
}
