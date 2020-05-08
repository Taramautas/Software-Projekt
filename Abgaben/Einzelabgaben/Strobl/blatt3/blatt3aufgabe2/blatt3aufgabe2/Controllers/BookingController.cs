using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blatt3aufgabe2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace blatt3aufgabe2.Controllers
{
    public class BookingController : Controller
    {

        List<Booking> bookings = new List<Booking>
        {
            new Booking { startzeit = new DateTime (2020, 5, 15, 12, 0, 0), endzeit = new DateTime (2020, 5, 15, 13, 30, 0), fahrstrecke = 50, ladestand = 25},
            new Booking { startzeit = new DateTime (2020, 5, 15, 14, 0, 0), endzeit = new DateTime (2020, 5, 15, 16, 30, 0), fahrstrecke = 80, ladestand = 10},
            new Booking { startzeit = new DateTime (2020, 5, 16, 9, 30, 0), endzeit = new DateTime (2020, 5, 16, 12, 15, 0), fahrstrecke = 75, ladestand = 30},
            new Booking { startzeit = new DateTime (2020, 5, 16, 12, 30, 0), endzeit = new DateTime (2020, 5, 16, 15, 30, 0), fahrstrecke = 60, ladestand = 60}
        };


        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["Bookings"] = bookings;
            return View();
        }
    }
}
