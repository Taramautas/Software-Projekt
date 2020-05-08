using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A3._2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace A3._2.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            List<Booking> booking = new List<Booking>()
            {
                new Booking
                {
                    aktuellerLadestandDesFahrzeuges = 10,
                    benoetigeFahrstrecke = 50,
                    startZeit = new DateTime(2020, 05, 20, 10, 45, 0),
                    endZeit = new DateTime(2020, 05, 20, 11, 45, 0),
                },
                new Booking
                {
                    aktuellerLadestandDesFahrzeuges = 100,
                    benoetigeFahrstrecke = 350,
                    startZeit = new DateTime(2020, 06, 20, 10, 45, 0),
                    endZeit = new DateTime(2020, 06, 20, 11, 45, 0),
                },
                new Booking
                {
                    aktuellerLadestandDesFahrzeuges = 60,
                    benoetigeFahrstrecke = 150,
                    startZeit = new DateTime(2020, 07, 20, 10, 45, 0),
                    endZeit = new DateTime(2020, 07, 20, 11, 45, 0),
                },
                new Booking
                {
                    aktuellerLadestandDesFahrzeuges = 80,
                    benoetigeFahrstrecke = 350,
                    startZeit = new DateTime(2020, 06, 20, 12, 45, 0),
                    endZeit = new DateTime(2020, 06, 20, 13, 45, 0),
                }
            };
            return View(booking);
        }
    }
}
