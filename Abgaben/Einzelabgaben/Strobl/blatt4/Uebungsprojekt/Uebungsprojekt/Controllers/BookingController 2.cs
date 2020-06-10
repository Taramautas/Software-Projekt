using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {

        List<Booking> bookingList;
        private IMemoryCache _cache;


        public BookingController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            // Look for cache key
            if (!_cache.TryGetValue("booking", out bookingList))
            {
                // Key not in cache, so create new bookingList
                bookingList = new List<Booking>();
                // Save data in cache
                _cache.Set("booking", bookingList);
            }
        }
        //Gebe View mit den im Cache gespeicherten Buchungen zurück
        public IActionResult Index()
        {
            return View(_cache.Get("booking"));
        }

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            //kontrollieren ob valide Daten eingegeben wurden
            if (ModelState.IsValid)
            {
                //Buchung aus Cache herausziehen
                bookingList = (List<Booking>)_cache.Get("booking");
                //Buchung zur bookingList hinzufügen
                bookingList.Add(booking);
                //Buchung zurück zur Cache hinzufügen
                _cache.Set("booking", bookingList);
                //Indexseite aufrufen um Buchung zur Tabelle hinzuzufügen
                return RedirectToAction(nameof(Index));
            }
            //falls keine valide Daten eingegeben wurden, leere View zurückgeben
            return View();
        }
    }
}
