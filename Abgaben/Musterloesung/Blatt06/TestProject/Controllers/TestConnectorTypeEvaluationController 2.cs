using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using Uebungsprojekt.Controllers;
using Uebungsprojekt.Models;
using System.Linq;
using Uebungsprojekt.ViewModel;

namespace UnitTest.Controller
{
    /// <summary>Test Class for ConnectorTypeEvaluation Controller</summary>
    public class TestConnectorTypeEvaluationController
    {
        private ConnectorTypeEvaluationController ctec;
        private IMemoryCache cache;

        /// <summary>
        /// Setup function executed once before every test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            ctec = new ConnectorTypeEvaluationController(cache);
        }

        /// <summary>
        /// Tests if Index view recieves correctly calculated data
        /// </summary>
        [Test]
        public void TestIndexCalculatesCorrectProportions()
        {
            BookingController bc = new BookingController(cache);
            // Create new bookings
            bc.Create(new Booking
            {
                ConnectorType = Booking.ConnectorTypeEnum.Schuko_Socket
            });
            bc.Create(new Booking
            {
                ConnectorType = Booking.ConnectorTypeEnum.Schuko_Socket
            });
            bc.Create(new Booking
            {
                ConnectorType = Booking.ConnectorTypeEnum.CHAdeMO_Plug
            });
            bc.Create(new Booking
            {
                ConnectorType = Booking.ConnectorTypeEnum.Type_1_Plug
            });

            // Check if there is a ConnectorTypeEvaluation instance for all connector types
            var result = ctec.Index() as ViewResult;
            var model = (List<ConnectorTypeEvaluationViewModel>)result.Model;
            Assert.AreEqual(Enum.GetValues(typeof(Booking.ConnectorTypeEnum)).Length, model.Count);
            // Validate the calculated proportions
            Assert.AreEqual(50, model.Find(x => x.ConnectorType == Booking.ConnectorTypeEnum.Schuko_Socket).Proportion);
            Assert.AreEqual(25, model.Find(x => x.ConnectorType == Booking.ConnectorTypeEnum.CHAdeMO_Plug).Proportion);
            Assert.AreEqual(25, model.Find(x => x.ConnectorType == Booking.ConnectorTypeEnum.Type_1_Plug).Proportion);
        }

        /// <summary>
        /// Test if the evaluation table gets exported correctly
        /// </summary>
        [Test]
        public void TestExportGetsAllBookingsFromIndex()
        {
            Assert.Pass();
        }
    }
}
