using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BookingController : Controller
    {
        List<Booking> bookings = new List<Booking>();

        public BookingController()
        {
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
            bookings.Add(new Booking(20, 200, new DateTime(2020, 10, 10, 10, 10, 00), new DateTime(2020, 10, 10, 20, 0, 0)));
        }

        
        public IActionResult Index()
        {
            return View(bookings);
        }
    }
}