using System.Collections.Generic;
using Uebungsprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Uebungsprojekt.Controllers
{
    public class BookingController : Controller
    {
        // Erstellen des IMemoryCache
        private IMemoryCache _cache;
        List<Booking> bookingList;


        //Der Konstruktor bekommt den Cache übergeben
        public BookingController(IMemoryCache memoryCache)
        {

            _cache = memoryCache;

            //Es wird überprüft ob das in dem Cache gespeicherte Objekt bookingList vorhanden ist.
            //Falls nicht wird es erstellt und dem Cache hinzugefügt
            if (!_cache.TryGetValue("booking", out bookingList))
            {
                bookingList = new List<Booking>();
                _cache.Set("booking", bookingList);
            }
        }

        //_cache.Get("booking") liefert das im Cache gespeicherte bookingList Objekt
        public IActionResult Index()
        {
            return View(_cache.Get("booking"));
        }

        //Identifiziert eine Methode die HTTP Get unterstützt
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Identifiziert eine Methode die HTTP Post unterstützt
        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            //Überprüfung ob ein Booking-Objekt über die Eingaben erstellt werden kann.
            //Falls nicht wird die Create view nochmal neu angezeigt
            if (ModelState.IsValid)
            {
                //bookingList aus Cache laden
                bookingList = (List<Booking>)_cache.Get("booking");
                //erstellte Booking Objekt hinzufügen
                bookingList.Add(booking);
                //Zurück in den Cache speichern
                _cache.Set("booking", bookingList);
                //Öffnet die Buchungswebsite
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
