using System;
using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {
        List<Booking> list;
        private IMemoryCache memoryCache;
        
        public BookingController(IMemoryCache cache)
        {
            memoryCache = cache;
            if(!cache.TryGetValue("Buchungen", out list))
            {
                list = new List<Booking>();
                memoryCache.Set("Buchungen", list);
            }
        }
        
        public IActionResult Index()
        {
            return View(memoryCache.Get("Buchungen"));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking b)
        {
            if (ModelState.IsValid)
            {
                list = (List<Booking>) memoryCache.Get("Buchungen");
                list.Add(b);
                memoryCache.Set("Buchungen", list);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult VerifyCharge(int n)
        {
            if (n < 0 || n > 100)
            {
                return Json($"Ladestand muss zwischen 0 und 100 liegen");
            }

            return Json(true);
        }

        public IActionResult VerifyNeededDistance(int n)
        {
            if (n < 1 || n > 1000)
            {
                return Json($"Nötige Distanz muss zwischen 0 und 1000 liegen");
            }

            return Json(true);
        }
    }
}
