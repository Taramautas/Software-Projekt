using System;
using System.Collections.Generic;
using System.Collections;

using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {
        private IMemoryCache _cache;
        List<Booking> bookingList;

        public BookingController(IMemoryCache memoryCache)
        {

            _cache = memoryCache;

            if (!_cache.TryGetValue("booking", out bookingList))
            {
                bookingList = new List<Booking>();
                _cache.Set("booking", bookingList);
            }
        }

        public IActionResult Index()
        {
            return View(_cache.Get("booking"));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                bookingList = (List<Booking>)_cache.Get("booking");
                bookingList.Add(booking);
                _cache.Set("booking", bookingList);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
