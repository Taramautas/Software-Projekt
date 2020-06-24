using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using System.Linq;

namespace UnitTest.Controllers
{
    /// <summary>Test Class for Booking Controller</summary>
    public class TestBookingController
    {
        private BookingController bc;
        private IMemoryCache cache;

        /// <summary>
        /// Setup function executed once before every test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            bc = new BookingController(cache);
        }

        /// <summary>
        /// Tests if bookings created in Create View are added to the Index table
        /// </summary>
        [Test]
        public void TestCreateAddsBookingToIndex()
        {
            // Check if the initial amount of bookings is 0
            var result = bc.Index() as ViewResult;
            var model = (List<Booking>)result.Model;
            Assert.AreEqual(0, model.Count);

            // Create new booking and check if list of bookings objects returned to Index View has saved that booking
            bc.Create(new Booking 
            { 
                start_time = new DateTime(2020, 5, 28),
            });

            result = bc.Index() as ViewResult;
            model = (List<Booking>)result.Model;
            Assert.AreEqual(1, model.Count);

            // Check it twice
            bc.Create(new Booking
            {
                start_time = new DateTime(2020, 5, 28),
            });

            result = bc.Index() as ViewResult;
            model = (List<Booking>)result.Model;
            Assert.AreEqual(2, model.Count);

            // Validate all saved booking objects match the data used for creating them
            foreach (Booking booking in model)
            {
                Assert.AreEqual(new DateTime(2020, 5, 28), booking.start_time);
            }
        }

        /// <summary>
        /// Test if an imported json bookings are saved correclty
        /// </summary>
        [Test]
        public void TestImportAddsBookingsToIndex()
        {
            Assert.Pass();
        }

        /// <summary>
        /// Test if an all bookings are correclty exported
        /// </summary>
        [Test]
        public void TestExportGetsAllBookingsFromIndex()
        {
            Assert.Pass();
        }
    }
}