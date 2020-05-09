using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Softwareprojekt.Models;

namespace Softwareprojekt.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            List<Booking> bookings = new List<Booking>();
            bookings.Add(new Booking()
                {
                    SoC = 0.75,
                    NeededDistance = 125,
                    Start = new DateTime(2020, 5, 20, 12, 0, 0),
                    End = new DateTime(2020, 5, 20, 14, 0, 0),
            }
            );
            bookings.Add(new Booking()
            {
                SoC = 0.5,
                NeededDistance = 200,
                Start = new DateTime(2020, 5, 24, 9, 0, 0),
                End = new DateTime(2020, 5, 24, 12, 30, 0),
            }
            );
            bookings.Add(new Booking()
            {
                SoC = 0.2,
                NeededDistance = 110,
                Start = new DateTime(2020, 5, 29, 13, 0, 0),
                End = new DateTime(2020, 5, 29, 14, 45, 0),
            }
            );
            bookings.Add(new Booking()
            {
                SoC = 0.8,
                NeededDistance = 50,
                Start = new DateTime(2020, 6, 1, 8, 30, 0),
                End = new DateTime(2020, 6, 1, 12, 0, 0),
            }
            );
            bookings.Add(new Booking()
            {
                SoC = 0.4,
                NeededDistance = 190,
                Start = new DateTime(2020, 6, 3, 9, 15, 0),
                End = new DateTime(2020, 6, 3, 13, 15, 0),
            }
            );

            ViewData["bookings"] = bookings;

            return View();
        }
    }
}