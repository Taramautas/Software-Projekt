using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BookingController : Controller
    {
        List<Booking> list = new List<Booking>();
        public BookingController ()
        {
            list.Add(
                new Booking()
                {
                    SoC = 25,
                    reqDistance = 50,
                    start = new DateTime(2020, 5, 12, 12, 30, 00),
                    end = new DateTime(2020, 5, 12, 14, 00, 00),
                });
            list.Add(
                new Booking()
                {
                    SoC = 35,
                    reqDistance = 56,
                    start = new DateTime(2020, 5, 12, 13, 00, 00),
                    end = new DateTime(2020, 5, 12, 15, 30, 00),
                });
            list.Add(
                new Booking()
                {
                    SoC = 33,
                    reqDistance = 140,
                    start = new DateTime(2020, 5, 16, 16, 30, 00),
                    end = new DateTime(2020, 5, 17, 8, 30, 00),
                });

        }

        public IActionResult Index ()
        {
            ViewData["BookingTable"] = list;
            return View();
        }
    }
}
