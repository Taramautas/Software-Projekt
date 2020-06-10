using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;


namespace Tests.Controller
{
    public class TestBookingController
    {
        private IMemoryCache _cache;
        private BookingController booking;
        private Booking b;

        [SetUp]
        public void Setup()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            booking = new BookingController(_cache); 
        }


        [Test]
        public void TestEditBuchung()
        {
            var result = booking.Index() as ViewResult;
            var model = (List<Booking>)result.Model;
            var counter = model.Count;
            Assert.IsTrue(counter > 0);    // check if there are no bookings in the list, since there is no edit without a booking

            booking.Edit(b);
            Assert.IsTrue(b.Charge >= 0 && b.Charge < 101);     // check if the charge % is between 0 and 100
            Assert.IsTrue(b.Needed_distance > 0 && b.Needed_distance < 1001);       // check if the needed distance is between 1 and 1000
            Assert.IsTrue(b.Start_time < b.End_time);       // check if the end time is later than the start time

            Assert.AreEqual(counter, model.Count); // check if there are no extra bookings
        }
    }
}