using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;

namespace UnitTest.Controller
{
   

    /// <summary>
    /// Testing BookingController
    /// </summary>
    public class TestBookingController
    {

        private IMemoryCache cache;
        private BookingController book;

        /// <summary>
        /// Initalizing the SetupEnvironment
        /// </summary>
        [SetUp]
        public void setup()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            book = new BookingController(cache);
        }

        [Test]
        public void TestAddBookings()
        {
            var result = book.Index() as ViewResult;
            List<Booking> BookingList = (List<Booking>) result.Model;
            // Check if initial List is 0
            Assert.AreEqual(0, BookingList.Count);

            // Create new Booking and check if it was saved
            BookingList.Add(new Booking
            {
                Charge = 4
            });

            result = book.Index() as ViewResult;
            BookingList = (List<Booking>)result.Model;
            // Count should be 1
            Assert.AreEqual(1, BookingList.Count);

        }
    }
}