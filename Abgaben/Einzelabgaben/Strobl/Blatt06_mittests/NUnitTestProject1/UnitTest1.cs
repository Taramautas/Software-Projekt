using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using NUnit.Framework;

namespace NUnitTestProject1
{
    public class Tests
    {
        private BookingController bc;
        private IMemoryCache cache;
        [SetUp]
        public void Setup()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            bc = new BookingController(cache);
        }

        [Test]
        public void TestDeleteBooking()
        {
            //create new Bookinglist, add new Booking and check if there really is one booking in bookinglist
            var result = bc.Index() as ViewResult;
            var model = (List<Booking>)result.Model;
            Assert.AreEqual(0, model.Count);

            Booking testBooking = new Booking
            {
                Start_time = DateTime.Today.AddDays(1).Add(new TimeSpan(14, 00, 00)), //makes sure booking is in the future
                End_time = DateTime.Today.AddDays(1).Add(new TimeSpan(18, 00, 00)),
            };
            bc.Create(testBooking);

            result = bc.Index() as ViewResult;
            model = (List<Booking>)result.Model;
            Assert.AreEqual(1, model.Count);

            //delete the Booking and check if Bookinglist is empty
            bc.deleteBooking(testBooking);
            result = bc.Index() as ViewResult;
            model = (List<Booking>)result.Model;
            Assert.AreEqual(0, model.Count);

            //Test if timeslot is free again after deleting the booking by creating a Booking on the same time slot
            Booking testBooking2 = testBooking;
            bc.Create(testBooking2);

            result = bc.Index() as ViewResult;
            model = (List<Booking>)result.Model;
            Assert.AreEqual(1, model.Count);
        }
    }
}